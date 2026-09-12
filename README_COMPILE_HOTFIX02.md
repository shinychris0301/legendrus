# v1.17 Step 5 Compile Hotfix 02

Fixes Unity 6000.5 CS0619 errors originating from Addressables 2.7.6 calls to
`Object.GetInstanceID()` by upgrading `com.unity.addressables` to 2.8.1 and aligning
Step 4/Step 5 self-tests and static verification with that pin.

Apply over the Step 5 project root while Unity is closed. Then reopen Unity and allow
Package Manager to resolve Addressables 2.8.1. Do not edit `Library/PackageCache`.
If Unity continues to reuse the old package, close Unity and delete the cached
`Library/PackageCache/com.unity.addressables@...` folder, then reopen.
