# Home reference rebuild R3 — implementation and verification

> Latest user review: **Home R3 visual match rejected.** A direct recheck confirms substantial composition and asset mismatches. The implementation/test results below remain historical functional evidence. See [current mismatch audit](QA/UserRejectionAudit/Findings.md); visual correction is required.

The active Unity Home page now uses **22 independently generated foreground PNGs**, with a more radiant Firebird, a reference-led uppercase crest, an emerald Play control, four navigation cards, a carved-wood footer and complete top resource controls. The original whole Home reference was inspected against both the historical R1 browser composition and actual R2 Unity output. R2 followed a simplified composition and did not match the original. This delivery replaces Home foregrounds and their native layout; it does not alter the accepted Splash or Loading visual assets.

Project: `/home/k2/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL`. Recorded and executed Unity: **6000.5.9f1 (b57deb96f08d)**. Active Home UI revision **6**, saved and read back through its existing owner. User visual acceptance is pending. This is an implemented reconstruction; exact reference geometry, individual painted details and numerical color identity are not claimed.

## Review the actual result

[Standalone browser review](Home_Reference_R3_Review.html) displays one portrait at a time, with actual Unity Russian/English/phone captures and the original reference. It is a review viewer, not runtime screenshot UI. The older `R1/Home_Alignment_R1.html` remains historical.

[Actual Unity portrait](UnityVerification/Home/Home_941x1672_RU.png) · [Simulated phone safe area](UnityVerification/Home/Home_576x1280_safearea_RU.png) · [Independent visual comparison](QA/FinalVisualReview.md).

## Assets and provenance

All final PNGs are saved under `Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/`; exact unchanged source copies are under this report's `Sources/`. [SelectedAssets.json](SelectedAssets.json) maps each source/runtime path, dimensions and SHA-256. Thirteen existing PNGs are replaced in place, retaining GUIDs; nine new PNGs and their unique metadata GUIDs are added. The preserved background is the twenty-third texture in Home's resource contract.

Every selected image was generated with the **built-in imagegen tool**, following the imagegen skill. No CLI/API fallback, screenshot extraction, screenshot-region crops, full-page runtime texture, or post-generation pixel manipulation was used. Original generator files match source and runtime hashes. True transparency was checked by decoded alpha and direct black/white/saturated-magenta inspection. The RGB HUD is an intentionally opaque panel; native solid backings keep nearly opaque RGBA interiors visually solid. Render colors remain untinted; PNGs use sRGB import. The hero and perched Firebird form one independently generated foreground pair.

Final prompt/provenance records:

- `HeroRefinementPrompt.json` and `LogoRefinementPrompt.json`: final paired hero/Firebird and crest.
- `NavRefinementPrompts.json`: final four navigation cards.
- `QuestRewardGemPrompt.json`: dedicated bulky hexagonal footer gem, distinct from the top currency diamond.
- `TopAssetPrompts.json` and `TopAvatarRevisionPrompt.json`: HUD, final avatar, resource capsule, coins/gems/lives, settings and plus control.
- `RootSelectedAssets.json` / `FooterPrompts.json`: final Play base, wood quest frame, progress trough/fill and reward compartment; initial hero/logo entries are superseded by the refinement records.
- `NavFooterPrompts.json`: final mail/reward medallions; initial navigation entries are superseded.

Initial checkerboard attempts and superseded candidates remain explicitly historical in `QA/Candidates/` and the initial prompt records. They are not runtime dependencies. Final isolation evidence is in `QA/TopAlphaChecks.json`, `QA/RootAlphaChecks.json` for unchanged root items, `QA/HeroRefinementAlphaChecks.json`, `QA/LogoRefinementAlphaChecks.json`, `QA/NavRefinedAlphaChecks.json`, `QA/NavFooterAlphaChecks.json` for unchanged medallions, and `QA/QuestRewardGemAlphaChecks.json`.

| Selected foreground | Source dimensions | Surface |
| --- | --- | --- |
| `avatar_portrait.png` | 1161 × 1355 | RGBA; genuine exterior alpha |
| `coin_icon.png` | 1254 × 1254 | RGBA; genuine exterior alpha |
| `energy_icon.png` | 1254 × 1254 | RGBA; genuine exterior alpha |
| `gems_icon.png` | 1254 × 1254 | RGBA; genuine exterior alpha |
| `home_hero_firebird.png` | 1086 × 1448 | RGBA; genuine exterior alpha |
| `home_logo.png` | 1448 × 1086 | RGBA; genuine exterior alpha |
| `home_play_button.png` | 2172 × 724 | RGBA; genuine exterior alpha |
| `hud_frame.png` | 2164 × 727 | RGB; intentionally opaque HUD panel |
| `mail_medallion_generated.png` | 1254 × 1254 | RGBA; genuine exterior alpha |
| `nav_characters_generated.png` | 1254 × 1254 | RGBA; genuine exterior alpha |
| `nav_events_generated.png` | 1254 × 1254 | RGBA; genuine exterior alpha |
| `nav_map_generated.png` | 1254 × 1254 | RGBA; genuine exterior alpha |
| `nav_shop_generated.png` | 1254 × 1254 | RGBA; genuine exterior alpha |
| `progress_fill.png` | 2172 × 724 | RGBA; genuine exterior alpha |
| `progress_frame.png` | 2172 × 724 | RGBA; genuine exterior alpha |
| `quest_panel.png` | 2172 × 724 | RGBA; genuine exterior alpha |
| `quest_reward_frame.png` | 1122 × 1402 | RGBA; genuine exterior alpha |
| `resource_capsule.png` | 2172 × 724 | RGBA; genuine exterior alpha |
| `resource_plus.png` | 1254 × 1254 | RGBA; genuine exterior alpha |
| `reward_medallion_generated.png` | 1254 × 1254 | RGBA; genuine exterior alpha |
| `settings_medallion.png` | 1254 × 1254 | RGBA; genuine exterior alpha |
| `quest_reward_gem.png` | 1254 × 1254 | RGBA; genuine exterior alpha |

## Active ownership and behavior

The active prefab is `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab`, owned by `Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs` and `Scripts/Runtime/UI/ProductionHomeRuntime.cs`. `ProductionHomeView.cs` owns live bindings, `ProductionHomeLayout.cs` owns safe-area placement, and `LockedDesignImporter.cs` defines the narrowly scoped Home imports. These are the existing `Assets/LegendsOfRus/` paths; no new startup, routing system, Canvas or EventSystem was introduced.

All 212 original prefab object/component IDs survive; 102 are added. The nine original actions remain bound, and three visible resource plus buttons use the established Shop route. Pointer tests exercise the real GraphicRaycaster and existing navigation/story gates. Coins, gems, energy and level follow current GameState. The daily reward badge follows the persisted UTC claim rule. The project has no persisted blue-gem quest or experience measurements, so their progress/reward values show dashes and the generated blue fill remains empty/hidden. Reference numbers such as `15/30` and `250` are not fabricated.

The complete logo sits below the header. The newly bound header backing moves with its safe-area frame; this fixes the detached dark rectangle found in the first tall-screen R3 portrait. The refined hero places the broad Firebird tail along the right side. Play uses a taller native Image while its hit rectangle ends at navigation. Navigation artwork expands within non-raycast children to compensate for source alpha margins, while the four original hit rectangles remain separated. Live 25-logical-pixel captions fit the newly enlarged plaques.

Loading prepares **23 distinct textures plus one prefab: 24 actual preparation operations**, using its existing measured loading path. No progress timer or invented quest progress was added.

## Checks performed

- Home-only owner import, C# compilation, save and revision-6 readback completed successfully in the installed Unity Editor.
- **30/30 Edit Mode cases passed; zero failures/skips.** Home imports for all 23 textures, source-byte equality/dimensions/sRGB/alpha/filter settings, native hierarchy/bindings/revision, reference-only dependency exclusion, full resource preparation contract and preserved Splash wiring.
- **21/21 Play Mode cases passed; zero failures/skips.** Live state and reentry; RU/EN labels; complete resource digits and caption glyphs; twelve visible controls through real pointer routing; duplicate-action prevention; chapter/story gates; daily reward availability; Splash → Loading → Home; real preparation/progress, cancellation, failure/retry and single completion. First-canvas and repeated safe-area checks cover the header backing, whole crest, separate hit rectangles, full background/shading coverage and control/text containment.
- Seven actual Home fixtures were captured using **OpenGLCore**: 941×1672 RU, 576×1280 RU with simulated safe insets, 1080×2340 RU, 1080×2400 RU with simulated safe insets, 941×1672 EN, RU language roundtrip, and 576×1280 maximum balances. Existing Splash/Loading regression captures are retained too.
- Direct and independent visual comparison checked original reference, materials, silhouette, labels, ornament clearance, opacity, safe areas and the former bottom/header seams. Exact results and differences are in `QA/FinalVisualReview.md`.
- Final Russian and RU → EN → RU captures are **pixel-identical: zero differing pixels**. An earlier 75-pixel difference in the reward badge exposed a missing badge entry in the synchronous capture fixture's text refresh list; the fixture was corrected and all 21 Play Mode cases rerun. No production typography changed for this fixture correction. Edit Mode checks ran on the final production files before that test-only change.
- Preserved Splash portrait comparison: 28 pixels differ by at most 1/255 per channel. Preserved Loading portrait comparison: 58 pixels differ by at most 1/255. Protected source hashes are unchanged; these capture comparisons are not claims of numerical identity for reconstructed Home.
- Baseline preservation audit: 1,465 recorded files; 1,431 unchanged and 34 expected modifications, no missing/unexpected changes. Thirteen replacement GUIDs preserved; nine new GUIDs unique. Background source/metadata/object blocks, Splash/Loading assets, package files and project settings unchanged.
- Exact production inventory: **52 files — 34 modified, 18 added** — in `SourceChangeManifest.json`. Modified files are 13 PNGs, 13 import metadata files, seven C# files and Home.prefab. Added files are nine PNGs and nine metadata files. Authored source/document whitespace and JSON records were checked; Unity's serialized empty-field whitespace is preserved.

Parsed final case results are in `UnityVerification/FinalEditMode.json` and `FinalPlayMode.json`. Pixel checks are read-only decoded PNG comparisons. Initial failed, intermediate passing and pre-badge-fixture runs remain historical evidence. Source diffs against task-start backups are in `ExistingSourceChanges.diff`.

## Remaining differences and checks not performed

The preserved winter background is brighter and more daytime-oriented than the original dusk reference. New crest curves, face/pose, feather structure, filigree and small item details remain generated interpretations. The shared live font retains the existing assignment/fallback; its exact reference identity and the reference's painted lettering bevel are unverified. Extreme 32-bit currency values remain complete but small at 576px width. These differences are visible and are not marked exact or approved.

No APK/AAB was built, no phone installation/rendering/input was tested, and no performance/full-project regression, physical display-resolution-change test, release/custom-signing validation or exact font identification was performed. The previous APK predates this Home implementation. Android guards, packages, Player/signing configuration and shared Splash/Loading visuals were not changed. No fonts or secrets were copied. No user processes were killed, locks deleted, files staged/committed/reset, or earlier edits discarded.

CurrentTask.md and SessionLog.md retain earlier acceptance history and record this implementation, verification and remaining fidelity limits. Next action: review the final Unity portrait; user visual approval remains pending.
