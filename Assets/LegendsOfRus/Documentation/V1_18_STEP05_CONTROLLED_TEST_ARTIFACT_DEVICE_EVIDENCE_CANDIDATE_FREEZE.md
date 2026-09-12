# v1.18 Step 5 — Controlled Test Artifact, Device Evidence, and Candidate Freeze

## Purpose

Turn the release-candidate preparation work from Step 4 into a reproducible evidence/freeze boundary without manufacturing missing external evidence or enabling publication.

## Signed artifact evidence

The operator may place exactly one explicitly authorized `.aab` under `ProjectDocs/ClosedTestEvidence/Artifacts`. The Editor runner recomputes its filename, byte length, and SHA-256. Companion metadata supplies version/API/signing/ABI/16-KB attestations and is validated by the inherited Step 4 `ControlledTestArtifactValidator`.

No AAB is included in this source candidate.

## Physical-device evidence

Five profile buckets are required:

- `android_low_16_9_720p`
- `android_mid_18_9_1080p`
- `android_mid_19_5_9_1080p`
- `android_high_20_9_1080p`
- `android_high_20_9_1440p`

Every record must reference the same staged AAB SHA-256 and pass fresh install, core loop, suspend/resume, crash-free, blocking-visual-defect-free, minimum-duration, and tester-attestation checks. Unknown, duplicate, missing, wrong-aspect, cross-build, or failing records invalidate the set.

The schema intentionally excludes persistent device identifiers and tester identity.

## Candidate freeze

The local freeze tool computes SHA-256 evidence for:

- source tree (`Assets`, `Packages`, `ProjectSettings`, `Tools`);
- `Packages/packages-lock.json`;
- the complete `ProjectSettings` directory;
- the Step 3 release checklist;
- the staged signed AAB;
- physical-device evidence.

`CANDIDATE_FREEZE_MANIFEST.json` can only be generated when all freeze gates are true. The generated manifest is itself SHA-256-pinned. Generation does not build, upload, publish, sign, or contact Google Play.

## Shipped activation state

Local validation/tooling is approved. All actual evidence/authorization flags remain false: Unity/Test Runner evidence, signed AAB, physical-device evidence, store/privacy approval, production identity/versioning/signing, controlled-test build, candidate freeze, closed-test upload, and store publishing.

## Unity verification

1. Open in Unity 6000.5.9f1 and confirm zero compile errors.
2. Run `Legends of Rus > Tests > Run v1.18 Step 5 Self-Tests`; expected `36/36`.
3. Run complete EditMode and PlayMode suites.
4. Run `Legends of Rus > QA > Print v1.18 Step 5 Candidate Freeze Preflight`.
5. Do not enable evidence/authorization flags until the matching evidence has actually been retained and reviewed.
