# Doubled gold reward recovery R1 — 2026-10-03

`GameState.TryDoubleLastLevelGoldReward` now returns false when the entire bonus cannot fit or its journal write is blocked. Gold and the doubled flag are prepared on a cloned candidate and published only after the existing `SaveService.TrySave` succeeds. Arithmetic rejection preserves the original save, eligibility, journal, previous write result and persistence availability. A write rejection retains the original in-memory save and follows the existing persistence-lock behavior. A successful commit raises one change event.

This is a verified **source-only** change. The current development APK remains 766,014,334 bytes / SHA256 `9f72045fe3a2e68114be24d687dfb2a51c07c48d39ec58aa4afbafe9a5744654`, with unchanged checksum sidecar. It packages the earlier daily-reward fix but **does not include this doubled-gold fix**. [Current APK packaging record](../../DailyRewardRecovery/R2AndroidRefresh/TestResults.md).

## Reproduction and executed checks

| Run | Actual outcome | Evidence |
| --- | --- | --- |
| Original production method with regression fixtures | Three overflow cases returned true; the blocked-save case emitted the existing Persist error | [Initial reproduction](QA/Reproduce/summary.json) |
| Verified behavioral reproduction | All four cases failed with Expected False / Actual True; Unity exit 2 | [Reproduction](QA/ReproduceVerified/summary.json), [XML](QA/ReproduceVerified/results.xml) |
| First post-fix Play Mode suite | 36 passed / one screenshot fixture failure: star reveal had not finished | [Retained initial run](QA/InitialVerification/summary.json) |
| Final Play Mode suite | **37 distinct passed**, zero failed/skipped; launcher exit 0 | [Summary](QA/FinalPlay/summary.json), [XML](QA/FinalPlay/results.xml) |
| Save-journal write and native Victory/DailyReward Edit Mode contracts | **45 distinct passed**, zero failed/skipped; launcher exit 0 | [Summary](QA/Edit/summary.json), [XML](QA/Edit/results.xml) |
| Input, patch, artifact and handoff reconciliation | Two existing input files changed, 7,837 unchanged; no additions/deletions | [Scope](QA/Scope.json), [final validation](QA/FinalValidation.json) |

Final total: **82 distinct Unity checks passed**, targeted coverage rather than a full project sweep. Editor was the installed 6000.5.9f1. Runtime tests used isolated product storage, the real router and native Victory prefab; Vulkan was selected for the test process only. No normal personal journal or signing material was read.

The overflowing inputs were `(gold=int.MaxValue, bonus=1)`, `(gold=int.MaxValue-139, bonus=140)` and `(gold=1, bonus=int.MaxValue)`. Repeated attempts must return false with byte-identical full in-memory save and journal key presence/values, no event, unchanged completion result/write result and preserved persistence availability. A future-version journal fixture verifies that saving cannot publish or consume the bonus; a repeated blocked call preserves the failed write result and journal. The fixture suppresses the old expected error log around that single service call so the incorrect boolean result remains observable during reproduction.

The Russian and English native recovery journey uses an actual round-3 service completion (140 gold / 5 gems), rejects a one-coin overflow, frees that capacity through existing `SpendGold(1)`, and commits exactly to `int.MaxValue`. It checks the full expected DTO, unchanged completion result, one event, Victory's 280-gold / 5-gem presentation and disabled double button. Subsequent calls, a page return, journal load and retry against the reloaded DTO reject duplicate credit. This is service/UI and same-process journal-load evidence, not cold-process Android execution or verified rewarded-video delivery.

Existing normal first-clear/replay/double-grant tests, unavailable-video routing, all authored native reward presentations, progression/story behavior and daily-reward overflow/recovery pass in the final suite. The screenshot fixture now waits for the existing reveal before applying the existing capture assertions; production animation was not changed. The first verification and its failure are retained. A fresh-log collision prevented one rerun launcher from starting Unity; the subsequent unique `FinalPlay` directory preserved earlier evidence. [Launcher-only collision record](QA/RetainedLogCollision.json).

## Actual native capture and scope

The final run retained 79 actual Unity PNG captures with their metadata. The [Russian recovered Victory portrait](QA/FinalPlay/Captures/LOR_VICTORY_CAPTURE_DIR/Victory_OverflowDoubleRecovered_ru.png) was inspected; [English portrait](QA/FinalPlay/Captures/LOR_VICTORY_CAPTURE_DIR/Victory_OverflowDoubleRecovered_en.png) is also retained. These show existing live native UI after a synthetic boundary exercise, not a generated mockup, new design approval, human playtest or phone capture. Victory's existing resource header remains intentionally hidden; its gold balance is verified through the service assertions.

Only [GameState.cs](../../../../Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs) and the existing [VictoryAlignmentPlayModeTests.cs](../../../../Assets/LegendsOfRus/Tests/PlayMode/VictoryAlignmentPlayModeTests.cs) changed from this task's baseline. Other GameState methods, including the earlier daily claim guard, remain unchanged. [Task-only patch](Sources/TaskChanges.patch) is retained against the original working-tree files; prior local edits and review directories remain present. Four continuity documents and this task's own tools/evidence were added or updated.

Reward amounts, multiplier, provider approval gates, routing, story content, first-clear semantics, other credit APIs, saved schema, GUIDs/metas, prefab owners/revisions, art/fonts, approved Splash/Title, packages/settings/backend and Android build guards were preserved. No clamping, new cap or partial payout is introduced. No new APK, device enumeration/install/launch, hosted activation, production/store action or Git commit/push occurred. Temporary test scenes were removed; generated caches are outside the patch.

## Remaining work

Ordinary first-clear grants and `AddGold` / `AddGems` / `AddBooster` still need separate overflow review; `stock-max` remains display/consumption only. This fix cannot certify real rewarded-video settlement, receipt recovery or revenue. A later guarded APK refresh is needed to deliver the new service code.

Human rounds 201–210 recordings/ratings/block observations remain the next acceptance dependency; [recording intake](../../ChapterOneHumanAcceptance/R2RecordingReview/README.md) is ready. Review real feedback before tuning, then subsequent blocks and finale. Phone/hosting remain deferred, all 18 device rows pending, effects last. The next independent source task is reproduction and transaction review of ordinary first-clear credit overflow while preserving progression and complete reward semantics.
