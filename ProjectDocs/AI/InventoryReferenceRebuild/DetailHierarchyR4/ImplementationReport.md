# Inventory R4 — Detail hierarchy

The reference separates descriptive text from the usage note with an ornamental rule. R3 used a single paragraph block and lacked that separation. R4 gives the description and usage note independent live text regions and reuses the approved Mail parchment divider. The body text is larger, with separate fitting limits, while the original wording and unavailable Use behavior remain intact.

The divider follows the reference position and antique-gold style; it is an existing approved asset, not a new exact extraction from Inventory. No images or fonts were generated, edited or replaced, and no Scenario call was needed (0 CU).

## Implementation

The Inventory runtime builds the divider and UsageNote with serialized view bindings. Responsive layout moves both with the detail panel. The view updates both localized text sections when selecting items, switching categories or changing language. Revision5 is shared by the view, builder, rebuilt active prefab and cutover state.

The existing Play Mode layout verification now checks title/description/divider/usage/button clearance in every capture. An additional tall Russian capture covers the longest booster title. The same tests still exercise routing, saves, quantities, modal blocking, press feedback and localization.

## Remaining reference differences

This pass preserves factual preview/booster descriptions rather than introducing the reference's unimplemented crafting behavior. Text length therefore differs. The approved font is a fallback, not verified as the exact reference typeface. Alternate selected-item artwork still uses the existing card frame, and reconstructed panel surfaces and tall-screen spacing remain adaptations.

Final Verify02 passed 2 Edit Mode and 8 Play Mode tests (10 total). The review contains 18 actual Unity captures and 20 browser-verified views. Default Russian, tall English and tall Russian Pearl captures were visually examined. Seven existing Assets files changed, with no additions/deletions. All 5,615 protected art/meta/font/other-page prefabs remain unchanged; three matching R3 comparisons differ only within the detail-text region. Scoped source/cutover whitespace checks passed. Verify01 also passed automated tests; visual inspection prompted the divider-height correction verified in Verify02. Full results are recorded in QA/ScopeAndVerification.json, QA/IntegrityChecks.json and QA/BrowserChecks.json. The review distinguishes actual Unity captures from the original and prior-version images. No Android/device/full-suite/backend/performance validation is claimed.
