# Characters R10 — detail-panel geometry

User explicitly selected continued Characters refinement. The main portrait and detail frames now begin at source y309 instead of323, with their bottom at843 unchanged. This restores the intended panel height and reduces the excess gap beneath the rarity tabs.

The main portrait fills the taller window at its original aspect ratio, with a small proportional enlargement. Alternate portrait positioning follows viewport height and covers irregular source cut bases beneath the frame. No portrait pixels were changed.

Detail action controls move from y775 to source y758; their area height increases from60 to67. The character name is higher and centered within its header space. Abilities and team-bonus information move upward to match the reference hierarchy and leave room for the action row. Detail labels use double-density live text with the same selected embedded font families.

Current hero, unavailable controls, true save values and all preview wording remain unchanged. R9 title/header, R8 footer, collection positions, Ivan-only highlight and locked-row scrolling are preserved.

## Scope

Five existing production files changed: Characters runtime/view/editor builder, active prefab and cutover state, all on revision10. No new/deleted production files. No changes to existing art, fonts, materials, shaders or metas; other pages/settings/packages/Android guards untouched. See [scope audit](QA/ScopeAudit.json).

Existing Scenario-derived art is reused. **Zero new API calls/zero credits/no image editing.** No alpha cleanup or new alpha acceptance checks were necessary.

## Verification

- Unity6000.5.9f1 import/compile, prefab rebuild/readback and revision10 cutover passed.
- Final Verify02: **2 EditMode + 6 PlayMode tests passed**, zero failures/skips. [Individual cases](QA/TestSummary.json).
- Existing tests cover RU/EN values and responsive layouts, preview/filter/save integrity, source portrait cutoff bounds, independent current-Ivan highlight, locked-card drag scrolling, overlay input blocking, nine header/footer routes and callback lifecycle.
- Nine final actual Unity PNG/JSON captures retained:941×1672,576×1280 and1080×2400 with simulated safe areas and alternate states. Russian Ivan, narrow English and Firebird preview inspected visually.
- Initial Verify01 also passed eight tests; capture review exposed a narrow cut base on the Firebird preview. Alternate portraits were lowered14 design units, then targeted verification rerun.
- Scoped whitespace and session-start art/font/material/shader/meta hash preservation checks passed.
- Standalone review browser check at390×844: eleven image choices, no browser errors, missing links or horizontal overflow.

Evidence: `QA/UnityVerification/Verify02/`, [source checks](QA/SourceChecks.json), [browser check](QA/BrowserCheck.json), [asset reuse](AssetProvenance.json).

## Remaining differences and checks not performed

Exact source-font identity, header artwork, outer carved perimeter and varied collection scenery/portrait resolution remain different. Disabled actions and unavailable stats intentionally differ from the source's sample values; no hero progression service was added. Hidden scenery/backing remain reconstructed from earlier passes.

No APK build, physical-phone/touch/hardware-Back test, full project test suite, profiling or backend/hero-service test. No new Scenario access check or image generation.

Next: user review of R10. The next suggested art refinement is the outer carved perimeter; it is not implemented or presumed approved here.
