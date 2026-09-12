# Legends of Rus — BuildPreflightFix03

## Scope and status

This patch targets `LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL.zip` (recorded Unity version 6000.5.9f1). It corrects early build validation and diagnostic reporting. **It does not unblock or certify an Android APK build.** The inherited v1.17 package approval and other policy requirements remain in force. No Splash image, prefab, font, layout, runtime script, package version, or Player setting is changed.

## Findings in the uploaded archive

1. The supplied error log runs through `BuildPlayerWindow.BuildPlayerAndRun` and Addressables content building while Unity reports Play Mode. Stop the game; Pause is not Stop. Wait for Edit Mode and any import/compilation before a content build or Player build.
2. The existing `AndroidBuildPlayerOptionsGuard` accepts exactly `BuildOptions.Development`, or `Development | ShowBuiltPlayer`. `AutoRunPlayer` (Build And Run), profiler/debugging flags, compression flags and non-development builds are outside that project's current allowlist. This is a project-specific restriction, not a universal Unity restriction. The log does not record all flags used in that attempt.
3. `AndroidBuildStateReader` still has `private const bool ResolvedPackageLockApproved = false`. Merely generating a lock file does not approve it under this code.
4. The archived `Packages/manifest.json` hash is different from the reader's pinned hash. A blind one-line approval change would expose another policy failure rather than establish a reviewed build baseline.
5. The supplied FULL archive includes no `Packages/packages-lock.json` and no `ProjectSettings/ProjectSettings.asset`. Your running local project may have generated both. The archive cannot establish the state of those local files.
6. Both original final guards use maximum callback order. The observed Addressables errors occur before their rejection. The patch adds an early check but retains those final guards and their existing order.

## Apply

1. Stop Play Mode. Close Unity and back up the current project.
2. Merge this package's `Assets` folder into the current FidelityFix02 project. Replace matching `.cs` files. Do not replace the whole Assets folder and do not delete local `.meta` files. This patch supplies metadata only for new scripts (or byte-identical existing metadata where available in the archive).
3. Copy `ProjectDocs` into the project for the audit/diff, reopen Unity, and wait for import and compilation.
4. While in Edit Mode, run:
   `Legends of Rus > Android > Build Fix03 > Export Build Diagnostics`
5. Read the generated report at:
   `ProjectDocs/AndroidBuildFix03/AndroidBuildDiagnostics.json`

The export is read-only with respect to the project configuration. It writes only its local report. It reports policy booleans, known-file fingerprints and installed `com.unity.*` package names/versions/sources. It does not read credentials, keystore paths/aliases/passwords, private registry URLs, or local account paths into the report. It does not approve package pins or launch a build.

The report is a snapshot of current UI/settings, not a reconstruction of the prior failed attempt's options. New rejected attempts include their actual scheduled BuildOptions in the error, without printing paths or credentials.

## Expected outcome

- A Player build attempted during Play Mode is rejected by the early Android preflight with `ANDROID_EDITOR_PLAY_MODE_ACTIVE` before lower-priority content processors.
- Build And Run remains rejected. Correct the requested workflow to Development Build + ordinary Build; do not silently strip flags or suppress exceptions.
- After immediate option problems are corrected, the inherited package-approval issue can still report `ANDROID_PACKAGE_LOCK_UNAPPROVED`. This is expected, not a new permission change.
- The new report lists current checks together so configuration drift can be corrected from evidence rather than by serial trial-and-error builds.
- If Unity fails inside some other earlier callback, this patch cannot intercept that callback. Callback order and actual Addressables execution have not been tested here.
- A separately invoked Addressables content-only build is not a Player build and does not pass through this Player preflight. Always run content-only builds in Edit Mode as well.

Do not disable Addressables' Player-build integration merely to hide its failure. No cache deletion or package upgrade is part of this patch. If manual content verification is needed after Edit Mode is restored, use `Window > Asset Management > Addressables > Groups`, then `Build > New Build > Default Build Script`. That is a content build, not an APK build and not proof that the Android policy is satisfied.

## Follow-up evidence for the remaining build configuration

Provide the generated diagnostic JSON together with the actual local `Packages/manifest.json` and `Packages/packages-lock.json`. No images, Library folder, keystore, passwords, or new full project archive are needed for this review. Keep the Package Manager-generated lock file unchanged. Do not toggle `ResolvedPackageLockApproved`, production approval flags, or hash pins just to silence a failure.

## Files

Modified:
- `Assets/LegendsOfRus/Editor/AndroidBuild/Unity/AndroidBuildGuard.cs`
- `Assets/LegendsOfRus/Editor/AndroidBuild/Unity/AndroidBuildStateReader.cs`
- `Assets/LegendsOfRus/Editor/AndroidBuild/Core/AndroidBuildReasonCodes.cs`

Added:
- `Assets/LegendsOfRus/Editor/AndroidBuild/Unity/AndroidBuildEarlyPreflight.cs`
- `Assets/LegendsOfRus/Editor/AndroidBuild/Unity/AndroidBuildFix03Diagnostics.cs`
- `Assets/LegendsOfRus/Tests/AndroidBuildEditMode/AndroidBuildFix03EditModeTests.cs`

The existing Editor test assembly includes the new tests. There are 12 parameter-expanded new NUnit cases, covering state checks, early/final callback orders, disallowed flags, absence of Development, and preserved lock denial. They have been authored but not executed in Unity.

## Verification limits

28 portable source/resource checks passed. These check preserved constants, option comparisons, metadata and patch scope, among other properties. They are **not C# compilation, Unity Test Runner, Addressables build, Gradle build, APK installation or device verification**. No Unity Editor or C# compiler is installed in the available environment. The source changes must compile and be tested in your local Unity project.

All 708 pre-existing archive `.meta` files remain unchanged when this additive patch is merged; new script GUIDs are distinct. Existing permission/policy files and all six protected SHA-256 pins are unchanged. The original package-lock approval remains false. Review `source_changes.diff` and `StaticVerification.json` in `ProjectDocs/AndroidBuildFix03`.
