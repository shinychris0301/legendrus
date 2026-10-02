# Characters — Text, Surfaces and Active Border R4

The current hero now has an explicit bright-yellow perimeter rendered above the antique-gold card frame. It remains on Ivan while another character is previewed. No equip/ownership behavior was added.

Detail-button captions moved7reference pixels left and1down to align with the visible colored faces. The page title moved10pixels down. The character-name rectangle moved down and was tightened to remain clear of the stars. RU/EN text remains live and localizable.

Four new sprite variants repair the title, detail-panel and two control faces with continuous source-matched tone and bounded texture. Title alpha cleanup removes attached dark scenery and detached fragments around its gold/sapphire silhouette. Original assets and their GUIDs are retained; no complete screenshot UI is used. Original RGB outside each recorded repair mask is unchanged. Other alpha is unchanged; title alpha is intentionally cleaned. Alpha was inspected over black, white and magenta. Initial repeated-donor flecks were caught during inspection and corrected before Unity verification. This is local cleanup of prior Scenario-derived assets, with no new API requests or generation (0CU).

The existing prefab and owning builder/runtime/view now use revision4. R3scrolling/current-vs-preview behavior, unavailable ownership/upgrades, saved state and navigation remain intact.

## Verification

Unity6000.5.9f1 import/compile, rebuild/save/readback and revision4 cutover passed.2Edit+6Play=8tests passed with0failures/skips. Added assertions that all four opaque yellow edges render above the frame and cannot intercept clicks. Existing coverage includes current-vs-preview selection, upward collection drag/full locked-row visibility, filter scroll reset, RU/EN text, safe-area/raycasts, header/navigation/input and save preservation.

Nine actual Unity captures retained. Visually inspected reference-sizeRU, Firebird preview with Ivan highlighted, and narrow-safeEN. Browser390px reviewed11views with no missing images/links, script errors or horizontal overflow; details inQA/BrowserCheck.json. Source syntax/whitespace and asset integrity checks recorded inQA/SourceChecks.json; change scope inQA/ScopeAudit.json.

## Limits

No APK or physical-phone/touch/hardware-Back test, full-project suite, profiling, or hero backend implementation. Main hero/background composition, low-resolution enlarged alternative portraits, exact source font and footer icons still differ from the reference. This pass fixes the reported alignment/surface/current-highlight issues; it is not an exact replica claim.

[Open the before/after review](Characters_TextAndSurface_R4_Review.html).
