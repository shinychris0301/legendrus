# Android gameplay preparation R1 — test results

**8 selected Unity Play Mode tests passed; 0 failed, 0 skipped. Physical phone testing is postponed at the user's request.** No production change was needed for this selected coverage.

| Actual run | Passed | Failed / skipped | Evidence |
|---|---:|---:|---|
| Native journeys, map/PreLevel Back, lifecycle callbacks and energy clock | 6 | 0 / 0 | [EditorJourney](QA/EditorJourney/summary.json) |
| Win and save progress/preferences/inventory | 1 | 0 / 0 | [Writer](QA/Writer/summary.json) |
| Restore in a different process and reject duplicate first-clear grant | 1 | 0 / 0 | [Reader](QA/Reader/summary.json) |

Unity 6000.5.9f1; three separate verification processes with disposable product configurations. Writer and Reader share the isolated test store; their test asserts different process IDs. Existing licensing-directory link only, no license contents copied/read; previously approved process-only Vulkan. These are selected tests, not a full-project run. Earlier 105-case regression results are not counted here.

## Observed results

- Russian native victory journey: 6 legal moves, energy 5 → 4, gold 12,540 → 12,660. Native Pause/Settings/Resume preserved the round; story and return navigation passed. [Result](QA/JourneyCaptures/Win_RU_Result.json).
- English native failure journey: 28 legal moves, energy 2 → 1, gold unchanged at 12,540. Zero-energy gating, fresh free retry without another energy charge, and exit/navigation passed. [Result](QA/JourneyCaptures/Fail_EN_Result.json).
- Simulated Unity pause/focus messages preserved the board and released interrupted input. Explicit timestamp adjustment verified capped, non-repeated energy catch-up. These are not OS suspension, physical touch, or a real-time phone regeneration wait.
- Writer persisted the completed round/story, all four settings off, English, one consumed mace, 17 spent gems and selected round 2. Reader compared the entire saved payload with fresh GameState initialization, then verified a service-level repeat completion did not grant gold, gems or a mace again. [Fresh-process outcome](QA/ColdProcessCheckpoint.json.read.json). This is not a second manually played round or an Android process-death test.
- Inspected actual Unity [Victory](QA/JourneyCaptures/Win_RU_11_Victory.png) and [restored Settings](QA/PersistenceCaptures/PersistRead_01_RestoredSettings.png) captures at 576×1280. Controls fit these captures and restored switches are off; this does not establish native phone text readability or all-page visual acceptance. Journey fixtures mark tutorial flags complete, so first-time tutorial acceptance is outside this run.

## Phone preparation

Added a [device checklist](DeviceChecklist.md) and [read-only preflight](Sources/DevicePreflight.py). Initial ADB query found 0 devices. Local development APK SHA-256 remains `6a379c4671143b595d1d0e5ce731cf12d7c40104e8f3dbb3ffbfc26f48a761f0`, matching the R5-verified package. [Preflight result](QA/DevicePreflight.json). No APK build/install/launch, app-data change or credential export occurred. Python source syntax checked successfully; the connected-device branch has not been executed.

User selected **“Prepare the checks; test the phone later.”** Actual touch, Android Back, lifecycle/process death, retained-failure recovery, frame time/memory and phone readability remain pending. A disposable Android gameplay profile/save fixture must be established before controlled device tests; this preparation does not implement that isolation or authorize clearing the existing player's data. Current failed-round recovery is supported; arbitrary mid-round resume and live paid/ad continuation are not implied.

## Scope and next action

[Task-start hash comparison](QA/Scope.json): **7,785 Assets/Packages/ProjectSettings/Backend files unchanged; 0 changed, 0 added, 0 deleted.** Added only this review package and updated CurrentTask, SessionLog and DevelopmentSchedule. Existing edits preserved. No runtime/rule/art/prefab/backend/settings changes, commit or deployment.

Next core task while the phone is postponed: run the broader current Play Mode suite with correct isolated fixture/phase configuration, separate genuine defects from stale contracts, and report the remaining coverage precisely. Device execution resumes separately when requested. Human Chapter 1 difficulty/pacing remains open; hosting/online activation is postponed and animation/effect work stays last.
