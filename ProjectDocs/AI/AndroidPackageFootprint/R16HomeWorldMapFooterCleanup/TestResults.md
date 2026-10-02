# R16 — Home/WorldMap footer cleanup, 2026-10-01

Implemented the approved scheduled footer cleanup from the [R14 classification](../R14HomeResourceClassification/TestResults.md) and R15 next action. Verified Unity6000.5.9f1(b57deb96f08d), project instructions/handoff and current local files. Preserved inherited working-tree edits and approved Splash.

## Result and changed files

Home fresh/legacy construction no longer requests footer_base_generated; its existing ConnectedCarvedBase Image receives current source_footer_carving during the same synchronous source composition. WorldMap's original WORLD_MAP_ALIGNED_CONTENT/FooterOrnament survives with a null sprite. Its separate live view.FooterOrnament still uses WorldMapReferenceAligned/footer_backing. No live footer, routes, controls, wording or layout redesign.

The owning runtime valid-instance path and both source-composition paths clear the historical sprite only if the Image differs from the live FooterOrnament field. During legacy source migration, clearing happens after the replacement field is assigned. Fresh WorldMap no longer copies artwork into that dormant container. Repeated runtime/source owner calls clear reinjected archived sprites without changing components or live targets; a dedicated test protects a field still pointing at the historical Image before replacement.

Changed existing inputs:

- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/WorldMap.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/LockedDesignImporter.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionWorldMapRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionWorldMapSourceComposition.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/RetiredHomeHeaderQuestEditModeTests.cs`

Added eleven permanent cases in [RetiredHomeWorldMapFooterEditModeTests.cs](../../../../Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/RetiredHomeWorldMapFooterEditModeTests.cs) plus metadata. Updated existing header/quest regressions to expect18remaining keys and use still-present quest_reward_frame for retained-resource/missing-key checks. Current-binding assertions remain intact. [Narrow patch](QA/TaskChanges.patch), [before sources](Sources/Before/).

Exactly one WorldMap prefab m_Sprite field changed. All2,201serialized object headers, every other byte and revision unchanged; other71prefabs byte-identical. [WorldMap identity](QA/PrefabIdentity.json), [other prefabs](QA/OtherPrefabIdentity.json).

Moved the existing PNG/meta pair byte-identically to Art/ReferenceOnly/RetiredHome, preserving GUID51e28d72f0853db939602ec4f6546f7c. Exact-path LockedDesignImporter rule reuses its existing Home import owner. Source2143×734,RGBA32,12mips,trilinear,read/write-disabled,Android4096format4 retained. [Exact hashes/move](QA/Scope.json). No art editing, resizing, recoloring, new font or blanket compression.

## Verification

| Run | Result | Scope |
|---|---|---|
| PlayBefore | 4passed,0failed/skipped | Native four-page pre-edit captures; all7,788input hashes still matched afterward |
| EditAccepted(initial) | 189passed,2failed,0skipped | New11footer cases passed; two old header/quest cases expected19instead of18loads |
| EditFinal | **191passed,0failed/skipped** | Full affected Editor suite after correcting historical count expectations |
| PlayFinal | **44passed,0failed/skipped** | Home13,WorldMap7,ChapterMap5,PreLevel6,Home preparation7,Loading4,Splash2 |

**235distinct final cases**, not counting pre-edit captures or reruns twice. [Initial Editor evidence](QA/EditAccepted/summary.json), [final Editor evidence](QA/EditFinal/summary.json), [runtime evidence](QA/PlayFinal/summary.json) and XML retained. Corrected only test expectations after the first run; no post-failure production repair. Saved initial R16 test version under Sources for review.

Eleven new cases cover archive identity/imports and zero consumers among99recursive dependency roots; Home fresh/legacy/current loader denial with18/18/0requests; fresh/installed map current-footer parity; repeated runtime/source owner stale-sprite clearing with component/target identity; legacy source reconstruction on a disposable instance; live-field protection; and loading/cloned-header graphs/retained resources. Legacy reconstruction removes only the disposable current stage before invoking the existing source owner, not an authored prefab/scene.

Existing checks cover native navigation/input, inventory/energy/boosters, RU/EN, map scrolling and portraits/safe areas, actual loading preparation/progress/retry/cancellation. Sequential Unity runs used isolated temporary storage and existing startup/router; process-only Vulkan for Play was previously authorized. Reviewed sandbox exceptions enabled Unity local IPC, without changing packages/project graphics/settings or guards. This is targeted Editor/runtime coverage, not Android gameplay acceptance or a new broad campaign sweep.

## Actual Unity captures

35baseline captures:14Home,9WorldMap,5ChapterMap,7PreLevel. Final100captures:22Home,19WorldMap,10ChapterMap,33PreLevel,7Splash,9Loading. **33/35pairs pixel-identical**, including all Home/ChapterMap/PreLevel and7WorldMap pairs. Two tall WorldMap pairs retain the same tiny difference pattern recorded in R15:

| Pair | Differing pixels | Max channel delta | Inclusive box, top-left origin | Layout JSON |
|---|---:|---:|---|---|
| 1080×2400safeareaRU | 9/2,592,000 | 1/255 | (730,1185)–(747,1230) | Identical |
| 576×1280safeareaRU | 17/737,280 | 1/255 | (326,738)–(472,897) | Identical |

No complete-image identity claim for those two or causal claim about their raster differences. Other65finalcaptures are unpaired. Comparisons read PNGs only; first matching-settings/inflated-scanline proof, then local libpng RGBA8 decoding for unequal pairs. [All pairs](QA/CaptureComparison.json), [decoded differences](QA/MapPixelComparison.json). No phone screenshots or new visual acceptance.

Inspected standalone actual Unity [Home](QA/PlayFinal/HOME/Home_941x1672_RU.png), [WorldMap](QA/PlayFinal/WORLDMAP/WorldMap_941x1672_RU.png) and [ChapterMap](QA/PlayFinal/CHAPTERMAP/ChapterMap_941x1672_RU.png) pages.

## Guarded artifact and wrapper recovery

Invoked unchanged AndroidDevelopmentBuildCommand.BuildDevelopmentApk with normal package/approval/signing guards. Prior APK hash verified against R15 and preserved under BeforeHomeWorldMapFooterCleanupR16 with inherited sidecar; only final launcher/package-incremental directories isolated under Library. No Gradle/configuration/signing-state reset. [Isolation](QA/PackagingIsolation.json).

**Runner interruption:** wrapper process terminated with143 before writing its post-Unity summary/artifact record and running its finally cleanup. No Unity process remained. Actual exported Unity packed report says Succeeded and its log contains successful batch shutdown. Output APK exists at the reported path and differs from the hash-verified previous APK. Unity's process exit code was not captured; it is null in [recovered build summary](QA/Build/summary.json), not fabricated as0. Cause of wrapper termination is unknown.

Recovery verified those conditions, removed the temporary helper only after byte-matching its retained source, removed helper metadata, computed the actual artifact SHA256 and sidecar, then ran all independent APK and scope checks. No player rebuild or guard bypass. [Recovery evidence](QA/BuildRecovery.json), [recovery script](Sources/RecoverBuild.py). Actual successful report plus verified artifact establishes the build result; the wrapper execution itself did not complete normally.

| Artifact metric | Result |
|---|---|
| Prior APK | 808,099,626bytes |
| Current APK | 805,137,130bytes |
| Actual saving | 2,962,496bytes (0.367%) |
| SHA256 | `91afa7f452e20b2991ff84ec2428c88f197dc7bb12d0140a51b4b78e4951b9a0` |
| Packed membership | 29,352→29,349rows; exactly3oldfooter rows removed |
| Other membership | Path/GUID/type multiplicities identical; zero additions |
| Retired GUIDs | All9excluded, no archive paths packed |
| ZIP | 403paths retained,398payloads identical,zero stale unindexed bytes |

Removed8,385,112packed-object bytes are distinct from measured APK saving. Unity data payload shrank2,963,293bytes; compiled code/metadata and packaging overhead account for the remaining delta. Five changed payloads reviewed: Unity data, IL2CPP library, runtime metadata, boot.config,unity_app_guid. Boot configuration changes only build-guid/player-connection-guid. [Packed comparison](QA/PackedComparison.json), [full payload accounting](QA/ArtifactComparison.json), [metadata review](QA/BuildMetadataComparison.json).

Signature and unchanged development certificate verified; packagecom.example.legendsofrus.v117debug,versionCode1/versionName0.0.0,ARM64,API26→36,portrait/debuggable and permissions preserved. ZIP CRC,16KiBZIP alignment and all6ELF library load-segment checks pass. Expected credential/gameplay-QA metadata,9credential DEX classes/JNI methods/operation fields and4Java source/meta pins verified. [APK checks](QA/ApkVerification.json), [DEX](QA/CredentialDex.json), [Java pins](QA/PinnedPluginInventory.json), [artifact](QA/Artifact.json).

## Scope and next action

Of7,788baseline Assets/Packages/ProjectSettings inputs:7,780unchanged,6changed,4added,2removed. Additions/removals include the exact PNG/meta move and newtest source/meta. Only newly generated Addressables linker files cleaned; other generated state restored/preserved from pre-build snapshot. [Scope](QA/Scope.json), [generated cleanup](QA/GeneratedFileCleanup.json).

Prior local edits, approved Splash, current source artwork, opaque surfaces, fonts, packages/settings, GUIDs and guards retained. Raw logs/configuration and generated packaging backups remain outside the authored patch. No caches, secrets, keystores/passwords or player credentials packaged in the patch. No device installation/execution, hosting, commit or deployment. Human Chapter1 pacing, font provenance and Android lifecycle/performance/readability remain open.

**Next:** Complete one coordinated cleanup of the nine remaining dormant Home-only resources identified in R14: four nav_*_generated sprites, mail_medallion_generated, reward_medallion_generated, quest_reward_frame, quest_reward_gem and home_map_foliage. Recheck current dependencies, remove their temporary loader/assignment paths, clear only their dormant Home prefab references, align the owning source/migration paths and relevant historical diagnostics, and preserve all objects/current controls/artwork/revisions. Add missing-key/component/target regressions and native render/input checks before exact GUID/byte/import-preserving archives and a guarded APK measurement. Retain the nine live/shared resources, especially Login logo, gameplay portrait and currency. Phone execution/hosting remain deferred; final gameplay effects remain last.
