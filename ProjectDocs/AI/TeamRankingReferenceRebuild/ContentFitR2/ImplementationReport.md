# Team Ranking R2 — Ribbon and Reward Fit

User reported podium names obscuring ribbons, podium rewards covering decoration, and ranking rewards crossing the highlighted border. This pass changes native layout bounds, not the extracted artwork.

## Changes

- Podium names now sit inside the flat ribbon face with 32–39 reference-pixel side insets and a 32-pixel label height. Smaller localized font fitting keeps the ribbon folds visible.
- Podium reward cells have explicit internal padding. Icons and counts occupy separate rectangles, with room above and below the decoration. Counts use 20-point maximum size.
- Ranking reward slots now have 12 pixels of clearance from the 86-pixel row's top and bottom. Smaller icons and counts sit inside each slot; they no longer cover the highlighted row's gold rails.
- Preserved approved sprites, selected fonts, shared header, sample values, navigation and native controls.
- Real prefab, owning layout, view, builder and cutover updated together to internal revision 3.

## Verification

Unity 6000.5.9f1 Verify01: prefab rebuild/readback succeeded; 11 Edit Mode and 15 Play Mode tests passed, zero failed/skipped. Expanded capture checks assert icons/counts stay inside reward slots, do not overlap each other, and keep slots clear of row borders. Existing checks cover safe areas, labels, hit targets, localization, routes, idempotency and save preservation. Team Hub and Individual Ranking regressions passed.

Nine actual native Team Ranking captures retained, including Russian/English, narrow safe areas, Details, selected filters and maximum balances. Full Russian and narrow English captures visually inspected against R1 and the original. Eleven review views/comparison toggle passed browser checks.

Six existing Assets files changed, no additions/deletions. All PNGs/metas/fonts/other page prefabs unchanged. See `QA/ScopeAndVerification.json` and `QA/PreservationChecks.json`. No image regeneration, Scenario calls or credits. No Android/device/performance/full-suite testing.
