# Booster Selection — reference rebuild R1

Delivered 2026-09-23. The user approved four working boosters plus four clearly labelled previews. The real BoosterSelect prefab, runtime, layout and owning builder now use native revision 2.

## Result

The original navy/gold panel perimeter, divider, Russian title and complete ИГРАТЬ button lettering are extracted from `match3final/booster select.png`. The panel presents four columns in two rows. The upper row retains the four supported tools, live inventory counts, inspection and accurate effect descriptions. The lower row displays the reference’s Luck Scroll, Lightning Orb, Veles Bomb and Swap artwork, with localized “Preview / Not available yet” labels. These previews have no buttons, inventory counts, purchase controls or equip toggles.

Russian static title/button lettering uses source sprites; English and dynamic content remain live. Existing Close, selected-level Play, energy gate and standalone routes are preserved. Inspecting a tool does not spend inventory; Play spends energy once through the existing flow. Boosters are still used during a round. The modal retains the actual underlying Pre-Level page and shared player header.

## Asset workflow

Three accepted Scenario Pixa background-removal jobs: panel, Play button and combined preview atlas, 3 CU each, **9 CU total**, no paid retries. Existing approved Scenario blank navy/emerald materials are reused for covered surfaces; no new RGB generation was requested. Source RGB is retained beneath the accepted alpha. Preview inventory/plus markings were removed using matching unobstructed rim donors within bounded masks; those repaired rim pixels are approximate.

The Russian title and Play button preserve **68,947 visible source pixels with zero RGB mismatches**, as recorded in [OriginalRGBCheck.json](QA/OriginalRGBCheck.json). This measurement concerns those two assets, not the whole page. Transparent resources were inspected on black, white and magenta. Intentionally opaque panel surfaces remain opaque. Upper ornament overlap, old button perimeter residue in the panel and the Play button’s lower gem were corrected before the final verification.

## Verification

Final **Verify05**, Unity **6000.5.9f1**:

- Prefab rebuild/import and serialized readback succeeded.
- **8 EditMode + 12 PlayMode tests passed**, covering Booster Selection and Out of Energy regression checks.
- Checks include repeated-build stability, source/live language states, preview noninteraction, text/layout clearance, actual stock, no stock spending during inspection, single energy spending, empty energy routing, Close/reopen and standalone routes.
- Native Russian/English captures, tall safe-area layouts, empty inventory and standalone state are included in the review. Final Russian and tall English captures were visually inspected.
- Browser review: **9 views**, no browser errors or horizontal overflow.
- Scoped source whitespace checks passed.

These are actual Unity captures with isolated test saves. No Android build, physical-device run, full test suite or new booster backend implementation was performed.

## Changed files

Eight existing Assets files changed: `ProductionBoosterSelectionRuntime.cs`, `ProductionBoosterSelectionView.cs`, `ProductionBoosterSelectionLayout.cs`, `ProductionBoosterSelectionPrefabBuilder.cs`, `BoosterSelect.prefab`, its cutover-state file and the two Booster Selection test files. Eight new sprites and their metas were added under `ProductionUI/BoosterSelectionAligned/`. No Assets files were deleted. All **147** previous prefab records are retained (**199** total). Existing sprite/meta files and other page prefabs were not changed. The builder no longer rebuilds the unrelated Pre-Level page as a side effect.

See [ScopeAndVerification.json](QA/ScopeAndVerification.json) for exact paths and results, and [AssetProvenance.json](Sources/AssetProvenance.json) for source and Scenario records.

## Remaining reference differences

This is not a pixel-identical copy. The four working tools retain established artwork, names and functions rather than claiming the reference’s eight equipped tools are implemented. The real Pre-Level underlay differs from the reference’s older Gameplay underlay. Hidden navy/emerald surfaces, preview rim repairs, fine alpha and live typography remain approximations; exact source font identity is still unverified. Preview labels and instructions intentionally replace unsupported inventory/purchase/equip UI. The Play button is slightly raised to preserve lower ornament clearance.

Next: visual review, then **Visual Page 18 — Events reference rebuild** if continuing.
