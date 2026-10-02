# Event Detail R6 — progress indicator and panel frames

The progress rail retained cropped pieces of its source diamonds while separate diamonds were drawn above them. Its backing used an unrelated plaque with preserveAspect, shrinking an 847×20 region into a short central image. R5's frame masks also cut through ornament curves and left rectangular dark corner patches.

## Corrections

- Replaced the rail/backing combination with one source-derived continuous empty track. Original end caps retained; a clean source rail span bridges the old marker footprints without hard colour joins. Original unfilled navy material spans the full tapered channel.
- Extracted each diamond around its actual source centre, removing adjacent gold fill and rail fragments. One marker per milestone, aligned with its threshold and reward card. Corrected the previous half-design-pixel card-centre offset.
- Expanded the original panel-frame masks to retain complete curved ornaments, bevels and shared junctions; excluded adjacent labels, rank medals, inner panels and progress caps. Rounded backing alpha follows the frame corners. Frames render at their source aspect ratio; opaque section faces still meet without overlap.
- Preserved shared gem/booster bindings, all R5 chest art, exact Home gift/background, shared header, navigation and unavailable event state. No new Scenario requests or credits.

## Files and validation

Six existing Assets files changed relative to R5: ProductionEventDetailRuntime.cs, ProductionEventDetailView.cs, ProductionEventDetailPrefabBuilder.cs, EventDetail.prefab, EventDetailCutoverState.txt and EventDetailAlignmentPlayModeTests.cs. Internal revision7 agrees across owner/prefab/cutover. Eleven new PNGs plus metadata: track, five markers, complete frame and four rounded backing faces. No prior sprites/metas or other page prefabs changed; prior working-tree edits preserved. Exact scope: QA/ScopeAndVerification.json.

Final **Verify03**, Unity **6000.5.9f1**: prefab rebuild/readback succeeded, **4 Edit Mode +10 Play Mode =14 passed**, zero failed/skipped. Coverage includes exact shared resources, track/frame aspect, marker/threshold/card centres, layer order, nonoverlap, safe areas, RU/EN text fit, navigation and unchanged preview state. Six Event Detail and eight Events regression captures used isolated saves. Final native RU/narrowEN and progress/frame close-ups inspected. Black/white/magenta alpha inspected. Nine browser views and comparison toggle passed.

Verify01 caught the half-pixel reward-card centring error at the larger screen scale; geometry was corrected rather than relaxing tolerance. Verify02 passed; close-up inspection led to tighter marker alpha and a smoother rail bridge. Verify03 verifies those final resources. Asset audit initially included the two-pixel anti-alias fringe next to the frame; its text-exclusion check now inspects the actual text-bearing interior. Sources/Audit.py verifies continuous channel/rails, unchanged approved resources, frame source RGB and excluded heading text.

No Android build, physical-device test, performance test or full-suite run. The event service remains unimplemented: an empty preview track is deliberate, not a claimed live progress value. Dates, thresholds and quantities remain unknown; no reference sample progress is fabricated.
