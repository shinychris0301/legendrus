# Team Ranking R3 — Ribbons and Reward Proportions

User approved the full-reference review recommendation. Restored podium ribbon surfaces and continuous original reward-frame outlines, then enlarged reward contents without returning to the earlier overflow.

## Implementation

- Three new podium variants restore original ribbon trim, warm gold/bronze ribbon surfaces and silver podium navy. Lettering footprints use clean material from the same source ribbons; silver/bronze repair texture is locally smoothed to suppress repetition. Names remain live and localized.
- Existing podium alpha, crests, rank medallions and all pixels outside the ribbon region are preserved. `QA/PodiumPreservation.json` verifies this.
- A source-derived continuous reward enclosure replaces three overlapping generic slot images on each podium and table row. The chamfered source rails/perimeter are retained; source navy material clears the reward faces. Existing slot objects remain as layout bounds with rendering disabled, preserving bindings.
- Approved sapphire, rainbow reward and chest sprites remain unchanged. Podium icons increase from 43 to 51 reference pixels in height, maximum count size from 20 to 22. Table icons increase from 34 to 42; count size from 17 to 19. Widths and cell centers follow the new enclosure.
- Table reward groups have seven reference pixels of clearance from each row edge, with internal icon/count separation. Podium names retain R2's ribbon insets.
- Native prefab, owning layout, builder, view and cutover agree on internal revision 4. The shared header, sample standings, native controls and saved data behavior remain unchanged.

## Workflow and scope

Four new PNGs and their metas: three bounded podium variants and one continuous reward enclosure. Original visible ribbon/frame pixels and existing approved source materials are reused; hidden text/icon footprints are reconstructed, not recovered exactly. No Scenario calls or credits. All existing PNGs/metas/fonts and other page prefabs remain unchanged.

Seven existing Assets files changed; four PNGs + four metas added; zero deletions. `QA/ScopeAndVerification.json` lists exact paths; `QA/PreservationChecks.json` and `QA/PodiumPreservation.json` record preservation. Transparent assets inspected over black, white and magenta. No new reward artwork or font files.

## Validation

Final Verify03: Unity 6000.5.9f1 prefab rebuild/readback; 12 Edit Mode + 15 Play Mode tests, all passed. New enclosure identity/draw order/disabled old-frame checks plus existing containment, icon/count separation, row-rail clearance, text bounds, localization, safe areas, hit targets, routes and save-preservation checks. Team Hub and Individual Ranking regression coverage retained.

Verify01/02 passed behavior checks, but close-up asset inspection found a copied gold line, lettering remnant and repetitive repair texture. These were corrected before final Verify03. Nine actual native Team Ranking captures include RU/EN, narrow safe areas, Details, filter state and maximum balances. Final RU/narrowEN inspected; eleven browser review views/comparison checked.

No Android build, device, performance or full-suite testing. The approved scope is delivered; full-page pixel identity is not claimed. See the [fidelity review](QA/ReferenceFidelityReview.md).
