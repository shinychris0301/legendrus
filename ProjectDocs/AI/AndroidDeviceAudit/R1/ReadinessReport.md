# Android Device Audit R1 — Development APK ready

The approved one-line application-ID change is applied. The guarded Development APK build succeeded with Unity 6000.5.9f1. No Android phone is connected, so installation and physical-device testing are pending.

[Development APK](../../../../Builds/Verification/AndroidDeviceAuditR1/LegendsOfRus-UI-Audit-R1-development.apk) — **1.29 GiB** (1,380,231,718 bytes). [SHA-256 sidecar](../../../../Builds/Verification/AndroidDeviceAuditR1/LegendsOfRus-UI-Audit-R1-development.apk.sha256).

[Verification](QA/ApkVerification.json) · [Readiness](QA/Readiness.json) · [Device checklist](DeviceChecklist.md) · [Scope](QA/PostBuildScope.json)

| Property | Verified value |
| --- | --- |
| Package | `com.example.legendsofrus.v117debug` |
| Version | `0.0.0`, code 1 |
| Build | Development/debuggable, ARM64, IL2CPP |
| Android | Minimum API 26; target API 36; portrait |
| APK SHA-256 | `020a1fdd8c71dcb5847f0b7c9d6419ac56d5d65c9bf8b6b514c4358d25498f70` |
| Certificate SHA-256 | `d70ff623d151109369ef9bac67a1e3fc25614002843761741217a2988faf51f0` |

## Work and validation

Applied exactly the user-approved Android application identifier change in `ProjectSettings/ProjectSettings.asset`. No signing, backend, package, version or guard/policy changes were made. The existing `AndroidDevelopmentBuildCommand.BuildDevelopmentApk` performed its normal guarded build; there was no auto-install or auto-run.

Verify02 passed **99/99 Android policy EditMode tests**, zero failures/skips. The build then completed successfully. `apksigner` verified the APK signature, `aapt` confirmed package/version/API/ABI/debuggable/portrait properties, ZIP integrity passed all 401 entries, and `zipalign -c -P 16 4` passed. All six packaged ARM64 ELF libraries have load-segment alignment of at least 16,384 bytes. These are static alignment checks, not a real 16 KB page-size device test.

The certificate matches the prior local September-15 development APK. Installed-phone certificate compatibility remains unverified, and the earlier September-12 artifact used a different certificate. Do not infer compatibility with every historical install. Different package IDs use separate Android app data; this change does not migrate a DefaultCompany installation's progress.

IL2CPP metadata includes `AdaptiveCanvasScaler`, `lastTextScale` and `ProductionSettingsView`, confirming the new text-refresh field is packaged. This is static artifact evidence, not proof of visual clarity on a phone. The earlier Text Clarity R1 Unity captures and 24 PlayMode checks remain separate evidence.

All **7,198 snapshotted Assets and Packages files remained byte-identical**. The project-setting diff contains only the approved one-line ID change. The build updated ordinary generated caches, which are not an authored source patch. Previous APKs/checksums are preserved in `Builds/Verification/v1_17_step01/BeforeAndroidDeviceAuditR1/` and `BeforeAndroidDeviceAuditR1_Verify02/`. The successful policy-path APK was copied to the clearly named linked artifact without changing its bytes.

## History and limits

Verify01 was blocked by `ANDROID_DEVELOPMENT_IDENTITY_MISMATCH`; that evidence remains in `QA/Verify01` and `QA/ReadinessBlockedVerify01.json`. The user approved the prepared patch before Verify02. The earlier exported diagnostics describe the blocked state and are retained as historical evidence, not the successful build's current status.

ADB reported zero devices both before and after this build. No installation, launch, native phone screenshot, actual background/foreground, force-close, hardware Back, readability or performance check was performed. No release/AAB/custom-signing build, upload, distribution or store publication occurred. No credential or private key was inspected/copied. Policy tests used isolated storage; the APK build used the normal machine environment to retain the existing default debug-signing identity.

Next: connect and authorize the intended phone, compare the installed app certificate if present, then install without clearing data and run the device checklist. Until then this artifact is **build-verified, not device-accepted**.
