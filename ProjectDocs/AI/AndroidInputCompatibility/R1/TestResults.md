# Android input compatibility R1 — results

**Fixed:** Active Input Handling now uses **Input System Package (New)** only. The rebuilt Android APK no longer emits the `Both`/unsupported-input warning. **Final: 18 Play Mode compatibility tests and 120 build-policy Edit Mode tests passed; 0 failed or skipped.**

## Change and reasoning

RuntimeInput already reads Keyboard through Unity.InputSystem, and ScreenRouter already installs InputSystemUIInputModule and removes any legacy StandaloneInputModule. No runtime legacy Input API calls were found in project sources. The existing Unity6ProjectSettingsMigration helper also expects value1. The global project setting was still2/Both; it is now1/New. No runtime adapter, page, input binding, art or gameplay rule needed replacement.

The user explicitly approved addressing this Android warning. ProjectSettings.asset differs from the task-start copy by **one value only: activeInputHandler 2 → 1**. No graphics backend, color-space, package, API, signing, application identity or build guard change. The existing helper already agrees with the new setting.

## Verification

| Check | Result |
|---|---|
| New-only compiled backend and UI module | Passed: new system enabled, legacy backend disabled, one EventSystem, new UI module, no legacy module |
| Queued touch/mouse/Escape through real Home/map controls | Passed: touch opens World Map; Escape follows Back to Home; mouse opens World Map; no energy/currency changes |
| Match-3 compatibility | 12 cases passed: four drag directions, tap-pair swap, pause/resume hash, booster cancel/invalid/use accounting, blocked swap, timeout/refund, special-chain wave hashes |
| Native journeys/lifecycle/clock | 4 cases passed: win/reward/return, fail/free Retry/exit, callback/input recovery and capped energy catch-up |
| Build-policy Edit Mode tests | 120 passed |
| Guarded Android ARM64 IL2CPP APK | Succeeded, exit0, approximately 95seconds; dual-input warning absent |
| Package inspection | Signature matches archived prior APK; permissions/identity unchanged; ZIP CRC,16KiB ZIP alignment and all six ELF-library alignments passed |
| Existing credential plugin | Approved source/importer fingerprints and nine DEX classes/JNI member signatures verified |
| Phone | Deferred: no ADB invocation, installation, launch or hardware input measurement |

Queued device events use Unity's actual Input System and native UI processing, not direct button.onClick or ExecuteEvents calls. The headless Editor test temporarily clones input settings to ignore Game-view focus, restores them afterward and removes its virtual devices. This verifies software routing, not real Android touch, hardware Back or background focus behavior. The separate lifecycle tests simulate callbacks; physical lifecycle acceptance remains pending.

The first new test run passed the backend check but did not deliver touch events because the headless Game view lacked focus. The next run delivered events but exposed a harness cleanup error: replacing the Input System's temporary settings destroyed the original object. The harness now retains/restores it correctly. The final combined18-case run passed. These intermediate failures were test-harness issues; no production focus policy was weakened.

## Artifact

[Archived input-compatibility APK](../../../../Builds/Verification/v1_17_step01/BeforeAddressablesProfileR1/LegendsOfRus-v1.17-step01-development.apk): **1,388,384,859bytes**.

SHA-256: `7eac5728db19f01bff499ae86714371962ab975e7cf85c68bb3abf2213a0a1f2`.

Package `com.example.legendsofrus.v117debug`, version0.0.0/code1, portrait, ARM64, minimum API26 and target API36, debuggable. Previous APK archived at `Builds/Verification/v1_17_step01/BeforeAndroidInputR1/`. The new disposable gameplay profile remains present in packaged metadata. This is a development build, not a RuStore release or device-tested artifact.

## Evidence and limits

[Compatibility summary](QA/Compatibility/summary.json) · [compatibility XML](QA/Compatibility/results.xml) · [policy summary](QA/PolicyTests/summary.json) · [build summary](QA/Build/summary.json) · [input-setting/warning evidence](QA/InputConfigurationEvidence.json) · [APK inspection](QA/ApkVerification.json) · [permissions/plugin comparison](QA/ArtifactDelta.json) · [scope](QA/Scope.json) · [task patch](Sources/TaskChanges.patch).

Changed the project setting and test-assembly reference; added one Play Mode test and its metadata. **7,789 baseline files unchanged,2 changed,2 added,0 deleted.** Newly generated Addressables linker files removed after building; existing generated state retained. No production C# or prefab edit. Existing GUIDs and approved assets preserved. Play Mode used isolated product storage and the existing process-only Vulkan authorization; no real player save was used. Build used the current development signing configuration through existing guards.

The remaining build warnings concern unverified Splash font provenance and an empty Addressables profile reference. Neither was changed in this task. No full-project regression, release build, physical-device gameplay/performance/readability acceptance, backend activation, deployment or commit.

**Next:** inspect and correct the empty Addressables profile-reference warning through its existing local-content configuration, with a narrow build/loading regression. Keep phone installation/testing deferred. Human Chapter1 pacing, Android QA-profile/lifecycle/input acceptance and font provenance remain open; hosting/live services deferred and gameplay effects last.
