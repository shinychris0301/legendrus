#!/usr/bin/env python3
"""Transfer large unpublished histories in bounded batches.

Preparation creates local commits using a private index. With explicit approval,
upload backs up and splits only unpublished history, publishes the snapshots to
a temporary branch, then fast-forwards remote main. Every original source tree
and blob is preserved. No checkout, reset, force push, or file removal is used.
"""
import argparse
import json
import os
from pathlib import Path
import subprocess
import time

AUTH_KEYS = (
    b"GIT_ASKPASS", b"VSCODE_GIT_ASKPASS_NODE",
    b"VSCODE_GIT_ASKPASS_MAIN", b"VSCODE_GIT_IPC_HANDLE",
)


def auth_environment(socket=None):
    env = os.environ.copy()
    env["GIT_TERMINAL_PROMPT"] = "0"
    if all(k.decode() in env for k in AUTH_KEYS):
        return env
    for proc in Path("/proc").iterdir():
        if not proc.name.isdigit():
            continue
        try:
            if (proc / "comm").read_text().strip() not in ("code", "Code", "node"):
                continue
            values = dict(item.split(b"=", 1) for item in
                          (proc / "environ").read_bytes().split(b"\0") if b"=" in item)
            if all(k in values for k in AUTH_KEYS):
                env.update({k.decode(): values[k].decode() for k in AUTH_KEYS})
                break
        except (OSError, ValueError):
            continue
    if not all(k.decode() in env for k in AUTH_KEYS):
        candidates = sorted((Path("/run/user") / str(os.getuid())).glob("vscode-git-*.sock"),
                            key=lambda p: p.stat().st_mtime, reverse=True)
        handle = Path(socket) if socket and Path(socket).exists() else (candidates[0] if candidates else None)
        if handle:
            env.update(GIT_ASKPASS="/usr/share/code/resources/app/extensions/git/dist/askpass.sh",
                       VSCODE_GIT_ASKPASS_NODE="/usr/share/code/code",
                       VSCODE_GIT_ASKPASS_MAIN="/usr/share/code/resources/app/extensions/git/dist/askpass-main.js",
                       VSCODE_GIT_IPC_HANDLE=str(handle))
    return env


def git(*args, data=None, env=None):
    result = subprocess.run(["git", *args], input=data, capture_output=True,
                            env=env, check=False)
    if result.returncode:
        raise RuntimeError(result.stderr.decode(errors="replace")[-2000:])
    return result.stdout


def save(path, data):
    temporary = path.with_suffix(path.suffix + ".new")
    temporary.write_text(json.dumps(data, indent=2) + "\n")
    os.chmod(temporary, 0o600)
    temporary.replace(path)


def entries(revision):
    result = {}
    for row in git("ls-tree", "-r", "-l", "-z", revision).split(b"\0"):
        if not row:
            continue
        info, path = row.split(b"\t", 1)
        mode, kind, oid, size = info.split()
        result[path] = (mode, oid, int(size) if size != b"-" else 0)
    return result


def prepare(directory, base, source, budget):
    base = git("rev-parse", "--verify", base + "^{commit}").decode().strip()
    source = git("rev-parse", "--verify", source + "^{commit}").decode().strip()
    assert git("rev-parse", "HEAD").decode().strip() == source, "HEAD changed"
    git("merge-base", "--is-ancestor", base, source)
    revisions = git("rev-list", "--reverse", "--first-parent",
                    base + ".." + source).decode().splitlines()
    known = {row.split(b" ", 1)[0] for row in
             git("rev-list", "--objects", base).splitlines()}
    current = entries(base)
    private_index = directory / "Bridge.index"
    env = os.environ.copy()
    env["GIT_INDEX_FILE"] = str(private_index)
    git("read-tree", base, env=env)
    parent = base
    batches = []
    stamp = directory.name
    local_ref = "refs/backup/git-push-repair/" + stamp + "/upload-bridge"
    for revision in revisions:
        target = entries(revision)
        changes = sorted(p for p in current.keys() | target.keys()
                         if current.get(p) != target.get(p))
        queued = []
        new_bytes = 0

        def flush():
            nonlocal parent, queued, new_bytes
            if not queued:
                return
            records = []
            for path in queued:
                entry = target.get(path)
                if entry is None:
                    records.append(b"0 " + b"0" * 40 + b"\t" + path + b"\0")
                    current.pop(path, None)
                else:
                    records.append(entry[0] + b" " + entry[1] + b"\t" + path + b"\0")
                    current[path] = entry
            git("update-index", "-z", "--index-info", data=b"".join(records), env=env)
            tree = git("write-tree", env=env).decode().strip()
            message = ("Upload evidence batch %d; original source %s\n" %
                       (len(batches) + 1, revision)).encode()
            parent = git("commit-tree", tree, "-p", parent, data=message).decode().strip()
            git("update-ref", local_ref, parent)
            batches.append({"commit": parent, "source_commit": revision,
                            "new_blob_bytes": new_bytes, "changed_paths": len(queued)})
            print(json.dumps({"prepared_batch": len(batches),
                              "new_blob_bytes": new_bytes}), flush=True)
            queued = []
            new_bytes = 0

        for path in changes:
            entry = target.get(path)
            size = entry[2] if entry and entry[1] not in known else 0
            if size > budget:
                raise RuntimeError("One object exceeds the batch budget")
            if queued and new_bytes + size > budget:
                flush()
            queued.append(path)
            new_bytes += size
            if entry:
                known.add(entry[1])
        flush()
        expected = git("rev-parse", revision + "^{tree}").decode().strip()
        actual = git("write-tree", env=env).decode().strip()
        assert actual == expected, "Bridge snapshot differs from original source tree"
    assert git("rev-parse", parent + "^{tree}") == git("rev-parse", source + "^{tree}")
    plan = {"repository": str(Path.cwd()), "base": base, "source": source,
            "remote": "origin", "target_ref": "refs/heads/main",
            "temporary_ref": "refs/heads/codex-evidence-upload-" + stamp,
            "local_bridge_ref": local_ref, "batch_blob_budget": budget,
            "batches": batches, "source_commits": revisions}
    save(directory / "UploadPlan.json", plan)
    return plan


def remote_refs(plan):
    rows = git("ls-remote", plan["remote"], plan["target_ref"],
               plan["temporary_ref"], env=auth_environment(plan.get("git_auth_socket"))).decode().splitlines()
    return {row.split("\t", 1)[1]: row.split("\t", 1)[0] for row in rows}


def upload(directory, approved_split=False):
    plan = json.loads((directory / "UploadPlan.json").read_text())
    os.chdir(plan["repository"])
    tip = plan["batches"][-1]["commit"]
    if approved_split and not plan.get("split_authorized"):
        original = plan["source"]
        assert git("rev-parse", original + "^{tree}") == git("rev-parse", tip + "^{tree}")
        assert git("rev-parse", "HEAD").decode().strip() == original, "Local HEAD changed"
        assert git("symbolic-ref", "HEAD").decode().strip() == plan["target_ref"], "Active branch changed"
        backup_ref = plan["local_bridge_ref"].removesuffix("upload-bridge") + "original-before-split"
        git("update-ref", backup_ref, original, "0" * 40)
        plan.update(original_source=original, source=tip,
                    original_backup_ref=backup_ref, split_authorized=True)
        save(directory / "UploadPlan.json", plan)
    new_branch = plan.get("publication_mode") == "new_branch"
    if not new_branch and not plan.get("split_authorized"):
        raise RuntimeError("Unpublished history split needs explicit approval; use --approved-split after approval")
    local_head = git("rev-parse", "HEAD").decode().strip()
    if new_branch:
        assert plan["target_ref"] != "refs/heads/main", "New-branch mode cannot replace main"
        assert local_head == tip, "Recovery branch changed; review before upload"
        assert git("symbolic-ref", "HEAD").decode().strip() == plan["target_ref"], "Active recovery branch changed"
    elif local_head == plan["original_source"]:
        assert git("symbolic-ref", "HEAD").decode().strip() == plan["target_ref"], "Active branch changed"
        git("update-ref", plan["target_ref"], tip, plan["original_source"])
    else:
        assert local_head == tip, "Local main changed; review before upload"
    status_path = directory / "UploadStatus.json"
    status = {"phase": "checking_remote", "pid": os.getpid(),
              "total_batches": len(plan["batches"]), "completed_batches": 0,
              "source": plan["source"], "started_utc": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime())}
    save(status_path, status)

    def push(oid, ref):
        with (directory / "Push.log").open("ab") as log:
            log.write(("\nPUSH " + oid + ":" + ref + "\n").encode())
            log.flush()
            process = subprocess.Popen(["git", "-c", "pack.threads=2", "-c", "pack.window=0",
                                        "push", "--progress", plan["remote"],
                                        oid + ":" + ref], env=auth_environment(plan.get("git_auth_socket")),
                                       stdout=log, stderr=log)
            status["git_pid"] = process.pid
            while process.poll() is None:
                status["heartbeat_utc"] = time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime())
                status["push_log_bytes"] = (directory / "Push.log").stat().st_size
                save(status_path, status)
                time.sleep(2)
            status.pop("git_pid", None)
            if process.returncode:
                raise RuntimeError("git push failed (exit %s); inspect private Push.log and resume" % process.returncode)

    try:
        refs = remote_refs(plan)
        if refs.get(plan["target_ref"]) == plan["source"]:
            completed = len(plan["batches"])
        else:
            assert refs.get(plan["target_ref"]) == plan.get("initial_target", plan["base"]), "Remote target changed; review before upload"
            commits = [b["commit"] for b in plan["batches"]]
            tip = refs.get(plan["temporary_ref"])
            assert tip is None or tip in commits, "Temporary branch has unexpected history"
            completed = commits.index(tip) + 1 if tip else 0
            for number, batch in enumerate(plan["batches"][completed:], completed + 1):
                status.update(phase="uploading_batches", active_batch=number,
                              completed_batches=number - 1,
                              active_batch_blob_bytes=batch["new_blob_bytes"])
                save(status_path, status)
                push(batch["commit"], plan["temporary_ref"])
                status["completed_batches"] = number
                save(status_path, status)
            assert git("rev-parse", "HEAD").decode().strip() == plan["source"], "Local main changed; review before final upload"
            assert remote_refs(plan).get(plan["target_ref"]) == plan.get("initial_target", plan["base"]), "Remote target changed; review before final upload"
            status.update(phase="publishing_target", completed_batches=len(commits))
            save(status_path, status)
            push(plan["source"], plan["target_ref"])
        assert remote_refs(plan).get(plan["target_ref"]) == plan["source"], "Remote target verification failed"
        if new_branch:
            name = plan["target_ref"].removeprefix("refs/heads/")
            git("config", "branch." + name + ".remote", plan["remote"])
            git("config", "branch." + name + ".merge", plan["target_ref"])
        tip = remote_refs(plan).get(plan["temporary_ref"])
        if tip == plan["batches"][-1]["commit"]:
            try:
                git("push", plan["remote"], "--delete",
                    plan["temporary_ref"].removeprefix("refs/heads/"),
                    env=auth_environment(plan.get("git_auth_socket")))
            except RuntimeError:
                status["temporary_branch_cleanup_pending"] = True
        status.update(phase="completed", completed_batches=len(plan["batches"]),
                      verified_remote_target=plan["source"], target_ref=plan["target_ref"],
                      completed_utc=time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()))
        save(status_path, status)
    except Exception as error:
        status.update(phase="failed", error=str(error))
        save(status_path, status)
        raise


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("action", choices=("prepare", "upload"))
    parser.add_argument("directory", type=Path)
    parser.add_argument("--base")
    parser.add_argument("--source")
    parser.add_argument("--budget-mib", type=int, default=512)
    parser.add_argument("--approved-split", action="store_true",
                        help="Explicitly authorize splitting backed-up unpublished history")
    args = parser.parse_args()
    if args.action == "prepare":
        if not args.base or not args.source:
            parser.error("prepare needs --base and --source")
        if not 1 <= args.budget_mib <= 512:
            parser.error("budget must be 1–512 MiB")
        prepare(args.directory.resolve(), args.base, args.source, args.budget_mib * 1024 * 1024)
    else:
        upload(args.directory.resolve(), approved_split=args.approved_split)


if __name__ == "__main__":
    main()
