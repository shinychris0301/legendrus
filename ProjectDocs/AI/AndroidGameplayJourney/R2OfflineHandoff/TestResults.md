# Offline Android QA handoff R2 — 2026-10-01

Completed the approved next preparation milestone. The current checklist, profile instructions and preflight now agree on the verified Gameplay revision-24 APK. Phone execution and hosting remain postponed. Every physical gameplay row remains pending; three large-stock rows additionally need a separately reviewed disposable fixture.

## Current artifact and preparation

[Stock-refresh R6 APK](../../../../Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk): **765,764,694 bytes**, SHA256 `42b29fca2c6941ed2ac96e5c7dd00f9e6c79aab70dfd5b0892a64d7faa815ce5`. The local hash, size and sidecar were checked again and agree with R6's actual successful build, inspection and final reconciliation. Profile and stock markers are recorded in that inspection; this task did not recompile the player or repeat signature/ELF verification. [New offline preflight](QA/OfflinePreflight.json), [prior build evidence](../../CoreOutcomeReadability/R6StockAndroidRefresh/TestResults.md).

Local `aapt dump badging` rechecked the actual APK manifest: package `com.example.legendsofrus.v117debug`, version `0.0.0` / 1, debuggable, portrait, ARM64, API 26–36, launch activity `com.unity3d.player.UnityPlayerGameActivity`. [Manifest output](QA/LocalManifest.txt). Gameplay revision 24 and four layouts are established by the hash-linked R6 packed report; the manifest alone does not show a prefab revision.

The [current tool](Sources/Preflight.py) defaults to local-only checks. `--device` explicitly selects read-only device identity queries; it is not used in this task. Missing/changed APK, sidecar or inclusion evidence blocks readiness before any device query. Optional device mode requires exactly one authorized attachment and a matching monolithic installed APK; mixed/unauthorized devices, split packages, invalid paths, unsupported API, query failures, unavailable/mismatched hashes and timeouts remain review states. Even a matching device returns `device_identity_verified_qa_activation_pending`, never a gameplay pass. The [existing R1 entry](../R1/Sources/DevicePreflight.py) delegates to the same tool, preserving its familiar command path. R1's historical device result is unchanged; new evidence is written under R2.

## Checklist and profile limits

[Active device checklist](../R1/DeviceChecklist.md) and [profile usage](../../AndroidGameplayProfile/R1/Usage.md) now describe the packaged disposable save facility accurately. Cold-launch `fresh`, `resume`, and `empty-energy` remain the complete command set. QA activation requires the visible yellow indicator before controlled actions; launcher-icon startup without an extra uses normal data. No new activation command or stock override was introduced.

Three added rows cover full quantities 999 / 1 000 / 9 999 / 2 147 483 647 in RU/EN, 1 000→999 Sea Pearl consumption / +5 moves / duplicate busy tap, and input forwarding / targeted cancellation. Full ten-digit readability remains a known risk. The packaged profile cannot seed arbitrary quantities; synthetic R5 Editor fixtures cannot be applied to a phone via a made-up intent extra. These stock rows are blocked on fixture preparation and physical execution, while ordinary journey rows remain pending QA activation and physical execution. No stock acceptance or human result is inferred.

## Checks performed and preservation

- **36 offline boundary checks passed**, zero failed. Synthetic file/evidence changes and a mocked read-only ADB response matrix exercise the readiness failures and default-mode no-device boundary. No real ADB was invoked. [Cases](QA/PreflightBoundaryChecks.json), [test source](Sources/CheckPreflight.py).
- The actual offline preflight, through the existing compatibility command, exited 0 with `offline_prepared_device_deferred`. Local aapt exited 0. These are Python/tool/manifest checks, not Unity compilation, Play Mode, Android JNI or lifecycle tests.
- [Final validation](QA/FinalValidation.json) reconciles the exact **7,825 unchanged** project inputs, APK/sidecar, prior R4 archive, five unchanged manual recordings, historical R1 device record, scoped task changes and current documentation links. [Scope](QA/Scope.json), [changed-file inventory](QA/ChangedFiles.json).

Two final-reconciliation assertions needed correction: the first expected the XML attribute name `screenOrientation` in aapt's badging text; the second counted the table header as a device row. Checks now use the actual portrait feature declaration and scenario IDs. Product/manifest inputs were unchanged. [Initial validation failures](QA/InitialValidationFailure.json) retained separately from the 36 passing tool cases.

Existing files changed: R1 DevicePreflight.py, R1 DeviceChecklist.md, profile R1 Usage.md, CurrentTask.md, SessionLog.md, DevelopmentSchedule.md and ConversationHandoff.md. Added: this report, canonical preflight, boundary/final validation sources and task evidence/previous-file snapshots. No Assets/Packages/ProjectSettings/Backend edits, new APK, artwork/fonts/prefabs/GUID changes, settings/package upgrade, guard bypass or cache patch. Prior user edits and approved Splash preserved. No Unity invocation, new runtime captures, ADB/device operation, install/launch, player-data access, hosting, deployment or commit.

## Next action

Evaluate the separately scheduled returning-player catch-up/recap need against the installed Home/map/story/save flow before adding feature scope. Current human Chapter 1 acceptance still requires 201–210 recordings through the existing Active recorder, followed by the gap queue before tuning. Physical lifecycle/performance, large-stock fixtures/readability and exact font provenance remain open. Phone execution/hosting remain deferred; final gameplay effects stay last.
