# R13 — Home environment/Play cleanup, 2026-10-01

Completed the user-approved implementation following the [R12 audit](../R12HomeEnvironmentPlayAudit/TestResults.md). Verified Unity 6000.5.9f1 (b57deb96f08d). This is an implementation and Editor/package verification milestone; no Android device execution or new visual approval is claimed.

## Implemented behavior

Fresh and legacy Home no longer request home_environment_generated or home_play_button. The existing synchronous composition assigns clean_environment and current source Play art before construction returns. The dormant ReferenceArtwork Image is retained with a null sprite in Home, ChapterMap and PreLevel. Their installed/fresh owners clear any stale assignment after assigning the live targetGraphic; current Home's valid fast path also clears it. No new startup system, objects, revision change or visible asset substitution.

Each active prefab differs from its task-start bytes in exactly one m_Sprite field. All other bytes and serialized object headers match: Home 480 objects, ChapterMap 1,019 and PreLevel 570. [Identity proof](QA/PrefabIdentity.json).

Both PNG/meta pairs moved byte-identically to Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome. LockedDesignImporter retains the exact archive paths using its existing Home import owner. Environment retains RGB24, one mip, bilinear, Android 2048; Play retains RGBA32, eleven mips, trilinear, Android 4096. Old Resources keys are absent. Exact SHA256 move evidence is in [Scope](QA/Scope.json).

Changed existing project files:

- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/ChapterMap.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/PreLevel.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/LockedDesignImporter.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionChapterMapSourceControls.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeSourceComposition.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPreLevelSourceControls.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/PreLevelAlignmentPlayModeTests.cs`

Added permanent regression source/meta: Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/RetiredHomeEnvironmentPlayEditModeTests.cs. Added four archive paths and removed their four previous Resources paths. No other project input changes. [Task-start patch](QA/TaskChanges.patch) includes only the nine existing file changes, not unrelated inherited working-tree edits. [Before snapshots](Sources/Before/) and [baseline hashes](QA/BeforeHashes.json) retained.

## Unity verification

| Run | Outcome | Purpose |
|---|---|---|
| PlayBefore | 3 passed, 0 failed/skipped | Native Home, ChapterMap and PreLevel capture baseline; inputs unchanged after run |
| EditAccepted | 168 passed, 0 failed/skipped | New cleanup 12, fresh builder 6, Home delivery 6, retired hero 9, retired Firebird 5, Android policy/content delivery 130 |
| PlayAccepted (initial) | 36 passed, 1 failed, 0 skipped | Full-energy timestamp-only save comparison failed in existing booster-guide test |
| PlayFinal | 37 passed, 0 failed/skipped | Home 13, ChapterMap 5, PreLevel 6, Home preparation 7, Loading 4, Splash 2 |

**205 distinct cases in the final accepted coverage**, not counting baseline or reruns twice. NUnit XML and exact commands are retained in [EditAccepted](QA/EditAccepted/summary.json), [first runtime run](QA/PlayAccepted/summary.json) and [final runtime run](QA/PlayFinal/summary.json). Tests used isolated temporary player storage and existing bootstrap/router. Play used the previously authorized process-only Vulkan option; project graphics/backend settings unchanged.

The first runtime failure compared full save JSON across yielded frames. The only differing field was lastEnergyUtcTicks while energy remained full. Existing GameState.RefreshEnergyFromClock refreshes that timestamp on its periodic check. The corrected assertion parses copies, requires a nondecreasing timestamp only when expected energy is full, normalizes that clock field, and compares every other serialized field. It does not mutate the real save or change the energy service. Full runtime rerun passed.

The twelve new Editor cases verify archive identity/imports and zero consumers across 99 roots per candidate, fresh/legacy/current Home missing-key denial, installed/fresh three-page bindings, repeated builder removal of reinjected stale art with component sequence/live target preservation, and Splash/Loading graphs/current loading keys. Fresh and legacy construction request 21 remaining resources; current migration requests zero through the legacy delegate. Existing navigation and runtime checks cover controls, inventory, RU/EN, portrait/safe areas and actual progress/retry/cancellation.

## Actual Unity captures

Final run retained 81 PNG captures: Home22, ChapterMap10, PreLevel33, Splash7, Loading9. **All26 baseline pairs are pixel-identical:**14Home,5ChapterMap,7PreLevel. Comparison checks equal PNG settings and inflated filtered scanlines, sufficient to prove identical decoded pixels. Remaining55final captures have no same-task pre-edit pair; no identity claim for them. Separate Editor runs, not phone captures. [Comparison](QA/CaptureComparison.json).

Inspected these standalone actual Unity portrait pages:

- [Home](QA/PlayFinal/HOME/Home_941x1672_RU.png)
- [ChapterMap](QA/PlayFinal/CHAPTERMAP/ChapterMap_941x1672_RU.png)
- [PreLevel](QA/PlayFinal/PRELEVEL/PreLevel_941x1672_RU.png)

## Guarded Android artifact

Existing AndroidDevelopmentBuildCommand.BuildDevelopmentApk used unchanged, with all normal approval/package guards active. Build succeeded; wrapper exported the actual Unity packed report and was removed with metadata. Only prior launcher final-output/package-incremental directories were isolated under Library; no Gradle/signing/player configuration reset. Prior APK and inherited hash sidecar archived under Builds/Verification/v1_17_step01/BeforeHomeEnvironmentPlayCleanupR13. Actual prior hash verified against R11 before build. [Command/outcome](QA/Build/summary.json), [isolation](QA/PackagingIsolation.json).

| Metric | Verified result |
|---|---|
| Prior APK | 824,676,966 bytes |
| Current APK | 815,985,566 bytes |
| Measured saving | 8,691,400 bytes (1.054%) |
| Current SHA256 | `f0be6efcfe4ad0b5e60cd38cdc0baccaec84340233ac5d6dba033c0f60b76a6a` |
| Packed rows | 29,364 →29,358 |
| Removed membership | Exactly six rows across the two approved candidate paths |
| Other object membership | Path/GUID/type multiplicities identical; no additions |
| Retired GUIDs | All six excluded, including prior progress/Firebird/hero archives |
| ZIP paths | 403 retained, no additions/removals,398payloads identical |
| Stale unindexed payload | Zero bytes |

Removed packed-object bytes total13,105,104; this is distinct from the measured APK saving. Five ZIP payload changes were reviewed: Unity asset data, IL2CPP library, runtime metadata, boot.config and unity_app_guid. Boot configuration changes only build-guid and player-connection-guid; other settings match. Asset payload shrank8,689,857bytes; remaining artifact change includes compiled code/metadata and ZIP/alignment/signature overhead. [Packed proof](QA/PackedComparison.json), [full payload accounting](QA/ArtifactComparison.json), [build metadata review](QA/BuildMetadataComparison.json).

Signature verification and same development certificate pass. Package com.example.legendsofrus.v117debug, versionCode1/versionName0.0.0, ARM64, minimumAPI26/targetAPI36, portrait and debuggable preserved; permissions identical. ZIP CRC, 16KiB ZIP alignment and all six ELF library load-segment checks pass. Expected QA/credential runtime markers present; nine credential DEX classes, JNI methods and operation fields retained. All four Java source/meta pins rehashed and matched. [APK verification](QA/ApkVerification.json), [DEX](QA/CredentialDex.json), [Java pins](QA/PinnedPluginInventory.json), [artifact](QA/Artifact.json).

## Scope and remaining limits

Task-start snapshot:7,784 Assets/Packages/ProjectSettings inputs;7,771 unchanged,9changed,6added,4removed. Moves preserve exact PNG/meta hashes. The approved Splash, other visual art, fonts, all packages/settings, guards, prior local edits and shared Login logo remain unchanged. Only newly generated Addressables linker files removed; other generated state retained or restored from pre-build copies. [Generated cleanup](QA/GeneratedFileCleanup.json). Raw logs/configuration and generated packaging backups remain outside the authored patch; no secrets, keystores or player credentials included.

No device installation/run, physical-device lifecycle/performance/visual acceptance, hosting, commit or deployment. Human Chapter1 difficulty/pacing and exact font provenance remain unverified. These205checks are targeted coverage, not a fresh full-project gameplay sweep.

## Next scheduled task

Consolidated read-only classification of the remaining 21 legacy Home constructor resources, starting with footer_base_generated.png and quest_panel.png. Trace serialized, dynamic, shared-page and cloned-header consumers from the new packed report before proposing any removal. Retain the Login logo, live controls, currency imports and approved Splash. Phone execution and hosting remain deferred; final gameplay effects remain last. Human Chapter 1 pacing, font provenance and Android lifecycle, performance and readability remain open.
