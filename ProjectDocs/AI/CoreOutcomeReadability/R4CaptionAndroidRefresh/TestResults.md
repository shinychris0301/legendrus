# Core caption Android refresh R4 — 2026-10-01

Built the approved guarded development APK with the installed [R3 Gameplay/Victory captions](../R3CaptionReadability/TestResults.md), revisions **23/12**. **130 current Android policy/content Edit Mode cases passed**, zero failed/skipped. Unity 6000.5.9f1 exited 0 and its actual build report returned Succeeded. No production source, art, prefab, metadata, package or project setting changed during this packaging task.

APK: [LegendsOfRus-v1.17-step01-development.apk](../../../../Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk).

**765,772,146 bytes**; SHA256 `924ecabcdbdb58f30512e6e2b134db4246be22c8f9ecc6b6086551e4ad937eff`. Current checksum sidecar matches. [Artifact](QA/Artifact.json).

| Measurement | Preserved R2 | R4 |
| --- | ---: | ---: |
| APK bytes | 765,773,174 | 765,772,146 |
| Packed object rows | 29,347 | 29,363 |
| ZIP paths | 403 | 403 |

The 1,028-byte difference (0.000134%) is incidental packaging variation, not a meaningful footprint optimization. Previous R2 APK and its verified inherited checksum are preserved in `Builds/Verification/v1_17_step01/BeforeCoreCaptionRefreshR4/`. [Archive record](QA/PreviousArtifact.json).

## Checks actually run

Current Android policy/content suite: **130/130 Edit Mode cases**, 14.83s invocation, exit 0. Includes existing Android policy, request/options, approval/package-lock, credential-plugin and content-delivery guards. Tests used isolated configuration/storage. [Summary](QA/Policy/summary.json) · [XML](QA/Policy/results.xml).

The existing AndroidDevelopmentBuildCommand.BuildDevelopmentApk ran through its unchanged guarded Addressables/player/IL2CPP/linker/Gradle path. Unity exited 0 in **99.84s**. A temporary helper invoked that command, required Succeeded and exported the actual packed inventory; it did not replace approval logic. [Invocation](QA/Build/summary.json) · [Packed report](QA/PackedAssets.json). Reviewed permissions allowed local Unity Package Manager IPC. Raw logs remain in `/tmp/lor-caption-android-refresh-r4/`.

Signature verification and same-certificate comparison against archived R2 passed. Identity remains `com.example.legendsofrus.v117debug`, versionCode 1 / versionName 0.0.0, Android Debug signed, ARM64, API 26–36, portrait and debuggable. Permissions unchanged. All 403 ZIP CRCs, exact ZIP accounting with zero unindexed bytes, 16 KiB ZIP alignment and all six ARM64 ELF load-segment alignments passed. IL2CPP metadata contains the retained credential/development-profile/touch markers and caption component/method/field names. [APK verification](QA/ApkVerification.json).

Nine compiled credential DEX classes, JNI entry definitions and operation fields passed. All four Java source/meta pins unchanged. No credentials, passwords or keystore contents were read or printed by these audits. [DEX definitions](QA/CredentialDex.json) · [Source pins](QA/PinnedPluginInventory.json).

R3's 19 Edit + 25 Play passes and 105 captures remain prior results; they were not rerun or counted among R4's 130 cases. No new Play Mode/capture, broad campaign sweep, device execution or human playtest ran. Build/package checks do not establish Android touch, readability, lifecycle or performance acceptance. Existing source warnings/font provenance remain unresolved; no font assignments were changed or OS fonts distributed.

## Reviewed package changes

Exactly **16 Gameplay serialized objects** added: four each of GameObject, RectTransform, CanvasRenderer and MonoBehaviour, corresponding to the four native caption faces. Gameplay 413→429 objects, +1,424 packed-object bytes; Victory object membership unchanged. **Every other path/GUID/type membership is identical**, with no added or removed asset paths or MonoScripts. Packed-object bytes are distinct from compressed APK size and device memory. [Strict packed comparison](QA/PackedComparison.json).

All 403 ZIP paths retained; **398 payloads byte-identical**. Exactly five differences reviewed: player data, managed metadata, compiled libil2cpp.so, boot.config and unity_app_guid. Stable boot settings unchanged; only build/player-connection identifiers differ. Unlike the preceding R2 refresh, **libunity.so is byte-identical**, with no binding or dynamic-symbol differences. [Package comparison](QA/ArtifactComparison.json) · [Payload review](QA/PayloadReview.json) · [Native engine comparison](QA/NativeEngineComparison.json).

The build used the actual installed caption input hashes/revisions recorded in [InstalledCaptionInputs](QA/InstalledCaptionInputs.json). Source artwork is retained, including the existing blank Victory panel and baked Russian variant; no new raster assets or imports were introduced.

## Preservation and next action

All **7,802 task-start Assets/Packages/ProjectSettings inputs are byte-identical**, zero changed/added/deleted. Approved Splash, all active prefabs, current source art/fonts/GUIDs, previous user edits, settings, package locks and guards preserved. [Scope](QA/Scope.json). Temporary build helper/meta removed normally. Only two newly generated Addressables linker files removed; existing generated metadata protected. [Cleanup](QA/GeneratedFileCleanup.json).

Only three launcher packaging output/incremental directories were isolated into the project's Library backup folder, preserving player assets, IL2CPP project and signing configuration. [Packaging isolation](QA/PackagingIsolation.json). Authored additions are task scripts/evidence/report and CurrentTask/SessionLog/Schedule/Handoff updates; no caches are included. [Final validation](QA/FinalValidation.json).

No installation, hosting, deployment, release signing, version bump or commit. Physical-phone work remains postponed. The earlier clean broad R5 regression predates the subsequent resource cleanup and caption changes. **Next:** run one current broad Unity core regression and the existing isolated local restart phases, reconcile any demonstrated fixture/runtime failures, and preserve current art/services/save semantics. Report conditional backend exclusions separately; do not infer phone or human Chapter 1 pacing acceptance. Large stock badge limits, font provenance and Android lifecycle/performance remain open. Final gameplay effects stay last.
