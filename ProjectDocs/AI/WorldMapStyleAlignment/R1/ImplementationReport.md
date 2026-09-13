# Visual Page 04 — World Map R1

The canonical World Map is implemented in Unity at **revision 1**, ready for visual review. It uses a new clean landscape, independent title/navigation art, live chapter states and Home's accepted HUD. This result has not yet been accepted by the user.

[Open the complete-portrait review](WorldMap_Alignment_R1_Review.html). The viewer includes the original reference, previous Unity page, final Russian/English and simulated phone/tall captures, and the Tasks panel.

## Project and baseline

Project root: `/home/k2/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL`. Recorded and tested Editor: **6000.5.9f1 (b57deb96f08d)**. Root AGENTS.md, ConversationHandoff.md, CurrentTask.md and REFERENCE_PROVENANCE.json were read; existing local changes were inspected before editing.

All 1,514 recorded Home R11 production hashes matched initially. The handoff's ColorFix06/PrefabFix07 Splash acceptance is historical; later user approvals establish **Splash 7, Loading 4 and Home R11 revision 14**, all preserved here. The previous World Map was active through `ProductionUI/Pages/WorldMap` and the FIX01 runtime builder. Its purported clean plate contained leftover numbers/stars and visible extension seams. Before captures were produced in Unity and retained separately.

The available reference is `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/WorldMap.png`, SHA-256 `b13e01a68e3ef8641e6094abefabe34faa3c1a0d2e2cec9f7fb2a5afc7993e19`. Existing legacy map art, Home resources and the serialized active page were inspected. The reference is composition provenance; it is not a runtime dependency.

## Implemented appearance and behavior

- A newly generated landscape retains the reference's fortress, birch forest, river valley, wooden buildings, carved idol and blue shrine. No labels, chapter numbers or controls are baked into this environment.
- Four independently generated controls provide the blank title plaque and Map, Tasks and Guild medallions. Live Russian/English labels sit on their opaque navy surfaces. Home's actual HUD sprites, shared typography/material, scene tone and footer ornament are reused.
- Fifteen individual chapter buttons have native gold bevels, stars, locks and a luminous dotted path. The background covers the physical display; controls stay within simulated safe areas while chapter spacing adapts to the available height.
- Home's Map action still opens World Map; Home's Play action still opens Chapter Map. Chapter 1 uses the existing Chapter Map/story route. Back returns Home; hardware Back first closes Tasks when it is open.
- Profile, Settings, coin/gem additions, energy and Guild use the existing Profile, Settings, Shop, OutOfEnergy and TeamHub destinations. Tasks opens a local summary of real chapter completion. Map and Close dismiss that summary.

The existing progression model defines these as **chapter nodes**. Only **Chapter 1, with 10 authored levels**, is available. Chapters 2–15 remain locked after completing it; no missing chapter is silently unlocked. Chapter stars show completed thirds of the actual earned-star total, with the third star requiring all 30 available stars. Tasks displays the exact level/star totals. No new quest, reward, XP or guild backend is invented.

The new environment and ornament details are a reference-informed recreation, not a pixel-identical replica. Compared with the reference, Home's HUD/footer artwork is intentionally shared, a Back control is present, and the fresh-save state has locked chapters and unearned stars.

## Actual production changes

Six existing files changed relative to the inspected local baseline:

| File beneath Assets/LegendsOfRus/ | Change |
| --- | --- |
| `Resources/ProductionUI/Pages/WorldMap.prefab` | Serialized native composition and direct bindings |
| `Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs` | World Map-only owner/import/save/readback and validation |
| `Scripts/Editor/LockedDesignImporter.cs` | Import contract scoped to the new WorldMapAligned folder |
| `Scripts/Runtime/UI/ProductionUiFix01Runtime.cs` | Dispatch World Map to its native implementation |
| `Scripts/Runtime/UI/ScreenRouter.cs` | Bind individual controls, suppress duplicate HUD, dismiss Tasks on Back |
| `Tests/PlayMode/FunctionalDemoNavigationPlayModeTests.cs` | Exercise native World Map Back in the existing map hierarchy test |

Twenty-three files were added: four World Map runtime classes and metadata; two test classes and metadata; five new PNGs and metadata; one resource-folder metadata file. The runtime classes are `ProductionWorldMapRuntime`, `ProductionWorldMapView`, `ProductionWorldMapLayout` and `ProductionWorldMapGraphic`.

[Exact changed/added paths and hashes](SourceChangeManifest.json) · [Applied text diff](AppliedSourceChanges.diff) · [Preservation audit](FinalSourceAudit.json).

The audit finds **1,508 of 1,514 baseline files unchanged**, all **785 existing metadata files** byte-identical, all five original World Map prefab object IDs retained, and unique GUIDs for every new resource/script. Accepted Home/Splash/Loading source and art, packages, project settings and Android guard/signing code are unchanged. No caches or secrets are included in the delivery.

## Generated art and transparency

The built-in image generator produced five complete resources. [Prompts](Prompts/GenerationPrompts.json), [source manifest](Sources/GenerationManifest.json), [runtime-copy hashes](Sources/RuntimeInstallManifest.json) and [cleanup details](Sources/LocalAlphaCleanup.json) are retained. The clean environment is copied unchanged. No screenshot-region crop or complete reference screenshot is used as runtime UI.

The four controls arrived with painted checkerboards. The user's earlier explicit **“Allow local transparency cleanup”** authorization was reused, as communicated. Cached local alpha-matting tools were used outside the Unity project. A first Guild matte incorrectly removed part of the blue interior; inspection caught it and the original solid interior was restored. Opaque pixels retain the exact generated-source RGB. All four final resources were directly inspected over black, white and saturated magenta; navy panel surfaces remain opaque. [Alpha checks](QA/AlphaChecks.json) record dimensions, real alpha, empty corners, solid panels and source-RGB preservation.

New controls use full-rectangle 2D sprites with mipmaps/trilinear filtering for small-screen ornaments; the environment uses bilinear filtering without mipmaps. This importer change affects only new World Map resources. Existing local font assignments are reused. Exact source-font identity remains unverified; no OS/container font files were copied.

## Checks performed

- Unity owner compilation, import, prefab save and readback at revision 1 passed. The final owner pass includes the chapter-number stability adjustment.
- **47/47 focused Edit Mode tests passed**, zero failures/skips: World Map serialization, shared resources, whole sprites, import shape/alpha, dependency restrictions, plus existing Home and Splash/build-wiring checks.
- **29/29 final Play Mode tests passed**, zero failures/skips. They cover World Map state/routes, all 15 nodes, locked-state preservation, real completion/star updates, Tasks/Back/Guild/HUD routes, native meshes, GraphicRaycaster hits, safe-area containment and repeated layout. They also include existing Home, Splash, Loading, real Home preparation and map-back regressions. Route tests invoke listeners; these are not physical-touch tests.
- Eight final World Map PNGs with fixture JSON were captured through the actual router Canvas: 941×1672 RU/EN/RU roundtrip, 576×1280 safe area, 1080×2400 safe area, and three Tasks views. Final RU/EN/phone/tall and all Tasks views were directly inspected. **66 final capture/fixture files** are retained across World Map and the accepted-screen regressions, copied without image edits.
- All **15 fixed Splash/Loading reference captures** are pixel-identical to accepted R11 regression evidence. Five of seven Home portraits are exact; two differ at 13 and 8 pixels respectively, at most **1/255** per channel. Final World Map RU roundtrip differs at **35 pixels, at most 1/255**, with no alpha differences. These small deltas are reported rather than called exact. [Pixel comparison](UnityVerification/RegressionPixelComparison.json).
- Real Splash→Loading→Home preparation still reports **27 operations**, **28 observed progress samples from 0 to 1**, visible completion and two route changes. No timer or injected progress is used. [Actual flow evidence](UnityVerification/LoadingRegression/ActualLoadingFlow.json).
- Chrome loaded all nine review selections, all three local report links and the 360px viewer layout without horizontal overflow. [Browser checks](QA/StandaloneReviewBrowserChecks.json). Browser checks validate the viewer; they are not fresh Unity rendering.
- Source hashes, metadata/GUID preservation, whole-source runtime PNG hashes and C# whitespace/newline checks passed. CurrentTask.md and SessionLog.md retain the earlier acceptance history and this delivery.

[Test cases, launch arguments and isolated storage](UnityVerification/Reproduction.json) · [Edit Mode result](UnityVerification/FinalEditMode.json) · [Play Mode result](UnityVerification/FinalPlayMode.json).

## Corrected issues and retained history

Initial metadata caused the new textures to import as cubemaps; a diagnostic confirmed this and the new-folder importer now explicitly requires Texture2D. A cloned HUD child-path typo was also corrected. The first map tests passed navigation checks, but direct rendering exposed absent CanvasRenderer components; the implementation and serialized owner were fixed and mesh assertions added. One mesh-test API overload failed compilation and was corrected to the installed Editor API.

Later capture checks exposed a same-frame Tasks activation issue in the fixture, initial native button fades, and a one-pixel snapped chapter-number shift after resolution changes. The fixture now waits for rendered panel activation and actual normal button colors; chapter numbers alone disable pixel snapping. No accepted shared text effect was changed. Earlier XML results, import-log hashes and diagnostic portraits are retained in [test history](UnityVerification/TestRunHistory.json), [import history](UnityVerification/ImportAndCompileHistory.json) and QA/Iterations. The first local matte command also used the wrong model-cache directory and timed out; using the existing cache succeeded. No project dependency was installed.

## Not performed and remaining scope

No APK/AAB or Android Player build, physical-phone install/render/input test, release/signing check, full-project test suite, GPU/frame-time/memory benchmark or physical resolution-change test was performed. The existing APK predates this World Map implementation. The source/import checks and Editor captures do not establish Android performance.

World Map R1 is ready for user visual review. Chapters 2–15 need authored content before they can become playable. Dedicated quest/guild systems and their later page refinements remain outside this visual-page task. After review, the next page in the existing flow is Chapter Map.
