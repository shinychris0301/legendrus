# Campaign Foundation R10 — playtest collection

User confirmed that rounds11–20 have not yet been played and requested collection preparation. The existing local JSON files were older five-round mechanics-sandbox sessions, not current campaign results. The current campaign evidence report contains zero manual sessions. No level budgets, layouts, rewards, story assets, artwork or effects were changed.

## Implemented

- **Campaign Playtest11-20 (Recorded)** editor entry opens the existing R9 candidate at the hearth scene/round11 with explicit disposable1–10progress,30test energy and zero starting inventory boosters. Other preview menus keep their prior behavior. Live saves are not read/imported; campaign test progress remains in memory.
- **CampaignPlaytestRecorder** is attached only by that explicit entry or tests. Existing CampaignDevelopmentUI supplies board-start, board-result-before-destruction and stop callbacks. It records chapter/round, committed attempt/run, rules, exact content and level fingerprints, board hashes, moves/score/goals, active time, outcome, inventory usage and feedback. It does not issue commands or mutate campaign saves.
- Existing MechanicsPlaytestLog/Storage is reused. Added optional campaign context and special-opportunity rating; default sandbox constructors/source identifiers remain compatible. Local atomic checkpoints and previous-copy behavior are reused. Completed attempts cannot become interruptions on teardown; retry attempts remain separate. A crash can leave an ongoing snapshot, never an inferred loss.
- The **Campaign Playtest Monitor** is an EditorWindow, so no controls obscure the approved game pages. It shows save status/path and offers difficulty/goal/special-opportunity ratings, Save, Copy path and Reveal. Rate before Continue/Retry. There is no free-text field or automatic upload.
- The recorder saves at start, settled commands, periodic active checkpoints, focus/pause and end. Save failure is visible/retryable and does not prevent gameplay. Active time excludes application pause/background and board pause. The terminal outcome represents the board result, not a claim that subsequent reward storage succeeded.
- `AnalyzeSessions.py` selects manual sessions matching the current four-file content fingerprint, deduplicates snapshots and separates boosted/unknown-inventory, unfinished and interrupted attempts. It rejects malformed/inconsistent records and never incorporates old sandbox or automated runs into human outcomes. Session count is not a unique-player count. It does not produce a population win-rate estimate.

No command-level replay collection was added. R9's deterministic solver witnesses remain separate from these outcome records. On this machine recordings go under `Application.persistentDataPath/CampaignPlaytests/Sessions/manual_campaign`; the monitor gives the actual path.

## Verification

Unity6000.5.9f1 using prior approved process-only Vulkan/descriptor4096, isolated test configuration; project backend/settings unchanged.

Final Verify02: **40/40 passed**, zero failures/skips:16recorder/storage compatibility checks,14candidate checks,10campaign UI/board regressions. New coverage includes real UI board win captured before destruction, no-inventory usage, ratings persisted after teardown, retry/exit separation, paused-time exclusion, failed local writes and retry without campaign mutation, manual/automated source separation, and30-energy/zero-stock import/start. Existing tests cover atomic replace/backup behavior. Verify01 recorder15/15 also passed before the explicit test-preset check was added.

[Automated sample recording](QA/Verify02/Recorder/Journey/AutomatedRecorderExample.json) is a test round1 capture labeled `automated_campaign` with a test fingerprint. It is **not a human rounds11–20 session**. Its completed goal counters, final hash and persisted ratings were inspected.

Python analyzer:3tests passed covering source/content filtering, duplicate snapshots, unfinished/boosted separation, missing evidence and inconsistent wins. Current manual report:0sessions. Browser review430/1440: no JavaScript errors or horizontal overflow; mobile page inspected. Changed/added C# whitespace check passed.

No physical Android testing, human difficulty acceptance, full repository suite or manual interaction walkthrough of the EditorWindow was performed. The editor window compiled; runtime recording and persistence behavior were exercised automatically. Test progress/log fixtures do not establish production save durability or OS power-loss guarantees.

## Scope

Three existing files changed: CampaignDevelopmentPreviewMenu.cs, MechanicsPlaytestLog.cs and CampaignDevelopmentUI.cs. Added CampaignPlaytestRecorder.cs, CampaignPlaytestWindow.cs, CampaignPlaytestRecorderTests.cs and their metas. [Task scope](QA/Scope.json):3changed/6added/0deleted/7459unchanged Assets/Packages/ProjectSettings files. Existing user work preserved; temporary test artifacts/linker and task Python cache cleaned. No prefab rebuild or serialized binding changes, no Git commit.

## Next action

Follow [PlaytestInstructions.md](PlaytestInstructions.md), play11–20, rate each result before advancing, then share the generated JSON/path. Review human evidence before tuning budgets or authoring21–30. All animation/gameplay-effect refinement remains the final development step.
