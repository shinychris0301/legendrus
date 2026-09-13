# Home R7 — Event and Gift badge depth polish

Implemented native Home revision **10**. The two flat red badges now have a lit ruby dome, beveled gold rim, translucent contact shadow and lightly shaded live symbols. Home R6 remains the accepted overall design. This pass changes only badge appearance.

[Before/after review with 4× badge views](Home_BadgePolish_R7_Review.html) · [Actual Unity portrait](UnityVerification/Home/Home_941x1672_RU.png)

## Implementation

`ProductionHomePanel.cs` adds an opt-in badge style using native UI geometry and vertex colors. Only Event and Gift enable it. Gold bands receive directional lighting, the red dome receives diffuse/specular shading, and a small translucent shadow separates each badge from its icon. Existing panel rendering stays unchanged when the flag is off. This is a raised appearance within the existing 2D UI, with no added camera, light, material or texture dependency.

`ProductionHomeRuntime.cs` enables that style and applies the existing text geometry, Shadow and gold-text components to the two live symbols. The fixed Event ! remains reference decoration; the Gift 1 still follows actual daily reward availability and disappears after claiming. Neither badge intercepts input. Text stays live and uses the existing local font configuration. The badge rectangles remain 46×46 and 44×44 at their original positions.

`ProductionHomeView.cs` and the active `Resources/ProductionUI/Pages/Home.prefab` now agree on revision 10 and the two required badge-style flags. The existing editor owner `Step20ProductionUiFix01SerializedPrefabBuilder.RebuildHomeAlignment` compiled, rebuilt, saved and read back this prefab; its source needed no change because it already consumes the runtime revision and resource contract. Home still prepares 25 textures plus 1 prefab through 26 measured operations.

No PNG generation, replacement, editing or screenshot-region asset extraction occurred. All 26 existing Home PNG files, including the retired unused sprite, remain byte-identical. The accepted background, characters/Firebird, logo, Play button, HUD, navigation, footer and all existing layout transforms remain unchanged. No additional suggested polish was applied to them.

## Checks performed

- Unity **6000.5.9f1 (b57deb96f08d)** Home-only import/compilation/save/readback: passed, exit 0.
- **36/36 Edit Mode and 21/21 Play Mode passed on the first run**, zero failures/skips. Existing focused tests were extended to verify exactly two opted-in badges, preserved rectangles, noninteractive artwork, directional mesh colors, finite/bounded vertices, a translucent external shadow and complete live glyph/shadow geometry. Existing daily reward hide/reappearance, 12-control routing, language, safe-area, real loading/cancel/failure/retry and Splash/Loading checks passed.
- Seven unaltered actual Unity Home PNGs retained with fixture JSONs; four portraits were directly inspected: reference-size RU/EN, 576×1280 safe area and 1080×2400 safe area. Native lighting reads at ordinary size and in the 4× review. These are Editor renders with simulated displays, not Android screenshots.
- Pixel comparison against accepted R6 at 941×1672: **4,307 changed pixels, all confined to the two badge/shadow rectangles; zero changed pixels elsewhere**. Splash and Loading regression captures are pixel-identical to R6. RU language roundtrip differs in two Play-letter pixels by at most 1/255 per channel; it is not pixel-identical.
- Local Chrome review checks: all six image selections loaded without horizontal overflow; six Event/Gift zoom checks across current/before/original passed. The magnifier displays complete existing images through a viewport; it creates no runtime crop asset. Viewer screenshots are labeled separately from raw Unity captures.
- Source audit: **1,489 recorded baseline files, 1,483 unchanged, exactly six intended modifications, no additions in the declared scan scope**. All **772 audited existing metadata files/GUIDs**, all **369 previous prefab IDs**, and all **84 RectTransform blocks** are preserved. Six text-effect component IDs were added, for 375 total. All five changed C# files passed whitespace checks. Protected Splash, Loading, package, ProjectSettings and Android build/signing paths match their baseline hashes.

Exact test cases and invocation settings: `UnityVerification/FinalEditMode.json`, `FinalPlayMode.json`, `Import.json`, `Reproduction.json`. Capture hashes: `UnityVerification/CaptureManifest.json`. Pixel results: `HomeBadgePixelComparison.json`, `SplashPixelComparison.json`, `LoadingPixelComparison.json`, `LanguageRoundtripPixelComparison.json` in that directory. Source paths/hashes/diff: [SourceChangeManifest.json](SourceChangeManifest.json), [FinalSourceAudit.json](FinalSourceAudit.json), [AppliedSourceChanges.diff](AppliedSourceChanges.diff).

An exploratory expansion of the additions scan initially flagged legacy ProductionUI folders absent from the recorded R6 baseline. The scan was corrected to the established Scripts/Tests/HomeAligned scope; the 1,489 baseline hash checks remained unchanged and passed. This was an audit-scope correction, with no production correction or Unity failure.

## Boundaries

No APK/AAB build, physical-phone installation/render/input, full-project regression, performance/memory profiling, physical display-resolution-change test or release/custom-signing validation ran. Android guards, packages, signing/Player settings and approved Splash/Loading sources remain unchanged. No credentials, keystores, OS font files, commits, staging or user-edit resets were involved.

The new badge lighting is a local approximation of the reference material, not a claim of pixel identity. The original font remains unidentified. Earlier optional Play-letter/frame/mail refinements and broader illustration differences are unchanged; Home's acceptance is preserved. CurrentTask and SessionLog record this focused delivery.
