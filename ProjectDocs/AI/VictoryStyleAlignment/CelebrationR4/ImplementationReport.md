# Victory — central celebration R4

Date: 2026-09-15. Implemented the user-approved next refinement in the active Unity Victory page. Visual acceptance of R4 remains pending.

## Result

Victory now uses a darker red-brown ribbon with finer gold engraving and longer curled tails, a live title arranged on a center-high arc, and larger/lower stars with smoother gold facets, warm rims and soft local light. All halos render behind all star faces, so overlapping glow cannot wash out a neighboring star. Russian and English text remain native and localizable.

The R3 evening-city environment, Firebird and resource header retain their exact existing assets and placement. Reward contents, score, energy state, story routing and native button behavior are retained. This pass deliberately focuses on the central celebration; reward sizing, lower actions and footer ornamentation remain the next reference-alignment work.

[Actual Unity portraits, R3 and reference](Victory_Celebration_R4_Review.html) · [Reference comparison](ReferenceComparison.md) · [Preservation audit](QA/SourceAudit.json).

## Implementation

- Active prefab: `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Victory.prefab`, revision 4. Existing owner `Step20ProductionUiFix01SerializedPrefabBuilder.RebuildVictoryAlignment` rebuilt it in place and read it back successfully. No duplicate page/startup system or new control objects were introduced.
- `ProductionVictoryRuntime.cs` assigns `banner_r4`, curved title and Victory-only halo/depth effects. The banner sits at body `(20,375,901,425)`. The title uses `(110,463,721,150)`, reference font size 112 and arc rise 32. Side stars are 150 square at body y634; the middle is 170 square at y618. Existing glow objects are 264 square and are reordered behind every star; prior IDs are preserved.
- New `ProductionVictoryTitleLettering.cs` transforms whole glyphs rigidly along the arc, using their live atlas UVs and the existing Home gold material. It adds warm face bands, a bronze extrusion and a shadow. It supersedes the enabled effects on this title only: old `Shadow` and `ProductionHomeGoldText` components remain serialized but disabled. Continue retains its existing effects/material. Font assets and assignments are reused; the exact reference typeface remains unidentified.
- `ProductionVictoryStarDepth.cs` now renders complete layers in order, preventing offset shadow wedges from painting across finished neighboring facets. Shared edge colors and smooth face transitions retain dimensional gold without the earlier dark radial seams. Empty stars retain subdued metal and dark inset faces.
- New `ProductionVictoryStarHalo.cs` supplies a soft radial glow, 18 tapered rays and seven sparse fixed sparkles per earned star. It respects the existing reveal alpha and earned-state visibility. These are native mesh details with the existing one-time reveal, not a new particle simulation or flashing loop.
- `ProductionVictoryView.cs` and required-binding checks agree on revision 4. Existing services, real current-run result/reward state and navigation are unchanged.
- `LockedDesignImporter.cs` adds one exact-path rule for `banner_r4.png`: native 1529×935 single FullRect sprite, sRGB, no resizing, uncompressed and RGBA32 Android import, transparent edges and mipmapped/trilinear minification. No old artwork/meta was overwritten.

## Artwork and provenance

New asset: [banner_r4.png](../../../../Assets/LegendsOfRus/Resources/ProductionUI/VictoryAligned/banner_r4.png).

Used the builtin imagegen tool, with the old blank ribbon as the edit target and the original Victory image as style guidance. No region of the screenshot was cropped into the runtime UI. The first generated ribbon was too vivid and retained oversized raised leaves; a second edit deepened the face and replaced those with finer engraved motifs. Both outputs painted a checkerboard instead of providing alpha.

Used the user's existing **“Allow local transparency cleanup”** authorization. Removed only border-connected near-neutral exterior pixels, then trimmed transparent margins with six-pixel padding. All retained artwork RGB is unchanged; opaque face/folds remain intact. Source was 1536×1024 RGB; final sprite is 1529×935 RGBA. Cleared 923,983 exterior pixels. Final image was inspected over black, white and magenta with no visible checkerboard or filled central hole. Native-resolution edges are binary alpha from this cleanup; Unity's minification uses the configured mipmaps/trilinear filter.

The selected raw source is retained in [Sources/Banner_Generated_Raw.png](Sources/Banner_Generated_Raw.png). [Exact prompts](Sources/GenerationPrompts.json), [provenance](Sources/AssetProvenance.json), [cleanup method/results](QA/BannerTransparencyCleanup.json) and the reproducible cleanup script are retained. Title/stars are code-rendered; no generated or baked text was added.

## Verification performed

| Check | Result |
| --- | --- |
| Project inspection | Root `/home/k2/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL`; recorded/installed Unity 6000.5.9f1. Read AGENTS, current task, historical handoff and provenance; inspected current local changes. No nested AGENTS found. Editor was closed. |
| Baseline | Verified and backed up all 1,760 entries of the current R3 production manifest before edits; zero mismatches. Historical Loading handoff did not override current Victory files. |
| Unity run1 | Compile, Victory-only owner rebuild/readback, 6/6 EditMode and 14/14 PlayMode passed, zero skipped. Inspected RU941, EN576/five-reward and RU941/one-star portraits. |
| Final Unity run2 | After correcting halo draw order and adding ordering assertions: compile/rebuild/readback and all 6 EditMode + 14 PlayMode passed again, zero skipped. No production edits after this run. Same three final portrait states visually inspected. |
| New rendering checks | Actual rendered RU→EN→RU glyph bounds within the ribbon face; raised center and opposing outer-glyph slopes; stable vertices over repeated refreshes; no duplicate effects; halo ordering; earned-only visible glow; opacity/transparent-edge/mesh bounds. |
| Existing regression checks | Victory and Pre-Level selected fixtures retain tests for all ten authored reward definitions, current-run/replay score, earned stars, no-result/reset, reward rendering without grants, once-only coins doubling domain operation, unavailable video, resource/Continue routing, double-click handling, real registered board-win callback, live labels, native input and safe areas. |
| Responsive captures | Retained 12 final Victory PNG/JSON pairs and 20 Pre-Level pairs. Includes 941×1672 RU/EN, 576×1280 and 1080×2400 with simulated safe areas; energy countdown and multiple reward/eligibility states. Not every capture was manually inspected. |
| Independent/source checks | Standalone title compilation against installed Unity/uGUI assemblies passed; source/metadata/whitespace checks and independent mesh/lifecycle review completed. An offline star mesh diagnostic was used for material tuning, clearly separate from actual Unity evidence. |
| Review viewer | All 14 portrait choices, three document links and the single default image resolve. Temporary headless Chrome rendered the local page at 390×1100; screenshot inspected. Dropdown interaction was not exercised. |
| Final integrity | All 1,766 final production-manifest entries and 93 retained evidence entries match. All 127 old R3 evidence hashes remain unchanged. |

Raw logs, XML, commands and the runner are under `UnityVerification/`; both run histories are retained, plus the three inspected first-run portrait pairs. [TestHistory.json](QA/TestHistory.json) records the exact outcomes. Unity needed existing desktop display/licensing services; automatic approval review allowed those launches with isolated per-run test storage and a temporary browser profile. There was no new rejected launch or failed selected test this turn. No user save directory was used for these tests.

## Preservation and remaining limits

Of 1,760 baseline files, 1,752 are unchanged. Eight existing files changed: the Victory prefab, owner builder, exact-path importer, three Victory runtime/view/depth classes and two Victory test fixtures. Added six production files: one ribbon and two Victory-only scripts, each with its own new meta. All 910 existing meta files and all 319 prior Victory object IDs are preserved (323 IDs now). No shared font/material/effect source, GameState/router, other page asset/prefab, package, project setting, signing setting or Android guard changed in this pass. Previous Gameplay/Pre-Level/local edits remain intact. Full changed-file lists and manifest scope are in the preservation audit.

Not performed: APK/AAB build, physical Android testing, full project test suite, successful live rewarded-ad transaction, a complete manual gem-swap win session, manual reveal-animation playback, performance/prolonged-play profiling, calibrated color measurement or exact font identification. The board-win test invokes the registered callback, and the coins-doubling test uses the domain operation; neither is a claim of end-to-end manual gameplay or live monetization.

Reference fidelity is improved, not exact. Ribbon contours, title font/material and star artwork still differ from the painted reference. Existing English story content can fall back to Russian, and the small secondary energy status still needs physical-phone readability review. Next focused visual task: reward illustration/card scale and spacing, then Continue/video surfaces and footer ornamentation.
