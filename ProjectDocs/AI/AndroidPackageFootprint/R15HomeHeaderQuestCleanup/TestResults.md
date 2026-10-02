# R15 — Home header/quest cleanup, 2026-10-01

Implemented the approved next task from the [R14 classification](../R14HomeResourceClassification/TestResults.md). Verified Unity 6000.5.9f1 (b57deb96f08d), current instructions/handoff, local files and inherited working-tree edits. No visual redesign or new startup system.

## Implemented behavior and identity

Home fresh construction and legacy migration no longer request hud_frame or quest_panel. Removed exactly those two TextureNames entries and four temporary dictionary sprite assignments. Retained the existing HomeHud, ReferenceArtwork and QuestPanel objects; existing synchronous ProductionPlayerHeader/source composition supplies source_header and source_quest_ru/source_quest_blank before construction returns. Current migration still uses zero requests through the legacy delegate. Unrelated missing resources remain errors.

HasGeneratedControlBindings was an unused historical diagnostic requiring obsolete art. It now delegates to HasRequiredBindings, matching the existing current source diagnostic; an incorrect current header fails both. No strengthening/relaxation of HasRequiredBindings or quest service/save semantics.

Changed existing files:

- `Assets/LegendsOfRus/Scripts/Editor/LockedDesignImporter.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeView.cs`

Added [RetiredHomeHeaderQuestEditModeTests.cs](../../../../Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/RetiredHomeHeaderQuestEditModeTests.cs) with metadata and twelve permanent cases. [Narrow task patch](QA/TaskChanges.patch) and [original sources](Sources/Before/) retained; inherited edits are excluded from the task-start comparison.

Both PNG/meta pairs moved byte-identically from Resources/ProductionUI/HomeAligned to Art/ReferenceOnly/RetiredHome. The importer adds only the two exact archive paths and uses its existing ConfigureHomeAlignmentTexture contract. GUIDs: hud_frame82085c57945c64ecab5231171e999c10; quest_panel03d94585ef770c0a9aca2184b5239550. Both retain RGBA32,12mips,trilinear,read/write disabled and Android4096format4. Original dimensions2176×723 and2206×713 remain. [Exact four-path SHA256 moves](QA/Scope.json).

Both had zero serialized consumers; there was no active prefab assignment to repair. **All72prefabs are byte-identical**, preserving revisions, GUIDs, object IDs and serialized references. [Prefab proof](QA/PrefabIdentity.json). Current source art, footer, Login logo, gameplay portrait, currency resources, opaque surfaces and font assignments unchanged.

## Unity tests

| Run | Passed | Failed/skipped | Purpose |
|---|---:|---:|---|
| PlayBefore | 4 | 0/0 | Native Home/WorldMap/ChapterMap/PreLevel portrait baseline before any production edit |
| EditAccepted | 180 | 0/0 | Twelve new cases,38existing Home/fresh-builder/archive cases,130Android policy/content-delivery cases |
| PlayFinal | 44 | 0/0 | Home13,WorldMap7,ChapterMap5,PreLevel6,Home preparation7,Loading4,Splash2 |

**224distinct final cases**; the four baseline cases repeat final coverage and are not added again. No failed attempts or test corrections in this task. [Editor command/outcome](QA/EditAccepted/summary.json), [runtime command/outcome](QA/PlayFinal/summary.json) and NUnit XML are retained. Tests ran sequentially with isolated temporary configuration/player storage and existing bootstrap/router; Play used previously authorized process-only Vulkan. Reviewed sandbox exceptions allowed Unity's local IPC; no project graphics/settings or approval guard changes.

The twelve new cases cover exact archive identity/imports and zero recursive consumers among99roots per candidate; fresh/legacy/current Home with denied retired keys and19/19/0requests; fresh/installed WorldMap/ChapterMap/PreLevel shared headers; repeated owner/migration component sequence and live Selectable target preservation; current historical diagnostic rejection of wrong header; current quest variants/loading graphs and retained shared resources; and missing unrelated footer rejection in both fresh/legacy construction. Tests do not replace service progress with timers or mutate production saves.

Existing runtime coverage includes native navigation/action ownership, input-safe controls, RU/EN, safe-area and portrait layouts, inventory/energy comparisons, WorldMap scrolling, actual loading preparation/progress, retry and cancellation. This is targeted coverage, not a fresh full-project campaign regression or Android gameplay acceptance.

## Actual Unity capture comparison

Captured35baseline PNGs before editing: Home14,WorldMap9,ChapterMap5,PreLevel7. Confirmed every7,786snapshot input hash unchanged after that baseline run. Final runtime run retained100PNGs: Home22,WorldMap19,ChapterMap10,PreLevel33,Splash7,Loading9.

**33/35paired captures pixel-identical** from matching PNG settings and inflated filtered scanlines: all14Home,5ChapterMap,7PreLevel and7WorldMap pairs. Two WorldMap pairs were decoded via local libpng RGBA8 to inspect actual pixel differences:

| Pair | Differing pixels | Max channel delta | Inclusive bounding box, top-left origin | Layout JSON |
|---|---:|---:|---|---|
| WorldMap_1080x2400_safearea_RU | 9/2,592,000 | 1/255 | (730,1185)–(747,1230) | Identical |
| WorldMap_576x1280_safearea_RU | 17/737,280 | 1/255 | (326,738)–(472,897) | Identical |

Those two are not full-image identical; no causal claim about their tiny raster differences. No geometry change recorded. The other65final captures have no same-task pre-edit pair. No phone or design-approval claim. [All pairs](QA/CaptureComparison.json), [decoded differences](QA/MapPixelComparison.json); scripts only read images and never edit them.

Inspected these standalone actual Unity portrait pages:

- [Home](QA/PlayFinal/HOME/Home_941x1672_RU.png)
- [WorldMap](QA/PlayFinal/WORLDMAP/WorldMap_941x1672_RU.png)
- [PreLevel](QA/PlayFinal/PRELEVEL/PreLevel_941x1672_RU.png)

## Guarded Android build and actual saving

Built through the unchanged AndroidDevelopmentBuildCommand.BuildDevelopmentApk with all normal guards active. [Successful command](QA/Build/summary.json). Verified the prior artifact hash against R13 before archiving it and its inherited sidecar under Builds/Verification/v1_17_step01/BeforeHomeHeaderQuestCleanupR15. Isolated only launcher final output/package-incremental directories under Library/LegendsOfRusPackagingBackups; no Gradle/player/signing/package configuration reset. Temporary build wrapper exported the actual successful Unity packed report, then was removed with metadata. [Packaging isolation](QA/PackagingIsolation.json).

| Metric | Verified result |
|---|---|
| Prior APK | 815,985,566 bytes |
| Current APK | 808,099,626 bytes |
| Actual APK saving | 7,885,940 bytes (0.966%) |
| SHA256 | `a62cfd9404591e698e3bd286ab729c742c439eaa6a60a11a6a53082f774e077e` |
| Packed rows | 29,358→29,352 |
| Removed rows | Exactly six across hud_frame.png and quest_panel.png |
| Other membership | Path/GUID/type multiplicities unchanged; no additions |
| Retired artwork | All eight retired GUIDs absent; reference archives not packed |
| ZIP inventory | All403paths retained,398payloads identical |
| Stale unindexed bytes | Zero |

Removed packed-object bytes total16,771,660, which is distinct from actual APK saving. Unity data payload shrank7,886,345bytes; remaining APK delta includes code/metadata/build IDs and packaging alignment/signature overhead. Five changed ZIP payloads were reviewed: Unity data, IL2CPP library, runtime metadata, boot.config and unity_app_guid. Boot settings change only build-guid/player-connection-guid. [Packed membership](QA/PackedComparison.json), [full payload accounting](QA/ArtifactComparison.json), [build metadata](QA/BuildMetadataComparison.json).

Signing and unchanged development certificate pass. Package com.example.legendsofrus.v117debug,versionCode1/versionName0.0.0,ARM64,minimumAPI26/targetAPI36,portrait and debuggable retained; permissions unchanged. ZIP CRC and16KiBZIP alignment pass; all six ELF libraries meet load-segment alignment. Expected credential/gameplay-QA runtime metadata present; nine credential DEX classes, JNI methods and operation fields verified. Rehashed all four Java source/meta pins and matched. [APK verification](QA/ApkVerification.json), [DEX evidence](QA/CredentialDex.json), [Java pins](QA/PinnedPluginInventory.json), [artifact/hash](QA/Artifact.json). No installation or launch.

## Task scope and remaining work

Of7,786baselineAssets/Packages/ProjectSettings inputs:7,779unchanged,3changed,6added,4removed. Additions/removals include four exact moved PNG/meta paths and one test source/meta. All prior edits preserved; approved Splash, fonts, packages/settings, content and guards unchanged. [Scope](QA/Scope.json).

Only newly generated Addressables linker files cleaned; other generated state preserved/restored from pre-build copies. [Generated cleanup](QA/GeneratedFileCleanup.json). Raw logs/isolated configuration and packaging backups stay outside the authored patch; no Library/Temp/obj/Logs changes included, secrets/keystores/passwords read or packaged, commit or deployment.

**Next:** Complete the separately scoped footer_base_generated cleanup using the R14 evidence: remove its temporary Home loader/assignment, clear only the historical disabled WORLD_MAP_ALIGNED_CONTENT/FooterOrnament sprite in the active WorldMap prefab, and align Home/WorldMap owning paths so construction or migration cannot restore it. Preserve the live view.FooterOrnament using WorldMapReferenceAligned/footer_backing, current Home source_footer_carving, all objects/bindings/revisions and other retained resources. Add missing-key, repeated-owner and identity regressions, verify native Home/WorldMap and cloned-header routes plus Splash/Loading, then archive the single PNG/meta pair with exact identity/import preservation and measure the guarded APK. Phone execution and hosting remain deferred; final gameplay effects remain last. Human Chapter1 pacing, font provenance and Android lifecycle/performance/readability remain open.
