# Visual Page33 — Maintenance R1

Implemented the dedicated original as a native Unity page: approved logo, red/gold title and parchment panel, completion plaque, isolated compensation chest and green “Понятно” action. Labels are live/localizable RU/EN. The page fits the safe area while the clean scenery covers the display. The existing MaintenanceState route now loads its canonical prefab and returns to Home through Understood or system Back.

## Asset workflow

Four Scenario Pixa source-alpha jobs completed for panel, plaque, chest and button: **12CU**, zero RGB generation or paid resubmission. A panel asset request reset once; the same accepted output was recovered. Source, immutable input, masks, raw outputs, jobs, billing and hashes are retained. An initial red donor included ornament pixels; corrected locally with a clean source patch before acceptance. Occupied parchment and red areas use mirrored source texture; original dividers/perimeter remain. Letters on button/plaque were cleared for live text. Alpha inspected over black/white/magenta; intentional opaque panel faces retained.

Approved Home logo, clean Loading dusk environment and embedded ranking fonts are reused unchanged. The full original is documentation only. No screenshot surface or invisible hotspot is used as runtime UI.

## Behavior and limits

There is no maintenance-service integration in the project. Timing says **“Время не объявлено” / “Not announced”**, rather than inventing a countdown. Compensation is unconfirmed and the chest is explicitly a gift preview. Understood does not grant rewards, change saved progress or pretend to reconnect. This UI does not automatically detect server maintenance or block offline play. Future service integration must supply validated notices/timing/reward eligibility; it is not claimed implemented here.

The reference's exact city scenery, font identity/embossing and occupied hidden texture are not reproduced. Existing clean scenery and approved typography are retained. The chest is slightly reduced to separate it from Russian text. Support for actual maintenance status, timing and compensation remains feature work.

## Checks performed

- Unity6000.5.9f1 compilation/import, prefab rebuild/readback and revision1 cutover passed.
- Final Verify02: **4 Edit Mode + 8 Play Mode =12 tests passed**, no failures/skips:5Maintenance checks plus7Offline regressions.
- Four actual Maintenance captures:941×1672 RU/EN,576×1280 safe EN,1080×2400 safe RU. Six Offline regression captures.
- Verified native bindings/no screenshot dependencies, idempotent builder, localization, absence of fabricated timing/reward promises, unchanged save, button/systemBack routing, rebind/repeated-click/disabled guards, safe-area/input targets and rendered text.
- Source RGB outside recorded masks, immutable input and reused asset hashes verified. Final alpha reviewed on black, white and magenta.
- Initial Verify01 also passed12tests; visual inspection identified a Russian text/chest overlap, corrected before final run.
- Source-script syntax and scoped whitespace checks passed; browser results recorded in `QA/BrowserReview.json`.
- Scope audit preserves every existing art file, prefab and GUID/meta, other pages including Gameplay/Splash/Offline, packages, settings and Android guards. Only existing production edit is the Maintenance router binding.

Not performed: APK build, physical phone/touch, actual maintenance server/HTTP/backend testing, countdown/compensation integration, full-project tests or profiling.

## Changed production files

- `Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/MaintenanceAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/MaintenanceAlignmentEditModeTests.cs.meta`
- `Assets/LegendsOfRus/Tests/PlayMode/MaintenanceAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/MaintenanceAlignmentPlayModeTests.cs.meta`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionMaintenancePrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionMaintenancePrefabBuilder.cs.meta`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionMaintenanceLayout.cs.meta`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionMaintenanceView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionMaintenanceRuntime.cs.meta`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionMaintenanceRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionMaintenanceView.cs.meta`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionMaintenanceLayout.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/MaintenanceAligned.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/MaintenanceState.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/MaintenanceState.prefab.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/MaintenanceAligned/timer.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/MaintenanceAligned/chest.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/MaintenanceAligned/button.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/MaintenanceAligned/button.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/MaintenanceAligned/panel.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/MaintenanceAligned/chest.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/MaintenanceAligned/panel.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/MaintenanceAligned/timer.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/MaintenanceStateCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/MaintenanceStateCutoverState.txt.meta`
