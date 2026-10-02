# Failure original Russian lettering — R12

Implemented and verified in Unity 6000.5.9f1 after the user closed the Editor. [Portrait review](Fail_OriginalLettering_R12_Review.html) contains actual Unity captures and separate reference/before choices. Visual acceptance is still for the user to assess.

## Implementation

The user approved retaining fixed Russian text with extracted artwork, a specific exception to the earlier live-only label rule. Six new resources preserve original source RGB exactly: title with normal failure subtitle, retry button, and four distinct booster nameplates. Existing Scenario-derived masks were reused; two plaque outlines required local alpha corrections. No new Scenario/API requests or creative-unit charges.

Russian selects these variants and suppresses duplicate live lettering. English uses blank R11 sprites and existing localizable text. Russian without a failed round uses the blank title and truthful live no-result subtitle. Quantities, goals, currency, price, availability and navigation remain live. Paid/ad continuation remains unavailable and scheduled under FAIL-CONT-01 after UI approval. Scene, hero, main frame, layout and other pages retain the R11 state.

Changed existing production files: Failure prefab, its owning Step20 serialized builder, ProductionFailRuntime, ProductionFailView, and Failure EditMode/PlayMode fixtures. Added six PNGs and their six metas in Resources/ProductionUI/FailAligned. Prefab and code revision are 12. See [exact source diff](AppliedSourceChanges.diff) and [file audit](QA/SourceAudit.json).

## Verification performed

- Unity import/compile and Failure-only canonical prefab rebuild/readback passed at revision 12. Earlier run1 exited before import/compilation because the Editor held the project lock; run2 completed after closure.
- Targeted Failure, Victory and Pre-Level fixtures: 10/10 EditMode and 18/18 PlayMode tests passed, zero failures/skips, isolated save/config storage. Includes RU→EN→RU selection, duplicate-text suppression, live quantities, no-result fallback, existing routing/data/safe-area assertions. [Results](QA/UnityResults.json).
- Saved 10 Failure, 12 Victory and 20 Pre-Level PNG/JSON capture pairs. Visually inspected five final Failure captures: reference-size RU and EN, RU no-result, tall safe-area RU, and tall three-goal EN. Did not visually inspect every regression capture.
- All six extracted sprites have byte-identical source RGB and installed hashes match the preparation audit. All six were inspected over black, white and magenta during preparation.
- 1,865 baseline production files: 1,859 unchanged, six changed; 12 new sprite/meta files. All 963 existing metas and 511 prefab object IDs preserved. Six new GUIDs unique. All 98 prior R11 evidence hashes preserved. Final 1,877 production hashes verified and 95 current Unity evidence hashes recorded.
- Three Python scripts parsed; changed C# files have no trailing whitespace. All 12 HTML portrait choices and 17 local path references resolve. No ProjectSettings/Packages git changes. [Delivery checks](QA/DeliveryChecks.json).

## Limits and next action

Original lettering is preserved as pixels, not an identified editable font. Alpha edges still require interpretation of a flattened source; hidden edges and the whole page are not claimed pixel-perfect. English typography, scenery, Ivan, header and main-panel ornamentation retain documented differences. No APK/AAB build, physical Android test, full suite, browser interaction, extended manual gameplay, profiling, live purchase/ad integration, exact-font identification or calibrated-color test was performed.

Review R12's actual portraits next; phone readability/touch verification remains a separate device check. Earlier page approvals and feature deferrals remain unchanged.
