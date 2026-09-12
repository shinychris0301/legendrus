# v1.18 Step 1 — Device Matrix QA and Closed-Test Release Preflight Foundation

## Scope

This checkpoint begins v1.18 Closed Test / Soft-Launch Candidate work without authorizing a store upload.
It adds deterministic QA buckets, a local-only coarse device profile, a bounded observation buffer, and a fail-closed release preflight model.

## Device matrix

The source catalog covers five synthetic Android QA profiles across 16:9, 18:9, 19.5:9 and 20:9, and across low/mid/high QA capability bands. These profiles are test targets, not a promise that a specific physical device is supported.

Physical-device evidence remains pending until the exact Step 1 bytes are run on representative hardware.

## Privacy boundary

The QA runtime does not use `deviceUniqueIdentifier`, advertising IDs, device model/name, account identity, email, recovery codes, or free-form user strings. The runtime snapshot contains only coarse memory/CPU/texture/screen dimensions and derived QA buckets. Observation events are fixed enums plus small integers, kept in a bounded in-memory buffer and never uploaded or persisted in this checkpoint.

## Release preflight

The preflight distinguishes source readiness, Unity compile/tests, device-matrix evidence, Android build evidence, privacy/data-safety approval, store listing assets, production signing, controlled-build authorization, and publishing authorization. Missing evidence blocks progression rather than being inferred.

## Explicitly blocked

- remote QA upload/provider;
- production analytics/ads/IAP activation;
- remote Addressables delivery;
- production signing;
- controlled-test build authorization;
- Google Play publishing;
- final privacy/data-safety approval;
- final store listing assets.

## Unity verification

1. Open in Unity 6000.5.9f1.
2. Confirm zero compile errors.
3. Run `Legends of Rus > Tests > Run v1.18 Step 1 Self-Tests`.
4. Expected source/editor result: `22/22`.
5. Run complete EditMode and PlayMode suites.
6. Exercise Home → Pre-Level → Gameplay → Victory/Fail → Home.
7. Capture representative-device results separately; do not mark synthetic matrix entries as physical-device passes.

## Exit gate

Step 1 is closed only after the exact package compiles and its new tests pass in Unity. Physical-device, signed-build and Play Console evidence remain separate v1.18 gates.
