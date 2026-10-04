# 2026-10-03 — Round lifecycle recovery R1: source delivery

The user approved the next core task after the selection recovery APK. This delivery fixes reproduced round-registration/failure defects in the existing runtime. Backups of four production files, four fixtures and four continuity documents were taken before edits. Human rounds 201–210 remain unrecorded from the user's prior confirmation; no rescan, gameplay tuning, phone or hosting work was performed.

Eight pre-fix cases failed in isolated Unity: rejected/locked registration and legacy failure writes mutated live progress/presentation; attempt/retry counters wrapped; native Play entered Gameplay after attempt overflow; retained failure finalization wrapped and discarded the checkpoint. [Reproduction](QA/Reproduce/summary.json) retains eight primary assertion failures, zero skips, launcher exit 2. [Focus](QA/Focus/summary.json) subsequently passed all eight. [Boundary checks](QA/BoundariesFinal/summary.json) passed 11/11.

`TryRegisterLevelAttempt` and the legacy integer wrapper now return an acknowledgement/zero and publish a cloned candidate only after commitment. `TryStartLevel` commits real energy-clock catch-up, one energy debit, the selected level and attempt/retry counters in one journal transaction. Invalid levels, pending booster recovery, insufficient energy and arithmetic rejection cannot publish partial changes. A rejected journal write locks further ordinary writes until existing recovery/reinitialization; it leaves live balances, timestamp, counters and events untouched. As in earlier transaction paths, a durably committed primary is adopted even if later cleanup fails, with further saving locked; this policy remains covered by the journal tests, not a new Android crash test.

Actual `ScreenRouter.StartLevel` recovers pending inventory, checks normalization, acknowledges the combined start and stays on the preparation page with the existing localized error modal on rejection. Its scoped reentry guard prevents a Changed callback from starting twice. The synchronous Gameplay build consumes the exact committed level/attempt once; its ordinary initializer validates the journal attempt and does not register again. Existing ordinary inventory receipts, campaign/sandbox ownership, retained-round rules and deterministic seed utility remain intact. RU/EN tests cover actual Play rejection after normalization, repeated locked Play and callback reentry; normal-board tests verify the exact seed, unchanged attempt/energy during initialization and a single Applied SeaPearl receipt with five moves. The no-energy test exercises the existing modal Close button; Android Back behavior is not changed or newly accepted here.

`TryRecordLevelFailure` and its legacy void wrapper now commit progress before publishing the copied/clamped transient failure result. Changed observers see both committed counters and that result. Attempts/retries/failures use checked increments. Existing retained failure finalization rejects overflow and preserves the checkpoint/journal. The retained memory checkpoint and deliberate in-session retry policy remain unchanged; legacy failure recording does not clear that optional memory owner. These changes do not add an arbitrary mid-round checkpoint or paid continuation.

Final actual Unity 6000.5.9f1 (b57deb96f08d), fresh isolated storage per launcher:

| Group | Mode | Passed | Failed | Skipped |
|---|---|---:|---:|---:|
| Core | PlayMode | 178 | 0 | 0 |
| Navigation | PlayMode | 38 | 0 | 0 |
| Runtime | PlayMode | 72 | 0 | 0 |
| Retained | PlayMode | 52 | 0 | 0 |
| Edit | EditMode | 112 | 0 | 0 |

**340 Play + 112 Edit = 452 distinct final passes**, zero failures/skips, all five final launchers exit 0. Focus 8 and BoundariesFinal 11 are subsets counted once. [Reconciliation](QA/TestReconciliation.json) · [Source preservation](QA/SourceValidation.json) · [Final validation](QA/FinalValidation.json). XML, summaries, fixture cleanup records and copied launcher/Unity logs are retained. Play Mode uses process-only Vulkan; no project backend/color-space settings changed. Fresh GameState recovery is same-process Unity evidence, not an Android/process restart test or human acceptance. The suites cover previous rewards, spending/refunds, energy, selection/tutorial and journal/migration policies, native win/fail/retry, ordinary boosters, retained attempts and continuation recovery. This is targeted regression, not a fresh full-project sweep or physical performance measurement.

The first boundary launch had two test compilation errors (seed helper signature and the receipt's Applied enum); corrected before execution. The corrected launch passed 9/11 and exposed two inaccurate test expectations: startup already applies energy regeneration, and no-energy Close must be exercised through the existing button rather than unrelated generic Back navigation. Those assertions were corrected without changing the production clock/navigation policy. [Initial boundary evidence](QA/Boundaries/summary.json) and [9/11 run](QA/BoundariesCorrected/summary.json) are preserved separately from the final passing run.

Changed source/fixtures:

- `Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.RetainedRound.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/Match3RuntimePlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/PreLevelAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/RetainedRoundPlayModeTests.cs`

Task-local [patch](Sources/TaskChanges.patch) reverses cleanly against current files. Exactly eight of 7,839 inputs changed, 7,831 unchanged, none added/deleted. GameState outside attempt/failure methods, retained partial outside the checked failure increment, router outside normal start/attempt forwarding, board outside ordinary initialization/acknowledgement, and all earlier fixture checks are byte-preserved. Approved art, metas/GUIDs, actual prefabs/builders, fonts, settings, packages, backend, Android guards and other user changes remain unchanged. Four continuity documents retain earlier acceptance/history.

Current development APK remains **766,023,250 bytes**, SHA256 `0c7c454dc83154e622fff646ac865a2c03fee5b5852ab5f46be24703bf48fb7a` with unchanged sidecar. Its inspected metadata lacks TryStartLevel: **this APK does not include R1 source fixes**. No build, signing/credential access, personal-save access, Git commit/push, installation, store upload or phone/hosting action occurred.

**Next:** use the existing guarded build workflow to refresh the Android APK with round lifecycle recovery, then update artifact pins and evidence. Human 201–210 recording/ratings/review before tuning remains parallel; all 18 physical acceptance rows remain pending. Hosted identity, real paid/video settlement, RuStore production readiness and profitability validation remain later unverified work; stock-max remains restricted and final effects stay last. Settings and other general Persist/lifecycle callers remain outside this source patch.
