# Splash and Loading — Home style alignment R1

Implemented for review on 2026-09-13. **Home R10 is accepted and unchanged.** Splash revision **7** and Loading revision **4** now share its visual family. The user's latest request explicitly authorizes this Splash/Loading redesign; the earlier ColorFix06 + PrefabFix07 acceptance remains historical, with its original files and GUIDs preserved. New visual acceptance is not inferred.

[Open the standalone portrait review](FrontEnd_HomeStyle_R1_Review.html). It displays one actual Unity capture at a time, with before/after, RU/EN, narrow/tall safe-area views and the accepted Home reference. No screenshot is used as runtime UI.

## Result

- Splash reuses the **exact existing Home logo, complete emerald button sprite, gold lettering material and clean environment**. Its live label remains **Начать / Start**. The logo retains its prior top/width placement with height fitted to the Home crest. Only button artwork proportions change; all five control hitboxes remain intact.
- Splash Ivan follows Home's younger, clean-shaven character and sapphire/crimson embroidered costume. Masha retains her established braids, jeweled kokoshnik and traditional costume, with compatible dusk lighting. They are separate transparent sprites, retaining the existing bottom fade and character rectangles.
- Loading reuses the same Home crest, with a new clean autumn-dusk journey environment and a separately illustrated walking Ivan. Its existing standalone Firebird, parchment, native progress bar, live labels and error/retry controls remain in place.
- Both pages use Home R10's neutral environment multiplier **235/255** and character/Firebird multiplier **242/255**. Home artwork, materials, geometry, colors, navigation and services are unchanged.
- The existing bootstrap/router remains the authority. Home preparation still measures **27 real resource operations**. No timer, replacement startup system, duplicate Canvas/EventSystem or new route was introduced.

## Assets and provenance

All new runtime files are in `Assets/LegendsOfRus/Resources/ProductionUI/FrontEndHomeAligned/`:

| File | Runtime use | Source |
|---|---|---|
| `masha_dusk.png` | Splash Masha | New lighting variant, then authorized alpha cleanup |
| `ivan_splash_dusk.png` | Splash Ivan | New Home-style portrait, then authorized alpha cleanup |
| `ivan_loading_dusk.png` | Loading Ivan | New walking pose, then authorized alpha cleanup |
| `loading_environment_dusk.png` | Loading environment | New clean plate; generator output copied unchanged |

The **built-in image_gen** tool created the illustrations. Full prompts, source references, saved paths and hashes: [PromptSet.json](PromptSet.json). Extraction retry prompts: [AlphaRetryPrompts.json](AlphaRetryPrompts.json). Final complete sources are under [Sources/Final](Sources/Final); these are byte-identical to their runtime copies. Every new asset/folder has a new unique GUID; no existing metadata was overwritten.

Initial character generation and extraction retries produced RGB images with painted checkerboards. Those raw files were rejected for runtime use. The user explicitly answered **“Allow local transparency cleanup.”** Local `rembg`/`isnet-general-use` alpha matting then removed those backgrounds. Source RGB was restored in solid character interiors; soft boundary pixels retain matting. Dependencies/model stayed in `/tmp`, outside Unity. This was local cleanup of complete newly generated illustrations, with no reference screenshot cropping or source-font copying. No image API/CLI generation fallback was used.

[Cleanup procedure and hashes](Sources/LocalAlphaCleanup.json) · [Reproducible cleanup script](QA/CleanCharacterAlpha.py) · [Black/white/magenta inspection](QA/CharacterAlphaReview.html) · [Decoded alpha evidence](QA/CharacterAlphaChecks.json).

All three final 1024 × 1536 character sprites have real RGBA transparency. Their complete silhouettes, hair/fur/cloak edges and opaque skin/clothing were directly inspected over black, white and saturated magenta. No painted checkerboard or scene rectangles remain visible.

## Active implementation and exact production changes

Root: `/home/k2/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL`. Recorded and executed Unity: **6000.5.9f1 (b57deb96f08d)**. `Assets/Scenes/Bootstrap.unity` continues through the existing bootstrap and ScreenRouter to Splash → Loading → Home.

Thirteen existing files changed under `Assets/LegendsOfRus/`:

| Path | Change |
|---|---|
| `Resources/ProductionUI/Pages/SplashTitle.prefab` | Shared Home art/material, new characters, tints, revision 7 |
| `Resources/ProductionUI/Pages/Loading.prefab` | Shared crest, new Ivan/plate, tints, revision 4 |
| `Resources/ProductionUI/SplashTitleConfig/SplashTitleLayout.json` | Matching resource paths, tints and two artwork rectangles |
| `Scripts/Editor/Step20SplashTitleR3PrefabRebuilder.cs` | In-place owner migration, shared Start lettering, combined narrow build entry |
| `Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs` | In-place Loading migration and existing readback validation |
| `Scripts/Editor/SplashTitleR3Validation.cs` | Recognize the shared Home button's authored display proportions |
| `Scripts/Runtime/UI/SplashTitleView.cs` | Revision 7 and required Home-style bindings |
| `Scripts/Runtime/UI/ProductionLoadingRuntime.cs` | New resource constants, matching runtime construction and in-place editor migration |
| `Scripts/Runtime/UI/ProductionLoadingView.cs` | Revision 4 and required new bindings |
| `Tests/ProductionCoreLoopUiEditMode/HomeAlignmentDeliveryEditModeTests.cs` | Exact shared logo/button identity, retention of old Splash asset GUIDs |
| `Tests/ProductionCoreLoopUiEditMode/SplashTitleBuildWiringEditModeTests.cs` | Shared branding, final full-source/alpha/import checks |
| `Tests/PlayMode/SplashResponsivePlayModeTests.cs` | New crest height, EN capture, settled native button colors, correct glyph capture refresh and exact roundtrip |
| `Tests/PlayMode/LoadingAlignmentPlayModeTests.cs` | Narrow safe-area/EN/roundtrip captures and correct glyph capture refresh |

Nine production files were added: the four PNGs above, their four `.meta` files and the folder `.meta`. [Exact change manifest](SourceChangeManifest.json), [owned text diff](AppliedSourceChanges.diff), [final source audit](FinalSourceAudit.json) and [final production hashes](FinalProductionHashes.json) retain the evidence. Existing local changes form the baseline; this is not a claim that the entire Git tree is clean.

The Splash owner preserved **all 117 existing serialized IDs**, adding one gold text component. Loading preserved **all 68 IDs**, adding none. Splash's Logo and Start Artwork are the only changed RectTransform blocks; Loading's logo is its only changed RectTransform block. All other **24 Splash and 15 Loading** RectTransform blocks are byte-identical, including character geometry, control hitboxes, labels, navigation and footer layout. Existing safe-area callbacks and full-display shade coverage remain in force.

## Checks performed

- Required root instructions, handoff, current task, reference provenance, active paths and local changes inspected before editing. All **1,497** recorded Home R10 production hashes matched initially. Final audit: **1,484 unchanged, 13 modified**, all **776 existing metadata files** and **41 recorded PNGs** unchanged, including all **27 Home PNGs**. Protected Home group (77 paths), Packages (2), ProjectSettings (25), Android/build/signing source group (23) unchanged. No secret material was inspected.
- Installed Unity owner compiled, imported the four new resources, saved both active prefabs in place and passed revision/binding readback. Exit 0; no C# compiler or shader errors. [Import evidence](UnityVerification/Import.json).
- **43/43 Edit Mode tests passed**, zero failures/skips. [Results and individual cases](UnityVerification/FinalEditMode.json).
- **24/24 final graphics-enabled Play Mode tests passed**, zero failures/skips. Tests exercised actual Start → measured Loading → Home, error/Retry/double tap, cancellation/stale results, once-only navigation, progress pixels, existing Home bindings/routes, automatic layout, full-display background/shade coverage, language input and portrait geometry. [Results and individual cases](UnityVerification/FinalPlayMode.json).
- Observed real progress advanced from 0 to 1 through **28 samples / 27 completed operations**, with one visible completion frame and exactly **two route changes**. [Actual flow](UnityVerification/Loading/ActualLoadingFlow.json). The separate 42% portraits are explicitly test-injected layout fixtures, not claims about an observed resource fraction.
- Rendered portrait checks covered 941 × 1672 RU/EN; 576 × 1280 with simulated safe area; 1080 × 2340; and 1080 × 2400 with simulated safe area. Splash also checks a wide Editor view and portrait return. Directly inspected the eight final RU/EN/narrow/tall Splash/Loading portraits. Backgrounds cover the screen without a finite bottom seam; controls and text remain legible in these captures.
- **Nine exact pixel comparisons passed**: seven accepted Home R10 portraits unchanged, and Splash/Loading both return to identical RU pixels after language/size changes. [Pixel comparison](UnityVerification/ProtectedPixelComparison.json).
- Final **47 unedited capture/fixture files** retained with source hashes. [Capture manifest](UnityVerification/CaptureManifest.json). XML results and reproduction parameters retained alongside them.
- Headless Chrome verified all **12** review selections loaded with no image filter, full opacity and no horizontal overflow; the 360-pixel browser viewport also fits. [Browser checks](QA/StandaloneReviewBrowserChecks.json).
- Source whitespace, asset/source byte identity, import dimensions, whole-sprite rects, native alpha presence, new GUID uniqueness, and protected-file hashes checked.

The initial Play Mode run also passed 24/24, but subsequent review found its synchronous capture fixtures retained text meshes from a previous Editor Canvas scale. Splash RU roundtrip differed by 30,790 pixels. Home already refreshed those glyph meshes; the Splash/Loading fixtures now do the same and explicitly assert identical roundtrip pixels. The final rerun passes those stronger assertions. No production font, typography, color or timing change was made for this fixture correction. [Initial iteration and evidence](UnityVerification/InitialCaptureIteration/README.md).

## Not performed and remaining limits

No APK/AAB, Android shader/player build, phone installation/render/input, full-project regression, performance/memory profiling, release/signing checks or physical resolution-change validation ran. The existing APK predates this work. Editor safe-area tests do not establish physical-phone acceptance.

Exact source-font identity remains unverified. Existing shared typography/fallback assignments are unchanged; no OS/container font file was copied into the project. The new character illustrations are visual adaptations, not a claim of pixel-identical faces across poses. Original Splash/Loading assets remain available under their previous paths and GUIDs.

CurrentTask and SessionLog are updated; the open Home R10 viewer links here. **Next action: review these two implemented screens together with accepted Home R10.**
