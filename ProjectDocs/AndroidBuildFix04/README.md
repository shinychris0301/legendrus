# Legends of Rus — BuildOptionsFix04

## Status and scope

This is a cumulative Editor-source correction for the supplied **SplashTitle R3 FidelityFix02** project and the **BuildPreflightFix03** patch. It fixes build-option validation, diagnostic formatting, and provides an explicit Development APK build-only command. It is **not an APK, not a successfully tested Android build, and not package-lock approval**.

There are no changes to Splash artwork, prefabs, fonts, runtime navigation, package versions, application identity, Android API levels, keystores or production permissions. Fix03 prerequisites are included. The original early Play Mode check remains.

## What the new log establishes

The submitted request reports `Development=False`, includes `AutoRunPlayer`, and also includes `CompressWithLz4` and `StrictMode`. The current project is configured around a Development APK-only workflow. A non-development auto-run request does not meet that intent.

The previous guard also used an unnecessarily exact numerical comparison: only `Development`, or `Development | ShowBuiltPlayer`, passed. That is why simply enabling Development could still fail with ordinary LZ4 or StrictMode options present. The previous blanket advice to turn every additional flag off was too restrictive.

Several additional, legacy-looking enum names appear in the submitted diagnostic. That diagnostic concatenated the enum's default string representation. This patch does **not** assume that each printed name corresponds to a distinct enabled nonzero flag. It prints numerical masks, identifies active individual nonzero bits, and exports the enum definitions from the user's installed Editor. Zero-valued aliases and composite aliases cannot appear as enabled individual switches in the new formatter. No obsolete flag is directly referenced or assigned, and no unknown nonzero flag is silently admitted. Do not change the IL2CPP scripting backend merely because `Il2CPP` appears in that old formatted message; the existing project checks the actual backend separately.

## Implemented validation

`AndroidDevelopmentBuildOptions.Validate` now:

- Requires the Development bit and reports `ANDROID_DEVELOPMENT_BUILD_REQUIRED` when absent.
- Permits `ShowBuiltPlayer`, `CompressWithLz4`, `CompressWithLz4HC` and `StrictMode` alongside Development.
- Continues to reject AutoRunPlayer and other non-permitted bits for this build-only workflow.
- Does not modify a scheduled request, clear flags behind the caller's back, or convert a release request into a development request.

This is an intentional, narrow revision of the old flag allowlist, not a claim that Build And Run, debugging, or profiling are generally unsafe in Unity. It does not change the separate production/signing/package policies. StrictMode remains an error-checking option; LZ4/LZ4HC are Player-data compression choices, not image-art edits.

The dedicated command creates exactly this request:

```csharp
BuildOptions.Development | BuildOptions.CompressWithLz4 | BuildOptions.StrictMode
```

It takes the Bootstrap scene and output filename from the existing validated policy. It checks scheduled options and policy readiness before any build or output-directory creation, asks about replacing a prior APK, offers Unity's normal scene-save prompt, and rechecks after that prompt. It uses `BuildPipeline.BuildPlayer` so Addressables and existing final guards remain part of the pipeline. There is no auto-run, installation, automatic platform switch, delayed-call build retry, or global build-menu override. UI-facing development/debug flags are temporarily aligned during that explicit build and restored afterward.

Success is logged only when Unity reports `BuildResult.Succeeded` and the expected APK exists. That is still not device-test evidence. When readiness is blocked, the command exports diagnostics and throws; it does not pretend an APK was built.

## Apply

1. Stop Play Mode, close Unity, and back up the current project.
2. Merge this ZIP's `Assets` folder into the existing FidelityFix02/Fix03 project and replace matching source files. Do not replace the whole Assets directory. Preserve existing `.meta` files; the patch only supplies unchanged prerequisite metadata and metadata for its new files.
3. Copy `ProjectDocs` for the audit. Open the project in its recorded Unity version, **6000.5.9f1**, and wait for compilation/import.
4. With Android active, run:

   **Legends of Rus > Android > Build Fix04 > Prepare Development Options and Export Diagnostics**

This explicit preparation command turns Development ON and turns Script Debugging, Autoconnect Profiler, Scripts Only and Deep Profiling OFF. It does not start a build. It leaves compression, scenes, application identity, API levels, signing, package approval and profile assets unchanged. Under the inherited no-custom-profile policy, it refuses to change options when a custom profile is present and exports the reason instead; it does not delete profiles.

The diagnostic file remains at the established location:

```text
ProjectDocs/AndroidBuildFix03/AndroidBuildDiagnostics.json
```

Its schema/revision now identifies **BuildOptionsFix04**. It includes all existing policy booleans and protected-file fingerprints, the last actual Android scheduled-option snapshot observed since this patch was loaded (when available), and installed BuildOptions enum names with their numerical values. A snapshot is not invented from a previous log or the current UI.

Once the entire readiness report passes, use:

**Legends of Rus > Android > Build Fix04 > Build Development APK (No Run)**

Do not select Unity's separate Build And Run action for this build-only contract. For a manual build, enable Development and use ordinary Build; Default, LZ4 or LZ4HC compression and StrictMode are no longer rejected by this option gate. All other policy checks still apply.

## Remaining known blocker — not newly introduced

The supplied code still contains `ResolvedPackageLockApproved = false` in `AndroidBuildStateReader.cs`, and that reader remains byte-for-byte unchanged relative to Fix03. Its six protected SHA-256 pins also remain unchanged. The existing policy loader, policy JSON, production-compiled flag and release/custom-signing restrictions are not altered.

Therefore **this patch alone does not make the supplied archive Android-build-ready**. An unchanged archive will still fail the independent package-approval check, or an earlier unmet prerequisite. The prior audit also found manifest/pin mismatch. The user's actual current `Packages/manifest.json` and Unity-generated `Packages/packages-lock.json`, together with the exported diagnostic JSON, are still required to reconcile that baseline. Local files may have been generated since the archive was uploaded. No lock file is fabricated or auto-approved here.

Do not repeatedly attempt APK builds while readiness reports blocked. Do not edit Library/PackageCache or delete Addressables to remove a policy error.

## Files

New Fix04 files:

```text
Assets/LegendsOfRus/Editor/AndroidBuild/Unity/AndroidDevelopmentBuildOptions.cs
Assets/LegendsOfRus/Editor/AndroidBuild/Unity/AndroidDevelopmentBuildCommand.cs
Assets/LegendsOfRus/Tests/AndroidBuildEditMode/AndroidBuildFix04EditModeTests.cs
```

Modified from Fix03:

```text
Assets/LegendsOfRus/Editor/AndroidBuild/Unity/AndroidBuildGuard.cs
Assets/LegendsOfRus/Editor/AndroidBuild/Unity/AndroidBuildFix03Diagnostics.cs
Assets/LegendsOfRus/Editor/AndroidBuild/Unity/AndroidBuildMenus.cs
Assets/LegendsOfRus/Editor/AndroidBuild/Core/AndroidBuildReasonCodes.cs
Assets/LegendsOfRus/Tests/AndroidBuildEditMode/AndroidBuildFix03EditModeTests.cs
```

The Fix03 early preflight and StateReader are included unchanged as prerequisites. Original source metadata that is absent from the archived files is deliberately not invented/replaced; retain the local project's existing metadata. Review `source_changes.diff` for the precise change from Fix03.

## Verification performed here

**46 source/archive/model checks passed.** These include controlled mask checks, preservation of existing restrictions and metadata, lexical delimiter checks (not a C# parser), and an independent Python flag model covering 1,024 combinations using explicitly synthetic bit values. The model does not execute the C# implementation or establish the installed Unity enum's numeric values.

**27 new parameter-expanded Unity Editor test cases are supplied**, including an additional loop over optional-flag combinations. They cover missing Development, forbidden AutoRun, accepted compression/StrictMode, request construction, zero/composite/duplicate alias formatting, unknown high bits and the preserved package-lock requirement. The old Fix03 LZ4-rejection test was updated for the deliberate allowlist change. These Unity tests have **not** been run here.

All **711 pre-existing Assets metadata files in the full-project + Fix03 union** are preserved; three new script metadata files have distinct GUIDs. The patch contains no runtime image/font/prefab/scene or package/settings changes. ZIP integrity is checked during packaging.

**Not performed:** C# compilation, Unity Test Runner, Play Mode execution, actual Addressables build, Android/Gradle build, APK installation, or device testing. No Unity Editor or C# compiler was found in the accessible environment. A local compile and Unity test run are still required.

## Official API references consulted

The sources below support API behavior, not claims that this project compiled or that its package baseline was reviewed.

- Unity 6.5 BuildOptions: `https://docs.unity3d.com/6000.5/Documentation/ScriptReference/BuildOptions.html`
- Development: `https://docs.unity3d.com/6000.5/Documentation/ScriptReference/BuildOptions.Development.html`
- AutoRunPlayer: `https://docs.unity3d.com/6000.5/Documentation/ScriptReference/BuildOptions.AutoRunPlayer.html`
- StrictMode: `https://docs.unity3d.com/6000.5/Documentation/ScriptReference/BuildOptions.StrictMode.html`
- LZ4: `https://docs.unity3d.com/6000.5/Documentation/ScriptReference/BuildOptions.CompressWithLz4.html`
- BuildPipeline.BuildPlayer: `https://docs.unity3d.com/6000.5/Documentation/ScriptReference/BuildPipeline.BuildPlayer.html`
- Editor scene-save prompt: `https://docs.unity3d.com/6000.5/Documentation/ScriptReference/SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo.html`
- Microsoft enum zero-mask behavior: `https://learn.microsoft.com/en-us/dotnet/api/system.enum.hasflag?view=net-9.0`

The exact numerical definitions in the user's installed Unity Editor are intentionally collected locally rather than assumed from legacy enum-name strings.
