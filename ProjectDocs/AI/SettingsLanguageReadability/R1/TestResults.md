# Settings and Language Select readability — R1 results

2026-09-30. User approved the next small-portrait audit. Unity **6000.5.9f1**, canonical prefabs, isolated disposable product storage and previously authorized **process-only Vulkan**. No physical-phone execution.

## Final verification

| Checks | Passed | Failed | Skipped | Evidence |
|---|---:|---:|---:|---|
| Settings and Language Select Play Mode | 13 | 0 | 0 | [Runtime summary](QA/CaptionFitPlayMode/summary.json) |
| Canonical prefabs and idempotent runtime builders | 4 | 0 | 0 | [Edit Mode summary](QA/FinalEditMode/summary.json) |

Final totals are **17 passed, 0 failed/skipped** from the two final runs only. Both owning prefab builders completed successfully.

- Russian/English Settings and language dialogs captured at **360×640**, reference portrait, and tall safe-area sizes. All six existing general information variants checked at 360×640 in both languages for rendered text containment and close behavior.
- Settings toggle and language dialog targets meet **44 rendered pixels** in the tested small portrait. Expanded toggle targets stay separate; raycasts at their edges resolve to the intended controls. Preference rows remain on the parchment surface.
- Native language selection at 360×640 from Splash, Home and Settings persists the selected language, preserves progress, and closes back to its origin exactly once.
- All four preferences persist independently, refresh when returning and preserve currency. Rebinding controls does not duplicate actions.
- Pause → Settings → language selection → Settings → Pause → resume preserves the same board, moves and score; board input remains blocked while paused. Small English captures from paused gameplay included.
- Shared-header routes, press feedback, overlay blocking, safe edges, unavailable-service explanations, consent-state preservation and Application.version remain verified by the selected fixtures.

## Implemented

**Settings revision5:** redistributed preference rows and smaller title/footer navigation art to accommodate taller, separate toggle hit areas. Larger preference, availability, version and general explanation captions. Wider local-save caption column prevents unnecessary shrinking in Russian. Larger general-information Close control. PreferencesPaper now uses nine-slicing with preserved corner regions; only its sprite-border metadata changed, not image pixels.

**Language Select revision4:** larger dialog and choices, both standalone and inside Settings; expanded close target. Retains original isolated panel/button artwork and the unchanged approved Splash backdrop.

Owning builders update existing hierarchies instead of deleting/recreating them. [Serialized identity comparison](QA/SerializedObjectIds.json): all **389 Settings** and **170 Language Select** object IDs preserved, with no added or removed IDs. Cutover revisions match runtime revisions. Existing fonts, source lettering and service semantics retained.

[Before/after portrait review](Settings_Language_Readability_R1_Review.html) shows actual Unity captures, one portrait at a time. All109 capture links resolve locally. Baseline 360 captures are Russian; the baseline tests stopped before their English small-size captures because of the undersized targets. Other baseline states retain their actual recorded sizes. Reviewed final small Russian/English pages, both language dialog contexts, explanation overlays and tall Settings.

## Findings and corrections

Baseline: **10 passed, 2 failed**. New360 captures exposed existing minimum-target violations for Settings switches and standalone language controls. Initial revised runtime run13/13 passed, but visual inspection caught the aspect-preserved parchment failing to fill its taller rectangle; corrected via nine-slicing and added row containment checks. A subsequent run10/13 exposed the Russian local-save caption shrinking below its fitted-size range; widened its column. Final rerun13/13 passed. Earlier pass counts are not accumulated.

Initial whole-working-tree whitespace check reported historical Unity YAML blank-value spaces. The task-only check against retained start snapshots passes; unrelated prefab formatting was preserved. Python runner/review/audit syntax checks pass.

## Scope and limits

[Hash scope](QA/Scope.json): **15 existing files changed, 7,770 unchanged, 0 added/deleted** across Assets, Packages, ProjectSettings and Backend. [Task-only patch](Sources/Changes.patch) and task-start snapshots retained.

- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/LanguageSelectCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/SettingsCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/LanguageSelect.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Settings.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/SettingsAligned/paper.png.meta`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionLanguageSelectPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionSettingsPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionLanguageSelectLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionLanguageSelectRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionLanguageSelectView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionSettingsLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionSettingsRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionSettingsView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/LanguageSelectAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/SettingsAlignmentPlayModeTests.cs`

This is targeted Editor verification, not whole-app visual acceptance or physical-phone testing. Some shared captions and Settings navigation rows remain below a universal minimum text/touch size. No image generation, pixel edits, font replacement, approved Splash changes, gameplay-rule/reward/save-schema changes, backend activation, project graphics settings changes, APK build/install, commit or deployment. Physical-phone lifecycle/readability/performance, human31–500 difficulty/pacing and optional recap remain open. Hosting is deferred; animation/effects stay last.

**Next:** one clean broad Play Mode regression run against the current project after the recent caption/control corrections, using the established isolated phase configuration and reporting any prerequisite-dependent skips separately. Phone execution remains deferred until requested.
