# Unity 6000.5.9f1 migration — Legends of Rus v1.2

The two console messages are migration warnings, not game-code compile errors.

## What was changed in v1.2

1. Added `com.unity.inputsystem` 1.17.0.
2. Removed runtime `UnityEngine.Input` usage.
3. Added `RuntimeInput.cs` using `UnityEngine.InputSystem.Keyboard`.
4. Runtime UI now creates `InputSystemUIInputModule`, so mouse/touch buttons work with the new Input System.
5. Added `Legends of Rus > Unity 6 > Apply Recommended Project Settings`.
6. The migration tool sets Active Input Handling to **Input System Package (New)**.
7. The migration tool disables **Dynamic Batching** for Android and the active build target.
8. Project version baseline updated for Unity `6000.5.9f1`.

## First open

1. Open the project in Unity 6000.5.9f1.
2. Wait for Package Manager to install Input System.
3. Use:
   `Legends of Rus > Unity 6 > Apply Recommended Project Settings`
4. Restart Unity once if prompted / if the input backend changed.
5. Use:
   `Legends of Rus > Unity 6 > Validate Project Settings`
6. Expected Console output:
   - `activeInputHandler=1`
   - `Android Dynamic Batching=False`

## Manual equivalent

If you prefer to set these yourself:

- `Edit > Project Settings > Player > Other Settings > Configuration`
- Active Input Handling: **Input System Package (New)**
- `Edit > Project Settings > Player > Other Settings > Rendering`
- Dynamic Batching: **Off**

Changing Active Input Handling requires an Editor restart.

## Why Dynamic Batching can be off

The current vertical slice is primarily Canvas/uGUI and 2D art. Unity's deprecated mesh Dynamic Batching setting is not required for the UI implementation. Later 3D/environment optimization should use GPU Instancing / SRP Batcher where appropriate.
