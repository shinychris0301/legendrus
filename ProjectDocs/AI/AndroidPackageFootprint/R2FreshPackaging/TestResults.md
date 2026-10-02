# Android fresh packaging R2 — 2026-10-01

**Completed the schedule's next task: the guarded development APK is now 838,219,234 bytes, down from 1,388,384,859 bytes.** The saving is **550,165,625 bytes (39.63%)**, with no artwork, importer, gameplay, package, settings or build-guard change.

## Project review and authorization

The user requested review of the current project and continuation according to the development schedule after chat deletion. Reviewed AGENTS.md (only project instruction file found), ConversationHandoff, current task entries, session log, DevelopmentSchedule, current working-tree changes, owning build command and recent verification scripts/artifacts. The project version and installed Editor are Unity **6000.5.9f1**. Large existing tracked/untracked changes are retained, not reset or treated as this task's patch.

Current records establish the core-first order: Chapter 1 rounds 1–500 and finale are installed; R5 recorded 1,334 distinct passing runtime cases and three deferred backend cases; recent core UI fixes, disposable gameplay profile, input configuration and Addressables repairs are present. Those counts are historical evidence reviewed here, not tests rerun in this task. The current artifact hash matched the latest package audit before rebuilding. The older Splash/Loading handoff is historical context; its early next-step recommendations do not supersede the current schedule. Approved Splash artwork remains protected.

The selected bounded task was the schedule's explicit immediate action: fresh guarded packaging, followed by exact payload and signature checks. Phone execution, hosted services and final effects retain their recorded deferral. No new visual approval is inferred.

## Build and artifact

- Existing entry point: `LegendsOfRus.AndroidBuild.AndroidDevelopmentBuildCommand.BuildDevelopmentApk`, batch mode, no installation or launch. The existing preflight and normal build guards remained active.
- Prior APK preserved at `Builds/Verification/v1_17_step01/BeforeFreshPackagingR2/LegendsOfRus-v1.17-step01-development.apk`; its SHA256 remains `456c9d9475461f4bb7bad8d155cbf629cac34ac25a27ff4a0b5a20b8986a1f19`.
- Three generated launcher directories were moved, not deleted: `outputs/apk/debug`, `intermediates/incremental/packageDebug`, and `intermediates/apk_ide_redirect_file/debug`. Restorable originals are under `Library/LegendsOfRusPackagingBackups/BeforeFreshPackagingR2/`. Player data, IL2CPP output, imported textures, Gradle configuration and signing configuration were retained. [Exact isolation inventory](QA/PackagingIsolation.json).
- Unity guarded build exited **0 in 39.58 seconds**. It reused unchanged player content; this is not a clean compilation/rebuild of every source file. [Command and outcome](QA/Build/summary.json).
- Current APK: [LegendsOfRus-v1.17-step01-development.apk](../../../../Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk), **838,219,234 bytes** (838.22 decimal MB / 799.39 MiB).
- SHA256: `480cb80ddeea115741fb25b779fed855da0b740cf3c55d88721c5b1dfd12057c`. [Artifact record](QA/Artifact.json).
- Replaced an inherited stale `.apk.sha256` sidecar with the verified current hash. The inherited text is preserved beside the archived APK with an `.inherited` suffix; it did not match the prior APK and must not be used to verify it. [Checksum repair record](QA/ChecksumSidecar.json).

## Exact comparison

| Archive component | Prior bytes | New bytes |
|---|---:|---:|
| Indexed compressed payload | 838,143,791 | 838,143,790 |
| Indexed local headers/extra fields | 32,733 | 32,734 |
| Unindexed local records | 550,165,625 | **0** |
| Trailing signing block/central directory | 42,710 | 42,710 |
| Total | 1,388,384,859 | **838,219,234** |

All **403 indexed entries** remain present, with no additions/deletions; **398 retain identical contents at the same paths**. All entries were SHA256-compared, not only CRC-compared. The other five paths are fully accounted for:

- `boot.config`: fresh build/connection identifiers and reordered existing connection addresses. All other values and the complete multiset of addresses are unchanged.
- `unity_app_guid`: a fresh, valid build-specific app identifier.
- `classes3.dex`, `classes4.dex`, `classes5.dex`: renumbered to classes4, classes5, classes3 respectively. The complete multiset of all five DEX files is byte-identical, including multiplicities. No compiled Java payload was added or removed.

Unity `data.unity3d`, native libraries, IL2CPP metadata, Android manifest and all remaining entries retain identical bytes. Unity asset payload SHA256: `4045f968b83e3f96d779c83acb8e96012ae059555be8cc509da6a384a7622827`. The 8,407 unnamed zero-payload local records are gone; the size reduction exactly equals their previous space. The one-byte compression change in boot metadata is offset by header/alignment overhead. [Full comparison](QA/ArtifactComparison.json) · [Reviewed differences](QA/ReviewedDelta.json).

An initial inspection assertion assumed boot metadata changed only its build GUID and conflated two independent GUIDs. Reading the actual entries showed connection-ID/address-order changes and distinct valid GUIDs. Corrected the inspector to preserve duplicate keys, compare address multisets, and validate each GUID independently; the final review passed. No production correction or second build was required.

## Checks run

- **130/130 Unity Edit Mode build-policy/content-delivery tests passed; zero failures/skips.** [Results](QA/PolicyTests/results.xml) · [Summary](QA/PolicyTests/summary.json).
- Existing guarded development APK build succeeded, including its Addressables/Player build callbacks.
- APK signing verification passed; development certificate matches the archived prior APK. Package `com.example.legendsofrus.v117debug`, version 0.0.0/code1, ARM64, API26 minimum/API36 target, portrait and debuggable verified. Permissions unchanged.
- Both APKs passed all indexed ZIP CRC checks. Fresh APK passed 16 KiB ZIP alignment and all six ELF libraries' load-segment alignment checks.
- Runtime metadata markers, all nine credential DEX classes/JNI members and approved Java source/metadata pins verified. [APK verification](QA/ApkVerification.json) · [DEX inspection](QA/CredentialDex.json).
- **All 7,774 baseline Assets/Packages/ProjectSettings files unchanged, zero changed/added/deleted**, after removing only this run's newly generated `Assets/AddressableAssetsData/link.xml` and `.meta`. Existing generated metadata was preserved. [Scope](QA/Scope.json) · [Cleanup](QA/GeneratedFileCleanup.json).

Raw Unity/launcher logs remain local under `/tmp/lor-package-footprint-r2/`; raw logs and generated caches are not part of the authored patch. The build emitted the existing unverified Splash typography warning. Licensing logged an unavailable access token update but completed the build successfully; no licensing or guard workaround was made.

Not run: new Play Mode suite, new rendered captures, physical-device installation/input/gameplay/lifecycle/performance tests, release/custom-signing build or backend deployment. Unity's own Android tooling ran as part of the build; no device commands were issued by the task scripts. No device acceptance is implied by package equivalence.

## Changed files and next action

Authored changes are the six scripts in `Sources/` (`Build.py`, `Compare.py`, `VerifyApk.py`, `InspectCredentialDex.py`, `ReviewDelta.py`, `RecordScope.py`), this report, QA evidence and task/schedule/session/handoff documentation. VerifyApk and InspectCredentialDex adapt the existing Addressables verification scripts. No Assets/Packages/ProjectSettings source patch, commit, installation or deployment. The old APK and generated packaging backups remain available locally.

**Next scheduled local task:** audit the largest shared UI controls' complete Resources/serialized usage and largest rendered size, starting with the Home coin and progress fill. Establish a bounded optimization candidate and native before/after acceptance requirements before changing imports. The remaining ~838 MB is largely real texture payload, so further savings cannot be inferred from ZIP cleanup or source-image duplication. Preserve Splash and approved art; do not apply blanket compression or delete duplicate-looking resources. Physical Android gameplay/lifecycle/performance/readability, human rounds31–500 pacing and font provenance remain open. Hosting stays deferred and final effects remain last.
