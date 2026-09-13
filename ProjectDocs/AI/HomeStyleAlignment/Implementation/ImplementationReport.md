# Home R1 — native Unity implementation

The user's “okay continue” after reviewing Home R1 authorized implementation. The active Home page now uses individual artwork, native panels, live labels and nine visible buttons. It preserves the existing startup, router, save/energy services and chapter/story gates. The approved offline portrait is a design target, never a runtime screen texture.

Status: revision 2 compiled and saved; **20/20 focused Play Mode tests and 12/12 Edit Mode tests passed**, with no failures/skips. The six control texture refinements are applied and verified. The final recapture and pixel comparison confirm clear, identical Russian text before and after the language roundtrip. No APK was rebuilt for this Home task.

## Active ownership and behavior

`Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab` remains the active registry resource. Its original GUID and five pre-existing root/component object IDs are retained. `ProductionUiFix01PageBootstrap` still enters the existing runtime; `ProductionUiFix01Runtime` delegates Home to `ProductionHomeRuntime` before the historical FIX01 composition. The existing `Step20ProductionUiFix01SerializedPrefabBuilder` owns Home creation, revision/binding validation and in-place repair through **Build or Repair Home Only**. Historical Wave01/Home and screenshot components remain inactive.

`ProductionHomeView` binds saved gold, gems, energy and unlocked level, refreshes through `GameState.Changed`, and follows the existing language setting. Gold/gems use complete grouped values and shrink to fit; extreme integer balances may use two lines inside their pill. The regular/play fonts resolve through the unchanged shared Splash typography configuration. No OS font files were copied or claimed to be the exact reference face.

Play delegates to the existing `OpenChapterMap`, preserving pending dialogue and chapter completion. Energy is still spent at the existing later level-start boundary. Other buttons retain Profile, Settings, WorldMap, Shop, Events, Characters, Mail and DailyReward. Each accepted navigation action produces one existing feedback-service cue; Play retains its Primary role. No second click handler, invisible hotspot layer, duplicate HUD, Canvas, EventSystem or startup route was added.

The quest retains its localized copy without inventing a measured quest objective, reward or claim badge. Downstream pages retain their existing capabilities and limitations; Home completion does not establish their production-art completion.

Loading prepares one real Home prefab request plus **15 distinct texture requests**. The existing measured progress, cancellation, error/retry and once-only transition remain in place; the old 22-texture Home list is removed. No timer is presented as loading progress.

## Artwork and native layout

| Element | Runtime source under Assets/LegendsOfRus/Resources/ProductionUI | Treatment |
| --- | --- | --- |
| Clean village | `HomeAligned/home_environment_generated.png` | Exact reviewed RGB environment |
| Map / events / mail | Corresponding `HomeAligned/*_generated.png` files | Actual outer alpha; opaque object/panel surfaces retained |
| Shop / characters / reward | Corresponding `HomeAligned/*_generated.png` files | Intentional opaque navy surfaces; reward uses native circular stencil presentation |
| Logo / Ivan / Firebird / Play / gear | Existing `SplashTitleReconstructed/` sources | Same approved sprites, source PNGs, metadata and GUIDs; no recoloring or regeneration |
| Gold / gems / energy glyphs | Existing `Assets/Fix01/Icons/` resources | Original small separate glyphs |
| HUD / cards / quest panel / circular masks | `ProductionHomePanel` | Native rounded geometry and navy/gold surfaces |
| Lower wash / Ivan lower fade | `ProductionHomeShade` / `ProductionHomeArtFade` | Native mesh effects; no source-image pixel edits |

`ProductionHomeLayout` positions the header at the safe top and Play/navigation/footer at the safe bottom, while scenery and shading cover the physical Canvas. Character placement distributes extra portrait height according to R1. Shared Splash/Loading layout components and the common scaler remain unchanged.

The generic texture postprocessor would override the new Home settings. Its exception is limited to `HomeAligned/`: exact dimensions, no NPOT resizing, FullRect sprites, uncompressed RGB/RGBA and selective alpha treatment. Six heavily minified new control illustrations are configured for BoxFilter mipmaps and trilinear sampling; the environment stays bilinear without mipmaps. Approved Splash/Loading texture imports are outside this exception. The final Home-only owner reimport/readback and six importer assertions passed after the user closed the desktop Editor.

## Verification record

- Read AGENTS, handoff, CurrentTask and reference provenance; inspected local changes and verified root/Unity **6000.5.9f1 (b57deb96f08d)** before implementation.
- Compiled and imported with the installed Unity Editor; revision 1 and then revision 2 were saved/read back through the real Home owner, outside Play Mode.
- Final native-code Play Mode pass: **20/20**, no failures/skips. Seven Home cases cover actual prepared resources, state/energy/level updates and reentry, RU/EN, nine tall-safe pointer routes with repeated taps and feedback cardinality, chapter/story gates at zero energy, the actual Splash → Loading → Home route, and real portrait rendering. The other 13 cases rerun the existing Splash/Loading/resource-preparation regressions.
- Actual Home rendering captured at 941×1672 RU, 576×1280 RU with simulated top 50/bottom 24 insets, 1080×2340 RU, 1080×2400 RU with side/top/bottom insets, 941×1672 EN, a RU roundtrip, and a separate maximum-balance fixture. Tests check automatic first Canvas layout, physical background/shade coverage, all visible button centers, safe bounds and repeated-layout stability. These are simulated Unity display fixtures, not physical-phone captures.
- First Play Mode run was 18/20. Corrected two test assumptions about font-quad padding versus the actual numeric pill area and the ordinary frame needed after route replacement. The strengthened checks still require every balance digit and exactly one accepted navigation/feedback cue.
- First Edit Mode run was 11/12; corrected its assumption that a button's visible tint target must also be its input graphic. A later 6/12 run correctly caught six cached imports that had not yet acquired the new mipmap settings. After the user closed Unity, the owner reimport completed and all 12 focused Edit Mode cases passed: nine Home delivery/import cases and three existing Splash wiring cases.
- Viewed the actual main/tall/English/extreme-balance images and compared them to R1. No visible clipping, overlap, unsafe control or bottom seam in the inspected captures. High-frequency minification in new small gold illustrations motivated the targeted sampling refinement. A preliminary capture was byte-identical because cached imports had not changed; after the actual reimport, 76,555 pixels changed only within the six new illustration rectangles. Separate captures revealed soft localized text after a language roundtrip; the synchronous fixture now explicitly rebuilds text at its declared Canvas scale. The final run passed 20/20 again; its initial and roundtrip Russian portraits have zero differing pixels. The main Russian portrait is also pixel-identical to the post-import main capture, so the fixture correction preserves the composition. This is a controlled capture comparison; automatic text refresh during a physical resolution change is not established.
- Source preservation audit: of 80 baseline paths, 74 are byte-identical; only the five authored existing files and external `.gitignore` differ. All seven reviewed PNGs match R1 exactly; Home GUID and five original object IDs are retained. All 17 newly added metadata GUIDs occur exactly once across 2,161 Assets metadata files.
- Splash reference pixel comparison against the earlier responsive-fix capture found only 44 differing pixels, each at most 1/255 in a channel. All protected Splash source/art/metadata hashes remain unchanged.
- Recorded exact test summaries and capture metadata under `UnityVerification/`. Final results are `FinalEditMode.json` and `FinalPlayMode.json`; intermediate failures and the earlier capture-fixture comparison are retained under explicitly historical filenames. `HomeLanguageRoundtripPixelComparison.json` records zero differing pixels.
- Scoped source/documentation whitespace checks passed. Unscoped `git diff --check` reports only Unity-authored blank `m_Name: ` fields in Home.prefab; the serialized output is retained unchanged.

## Scope and remaining verification

No APK build, phone installation/rendering/navigation/performance test, physical display-resolution-change text-refresh test, full project regression, release/custom-signing action or exact source-font identity verification was performed for this Home task. The earlier Development APK predates Home implementation. Existing Android guards, packages, Player/signing settings, Splash and Loading visual assets remain preserved.

An external commit `dc362f1` appeared during work and included some in-progress Home files; an external `.gitignore` edit was also observed. Root and agents did not make that commit or reset those changes. Source backups and the manifest establish this task's authored changes independently of current Git status. A desktop Unity instance subsequently acquired the project lock; it was not killed and its lock was not deleted. The user saved/closed it and authorized continuation. The Home-only import and verification then ran normally; no automatic approval rejection occurred.

Implementation and focused verification are complete. Review the actual standalone portrait in `UnityVerification/Home/Home_941x1672_RU.png`; the previous APK must be rebuilt before testing this Home implementation on a phone. Final user acceptance of the rendered Unity page remains separate from the authorization to implement R1.
