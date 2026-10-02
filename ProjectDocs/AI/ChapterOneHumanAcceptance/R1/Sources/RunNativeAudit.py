from pathlib import Path
import argparse
import json
import os
import resource
import subprocess
import time

BASE = Path(__file__).resolve().parent.parent
ROOT = BASE.parents[3]
OUT = BASE / "QA/NativeAudit"
HELPER = ROOT / "Assets/LegendsOfRus/Scripts/Editor/ChapterOneAcceptanceAudit.cs"


def run():
    global OUT
    parser = argparse.ArgumentParser()
    parser.add_argument('--output', type=Path, default=OUT)
    OUT = parser.parse_args().output.resolve()
    assert OUT.is_relative_to(BASE / 'QA'), 'Audit output belongs under this task QA directory'
    assert not any(name.strip() in ("Unity", "Unity Main Thre") for name in
                   subprocess.check_output(["ps", "-eo", "comm="], text=True).splitlines()), "Close Unity first"
    assert not HELPER.exists() and not Path(str(HELPER) + ".meta").exists()
    OUT.mkdir(parents=True, exist_ok=False)
    config = Path("/tmp/lor-chapter-one-acceptance-r1/config")
    licensing = config / "unity3d/Unity"
    licensing.mkdir(parents=True, exist_ok=True)
    link = licensing / "licenses"
    if not link.exists():
        link.symlink_to("/home/k2/.config/unity3d/Unity/licenses", target_is_directory=True)
    env = {key: value for key, value in os.environ.items() if not key.startswith("LOR_")}
    env.update(XDG_CONFIG_HOME=str(config), LOR_LOADING_TEST_CONFIG_ROOT=str(config),
               LOR_ACCEPTANCE_AUDIT_OUTPUT=str(OUT / "InstalledContent.json"))
    command = ["/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity", "-batchmode",
               "-projectPath", str(ROOT), "-executeMethod", "LegendsOfRus.Editor.ChapterOneAcceptanceAudit.Run",
               "-logFile", str(OUT / "unity.log"), "-force-vulkan"]

    def limits():
        resource.setrlimit(resource.RLIMIT_NOFILE, (4096, resource.getrlimit(resource.RLIMIT_NOFILE)[1]))

    start = time.time()
    removed = []
    try:
        HELPER.write_bytes((BASE / "Sources/ChapterOneAcceptanceAudit.cs").read_bytes())
        with (OUT / "launcher.log").open("w") as stream:
            code = subprocess.call(command, env=env, stdout=stream, stderr=subprocess.STDOUT, preexec_fn=limits)
    finally:
        for path in (HELPER, Path(str(HELPER) + ".meta")):
            if path.exists():
                path.unlink()
                removed.append(str(path.relative_to(ROOT)))
    report = {"exitCode": code, "durationSeconds": time.time() - start,
              "command": command, "isolatedConfig": str(config), "temporaryFilesRemoved": removed,
              "processOnlyVulkan": True, "scope": "Editor content/fixture audit; no gameplay, test-runner suite or human play"}
    (OUT / "summary.json").write_text(json.dumps(report, indent=2) + "\n")
    print(json.dumps(report, indent=2))
    assert code == 0 and (OUT / "InstalledContent.json").exists()


if __name__ == "__main__":
    run()
