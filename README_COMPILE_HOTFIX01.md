# v1.17 Step 4 Compile Hotfix 01

Fixes Unity Test Framework 1.6.0 / NUnit constraint compile error:

`CS1503: Argument 1: cannot convert from 'System.Type' to 'string'`

File:
`Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs`

Change:

```csharp
// Before
Assert.That(parameterTypes, Does.Not.Contain(typeof(string)));

// After
Assert.That(Array.IndexOf(parameterTypes, typeof(string)), Is.EqualTo(-1));
```

The assertion has the same intent: verify that `ErrorReportDraft` has no `string` constructor parameter.
This is a test-only compatibility fix and does not change runtime/gameplay/save/economy/commercial behavior.

Apply by extracting this archive into the Unity project root and allowing the file to be overwritten.
