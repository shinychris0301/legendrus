# Friends List — Footer Symbols R7

Approved scope: remove the copied Map selection arc, fit complete footer symbols, restore World Map medallion scale and improve caption clearance.

## Changes

- Retains the exact World Map gold/active-blue frame sprites and complete Shop icon. Uses the approved World Map navy card surface for neutral inset backing. Map no longer copies the active Map icon face; Friends alone retains the active outer rim.
- Four imagegen-assisted transparent symbol adaptations (map, event scroll, friends, helmet), saved as `Assets/LegendsOfRus/Resources/ProductionUI/FriendsListAligned/symbol_*_r7.png`. They reconstruct small details and are not claimed as pixel-identical source cuts. No original image was overwritten. The built-in imagegen tool was used in four edit calls; exact prompts/output provenance are in `Sources/ImagePrompts.json`.
- The original generated PNG alpha/RGB is preserved. Native Image controls draw complete sprites at uniform scale using visible-alpha bounds for alignment; no silhouette clipping or stretching. Rendering uses a 0.76 RGB tint to suit the dark approved palette. The helmet includes its full pointed top and mail skirt; both event-scroll rollers remain visible.
- All five medallions now use 236px reference height, matching World Map. Friends/request rows scale uniformly to 94% and remain centered; lower controls/frame bottom move up 62px to clear the footer. Safe-bottom clearance remains 6px. This small row reduction is an explicit space tradeoff.
- Live RU/EN captions use the existing approved player-name font, with inset margins and scale-aware size fitting. Exact source-font identity remains unverified.
- Runtime, builder, active prefab and cutover revision are 8. All five routes and the read-only sample friend/request behavior remain intact.

## Verification

Final **Verify02 passed 15 Unity checks (2 Edit Mode + 13 Play Mode)**, covering bindings, all five routes, localization, safe areas, native symbol rendering, uniform aspect and silhouette clearance; World Map regressions also passed. Fourteen native Friends captures retained. Reference RU and small tall EN final captures inspected; all nine HTML capture views loaded without errors/overflow. Verify01 also passed before the final helmet/caption refinement.

Scope: eight existing Assets files changed, four PNG/meta pairs added, no deletions. All previous PNGs/fonts/World Map prefab and frame resources remain byte-identical.

Transparent output inspected over black, white and saturated magenta (`AssetReview.html`, `QA/Alpha*.png`). No visible rectangular background, old circular rim or selection halo remained in the isolated symbols. Some near-zero-alpha generated pixels exist outside the main silhouette; the source alpha is preserved. Alignment bounds use alpha above 8/255.

No Android build, physical-device, backend, performance or full-project suite run. Existing background/live-typography differences and space beneath finite sample rows on tall devices remain outside this footer pass.
