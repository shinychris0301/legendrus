# Characters R9 — header and live title typography

The R8 page title was visibly smaller and flatter than the reference, and header/tab captions crowded their available space. R9 enlarges the live title, adds the existing gold gradient/bevel material with a restrained shadow, and aligns the plaque and tab row with the original positions. Header name/level and resource values are smaller; tab captions have more breathing room. Title, tabs and name/level use double-density glyphs while preserving their visible bounds.

Selected embedded fonts remain unchanged: FreeSerif Bold for the title, Roboto Serif SemiBold for names/body roles. Exact reference-font identity remains unknown. All labels remain live and localizable. Real balances and routes are unchanged. No fabricated experience progress was added: the game has no corresponding XP model.

## Scope and artwork

Characters native prefab, owning runtime/view/editor builder and cutover now agree on revision9. Existing EditMode coverage checks title font/material binding and nonblocking text. Six existing production/test files changed, no new or deleted production files. See [scope audit](QA/ScopeAudit.json).

Reused the existing Scenario-derived title plaque and approved Home live-gold material. **Zero new Scenario requests, zero credits, no image edits.** All preexisting art, font, material, shader and meta files match session-start hashes. Shared navigation, other pages, Gameplay/Splash, packages, settings and Android guards are unchanged. R8 footer, R7 main scene, exclusive current-Ivan highlight, locked-card scrolling and unavailable hero states are preserved.

## Checks performed

- Unity6000.5.9f1 import/compile, prefab rebuild/readback and revision9 cutover passed.
- **2 EditMode and 6 PlayMode tests passed**, zero failures/skips; [individual results](QA/TestSummary.json).
- Existing live RU/EN header, safe-area layouts, preview/filter/save behavior, exclusive highlight, scroll-to-locked-row, overlay blocking, nine header/footer routes and callback lifecycle checks passed.
- Nine actual Unity captures retained at941×1672,576×1280 and1080×2400, including simulated safe areas and alternate states. Russian full page and narrow English capture inspected visually for title rendering, fit and header clarity.
- Scoped whitespace check passed; original reference hash recorded; all existing art/font/material/shader/meta hashes verified unchanged.
- Standalone review checked at390×844: eleven image choices load, no browser errors, missing links or horizontal overflow.

Logs and captures: `QA/UnityVerification/Verify01/`. Browser and source audits: [browser](QA/BrowserCheck.json), [source](QA/SourceChecks.json).

## Limits and next action

This improves live typography and placement; it is not an exact replica of the embossed reference lettering. Header portrait/resource artwork, carved perimeter, alternative portrait resolution and collection scenery remain known differences. The main scene/backing contain reconstructed hidden areas from earlier revisions.

No APK, physical-phone/touch/hardware-Back check, full project test suite, profiling, backend/hero-service test or exact-font identification was performed. No new alpha QA was needed because no images changed. Next: user review of R9 before choosing another visual refinement or proceeding to the next page.
