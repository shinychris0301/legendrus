# SplashTitle R3 — WarningFix01

## Scope and honest status

This patch is for the already-installed `LegendsOfRus_SplashTitle_R3_DirectAssets_PATCH` implementation on top of the supplied VisualPage01R Unity project (recorded version 6000.5.9f1). It is not a standalone project and must not be installed on an older project without R3.

It corrects duplicate font diagnostics and distinguishes Editor memory observations from Player budget warnings. It does **not** identify or supply the exact font, declare visual fidelity complete, or prove reduced Android memory use. No font binaries are included.

## Installation

1. Stop Play Mode and close Unity. Back up the current project or commit it to version control.
2. Extract this ZIP and merge its `Assets` folder into the existing project's `Assets` folder. Overwrite the two matching `.cs` files, not the entire Assets directory. Do not delete existing `.meta` files. Keep `ProjectDocs` for the audit/diff.
3. Reopen the same project and allow compilation. No prefab rebuild, package upgrade, or source-art reimport is required by this patch.
4. Open `Legends of Rus > Production UI > Splash R3 > Select Typography Settings`.
5. Run Play Mode and use `Legends of Rus > Production UI > Splash R3 > Print Font and Memory Diagnostics` to print current font slots and the largest already-loaded textures.

The patch manifest contains old/new hashes. Review `SourceChanges.diff` first when these scripts have local edits since R3.

## Why the warnings occur

### Typography

The delivered configuration has `RegularFont` and `StartFont` set to `fileID: 0`, `ReferenceIdentityVerified: 0`, and empty `SourceEvidence`. No font binaries were found in the supplied baseline project or R3 patch archives. The fallback resolver therefore uses a local OS font; in the supplied log it is Liberation Serif. This is an unfinished requirement in the earlier delivery, not a Unity engine failure or a setup error by the user.

The original warning flag and OS-font cache belonged to a loaded ScriptableObject instance. Reloading that asset can reset the warning state and create another fallback instance. The two user traces show warning calls at startup and after Back navigation; the traces alone do not prove exactly when the asset was unloaded. This patch uses per-session status deduplication and a shared fallback cache so ordinary screen recreation does not repeat the same warning. A new Play session or script reload can correctly report the unresolved status again.

The generated image does not establish the identity of a source typeface. It may contain authored/generated lettering rather than lettering rendered from a recoverable font file. The logo remains the supplied PNG. A visual resemblance or glyph-coverage check is not proof of an exact font match.

### Memory

The thresholds in `Scripts/OptimizationCore/OptimizationPolicy.cs` are custom project development guardrails, not universal Unity limits:

| Metric | User sample, MiB | Warning, MiB | Critical, MiB | Result |
|---|---:|---:|---:|---|
| Allocated | 350 | 384 | 512 | Below warning |
| Reserved | 695 | 640 | 768 | Warning threshold crossed |
| Managed | 31 | 96 | 128 | Below warning |

The reserved value triggers the supplied Warning. Reserved memory includes memory held for current and future allocations. Allocated, reserved, and managed are not three independent quantities to add. The Editor process and an Android Player are different measurement environments. This single sample is neither proof of a leak nor proof that the Android budget is safe.

## Source changes

Modified:
- `Assets/LegendsOfRus/Scripts/Runtime/UI/SplashTitleTypography.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimeOptimizationService.cs`

Added:
- `Assets/LegendsOfRus/Scripts/Editor/SplashTitleWarningDiagnostics.cs` and its `.meta` file.

### Font behavior

- Reuses one development fallback font across normal Splash recreation.
- Emits the same unresolved-font status once per Play session instead of once per reloaded configuration instance.
- Resets session state at SubsystemRegistration, including sessions with domain reload disabled.
- Releases only the script-created OS font at session cleanup; never destroys bound imported fonts or the built-in font.
- Keeps the serialized font fields and existing font asset GUID unchanged.
- Checks required glyphs before using a bound font; inadequate fonts remain an explicit blocker.
- Provides a persistent Inspector status without automatically selecting fonts or asserting an exact match.
- Preserves the existing non-development build guard.

A font warning is still expected at first display while the exact-font requirement is unresolved. Turning off `AllowDevelopmentFallback` with no bound fonts can leave live labels without a usable font. Do not use that switch to silence the warning.

### Memory behavior

- Retains the same 384/640/96 MiB warning and 512/768/128 MiB critical thresholds.
- Retains bounded evidence recording and the existing resource-cleanup flow.
- Includes the exact metric/threshold responsible for a comparison warning.
- Logs ordinary Editor Warning-band observations as informative `EDITOR memory observation` messages, explicitly not an Android verdict.
- Keeps Editor Critical-band comparisons and Player Warning/Critical comparisons as warnings.
- Logs actual `Application.lowMemory` signals as warnings and still requests cleanup in every build.
- Does not force garbage collection, raise thresholds, disable evidence, resize art, or change texture compression.

This is a reporting correction, not evidence of a memory reduction.

## Resolving the outstanding font requirement

Use the existing Typography settings asset:
`Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleConfig/SplashTitleTypography.asset`.

When source font assets have actually been identified, import them locally into the project's existing font folder and assign the imported **Unity Font** objects to `RegularFont` and `StartFont`. This implementation uses `UnityEngine.UI.Text`, not TextMeshPro. A TMP font asset is not the expected field type.

Bind the appropriate family/styles, check Russian letters including Ё/ё and the supplied punctuation, and compare at the 941 × 1672 reference viewport. Record the real source family/styles and evidence. Set `ReferenceIdentityVerified` only when that identity is genuinely established. A substitute may be tested by binding it, but remains unverified and must not be falsely declared exact. When no source typeface is identifiable, completing typography requires an explicit new design approval rather than pretending a recovered original exists.

No automatic font downloads, licensing changes, or font redistribution are performed by this patch.

## Required Unity/device verification (not performed here)

After importing, verify compilation and run the real Bootstrap startup flow. Revisit Settings/Language/Help and return to Splash. The first unverified-font warning should be retained, but returning with unchanged font status should not repeat it. Start a new Play session and verify it reports the unresolved state again.

Run the new diagnostic command. Check assigned font assets and required glyph status. Treat texture sizes as Editor observations; the list includes Editor previews/internal textures and is not a device snapshot.

For memory acceptance, build an Android Development Player and attach the Profiler. Measure cold startup, repeated route transitions, and steady-state behavior on representative devices. Capture before/after memory snapshots when memory or object counts keep growing. Optimize the retained/oversized resources actually found there, preserving alpha and checking the reference after any compression change.

The R3 import exception already requests Read/Write off and mipmaps off for this art, with full-resolution uncompressed imports for review. Verify the actual imported settings locally; merely repeating an already-disabled setting is not a demonstrated saving. This patch deliberately leaves importer policy unchanged. The older v1.17 global texture audit predates the R3 exception and can report that exception as a policy violation; this patch does not claim that old audit passes.

## Verification performed here

Static archive/source inspection, basic delimiter/preprocessor balance, preservation checks, and arithmetic using the source thresholds were performed. Results are in `ProjectDocs/SplashTitle_R3_WarningFix01/StaticVerification.json`.

There is no Unity executable or C# compiler in this workspace. **No Unity compilation, Play Mode run, device memory capture, font-match test, or visual comparison was performed for this patch.** Static checks are not compilation or runtime evidence.

## Official documentation consulted

- Unity OS-font resolution: https://docs.unity3d.com/6000.5/Documentation/ScriptReference/Font.CreateDynamicFontFromOSFont.html
- Unity allocated memory: https://docs.unity3d.com/6000.5/Documentation/ScriptReference/Profiling.Profiler.GetTotalAllocatedMemoryLong.html
- Unity reserved memory: https://docs.unity3d.com/6000.5/Documentation/ScriptReference/Profiling.Profiler.GetTotalReservedMemoryLong.html
- Unity memory categories: https://docs.unity3d.com/6000.5/Documentation/Manual/ProfilerMemory.html
- Profiling target devices: https://docs.unity3d.com/6000.5/Documentation/Manual/profiling-target-device.html
- Profiler build settings: https://docs.unity3d.com/6000.5/Documentation/Manual/profiler-build-settings-reference.html
- Sprite Read/Write and mipmaps: https://docs.unity3d.com/6000.5/Documentation/Manual/texture-type-sprite.html
- Runtime session initialization: https://docs.unity3d.com/6000.5/Documentation/ScriptReference/RuntimeInitializeLoadType.SubsystemRegistration.html
