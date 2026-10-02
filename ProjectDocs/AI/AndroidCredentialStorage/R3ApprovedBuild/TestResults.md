# Android credential packaging verification

**Device follow-up:** [R5](../R5DeviceExecution/TestResults.md) fixes the probe JNI entry and a startup callback error, then passes all three storage-probe phases on an Android 16 handset. The results below describe the earlier packaging/probe implementation stage.

2026-09-30. Applied the user-approved, exact two-plugin exception and successfully built the current Unity project as a development APK. Compilation and package inspection passed. Android credential storage has **not** yet been exercised on a device, and the normal account service remains unconfigured.

## Test results

| Check | Result |
|---|---|
| Unity 6000.5.9f1 build-guard Edit Mode suite | **120 passed, 0 failed, 0 skipped** |
| New guard regression coverage | 21 of those cases verify the exact inventory and continued rejection of altered/missing files, extra plugins, manifests, templates, forbidden directories and embedded packages |
| Guarded Android development build | **Succeeded**, exit 0, approximately 182 seconds |
| APK signing | Verified; same public signing certificate as the previous local development APK |
| Manifest | Expected development package/version, ARM64, portrait, minimum API 26, target API 36, debuggable |
| Permissions | Identical to previous local APK; no new storage permission |
| ZIP integrity and alignment | CRC passed; 16 KiB page alignment check passed |
| Native libraries | All six ARM64 ELF libraries have load-segment alignment of at least 16 KiB |
| Java plugin packaging | Nine credential classes present in DEX; both public static JNI entry methods and public volatile result fields have the expected definitions/signatures |
| Managed runtime packaging | Credential store, account service and session client names present in IL2CPP metadata; this is presence evidence, not execution evidence |
| Android device | No connected device; no install, launch, storage probe or restart test performed |

The first test launch used a fresh isolated configuration and exited 198 before compilation because Unity could not obtain an Editor license there. The retry with the machine's normal configuration compiled and passed all 120 Edit Mode cases. No license change or bypass was made. These tests only exercise build policy and disposable filesystem fixtures; no Play Mode or player-save test ran.

## Built artifact

[Development APK](../../../../Builds/Verification/v1_17_step01/BeforeCredentialStorageR4/LegendsOfRus-v1.17-step01-development.apk): 1,388,383,486 bytes, approximately 1.39 GB. Package `com.example.legendsofrus.v117debug`, version `0.0.0`, version code `1`.

SHA-256: `2fc7a80e92e867037c277b9a59f740042a7a73d0b5902f009d992f793b51a9cf`.

The previous APK is preserved in `Builds/Verification/v1_17_step01/BeforeCredentialStorageR3/`. Signature comparison was against that file, not an installed application. This is a local debug artifact, not a RuStore release build or live-account release.

## Installed changes

The approved patch permits only the exact reviewed paths for `CredentialVault.java` and `AndroidCredentialBridge.java`, and requires both sources and both importer metadata files to match their pinned SHA-256 fingerprints. All general Java/plugin restrictions and release, signing, package, configuration and commercial activation guards remain in force.

Changed `AndroidBuildStateReader.cs`; added `ReviewedAndroidCredentialPlugins.cs`, its metadata, and `AndroidCredentialPluginGuardEditModeTests.cs` with metadata. Across Assets, Packages, ProjectSettings and Backend: **one existing file changed, four added, none deleted, 7,762 unchanged**. No runtime, art, campaign, gameplay or settings change in this task. Removed two newly generated Addressables linker files after packaging; task-start content-state files remained unchanged. No commit or deployment.

## Evidence and next step

- [Unity test XML](QA/PolicyTestsDefaultConfiguration/results.xml), [test summary](QA/PolicyTestsDefaultConfiguration/summary.json), [initial isolated launch](QA/PolicyTests/summary.json).
- [Build summary](QA/Build/summary.json), [build log](QA/Build/unity.log), [APK inspection](QA/ApkVerification.json), [DEX definitions](QA/CredentialDex.json), [permissions comparison](QA/Permissions.json).
- [Device status](QA/DeviceStatus.json), [source scope](QA/Scope.json), [task patch](Sources/TaskChanges.patch), [generated-file cleanup](QA/GeneratedFileCleanup.json).

Next: prepare and execute an explicit development-only credential probe on a connected Android device, using disposable test state. Verify pristine read, write/read, process restart, interrupted write, corrupted record and invalidated/missing key behavior without accessing a player's real credentials. Normal game launch alone will not exercise the currently unconfigured account adapter. Then verify the session flow against hosted HTTPS before account UI activation.

AndroidKeyStore/JNI/AtomicFile execution, power-loss behavior, hardware-backed protection, hosted backend connectivity and device performance remain unverified. The earlier broad runtime failures and human campaign acceptance remain separate open work. Animation and gameplay effects remain last.
