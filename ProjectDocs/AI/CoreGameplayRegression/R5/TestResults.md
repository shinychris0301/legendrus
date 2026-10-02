# Core gameplay regression R5 — clean broad runtime run

2026-09-30. User approved a broad current regression after the recent readability corrections.

**Single broad run: 1,325 passed, 0 failed, 12 skipped. All ten dedicated restart phases then passed, covering nine of those skipped cases. Final distinct coverage: 1,334 passed, 0 failed, 3 deferred backend cases skipped.**

The broad run itself completed without corrections or reruns. Its overall XML status is `Skipped:Ignored` because of the conditional cases; it is not a claim that every project test passed. The final distinct total combines that single broad run with the required fresh-process phases. Language writer/reader use the same case name and count once in distinct coverage.

## Executed checks

Unity **6000.5.9f1**, all eight Play Mode assemblies, about31 minutes for the broad run. Disposable product storage, correct continuation-fixture directory prefix, existing licensing-directory link only, descriptor limit4096 and previously authorized **process-only Vulkan**. No project graphics settings change.

| Run | Passed | Failed | Skipped |
|---|---:|---:|---:|
| Broad, all eight assemblies | 1,325 | 0 | 12 |
| Retained failure: write → recover/decline → restart after decline | 3 | 0 | 0 |
| Continuation reservation: write → refund → reject duplicate replay | 3 | 0 | 0 |
| Language: separate write/read processes | 2 | 0 | 0 |
| Complete player save: separate write/read processes | 2 | 0 | 0 |

Do not sum rows to obtain distinct-case coverage. [Broad summary](QA/Broad/summary.json) · [Assembly results](QA/BroadAssemblyResults.json) · [Run inventory and skips](QA/RunInventory.json) · [Latest result per case](QA/LatestCases.json).

Broad assembly coverage: Campaign751; Runtime536 passes and12 conditional skips; Commercial11; Compliance6; Balance6; Diagnostics7; Optimization4; QAReadiness4. All eight assemblies have zero failures.

## Results

- **500 rounds ×3 seeds =1,500 completion replays**, all within move limits, using2–20 moves without inventory boosters. [Witness summary](QA/CampaignWitnessSummary.json). This is automated solvability/replay evidence, not human difficulty, pacing or every-seed acceptance.
- Board input and recovery, matching rules, item/booster consumption, current animation contracts, campaign progression/finale, native UI/navigation and local service guards passed their selected runtime assertions. Passing existing animation assertions does not approve final visual quality.
- The recent Chapter Complete, Fail/Energy, PreLevel/Pause and Settings/Language Select corrections ran in the same broad sequence, with no recurrence of the previous order-dependent Settings failure.
- Retained Failure was restored after process restart and its decline remained finalized on the next restart. Local reservation/refund replay protection passed. These checks do not enable playable paid/ad continuation.
- Saved language was restored in a new Unity process. The player-save reader restored the complete saved payload, settings, language and inventory; repeating completion at service level did not duplicate first-clear rewards. [Restart outcome](QA/SaveCheckpoint.json.read.json). This is not Android process death or a second human-played round.
- Broad run generated718 page captures. Five360px captures were inspected: Chapter Complete, Settings, Language Select, Pause and PreLevel help. [Visual spot checks](QA/VisualSpotChecks.md). This is not comprehensive visual acceptance of all captures.

## Remaining exclusions

Three cases were not executed: real backend HTTP bootstrap/refresh and the two backend session-rotation restart phases. They require the isolated loopback backend runner; hosting/online work remains deferred. Exact case names/reasons appear in the run inventory. Historical backend passes are not counted here.

No Edit Mode suite, Android build/install, device touch/process-death/performance test or human campaign playtest ran in this task. Phone execution remains explicitly postponed. Human31–500 difficulty/pacing, optional catch-up recap, full-device gameplay acceptance and final animation/effect presentation remain open. Existing small source captions/secondary controls retain their earlier limitations.

## Scope and next step

**No production or test-code changes were needed.** [Hash scope](QA/Scope.json): all7,785 baseline files under Assets, Packages, ProjectSettings and Backend unchanged;0 changed/added/deleted. Existing working-tree edits preserved. Only new verification scripts/evidence and task/schedule/session documentation were written. Python syntax checks passed. No commit, deployment, purchase, secret access or player-data modification.

**Next recommended task:** prepare a development-only disposable Android gameplay profile and its verification harness, closing the isolation prerequisite identified by the earlier Android journey preparation. Preserve normal player data and default startup behavior; prove isolation in Editor before later controlled device execution. Phone tests remain deferred; hosting remains postponed and animation/effect redesign stays last.
