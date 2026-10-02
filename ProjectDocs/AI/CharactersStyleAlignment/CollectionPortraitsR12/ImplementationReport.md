# Characters R12 — Collection Card Portraits and Backgrounds

The collection previously enlarged all eight original portrait crops and repeated the Gameplay town behind every hero. R12 renders the existing isolated portraits at source pixel proportions and offsets, adds blue-town or dark-forest plates according to the reference setting, and blends the cropped bases into a darker caption area. Locked cards use the subdued forest plate. The main detail portrait and R11 outer carving are unchanged.

[Review actual Unity captures](Characters_CollectionPortraits_R12_Review.html) · [Asset provenance](AssetProvenance.json) · [Scenario audit](QA/ScenarioAudit.json) · [Final tests](QA/TestSummary.json).

## Source artwork and Scenario use

The original reference and source regions were inspected alongside R11. Eight original portraits were already isolated through Scenario in R1; their faces, hair, costumes and RGB are reused. New card-only variants alter alpha in the bottom eight pixels to soften the source cutoff. Original sprites remain unchanged, including those used for alternate detail portraits. No character was regenerated, sharpened or recolored.

The background pixels hidden behind bodies cannot be extracted. Two reference-guided Scenario requests reconstruct only that scenery: a muted blue Russian town and a dark woodland. Both returned clean plates without figures, text or UI, and their PNGs are copied unchanged. Native display tint controls brightness. Two accepted jobs cost **24 CU**, with zero retries or duplicate submissions. No new isolation job was needed for the reusable portraits. Source guidance crops are offline inputs only; they are not runtime screenshot regions.

Sources/Assessment.json records original hash/bounds and extraction-versus-reconstruction decisions. Sanitized requests, uploaded-reference hashes, job IDs/status, numeric billing, original downloaded outputs and final hashes are retained. Existing Scenario-derived frame, footer, hero and caption-fade sprites are reused.

## Implementation

Runtime constructs independent scenery, original portrait, caption fade and shadow layers inside the existing clipped card window. Art cannot intercept taps. The shadow becomes opaque before the original crop base, preventing a scenery stripe beneath the bust. Card names, preview statuses and stars remain native and localizable. Current-Ivan highlighting, filters, scrolling, routes, saved balances and unavailable hero-progression states remain intact.

Runtime/view, owning editor builder, active prefab and cutover agree on revision 12. The existing native-binding test additionally checks independent Characters scenery, source portrait assignment, aspect preservation, draw order and noninteractive art. QA/ScopeAudit.json lists all production changes relative to the session-start snapshot: six existing files, ten new PNGs and ten metas, zero deletions. Existing artwork/GUIDs, shared pages including Gameplay and Splash, packages, settings and Android guards are unchanged. Documentation and review files are additional changes.

## Verification performed

- Unity 6000.5.9f1 import/compile, prefab rebuild/readback and revision-12 cutover passed.
- Final Verify02: **2 EditMode + 6 PlayMode tests passed**, with zero failures/skips. Coverage includes native bindings, idempotent building, current-hero highlight, locked-card scrolling, localization/simulated safe areas, modal input blocking, real routes, preview ownership safety and callback cleanup.
- Verify01 also passed eight tests. Its captures exposed the remaining straight bust cutoff, prompting the caption-shadow correction and final Verify02 rerun. Both runs are retained.
- Nine final actual Unity captures were produced. RU 941×1672, narrow safe-area EN and scrolled safe-area EN were visually inspected against the original/R11. Generated plates were inspected independently. All eight portrait variants were inspected on black, white and magenta.
- Original/guidance/upload/download/final hashes passed. Portrait RGB and all pixels above the bottom alpha-taper strip are preserved; both new plates are byte-identical to Scenario outputs. Three source Python files parsed; scoped whitespace checks passed.
- The standalone review offers nine R12 captures, R11 and the original comparison. QA/BrowserCheck.json records image/link/error/overflow checks at 390×844.

## Limits and checks not performed

The new environments are reconstructions, not exact hidden source pixels; two plates are shared across cards. Original source portraits remain low resolution and cropped above baked lettering. The deeper caption shadow conceals those bases rather than inventing missing clothing. Header artwork/portrait, exact font identity and earlier reconstructed surfaces remain reference differences. Hero ownership, upgrades and bonuses are still unavailable/preview.

No APK build, physical-phone/touch/hardware-Back test, full project test suite, profiling, backend or hero-service test was performed. Simulated display captures do not establish physical-device behavior. R12 is ready for user visual review; acceptance is not presumed.
