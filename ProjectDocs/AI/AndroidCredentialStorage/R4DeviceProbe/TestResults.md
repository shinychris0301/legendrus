# Android credential device probe implementation and results

**Device follow-up:** [R5](../R5DeviceExecution/TestResults.md) fixes the probe JNI entry and a startup callback error, then passes all three storage-probe phases on an Android 16 handset. The results below describe the earlier packaging/probe implementation stage.

2026-09-30. Implemented an explicitly invoked development-only tool for testing the existing Unity-to-Java credential bridge. The current development APK builds and passes package inspection. **No Android device is connected, so no on-device storage, restart or fault-injection result is claimed.**

## Executed checks

| Check | Result |
|---|---|
| Unity 6000.5.9f1 Edit Mode | **148 passed, 0 failed/skipped:** 120 existing build-policy cases and 28 new probe-protocol cases |
| Probe protocol fixtures | Three-process sequence, existing-vault refusal, account/development gate, ownership/phase/PID checks, changed ciphertext, marker/write/read failures, corruption/missing-key failure detection and cleanup restrictions |
| Device runner report validation | **12 passed:** complete result requirements, stale phase, unexpected fields, malformed checks, incomplete/duplicate success, wrong schema, oversized data and unsupported session-success claims |
| Guarded development APK | **Succeeded**, exit 0, approximately 70 seconds |
| Package inspection | Signature matches previous APK; manifest and permissions unchanged; ZIP CRC and 16 KiB alignment passed; six native libraries have at least 16 KiB ELF load alignment |
| Java packaging | Nine existing credential DEX classes and expected JNI method/field definitions verified; both Java sources and importer metadata still match the approved fingerprints |
| Probe packaging | Probe protocol, Android host and entry point present in IL2CPP metadata; native library and metadata changed from the prior APK |
| Non-development exclusion check | Standalone C# compile without Editor/development symbols emitted zero probe types; **not an actual Unity release build** |
| Android device execution | **Not run:** zero connected devices; no installation or credential access attempted |

The 28 protocol tests use simulated storage and do not execute AndroidKeyStore, AtomicFile or JNI. Previous host cryptography tests were not rerun because the Java implementation is unchanged. Neither these results nor the new APK establish hardware-backed protection, power-loss durability, full authenticated session recovery or commercial readiness.

## Implemented behavior

The entry point runs on the existing `BackendAccountService` when an Android development build receives the explicit `lor_credential_probe` intent extra. It adds no screen, Canvas, startup object, automatic network request or production account activation. Native operations and local test-file work run on an attached background JNI thread; polling is bounded. Normal launch without the extra does not run the probe.

The probe uses the actual reviewed Java bridge and its normal vault, **only after checking that the credential record, AtomicFile sidecars and Keystore alias are all absent**. It writes a durable ownership marker before accessing the vault and uses a clearly synthetic, non-account payload. An existing record, alias, ownership directory or configured account causes refusal before credential reads/writes. Account configuration is blocked while the probe runs or its ownership directory remains. No real account token or player save is used.

The sequence is:

1. `prepare`: claim an empty vault, verify pristine read, write/read the disposable payload, record its ciphertext fingerprint and current process ID.
2. `restart`: require a different process ID and unchanged owned ciphertext; verify read after restart. Begin and sync an unfinished AtomicFile write and retain the open stream until force-stop.
3. `verify`: require another process and the exact expected partial-write evidence, supporting both older `.bak` and newer `.new` AtomicFile layouts. Verify previous committed data recovery, corrupt the owned ciphertext and require read/write rejection, restore it, delete only the owned test key and require missing-key rejection, then remove the verified test fixture.

The key test is deliberate deletion of the test alias, not a biometric/hardware invalidation simulation. Force-stop is process termination, not a physical power-cut test. Results explicitly report `authenticatedSessionTested: false`; hosted session recovery is a later integration step.

Only fixed statuses and check names are exported. Exceptions, payloads, ciphertext, account identifiers and keys are never included in the report. Failures retain ownership and stop further work. Cleanup refuses changed/unknown data, initializing states and unfinished writes. It does not uninstall the app, clear app data or reset player progress.

## Running on an Android test device

Use an idle development test installation with USB debugging authorized. From the repository root:

```bash
python3 ProjectDocs/AI/AndroidCredentialStorage/R4DeviceProbe/Sources/RunDeviceProbe.py --phase check
python3 ProjectDocs/AI/AndroidCredentialStorage/R4DeviceProbe/Sources/RunDeviceProbe.py --phase all --install
```

`--install` replaces the development APK using `adb install -r`, preserving application data. `all` explicitly force-stops and starts the app three times. If the credential vault already contains anything, the tool refuses instead of resetting it. With multiple devices, specify `--serial`.

Individual phases are `prepare`, `restart`, `verify` and `cleanup`. A `failed_retained` or `refused` result stops the runner; do not clear real credentials to force a pass. `cleanup` only removes a fingerprint-verified owned fixture when its state is eligible. An incomplete initializing marker or unmatched storage requires investigation on that test installation; the runner does not guess ownership.

The runner removes only its previous redacted report after force-stop, then waits for a fresh phase-matching report. Successful reports must include the complete expected check list. Collected results go under `QA/DeviceRuns/`; no such execution evidence exists yet.

## Artifact and evidence

[Development APK](../../../../Builds/Verification/v1_17_step01/BeforeCredentialStorageR5/LegendsOfRus-v1.17-step01-development.apk): 1,388,383,486 bytes. SHA-256 `bb1d006df340c3760ec5c9b27cdaf5ed8a3636cba4e838ebda25467438aeea15`. Development package `com.example.legendsofrus.v117debug`, version `0.0.0`/1, ARM64, API 26–36, portrait. The previous APK is preserved under `BeforeCredentialStorageR4/`.

- [Unity tests](QA/PolicyTestsDefaultConfiguration/results.xml), [summary](QA/PolicyTestsDefaultConfiguration/summary.json), [device-runner tests](QA/DeviceRunnerTests.log).
- [Build summary](QA/Build/summary.json), [APK inspection](QA/ApkVerification.json), [DEX definitions](QA/CredentialDex.json), [artifact and permissions delta](QA/ArtifactDelta.json), [exclusion check](QA/ReleaseExclusion.json).
- [Device status](QA/DeviceStatus.json), [source scope](QA/Scope.json), [task patch](Sources/TaskChanges.patch), [cleanup](QA/GeneratedFileCleanup.json).

Changed two runtime service files; added three development-probe sources, one Editor test source and their four metadata files. Scope: two existing files changed, eight added, none deleted, 7,765 unchanged across Assets/Packages/ProjectSettings/Backend. Existing Java files, guard pins, production settings, art, campaign, rules and effects are unchanged. Removed two newly generated Addressables linker files after building. No release build, deployment or commit.

**Next:** connect an Android test device and execute the three-phase probe. Resolve any real JNI/AtomicFile/Keystore failures before hosted HTTPS account-session integration. Linked-account recovery, live commerce/social, broad runtime and human campaign acceptance remain pending. Animation/effects remain last.
