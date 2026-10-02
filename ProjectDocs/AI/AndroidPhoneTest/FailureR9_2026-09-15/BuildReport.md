# Failure R9 — Android test build

[Download the Development APK](../../../../Builds/Verification/FailureR9_2026-09-15/LegendsOfRus-Failure-R9-development.apk) — **559.9 MiB** (587,143,496 bytes).

**Update compatibility is unverified:** this APK uses the machine's existing default debug certificate, which differs from the archived September12 test APK. Android will reject an update over an app signed with that earlier certificate. No phone is connected, so the installed app's certificate could not be compared. Do not uninstall or clear the existing app to work around rejection; preserve its save. The original certificate's temporary build configuration directory is no longer present. No private key was inspected/copied, and no project signing settings or package ID were changed.

| Property | Verified value |
|---|---|
| Package | `com.example.legendsofrus.v117debug` |
| Version | `0.0.0`, code1 |
| Platform | ARM64, IL2CPP, Development/debuggable |
| Android | Minimum API26; target API36 |
| Orientation | Portrait |
| Unity |6000.5.9f1 |
| Certificate SHA-256 | `d70ff623d151109369ef9bac67a1e3fc25614002843761741217a2988faf51f0` |
| APK SHA-256 | `b55549761a0920e2cb24b8a32fb32c095127ec87eba5e85d87d3ab2c98a5f298` |

## Work completed

- Inspected the existing instructions, current local changes, policy and guarded build command. Verified all1,831 R9 production hashes before and after work. Preserved all17 snapshotted package/guard/policy files and all93 R9 capture-evidence hashes.
- Ran the existing Android policy test assembly: **99/99 EditMode tests passed**, zero failures/skips.
- Built through `AndroidDevelopmentBuildCommand.BuildDevelopmentApk`, with Development/LZ4/StrictMode and no automatic installation/run. Guards allowed the request; no guard, package, version, signing or production-approval edits.
- First successful build used an isolated XDG configuration and produced a new debug identity. The initial certificate-equality assertion failed. Archived that artifact and rebuilt with the machine's normal environment/default debug identity. Second build succeeded, but its certificate still differs from the historical APK. The final verification records this difference rather than treating it as an update-compatible build.
- `apksigner` verified the final signature; ZIP CRC/integrity passed for401 entries; `zipalign -c -P 16 4` passed. All six packaged ARM64 ELF libraries have load-segment alignment of at least16,384 bytes. No large unreferenced ZIP gaps were found.
- `aapt` verified manifest identity, version, SDK levels, ABI, debuggable flag and portrait orientation. IL2CPP metadata contains `ProductionFailView`, `paid_r9` and `ad_r9` markers. This is static evidence of current code, not an Android rendering test.
- Preserved the previous APK/checksum under `Builds/Verification/v1_17_step01/BeforeFailureR9_2026-09-15/`. The guarded output remains at its policy path; a byte-identical, clearly named copy is linked above. SHA-256 sidecars accompany the final files.

No authored production-file changes. Builds may update ordinary Addressables/generated cache output; these are not included as an authored design patch. Full ProjectSettings byte identity was not snapshotted; relevant settings passed the unchanged guards. User saves were not cleared or migrated. Policy tests used isolated config/save locations; the final build used the normal local environment solely to retain its default debug-signing identity.

## Checks not performed

No device installation/launch, screenshots, Android safe-area/readability/touch/navigation/performance checks, full project suite, new PlayMode UI run, release/AAB/custom-signing build, distribution/publishing, live ad/payment tests or exact-font verification. R9's prior28 Unity UI tests and captures remain separate evidence. Current paid/ad offers still display unavailable; their functionality remains scheduled after UI completion.

[Phone checklist](PhoneChecklist.md) lists the pending checks. The next useful step is to connect the intended phone with USB debugging enabled, inspect its installed test-app certificate, and verify compatibility before attempting an update. The saved user data must remain intact.
