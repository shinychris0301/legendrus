# R43 — Full-project regression audit

**Edit Mode: 1,917 passed, 0 failed, 0 skipped. Runtime remains open: 857 passed and 40 failed in the latest evidence per distinct test across runs. This is not a single clean post-fix Play Mode sweep.**

Unity 6000.5.9f1. Inventoried all 40 project test assemblies. Used isolated storage, the previously approved process-only Vulkan backend and descriptor limit 4096. No Android build/device test or external-service activation.

| Run | Passed | Failed | Skipped |
|---|---:|---:|---:|
| Initial Edit Mode filter | 1,890 | 11 | 0 |
| Expanded Edit Mode, including older balance namespace | 1,913 | 2 | 0 |
| Final complete project Edit Mode | **1,917** | **0** | **0** |
| Initial combined Play Mode | 617 | 277 | 3 |
| General runtime suite in a fresh process | 411 | 41 | 3 |
| Corrected balance isolation, analytics, booster UI and Victory captions | **25** | **0** | **0** |
| Separate-process language write/read | **2** | **0** | **0** |
| Separate-process save write/read | **2** | **0** | **0** |
| Journey checks with required capture configuration | **4** | **0** | **0** |

Counts overlap; do not add rows. The three initially skipped test cases were subsequently exercised in dedicated processes. Full XML, commands and summaries are retained in [run inventory](QA/RunInventory.json); [latest runtime status](QA/LatestRuntimeStatus.json) identifies every outstanding case and its last run.

## Verified corrections

- Winning-move margin no longer grows with the number of wins. Five moves remaining out of twenty now yields 0.25 for one, three and six wins. Low-margin detection passes again.
- The analytics schema accepts all existing screen mappings, including the 15 appended routes. The offline-adapter test passes; invalid routes remain rejected. Production activation and consent safeguards are unchanged.
- The balance-test fixture preserves the shared runtime root and restores its original observation buffer. Previously it destroyed that root, causing widespread later setup failures. The combined focused run verifies service preservation and subsequent native UI tests.
- Ten obsolete UI assertions now reflect current native controls, expanded page delivery and approved framed Inventory booster art. The replacement-save fixture now uses the identical replacement envelope written by the real journal. A separate mismatch-intent rejection test preserves the stricter recovery guard.

## Remaining failures

Forty runtime cases remain failed, not waived: 23 board/input/motion tests use mismatched QA-board dimensions; five assert the old VFX path; eleven reference superseded campaign/UI/artwork expectations, including three historical candidate cases; one compares the independently advancing energy timestamp. These are source-based diagnoses requiring corrected assertions and passing reruns. See [exact triage](QA/RuntimeFailureTriage.json) and [follow-up](FollowUp.md).

The active-campaign witness tests passed for three attempt seeds on every round 1–200. All 600 [witness records](QA/PlayMode01/ActiveWitnesses) were retained. These are automated legal completion replays, not human difficulty evidence. Win/return, fail/retry, lifecycle interruption, language restart and save restart checks passed; the save reader confirmed no duplicate replay reward.

## Scope and limits

Nine existing project files changed: two production files and seven test files; no project files added/deleted; 7,698 baseline files unchanged. [Scope](QA/Scope.json), [reviewable patch](Sources/Changes.patch), and original test files are retained. Campaign content, approved UI/art/prefabs, settings/packages, matching rules and effects are unchanged. Generated test scenes/linker changes were cleaned up. No commit.

The 639 baseline runtime screenshots remain linked from `QA/RuntimeIsolated01/Captures` to `/tmp/lor-regression-r43/RuntimeIsolated01/Captures` to conserve workspace storage; that baseline evidence is temporary and not portable. Final focused and journey captures remain under their QA run directories. Captures are actual Unity output, not a comprehensive visual acceptance review.

Still unverified: a clean full Play Mode rerun after resolving the remaining cases, human rounds 31–200 difficulty, current Android build/device behavior, broad visual acceptance and optional returning-player recap. Animation/effect development remains last.
