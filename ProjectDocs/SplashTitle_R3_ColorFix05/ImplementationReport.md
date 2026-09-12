# Splash/Title R3 — ColorFix05 implementation report

## Scope and result

Nine existing foreground PNGs have been modified in the actual Unity resource
folder. This is an asset-only color correction on top of FidelityFix02, not a new
scene, an alternate prefab, a checklist-only delivery, or a screenshot overlay.

The complete distribution is assembled from the supplied FidelityFix02 full project
plus the previously delivered cumulative BuildOptionsFix04 patch. The ColorFix05
patch itself contains no C# changes, no .meta files, no new Unity package dependencies,
and no changes to project settings. It can be merged into the current FidelityFix02
project with or without that earlier build patch.

Only the nine PNGs listed below differ under Assets from that merged baseline.
The active prefab and its builder still point to the same GUIDs and paths, so both
the serialized screen and a deliberate prefab rebuild use the revised artwork.
There is no additional Image tint, shader grade, global Gamma/Linear switch,
camera overlay, or post-processing operation in this revision.

## Color adjustments

These are ADDITIONAL corrections relative to the already-calibrated FidelityFix02
textures, not relative to the first generated images. The adjustments are a
controlled review candidate responding to the user's latest Unity capture. They
are not the output of a colorimeter or an exact pixel-match optimization.

| Element | Additional exposure (stops) | Additional chroma multiplier | Measured average sRGB-luma decrease |
|---|---:|---:|---:|
| Title / logo | -0.24 | 0.94 | 7.24% |
| Ivan | -0.18 | 0.96 | 5.72% |
| Masha | -0.18 | 0.96 | 5.40% |
| Firebird | -0.10 | 0.97 | 2.67% |
| Start button artwork | -0.27 | 0.93 | 7.67% |
| Language selector artwork | -0.20 | 0.95 | 6.63% |
| Settings artwork | -0.20 | 0.95 | 6.26% |
| Account artwork | -0.20 | 0.95 | 6.22% |
| Help artwork | -0.20 | 0.95 | 6.40% |

The final column is computed on the actual source image pixels, weighted by alpha.
It is NOT the percentage brightness change of a whole Unity frame or a physical
display measurement. Perceptual appearance also depends on compositing and local
contrast. Both the pre/post statistics and exact parameters are in ColorCalibration.json.

Processing: decode RGB from sRGB to linear light; compute Rec.709-weighted luminance
Y = .2126 R + .7152 G + .0722 B; mix chroma toward Y; apply the specified exposure;
encode back to sRGB and round to 8-bit. Alpha and all RGB bytes in zero-alpha pixels
remain unchanged. No geometry, resampling, sharpening, recoloring by hue rotation,
or new reconstruction was applied to the source PNGs.

The existing sprite import settings remain sRGB, input alpha, Alpha Is Transparency,
no Read/Write, no mipmaps, uncompressed review textures. These settings have not
been toggled to artificially darken the rendering. Ten serialized sprite Images
retain white color and no custom material; existing button feedback has no color
components above 1.0.

## Files actually modified in the color revision

- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/legends_of_rus_ornate_crest.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/slavic_fantasy_prince_warrior_portrait.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/regal_slavic_princess_in_traditional_finery.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/majestic_fiery_phoenix_in_flight.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/ornate_emerald_fantasy_ui_button.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/ornate_gold_navy_language_selector.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/ornate_gold_gear_settings_emblem.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/ornate_gold_user_medallion.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/ornate_gold_question_mark_button.png`

No text or label PNGs were introduced. Start, language, version, bottom labels,
slogan, and legal wording remain existing live text. The Start label's weight,
geometry correction, raster-density settings and current font configuration are
unchanged.

## Background and project state preserved

The environment file remains:
`Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/golden_fortress_at_winter_sunrise.png`

Its SHA-256 is `fd49e708207129ad8ffe6590bd32ae2ceb0d8ba6016a5e58858243de4390f89b`,
identical to the FidelityFix02 file. Its earlier localized foliage repair is
retained; this revision makes no additional background or foliage changes.

Canonical prefab:
`Assets/LegendsOfRus/Resources/ProductionUI/Pages/SplashTitle.prefab`

Startup remains Bootstrap -> RuntimeBootstrap -> ScreenRouter ->
ProductionMetaPageRuntime -> ProductionSplashTitleRuntime -> canonical SplashTitle.
The five button bindings, responsive layout, footer gradient, live text, application
version source, and localization code are preserved byte-for-byte in this color pass.
The layout schema and SplashTitleView revision stay at 4 because no hierarchy or
serialized binding change was needed. ColorFix05 is the ART revision, not a new
layout schema.

All 714 existing .meta files from the merged baseline are
byte-for-byte unchanged. There are no new files under Assets. Evidence and tooling
are outside Assets and do not become runtime UI dependencies.

## Open or apply

For the current working local project: close Unity, back it up, and merge the
ColorFix05 PATCH into that project. Overwrite the matching nine PNGs, but preserve
local .meta files. The patch intentionally contains no .meta replacements, prefab,
font configuration, Player settings or build scripts. It therefore does not reset
local Android settings or locally assigned fonts. Do not delete/recreate the sprites.

For a separate copy: extract ColorFix05 FULL into a new folder and open the folder
containing Assets, Packages and ProjectSettings using the project's recorded Unity
6000.5.9f1. The full archive retains the settings limitations of the supplied baseline;
it is not a copy of locally generated settings that have not been uploaded.

Open `Assets/Scenes/Bootstrap.unity` and use the existing 941 x 1672 Game view for
the first comparison. No manual prefab rebuild is required. Do not apply another
RGB multiplier to the Images; the correction is already in the texture files.

The existing menu `Legends of Rus > Production UI > Splash R3 > Capture Current Runtime Screen`
remains available in Play Mode. It still uses its established FidelityFix02 evidence
folder; no capture code was changed solely to rename an art revision.

## Verification performed

76 file/pixel/source checks passed, including unchanged
alpha and hidden RGB, identical dimensions, same sprite GUIDs referenced by the
canonical prefab, unchanged background, unchanged non-art project files and
reproduction of the output textures from fixed original sources.

The portable verification can be run without Unity or external dependencies:
`python Tools/verify_splash_colorfix05.py`

For explicit reproducibility only, `--rebuild` reconstructs from the preserved
originals under ProjectDocs/SplashTitle_R3_FidelityFix02/Before, then reapplies the
two fixed documented grades. It requires numpy and Pillow. It never grades the
current PNGs a second time. This operation was tested locally and reproduced all
nine output hashes. Do not run the old FidelityFix02 art-rebuild command after
applying ColorFix05: that historical tool intentionally reconstructs the earlier art.

## Evidence and limitations

BeforeAfter_OfflineComposite_NOT_UNITY.jpg is a Pillow composition of the actual
before/after PNGs with the same geometry on both sides. Its labels explicitly state
that it is NOT Unity. Text in that preview is an approximation using Liberation Serif;
it is neither a new font assignment nor proof of exact typeface identity. No font
binaries are included in either archive.

AlphaInspection contains black/white/magenta proofs for all nine updated sprites.
The supplied Unity screenshot is retained unchanged and marked BEFORE this revision.
No image in this package is represented as a new ColorFix05 Unity runtime capture.

No Unity executable is available in this environment. No Unity compile, Play Mode
interaction test, Addressables build, Android APK build, or new device screenshot was
performed. Exact reference color matching is not certified by these source checks.
A fresh Unity capture is required for final visual approval.

The exact-font identification and the prior Android package-approval restrictions
are unchanged. In particular, the inherited ResolvedPackageLockApproved flag remains
false; the full package includes the already-delivered Fix04 diagnostics but this
color pass does NOT unblock/certify an APK build. Do not toggle approvals to suppress
unresolved diagnostics.

## Technical references

Unity sprite input-alpha/import behavior:
https://docs.unity3d.com/6000.5/Documentation/Manual/texture-type-sprite.html

Unity sRGB color texture interpretation:
https://docs.unity3d.com/6000.5/Documentation/ScriptReference/TextureImporter-sRGBTexture.html

These references describe Unity settings, not the execution or verification status
of this project.
