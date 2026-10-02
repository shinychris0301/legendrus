# Addressables profile repair R1 — test results

**Fixed the empty profile-reference warning. Final: 13 Play Mode + 130 Edit Mode tests passed, 0 failed or skipped.** Guarded Android development packaging and artifact verification passed. No phone installation or execution.

## Cause and installed correction

The installed Addressables package evaluates `RemoteCatalogBuildPath` during build-layout generation even with remote catalogs disabled. The serialized build/load references were empty strings; the package getters only initialize null IDs. Restored both references to their existing `Remote.BuildPath` and `Remote.LoadPath` profile entries.

Updated the owning `Step17AddressablesContentDeliveryConfigurator` to repair only empty references on synchronization. Existing nonempty/custom references and profile values are preserved. Its validator rejects missing references and an enabled remote catalog. `BuildRemoteCatalog` remains false; `Remote.LoadPath` remains `<undefined>` because hosting is deferred. Production still uses its existing Resources fallback; local Addressables migration and all remote activation gates remain disabled. No package source edits or warning suppression.

## Verification

| Check | Result |
|---|---|
| Loading Play Mode | 13 passed: production bootstrap/Splash → measured Loading → Home, actual resource loading, cancellation, failure/retry, single infrastructure, alignment and activation gates |
| Edit Mode | 130 passed: 120 Android build-policy + 10 content-delivery policy/manifest/fallback cases |
| Existing foundation validator | Passed in Unity; compiled owner and installed configuration agree |
| Guarded Android build | Exit 0, about 35 seconds; Addressables content and build-layout generation succeeded |
| Empty profile warning | Prior APK build: 1 occurrence; current build: 0 |
| APK identity/signature/permissions | Passed; same signing certificate and permissions as prior local development APK |
| ZIP/native libraries | ZIP CRC and 16 KiB alignment passed; all six ELF libraries aligned |
| Runtime/plugin preservation | Approved Java pins and nine credential DEX classes verified. ZIP comparison found only `boot.config` and `unity_app_guid` changed; all other packaged entries match prior CRC, size and compression size |

Unity 6000.5.9f1. Loading tests use isolated product storage and previously approved process-only Vulkan; project rendering settings unchanged. Actual 576×1280 safe-area loading capture visually inspected. These results do not establish physical-phone performance or touch acceptance. Full-project suite not repeated for this configuration-only correction.

## Artifact and scope

[Development APK](../../../../Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk): **1,388,384,859 bytes**.

SHA256: `456c9d9475461f4bb7bad8d155cbf629cac34ac25a27ff4a0b5a20b8986a1f19`.

Prior input-compatibility APK preserved under `Builds/Verification/v1_17_step01/BeforeAddressablesProfileR1/`.

Exactly two existing project files changed:

- `Assets/AddressableAssetsData/AddressableAssetSettings.asset` — two reference IDs only.
- `Assets/LegendsOfRus/Scripts/Editor/Step17AddressablesContentDeliveryConfigurator.cs` — missing-reference repair and validation.

7,791 baseline files unchanged; 0 added/deleted project files. Newly generated linker files removed after packaging. Art, runtime code, prefabs, gameplay rules, save data, PlayerSettings, packages and Android guards unchanged. No commit, deployment or device command.

[Loading results](QA/Loading/summary.json) · [actual flow](QA/Loading/Captures/LOR_LOADING_CAPTURE_DIR/ActualLoadingFlow.json) · [Edit Mode results](QA/PolicyTests/summary.json) · [foundation validation](QA/FoundationValidation/summary.json) · [build](QA/Build/summary.json) · [configuration evidence](QA/ConfigurationEvidence.json) · [APK inspection](QA/ApkVerification.json) · [APK delta](QA/ArtifactDelta.json) · [scope](QA/Scope.json) · [patch](Sources/TaskChanges.patch).

## Remaining and next action

Splash font provenance remains explicitly unverified by the existing build guard. This is a development APK, not release approval. Human Chapter 1 pacing, physical Android QA-profile/input/lifecycle/performance/readability remain open; phone work and hosting/live services remain deferred, gameplay effects last.

**Next:** audit the roughly 1.39 GB APK's asset footprint and identify safe size reductions before modifying or removing approved resources. Separate measured packaged data from Editor-only source art and do not migrate production loading without evidence.
