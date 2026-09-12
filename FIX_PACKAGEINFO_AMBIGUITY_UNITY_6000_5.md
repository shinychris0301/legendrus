# Unity 6000.5 — PackageInfo ambiguity fix

## Error

```text
CS0104: PackageInfo is an ambiguous reference between
UnityEditor.PackageManager.PackageInfo and UnityEditor.PackageInfo
```

## Cause

Unity 6000.5 exposes two editor types named `PackageInfo`. The validator imported
`UnityEditor` and `UnityEditor.PackageManager`, then referenced `PackageInfo` without
qualifying which type it intended.

## Fix

The script now uses an explicit alias:

```csharp
using PackageManagerPackageInfo = UnityEditor.PackageManager.PackageInfo;
```

and calls:

```csharp
PackageManagerPackageInfo.FindForPackageName(PackageName);
```

## Apply

1. Close Unity or exit Play Mode.
2. Extract the v3 hotfix into the project root.
3. Overwrite `TestFrameworkSetupValidator.cs`.
4. Reopen Unity and wait for compilation.
5. Run `Legends of Rus > Tests > Validate Test Framework Setup`.

No Library deletion should be required for this one-line compiler correction. If Unity
still displays stale output, use **Assets > Reimport All** or close Unity and delete
`Library/ScriptAssemblies` before reopening.
