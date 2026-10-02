# Characters R8 — source footer medallions

Replaced the four Home square navigation illustrations with the original Characters reference's round map, shop chest, event scroll and guild crest. Added a separate dark carved backing and an isolated source caption plaque. Captions remain live Russian/English text; existing World Map, Shop, Events, Team Hub and Back routes are retained.

## Artwork and fidelity

Five Scenario Pixa source-alpha jobs (four medallions and the caption plaque) cost 15 CU. One hidden-backing reconstruction cost 12 CU: **27 CU total, six accepted jobs, no resubmissions**. Request, input hashes, job IDs, outputs and billing are under `Sources/Scenario/`.

Source RGB is preserved outside explicit repair masks. Restored opaque navy faces that automatic extraction incorrectly removed. The event's reference notification badge is removed by mirroring its unoccluded rim counterpart; no live notification state exists on this page. The plaque's baked caption was cleared with a bounded source navy donor for localized text. The generated backing's white exterior margin was removed; its carved panel pixels remain unchanged. No full screenshot or scenery-contaminated crop is used as a runtime control.

All five transparent sprites were visually inspected on black, white and magenta. Original/input/final hashes and source RGB outside repair masks were checked. See [provenance](AssetProvenance.json) and [API audit](QA/ScenarioAudit.json).

The footer is closer to the reference, but the reconstructed hidden backing is not an exact recovery. Its vertical position accommodates the existing collection information action. Exact font identity is still unknown; current font assignments are preserved. Title/header, card scenery/portrait resolution and other previously documented differences remain outside this footer revision. Hero stats/ownership/upgrades remain unavailable and preview-labelled.

## Native implementation

- Runtime builder, view revision, editor builder, active Characters prefab and cutover state agree on revision 8.
- Six new PNGs and their Unity-generated meta files under `CharactersAligned`: `nav_map_r8`, `nav_shop_r8`, `nav_events_r8`, `nav_team_r8`, `nav_caption_r8`, `footer_backing_r8`.
- Footer bindings, nonblocking backing and live-caption draw order added to existing EditMode coverage.
- Existing source assets/meta GUIDs, other pages, Gameplay, Splash, packages, settings and Android guards unchanged against the session-start file hash snapshot.
- Exclusive current-Ivan highlight, independent hero preview, locked-row scrolling, R7 scene and R4 surface/text corrections preserved.

Exact production file changes are listed in [scope audit](QA/ScopeAudit.json): six existing files, twelve new PNG/meta files, no deletions.

## Verification performed

Unity **6000.5.9f1**:

- Import/compile, owning prefab rebuild, serialized readback and revision-8 cutover: passed.
- Final Verify02: **2 EditMode + 6 PlayMode tests passed; zero failures or skips**.
- Existing coverage exercised RU/EN live values; 941×1672, 576×1280 and 1080×2400/simulated safe areas; preview/filter/save integrity; Ivan-only frame highlight; upward drag exposing locked cards; information overlay/input blocking; all nine header/footer routes; callback rebinding/disable behavior.
- Nine final actual Unity PNG captures and associated layout JSON retained. RU portrait and scrolled narrow EN visually inspected. Earlier Verify01 also passed all eight tests; capture inspection found crowded footer captions, so their position/size and plaque proportions were corrected and the full targeted checks rerun.
- Three source Python scripts parsed; immutable crop/source/request-input/final hash checks and scoped whitespace checks passed.
- Standalone review browser check at 390×844: eleven image choices, no script/resource errors, missing linked files or horizontal overflow.

See [test cases](QA/TestSummary.json), [source checks](QA/SourceChecks.json), [browser results](QA/BrowserCheck.json), and `QA/UnityVerification/Verify02/` for logs and captures.

## Not performed

No Android APK build, physical-phone/touch/hardware-Back test, full project test suite, profiling, backend/hero-ownership service test, or exact-font identification. Simulated safe-area and route checks do not establish physical-device behavior.

Next: user review of R8. Further header/title/typography refinement is separate work; it is not included or presumed approved.
