# Team Hub rebuild R1 — fidelity review

Compared the dedicated Guild Hub original with final Verify03 native Russian and tall-phone English captures. Navigation and resource consistency improved, but the full page is **not an exact replica**.

## Detailed recheck requested by user — 2026-09-23

Re-examined the original and the delivered Verify03 Russian 941×1672 Unity capture at the same dimensions, then checked owning layout/resource bindings. Original reference SHA, all ten new sprite hashes and the reused-resource hashes still match the delivery records. This was a visual/source review, not a new Unity test run. The previous 14 passing tests establish scoped behavior and layout checks, not exact artwork fidelity.

The page matches the broad composition and its four Russian navigation controls closely. It does **not** match the original overall. The most visible remaining discrepancies, in priority order:

1. **Guild summary and chest enclosure.** The main chest lacks its ornate circular surround and attached name plaque. The smaller chest is a scaled reuse of the same chest, while the reference has a distinct angled presentation. The membership group icon is missing. The guild-name and power faces have visibly reconstructed textures/borders, and the guild name/power lettering is flatter and heavier than the reference.
2. **Task section frame and reward alignment.** The continuous carved enclosure and decorated heading rail around all three tasks are absent. This leaves an abrupt horizontal scenery boundary under the banner. The small dark count plaques are missing. The blue reward strips sit lower in the cards than the reference, and the amounts are farther right relative to the gems. Task banner/swords sizes and frame corner details differ.
3. **Roster enclosure and rows.** The original has a carved surround and ornate row ends, with a warm bronze leader row. The implementation uses simpler repeated Ranking rails and a tinted standard row. The original guild rank badges differ from the reused laurels; ranks 4/5 are plain numerals in the reference but circular badges here. Contribution shields are noticeably smaller and shifted left. Portrait face assets are retained, but their previously reconstructed lower caps remain approximate.
4. **Footer.** The original wide red motto plaque, large guild shield and small information icon have been replaced by a navy motto panel, Back/Friends controls and different carving. This is a substantial visual difference, although existing navigation remains functional.
5. **Scenery and typography.** Sunset/building placement differs, especially behind the banner/progress area. Most live captions, names and numbers are heavier than the reference and lack its exact gold shading. Preserve the selected fonts while refining scale/spacing; their exact source identity remains unverified.
6. **Progress finish.** The previous backing overflow and visible source-text problem are corrected. The native flat blue fill still lacks the reference's faceted/glossy crystal appearance.

**Retain approved choices:** the shared Home header, actual saved balances, common gem/potion artwork, selected fonts and visible example disclosure. These intentional differences should not be treated as accidental extraction failures or replaced with fake live guild data. The original title/banner and source navigation lettering are the strongest matches; fine exterior alpha is still estimated.

**Recommended next revision: Team Hub R2 — Guild Summary and Task Frames.** Restore the guild-chest enclosure/name plaque and member-count icon; repair the continuous task-section frame and heading rail; align count/reward plaques. Reuse approved chest/gem/booster art, preserve source navigation cards and current routes. Address roster carving and footer composition in a later focused pass. No production code or assets changed during this recheck.

| Area | Current result and remaining difference |
|---|---|
| Header | Uses the real shared Home component, including coins/gems/energy. Saved values, shared portrait/type and unknown XP differ from the illustrative source. |
| Title/banner | Original accepted Russian title and banner preserved. Previous estimated alpha/hidden banner edge remain. |
| Navigation cards | Original Russian faces, icons, lettering and visible frame pixels restored. Live English labels use repaired opaque lacquer; fine exterior alpha and the repaired label texture are approximate. |
| Guild summary | Dark backing, restored power rail material, better spacing and two chest arrangement. Member-group pictogram and full guild-chest enclosure remain absent; backing/occluded frame reconstruction differ from source. |
| Progress | Gold source-derived rail, native empty channel/fill and live sample count. No overflowing rounded backing or baked text is displayed. Native gradient and cleared blue fragments differ from painted source lighting. The 75.6% value is explicitly an example. |
| Tasks | Direct approved sapphire and potion reuse. These silhouettes intentionally differ from the page-local original. Existing blank task frames, reward plaques and panel textures still have reconstructed details. |
| Roster | Larger original portraits and source-relative row positions restored. First row no longer uses blue personal-ranking highlight. Existing Ranking medals/row rails and portrait cap repairs still differ from original guild-specific ornamentation. |
| Typography | Uses existing embedded fonts; name font preserved. Caption weights, glyph shapes and gold treatment are not an exact source identification. |
| Footer | Back/Friends remain functional at the safe bottom; reused dark carving and motto panel differ from original red motto/crest/info treatment. |
| Scenery/tall screens | Existing generated clean environment retained. Buildings/light differ from source. Tall screens show extra scenery between the fixed-composition roster and the bottom controls. |
| Features | Membership, contribution, tasks and roster remain sample data. No online guild, donation, task or chest reward functionality is implied. |

If another Team Hub refinement is requested, prioritize the task-panel enclosure, guild-chest frame and guild-specific roster rails. Reuse accepted resources and extract visible original pixels before considering any new generation.
