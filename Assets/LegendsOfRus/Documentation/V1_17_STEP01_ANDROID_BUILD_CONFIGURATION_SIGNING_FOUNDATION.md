# v1.17 Step 1 — Android Build Configuration and Signing Workflow Foundation

## Decision

Implementation status:
`IMPLEMENTED_IN_SOURCE_PENDING_UNITY_ANDROID_CONFIGURATION_AND_VERIFICATION`

Android candidate status:
`DEVELOPMENT_APK_GUARD_READY_PACKAGE_LOCK_APPROVAL_PENDING_PRODUCTION_BLOCKED`

This is a source contract foundation, not a production build or signing
implementation. The compiled `ResolvedPackageLockApproved` gate is deliberately
`false`, so these exact bytes cannot pass Android preflight or build an APK.
**Do not build the current candidate.** Unity compilation, self-test, Test
Runner, APK build, signing, and representative-device results are
`NOT_RETAINED` for these exact bytes.

The inherited v1.15 economy and v1.16 commercial capabilities remain disabled.
No runtime, gameplay, scene, resource, package, or approved PNG is changed.

## Current Android production baseline

The policy was reviewed against official requirements current on 2026-09-02:

- New Play apps and updates must target Android 16 / API 36 or newer:
  <https://developer.android.com/google/play/requirements/target-sdk>
- New Play applications publish as Android App Bundles:
  <https://developer.android.com/guide/app-bundle>
- Native applications must support 64-bit architectures:
  <https://developer.android.com/google/play/requirements/64-bit>
- New Play applications use Play App Signing; the developer protects the
  separate upload key:
  <https://developer.android.com/studio/publish/app-signing>
- Android version codes are positive, unique, increasing, and no greater than
  2,100,000,000:
  <https://developer.android.com/studio/publish/versioning>
- API 35+ applications must support 16 KB pages on 64-bit devices:
  <https://developer.android.com/guide/practices/page-sizes>
- Closed, open, and production tracks require an accurate Data safety form and
  privacy policy:
  <https://support.google.com/googleplay/android-developer/answer/10787469?hl=en>
- Android 16 edge-to-edge and large-screen behavior must be tested; a correctly
  categorized game may retain its orientation treatment:
  <https://developer.android.com/about/versions/16/behavior-changes-16>

Unity 6000.5 exposes the API 36, AAB, ARM64, application-category, and signing
settings used by the read-only validator:
<https://docs.unity3d.com/6000.5/Documentation/ScriptReference/PlayerSettings.Android.html>.
The guard also requires `UserBuildSettings.DebugSymbols.level` to be `None`,
confirms the game category through the obsolete `androidIsGame` compatibility
alias, and rejects Unity Android library projects. The compatibility alias is
derived from `appCategory`; it is not an independent game-category gate:

- <https://docs.unity3d.com/6000.5/Documentation/ScriptReference/Android.UserBuildSettings.DebugSymbols-level.html>
- <https://docs.unity3d.com/6000.5/Documentation/ScriptReference/PlayerSettings.Android-appCategory.html>
- <https://docs.unity3d.com/6000.5/Documentation/Manual/android-library-project-import.html>

These requirements inform the production baseline, but production remains
blocked. Target API compliance alone is not permission to sign or publish.

## Assembly and trust boundary

| Layer | Assembly | Authority |
|---|---|---|
| Pure policy | `LegendsOfRus.AndroidBuildCore.Editor` | Boolean facts, stable reason codes, deterministic evaluator; Editor-only and engine-free |
| Unity bridge | `LegendsOfRus.AndroidBuild.Editor` | Read-only Unity settings adapter, strict policy loader, sanitized evidence, menus, scheduled-options guard, and BuildReport guard |
| Tests | `LegendsOfRus.AndroidBuild.Editor.Tests` | Isolated EditMode policy, denial, guard, and evidence fixtures |
| Existing runtime | unchanged | No dependency on any Android build assembly or config |

The policy file is outside `Resources` and therefore adds no player payload. It
uses an exact flat schema and is pinned to SHA-256
`95eec67ffb9ef8e45ad657524cbfcaa3c852258f4de2c7a54dc53ad18f96751c`.
Unknown, missing, duplicate, reordered, or otherwise changed bytes fail closed.

## Defined Development APK contract

The policy and guards define the only Development APK configuration that may be
enabled by a reviewed source follow-up. The current candidate does not allow it:
`ResolvedPackageLockApproved=false` forces a fail-closed
`ANDROID_PACKAGE_LOCK_UNAPPROVED` result before preflight or build. After the
package lock is reviewed, pinned, and explicitly approved in compiled source,
every row must be exact:

| Setting | Required value |
|---|---|
| Unity | `6000.5.9f1` |
| Mode | Development APK only |
| Active target | Android |
| Scheduled target context | Android group, subtarget `0` |
| Build profile | platform profile only; no active or persistent custom Build Profile |
| Scene | `Assets/Scenes/Bootstrap.unity` only |
| Package | `com.example.legendsofrus.v117debug` |
| Version | `0.0.0`, code `1` |
| Target API | exactly `36`; automatic/highest-installed is rejected |
| Minimum API | `26` through `36` |
| Backend / ABI | IL2CPP with ARM64 present |
| Manifest behavior | app category `game`; obsolete `androidIsGame` compatibility alias confirms the same value; Portrait |
| Signing | Unity default debug signing; custom keystore off |
| Packaging split | Split APK per CPU and Split Application Binary off |
| Native debug symbols | `None`; no symbols ZIP or embedded symbols |
| Build Options | `Development`, with only UI Build's optional `ShowBuiltPlayer`; use Build, not Build & Run; Compression Method Default |
| Android defines | persisted symbols and scheduled `extraScriptingDefines` both empty |
| AssetBundle manifest | empty |
| Output | `Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk` |

The output path is normalized, must remain inside the project at the exact
location, must end in `.apk`, and cannot traverse an existing reparse point.
The policy path text must match with exact case on every Editor platform. The
reparse walk prefers an exact filesystem entry, uses a unique case-folded
physical-entry fallback on every Editor host, and blocks ambiguous case-fold
matches. This closes default case-insensitive volumes, case-sensitive macOS
volumes, and case-folded network/Linux mounts.
This APK is only a local verification artifact.

## Global production stop

The boundary uses two public, Editor-only callbacks. A
`BuildPlayerProcessor` reads the scheduled `BuildPlayerOptions` before Player
compilation. It requires the exact Android target context, Bootstrap scene
array, output path, required `Development` option with only optional
`ShowBuiltPlayer`, empty `extraScriptingDefines`,
empty AssetBundle manifest, and no active or stored custom Build Profile. The
later `IPreprocessBuildWithReport` validates the BuildReport and read-only Unity
settings. Each callback requests `int.MaxValue` within its callback type and
applies to normal UI, command-line, and batch-mode Player builds. The candidate
contains no competing callback of either type. Any unexpected exception becomes
a constant fail-closed reason.

The following are unconditionally denied in Step 1, even if a synthetic policy
object attempts to enable them:

- non-Development/release APK;
- Android App Bundle;
- Gradle project export;
- split APK per CPU or Split Application Binary output;
- native debug-symbol level `SymbolTable` or `Full`, or any symbols package;
- custom signing;
- alternate scene, package identity, version, SDK, ABI, orientation, output,
  active or inactive persistent Build Profile, persisted scripting define, or
  per-build `extraScriptingDefines` value;
- Build & Run, patch, scripts-only, test-assembly, profiler, debugging,
  coverage, streamed-scene, compression, or any other additional Build Options
  flag;
- non-Android target context, alternate subtarget, empty/alternate scheduled
  scene array, or AssetBundle manifest;
- missing/changed policy, package manifest, Unity version, build settings,
  economy activation, commercial config, or commercial activation;
- Android plugin (including managed DLLs), `.androidlib` project,
  `AndroidManifest.xml` override, embedded package directory, native SDK,
  signing file, binary build artifact, response file, service configuration,
  or custom Gradle file under Assets.

`ProductionAndroidBuildCompiled` and `ResolvedPackageLockApproved` are private
constants set to `false`. The latter also blocks the defined Development APK
contract in this exact candidate until a reviewed package-lock follow-up. There
is no build coordinator, signing lease, credential source, provider integration,
or production bypass in this milestone.

## Credential handling

This step deliberately does not implement credential acquisition. It never
reads or logs the Unity keystore path, key alias, keystore password, or alias
password. It does not inspect environment variables or command-line arguments,
write Editor preferences, spawn a process, or access a network.

The only signing-state read is `PlayerSettings.Android.useCustomKeystore`.
When it is true, the build is denied. Any future signing workflow requires a
separate reviewed milestone with ephemeral credential installation and
restoration, secret-store integration, and retained security evidence.

## ProjectSettings boundary

The delivered source contains only the inherited
`ProjectSettings/ProjectVersion.txt` and
`ProjectSettings/EditorBuildSettings.asset`.
`ProjectSettings/ProjectSettings.asset` is absent, so no package identifier,
version, SDK, backend, ABI, orientation, or signing configuration is claimed by
the archive. `Packages/packages-lock.json` is also absent and its resolved graph
is not yet approved. Exactly Unity 6000.5.9f1 must generate/save
`ProjectSettings.asset` and resolve/save `packages-lock.json`; neither file may
be hand-authored or edited.

Run static and baseline verification on a separate clean extraction before
Unity import. The imported working copy will create `.meta` files and generated
roots such as `Library`, `Logs`, and `UserSettings`; those are execution state
outside the sealed 457-file source fingerprint. Do not run the sealed-source
verifier in that working copy. Return only the unedited
`ProjectSettings/ProjectSettings.asset` and `Packages/packages-lock.json` for a
reviewed source follow-up. That follow-up must inspect the lock for embedded,
local, or Git dependencies, pin its exact SHA-256 in the live guard and static
verifier, flip
`ResolvedPackageLockApproved` to `true`, and repin the verifier and evidence to
the resulting frozen candidate. Later Unity evidence is tied to the reviewed
source-archive SHA-256 plus those two explicitly retained Unity inputs, not to
literal post-import 457-file equality. Until then, preflight is intentionally
blocked regardless of manual Player settings. Do not build the current
candidate.

## Verification inventory

- strict loader tests for exact bytes, keys, duplicates, and false approvals;
- pure guard tests for the sole allow case and each independent blocker;
- scheduled-options tests proving Android per-build defines are denied without
  leaking their value, the target/scenes/options/output/manifest contract is
  exact, and non-Android builds remain untouched;
- synthetic attempts proving AAB, export, signing, and release remain blocked;
- sanitized-evidence tests proving no identity, output, credential, alias, or
  password value is serialized;
- Editor menu command:
  `Legends of Rus/Tests/Run v1.17 Step 1 Self-Tests`;
- static verifier:
  `node Tools/verify_v1_17_step01.mjs --source-only`.

Source attributes and Editor check sites are inventory, not execution results.

## Required package-lock follow-up and acceptance evidence

The current candidate has one permitted Unity action sequence:

1. Run static and baseline verification on a separate clean extraction before
   opening Unity.
2. Open a different working copy in exactly Unity 6000.5.9f1 and let
   import/compilation and Package Manager resolution finish. Do not run the
   sealed-source verifier in this generated working copy.
3. Save the Unity-generated `ProjectSettings/ProjectSettings.asset` and
   `Packages/packages-lock.json` without editing either file.
4. Return only those two files for review. Do not return generated `.meta` files
   or Unity working-copy roots. Do not run Android preflight or build an APK from
   the current candidate.

The reviewed source follow-up must verify the resolved dependency graph, pin the
exact package-lock SHA-256 in both the live and static boundaries, flip the
compiled package-lock approval gate, and repin its verifier and evidence. Only
on that resulting frozen candidate may the remaining acceptance evidence be
retained:

1. Unity 6000.5.9f1 import and compilation with zero errors.
2. Sanitized preflight output proving the reviewed generated files and exact
   configuration pass.
3. Project validator output.
4. v1.17 Step 1 Editor self-test output.
5. Complete EditMode and PlayMode NUnit XML with zero failures, skips, or
   inconclusive tests.
6. In a throwaway copy, retained fail-closed evidence for Android requests with
   a per-build define, empty/alternate scene array, extra Build Options flag,
   AssetBundle manifest, and inactive custom Build Profile.
7. Exact Development APK BuildReport and SHA-256.
8. Final merged manifest and packaged native-library inventory.
9. ARM64 and 16 KB page-size inspection.
10. Representative phone, tablet/foldable, safe-area, offline, suspend/resume,
    rotation/state-recovery, and Android back behavior.

Passing those checks authorizes only the local development verification
artifact. It does not authorize production identity, AAB, Play App Signing,
provider SDKs, economy/commercial activation, Data safety declarations, or
publishing.

## Deferred work

- production application identifier and permanent version sequence;
- Play Console application and Play App Signing enrollment;
- upload-key creation, protection, rotation, and certificate inventory;
- AAB generation and Play-generated APK inspection;
- symbol retention, crash reporting, and final native SDK inventory;
- 16 KB device certification and final merged-manifest review;
- privacy policy, Data safety, provider/permission, and legal approval;
- any production build or publishing authority.

The next development milestone is v1.17 Step 2 — Save Migration and Recovery
Foundation, after the Step 1 Unity verification checkpoint is retained.
