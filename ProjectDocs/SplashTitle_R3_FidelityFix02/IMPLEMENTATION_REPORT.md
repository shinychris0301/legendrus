# Implementation report — Splash/Title R3 FidelityFix02

## Delivered work and ownership

The revision modifies the existing Unity project's active Splash resources and
canonical prefab, not a new demonstration scene. All original GUIDs are preserved.
The runtime remains Bootstrap -> RuntimeBootstrap -> ScreenRouter ->
ProductionMetaPageRuntime -> ProductionSplashTitleRuntime -> the canonical
Resources/ProductionUI/Pages/SplashTitle prefab.

Internal visual revision is now 4; the development milestone remains Splash R3.
Both the serialized prefab and its owning builder use layout schema 4. A stale
revision-3 prefab can therefore be repaired in Edit Mode without allowing the
builder to restore the previous uncorrected text layout.

## Distinguishing causes from observations

The supplied screenshot showed a visual difference. Inspection confirms that the
R3 images were used with white Image tint, that text used regular FontStyle,
small labels had Shadow components, and selected button colour could exceed 1.0.
The project supplied here does not contain the user's local generated
ProjectSettings.asset, so no assertion is made about the user's selected global
Gamma/Linear colour setting. That setting is not changed in this revision.

The new foreground grade is a conservative, explicit review candidate, not the
result of a colourimeter or a claimed pixel-exact optimization. Brightness and
saturation were modified in the PNGs themselves. Alpha, source dimensions and
placement are retained; no duplicate runtime colour grade is applied.

The environment repair uses clean sky pixels from another unoccupied area of the
same source environment, blended only inside the documented finite mask. This is
an environment edit, not a screenshot crop used as an independent button/character.

## Text implementation

Start: effective 80-unit bold text, rendered at a 160-unit local glyph request with
a half-scale label. Language and other labels likewise use 2x local density. Text
rectangles are doubled only locally, then compensated by that half scale. Existing
button rectangles and the 941x1672 composition are not doubled.

SplashTitleTextGeometry processes generated glyph quads before the optional Start
shadow. It skips degenerate whitespace quads; translates the entire line to center
its visible geometry; then applies a single screen-pixel-aligned translation.
It leaves alpha and glyph shape untouched. This implementation is scoped to these
single-line labels and intentionally does not treat arbitrary multiline text as a
single optically centered word.

No claim is made that a font family mismatch alone explains blur. Actual viewport
resolution, preview resampling, glyph density, shadow treatment and placement all
need inspection in a real capture. The exact typeface requirement remains open.

## Existing behaviour preserved

Five native Buttons and ScreenRouter navigation bindings remain unchanged:
Start -> Loading -> Home; Language -> Language Select; Settings -> Settings;
Account -> Login; Help -> Tutorial Overlay. Application.version remains live.
Existing locale state, service initialization, safe-area policy, button press
feedback and WarningFix01 diagnostics remain in the full project.

No extra Canvas, EventSystem, screenshot hotspot, font download, font file, shader
or package dependency is introduced.

## Changed files
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/SplashTitle.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleConfig/SplashTitleLayout.json`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/golden_fortress_at_winter_sunrise.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/legends_of_rus_ornate_crest.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/majestic_fiery_phoenix_in_flight.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/ornate_emerald_fantasy_ui_button.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/ornate_gold_gear_settings_emblem.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/ornate_gold_navy_language_selector.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/ornate_gold_question_mark_button.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/ornate_gold_user_medallion.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/regal_slavic_princess_in_traditional_finery.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/slavic_fantasy_prince_warrior_portrait.png`
- `Assets/LegendsOfRus/Scripts/Editor/LockedDesignImporter.cs`
- `Assets/LegendsOfRus/Scripts/Editor/SplashTitleR3Validation.cs`
- `Assets/LegendsOfRus/Scripts/Editor/Step20SplashTitleR3PrefabRebuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/SplashTitleView.cs`
- `Tools/verify_splash_r3.py`

## New implementation files

- `Assets/LegendsOfRus/Scripts/Runtime/UI/SplashTitleTextGeometry.cs` and its .meta
- `Assets/LegendsOfRus/Scripts/Editor/SplashTitleFidelityFix02Validation.cs` and its .meta
- `Tools/rebuild_splash_fidelityfix02_art.py`
- This revision's readme, audit data, source diff and labelled offline inspections.

## Verification boundary

Portable asset/source checks pass. No Unity compilation, Play Mode execution,
fresh Unity rendering or device validation was performed in this environment.
The included Editor validation command and capture command run locally in Unity.
They are not claimed to have already been executed here.

The generated presentation image in the conversation is excluded from this
implementation. Its apparent font-license/identity labels and runtime/test badges
must not be treated as evidence.

## Technical documentation consulted

- https://docs.unity3d.com/2018.1/Documentation/ScriptReference/UI.Text-pixelsPerUnit.html
- https://docs.unity.cn/2018.2/Documentation/ScriptReference/UI.Text-alignByGeometry.html
- https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-CanvasScaler.html
- https://docs.unity3d.com/6000.5/Documentation/ScriptReference/Canvas-pixelPerfect.html

These sources describe Unity behaviour, not the verification status of this project.
