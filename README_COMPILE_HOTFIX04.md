# v1.18 Step 3 — Compile Hotfix 04

## Fix
Adds Unity Test Framework test-assembly visibility to:

`Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef`

The BalanceReview PlayMode tests use `[UnityTest]` and `UnityEngine.TestTools`, but the asmdef did not declare the Unity test assembly reference.

Added:

```json
"optionalUnityReferences": [
  "TestAssemblies"
]
```

This resolves the `CS0246` errors for `UnityTest` / `UnityTestAttribute` without changing gameplay or the test source.

## Apply
1. Close Unity or allow compilation to finish.
2. Extract this ZIP over the v1.18 Step 3 project root.
3. Replace the existing asmdef when prompted.
4. Reopen/return to Unity and allow scripts to recompile.

No Library/PackageCache edits are required.
