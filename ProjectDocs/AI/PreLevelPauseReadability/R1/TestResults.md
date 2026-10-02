# PreLevel and Pause readability — R1 test results

2026-09-30. Continued the approved small-portrait Russian/English audit. Unity **6000.5.9f1**, actual canonical prefabs, disposable product configuration, previously authorized **process-only Vulkan**. Device execution remains postponed.

## Final results

| Check | Passed | Failed | Skipped | Evidence |
|---|---:|---:|---:|---|
| PreLevel and Pause Play Mode | 9 | 0 | 0 | [Final runtime summary](QA/MarginPlayMode/summary.json) |
| Canonical prefabs / Edit Mode | 5 | 0 | 0 | [Final prefab summary](QA/MarginEditMode/summary.json) |

These counts come from the final two runs, not accumulated iterations. Both specific owning prefab builders completed successfully. No Android build or device test ran.

Runtime checks cover:

- All 500 authored PreLevel goal/reward presentations and rendering without save writes; captured round 500 and claimed-reward states at 360×640.
- All four booster explanations in Russian and English, with three visible lines, no overflow, checked painted-button margins and no inventory spent by browsing help.
- 360×640, 941×1672, 576×1280 and 1080×2400 captures with simulated safe areas; both languages at 360×640 and reference size, plus tall three-goal Pause in English. Not every state was captured in every size/language combination.
- Energy gate, returning to Play without double spending, native help/catalog/header routes, empty and multi-digit booster stock.
- Pause goal counts, three-goal layouts, live English text fit, actions at least 44 rendered pixels tall in the tested sizes, raycast reachability and modal input blocking.
- Resume preserves board/moves/score; settings return preserves the paused round; restart/exit use existing routes and reject double actions.
- Required serialized bindings and revision agreement; retained isolated artwork, no duplicate native objects and truthful missing-round state.

## Installed changes

PreLevel revision **6**: selected-round caption 18→28 source units, claimed-reward status 16→26, help description 20→28 with a smaller adjacent icon. Larger help actions preserve original art; the catalog wording uses an explicit two-line layout. Text mesh checks use the actual aspect-preserved painted face rather than the wider Button rectangle. Existing wording and local fonts remain.

Pause revision **3**: panel maximum scale 1.0→1.3, still constrained by safe-area width/height. Existing action art scales uniformly; remaining-goal counts 25→28, with icon/count separation preserved. Original Russian extracted captions remain in use.

[One-portrait before/after review](PreLevel_Pause_Readability_R1_Review.html) contains actual Unity captures, not generated mockups or physical-phone evidence. Inspected small RU/EN, booster guide, claimed reward, round 500, three-goal Pause and tall guide captures. Confirmed no displayed text cuts into the help buttons' checked ornament margins.

## Iterations and limits

Baseline: 9/9 passed before adding the stronger phone checks. An initial test-code insertion used a camera variable outside the capture helper; corrected before runtime verification. Subsequent checks exposed the old geometric-center assumption, inconsistent catalog wrapping at small size and a one-pixel trim intrusion. Corrected the geometry assertion, explicit line break and caption position. Intermediate runs and logs are retained; their pass counts are not added to the final totals.

PreLevel's secondary help controls and some embedded/shared captions are still small. This is a targeted improvement, not a universal minimum text/touch-size claim or user visual acceptance. Physical-phone readability, touch accuracy, lifecycle and performance remain unverified. No gameplay rule, energy policy, reward, save-schema, backend, image pixels, fonts, packages or project graphics settings were changed. Hosting remains deferred and animation/effects remain last. No commit or deployment.

## Scope and next action

[Hash scope](QA/Scope.json): **11 existing files changed, 7,774 unchanged, 0 added/deleted** across Assets, Packages, ProjectSettings and Backend. Task-start snapshots and [task-only patch](Sources/Changes.patch) preserve unrelated working-tree changes. Python source syntax and changed-file whitespace checks passed; all 73 review capture links resolve locally.

Changed files:

- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Pause.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/PreLevel.prefab`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPauseLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPauseRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPauseView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPreLevelSourceControls.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPreLevelView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/PauseAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/PreLevelAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/PauseAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/PreLevelAlignmentEditModeTests.cs`

Next recommended bounded task: Settings and Language Select phone-size readability and control audit, including Pause → Settings → language selection → return. Preserve current art and save preferences; test Russian and English at small and tall portrait sizes. Actual phone execution remains deferred until requested.
