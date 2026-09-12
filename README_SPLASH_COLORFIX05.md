# Splash/Title ColorFix05 — foreground textures updated

Implemented additional darkening and mild desaturation of the nine existing
foreground PNGs. Background, source alpha, GUIDs, layout, live text and routes are
unchanged. No new C# code is introduced by this color revision.

## PATCH — preserve your current local project

Close Unity and back up the project. Merge this archive into the current
FidelityFix02 project (with or without the earlier Fix04 build patch). Overwrite
the nine matching PNGs under Assets, retaining existing .meta files. This patch
contains no .meta files, Player settings, packages, fonts, prefab or build scripts.
No manual prefab rebuilding, new tint, or import-setting change is required.

## FULL — separate project copy

Extract into a new folder. Open the folder containing Assets, Packages and
ProjectSettings with Unity 6000.5.9f1. The full archive includes FidelityFix02 and
the previously delivered BuildOptionsFix04 code. It does not contain your
unuploaded local Player settings or package-resolution results.

Open Assets/Scenes/Bootstrap.unity. Use a 941 x 1672 Game view for comparison.
The files already used by SplashTitle.prefab contain the revised color values.

## Verification and remaining requirements

The nine textures' pixel contents, dimensions, alpha, source GUID references,
unchanged background and preserved project files were checked locally.
Unity was not run here; exact visual fidelity needs a new Unity capture.
The font-identification and independent Android build-approval issues are unchanged.
This is not an APK-build unblock or a certified reference match.

Detailed report, per-file parameters, alpha proofs and labeled offline comparisons:
ProjectDocs/SplashTitle_R3_ColorFix05/

Optional source-file verification (Python 3.10+):
python Tools/verify_splash_colorfix05.py
