# Unity 6000.5 — NUnit and duplicate TestRunner reference fix

## Cause

The previous test assembly enabled `TestAssemblies` and also repeated the same
references manually:

- `UnityEngine.TestRunner`
- `UnityEditor.TestRunner`
- `nunit.framework.dll`

`TestAssemblies` already injects NUnit and the TestRunner assemblies. Using both
mechanisms creates the duplicate-reference compiler error, and the malformed test
assembly then reports that `NUnit`, `Test`, `TestCase`, and `TestAttribute` cannot
be found.

## Correct assembly definition

```json
{
  "references": [
    "LegendsOfRus.GameplayCore"
  ],
  "includePlatforms": [
    "Editor"
  ],
  "overrideReferences": false,
  "precompiledReferences": [],
  "optionalUnityReferences": [
    "TestAssemblies"
  ]
}
```

## Apply

1. Close Unity.
2. Extract the hotfix into the project root.
3. Overwrite the `.asmdef` and validator script.
4. Reopen Unity and wait for compilation.
5. Run:
   `Legends of Rus > Tests > Validate Test Framework Setup`

Expected result:

`Test Framework setup PASSED`

## If stale errors remain

Close Unity and delete:

- `Library/Bee`
- `Library/ScriptAssemblies`
- `Temp`
- `obj`

Then reopen Unity.

Do not delete `Assets`, `Packages`, or `ProjectSettings`.
