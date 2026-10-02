# Inventory R2 — Original Grid and Quantity Frames

The generic ranking enclosure and rectangular quantity row are replaced by original Inventory carving and plaque artwork. The grid retains source gold/wood perimeter pixels; its occupied interior uses approved clean navy material. A small lower-rail section covered by the neighboring feather crest is continued from unobstructed source rail. The quantity plaque retains source edging with its baked text removed from the opaque inner face, so quantities remain live.

One Scenario Pixa background-removal sheet request was used (3CU, no retries or RGB generation). The submitted sheet, job, billing, returned alpha and local refinement scripts are retained. The first narrow source-gap donor produced repeated gold fragments and was rejected for the final grid face. The corrected face uses approved Individual Ranking navy. Intentional panel interiors remain opaque; bounded alpha support removes neighboring medallion/corner residue. Quantity artwork is uniformly enlarged3× for extraction, not claimed to gain original detail.

The preview label now sits inside the grid below its top carving. Collectible rows are adjusted to clear both the label and bottom ornament, while retaining5×4 order. Quantity label/value geometry follows the new plaque. Shared header/footer, approved item and booster illustrations, selection behavior, saved quantities and truthful preview semantics are preserved. Native revision3 agrees across builder/view/prefab/cutover after rebuild.

## Verification

Final run `/tmp/lor-inventory-grid-r2/Verify01`, Unity6000.5.9f1: import/rebuild/readback succeeded;2EditMode+7PlayMode=9passed,0failed/skipped. Updated assertions check preview-label/card separation, clearance from grid sides/lower carving, source frame bindings, and separate quantity title/value bounds. Existing maximum quantities, localization, safe areas, shared-art identities, selection, press feedback, routing and save-preservation tests passed.

15actual Unity captures were produced.17review views decoded at430px without browser errors or horizontal overflow. Visually inspected defaultRU, maximum-booster quantitiesRU and tallEN, plus both new sprites over black/white/magenta. Source RGB outside the final navy-face repair mask matches the prepared source input exactly. Local contour/hidden-surface repairs remain approximate.

All5,575 pre-existing artwork/font/meta/other-page resources retain their hashes. Three matched native captures differ only inside the grid and quantity areas; header/title/tabs and footer are pixel-identical toR1. Source/cutover whitespace passed; generated YAML formatting preserved. No Android, physical-device, full-suite, backend or performance checks.

## Changed production files


- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/InventoryCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Inventory.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionInventoryPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionInventoryLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionInventoryRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionInventoryView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/InventoryAlignmentPlayModeTests.cs`

Added two sprites and their Unity metadata:

- `Assets/LegendsOfRus/Resources/ProductionUI/InventoryAligned/grid_frame_r2.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/InventoryAligned/grid_frame_r2.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/InventoryAligned/quantity_frame_r2.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/InventoryAligned/quantity_frame_r2.png.meta`

No deletion or overwrite of approved prior artwork. Review scripts, source masks, alpha inspections, API evidence and handoff records accompany the change.

[Scope/results](QA/ScopeAndVerification.json) · [Integrity](QA/IntegrityChecks.json) · [Asset provenance](Sources/AssetProvenance.json) · [Fidelity](QA/ReferenceFidelityReview.md)
