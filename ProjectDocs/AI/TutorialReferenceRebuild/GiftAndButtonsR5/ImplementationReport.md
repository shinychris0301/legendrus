# Tutorial R5 — gift background and complete buttons

The gift used the Home source icon including its square carved surround. The Tutorial footer masks also removed pointed corners and parts of both horizontal gold rails. User requested checking/fixing both.

## Implemented

- New gift_medallion_r5 retains the original gift RGB inside its circular metal rim, removing the external carved square through alpha only. Original shared Home asset remains unchanged.
- Re-extracted green Done and blue First Level buttons from the original source with larger complete contours. Supersampled silhouettes retain Russian lettering, corner points, lower jewel and continuous upper/lower rails. Preserved intentionally opaque button faces.
- English variants share identical alpha and original decoration, repairing only interior text areas for native localized labels. Hidden surface reconstruction is approximate.
- Corrected button positions/sizes to source bounds, adjusted native English label areas, and wired the five new sprites through the owning runtime overview. View/builder/active prefab/cutover revision6 agree. Existing actions, panel ornaments, Gameplay gems and shared header retained.

## Verification

Unity6000.5.9f1. Verify01 and Verify02 each passed18 targeted checks; subsequent visual inspection corrected English text remnants and a few remaining rail alpha rows. Final Verify03 rebuilt/read back the prefab and passed5EditMode+13PlayMode=18. Fourteen actual Tutorial captures saved; final RU/reference-size and tall safe-area EN visually inspected. Existing native hit/press, first-level, header category and paused-board tests passed. Browser review16 labelled views at430px, no errors or horizontal overflow.

Asset checks prove Russian button RGB equals the original crops, English RGB outside recorded repair masks is unchanged, both localized variants use identical alpha, and top/bottom rail rectangles are fully opaque. Gift RGB unchanged;6,754 previously nontransparent outside pixels are now transparent. Reviewed black/white/magenta sheets.5,639 protected existing art/meta/font/other-page files unchanged; scoped source whitespace passed. Unity-generated empty-field YAML whitespace retained.

Six existing Assets files modified; five PNGs plus metas added.0Scenario jobs/0CU: source extraction only, no generation needed. No Android/device build, fullsuite, physical touchscreen, backend or performance verification. Existing repaired panel surfaces, reconstructed scenery and live typography remain approximate; no exact hidden texture/font claim.

## Files

- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/TutorialOverlayCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/TutorialOverlay.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionTutorialPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTutorialOverview.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTutorialView.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/TutorialAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/done_r5.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/done_r5.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/done_ru_r5.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/done_ru_r5.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/first_r5.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/first_r5.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/first_ru_r5.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/first_ru_r5.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/gift_medallion_r5.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/gift_medallion_r5.png.meta`
