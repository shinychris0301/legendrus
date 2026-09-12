> Historical artifact location: the APK described in this report is now archived under `Builds/Verification/v1_17_step01/BeforeSplashResponsive/`. The main APK path has been replaced by the subsequent Splash responsive correction; see `ProjectDocs/AI/SplashPhoneResponsive/2026-09-12/ImplementationReport.md` for its current checksum and verification.

The [Android phone-test APK](../../../../Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk) was built successfully with Unity **6000.5.9f1** and independently checked. It is **121,788,608 bytes (116.1 MiB)**. No phone installation or device test has been performed.

| APK property | Verified value |
|---|---|
| Package | `com.example.legendsofrus.v117debug` |
| Version | `0.0.0`, code `1` |
| Architecture | ARM64 (`arm64-v8a`), IL2CPP |
| Minimum Android API | 26 (Android 8.0) |
| Target Android API | 36 |
| Orientation | Portrait |
| Build/signature | Development, debuggable, ordinary Android debug signature |
| Launcher label | `LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL` — existing project label retained |

APK SHA-256: `9228eaf915f6a19aa0529d1a236ce3be63ae4f9050802d443985b2ad12c9cf00`.

The existing `AndroidDevelopmentBuildCommand.BuildDevelopmentApk` command completed with exit code 0 through Unity's regular Player, Addressables, IL2CPP and Gradle pipeline. It used Development, LZ4 and StrictMode, with AutoRunPlayer disabled. Existing production and custom-signing restrictions remain in place. [ApkVerification.json](ApkVerification.json) records the build result, manifest fields, verification commands and evidence.

**Checks performed:**

- **102/102 Edit Mode tests passed across two actual Unity runs:** 99 Android policy/options/package/evidence tests, including the eight new package-baseline cases; then three Splash wiring regression tests. Both runs had zero failures and skips. Results: [Android tests](EditModeResults.json), [Splash wiring tests](SplashWiringResults.json).
- Android SDK `apksigner` verified the APK signature, including scheme v2, with one Android Debug signer.
- ZIP integrity/CRC verification passed for all 401 archive entries. SHA-256 was calculated from the final APK.
- `zipalign -c -P 16 4` passed. All six packaged ARM64 ELF libraries have 16,384-byte load-segment alignment. These are static artifact checks, not device compatibility testing.
- All **26 protected Splash files** and **31 delivered Loading files** still match their accepted/delivered hashes. Packages and reviewed policy/configuration boundary files remain unchanged.
- The authored source patch passed a dry-run and replay against isolated before copies; all seven reconstructed files match final hashes. Existing source metadata remains unchanged, and the new test GUID is unique.

The user explicitly approved applying the reviewed development-build correction and building the APK. Five existing source files changed under `Assets/LegendsOfRus/`:

- `Editor/AndroidBuild/Unity/AndroidBuildStateReader.cs`: verifies the exact reviewed manifest and package-lock bytes and updates three explained stale pins.
- `Editor/AndroidBuild/Unity/AndroidBuildFix03Diagnostics.cs`: reports the expected lock fingerprint and current denial guidance.
- `Editor/AndroidBuild/Unity/AndroidBuildMenus.cs`: updates the obsolete package-denial checklist wording.
- `Tests/AndroidBuildEditMode/AndroidBuildFix03EditModeTests.cs`: adds the eight package-baseline cases.
- `Scripts/Editor/SplashTitleR3Validation.cs`: validates Button ancestors in inactive prefab hierarchies and exposes the same validation to isolated tests.

Added `Tests/ProductionCoreLoopUiEditMode/SplashTitleBuildWiringEditModeTests.cs` and its `.meta`, covering the canonical prefab, an inactive clone with five valid Button artwork targets, and rejection of stray raycast decoration. Exact paths, hashes and metadata checks are in [SourceChangeManifest.json](SourceChangeManifest.json); the reviewable changes are in [AppliedSourceChanges.diff](AppliedSourceChanges.diff).

Four approved development settings were applied: explicit Android package identifier, bundle version `0.0.0`, target API 36 and Portrait orientation. Version code remains 1. Only the relevant nonsecret settings were inspected; no complete ProjectSettings/signing-content audit is claimed.

Two intermediate build blockers were resolved. First, serialized orientation `1` meant `UnityEditor.UIOrientation.PortraitUpsideDown`; installed Unity enum metadata confirmed Portrait is `0`, and the setting was corrected. Second, the Splash Editor validator excluded inactive prefab parents and falsely rejected the five existing Button artwork targets. Including inactive parents fixed that check, and all three regression tests passed. The Splash prefab, artwork, builder, runtime and navigation were preserved.

Normal build output changed `Assets/AddressableAssetsData/Android/addressables_content_state.bin` and generated `Assets/AddressableAssetsData/link.xml` with metadata. These are recorded separately and excluded from the authored source patch, along with Library, Temp, Logs and APK output. No package upgrade was made.

**Not performed:** installation, physical-device launch, rendering/navigation/performance or safe-area checks; a new Play Mode runtime regression run for this Android task; the full project test suite; a release build, custom signing or publishing. Exact source-font identity and release font assignments remain unresolved; this Development APK uses the existing permitted fallback. Earlier Loading Play Mode results are separate evidence and are not presented as Android test results.

To test, copy the linked APK to your ARM64 Android phone and open it from the phone's Files app. If Android requests it, allow that app to install this APK, then install and open the game. Test Splash → Start → Loading → Home, including the progress display and phone Back behavior. The launcher will show the existing label listed above.
