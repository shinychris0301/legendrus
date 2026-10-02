# Team Ranking R4 — Table Surface and Guild-Name Fit

User requested the next recommended refinement after R3. Replaced the thick repeated row boxes with a continuous source-derived dark table and thin dividers, and fitted complete Russian/English guild names on one line using the approved font.

## Changes

- Three new sprites: original outer table enclosure with a clean opaque navy interior, thin source separator, and source gold highlight with a darker navy face. Old highlight brackets and candle/footer overlaps were removed from the outer border using neighboring clean source rails.
- Regular rows render only their thin bottom divider; the continuous table supplies their background. The highlighted row retains its gold perimeter with no bright blue fill.
- Names retain the selected embedded font. Their bounds are 176×36 reference pixels; actual font size is measured from 22 down to 17, preserving every character and space. Fitting refreshes on localization and canvas-scale changes, without relying on potentially overflowing native best-fit behavior.
- R3 podium ribbons, reward enclosures, reward positions/sizes, approved icons and count geometry are unchanged. Shared header, source title/Details button, native controls and truthful example standings remain.
- Active prefab and owning layout/view/builder/cutover agree on internal revision 5. Existing serialized controls and GUIDs preserved.

## Art workflow

No Scenario jobs/credits. Existing approved artwork is reused; new frame/surface assets derive from the original image. The dark surface uses a mirrored text-free source donor, so hidden content is reconstructed and subtle repetition remains. Original perimeter/divider material is retained except documented neighbor-rail continuations. Source boxes, donor boxes and hashes are recorded in `Sources/AssetProvenance.json`. Transparent assets were inspected over black, white and magenta; panel faces are intentionally opaque.

## Validation

Final Verify03 with Unity 6000.5.9f1: prefab rebuild/readback succeeded; **13 Edit Mode + 15 Play Mode tests passed**, zero failures/skips. Includes Team Ranking, Team Hub and Individual Ranking. New checks assert complete single-line names, measured width, separation from crest/member icon, continuous table/thin dividers and retained reward enclosure dimensions. Existing checks cover idempotency, native bindings, original-lettering localization, hit targets, safe edges, reward containment/separation, routes and save preservation.

Verify01 exposed a real fitting issue when canvas scale changed after fitting; the responsive layout now refits when pixels-per-unit changes. Verify02 passed all 28 tests. Independent visual review found one remaining candle tip at the lower border; its removal is included in final Verify03. Final native RU and narrow EN captures were inspected, with nine Ranking captures retained and regression captures for both related pages. Eleven review views and comparison toggle passed browser checks.

Eight existing Assets files modified; three PNGs + three metas added; zero deletions. Existing PNGs/metas/fonts, other page prefabs and prior work preserved. Exact paths are in `QA/ScopeAndVerification.json`; preservation results in `QA/PreservationChecks.json`.

No Android build, physical-device, performance or full-suite test. Actual desktop Unity captures with simulated safe areas are not device evidence. See the [fidelity review](QA/ReferenceFidelityReview.md) for remaining differences.
