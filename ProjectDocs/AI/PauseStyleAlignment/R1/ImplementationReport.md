# Visual Page 10 — Pause R1

Implemented and verified in Unity **6000.5.9f1**. [Open the portrait review](Pause_Alignment_R1_Review.html) for actual Unity captures and the separate original reference. User visual acceptance is not assumed.

## What changed

The active `Resources/ProductionUI/Pages/Pause.prefab` now contains an ornate source-derived panel, original Russian title/action artwork and four compact booster cards. English uses repaired blank variants with live localizable labels. Goals support two or three missions and display the paused board's remaining amounts; booster counts reflect inventory. Cards show stock without consuming a booster while paused.

Four native buttons call the existing Resume, Restart, Settings and Exit routes. Modal transition blocking and board lifecycle remain intact; duplicate actions are gated. The background is the actual current Gameplay screen. No whole-screen reference or scenery-contaminated screenshot rectangles are used at runtime.

Installed 16 sprites, three runtime/view/layout classes, and two targeted test fixtures. Updated the active Pause prefab, owning Step20 builder, narrowly scoped importer, FIX01 dispatch and ScreenRouter control binding. Two Gameplay regression assertions were updated: the established current Pre-Level revision and the native Resume button replacing the obsolete hotspot. [Exact changed/new files](QA/SourceAudit.json) · [Existing-file diff](AppliedSourceChanges.diff).

## Extraction and Scenario usage

Five Scenario alpha jobs completed (title and four buttons), **15 creative units total**, with no resubmission or generative redraw. Their original RGB pixels are byte-identical to source crops; alpha incorporates local junction cleanup and opaque-face retention. Requests, masks, hashes and numeric billing are retained in Sources/Scenario.

The panel's obscured interior was reconstructed from clean source lacquer. English lettering removal uses source texture cloning and bounded Poisson blending. Booster count faces were repaired for live quantities; the original illustrations and gold rims remain. These reconstructed areas are not claimed recovered original pixels. All asset variants were inspected over black, white and magenta. Preparation: Sources/PrepareSprites.py; dependencies reused from the existing local R11 image environment.

## Checks performed

Final run4:

- Unity import/compilation, Pause-only rebuild and revision1 save/readback passed.
- **11/11 EditMode and 15/15 PlayMode tests passed**, zero failures/skips, covering Pause plus existing Gameplay and Failure fixtures. Saves/config were isolated from the user's storage.
- Verified two/three goals, RU→EN→RU switching, live stock, safe-area layouts, native-button raycasts, source aspect ratios, text fit, input blocking, resume preserving board/moves/score, settings Back returning to the same paused board, restart creating a fresh zero-score round, exit routing to ChapterMap, and double-action prevention.
- Saved six Pause, ten Failure and nineteen Gameplay PNG/JSON capture pairs. Visually inspected final reference-size RU, tall safe-area RU and tall three-goal EN; earlier run3 reference-size RU/EN were also inspected. Not every regression capture was visually inspected.
- All 16 installed sprite hashes match preparation. Five Russian sprite RGB checks match source exactly. Four Python scripts parse, changed/new C# whitespace is clean, and all seven review choices/local references resolve. Browser interaction was not run.
- Of 4,469 baseline project-owned files, 4,462 remain unchanged; seven existing files changed and 43 files were added, with no deletions. All **2,345 existing metas**, all **five original Pause prefab object IDs** and all **95 prior Failure R12 evidence hashes** remain unchanged. New prefab has141 objects;22 new GUIDs are unique. Final4,512 file hashes and81 current Unity evidence hashes recorded. No ProjectSettings/Packages git changes.

Earlier run1 caught a generic importer override and stale Pre-Level revision assertion; run2 caught goal-count overlap and a stale Resume-hotspot assertion. Run3 caught a Settings-return test calling general navigation instead of the actual Back control. These were corrected; run4 is the passing final evidence. Earlier XML results are retained in QA/EarlierRuns.

## Limits and next action

[Reference differences](ReferenceComparison.md) remain in reconstructed hidden surfaces, exact font identity, some fine divider ornamentation and the deliberately preserved current Gameplay behind the modal. This is not a claim of whole-page pixel equality.

Not performed: APK/AAB build, physical Android install/render/touch testing, full test suite, browser interaction, extended manual play, performance profiling, calibrated-color comparison, exact-font identification, or live purchase/ad integration. Paid Failure continuation remains scheduled after UI completion.

Next: review the actual Pause R1 portraits. Physical-phone readability and touch acceptance remain a separate device check.
