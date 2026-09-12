# v1.20 Step 1 Compile Hotfix 05 — Step18 String Literal Repair

## Fixes

This hotfix repairs two inherited v1.18 Step 4 Editor scripts that contained invalid generated C# string literals:

- `Assets/LegendsOfRus/Scripts/Editor/Step18ControlledTestBuildPreflightRunner.cs`
- `Assets/LegendsOfRus/Scripts/Editor/Step18ControlledTestBuildReleaseCandidateSelfTest.cs`

Corrections:

1. Replaces physical line breaks embedded inside ordinary C# quoted strings with explicit `\n` escape sequences.
2. Replaces malformed generated JSON search literals such as `""key": value"` with valid escaped C# strings such as `"\"key\": value"`.
3. Does not change gameplay, saves, content-pipeline data, approved artwork, Packages, ProjectSettings, Addressables, economy, monetization, or publishing gates.

## Apply

1. Close Unity or allow it to finish compiling.
2. Extract this ZIP over the project root.
3. Allow replacement of the two files above.
4. Reopen/return to Unity and wait for compilation.

The CS1010/CS1003/CS1026/CS1002 syntax errors reported from these two Step18 files should be removed.
