# Fresh page builder repairs R6 — 2026-10-01

Implemented the three fresh-construction repairs identified by R5. WorldMap and ChapterMap now obtain profile/settings controls, player labels, balances and plus controls from the cloned `ProductionPlayerHeader`'s serialized references. They no longer dereference obsolete Home resource paths. PreLevel now applies `ProductionPreLevelSourceControls.Apply` after its initial layout, just as its existing-content path does, before final validation.

The active prefabs already contain the accepted presentation. Their bytes, object IDs and serialized revisions are unchanged; the builders now produce that existing contract. No revision bump, prefab migration, artwork/import change, scene change or new startup system was needed. Current owner `PageNeedsBuild` checks remain false.

## Changes

- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionWorldMapRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionChapterMapRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPreLevelRuntime.cs`
- New `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/FreshPageBuilderEditModeTests.cs` and its `.meta`.

The six permanent regression cases check all three fresh pages against disposable installed-prefab instances: header sprites/colors/fonts/geometry and live values, public control geometry/raycast state, complete view/header bindings, revision agreement, page-owned references and distinct resource controls. The installed-path cases apply each builder twice and require all original components and button target references to survive. No Canvas/EventSystem may be added. [Exact production patch against task start](QA/ProductionChanges.patch).

## Verification

Unity **6000.5.9f1**, isolated test storage, existing bootstrap and router. Edit Mode uses batch/nographics; Play Mode uses the previously authorized process-only Vulkan option. Project graphics settings are unchanged.

| Run | Passed | Failed | Skipped | Result |
|---|---:|---:|---:|---|
| New regression cases before production edits | 3 | 3 | 0 | Reproduced WorldMap/ChapterMap null lookups and PreLevel incomplete composition |
| Final targeted Edit Mode | 15 | 0 | 0 | Six new cases plus nine existing WorldMap/ChapterMap/PreLevel alignment cases |
| Final targeted Play Mode | 18 | 0 | 0 | Existing page navigation, state, localization and presentation regressions |

Final acceptance: **33 distinct passing tests**, no failures/skips. The baseline failures are retained as reproduction evidence and are not added to final counts. [Baseline](QA/Baseline/summary.json), [Edit Mode](QA/EditVerification/summary.json), [Play Mode](QA/PlayVerification/summary.json); each directory contains full NUnit XML.

Runtime coverage includes shared header values, locked chapter behavior, native HUD routes, scrolling/localization, the 500-slot chapter map, selected-level goals/rewards, energy gating, booster descriptions/inventory and portrait/safe-area captures. The fixtures retained 62 actual Unity page PNGs. Visually inspected these three standalone captures:

- [WorldMap, tall Russian safe area](QA/PlayVerification/WORLDMAP/WorldMap_1080x2400_safearea_RU.png)
- [ChapterMap, Russian safe area](QA/PlayVerification/CHAPTERMAP/ChapterMap_576x1280_safearea_RU.png)
- [PreLevel, round 500 at 360 px](QA/PlayVerification/PRELEVEL/PreLevel_Level500_360_RU.png)

These are Editor captures of installed pages, not phone screenshots or pixel-difference proof for fresh pages. Fresh construction parity was checked structurally in Edit Mode. No broad full-project suite or physical-device acceptance is claimed.

## Scope and handoff

[Scope verification](QA/Scope.json): **7,771 baseline files unchanged, exactly three runtime files changed, two test files added, zero deleted** across Assets/Packages/ProjectSettings. All active prefabs, art/import metadata, packages/settings and prior local edits outside the three owners are unchanged. Generated test scenes absent at task start were removed by the runner. Raw Unity logs remain under `/tmp/lor-fresh-builders-r6/`, outside the authored patch.

The R2 APK remains **838,219,234 bytes**, SHA256 `480cb80ddeea115741fb25b779fed855da0b740cf3c55d88721c5b1dfd12057c`; it does not contain these new source repairs. No Android build/install, hosting/backend activation, commit or deployment occurred.

Next: implement the bounded dormant-progress dependency cleanup identified by R5. Preserve hidden Gameplay Image components and their fill-state semantics, align Home's legacy loader/validation and the fresh/in-place builders, and clear only the 11 target sprite references in six active prefabs. Verify rendering and gameplay progress before any GUID-preserving archive relocation; preserve the importer contract and measure a guarded APK afterward. No texture was removed in R6 and no new size saving is claimed. Approved Splash and original coin imports remain intact; phone work/hosting deferred and final effects last.
