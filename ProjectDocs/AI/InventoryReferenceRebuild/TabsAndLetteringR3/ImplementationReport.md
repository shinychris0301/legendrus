# Inventory R3 — Tabs and original Details lettering

The generic category tabs have been replaced with isolated original frames and Russian lettering. Tab geometry follows the source image, including the Artifacts finial. Active/inactive sprite states preserve interactive selection. The Details button now includes its complete original Russian lettering; English uses repaired blank faces with live localized labels. Use remains visibly disabled because collectible use is unavailable.

## Art provenance

One Scenario Pixa background-removal request processed a four-tab sheet, at an actual cost of **3 CU**; no retries or new RGB generation. Original source RGB is restored under the resulting alpha, with traced contour limits and opaque panel interiors. The source-state Russian tabs retain original RGB; alternate states adjust only the face while preserving classified gold strokes. The original shows Artifacts active and the other three inactive, so alternate states are adaptations.

Details uses the original source crop at uniform 3× scale under the previously accepted alpha. Its English face uses a bounded repair with mirrored source-blue texture, retaining the surrounding decoration. English tab faces reuse approved navy/blue texture. These repairs are documented in `Sources/Finalize.py`, masks and `AssetProvenance.json`; they are not claims of exact recovery of hidden pixels. Black, white and magenta composites were visually inspected.

## Native changes

Six existing Assets files changed: Inventory runtime, view, builder, prefab, cutover state and Play Mode tests. Eighteen PNG/meta pairs were added under InventoryAligned: sixteen tab variants and two Details variants. No existing asset was deleted or overwritten. Runtime/builder/prefab/cutover agree on revision **4**. Existing shared header, footer routes, approved booster artwork, saved quantities, preview semantics and press feedback are preserved.

## Verification

Unity 6000.5.9f1 rebuilt the actual prefab and passed **2 Edit Mode + 8 Play Mode tests** in `/tmp/lor-inventory-tabs-r3/Verify01`. New coverage selects every category in both languages and verifies its state sprite, localized-label visibility, Details art and disabled Use. Existing coverage checks routing, real quantities without consumption, save preservation, modal input blocking, press feedback, text fit and safe areas.

The review contains **17 actual Unity captures**, plus separately labelled original and R2 views. Browser checks passed all **19 views** at a 430px viewport. Native default Russian, tall English and both selected-Materials captures were visually examined. SHA checks preserve all **5,579 existing protected art/meta/font/other-page prefab files**. Three matching R2/R3 captures have zero changed pixels outside tabs and Details. Scoped source/cutover whitespace check passed; generated prefab YAML was excluded.

No Android build, physical-phone test, full test suite, backend or performance test was run. See `QA/ScopeAndVerification.json`, `QA/IntegrityChecks.json` and captured test reports for evidence.

## Remaining work

Review the complete Inventory page before starting another refinement. Live description typography, selected-item framing, reconstructed surfaces and tall-screen spacing remain adaptations. Settings is the next page in the established sequence once Inventory is accepted.
