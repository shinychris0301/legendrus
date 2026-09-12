# v1.20 Step 2 — Canonical Chapter Package Generator and Chapter 1 Non-Destructive Migration Dry Run

## Status
Implemented in source; Unity 6000.5.9f1 compile/Test Runner execution pending for these exact bytes.

## Purpose
Prove that Chapter 1 can enter the canonical v1.20 content pipeline without changing runtime behavior or approved presentation assets.

## Implemented
- Deterministic Chapter 1 staging generator under `ProjectDocs/ContentPipeline/Staging/CH01`.
- Byte-for-byte copies of legacy level, story, and Russian localization payloads.
- SHA-256 source/staged equivalence evidence for every copied payload.
- Canonical `chapter_definition.json` derived from legacy Chapter 1 metadata.
- Canonical package manifest and migration dry-run report.
- Engine-free dry-run validation types/policy/validator.
- 50 dedicated EditMode dry-run tests.
- 80-check Editor self-test.
- Hotfix 05 Step18 string-literal repair retained.

## Deliberately not implemented
- No runtime loader cutover.
- No move from Resources.
- No Addressables mutation.
- No approved-art changes.
- No screenshot/UI replacement yet.
- No level/reward/progression/localization rewrite.

## Unity verification
1. Confirm 0 compile errors.
2. Run `Legends of Rus > Content Pipeline > Generate v1.20 Step 2 Chapter 1 Dry Run`.
3. Run `Legends of Rus > Content Pipeline > Validate v1.20 Step 2 Chapter 1 Dry Run`.
4. Run `Legends of Rus > Tests > Run v1.20 Step 2 Self-Tests` and expect `80/80`.
5. Run complete EditMode and PlayMode suites.

## Revised next checkpoint
**v1.20 Step 3 — Production UI Asset Reconstruction and Screenshot Removal.**
