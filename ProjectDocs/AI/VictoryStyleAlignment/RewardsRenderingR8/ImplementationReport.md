# Victory R8 — reward rendering corrections

Implemented 2026-09-15 in the existing Unity 6000.5.9f1 project. Active Victory prefab and its owner/runtime now use revision 8. User requested correction of expanding reward panels, weak stars, left-shifted reward illustrations and blurry text. Visual acceptance and pixel-identical fidelity are not assumed.

## Changes and causes

- The outer reward sprite is 1942 × 809 but R7 drew it at 841 × 580. Reward cards similarly fitted a 1050 × 1498 source to a much wider rectangle. New Victory-only mesh segmentation keeps corners, center jewels and side ornaments uniformly scaled while connecting wood spans resize. The outer panel is now 841 × 520, with less empty surround. Existing opaque surfaces and all source pixels remain intact.
- Unity's `preserveAspect` aligned the drawing to the top-left pivot of a wide icon rectangle. A square illustration could therefore appear about 22 design units left of the card center before body scaling. Each icon now has a rectangle matching its actual sprite aspect ratio, centered in a dedicated 129-high illustration area above the count. Read-only alpha measurements found relatively small horizontal source margins; these were not the main cause.
- Earned stars now have ten distinct light/shade gold planes instead of soft interpolation across every facet. Their warm rim glow and tapered rays are stronger. Empty stars, earned-star count, reveal order and one-shot animation behavior remain intact.
- Small live text uses pixel snapping, the existing local font and a short shadow. Decorative bevel overlays and long offset shadows are removed from small headings/video labels. R7 Continue lettering and the curved Victory title retain their existing treatments. No new font files or exact-font claims.

R7 Firebird/Continue assets and natural button ratio, R6 story/status inset, native rewards, actual score, energy, replay handling, navigation and provider behavior are retained. No new raster generation, screenshot crops, texture edits or import changes. Seven new mesh components attach to existing objects; all 329 prior prefab object IDs remain.

## Checks performed

Read project instructions, historical handoff/current task and reference provenance; inspected local changes and actual source/reference/captures. Verified and backed up all 1,776 R7 production-manifest entries before editing. Unity was closed. No subagents used.

Two isolated Unity runs both passed compilation, Victory-only rebuild/serialized readback, **6/6 EditMode and 14/14 PlayMode tests**, with zero failures or skips. Run 1 visual inspection showed excess outer-panel space; run 2 followed the height correction and stronger rays. No production changes after run 2. See [exact test history](QA/TestHistory.json) and commands/logs/XML under `UnityVerification/History/`.

New assertions check rendered corner/crest geometry for uniform scaling, illustration screen-space centering and sprite aspect equality, count clearance, pixel snapping and disabled small-text bevel overlays. Existing checks cover native hit targets, RU/EN and portrait safe areas, live title geometry, earned/empty stars and reveal opacity, actual level rewards/current score, replay/no-result, reward doubling domain behavior, unavailable video, energy/service clock, navigation and Pre-Level regressions. The lower message clearance check now uses the protected carving's source-pixel height rather than the obsolete stretched-panel percentage.

Retained 12 final Victory and 20 final Pre-Level PNG/JSON pairs, both run logs/XML/commands and the inspected earlier RU portrait. Visually inspected final RU 941 three-reward and EN 576 five-reward captures against R7 and the original reference. Captures use actual Unity rendering with simulated displays/safe areas and isolated test saves, not a physical phone.

Headless Chrome 390 × 1100 default viewer screenshot inspected. All 14 portrait choices and three links resolve; dropdown interaction was not exercised. Automatic approval review permitted both Unity runs and Chrome; no rejected launch.

Preservation audit: 1,768 baseline files unchanged, eight existing files changed and one new C# component plus its meta. All 918 existing metas, all prior Victory object IDs and all 75 R7 evidence hashes retained. Source textures, fonts, other pages, unrelated local changes, packages/settings/signing/Android guards unchanged. Exact paths: [SourceAudit](QA/SourceAudit.json). Changed C# trailing-whitespace check passed. Final production manifest contains 1,778 files.

## Limits and remaining differences

No APK/AAB, physical Android test, full project suite, successful live rewarded ad, manual gem-swap win, manual animation playback, profiling/prolonged play, calibrated blur/color metrics or exact-font identification. Not every capture was visually inspected. Narrow five-reward labels and the energy timer remain small; phone readability needs device review. The EN story can fall back to Russian. Existing scenery/title/illustration differences and the tall-phone footer gap remain. Stars have clearer facets but do not reproduce every painted reference sparkle. Real rewards are not replaced by the reference's decorative egg or illustrative amounts.
