# Gameplay QA development APK — build and inspection results

**Succeeded.** Unity 6000.5.9f1 built the current project through the existing guarded development command in approximately 120 seconds. No approval guard, package, project setting, source, art or prefab changes were required. The APK has not been installed or launched on a phone.

| Check | Result |
|---|---|
| Android build-policy Edit Mode suite | **120 passed, 0 failed, 0 skipped** |
| Compiled profile-entry guard harness | **22 passed, 0 failed**; real profile source, simulated JNI/preferences, development and release preprocessor variants |
| Guarded ARM64 IL2CPP development APK | **Succeeded**, exit 0 |
| APK signature | Verified; matches the archived previous development APK certificate |
| Manifest | Expected debug package, version 0.0.0 / 1, portrait, API 26–36, ARM64, debuggable |
| Permissions | Unchanged; no added storage permission |
| ZIP and native alignment | CRC passed; 16 KiB ZIP alignment passed; all six native libraries have at least 16 KiB ELF load alignment |
| Profile inclusion | Profile class, debug badge, intent key and isolated preference prefix present in packaged IL2CPP metadata |
| Startup registration | One existing RuntimeBootstrap.Boot; only added callback is profile SubsystemRegistration reset |
| Reviewed credential plugin | Both Java files and importer metadata still match approved pins; nine credential DEX classes and JNI member signatures verified |
| Source scope | **7,791 baseline Assets/Packages/ProjectSettings/Backend files unchanged; 0 changed, added or deleted** after cleanup of two newly generated Addressables linker files |
| Device work | **Deferred:** no ADB invocation, install, launch, player-data access or new device result |

The 22 harness checks cover ordinary launch, fresh/resume/zero-energy fixtures, invalid command, missing/wrong ownership marker, wrong package, missing debuggable flag, simulated JNI failure and fixture-write failure. Development failures remain blocked on subsequent access. All request variants remain inactive without DEVELOPMENT_BUILD, with no JNI/storage calls and no debug-badge type. This is a standalone compile of the actual source with fake platform dependencies, **not a Unity release build or real Android JNI execution**. Previous R1 gameplay passes were not rerun or counted in these results.

## Artifact

[Development APK](../../../../Builds/Verification/v1_17_step01/BeforeAndroidInputR1/LegendsOfRus-v1.17-step01-development.apk): **1,388,367,408 bytes**, approximately 1.39 GB.

- SHA-256: `019d3d6a66b2363e762d9c33a44621da8dcfdec65711605015065bce5506efd1`
- Package: `com.example.legendsofrus.v117debug`
- Launch activity: `com.unity3d.player.UnityPlayerGameActivity`
- Version: `0.0.0`, code `1`
- Previous APK preserved under `Builds/Verification/v1_17_step01/BeforeGameplayProfileR2/`; its hash matches the earlier R5 device-tested artifact.

This is a local development APK, not a RuStore release. Package identity/version was intentionally preserved. The seven changed ZIP entries reflect compiled/runtime/build data; Java sources and permissions are unchanged. Metadata presence establishes inclusion, not successful on-device behavior.

## Build warnings retained

The build completed but emitted existing warnings for dual Active Input Handling (`Both`, unsupported on Android), unverified Splash typography and an empty Addressables profile reference. The same input warning is present in the earlier R5 build log. No input setting, font, package or approval was silently changed. These remain Android/release readiness issues; build success does not accept input performance, source-font fidelity or remote content delivery.

## Evidence

- [Policy test summary](QA/PolicyTests/summary.json), [XML](QA/PolicyTests/results.xml).
- [Build summary](QA/Build/summary.json), [build log](QA/Build/unity.log), [artifact hash](QA/Artifact.json).
- [Compiled entry guard checks](QA/EntryGuards/summary.json), [harness source](Sources/EntryGuardHarness.cs).
- [APK inspection](QA/ApkVerification.json), [permissions/artifact delta](QA/ArtifactDelta.json), [DEX inspection](QA/CredentialDex.json), [bootstrap inspection](QA/BootstrapInspection.json).
- [Source scope](QA/Scope.json), [generated-file cleanup](QA/GeneratedFileCleanup.json), [previous artifact identity](QA/PreviousArtifact.json).

**Next:** review the existing Android input adapter and the `Both` warning, then prepare a narrow configuration correction with compatibility checks. Keep phone installation/testing deferred until resumed by the user. The new profile's cold-launch intent, QA label, save isolation and lifecycle still need real device validation. Hosting/live-service activation remains postponed; gameplay effects remain last.
