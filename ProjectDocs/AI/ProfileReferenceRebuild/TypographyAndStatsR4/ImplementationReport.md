# Profile R4 — Typography and Stats

Implemented the approved next refinement in the native Profile page. Revision 5 agrees between view, builder, active prefab and cutover state.

## Changes

- Six saved-stat values use the existing approved Bitter numeral font. Player names use Roboto Serif; section/tab titles use FreeSerif Bold. Exact source font identity remains unverified. Font assets are unchanged and no synthetic bold is added.
- Stat icons are 45 × 45. Labels and values share an inset text column, with separate 24- and 28-unit text areas. Labels use a quieter gold; values remain brighter. Adaptive sizing accommodates long Russian labels and large saved numbers.
- Reduced tab, hero-caption, identity, footer and detail text sizes; preview notes are subordinate to collection headings. Existing post-layout relic-caption fitting is retained.
- Profile refresh respects the shared header's font ownership. All live statistics, preview wording, routes, inventory quantities and history behavior are retained.

## Changed Assets files

1. `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileRuntime.cs`
2. `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileView.cs`
3. `Assets/LegendsOfRus/Scripts/Editor/ProductionProfilePrefabBuilder.cs`
4. `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Profile.prefab`
5. `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/ProfileCutoverState.txt`
6. `Assets/LegendsOfRus/Tests/PlayMode/ProfileAlignmentPlayModeTests.cs`

No Assets additions or deletions. Review, scripts, evidence and handoff documentation accompany these changes. The PlayMode layout check now also verifies numeral-font persistence and label/value separation.

## Verification

Final run: `/tmp/lor-profile-type-r4/Verify01`, Unity 6000.5.9f1.

- Import/rebuild completed successfully; 3 EditMode and 6 PlayMode tests passed, zero failed or skipped.
- 15 actual Unity Play Mode captures cover Russian, English, simulated phone safe areas, secondary sections, detail modal and maximum saved values. Visually inspected standard Russian, tall English and maximum-value captures against the original reference.
- All 17 browser review views decoded at a 430-pixel viewport; no JavaScript errors or horizontal overflow.
- SHA checks preserve 5,575 existing protected files: PNGs, metadata, font files and other-page prefabs. Header/title pixels through row 216 are unchanged from R3 in four matched captures.
- Scoped `git diff --check` passed for runtime, view, builder and PlayMode test source.
- Initial auxiliary audits could not find Pillow/NumPy or Playwright in temporary environments. Installed audit dependencies outside the repository and reran successfully; no Unity rerun was necessary.

No Android build, physical-device, full-suite, backend or performance verification. Safe areas are simulated. No image editing, extraction or Scenario request was needed: 0 requests, 0 CU. Approved artwork is reused unchanged.

See [scope and Unity results](QA/ScopeAndVerification.json), [integrity](QA/IntegrityChecks.json), [browser checks](QA/BrowserChecks.json) and [remaining differences](QA/ReferenceFidelityReview.md).
