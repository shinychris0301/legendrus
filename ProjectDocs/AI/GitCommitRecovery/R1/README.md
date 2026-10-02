# GitHub upload recovery — 2026-10-02

Local commits succeed. VS Code Sync/Push failed with HTTP500 and a disconnect while sending27.5GB of new file contents, mostly QA imagery. An oversized aggregate push is the likely cause; the original failed wire pack was not captured. GitHub limits individual pushes to2GiB. [GitHub documentation](https://docs.github.com/en/get-started/using-git/troubleshooting-the-2-gb-push-limit).

The user chose **upload all evidence through smaller batches** and repeated the request to resolve the failure. We use a separate recovery branch, `repair/full-evidence-batches-20261002T181726Z`, preserving original local main (ccedef75), its six unpublished commits, backup refs and remote main (13f8316e). The earlier request to replace main's unpublished history is superseded by this approach; main is preserved.

All original source checkpoints, trees/blobs/file versions are retained in55 bounded snapshots. A small final recovery/report commit is appended. The active workspace uses the recovery branch with identical production inputs; local main retains its original history. No files or evidence were removed, exclusions added, source/art/GUID/meta/settings/APK changed, or secrets exported.

`UploadInBatches.py` uses a private index, a temporary remote upload branch, acknowledged-batch resumption and a final publication to the new recovery branch. Every batch has at most512MiB of new blob contents; the maximum entire new object set is537,037,894bytes. The first actual wire pack is505,323,915bytes with Git exit0. Compression work is bounded. After final remote verification, the tool configures the recovery branch's upstream and removes the owned temporary upload branch. Ordinary commits and Sync/Push then use this recovery branch. The original main is not repointed, reset, rewritten or force-pushed.

The initial temporary-branch/original-main-history strategy was rejected before any live upload: exact object-set verification proved all original objects present in the bridge, but native Git still packed the oversized disconnected original history. The alternative main rewrite was prepared but not applied. Publishing the same verified snapshots as a separate branch avoids changing original main.

Private job/backup directory: `/home/k2/.local/state/legendrus-git-repair/20261002T181726Z/`. `UploadPlan.json` pins the source, target and batches. `UploadStatus.json` records live phase/counts; `Push.log` holds native Git output. After a failed attempt, resume from an authenticated VS Code terminal:

```sh
python3 ProjectDocs/AI/GitCommitRecovery/R1/UploadInBatches.py upload /home/k2/.local/state/legendrus-git-repair/20261002T181726Z
```

Keep VS Code signed in during transfer. The background worker retains checkpoints and can resume if the authentication transport closes. The large transfer must finish before a normal Sync can upload the complete branch in a small pack. Concurrent changes to the target are checked before publication. The worker uploads a pinned snapshot; later commits require their own push.

See `Diagnosis.json`, `CandidateReview.json` and `NewBranchFixtureResults.json`. The corrected new-branch fixture proves both local/remote main unchanged, exact original final tree published, first-batch resumption, original index/untracked preservation, upstream configuration, temporary-branch cleanup and idempotent completed resume. Earlier fixture/probe results are retained. No Unity test/build/device or hosting action was required for this Git repair.
