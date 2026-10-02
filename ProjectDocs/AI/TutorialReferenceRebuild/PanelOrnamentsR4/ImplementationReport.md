# Tutorial R4 — panel ornament restoration

The user reported cut-off parts in the extracted panel, highlighting the reward section. Inspection confirmed that R3 rectangular navy cleanup patches overwrote original reward side scrollwork, the lower rail, reward-title arrows and portions of the moves medallion/card edges. These were asset defects, visible even at the reference aspect ratio.

## Correction

Restored original source RGB in bounded decoration regions, keeping the parchment as one panel. Replacement surfaces now stop inside the reward frame. Russian retains the original НAГРАДЫ lettering and original illustrative28 medallion. English replaces only the label interiors, with native fitted text. Examples remains labelled above the tray. Two small Firebird-occluded frame spans use the complete adjacent source frame as a donor. Reward icons moved inward/reduced to clear the restored rail and title plaque; exact Gameplay gems and existing Home gift/coin assets retained.

Added panel_ru_r4.png and panel_r4.png with new metas. Earlier PNGs and GUIDs untouched. Runtime overview/view, owning builder, active TutorialOverlay prefab and cutover revision5 agree. Navigation/gameplay services unchanged this pass.

## Validation

Unity6000.5.9f1. Verify01:5EditMode+13PlayMode passed; visual inspection prompted a bounded cleanup of two original Firebird-over-frame spans and English caption remnants. Final Verify02 rebuilt the prefab and again passed5EditMode+13PlayMode=18 checks. Fourteen actual Tutorial captures retained; final RU/EN/reference-size and tall safe-area captures inspected. Browser:16 labelled review choices at430px, no JS errors or horizontal overflow.

Source RGB checks:110,940 protected pixels match exactly in RU;105,657 match in EN outside the local caption repair. Donor spans account for2,376 pixels. Existing panel alpha preserved; reviewed new assets on black/white/magenta. Conservative alpha-footprint checks find zero reward-icon overlap with protected decoration.5,635 existing PNG/meta/font/other-page-prefab files unchanged. Scoped C# whitespace check passed; Unity's serialized empty-field whitespace remains untouched.

No Scenario generation or API calls:0 jobs/0CU; reused original source and approved resources. No Android/device build, fullsuite, backend/performance or physical scroll testing. Earlier reconstructed scenery, repaired interior textures, live font differences, and reused gift artwork remain as before. Short-screen clipping is still intentional ScrollRect behavior; this repair addresses missing ornament pixels inside the panel.

## Changed assets and code

- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/TutorialOverlayCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/TutorialOverlay.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionTutorialPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTutorialOverview.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTutorialView.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/TutorialAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/panel_r4.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/panel_r4.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/panel_ru_r4.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/panel_ru_r4.png.meta`
