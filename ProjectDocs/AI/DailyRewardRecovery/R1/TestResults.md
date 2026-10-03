# Daily reward overflow recovery — 2026-10-03

Implemented the next bounded core recovery fix while human rounds201–210 and phone/hosting work remain pending. **Daily reward claims now return false on arithmetic overflow before any journal write or receipt publication.** The existing native UI shows its ordinary localized failure feedback and keeps the claim retryable.

Cause: `DailyRewardSchedule.TryApply` already calculates with checked arithmetic before mutating its candidate. `GameState.ClaimDailyReward` did not handle `OverflowException`, allowing it to escape the claim button. The fix catches only that exception around candidate calculation. Claim eligibility, balances, prior write result and persistence availability remain unchanged; successful reward values, storage policy and receipt routing are retained. This does not saturate balances, discard excess rewards or introduce a stock cap.

| Check | Actual outcome | Evidence |
|---|---|---|
| Initial fixture compile | Failed: incorrect `SpendBooster` name; corrected to existing `TryUseBooster` | [Summary](QA/Reproduce/summary.json) |
| Verified reproduction before product change | 1 runtime failure: max coin balance raised `OverflowException` through `GameState.ClaimDailyReward` | [XML](QA/ReproduceVerified/results.xml), [summary](QA/ReproduceVerified/summary.json) |
| Final Play Mode daily/receipt checks | **14 passed**, zero failures/skips; launcher exit0 | [XML](QA/Verify/results.xml), [summary](QA/Verify/summary.json) |
| Final Edit Mode schedule/native-page checks | **7 passed**, zero failures/skips; launcher exit0 | [XML](QA/Edit/results.xml), [summary](QA/Edit/summary.json) |
| Source/config scope | 7,837of7,839 inputs unchanged; one service and one existing Play fixture changed; none added/deleted | [Scope](QA/Scope.json), [task patch](Sources/TaskChanges.patch) |
| Final reconciliation | Existing APK/sidecar unchanged; revisions/prefabs/art/metas/settings/guards retained | [Validation](QA/FinalValidation.json) |

**21 distinct targeted Unity tests passed**, not a full-project regression or physical-phone acceptance. Two new Play tests cover seven arithmetic boundaries in one service test and a RU/EN native button/recovery journey in the other. Existing daily/receipt tests cover normal claims, seven-day rewards, repeat claims, failed persistence, energy-cap receipt, localization/layout and navigation. No broad suite rerun was needed for this narrow change.

Seven synthetic boundaries: max gold on day1, gems on day2, Living Light on day4, Sea Pearl on day5, Mace on day6, Firebird in the compound day7grant, and the current-month claim counter. Each rejection leaves the full in-memory save, three primary/backup/staging key values/presence, prior write result and Changed-event count unchanged; the returned receipt is empty and normal persistence remains available. The calendar day is not consumed. The low-level schedule's existing throwing contract is retained and still tested.

The actual native router journey repeatedly presses Claim against an overflowing day7Firebird inventory, verifies localized failure/no receipt/no grant, consumes one booster through the existing transaction and retries. The successful retry grants exactly1,000coins/40gems/one Firebird, opens one receipt, rejects a same-day duplicate and confirms without regranting. Synthetic fixture energy is4with a fresh timestamp to avoid unrelated full-energy clock refresh during assertions; this does not alter production energy rules.

Unity6000.5.9f1 ran with isolated product storage under `/tmp/lor-daily-reward-recovery-r1/`. Play verification uses a process-only Vulkan flag; project rendering settings remain unchanged. Launcher summaries record raw temporary log locations. Test-created InitTestScene files are removed by the runner. **Actual Unity Editor captures**, not generated previews or phone evidence: [recovered Russian portrait](QA/Verify/Captures/LOR_DAILY_REWARD_CAPTURE_DIR/DailyReward_OverflowRecovered_ru.png) (inspected) and [English portrait](QA/Verify/Captures/LOR_DAILY_REWARD_CAPTURE_DIR/DailyReward_OverflowRecovered_en.png). Existing screens and wording are reused; no design approval is inferred.

Production change: `Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs` only. Regression addition: `Assets/LegendsOfRus/Tests/PlayMode/DailyRewardAlignmentPlayModeTests.cs`. No prefab/component binding or builder configuration changes are needed for a service-only rejection path; native owner/revision checks pass. All existing metadata/GUIDs, approved Splash, art/fonts, other services, packages/settings/backend and Android approval guards remain unchanged. Six existing source/document files are backed up under `Sources/Before`; the task patch reverses cleanly.

The current **R5 APK remains766,013,502bytes**, SHA256 `91c38f1a6ae63482d8d9e713dcb8f0f891b9b0a67e8743b30de390ab906dc8a9`, and **does not yet package this service fix**. No Android build/install/launch, physical testing, normal personal journal access, hosted/commercial activation, Git commit or push occurred. Existing ordinary level/booster credit overflow paths remain a separate known issue; `stock-max` remains display/consumption only.

Next: package this verified service fix through a separate guarded development APK refresh, preserving R5 first and checking the existing signing/identity/asset guards. Human201–210 outcomes remain pending in parallel; phone/hosting deferrals and effects-last order remain in force.
