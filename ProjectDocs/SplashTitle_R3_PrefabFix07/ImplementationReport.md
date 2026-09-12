# Splash/Title R3 — PrefabFix07

## Scope and status

Repair of the ColorFix06 canonical-prefab revision mismatch. The package contains the corrected serialized prefab, not just a rebuild instruction. No artwork was regenerated, replaced, recolored, or rescaled at the pixel level in this repair. Unity compilation, asset import, Play Mode, screen capture, and Android builds have NOT been run in this environment.

## Evidence for the cause

ColorFix06 changed `SplashTitleView.CurrentRevision` from 4 to 5. Its patch did not include `Assets/LegendsOfRus/Resources/ProductionUI/Pages/SplashTitle.prefab`. The inherited prefab still serialized `Revision: 4`. Its existing `HasRequiredBindings` condition required both matching revision and non-null bindings. Therefore a fully referenced revision-4 prefab was rejected by revision-5 code and the runtime printed the same generic “missing or broken bindings” message used for missing objects.

The ColorFix06 prefab layout also remained at 110 x 110 icon rectangles until an Editor rebuild took place. Its authoritative layout JSON had the updated 126.5 x 126.5 rectangles. This repair synchronizes the shipped prefab to that existing JSON.

The log alone cannot prove why automatic repair did not complete on the user's machine. The old callback could be skipped while entering/in Play Mode and did not schedule a check upon returning to Edit Mode. This is addressed without writing scenes or prefabs during Play Mode.

## Implemented changes

1. **Actual serialized prefab delivery**: `SplashTitle.prefab` now stores `Revision: 5` and all ColorFix06 icon geometry. No new revision increment. Layout JSON schema remains 4; the JSON schema and view revision are different concepts.
2. **Reference preservation**: all 13 local control/text bindings, the typography configuration reference, the 10 reconstructed sprite GUIDs, hierarchy object IDs, and existing `.meta` files are preserved. No legacy screen image is introduced.
3. **Precise diagnostics**: the runtime reports resource-not-found, missing view component, outdated serialized revision, missing specific bindings, or a missing root RectTransform separately. A stale revision is no longer described merely as a missing prefab.
4. **Strict validation remains enabled**: `HasRequiredBindings` still requires `HasCurrentRevision && HasCompleteBindings`. This repair does not make that check return true unconditionally and does not re-enable screenshot fallbacks.
5. **Editor-only repair scheduling**: queued checks run after asset imports/domain reloads and after entering Edit Mode. A valid saved prefab is a no-op. The builder refuses work during Play Mode, compilation, importing, or Player builds. No automatic Play Mode cancellation, scene opening, or package modification is introduced.
6. **Save/read-back check**: a manual or necessary rebuild checks Unity's save result, synchronously imports the saved prefab, and verifies its actual saved revision and bindings before logging success.
7. **Validation menu**: `Legends of Rus > Production UI > Splash R3 > Validate PrefabFix07` checks the canonical wiring and serialized art/button/text rectangles against the authoritative layout JSON. This is a serialized-asset validator, not a runtime or exact-font approval.

## Preserved visual configuration

The seven non-character PNGs retain ColorFix06's already-applied 0.9 RGB multiplier. This repair does not apply it again. Ivan, Masha, the background, the Firebird image, the crest, button images and selector are all byte-identical to their respective ColorFix06 versions.

The bottom icon Images are already serialized at 126.5 x 126.5 reference units (110 x 1.15), with the outer group positions from ColorFix06: Settings x=54, Account x=364, Help x=675. The existing inward-spacing adjustment is preserved exactly as supplied; this repair does not reinterpret or further reduce it. Labels follow their existing button parents. Text strings, fonts, font size, optical alignment, shadows, and tint values are unchanged.

## Changed project files

- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/SplashTitle.prefab`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/SplashTitleView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionSplashTitleRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Editor/Step20SplashTitleR3PrefabRebuilder.cs`
- `Assets/LegendsOfRus/Scripts/Editor/SplashTitleR3Validation.cs`

The patch includes the matching existing `.meta` files byte-for-byte. It adds only documentation and a standard-library Python verifier outside `Assets`; no new packages or NUnit dependencies are required.

## Installation on the current ColorFix06 project

1. Stop Play Mode, close Unity, and back up the current project.
2. Merge the patch's `Assets` folder into the project and replace the matching files. Do not replace the whole Assets directory. Keep `.meta` files intact. The Tools and ProjectDocs additions are optional diagnostics/documentation.
3. Reopen with the recorded Unity version, 6000.5.9f1. Let scripts compile and imports finish before pressing Play.
4. Run `Legends of Rus > Production UI > Splash R3 > Validate PrefabFix07` in Edit Mode. The expected successful message starts with `Splash PrefabFix07 PASS: revision 5; bindings and serialized layout match.`
5. Open `Assets/Scenes/Bootstrap.unity`, then enter Play Mode. Use the 941 x 1672 Game view for the visual check.

A forced rebuild is normally unnecessary: the corrected prefab is in the patch. If validation says the prefab is still stale, stop Play Mode and use the existing `Build or Repair` command once, then validate again. If repair fails, the Console now identifies the actual save/import/binding reason. Do not lower the required revision, manually mark fonts verified, delete the art, or restore screenshot fallback layers.

The full-project archive is a separate reconstructed snapshot of the available FidelityFix02 project plus patches through ColorFix06 and this repair. Use it in a NEW folder. It does not contain local package-lock, Android signing/Player settings, or font assignments made only on the user's machine. The patch is preferable for retaining those local changes.

## Verification performed here

- The saved view revision matches the runtime constant (5).
- Five Button components, eight live Text components, and ten distinct reconstructed Image sprites are present.
- All local serialized file-ID references resolve inside the prefab; all 14 required view fields reference existing matching objects/assets.
- All art, button and text rectangles in the saved prefab match the ColorFix06 layout JSON, including text raster-density compensation.
- All 10 reconstructed PNGs and all 714 existing `.meta` files are unchanged from the merged ColorFix06 baseline.
- All original prefab object IDs and every GUID occurrence are preserved.
- Four negative controls confirm that the portable verifier detects an old revision, missing Start binding, old icon dimensions, and a missing background sprite reference.
- Package archive CRC checks are recorded separately after packaging.

These are static/source/serialized-artifact checks, not a Unity compilation or runtime result. The Python verifier can be run from the project root with:

```sh
python Tools/verify_splash_prefabfix07.py --project . --output ProjectDocs/SplashTitle_R3_PrefabFix07/LocalPortableVerification.json
```

## Remaining independent limitations

Exact typography identification and the existing Android package-approval/build restrictions are not modified by this repair. A font warning is separate from a missing or stale Splash prefab. No successful Android build or final reference match is claimed.

## API references used for the repair

- Unity 6.5 serialization rules: https://docs.unity3d.com/6000.5/Documentation/Manual/script-serialization-rules.html
- Unity 6.5 AssetPostprocessor.OnPostprocessAllAssets: https://docs.unity3d.com/6000.5/Documentation/ScriptReference/AssetPostprocessor.OnPostprocessAllAssets.html
- Unity 6.5 PrefabUtility.SaveAsPrefabAsset (success result): https://docs.unity3d.com/6000.5/Documentation/ScriptReference/PrefabUtility.SaveAsPrefabAsset.html
- Unity 6.5 EditorApplication.playModeStateChanged: https://docs.unity3d.com/6000.5/Documentation/ScriptReference/EditorApplication-playModeStateChanged.html
