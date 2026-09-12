# Legends of Rus — v1.17 Step 1

## Android Build Configuration and Signing Workflow Foundation

Status: `IMPLEMENTED_IN_SOURCE_PENDING_UNITY_ANDROID_CONFIGURATION_AND_VERIFICATION`

Android candidate status:
`DEVELOPMENT_APK_GUARD_READY_PACKAGE_LOCK_APPROVAL_PENDING_PRODUCTION_BLOCKED`

This milestone adds an Editor-only, credential-free readiness contract. The
current source deliberately keeps the compiled
`ResolvedPackageLockApproved` gate set to `false`, so this exact candidate
cannot pass Android preflight or build an APK. It does not create a production
Android build path. **Do not build the current candidate.**

### Shipped boundary

- One SHA-256-pinned, exact-key policy defines the sole local Development APK
  contract that a later reviewed follow-up may enable.
- One global `BuildPlayerProcessor` inspects scheduled Player options before
  compilation, and one global `IPreprocessBuildWithReport` guard validates the
  later BuildReport and Unity settings. Both request `int.MaxValue` callback
  order within their callback type.
- The defined follow-up configuration uses Unity 6000.5.9f1, Bootstrap only, the
  nonproduction package `com.example.legendsofrus.v117debug`, version `0.0.0`
  code `1`, target API 36, a supported minimum API, IL2CPP, ARM64, game
  category with the obsolete `androidIsGame` compatibility alias confirming the
  same category, Portrait, native debug symbols set to `None`, no active or
  stored custom Build Profile, no persisted or
  per-build Android scripting define, exact scheduled Android target/group,
  subtarget `0`, Bootstrap scene array, `BuildOptions.Development` with only
  Unity UI's optional `ShowBuiltPlayer`, no AssetBundle manifest, no split
  APK/binary mode, no Android library project or
  manifest override, and the exact verification output path.
- Release APK, AAB, Gradle export, custom signing, native symbols package, a
  changed commercial boundary, or any other output is denied before Unity
  builds.
- Non-Android builds pass through unchanged.

The private production switch is `false`. All application-identity, version,
SDK, Play App Signing, native-library, 16 KB page, Data safety, and production
approval fields are also `false`.

### Credential and mutation boundary

No code in this step reads a keystore path, alias, password, environment
variable, or command-line credential. Only Unity's non-sensitive
`useCustomKeystore` boolean is inspected, and `true` is rejected. No keystore,
certificate, Android binary, provider SDK, `.androidlib` project, embedded
package, managed plugin, Gradle template, custom manifest, or service
configuration is bundled.

The workflow has no `BuildPipeline.BuildPlayer` call and no PlayerSettings
setter. The menus validate, show the manual checklist, or open Unity settings;
they do not change a setting or start a build.

### Generate the Unity-owned checkpoint files

1. Open the project and let Unity finish importing it.
2. In exactly Unity 6000.5.9f1, let Package Manager resolve dependencies and
   save the project so Unity generates/saves both
   `ProjectSettings/ProjectSettings.asset` and
   `Packages/packages-lock.json`. Do not hand-author or edit either file.
3. Return only those two generated files, unedited, for a reviewed source
   follow-up. Do not return generated `.meta` files or Unity working-copy roots.
   Do not run the Android readiness preflight and do not start an APK build from
   this candidate; `ResolvedPackageLockApproved=false` intentionally blocks
   them.

That reviewed follow-up must inspect the resolved lock for embedded, local, or
Git dependencies, pin its exact SHA-256 in the live guard and static boundary,
flip the compiled package-lock approval gate, and repin the verifier and
evidence to the resulting frozen source bytes.

Only after receiving that follow-up candidate may verification continue:

1. Use `Legends of Rus/Android/v1.17 Step 1/Show Manual Configuration Checklist`.
2. Configure the exact Development APK values shown by the checklist.
3. Run `Legends of Rus/Android/v1.17 Step 1/Validate Development APK Readiness`.
4. Run `Legends of Rus/Validate Commercial Vertical Slice`.
5. Run `Legends of Rus/Tests/Run v1.17 Step 1 Self-Tests`.
6. Run all EditMode and PlayMode tests and retain both XML files.
7. Only after those pass, use **Build** (not Build & Run) with Development Build
   on and Compression Method set to Default. Unity may add `ShowBuiltPlayer` for
   normal UI Build; no other Build Options flag is allowed. Retain the Unity
   report and representative-device results.

The exact output contract for that reviewed follow-up is:

`Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk`

It uses Unity's default debug signing and is not a store or production
candidate.

### Source verification

Run static and baseline verification from a separate clean extraction before
opening it in Unity. Unity import creates `.meta` files and generated roots such
as `Library`, `Logs`, and `UserSettings`; those are execution state outside the
sealed 457-file source fingerprint. Do not run this sealed-source verifier from
the imported Unity working copy. Later Unity evidence is tied to the reviewed
source-archive SHA-256 plus the explicitly retained unedited
`ProjectSettings.asset` and `packages-lock.json`, not to literal post-import
457-file equality.

From the clean extraction's project root:

```text
node Tools/verify_v1_17_step01.mjs --source-only
```

Release-boundary verification additionally requires the immutable v1.16 Step 2
project:

```text
node Tools/verify_v1_17_step01.mjs --baseline <v1.16-step02-project>
```

Static discovery counts and source self-test sites are inventory only. They do
not prove Unity compiled or executed the candidate.

Detailed architecture, current Play requirements, and retained-evidence rules
are in
`Assets/LegendsOfRus/Documentation/V1_17_STEP01_ANDROID_BUILD_CONFIGURATION_SIGNING_FOUNDATION.md`.
