BuildOptionsFix04 — cumulative Editor patch for SplashTitle R3 FidelityFix02/Fix03.

Stop Play Mode and close Unity. Back up the project. Merge Assets, preserving existing .meta files.
Reopen in Unity 6000.5.9f1 and wait for compilation. Activate Android.
Run: Legends of Rus > Android > Build Fix04 > Prepare Development Options and Export Diagnostics
Report: ProjectDocs/AndroidBuildFix03/AndroidBuildDiagnostics.json

When all readiness checks pass:
Legends of Rus > Android > Build Fix04 > Build Development APK (No Run)

This patch fixes flag validation; it does NOT approve the existing package lock or claim that an APK builds.
No runtime artwork, prefab, font, package version, signing or production setting is changed.
Read ProjectDocs/AndroidBuildFix04/README.md for limits and tests. Unity compilation/tests were not run here.
