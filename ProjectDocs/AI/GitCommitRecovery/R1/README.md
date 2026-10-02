# GitHub upload recovery — 2026-10-02

VS Code local commits succeeded. Sync/Push failed with HTTP500/disconnect while attempting to send four unpublished commits containing27.5GB of new file contents, mostly QA imagery. An oversized aggregate push is the likely cause. GitHub documents a2GiB per-push limit; the original failed wire pack was not captured. [GitHub documentation](https://docs.github.com/en/get-started/using-git/troubleshooting-the-2-gb-push-limit).

The user chose to upload **all evidence through smaller batches**. No exclusions were added and no files were removed. Existing commits and staged state were backed up by local refs and a private index copy before the pending491-file preset change was committed as674a8741.

`UploadInBatches.py` uses a private index to construct a temporary chain of snapshots, with at most512MiB of new blob contents per batch. Each original commit boundary must reproduce its exact source tree. These snapshots upload to one temporary remote branch; main advances to the original source commit only after all snapshots arrive. Original commit IDs, authors, messages and history remain intact. Upload resumes from the last acknowledged batch. Concurrent changes to local or remote main abort the final update for review. The owned temporary branch is removed after main is verified. No force push, checkout, reset, credential export or deletion of evidence occurs.

Private job/backup directory: `/home/k2/.local/state/legendrus-git-repair/20261002T181726Z/`. `UploadPlan.json` holds the pinned source and batches; `UploadStatus.json` records progress; `Push.log` holds native Git output. To resume after a failed attempt from a VS Code terminal with GitHub sign-in:

```sh
python3 ProjectDocs/AI/GitCommitRecovery/R1/UploadInBatches.py upload /home/k2/.local/state/legendrus-git-repair/20261002T181726Z
```

Preparation is local only. The `upload` action explicitly publishes. Temporary local bridge refs remain as recovery evidence and are not part of ordinary main pushes. GitHub access was verified through VS Code's existing authentication transport; authentication secrets are never read or saved by the repair tools.

See `Diagnosis.json` and `FixtureResults.json`. The disposable local-remote fixture exercises multi-batch transfer, resumption, duplicate blob reuse, deletion/modification, executable mode, symlink, Cyrillic path, final original history, private-index isolation and temporary branch cleanup. No Unity runtime change or Unity test/build was required for this Git repair. Production Assets, metadata, settings and the approved Splash remain unchanged.
