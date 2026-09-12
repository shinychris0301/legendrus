# Splash/Title R3 — Direct Generated Asset Integration

## Status

Actual source and canonical prefab changes are included. All ten generated PNGs are used directly, byte-for-byte. No screenshot is used as runtime UI.

**Not signed off as an exact-reference or exact-font implementation.** Unity compilation/Play Mode were unavailable in the delivery environment. The reference font is not identified in the supplied project or the reference PNG metadata. Generated character/logo/control designs are close but not pixel-identical to the original reference.

## Install

Use the full project as a separate working copy, or merge the PATCH archive into a backed-up copy of the supplied `LegendsOfRus_Unity_v1_20_VisualPage01R_SplashTitle_ReferenceAccurate_FULL` project. The patch includes the preceding SourceFix01 routing changes. It is not a blind overlay for a different newer R3/FIX local branch.

Close Unity before merging the patch. Keep all `.meta` files. Open the existing project with its recorded editor version, **6000.5.9f1**. No package upgrade, input-backend change, or scene replacement is required by this patch.

The prefab is already serialized in the archive. After import, choose **Legends of Rus > Production UI > Splash R3 > Validate Serialized Wiring**. Only use **Build or Repair** when the prefab needs repair or deliberate layout regeneration; that command reapplies the included JSON layout, so back up intentional prefab edits first.

Open **Assets/Scenes/Bootstrap.unity** and press Play. The existing RuntimeBootstrap → ScreenRouter → SplashTitle flow should instantiate `ProductionSplashTitle` from `Resources/ProductionUI/Pages/SplashTitle.prefab`. This path is statically verified; it has not been exercised in Unity here.

## Typography — important

The supplied logo PNG retains its supplied gold lettering unchanged. It is not replaced with a generic text object.

The remaining labels are live `UnityEngine.UI.Text` objects. Version is `"v" + Application.version`; the language uses the existing `DemoFeatureState.LanguageCode`. The static preview uses the reference's v1.20.0 sample; the actual application may show a different version when its Player Settings say so.

There are **no font binaries in this delivery**, and none were present in the supplied project. The font slots are:

`Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleConfig/SplashTitleTypography.asset`

Use **Select Typography Settings** to select them. When the original licensed source fonts have been obtained locally, assign `RegularFont` and `StartFont` explicitly. Record the original family/style and supporting design/source evidence. Do not mark a visually similar candidate as the original font.

In the Editor/development builds only, an unbound slot may use an available local serif font (Times New Roman, Liberation Serif, Noto Serif, or Droid Serif); otherwise it uses Unity's built-in development font. A Console warning identifies this as **DEVELOPMENT ONLY**, never reference-approved. OS availability differs between devices, so this is not a shipping typography solution.

**Non-development builds intentionally fail the font gate until both source fonts and their reference identity are verified.** Enable Development Build for testing while that gate is open. This is separate from compilation correctness.

## What changed

- The canonical Splash prefab now contains ten direct PNG-backed Images, five real Buttons, and eight live text objects.
- The runtime loader instantiates that prefab. Old `SplashTitleFinal` crops and `SplashTitleLivingMotion` overlays are no longer used on the active Splash path.
- ScreenRouter binds the visible Buttons to the existing Loading/Home, Language Select, Settings, Login and Tutorial routes. It does not add duplicate invisible hotspots.
- The existing safe-area/Canvas fit remains authoritative. Only the environment expands to cover the full display, including insets. The stage is 941×1672.
- Normal/highlighted/pressed/selected/disabled appearances use native Button states; press feedback scales the actual control slightly. No Animator dependency is introduced.
- A scoped importer exception preserves these source sprites at up to 4096 pixels and uncompressed RGBA32 on Android during visual QA. Other pages retain their original importer rules. Optimize memory/compression only after visual acceptance; this is not a release-performance signoff.
- The old generic cutover/rollback tool is prevented from replacing an R3-owned Splash prefab. Valid R3 prefabs are not regenerated on every domain reload.
- A repair builder, wiring validator, runtime capture command and explicit release-font guard are included.

## Remaining art constraints

The generated selector includes its globe/arrow in the same graphic, and each generated round control includes its icon in the same graphic. The Start frame/fill are also one supplied sprite. These are used directly as requested; independent sub-part artwork or 9-slice versions are not falsely claimed. Ornamental assets are not stretched with 9-slicing.

No separate divider PNG was generated. The divider in this build is native reconstructed UI geometry, not the rejected tagline screenshot crop. It still needs visual approval.

The supplied portraits terminate at cropped lower edges. A small, reversible **runtime mesh fade at the bottom only** blends those terminations into the environment. Source PNGs are not repainted, segmented or changed. This does not reconstruct missing anatomy or contaminated silhouette details. Character breathing/head/hair/cloth animation is not added; the Firebird glow remains part of its supplied sprite.

## Verify in Unity

At the actual Bootstrap-started Splash, click Start and confirm Loading → Home. Revisit Splash and test Language, Settings, Account and Help with Back. Confirm no double navigation, input-blocking decoration, or missing text. The destination pages/back-end capabilities remain what the existing project already implemented; this patch does not add online authentication.

Review at 941×1672, 1080×1920, 1080×2340 and 1080×2400, then on a notched phone. Inspect the hierarchy and Console after a domain reload and another Play session. Confirm font glyphs and actual font identity separately.

For a real screenshot choose **Legends of Rus > Production UI > Splash R3 > Capture Current Runtime Screen** while Splash is visible and its transition has finished. This reuses the project's end-of-frame QACaptureService, avoiding the optional ScreenCapture module. The existing service writes capture files under `ProjectDocs/SplashTitle_R3_Integration/RuntimeEvidence`.

The included `OfflineComposition` and `Reference_vs_OfflineComposition` images are **offline compositing studies, not Unity screenshots**. They cannot prove the Editor compiled, the fonts rendered in Unity, or a button worked.

## Portable checks

`python Tools/verify_splash_r3.py --project .`

For a reference-sized actual Unity capture (Pillow required):

`python Tools/compare_splash_capture.py --capture "path/to/actual_capture.png" --output "ProjectDocs/SplashTitle_R3_Integration/MyRuntimeComparison"`

The comparison tool rejects mismatched sizes rather than stretching the image and never converts a similarity score into automatic visual approval.

## Evidence

See `ProjectDocs/SplashTitle_R3_Integration/` for the original PNG hash/alpha inventory, saved-file changes, source diff, geometry-only checks, independent black/white/magenta inspections, typography study and verification status. No original `.meta`, scene, package file, project setting, or artwork was deleted or changed.
