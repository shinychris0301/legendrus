# Team Ranking R6 — Tabs and Season Status

Continued the next refinement documented after R5. Replaced four separate generic tab frames with source-derived joined Local/Global and Week/Season groups and their original ornate outer ends. Controls are still four real native buttons, with independent scope/period selection.

The original tab contours/rails come from the dedicated Team Ranking reference. Eight normal/active variants use approved Individual Ranking navy and sapphire face material with a restrained warm tint for selected scope. Russian/English labels remain live and localizable in the approved project font. Existing wording is preserved. The season availability note and sample-data label are arranged in two compact lines beneath the title; no source countdown or unconfirmed season dates are fabricated.

## Scope

Six existing Assets files modified: active TeamRanking prefab/cutover, owning reference layout, view, builder and PlayMode checks. Eight PNGs with new metas added; zero deletions. View/builder/prefab/cutover agree on internal revision7. Existing PNGs/metas/fonts and other page prefabs are unchanged according to SHA audit. R5 podiums, rewards, shared Home header, saved-resource values, table, Details and Back retain their behavior.

No Scenario generation or credit use: original decorative pixels and existing approved face material were available. Alpha was inspected over black, white and magenta. Text-cleared surfaces, source-traced contour and face texture are reconstructed; full pixel identity is not claimed.

## Verification

Final **Verify02**, Unity6000.5.9f1: rebuild/readback passed; **13 EditMode +15 PlayMode =28 passed**, zero failures/skips. Includes TeamHub and IndividualRanking regressions. Verify01 found the long Russian Global label too close to its tab edge. Increased its inset and separated status text boxes before the successful final run.

Checks cover actual tab glyph bounds/completeness, selected state, source-podium and reward clearance, row names, RU/EN layouts, simulated safe areas, button raycasts, read-only preview filters, single navigation commits and save preservation. Nine actual TeamRanking captures include reference, narrow and tall dimensions, both languages, Local/Week and modal states. Final RU close-up and narrow EN were visually inspected. Browser review:11 image views/comparison toggle passed at430×900, no script errors or horizontal overflow.

No Android build, physical-device check, performance run or full test suite. Remaining differences: live-font appearance, repaired tab surfaces/contours, additional preview disclosure, outer scenery and footer banners/candles/carving. The next recommended pass is footer ornamentation.

[Review](TeamRanking_TabsAndSeason_R6_Review.html) · [Fidelity](QA/ReferenceFidelityReview.md) · [Scope/checks](QA/ScopeAndVerification.json) · [Asset provenance](Sources/AssetProvenance.json)
