# Splash/Title 01-R — Source Integration Fix 01

**This is a limited navigation/source-validation patch, NOT the completed R2 visual implementation.**

Base archive: `LegendsOfRus_Unity_v1_20_VisualPage01R_SplashTitle_ReferenceAccurate_FULL.zip`.
The supplied project contains the older R cutouts and rasterized labels. The previously described R2 transparent control bases and R3 prefab rebuilder are not present. The artwork and all prefabs are intentionally unchanged; this patch does not disguise old crops as newly reconstructed assets.

## Apply

Close Unity, back up the current project, and merge the PATCH ZIP's contents into the project root that contains `Assets`, `Packages`, and `ProjectSettings`. Overwrite the two matching C# files. Preserve any existing local `.meta` files, including a locally generated `ScreenRouter.cs.meta` that was not included in the uploaded archive.

This patch is based on the exact uploaded 01-R archive. Do not blindly overwrite a newer local R2/R3/FIX project: use `ProjectDocs/SplashTitle_R_IntegrationFix01/source_changes.diff` to review the two-file changes against that newer project first.

The FULL ZIP contains the uploaded project plus this same limited patch; it is not a different or visually finished build.

## Changes

- `Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs`: explicitly dispatch enabled production meta pages before legacy/demo presentation; guard all five Splash navigation callbacks by page generation; keep source-bound Back consistent with Splash auxiliary return behavior; clear transient return state when leaving the auxiliary page.
- `Assets/LegendsOfRus/Scripts/Editor/Step20SplashTitleProductionSelfTest.cs`: separate source checks from production-readiness checks; stop accepting file presence/pixel reconstruction scores as visual proof; stop rejecting live text; report the remaining legacy-asset, prefab, and typography blockers.

No PNG, sprite metadata, saved prefab, scene, package, project setting, or original `.meta` file was changed. No font binaries are included.

## Validation available in Unity

`Legends of Rus > Tests > Run Splash-Title Source Integration Checks (Not Visual Acceptance)` checks source integration only.

The pre-existing `Run v1.20 Visual Page 01-R Splash-Title Reference-Accurate Self-Test` menu now reports production readiness honestly. **It is expected to report blockers for this archive.** Its diagnostic exception is not a C# compilation error and does not indicate that this patch attempted an art rebuild.

Unity compilation, actual button interactions, aspect-ratio behavior, domain-reload persistence, and runtime screenshots have **not** been tested in the inspection environment. No Unity executable is available there.

Reproduce the static archive/source checks with Python 3.9 or newer:

```bash
python Tools/verify_splash_r_integration_fix01.py --baseline /path/to/LegendsOfRus_Unity_v1_20_VisualPage01R_SplashTitle_ReferenceAccurate_FULL.zip
```

Read `ProjectDocs/SplashTitle_R_IntegrationFix01/SOURCE_AUDIT.md` and inspect the alpha sheets before continuing visual implementation. The actual existing R2 asset package is needed to finish the requested screen without recreating or substituting artwork.
