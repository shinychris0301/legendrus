# Booster Selection R1 — implementation and verification

Implemented native Visual Page17 in Unity6000.5.9f1. Page owner, runtime/view/layout, active `Pages/BoosterSelect.prefab`, cutover revision1 and native router bindings are in agreement. The real Pre-Level guide now includes **All Boosters**; its Back action and original four inspections are preserved.

The source reference shows eight pre-equipped tools. Current GameState supports four tools activated during a round. An optional question about pre-equipping received no answer; proceeded with the communicated inspect-only assumption. No loadout or extra tool effects were invented. Real counts update while open. Empty stock is marked explicitly and remains inspectable. Play uses the existing selected-level/energy gate once; Close spends nothing. Modal uses real Pre-Level beneath it; direct route uses the existing clean village environment and Back route. No reference image is a runtime dependency.

## Art

Three new assets: empty navy/gold panel, empty emerald Play and original-RGB Close. Scenario jobs: panel13CU, Play12CU, panelalpha3CU, Playalpha3CU, Closealpha3CU = **34CU**, five successful jobs, zero automatic resubmissions or corrective generations. Requests, statuses, billing and hashes are retained under Sources/Scenario and AssetProvenance.json. Local cleanup removes neighbouring rails from Close's API alpha while retaining original RGB. Panel and Play retain their generated RGB and opaque faces. All final sprites inspected over black, white and magenta. Existing Pre-Level medallions reused without edits.

Exact font identity remains unknown. Existing project serif/fallbacks are used for live RU/EN labels. Fine ornamentation and eight-tool reference layout differ; see ReferenceComparison.md. No exact-match claim.

## Checks performed

- Read project instructions, handoff, CurrentTask, recorded reference provenance, reference and active implementation; inspected initial dirty working tree and snapshot before editing. Root and Unity version verified. Handoff is historical Splash context, while current task is Page17; earlier approvals retained.
- Final run3: Unity import, C# compilation, owning builder rebuild and serialized readback passed.
- **5/5 EditMode:** new page's native dependencies/imports/unbound state and repeated build identity; existing Pre-Level bindings/art sharing/imports. Pre-Level expected button count updated from12 to13 for the new native entry.
- **18/18 PlayMode:** seven new Booster Selection tests, six existing Pre-Level tests, five OutOfEnergy tests. All passed; zero failed/skipped.
- New tests cover real guide entry, four inspections/live inventory, empty stock/no mutation, blocked underlying controls, Close/reopen, repeated Bind/Play, exactly one energy spent/no tools spent, no-energy transition to existing modal and return, standalone Back/background coverage, RU/EN labels, safe areas, text fit, raycasts, nonoverlapping tool rows and lower-decoration clearance.
- Seven new actual capture pairs (PNG+JSON), twenty Pre-Level and eight OutOfEnergy regression pairs. Simulated941×1672,576×1280 and1080×2400 portrait/safe-area coverage. Manually inspected final standardRU, tallEN, emptyRU, standaloneRU; also run2 Pre-Level guide. Other images received automated geometry/text/input checks.
- Run1:5Edit passed,14/18Play passed; four Pre-Level captures failed due to misspelled output-directory environment variable. Corrected harness. Native page tests passed. Manual capture review moved Play inward from lower trim.
- Run2:all23 passed. Manual tallEN review prompted final row-spacing adjustment and an explicit overlap/decoration test. Run3 is the final evidence. Earlier logs/XML retained; no failed results hidden.
- Python AST, scoped C# whitespace and source-only git diff checks passed. Including the Unity YAML prefab in the first diff check reported trailing spaces on serialized empty fields; those serializer-produced fields were preserved. Review link existence checked; HTML browser execution not performed.
- Baseline asset hashes: **4753 original files, 4747 unchanged, 6 modified, 23 added, zero deleted**. Every pre-existing .meta unchanged. Gameplay implementation/art and all other page art/prefabs unchanged relative to snapshot; only Pre-Level prefab updated. ScopeCheck.json lists exact paths. Android guards/settings/packages/signing were not edited.

## Not performed

No APK/AAB, physical Android device or hardware Back, full project test suite, performance profiling, calibrated color/exact-font comparison, browser execution, or production purchase/ad/equip-provider testing. No new pre-equipping, additional booster types, monetization, Failure continuation or SMS features. Existing deferred feature tasks preserved. CurrentTask and SessionLog updated; visual acceptance remains for user review.

## Changed production and test files

- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/PreLevelAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPreLevelView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPreLevelRuntime.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/PreLevel.prefab`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/BoosterSelectionAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/BoosterSelectionAlignmentEditModeTests.cs.meta`
- `Assets/LegendsOfRus/Tests/PlayMode/BoosterSelectionAlignmentPlayModeTests.cs.meta`
- `Assets/LegendsOfRus/Tests/PlayMode/BoosterSelectionAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionBoosterSelectionPrefabBuilder.cs.meta`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionBoosterSelectionPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionBoosterSelectionView.cs.meta`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionBoosterSelectionRuntime.cs.meta`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionBoosterSelectionRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionBoosterSelectionLayout.cs.meta`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionBoosterSelectionView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionBoosterSelectionLayout.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/BoosterSelectionAligned.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/BoosterSelect.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/BoosterSelect.prefab.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/BoosterSelectionAligned/panel.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/BoosterSelectionAligned/close.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/BoosterSelectionAligned/play.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/BoosterSelectionAligned/play.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/BoosterSelectionAligned/panel.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/BoosterSelectionAligned/close.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/BoosterSelectCutoverState.txt.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/BoosterSelectCutoverState.txt`
