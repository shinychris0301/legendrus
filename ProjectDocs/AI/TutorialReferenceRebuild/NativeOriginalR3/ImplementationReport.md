# Tutorial / Help — native original reference R3

Implemented the approved supplied reference in the active Unity TutorialOverlay prefab. User then requested keeping the whole teaching panel, extracting only the title/buttons separately, and replacing its gems with Gameplay resources. The implementation follows that direction.

## Composition and behavior

- One isolated opaque parchment panel retains its outer decoration, section ribbons, illustration frames and correct original Russian text. Local repair masks remove replaced illustrations and incorrect instructions. Separate title and two buttons retain original Russian lettering; English uses live localized labels and repaired surfaces.
- Teaching board, red/blue goals and reward gem reference `ProductionGameplayArt.Gem` directly. Board rails/wood and match-created specials reuse Gameplay resources. Existing Home gift and coin art are reused. No replacement gem or booster set generated.
- Shared `ProductionPlayerHeader` cloned from the approved Shop prefab shows actual saved coins, gems and energy. Resource shortcuts open the appropriate Shop category or Energy page.
- Native Back/Got It return to the origin. Lesson headings open the existing detailed guide; its Close/Done return to the overview. First Level uses `SelectLevel(1,1)` and opens Pre-Level without spending energy. Actual Play still follows the existing start gate.
- Help opened during Gameplay is a blocking modal preserving the board. Its header shortcuts are disabled; Return to Round resumes the same round without resetting score or moves.
- Header/footer follow safe edges. The intact panel scrolls on short displays and retains reference proportions on taller ones. A full-screen reference with invisible hotspots is not used.
- Instructions reflect existing rules: 4→line, 5→color, T/L→area special. Crate objective replaces the unsupported shield. Moves/rewards explicitly labelled examples.

## Assets and Scenario

Source: `/home/k2/Downloads/1/Fantasy Match-3 Tutorial Screen.png` (941×1672), copied to Sources/Reference.png. Two Scenario isolation jobs (3 CU each) and one clean scenery reconstruction (13 CU), total 3 jobs / 19 CU. Request, result, billing and extraction inputs retained under Sources/Scenario. Local source RGB, bounded alpha cleanup and repair masks produce 9 new PNGs. Pixa initially retained crop backgrounds: local contour cleanup removed them. Transparent assets reviewed over black, white and magenta. Generated room is an independent reconstruction of hidden scenery, not exact recovery.

Runtime/view/layout, owning builder, active prefab and cutover agree on revision4. Existing GUIDs, source fonts, Gameplay gem art and other page prefabs preserved.

## Verification

Unity6000.5.9f1. Baseline01:15 targeted Tutorial/Login checks passed. Verify01 rebuild completed but launcher terminated before test stages; not counted as a passing run. Verify02: EditMode5 passed; PlayMode10 passed/2 failed (detailed-guide Done navigation and strict boundary comparison). Navigation fixed, inclusive 1px bounds used. Verify03:17 passed. Final Verify04:5EditMode+13PlayMode=18 passed; successful rebuild/readback; 14 actual Tutorial captures (overview RU/EN, tall safe area, short viewport and detailed sections). Tests exercise native pointer hits/press feedback, language, read-only save preservation, shared sprite identity, first-level route/energy preservation, resource categories and paused-board identity/hash/moves/score.

Browser review:16 labelled portrait choices at430px, no JS errors/horizontal overflow. Existing5,615 protected PNG/meta/font/other-page files unchanged. Scoped source/cutover whitespace checks passed. Full check reports Unity-serialized empty YAML fields with trailing spaces in the rebuilt prefab; serialized output was left intact. See QA/ScopeAndVerification.json for full file inventory and test totals.

No Android/device build, full test suite, touchscreen scroll-gesture test, backend, performance or fresh-process save persistence check. Short viewport capture shows intended clipping of scroll content above the fixed footer; full lower panel is available through ScrollRect. Local repair textures and live fallback type are approximate. Shared header values and supported instruction illustrations intentionally differ from reference samples.

## Files changed this task

- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/TutorialOverlayCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/TutorialOverlay.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionTutorialPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTutorialLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTutorialRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTutorialView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/TutorialAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/TutorialAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/done.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/done.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/done_ru.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/done_ru.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/environment.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/environment.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/first.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/first.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/first_ru.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/first_ru.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/panel.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/panel.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/panel_ru.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/panel_ru.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/title.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/title.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/title_ru.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal/title_ru.png.meta`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTutorialOverview.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTutorialOverview.cs.meta`
