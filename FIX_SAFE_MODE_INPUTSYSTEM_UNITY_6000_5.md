# Fix Safe Mode — Unity 6000.5.9f1

## Cause

The previous project pinned:

`com.unity.inputsystem = 1.17.0`

Unity 6000.5 treats several old Unity Editor APIs as obsolete errors. Input System 1.17.0 still calls those APIs in its Editor code, producing CS0619 errors from `Library/PackageCache`.

This project now pins:

`com.unity.inputsystem = 1.18.0`

Input System 1.18.0 contains Unity 6.4 / 6.5 compatibility fixes.

## Recovery steps

1. Close Unity completely.
2. Replace `Packages/manifest.json` with the v1.3 version.
3. Delete the project's `Library` folder.
   - This deletes only Unity's generated cache.
   - Do NOT delete `Assets`, `Packages`, or `ProjectSettings`.
4. Reopen the project with Unity 6000.5.9f1.
5. Wait while Package Manager downloads Input System 1.18.0 and Unity rebuilds `Library`.
6. Do not enter Play Mode until the Console shows no red compiler errors.
7. Exit Safe Mode.
8. Run:
   `Legends of Rus > Unity 6 > Apply Recommended Project Settings`
9. Restart Unity once if Active Input Handling changes.
10. Open `Assets/Scenes/Bootstrap.unity` and press Play.

## Important

Do not edit files under:

`Library/PackageCache/com.unity.inputsystem@...`

They are generated Package Manager cache files and will be replaced.

If Unity still resolves 1.17.0, verify `Packages/manifest.json` contains:

`"com.unity.inputsystem": "1.18.0"`

and delete `Library` again before reopening the Editor.
