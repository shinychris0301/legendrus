**Historical pre-authorization review. The user subsequently authorized the correction/settings and the APK was built successfully. See [BuildReport.md](BuildReport.md) and [ApkVerification.json](ApkVerification.json) for the final outcome.**

At this review checkpoint, the requested phone-test APK was **not built yet**. Unity 6000.5.9f1's existing diagnostic command completed successfully and reported `ANDROID_PACKAGE_LOCK_UNAPPROVED`. No Player/Addressables/Gradle build was attempted while this prerequisite remained blocked. No guard, package, signing, runtime, artwork or Player setting had been changed.

The existing build entry is `LegendsOfRus.AndroidBuild.AndroidDevelopmentBuildCommand.BuildDevelopmentApk`. It requests a Development APK with LZ4 and StrictMode, using only `Assets/Scenes/Bootstrap.unity`, at `Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk`. It retains the project's normal guards and does not install or run the app. The user's current Development checkbox is off; this dedicated entry already supplies Development for its request and temporarily aligns/restores the related UI switches.

The concrete unapplied correction is [GuardCorrection.proposed.diff](GuardCorrection.proposed.diff). It changes three Editor sources and one existing Editor test file, preserving their metadata. It replaces unconditional package denial with exact verification of the reviewed manifest and resolved lock; missing, unreadable or modified package inputs remain denied. It updates only three understood stale pins, exposes the expected lock hash in diagnostics, updates an obsolete checklist statement and supplies eight parameter-expanded tests. The existing policy JSON, production/custom-signing restrictions, commercial configuration pins, profile restrictions and output restrictions remain unchanged.

[PackageBaselineReview.json](PackageBaselineReview.json) contains the exact fingerprints and package graph. All 23 resolved packages are official Unity built-in/registry packages; there are no local/Git/embedded dependencies or missing dependency keys. The three stale pins have specific explanations: the manifest matches the documented supplied archive, ProjectVersion gained Unity's revision line, and EditorBuildSettings gained the existing Addressables default-object binding. The policy JSON and all three economy/commercial pins still match. No package upgrade is proposed.

Actual Unity preflight also found four Player settings that must meet the existing development-only policy before building:

| Setting | Current local value | Existing APK policy requires |
|---|---|---|
| Android application identifier | No explicit identifier serialized; Unity reports policy mismatch | `com.example.legendsofrus.v117debug` |
| Bundle version | `1.0` (Android version code already `1`) | `0.0.0`, code `1` |
| Android target API | Automatic (`0` serialized) | Explicit API `36` |
| Default orientation | Auto Rotation (`4` serialized) | Portrait |

These required values are stated here for review; no Player setting has been applied. The build can use Unity's ordinary debug signing because Custom Keystore is already off. No credentials or keystore contents need to be read. The application ID above gives this development build its own package identity; installing it would not replace an app with a different ID. Installation itself is not part of the present request.

Other actual checks passed: Android is already the active target; module available; no custom Build Profile; Bootstrap is the sole enabled scene; minimum API supported (26); IL2CPP and ARM64 enabled; game category configured; App Bundle/Gradle export/split APK/split data/custom signing disabled; native symbols disabled; safe scripting defines. Installed tool discovery found SDK platforms 34/36, build tools 36.0.0, NDK 27.2.12479018 (r27c), and OpenJDK 17.0.18. The development font fallback is permitted by the existing Splash build guard; exact release typography is still unverified.

Checks performed in this session:

- Read required project instructions, current task/handoff, current Git status/diffs, actual build entry, policy, evaluator, state reader, diagnostics and typography build guard.
- Verified all 31 delivered Loading files against their implementation manifest, and all 26 protected Splash hashes; no differences.
- Compared all protected policy fingerprints and the resolved package graph; explained the three mismatches against actual archive/Editor/Addressables files.
- Ran existing `AndroidDevelopmentBuildCommand.ExportDiagnostics` in real Unity 6000.5.9f1, batch mode, with isolated configuration and local IPC access; exit 0. See [UnityPreflight.json](UnityPreflight.json). Its reported `ReleaseApk` intent reflects the current UI checkbox, not an attempted release build.
- Inspected only the relevant nonsecret serialized identity/version/API/orientation fields to explain false preflight facts.
- Ran `patch --dry-run -p1` on the unapplied four-file proposal; all hunks apply. Rechecked that the actual four source files still have their original hashes. This is a text/diff check, not C# compilation or a Unity test pass.
- Compared the postlaunch source/art/package/settings snapshot. The only retouched snapshotted file was ProjectAuditorSettings.asset: same content hash and byte length, newer modification time. ProjectSettings.asset metadata stayed unchanged; no signing contents were read. Current Git root is this project, unlike the earlier historical handoff state.

Not performed: application of the proposal, its C# compilation or eight new tests, Android Player/Addressables/Gradle build, APK creation/signature/archive inspection, installation, physical-device testing, package upgrades, release-font verification, or a new full/Play Mode regression suite. The prior Loading session's 11 passing Play Mode tests remain separate evidence.

The next action requires authorization to change the guard's approval logic because the earlier user instruction explicitly forbade Android guard changes. [AGENTS.md](../../../../AGENTS.md) also requires separate authorization and a verified blocker before bypassing Android approval guards. This proposal provides a reviewed exact-input check rather than removing the guard. Once authorized, apply the proposed correction, configure the four documented development settings, run the focused guard tests/preflight, then run the existing Development APK command and verify the produced APK. Any later build failure must be reported from actual evidence; the current inspection does not promise a successful APK.
