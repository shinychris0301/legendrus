# v1.18 Step 5 — Controlled Test Artifact, Device Evidence, and Candidate Freeze

## Status

Implemented in source. Unity 6000.5.9f1 compilation, Test Runner execution, signed-AAB evidence, physical-device evidence, and candidate-freeze authorization remain pending.

## What this checkpoint adds

- engine-free `LegendsOfRus.CandidateFreezeCore`;
- strict five-profile physical-device evidence validation;
- every device record is bound to one exact signed-AAB SHA-256;
- duplicate/missing profile rejection;
- fresh-install, core-loop, suspend/resume, crash-free, and blocking-visual-defect attestations;
- minimum five-minute test-session evidence per required profile;
- deterministic source-tree, Packages lock, ProjectSettings, store-checklist, AAB, and device-evidence hashes;
- local candidate-freeze preflight and manifest generation;
- fixed evidence staging under `ProjectDocs/ClosedTestEvidence`;
- 18 EditMode tests and a 36-check Editor self-test.

## Privacy / security boundary

The retained physical-device schema contains no device model, serial, Android ID, advertising ID, IMEI, tester identity, email, or free-form logs. The tooling contains no network client, Play API, build invocation, keystore password, or signing-secret field.

## Freeze gate

A candidate can freeze only after all of the following are true for the same candidate bytes:

1. Unity compile, Step 5 self-test, EditMode, and PlayMode evidence are retained.
2. Exactly one signed `.aab` is staged and passes Step 4 artifact validation.
3. All five required physical-device profiles pass against that exact AAB SHA-256.
4. Packages lock and ProjectSettings reviews are complete.
5. Store release checklist and Privacy/Data safety are approved.
6. Production application ID, versioning, signing, and controlled-test build authorization are approved.
7. Candidate freeze receives separate explicit authorization.

Closed-test upload and Play publishing remain separate later authorizations.
