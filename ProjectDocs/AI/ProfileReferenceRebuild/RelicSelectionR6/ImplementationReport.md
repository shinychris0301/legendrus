# Profile R6 — Relic Selection and Detail

User requested the next Profile refinement after R5. The inspected relic now has a sapphire name accent in the compact row and expanded grid. This identifies the current preview; it does not indicate equipment or ownership. All other captions remain gold, and no overlay covers the source artwork or decorative borders.

The five non-sword detail cards now display their approved localized captions in the existing blank lower card surface. Their caption bounds follow the visible, aspect-fitted sprite and its top-left pivot. The approved sword detail extraction remains intact. Detail headings use the existing FreeSerif title font with uppercase Russian/English wording, closer to the reference hierarchy. Exact typeface identity remains unverified.

Runtime, view, owning builder, active Profile prefab and cutover state use revision 7. Existing source artwork, fonts, shared header, real saved statistics, preview notices, routing and save behavior are retained. No image extraction or generation was required: 0 Scenario calls, 0 CU.

## Verification record

Verify01 passed 3 EditMode and 7 PlayMode tests but visual inspection caught the new detail caption centered in the wider image container rather than the top-left-aligned visible sprite. Corrected the pivot calculation and added a geometry assertion before the final run. Final Verify02 completed successfully: import/rebuild, 3 EditMode tests and 7 PlayMode tests passed, zero failures or skips. Evidence is in QA/ScopeAndVerification.json.

The added PlayMode test traverses all six selections in both languages, captures standard Russian and tall English layouts, verifies only one sapphire caption, matches detail/card wording, confirms the heading font and retains selection when changing collection layout. Existing save-integrity, routing, text-fit, safe-area and input checks remain.

No Android build, physical-device, full-suite, backend or performance verification is claimed. Collection ownership and upgrades remain labelled previews. Selection accent and alternate relic detail views are native adaptations; the reference supplies only the sword detail state.

## Final evidence

29 actual Unity Play Mode captures include all six selections in both languages and both collection layouts. Visually inspected final Russian book selection and tall English egg selection; the detail captions follow the visible frames. The review also offers the original reference and previous R5 capture as separately labelled views.

SHA checks preserve 5,575 existing PNG/font/meta/other-page prefab files. Header/title top 217 pixels are unchanged in four matched captures. Checked Items RU and History RU captures are identical to R5. Scoped source whitespace checks passed. Temporary test saves were used.

## Changed Assets files

- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/ProfileCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Profile.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionProfilePrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/ProfileAlignmentPlayModeTests.cs`

Six existing Assets files changed; none added or deleted. Documentation, verification scripts and captures accompany the implementation.

[Scope and Unity results](QA/ScopeAndVerification.json) · [Integrity](QA/IntegrityChecks.json) · [Browser verification](QA/BrowserChecks.json) · [Fidelity](QA/ReferenceFidelityReview.md)

Browser validation: all 31 review views decoded at a 430-pixel viewport, with no JavaScript errors or horizontal overflow.
