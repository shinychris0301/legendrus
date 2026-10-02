# Android credential storage R1 — test results

2026-09-29 local date. **1,961 distinct Unity tests, 17 host Java crypto/storage tests and 34 backend tests passed.** The Android credential adapter is implemented and compiles against API36. Android device execution remains unverified: no device was attached and no APK was built or installed.

| Verification | Result | Evidence |
|---|---:|---|
| Full project Edit Mode, including existing Android guard tests | 1,917 passed | [XML](QA/EditMode/results.xml) |
| Combined credential/session/commercial runtime | 42 passed | [XML](QA/Integration02/results.xml) |
| Separate-process real HTTP lost-renewal response writer | 1 passed | [XML](QA/Restart-write/results.xml) |
| Separate-process real HTTP recovery reader | 1 passed | [XML](QA/Restart-read/results.xml) |
| Production Java source compilation against Android API36 | Passed | [Compiler/test log](QA/JavaVerification.log) |
| Production cipher envelope/policy with host JCE and injected storage faults | 17 passed | [Java results](QA/JavaVerification.json) |
| Existing backend/RuStore/session regression | 34 passed | [Backend log](QA/Backend.log) |
| Android device / Keystore / AtomicFile / JNI / APK | Not run | [Availability](QA/DeviceAvailability.json) |

[Verification summary](QA/Verification.json) · [Implementation and limitations](ImplementationNotes.md) · [Changed scope](QA/Scope.json) · [Task patch](Sources/Implementation.patch).

## Coverage

Host Java checks use real AES-GCM for round trips, fresh nonces, authentication-tag/cipher/header tampering, wrong keys, package-domain binding, payload limits and reopen behavior. Injected journals test before/after-commit failures, silent commit failure detected by readback, missing records/keys and first-write interruption. A missing key/record pair is pristine; one missing half fails closed without rekeying. These tests do not emulate Android hardware-backed Keystore or prove power-loss durability.

Unity checks exercise the asynchronous credential-store contract, serialization of pending rotation, malformed/future records, I/O failures, no synchronous/plaintext fallback, waiting for durable storage before network, concurrent operation exclusion, Editor factory refusal and actual Android-only Java importer settings. Existing session tests cover duplicate/lost responses, failed commits, identity/origin validation and unchanged local wallet behavior. The42 combined cases comprise11 new credential cases,20 session cases and11 commercial wiring cases.

The real local HTTP and separate-process tests use disposable synthetic fixture storage, **not** the Android adapter. They verify protocol compatibility after making storage asynchronous: a real server rotation commits, its response is deliberately discarded, and a fresh Unity process recovers the same player. Fixture credentials/database are removed from the private temporary folder afterward. No normal saves or actual service credentials were used.

The first verification launcher stopped before Unity because its copied temporary-path assertion and default directory differed. Corrected the launcher path and ran Integration02. No Unity case failed in the executed final runs. Compiler source/target8 bootstrap-classpath warnings were nonfatal; actual Android packaging is still pending.

## Scope and remaining acceptance

Unity6000.5.9f1; prior process-only Vulkan/descriptor4096 approval and isolated saves reused. Existing Android guard, SDK selection, minSdk26/target36, signing, minification, release flags and package settings unchanged. No art/prefab/campaign/difficulty/matching/effect changes. No commit or deployment.

Two existing runtime files plus backend README changed; ten files added (two Java sources, one runtime adapter, one test source and their file/folder metadata), none deleted. Of7,753 baseline Assets/Packages/ProjectSettings/Backend files,7,750 remain identical. Generated test scene/linker artifacts cleaned. The new Java plugin inventory needs review through the existing build gate before a verification APK. No gate was bypassed.

Pending: Android Keystore/JNI/AtomicFile execution, interruption/relaunch and key-invalidation checks, development APK packaging, hosted HTTPS and device acceptance. First-write key-without-record failures deliberately require explicit recovery rather than a silent new account. Linked-account/device-loss recovery, cloud-save reconciliation, deletion, authoritative rounds, playable continuation and real RuStore SDK acknowledgement/refunds remain pending. R43's historical40 runtime failures and human31–500 difficulty/pacing remain open; this is not a full Play Mode or broad visual audit. Animation/effects stay last.

**Next:** review the Java plugin inventory and guarded development-APK readiness, then perform Android storage/session lifecycle checks when a device is available. Keep account UI and purchases disabled until the required account, hosting and store acceptance work passes.
