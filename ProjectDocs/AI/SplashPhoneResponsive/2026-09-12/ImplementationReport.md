# Splash phone responsive correction — 2026-09-12

Implemented and rebuilt for Android testing. The Splash logo, version and language selector now follow the safe-area top on tall displays. Its existing footer shade continues smoothly to the physical display edges. Approved artwork, colors, native sizes, lower controls and reference composition are preserved. Physical-phone confirmation of this fix remains pending.

APK: `Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk` — 121,789,800 bytes (116.1 MiB). SHA-256: `8fd087e84e83debf1ed221ce185e9c2c612e274b2f8a18d22d92f9dd48938338`. The adjacent `.apk.sha256` records the checksum. Package `com.example.legendsofrus.v117debug`, version 0.0.0/code 1, ARM64, minimum Android API 26, target API 36, Portrait, Development. The verified Android Debug certificate matches the previously tested APK. The previous APK is preserved under `Builds/Verification/v1_17_step01/BeforeSplashResponsive/`.

Actual Unity portrait: [corrected phone-ratio capture](After/Splash_576x1280_inferredsafe.png). Compare [before correction](Before/Splash_576x1280_inferredsafe.png) and the [preserved reference size](After/Splash_941x1672.png). These are actual Unity 6000.5.9f1 Play Mode renders of the existing production Canvas, with simulated displays/safe areas. The 50px top inset is inferred from the user screenshot; phone model, Android version and exact insets were not provided.

## Implementation and preservation

The active prefab is `Assets/LegendsOfRus/Resources/ProductionUI/Pages/SplashTitle.prefab`, now revision 6. Its existing owner `Step20SplashTitleR3PrefabRebuilder.cs` performs an in-place migration for the complete revision-5 prefab and binds the same component on a full rebuild. All 116 original object IDs and every authored RectTransform block remain unchanged; one component was added. No artwork reconstruction occurred.

`SplashTitleForegroundLayout.cs` stores authored Logo/VersionText/LanguageSelector positions and moves them only by the extra safe-area top gap. It extends FooterContrast across the Canvas. `SplashTitleFooterShade.cs` preserves the original 366-unit gradient and continues its original final opacity below it. `SplashTitleView.cs` requires the new bindings/revision; `ProductionSplashTitleRuntime.cs` applies layout after stretching the existing root. Canvas callbacks also update it before graphic rendering and on dimension changes.

Five existing Assets files changed; four new source/meta files were added, including `SplashResponsivePlayModeTests.cs`. Exact paths/hashes and the reviewable patch are in [SourceChangeManifest.json](SourceChangeManifest.json) and [AppliedSourceChanges.diff](AppliedSourceChanges.diff). Both new script GUIDs are unique. All existing asset GUIDs, ten approved Splash PNGs and their metadata, layout/typography configuration, all 31 delivered Loading files, shared scaler/safe-area/background layout, packages and Player settings match their pre-change hashes. Existing unrelated changes are preserved. CurrentTask.md and SessionLog.md record progress and results.

## Checks actually performed

- Read project instructions, handoff/current task, active source/config/prefab, Unity version and working-tree changes before production edits. Saved source hashes and the previously tested APK.
- Captured the unmodified Splash in real Unity Play Mode; baseline 2/2 tests passed. The first baseline attempt found a new test-helper API mismatch, corrected to Unity's no-argument CanvasRenderer.GetMesh before rerunning. The baseline reproduces the screenshot's low header and bottom seam.
- Final focused Play Mode run: **13/13 passed**, zero failures/skips. Includes automatic first-Canvas-pass header/shade bounds, unchanged control/art geometry, actual footer mesh fade/constant-alpha continuity, repeated application, landscape-to-portrait roundtrip, real language-button pointer input exactly once, measured Loading progress, error/retry and Splash → Loading → Home regressions.
- Five portrait fixtures: 941×1672; 576×1280 full display; 576×1280 with inferred 50px top inset; 1080×2340; 1080×2400 with safe rect (45,100,990,2160). All were visually inspected. No header clipping/new overlap or bottom seam found. In the inferred phone fixture the header moves upward 103.27px and the shade reaches y=0 instead of stopping 103.27px above the display bottom.
- Reference-size before/after pixel comparison: only **36 of 1,573,352 pixels differ**, at most **1/255 per channel**. Reference and return-from-landscape PNGs are byte-identical within each run. See [PixelComparison.json](PixelComparison.json). Duplicate roundtrip PNGs are omitted; their geometry JSONs remain.
- Splash serialized/inactive-prefab wiring Edit Mode tests: **3/3 passed**, zero failures/skips. See [UnityTestResults.json](UnityTestResults.json). Independent read-only source and actual-image review found no blocker.
- Existing guarded Development Android build succeeded in Unity 6000.5.9f1, including Addressables, IL2CPP ARM64 and Gradle. No approval guard, package, signing or Player setting changed. The normal final guard reported `ANDROID_DEVELOPMENT_APK_ALLOWED`.
- Final APK: SHA-256; all 401 ZIP entries' CRCs; aapt package/version/SDK/ABI/orientation/debuggable checks; apksigner v2 signature and matching previous public certificate; zipalign 16KiB check; all six ARM64 ELF libraries' load-segment alignment; new responsive component present in packaged IL2CPP metadata. See [ApkVerification.json](ApkVerification.json).
- Source hashes rechecked after build. C# diff whitespace passed. Whole-working-tree whitespace check flags two normal Unity empty m_Name lines in existing Loading and updated Splash prefabs; no unrelated YAML normalization applied.

The first successful APK retained 88,033,975 bytes of unused incremental ZIP space. A physical-offset audit identified 1,346 empty local records. Moving only the generated Gradle APK and rerunning the same build produced the final compact APK above; it has no large unused gaps. No manual repacking/resigning occurred. Generated Addressables content state/link files are retained locally, separate from the authored patch. Raw logs/XML and duplicate captures stay in `/tmp/lor-splash-responsive`; caches and private signing material are excluded from evidence.

## Not performed

No installation or physical-device rendering, navigation or performance retest of the updated APK; no full project test suite or rerun of the historical 99 Android guard unit cases; no release/custom-signing build, publishing or exact source-font identity verification. Development font fallback remains unchanged. Static 16KiB alignment checks are not a device compatibility test. Simulated Unity captures and build success do not establish user visual acceptance.

Next action: install the updated APK over the existing test build and inspect Splash on the phone, then exercise Start → Loading → Home.
