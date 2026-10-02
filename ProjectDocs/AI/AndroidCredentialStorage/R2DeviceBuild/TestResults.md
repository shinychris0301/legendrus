# Android credential storage R2 — guarded build blocked

**Follow-up:** the user subsequently approved this exact exception. It is applied and the development APK passed packaging checks in [R3](../R3ApprovedBuild/TestResults.md). The report below preserves the original blocked attempt.

2026-09-30. Continued the approved Android plugin inventory/build step. The current development APK attempt stopped before BuildPipeline: `ANDROID_COMMERCIAL_BOUNDARY_CHANGED`. The guard categorically rejects `.java`; the two credential source plugins therefore invalidate its artifact check. Protected configuration hashes and the package lock still match. No new APK was produced or installed.

| Check | Actual result |
|---|---|
| Unity 6000.5.9f1 Android-build Edit Mode assembly | 99 passed, 0 failed/skipped |
| Guarded development APK command | Exit 1; rejected before packaging |
| Proposed exact plugin exception, standalone compiled C# | 26 checks passed, 0 failed |
| Proposed patch applicability | `git apply --check` passed; patch **not applied** |
| Project source/configuration scope | All 7,763 baseline files unchanged; 0 added/deleted |
| Android devices | `adb devices`: no devices attached |
| Previous development APK | Restored to its original output path; SHA-256 verified unchanged |

## Concrete proposal awaiting approval

[ProposedCredentialPluginException.patch](Sources/ProposedCredentialPluginException.patch) adds one pure C# inventory helper and its metadata, and changes one condition in `AndroidBuildStateReader`. It permits only the exact reviewed `CredentialVault.java` and `AndroidCredentialBridge.java` paths when **both sources and both Android-only importer metadata files** match pinned SHA-256 fingerprints. [Pinned inventory](QA/PinnedPluginInventory.json).

The blanket Java restriction remains for all other files. Existing forbidden extensions, plugin directories, embedded packages, configuration fingerprints, development identity, release/AAB/signing/commerce gates remain unchanged. No approval flag is enabled. Future plugin edits will require another inventory review. This does not certify production readiness or enable accounts.

[Host checks](QA/GuardExceptionHostTests.log) exercise both accepted sources, mutation of each source/metadata file, missing files, copied source at another path, unlisted plugin types, null/wrong roots and restored inventory. This is a standalone Mono compilation of the proposed helper, **not Unity integration or Android packaging verification of the proposed guard**. [Re-run harness](Sources/RunProposedGuardTests.py).

Root [AGENTS.md](../../../../AGENTS.md) says: “Do not upgrade Unity/packages, change backend/color space, or bypass Android approval guards unless separately authorized and justified by a verified blocker.” The verified blocker and exact proposed exception are now reviewable. The earlier approval to continue the build step was not treated as approval to change this guard.

## Evidence and limitations

- [99 policy test results](QA/PolicyTests/results.xml), [test summary](QA/PolicyTests/summary.json).
- [Failed build log](QA/Build/unity.log), [build command/result](QA/Build/summary.json), [sanitized diagnostics](QA/Build/AndroidBuildDiagnostics.json). Diagnostics export reflects the Editor UI's `ReleaseApk` intent; the actual attempted command explicitly requested the existing guarded **development** APK. No release build was attempted.
- [Scope](QA/Scope.json), [previous artifact and verified hash](QA/PreviousArtifact.json), [proposal checks](QA/ProposalVerification.json).
- The restored old APK is historical evidence and does **not** contain this new credential adapter or current campaign. Its temporary archive path no longer exists after restoration.
- JNI, AndroidKeyStore, Android AtomicFile interruption/restart/key invalidation, Gradle packaging and hosted HTTPS remain unverified. Merely packaging the adapter will not exercise it: the normal account service remains unconfigured.
- No art, gameplay, campaign, runtime source, package, settings or active build-guard change; no deployment/commit. The build attempt refreshed the existing diagnostic exports under `ProjectDocs/AndroidBuildFix03/`.

Next after specific approval: apply this exact exception, add/run Unity regression coverage for its integration with the remaining artifact bans, rerun guarded development packaging and inspect the resulting manifest/plugin inclusion. Then run explicit credential storage probes on a connected Android device before enabling account UI. Effects remain last.
