# v1.19 Step 1 — Production Launch Operations and Post-Launch Observability Foundation

## Status

Implemented in source. Unity execution and all real production evidence remain pending.

## Purpose

This checkpoint adds a fail-closed operations layer after v1.18 launch-candidate readiness. It does not publish, build, sign, upload, halt, resume, or roll back a Play release. It only evaluates retained evidence and produces a recommendation for human operators.

## Implemented

- Engine-free `LegendsOfRus.LaunchOperationsCore`.
- Production launch preflight with explicit monitoring, incident-response, rollback, support, release-history and manual authorization gates.
- First production release remains 100%; staged rollout is accepted only for production updates.
- Android vitals review for current Google Play bad-behavior thresholds.
- Human escalation for overall/per-device crash and ANR bad behavior.
- Additional local watch/review handling for wake locks, battery, incident severity and crash/ANR investigation.
- Privacy-minimal incident summary and Android vitals evidence templates.
- Fixed 64-record in-memory observation buffer.
- Editor commands to print the local preflight and write a sanitized local snapshot.
- Dedicated EditMode regression tests and Editor self-test.

## Current Play stability thresholds encoded

Policy snapshot rechecked 2026-09-09 from Google Play / Android Developers:

- User-perceived crash rate: 1.09% overall bad-behavior threshold.
- User-perceived ANR rate: 0.47% overall bad-behavior threshold.
- Per-phone-model crash rate: 8%.
- Per-phone-model ANR rate: 8%.
- Excessive partial wake locks: 5% overall.
- Excessive battery usage: 1% overall.

These thresholds trigger review/hold/escalation signals only. Step 1 never performs an automatic Play action.

## Deliberately blocked

- Production release start.
- Store publishing.
- Automatic staged-rollout increase.
- Automatic rollout halt/resume.
- Automatic rollback.
- Play Developer Reporting API.
- Any network telemetry provider.
- Keystore/signing-secret access.
- Tester identity persistence.
- Device identifier persistence.

## Evidence boundary

The source package contains templates only under `ProjectDocs/LaunchOperations`. A real launch requires operator-reviewed copies tied to the exact candidate/AAB hash.

## Unity validation

1. Open in Unity 6000.5.9f1 and wait for compilation.
2. Confirm zero Console errors.
3. Run `Legends of Rus > Tests > Run v1.19 Step 1 Self-Tests`.
4. Expected source self-test result: 88/88.
5. Run the complete EditMode and PlayMode suites.
6. Run `Legends of Rus > QA > Print v1.19 Step 1 Launch Operations Preflight`.

The preflight should remain NO-GO until real launch authorization and evidence are supplied.
