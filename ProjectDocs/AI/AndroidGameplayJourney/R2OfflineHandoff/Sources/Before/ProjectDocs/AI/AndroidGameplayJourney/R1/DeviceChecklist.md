# Offline Android gameplay — prepared checks

Status: **prepared; physical execution postponed by the user on 2026-09-30.** Editor evidence is in [TestResults.md](TestResults.md). All device rows below remain pending.

Update: [Disposable gameplay profile R1](../../AndroidGameplayProfile/R1/TestResults.md) now supplies the isolation prerequisite, verified in Editor. [Profile commands and scenarios](../../AndroidGameplayProfile/R1/Usage.md). A new guarded APK build and Android activation verification are still required; the installed credential-probe APK does not include this code. Historical entry conditions below remain as the original R1 checklist.

## Entry conditions

1. Resume device work only when requested. Connect one authorized phone and run from the project root:

   ```bash
   python3 ProjectDocs/AI/AndroidGameplayJourney/R1/Sources/DevicePreflight.py
   ```

   This records device count/model/API and compares the installed development APK with the local R5-verified APK. It does not install, launch, stop the app, read player data, or export credentials. Exit 2 means a prerequisite is missing. `ready_for_isolation_check` confirms APK identity only, not gameplay readiness. Review a different build identity before continuing.
2. Establish a disposable QA Android user/profile or an explicitly isolated game-save fixture before controlled energy, inventory, failure and restart tests. **This task does not implement Android gameplay save isolation.** Do not reuse the credential probe as a gameplay fixture. Do not clear/uninstall the existing app, overwrite normal saves, or export credentials to create a baseline. If isolation is unavailable, stop controlled fixture tests and record that dependency.
3. Use existing approved development packaging and startup. Record APK hash, Unity/build version, phone model/API, resolution/insets, language and QA profile ownership. Verify offline mode before starting; real purchases, ads, accounts and social services are outside this run.
4. Record visible baseline balances, booster quantities, selected round, first-clear status and settings. Keep evidence limited to the disposable profile. Exact Editor move counts are automated fixtures, not a phone pass criterion or a human difficulty target.

## Device execution matrix

| ID | Action | Expected result / evidence |
|---|---|---|
| NAV-01 | From normal startup, use actual touches through Home → World Map → Chapter Map → story/PreLevel → Gameplay. Return using visible Back controls. | Correct hierarchy, one active screen, no blocked taps or stuck overlay. Record route and screenshots. Repeat in Russian and English. |
| INPUT-01 | Make a valid swap, then an invalid swap; interrupt a drag with Android Home and return. | A valid move resolves once; invalid swap restores without consuming a move. Interrupted input releases, no phantom move or persistent touch lock. Record moves before/after and a short clip. |
| ENERGY-01 | On disposable state, start with zero energy; then start a fresh round with energy available. | Zero-energy start is blocked by the normal UI. Successful initial start charges once. Opening/closing PreLevel, Settings or Pause does not charge. Record balances and route. |
| WIN-01 | Win an uncleared round using legal touches. Continue through reward/story/map. Relaunch, then play and win that round again. | First-clear reward equals the round definition once; completed story/progress persists. Replay does not duplicate the first-clear grant. Record both before/after balances and completion state. This is a played replay, unlike the Editor service-level replay check. |
| FAIL-01 | Exhaust moves without meeting goals; use free Retry, then Pause → Exit. | Failure grants no win reward. Retry creates a fresh board with the round's initial move budget; preserves the existing free-retry policy without another energy charge. Exit follows the normal map route; no duplicated failure accounting. |
| BOOST-01 | Exercise each available inventory booster on disposable state, including cancel and an unavailable/empty inventory case. | One inventory charge per accepted use; canceled/unavailable actions do not charge. Input recovers and affected cells follow current rules. Record quantities before/after; do not judge deferred effect design as accepted here. |
| PAUSE-01 | Pause → Settings → Back → Resume; change language and settings. Try Android Back from these states. | Visible controls preserve the same round, moves and board. Android Back must not strand input or accidentally spend resources; record its actual route separately, since Editor visible-Back tests do not establish platform Back behavior. |
| LIFE-01 | Background/foreground while idle, dragging and resolving a cascade; test brief screen lock. | On same-process return, no duplicate action/reward or stuck input; board finishes consistently and settings persist. Record whether Android retained or killed the process; do not conflate these cases. |
| SAVE-01 | Change settings/language, complete a round and consume a booster; close and relaunch the disposable app process. | Durable progress, balances, inventory and preferences restore. No duplicate grant or stale transient Victory result. Do not expect arbitrary unfinished mid-round board recovery. |
| FAIL-SAVE-01 | Reach settled Failure; relaunch before Retry/Exit. Then choose Retry or Exit and relaunch again. | The compatible retained failed-round decision restores once with its original score/attempt and no added energy charge. Finalized loss stays finalized; no repeated failure/grant. This is the supported retained-failure contract, not playable paid/ad continuation. |
| CLOCK-01 | Record energy and time, leave app long enough for a real regeneration interval, return and reopen. | Energy catch-up respects its cap and is not repeated on a second load. Do not change the phone clock or treat the Editor timestamp simulation as this test. |
| UI-PERF-01 | Inspect normal gameplay, Pause, Victory, Failure and maps at native phone scale during the above journeys. | Captions legible, safe areas and controls intact. Capture representative frame-time/memory observations during cascades, with device conditions and recording overhead stated. No FPS or readability pass without measured/observed device evidence. |

For each row record: pending/pass/fail, APK hash, baseline/actual result, evidence path, reproduction steps and any open issue. Keep performance and visual judgment separate from functional assertions. Prior R5 credential/title success is not evidence for these rows.

## Reproducing the prepared Editor checks

`Sources/Verify.py` requires fresh run names and isolated `/tmp/lor-android-journey-r1/` product storage. Close the project in the Editor first. The captured options use this workspace's absolute paths. Existing results must not be overwritten.

- Journey: filter in `Sources/TargetFilter.txt`, options in `Sources/JourneyOptions.json`.
- Writer: `FunctionalDemoNavigationPlayModeTests.PersistenceWriterCompletesAndSaves`, options in `Sources/WriteOptions.json`.
- Reader: `FunctionalDemoNavigationPlayModeTests.PersistenceReaderRestoresWithoutDuplicateReward`, options in `Sources/ReadOptions.json`; run after Writer in a separate process sharing only the isolated product store. For another run, update both phase options to a new isolated store/checkpoint.

Use the `LegendsOfRus.Tests.PlayMode.` namespace prefix. Previously authorized `-force-vulkan` applies only to verification processes; project graphics settings remain unchanged.
