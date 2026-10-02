# Chapter Map — original controls and background R1

Implemented in the active native ChapterMap prefab (revision2), not just an HTML preview. The user approved the assessment recommendation and then explicitly requested matching the original background in this pass.

## Delivered

- Shared `ProductionPlayerHeader` with Home and WorldMap artwork, geometry, fonts, saved balances and routes. ChapterMap now uses the same component; legacy header rendering is disabled.
- Original Russian banner and Play lettering preserved together with their panels. English uses repaired blank surfaces and live localized labels. Extracted Back arrow, level medallion frame, full/empty stars and bottom carving are separate sprites.
- Native level numbers, lock/selection state and earned-star values remain save-driven. The ten authored levels and existing Story/PreLevel, Back, Profile, Settings and resource routes remain intact.
- Clean scenery retains original visible church, windmill, houses, river and much of the path. Scenario reconstructed the regions concealed by interface. Local blending repaired their seams; 822,875 pixels outside the explicit repair support remain exactly equal to the original image. The background contains no baked-in functional controls, numbers or stars.
- Foreground and main scenery share the reference-sized safe stage. A dimmed scenery layer covers surplus display margins without stretching the mapped main scene.

## Scenario usage and provenance

Seven Pixa alpha extractions:21CU. One original-guided scenery reconstruction:13CU. **Eight accepted jobs,34CU total; no duplicate submissions.** Original RGB restored beneath masks; local cleanup removed neighboring node/frame fragments, preserved opaque faces and removed static numbers for native text. English and numeral repair masks are archived. See `Sources/AssetProvenance.json`, `Sources/EnvironmentProvenance.json`, `Sources/Scenario/` and `QA/AlphaContactSheet.jpg`.

The banner, Play, Back and footer RGB exactly match their source crops. English/numeral assets retain source RGB outside documented repair masks. This does not mean the regenerated hidden scenery, English surfaces, font or fine alpha edges are exact reconstructions.

## Verification

Unity6000.5.9f1 compilation, targeted rebuild and serialized readback passed. Final `QA/Verify03`: **12 EditMode +20 PlayMode =32 passed,0failed/skipped** across ChapterMap, Home and WorldMap. Checks cover bindings, whole-sprite imports, original/shared resources, real progression/stars/locks, routes, control raycasts, RU/EN switching, English Play draw order, safe areas and node-star separation. No APK, physical-device or full-suite run.

Actual Russian, English, tall-phone, larger-safe-area, language-roundtrip and earned-star fixtures captured. Russian, English, tall phone and progress visuals inspected during the pass; clean scenery independently inspected without interface. Alpha inspected over black, white and magenta. Six review-selector images load at390px without horizontal overflow. Source/pixel/mask/Python-parse audit passed. Final Home and WorldMap Russian captures are pixel-identical to SharedHeaderR1 captures.

The first32checks passed before visual inspection discovered the English Play artwork obscuring its label. Fixed sibling ordering and added an assertion; Verify02 and final Verify03 both pass. Earlier captures/results retained as history. Background infill initially exposed sky seams and a small Play-frame fragment; local mask expansion and excluding that overlap from the last-node seam blend corrected them before final import.

## Scope and remaining work

All614 existing ChapterMap prefab records retained;751 after additive integration. No other existing page prefab or artwork changed. Existing startup, services, saves, gameplay, project settings and package configuration preserved. All changed/new production files are listed below and in `QA/ChangeAudit.json`.

**Next refinement: scrolling level path and reference-like node spacing.** The ten-node static arrangement and small lock/selection rendering remain from the existing implementation; they differ from the reference's seven visible example nodes. Original sample numbering10–16 was deliberately not treated as authored gameplay data. The selected live number/header fonts remain approximations. English face cleanup, fine sprite edges and reconstructed hidden scenery remain reviewable differences. User visual acceptance is still pending.

## Files changed in this task

- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/ChapterMapAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/ChapterMapAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionChapterMapRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionChapterMapView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionNavigationStyle.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPlayerHeader.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionChapterMapLayout.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/ChapterMap.prefab`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionChapterMapSourceControls.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionChapterMapSourceControls.cs.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/star_full.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/footer.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/banner_blank.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/environment.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/banner.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/back.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/footer.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/node_blank.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/star_full.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/banner.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/play_blank.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/node_blank.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/star_empty.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/environment.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/banner_blank.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/back.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/play.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/play.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/play_blank.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/star_empty.png.meta`

Post-test cleanup: normalized only empty `m_Name:` trailing spaces in the saved prefab; scoped whitespace check passed. No semantic changes after final verification.
