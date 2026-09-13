# Visual Page 05 — Chapter Map R1

The canonical Chapter Map — **«Глава 1 — Родная деревня»** — is implemented at native revision 1, ready for user visual review.

[Open the complete-portrait review](ChapterMap_Alignment_R1_Review.html). It presents one actual Unity portrait at a time: Russian, English, simulated phone/tall safe areas and earned progress, alongside the original reference and previous Unity implementation.

## Scope and preserved baseline

Root: `/home/k2/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL`. Recorded and tested Editor: **6000.5.9f1 (b57deb96f08d)**. Root AGENTS.md, handoff, CurrentTask.md and reference provenance were read; the local working tree was inspected before editing. All 1,537 preceding World Map R1 production hashes matched.

The handoff's ColorFix06/PrefabFix07 Splash baseline is historical. Later user approvals establish Home R11 revision 14, Splash 7 and Loading 4; this task preserves those pages and the delivered World Map R1. No new World Map acceptance is inferred from the request to move on.

The active page remains `Assets/LegendsOfRus/Resources/ProductionUI/Pages/ChapterMap.prefab`, owned by `Step20ProductionUiFix01SerializedPrefabBuilder`, routed through `ScreenRouter` and `ProductionUiFix01Runtime`. The old page had background seams, retained screenshot stars and flat controls; level selection was one large `LevelPath` hotspot. Its before images were captured and reviewed in Unity before production edits.

Original reference: `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/ChapterMap.png`, SHA-256 `e69143c5ec1d2122fed9e177c8b0cc48bee0cdc657d50080a2cf7113023122bf`. It is provenance, not a runtime dependency. Existing Fix01 art, Home assets and active progression/router code were inspected.

## Appearance and interaction

- A new full-bleed, clean village environment contains a blue church, golden domes, windmill, carved wooden houses, stream, birches and lantern-lit cobblestone path. Home's existing scene tone and lower shading are reused.
- Separate red velvet/gold/sapphire banner and navy/gold level medallion sprites provide opaque surfaces for live text. The actual Home HUD, resource icons, profile, Settings, footer, emerald Play artwork and gold lettering material are reused.
- Ten individually interactive levels show real unlock state, live numbers, selected-level glow and saved best stars. Empty stars have dark interiors and gold outlines; earned stars retain dimensional native facets. Back reuses the generated medallion with a native gold arrow.
- Touch targets stay inside simulated safe areas. Scenery covers the physical display; node spacing, top HUD and bottom controls adapt to available portrait height. Star rows are checked against other level frames for overlap.
- Selecting an unlocked node calls the existing `GameState.SelectLevel` and remains on the map. Play uses the unchanged `OpenSelectedLevel` story-intro/pending-story/PreLevel flow. Back returns World Map. Profile, Settings and resource actions use the existing destinations.
- Russian and English labels remain live. Existing local typography assignments are reused, including the separate Start-font resolution for Play. Exact reference font identity remains unverified; no font files were copied.

Chapter 1 contains **ten authored levels**. The reference's labels 10–16 do not create additional content. A fresh save has level 1 available and no earned stars. The progress fixture completes level 1 through the real completion API, shows its three earned stars and selects unlocked level 2. Locked taps report the existing progression requirement without changing the save. No new economy, progression, quest or startup service is introduced.

The artwork is a reference-informed recreation, not a pixel-identical replica. Scene geometry, banner filigree and node distribution differ from the original. The layout accommodates ten authored levels with distinct star rows; its scene is not a screenshot under invisible hotspots.

## Art provenance and alpha

The built-in image generator produced three final whole-image resources; the environment had one composition refinement to leave the church and windmill visible below the banner. [Generation records](Generation.json) and complete source images are retained under Sources. The final environment is copied byte-for-byte into `ProductionUI/ChapterMapAligned`.

Both controls arrived as RGBA with a transparent exterior and nearly opaque interiors. Under the user's existing **“Allow local transparency cleanup”** authorization, alpha values at least 245 were restored to 255. RGB pixels are unchanged; silhouette edges and intentionally solid velvet/navy/gems are preserved. Both final controls were directly viewed over black, white and saturated magenta. [Exact alpha/source checks](AssetAlphaChecks.json). No screenshot crops, stitched screenshot regions or full reference UI image are used at runtime.

The importer branch is limited to new ChapterMapAligned art and uses the established full-rectangle Texture2D sprite contract: original alpha, uncompressed, no Read/Write, mipmaps/trilinear for small controls, bilinear/no mipmaps for the scene. Existing assets, package versions, backend/color space, Android guards, signing/build code and project settings are preserved.

## Verification

- Unity 6000.5.9f1 compiled, imported and saved/read back the canonical ChapterMap prefab at revision 1. Four owner launches succeeded; no C# or shader compilation errors were found in their logs or the final test logs.
- **50/50 final focused Edit Mode tests passed**, zero failures/skips: Chapter Map serialization, ten distinct level buttons, 17 total controls, whole-sprite dependencies, import shape/alpha, shared Home art/typography, plus existing Home/Splash/World Map checks.
- **32/32 final Play Mode tests passed**, zero failures/skips: real unlock/earned-star/selected-level state, locked taps without save mutation, persisted selection across navigation, intro and PreLevel routes, HUD actions, Back hierarchy, native meshes, per-button GraphicRaycaster hits, responsive bounds and star-row/frame separation. Existing Home, Splash, Loading, actual Home preparation and World Map regressions are included.
- Six final Chapter Map portraits with fixture JSON were captured through the actual router Canvas: RU, EN, RU roundtrip, 576×1280 safe area, 1080×2400 safe area and earned progress. RU/EN/phone/tall/progress images were directly inspected. **78 unedited final capture/fixture files** are retained across all five pages, with source/copy hashes.
- Pixel comparisons against the preceding World Map R1 delivery: all eight fixed Loading portraits are exact; five of seven Splash portraits are exact and two differ at one pixel by 1/255. One of seven Home portraits is exact; six differ at 1–26 pixels by at most 1/255. Two of eight World Map portraits are exact; six differ at 2–5 pixels by at most 1/255. The new Chapter Map RU roundtrip differs at 40 pixels by at most 2/255, including 16 alpha values; it is not claimed pixel-identical. [Exact per-image counts](UnityVerification/RegressionPixelComparison.json).
- Real Splash→Loading→Home still records 27 resource operations, 28 observed progress samples from 0 to 1, visible completion and two route changes. No injected progress or timer. [Flow fixture](UnityVerification/LoadingRegression/ActualLoadingFlow.json).
- Chrome loaded all seven review selections, all three report links and the 360px viewer without horizontal overflow. [Browser checks](QA/StandaloneReviewBrowserChecks.json). This verifies the viewer, not new Unity rendering.
- All 1,537 baseline production paths audited: **1,531 unchanged, six modified, 19 additions**. All **797 existing metadata files**, their GUIDs and five original ChapterMap object IDs are preserved; new GUIDs are unique. The final prefab contains 610 serialized objects/components. Home 14, Splash 7, Loading 4, World Map 1, FrontEnd art, packages, project settings and Android guards/signing code are unchanged.
- New source PNG hashes, unchanged source RGB, control alpha, C# whitespace/newlines and delivery-file hashes were checked. CurrentTask.md and SessionLog.md are updated with prior history retained.

[Final Edit Mode cases](UnityVerification/FinalEditMode.json) · [Final Play Mode cases](UnityVerification/FinalPlayMode.json) · [Run history](UnityVerification/TestRunHistory.json) · [Import/compile history](UnityVerification/ImportAndCompileHistory.json).

Earlier baseline capture passed 1/1; initial Chapter Map Play Mode passed 3/3. The first wider runs passed 50/50 and 32/32, but direct image review exposed overlapping star rows missed by the original input checks. The final spacing correction and explicit rendered-bounds assertion address that gap. Earlier captures/results remain under QA/Iterations and UnityVerification. No test tolerance was relaxed to hide the overlap.

## Changes and reproducibility

[Changed-file manifest](SourceChangeManifest.json) · [Applied diff against the actual local baseline](AppliedSourceChanges.diff) · [Preservation audit](FinalSourceAudit.json) · [Unity launch arguments/fixtures](UnityVerification/Reproduction.json).

Only the canonical ChapterMap prefab and five existing owner/importer/router/test files change. New files supply Chapter Map Runtime, View, Layout, Decoration, two focused test fixtures, three art resources and their metadata. CurrentTask.md and SessionLog.md record this task without erasing prior acceptance.

## Not performed

No APK/AAB or Android Player build, phone install/render/touch test, release/signing check, full-project test suite, GPU/frame-time/memory benchmark, or physical resolution-change test. Editor captures simulate screen sizes and safe areas. Route assertions invoke native button listeners and verify GraphicRaycaster hits; they are not physical-touch tests. Chrome checks verify the review viewer, not fresh Unity rendering. The existing APK predates this implementation.

The next step is user visual review of this Chapter Map. Pre-Level is the next page in the existing game flow; it is not changed by this task.
