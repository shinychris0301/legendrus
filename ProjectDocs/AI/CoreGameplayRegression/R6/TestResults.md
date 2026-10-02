# Core gameplay regression R6 — 2026-10-01

Completed the approved current eight-assembly Unity regression and isolated local restart checks after the resource cleanup and caption changes. **Latest distinct coverage: 1,342 passed, zero remaining failures, three deferred backend cases.** This combines the broad baseline, focused fixture corrections and seventeen separate restart/profile processes. It is **not one clean post-correction full sweep**; the original failures remain in the evidence.

Unity **6000.5.9f1**, process-only Vulkan, descriptor limit 4096 and disposable product storage under `/tmp`. Only the existing Editor licensing directory is linked; license contents were not inspected or copied. Project graphics/settings and normal player data were not changed.

| Run | Passed | Failed | Skipped |
| --- | ---: | ---: | ---: |
| Broad, all eight Play Mode assemblies | 1,330 | 2 | 13 |
| Original spectacle fixture in isolation | 4 | 0 | 0 |
| Corrected spectacle plus adjacent animation checks | 14 | 0 | 0 |
| Corrected input fixture in the broad-run store | 2 | 0 | 0 |
| Retained failure: write/recover-decline/second restart | 3 | 0 | 0 |
| Continuation: reserve/refund/reject replay | 3 | 0 | 0 |
| Language: write/read processes | 2 | 0 | 0 |
| Complete player save: write/read processes | 2 | 0 | 0 |
| Gameplay profile: normal write, QA write/resume/corrupt/reset/empty-energy, normal read | 7 | 0 | 0 |

Rows overlap. Language writer/reader share one test name; all seven profile processes share another. Ten formerly skipped distinct local cases were exercised. Do not sum executions to obtain distinct coverage. [Run inventory](QA/RunInventory.json) · [Latest cases](QA/LatestCases.json) · [Initial broad summary](QA/Broad/summary.json) · [Assembly results](QA/BroadAssemblyResults.json). Broad invocation took 34.6 minutes and exited 2 because of its two failures; later verified runs exited 0.

## Demonstrated failures and bounded corrections

The input test required `/tmp/lor-android-input-r1/`, while the broad runner correctly uses the continuation fixture's isolated directory. The test now accepts that established broad-regression prefix as well as its dedicated prefix. Both XDG_CONFIG_HOME equality and Application.persistentDataPath containment remain mandatory. Queued touch, mouse and Escape still exercise real native navigation and retain energy/gold assertions. [Input verified 2/2](QA/InputVerified/summary.json).

The inventory-booster spectacle test missed a transient gem-fragment state. The broad recording has a 0.70s sample gap across the native fragment lifetime of 0.62s, consistent with capture-sensitive polling; the unchanged four-case fixture passed in isolation. The test now also observes actual GemShatterView pieces at CellsImpacted, after the native fragment handlers, resetting observation before each command. Polling, mesh bounds, board hashes, move cost, input release and cleanup checks remain. No effect timing, geometry or runtime behavior changed. [Baseline 4/4](QA/SpectacleBaseline/summary.json) · [Corrected and adjacent checks 14/14](QA/SpectacleVerified/summary.json).

The additional profile skip was conditional, not a production failure. Its seven required cold-process phases were added to the existing ten-phase plan. All seventeen passed. [Actual phase plan](Sources/PhasePlan.json).

## Coverage and preservation

All **500 rounds × three seeds = 1,500 completion replays** finish within the move budget, using 2–20 moves without inventory boosters. [Witness summary](QA/CampaignWitnessSummary.json). This establishes the sampled replay paths, not human difficulty, pacing or every possible seed.

Native board/input/recovery, campaign/story/finale, inventory, routing and current UI/service contracts ran in the broad sequence. Both new R3 caption matrix tests passed there. Restart phases restored retained failure, kept decline finalized, preserved local reservation/refund replay protection, restored language and the complete player save, and rejected duplicate first-clear rewards. QA-profile phases restored balances/preferences, locked persistence for corrupt QA saves, preserved synthetic normal-key fingerprints through resets/zero-energy, and returned to normal state. These checks do not enable paid/ad continuation, hosted accounts or arbitrary mid-round resume.

Retained **778 broad-run captures and 1,668 actual Unity PNGs across all runs**. Five current small portraits were inspected: Gameplay, Victory, ChapterComplete, Pause and PreLevel. [Visual observations](VisualSpotChecks.md) · [Capture inventory](QA/CaptureInventory.json). This is limited Editor review, not comprehensive capture or phone acceptance.

Exactly **two existing test files changed; 7,819 other baseline inputs unchanged; zero added/deleted** under Assets/Packages/ProjectSettings/Backend. [Scope](QA/Scope.json) · [Task-only patch](Sources/TaskChanges.patch). Source artwork/fonts, all prefabs/GUIDs/metadata, approved Splash, runtime/rules/services, settings/packages/backend and Android guards preserved. Temporary test scenes removed; prior user changes retained. Python/JSON/XML and patch/link/hash validation: [Final evidence validation](QA/FinalValidation.json).

The existing R4 APK remains **765,772,146 bytes**, SHA256 `924ecabcdbdb58f30512e6e2b134db4246be22c8f9ecc6b6086551e4ad937eff`; sidecar matches. [APK evidence](QA/ExistingApk.json). It already contains R3 captions; this task changed only Editor test fixtures. No new Edit Mode suite, Android build/install, device test, hosting, deployment or commit.

## Remaining acceptance and next action

Three backend HTTP/session-rotation cases remain skipped because their isolated loopback runner was not invoked. Exact names/reasons are in the run inventory; historical backend passes are not added here. A single clean full sweep after the fixture corrections has not been run. Physical-phone input/readability/lifecycle/performance, large stock badges, font provenance and human Chapter 1 difficulty/pacing remain open. Existing effect-contract passes do not approve final visual quality.

Next: review Chapter 1 human acceptance coverage using the existing manual recorder. Audit available manual exports against current content fingerprints and prepare the missing recording matrix for rounds 31–500, prioritizing 201–250, late checkpoints and 495–500. Preserve balance/art and use human outcomes before tuning. Phone execution and hosting remain deferred; final gameplay effects stay last.
