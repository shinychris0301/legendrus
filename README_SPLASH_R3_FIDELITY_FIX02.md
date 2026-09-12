# SplashTitle R3 — FidelityFix02

This package contains actual modified Unity source, the canonical serialized prefab,
and revised individual PNG resources. It is not a design image or a checklist.

## Open or apply

For the FULL package, extract it into a NEW folder and open that folder with the
Unity version recorded in ProjectSettings/ProjectVersion.txt: **6000.5.9f1**.
Open **Assets/Scenes/Bootstrap.unity**. Keep a separate backup of your working project.

For the PATCH package, stop Play Mode and close Unity, back up or commit your current
R3 DirectAssets project, then merge the supplied folders into its root. Overwrite
the corresponding files, not the entire Assets directory. Preserve .meta files.
The patch targets the R3 DirectAssets implementation from this conversation; it
does not replace unrelated local changes in an uninspected branch.

The FULL project includes the prior SourceFix01, R3 DirectAssets and WarningFix01
changes plus this correction. The PATCH contains the FidelityFix02 changes only.

Set Game view to the **941 x 1672** fixed-resolution portrait preview. The displayed
preview scale can be reduced to fit the Editor window; inspect text at 1:1 in an
actual captured image before judging pixel sharpness.

The canonical prefab is already updated. The builder safely repairs only a missing,
invalid or older-revision prefab in Edit Mode. Do not run repair during Play Mode.

## Implemented corrections

- Nine foreground PNGs are conservatively tone-calibrated in linear light and
  saved back as sRGB images, with every source alpha byte preserved. No global
  color-space switch, post-processing overlay or new runtime tint shader is used.
- The environment plate receives a local, same-plate sky repair in the visible
  gap between Ivan's hair and the Firebird. Pixels outside the finite repair mask
  are byte-identical to the earlier environment. The rest of the background is
  not regraded. The mask affects about 3.93% of the full plate.
- Start uses a bold live-text treatment. All eight live labels have deliberate
  2x local glyph raster density with inverse local scale, so their authored
  effective layout rectangles stay the same size.
- A scoped glyph-mesh effect aligns single-line labels using actual visible
  glyph bounds and snaps the whole line's placement to the screen pixel grid.
  It does not stretch individual letters. It is independent of font identity.
- The language label's target is lifted five reference pixels; the primary
  label is centered on the emerald face instead of relying on the font metrics
  box. The lower captions are lifted seven reference pixels.
- Seven small labels no longer use the duplicated dark Shadow geometry.
  Start retains one restrained shadow. Footer text has more contrast, with the
  footer gradient adjusted behind it.
- Button selection no longer multiplies artwork brightness above 1.0.
  The existing pointer-down feedback and five navigation routes are preserved.
- Import policy explicitly retains sRGB colour interpretation, no NPOT resizing,
  uncompressed review textures, alpha, no Read/Write copy and no mipmaps in the
  reconstructed Splash folder only.
- Runtime capture output now goes to this revision's RuntimeEvidence folder.

## Font status — still unverified

No exact source typeface was established from the image. No font binaries are
included, automatically downloaded, or marked source-verified.

The existing RegularFont/StartFont slots and the once-per-session development
fallback remain. On the user's Linux system that fallback was Liberation Serif.
The glyph-density, style and alignment fixes are implemented even while that
fallback is used. They do not establish exact font identity or guarantee identical
font rendering across operating systems.

The existing release font guard remains active. Development testing is possible;
a non-development release remains blocked while the exact-font requirement is
unverified. The generated showcase panel from the conversation is NOT a Unity
capture and is not evidence of a licensed or exact font. It is NOT included as
runtime content or acceptance evidence.

## Validation in Unity

Run **Legends of Rus > Production UI > Splash R3 > Validate FidelityFix02**.
This checks serialized wiring, label geometry/density, import policy and a small
synthetic glyph-alignment calculation inside Unity.

Start from Bootstrap and inspect the actual screen and all five buttons. Use
**Legends of Rus > Production UI > Splash R3 > Capture Current Runtime Screen**.
Real captures and their metadata go under:

ProjectDocs/SplashTitle_R3_FidelityFix02/RuntimeEvidence/

Capture at 941x1672 first, then verify 1080x1920, 1080x2340, 1080x2400 and a notched
portrait device. Do not judge the reference match from a Free Aspect landscape
preview or from the offline preview supplied here.

## Work verified here

- Portable source/resource checks and source lexical/delimiter checks.
- All ten revised PNGs exist and retain the original dimensions.
- All nine transparency masks are byte-identical to the original R3 masks.
- All 706 existing .meta files in the merged baseline are unchanged.
- Package files, scene files and existing project settings are unchanged.
- The five native Buttons, eight live Text objects, ten Image components and
  canonical runtime prefab route remain in the serialized/source implementation.
- Offline safe-area geometry checks and alpha inspection previews.

No Unity Editor/compiler is installed in the execution environment used to prepare
this package. Unity compilation, Play Mode navigation, real device profiling and
new Unity screenshots have NOT been performed here. The user's earlier screenshot
is evidence of the earlier R3 build, not of this modified build.

## Reversibility and evidence

Read IMPLEMENTATION_REPORT.md, ColorCalibration.json, asset_inventory.json,
StaticVerification.json, PortableVerification.json and SourceChanges.diff in
ProjectDocs/SplashTitle_R3_FidelityFix02.

The Before folder contains the ungraded R3 source PNGs, outside Assets/Resources.
They are preserved for audit and are not loaded by the runtime UI.

The opt-in tool Tools/rebuild_splash_fidelityfix02_art.py recreates the corrected
PNGs from those originals and the stored profile/mask; it never grades the already-
graded PNGs a second time. It requires Pillow and numpy. Without --apply it does not
write assets. A profile change still needs a new Unity visual review.

OfflinePreview_NOT_UNITY.jpg is a Pillow composition using the actual revised PNGs
and approximate offline live type. It is permanently marked NOT UNITY. It is not
a measured demonstration of Unity text rasterization, routing, or an exact match.
