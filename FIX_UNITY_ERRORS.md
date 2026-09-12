# Unity compile hotfix — v1.1

This revision fixes the errors reported when opening the commercial vertical slice.

## Fixed

### RuntimeBootstrap.cs
- Newer Unity: uses `Object.FindFirstObjectByType<ScreenRouter>()`.
- Unity 2022 LTS: retains `Object.FindObjectOfType<ScreenRouter>()` behind a version guard.
- This removes the CS0618 warning in Unity 2023.1+ / Unity 6 while preserving 2022 LTS support.

### QACaptureService.cs
- Removed the hard compile-time dependency on the `ScreenCapture` type.
- The service now resolves `UnityEngine.ScreenCaptureModule` by reflection.
- If the module is temporarily unavailable during package resolution, the game still compiles and QA capture fails gracefully instead of breaking the project.

### Packages/manifest.json
Added the built-in modules used by the project:
- `com.unity.modules.screencapture`
- `com.unity.modules.imageconversion`
- `com.unity.modules.jsonserialize`
- `com.unity.modules.ui`

## Recommended reopen procedure
1. Close Unity.
2. Delete the project's `Library` folder if Unity cached the previous compile failure. This is safe; Unity recreates it.
3. Open the v1.1 project.
4. Wait for Package Manager and asset import to finish completely.
5. Confirm Console has no red compile errors.
6. Open `Assets/Scenes/Bootstrap.unity` and press Play.
7. Press F12 to test QA screenshot capture.
