# Android device credential storage test results

2026-09-30. The final development APK was installed and tested on the connected **RMX3938, Android 16 / API 36, ARM64**. All three credential-probe phases passed through actual Unity JNI, AndroidKeyStore and Android AtomicFile. Two defects discovered during device verification were fixed. Normal startup now renders the approved title screen without the invalid-callback error overlay.

## Results

| Check | Actual result |
|---|---|
| Final Unity Edit Mode suite | **161 passed, 0 failed/skipped**: 120 build-policy, 28 probe-protocol and 13 campaign-coordinator cases |
| Final guarded development APK build | Succeeded, approximately 49 seconds |
| Device prepare | Passed: pristine read, write, read-back |
| Device process restart | Passed: previous committed value recovered in a different process |
| Device interrupted write | Passed: deliberately unfinished AtomicFile write recovered the previous committed value after force-stop |
| Device corruption | Passed: corrupt owned ciphertext rejected for reads and writes; known fixture subsequently restored |
| Device missing key | Passed: deleting the owned test alias caused reads and writes to fail closed |
| Device cleanup | Passed: test key absent; credential base file, `.new`, `.bak` and ownership directory absent |
| Normal launch without probe extra | Passed: app running in foreground, probe report unchanged, ownership directory remains absent |
| Startup callback error | Fixed: zero `Start() can not take parameters` errors on final normal launch; screenshot inspected |
| Package inspection | Signature, manifest, unchanged permissions, ZIP CRC/16 KiB alignment, native ELF alignment and existing JNI definitions passed |

The final device run reported **nine completed check groups across three phases**. A prior build with the JNI fix also passed; the entire sequence was repeated after the startup fix so final acceptance corresponds to the final APK. These are not synthetic Editor results.

## Fixes discovered on the phone

**Probe did not start.** The first installed R4 APK timed out without a report. `Intent.removeExtra(String)` was incorrectly called as returning an object. The installed Android API 36 SDK declares `void`; corrected the Unity JNI call to `intent.Call(...)`. The initial attempt had no ownership directory, so it stopped before claiming the test vault. [SDK signature evidence](QA/IntentApiSignatures.txt), [initial failure record](QA/InitialProbeFailure.json).

**Campaign helper collided with Unity startup.** A normal-launch screenshot exposed an error console over the title screen. `CampaignDevelopmentUI` implemented the interface method `Start(LevelData, int, Action<CampaignBoardResult>)` as a public method, which Unity interpreted as an invalid lifecycle callback. Changed it to an explicit `ICampaignBoardHost.Start` implementation. The method body, interface contract, artwork and gameplay rules are unchanged. [Before](QA/BeforeStartupFix.png), [final actual-device capture](QA/NormalLaunchDevice.png).

## Artifact and evidence

Installed APK: [LegendsOfRus development build](../../../../Builds/Verification/v1_17_step01/BeforeGameplayProfileR2/LegendsOfRus-v1.17-step01-development.apk), 1,388,383,486 bytes. SHA-256: `6a379c4671143b595d1d0e5ce731cf12d7c40104e8f3dbb3ffbfc26f48a761f0`. Package `com.example.legendsofrus.v117debug`, version `0.0.0` / 1, portrait, minimum API 26, target API 36. This remains a debug artifact, not a RuStore release.

- [Final device acceptance](QA/DeviceAcceptance.json), [prepare](QA/DeviceRuns/20260930T114950477480Z/prepare.json), [restart](QA/DeviceRuns/20260930T114950477480Z/restart.json), [verify](QA/DeviceRuns/20260930T114950477480Z/verify.json), [run status](QA/DeviceRuns/20260930T114950477480Z/RunStatus.json).
- [Cleanup verification](QA/FixtureCleanup.json), [normal startup](QA/NormalLaunch.json), [device metadata](QA/Device.json).
- [Final Unity tests](QA/FinalPolicyAndCampaignTests/results.xml), [build](QA/FinalBuild/summary.json), [APK inspection](QA/ApkVerification.json), [package delta](QA/ArtifactDelta.json).
- [Source scope](QA/Scope.json), [task patch](Sources/TaskChanges.patch), [reusable device runner](Sources/RunDeviceProbe.py).

Only two existing source files changed: `AndroidCredentialDevelopmentProbe.cs` and `CampaignDevelopmentUI.cs`. No added/deleted project files; 7,773 baseline files unchanged. Both Java plugins, their approved fingerprints, build guard, packages, settings, scene/prefab assets and gameplay content remain unchanged. Generated Addressables linker files were removed after the build. Prior APKs are preserved; no app uninstall, app-data reset, backend deployment or Git commit occurred.

## Remaining limits and next action

A Google Play `com.google.android.play.core.assetpacks.AssetPackManager` class-lookup error still appears in the final process startup log. The app rendered and the probe completed without an observed fatal exception, but this is an open Android/RuStore compatibility investigation; this report does not declare the entire startup log error-free. No Google Play SDK was added to suppress it.

Coverage is one Android 16 handset. Android 26–35, the older AtomicFile `.bak` branch, physical power loss and hardware-backed key attestation are not established by this run. Key deletion was tested, not every hardware or biometric invalidation path. No account credentials were exported: the probe used its disposable fixture and reported fixed statuses only.

The account service remains unconfigured. **Authenticated backend session recovery, hosted HTTPS, linked accounts, purchases, social features and full gameplay/device performance remain untested.** Next prepare the hosted HTTPS backend and explicit device session integration, and track the AssetPackManager lookup separately before release acceptance. Earlier broad regression and human campaign acceptance gaps remain open. Animation/effects remain last.
