# Android UI and lifecycle check — pending

The user-approved Development application ID is applied and a verified APK is ready: see [build report](ReadinessReport.md). No phone was connected before or after the build. The checks below remain planned, not completed device evidence.

Before installation, identify the intended phone and verify the APK package/certificate against any installed test app. Preserve its data; do not uninstall or clear storage to resolve an update conflict. The approved debug ID and the current DefaultCompany ID are different Android apps with separate data; changing IDs does not transfer progress.

1. Capture Home, Settings and the reported soft-text pages at the phone's native resolution in Russian and English. Check text at native pixels, safe areas and top/bottom touch targets.
2. Tap Home → World Map → Chapter Map → available round → Pre-Level → Gameplay. Verify one energy charge, correct selected round and working input.
3. Interrupt an idle board and an active swap with Home/background, notification shade and screen lock. Return and check board, moves, energy and sound; release any interrupted touch without an unintended swap.
4. Open Pause → Settings, background/resume, Back → Pause → Resume. Verify modal and board state, controls and hardware Back behavior.
5. Complete a round to Victory and return through the story/maps to Home. Verify rewards once. Exhaust a round to Fail, retry, then exit; preserve the current free-retry rule.
6. Change sound preferences/language and record balances/progress. Force-close and relaunch; confirm durable state returns. An unfinished board is currently not saved and must not be reported as restored.

Record device model/OS, app version/certificate, actual display/safe area, before/after balances, outcomes and native screenshots of the game only. Do not record unrelated phone screens or personal notifications. A Development APK/build pass is not physical-device acceptance.
