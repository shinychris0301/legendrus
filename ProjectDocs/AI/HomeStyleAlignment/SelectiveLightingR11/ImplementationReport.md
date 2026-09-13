# Home R11 — selective character, Firebird and foliage lighting

The authorized refinement is implemented in the active Home prefab at **revision 14**, ready for visual review. Ivan's clothing has deeper shadows and warm directional edge light. The Firebird has stronger amber light beyond its feather silhouette; Map foliage has darker olive surfaces and a much fainter warm edge. The accepted R10 background tone remains. R11 acceptance is not inferred.

[Open the actual Unity before/after/reference review](Home_SelectiveLighting_R11_Review.html). It displays one complete portrait at a time, with optional detail magnification. These are actual Editor camera captures; phone dimensions and safe areas are simulated.

The original whole Ivan/Firebird and foliage PNGs remain untouched. Two Home-only material profiles use soft UV masks and existing sprite alpha. A mesh effect adds a padded glow quad behind each original surface in the same Image, preserving the original surface mesh, lower fade, rectangle and hitbox. Lighting follows Firebird > character > foliage in strength. There are no new textures, screenshot crops, duplicate UI objects or global bloom. The selected face region is pixel-identical to R10.

The existing Home owner calls the updated runtime alignment and saved/read back the canonical prefab. Runtime/View revision checks both expect 14. The new materials are prefab dependencies; loading still reports **26 texture operations plus the prefab = 27 real operations**. Existing routing, controls, labels, font assignments, badges and live lettering remain in place.

**Production changes:** five existing files modified; eight files added. Paths below are relative to `Assets/LegendsOfRus/`. Full exact diff and hashes: [AppliedSourceChanges.diff](AppliedSourceChanges.diff), [SourceChangeManifest.json](SourceChangeManifest.json).

| Modified path | Purpose |
|---|---|
| `Resources/ProductionUI/Pages/Home.prefab` | Revision 14, two material bindings and two mesh effect components |
| `Scripts/Runtime/UI/ProductionHomeRuntime.cs` | Owner/runtime material and effect setup |
| `Scripts/Runtime/UI/ProductionHomeView.cs` | Revision and required lighting binding checks |
| `Tests/ProductionCoreLoopUiEditMode/HomeAlignmentDeliveryEditModeTests.cs` | Whole-sprite, component-order and material dependency checks |
| `Tests/PlayMode/HomeAlignmentPlayModeTests.cs` | Rendered lighting/preservation checks and three-background diagnostics; adapt fade/foliage bounds assertions |

| Added path, each with its own `.meta` | Purpose |
|---|---|
| `Scripts/Runtime/UI/ProductionHomeSceneLighting.cs` | Original surface plus padded light geometry |
| `Resources/ProductionUI/HomeAligned/home_scene_lighting.shader` | Selective shadows, rim light and alpha-derived exterior glow |
| `Resources/ProductionUI/HomeAligned/home_hero_lighting.mat` | Hero/Firebird profile |
| `Resources/ProductionUI/HomeAligned/home_foliage_lighting.mat` | Foliage profile |

**Checks performed and results:**

- Inspected root instructions, handoff, task, provenance, active source/resources, previous captures and existing local changes. Root and recorded/used Editor: `/home/k2/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL`, Unity **6000.5.9f1 (b57deb96f08d)**. Latest accepted Home R10 and Splash 7/Loading 4 supersede historical handoff milestones.
- Unity owner import/compile/save/readback passed. Final Play Mode launch compiled and rendered the corrected shader on **OpenGLCore**; no C# or shader compile errors in retained log checks. [Import evidence](UnityVerification/Import.json).
- **44/44 Edit Mode** and **25/25 final Play Mode** tests passed, zero failures/skips. Fixtures and launch arguments: [Reproduction](UnityVerification/Reproduction.json). Test cases: [Edit Mode](UnityVerification/FinalEditMode.json), [Play Mode](UnityVerification/FinalPlayMode.json). Includes responsive geometry, safe areas, language roundtrip, control raycasts, routes, resource balances, real preparation/retry and selective lighting.
- [R10/R11 pixel comparison](UnityVerification/LightingPixelComparison.json): **290,708 changed RGB pixels**, all inside the two padded sprite bounds; **zero alpha changes**. RGB-sum thresholds identify 170,376 darker and 49,355 brighter pixels. Face and nine protected UI sample regions are identical; the Play/lettering region differs by at most **1/255**. Home RU roundtrip is exact. All **15** fixed Splash/Loading portraits are identical to accepted FrontEnd R1 captures. The variable-moment ActualLoading capture is excluded from exact image comparison.
- [Actual loading flow](UnityVerification/LoadingRegression/ActualLoadingFlow.json): 28 observed samples from 0 to 1, visible completion frame, exactly two route changes. Progress is operation-based, not a timer.
- Directly inspected final RU/EN/narrow/tall Home portraits and magnified Ivan/Firebird/foliage details against the accepted R10 and original reference. Directly inspected both lit sprites on actual Unity black, white and magenta backgrounds. Background RGB is also asserted. Original silhouette/face readability and lower fade remain; no rectangular glow plate or painted checkerboard was observed.
- [Browser checks](QA/StandaloneReviewBrowserChecks.json): six capture selections, nine detail selections and 360px browser width passed. This verifies the viewer, separately from Unity rendering.
- [Source audit](FinalSourceAudit.json): all **1,506** baseline production paths audited; **1,501 unchanged**. All **781** existing metadata files, **27 Home PNGs**, **383 original prefab IDs** and **86 full RectTransform blocks** preserved. Two new component IDs; no new rectangles. Splash/Loading, accepted FrontEnd art, packages, settings and Android guard/build/signing source groups unchanged. Four new metadata GUIDs are unique.
- [Capture manifest](UnityVerification/CaptureManifest.json): **50** unedited capture/fixture files copied with verified source hashes. Documentation updated: CurrentTask, SessionLog, this delivery and links from the R10/FrontEnd viewers. No generated cache files are part of the delivery diff.

Earlier iterations are retained in [TestRunHistory](UnityVerification/TestRunHistory.json) and [iteration notes](QA/Iterations/Initial/README.md). Initial tests caught glow alpha changes; direct review then caught hidden diagnostic background colors and a material-baseline gap. The final test removes the custom materials for the original-UI comparison. Final premultiplied blending preserves R10 composite alpha exactly. No comparison threshold was relaxed to obtain the pass.

**Not performed:** APK/AAB or Android shader/player build; physical-phone installation, rendering or input; full-project test suite; device GPU/frame-time/memory profiling; custom stencil/soft-mask stress cases; physical resolution-change or release/signing checks. The new shader's extra alpha sampling requires device performance validation. No image generation, raster editing or font copying was needed. Exact source-font identity remains unverified. Existing XP/quest/event-state gaps and other reference geometry/material differences remain outside this lighting change; this is not an exact reproduction claim.

Next action: review the R10/R11 toggle, especially Ivan, feather tips and Map leaves. Preserve the newly accepted Splash/Loading while reviewing Home R11; no further visual change is implied.
