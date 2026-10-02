# Profile R5 — Relic Readability

User requested another Profile refinement after R4. Long relic names previously shrank to as little as 8 reference units to stay on one line. Captions now use intentional localized line breaks and a 15–18-unit range measured after canvas scaling. The compact row and expanded Relics grid both keep the text inside their existing lower card surfaces.

Russian compound names retain their hyphens. English names retain the approved wording; “Horn of Plenty” and “Word of Wisdom” use balanced two-line breaks. The selected-relic title remains continuous. Caption rectangles are 123 × 45 for the compact row and 146 × 54 for the grid. Existing fonts, icons, frames, button actions, preview semantics and saved data are reused.

Runtime/view/builder/active prefab/cutover use revision 6. The existing PlayMode layout checks also enforce the minimum caption size and clearance from the lower card edge. Existing rendered text width/height and input checks remain intact.

## Changed Assets files

- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/ProfileCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Profile.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionProfilePrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/ProfileAlignmentPlayModeTests.cs`

Seven existing Assets files changed; none added or deleted. Review evidence and handoff documents accompany these changes.

## Validation

Final run: `/tmp/lor-profile-relic-r5/Verify01`, Unity 6000.5.9f1. Import/rebuild succeeded; 3 EditMode and 6 PlayMode tests passed, zero failures or skips. Fifteen actual Unity captures cover Russian/English, simulated phone safe areas, secondary sections, modal, large saved values and collection selection. Visually inspected standard RU, tall EN and expanded Relics RU.

The standalone review has 17 successfully decoded browser views, no JavaScript errors or horizontal overflow at a 430-pixel viewport. SHA checks preserve all 5,575 protected existing artwork/font/meta/other-page prefab files. Standard RU/EN comparisons have zero changed pixels outside the relic caption areas. Checked Items RU and History RU captures are identical to R4. Scoped source whitespace checks passed.

No Scenario request or image editing was required: 0 calls, 0 CU. No Android build, physical-device, full-suite, backend or performance testing. Phone safe areas are simulated; the current font identity remains an approved approximation.

[Unity/scope evidence](QA/ScopeAndVerification.json) · [Pixel and file integrity](QA/IntegrityChecks.json) · [Browser results](QA/BrowserChecks.json) · [Fidelity](QA/ReferenceFidelityReview.md)
