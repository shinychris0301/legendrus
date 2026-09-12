# Legends of Rus — v1.17 Step 3

## Crash and Error Reporting Foundation

Status:
`IMPLEMENTED_IN_SOURCE_PENDING_V1_17_STEP02_PREREQUISITE_AND_STEP03_UNITY_VERIFICATION`

This pre-entry source candidate adds a detached, engine-free diagnostics core
for typed and data-minimized nonfatal and managed-crash records. It provides a
closed enum catalog with internally generated wire codes, a bounded
process-memory buffer, deterministic fingerprints and canonical JSON,
duplicate coalescing, timestamp-triggered hard retention, offline-safe delivery
decisions, capped retry scheduling, and adapter failure isolation.

It does not collect Unity Console messages or stacks, subscribe to a Unity log
callback, persist reports, create a GameObject, run background work, install an
SDK, or contact a network. The public API cannot inject a transport; the only
shipped production path uses the disabled transport and the compiled network
transport gate is false.

The v1.17 Step 2 Unity and representative-device evidence required before
formal Step 3 entry is still not retained. First complete that prerequisite,
then run the Step 3 baseline verifier, Unity 6000.5.9f1 compilation, project
validator, Step 3 Editor self-test, and complete EditMode and PlayMode suites.

All package-lock, economy, commercial, cloud, Android-production, crash-
provider, and publishing gates remain false. Start with
`Assets/LegendsOfRus/Documentation/V1_17_STEP03_CRASH_ERROR_REPORTING_FOUNDATION.md`.
