# v1.18 Step 6 — Closed Test Upload, Go/No-Go, and Rollout Readiness

## Purpose

Define the final fail-closed release decision boundary after candidate-freeze work, without pretending that Play Console, tester-cohort, production-access, or rollout evidence exists when it has not been retained.

## Three separate decisions

### 1. Prepare / roll out a closed-test release

Preparation requires a verified candidate-freeze manifest and matching artifact hash, completed Play app setup, testing-track release permission, configured closed track and tester access, reviewed country availability, approved release notes, and no outstanding release. Actual closed-test rollout additionally requires two explicit approvals: upload authorization and closed-test publishing authorization.

### 2. Closed-test go/no-go and production-access readiness

Retained upload evidence must describe the exact frozen AAB and a release accepted by Play. Health evidence must be tied to the same artifact SHA-256, include at least one observed test day and tester, contain no blocking crash/ANR/critical bug/blocking feedback, and show that feedback and the core loop were reviewed.

The release must be `AvailableToTesters` before production-access readiness can pass.

For a personal developer account created after 2023-11-13, production-access readiness additionally requires at least 12 continuously opted-in testers for at least 14 continuous days. Because the account type is external account information, the shipped config leaves `developerAccountTypeReviewed=false` and does not infer it.

### 3. Production rollout readiness

Production rollout is a later, separate decision. It requires production access, the store release checklist, Privacy/Data safety, content rating, target audience, ads and app-access declarations, rollback and monitoring plans, managed-publishing review, no outstanding release, go/no-go authorization, production rollout authorization, and store-publishing authorization.

A first production release must use 100%; staged rollout is allowed only for later app updates. The foundation validates this distinction but performs no rollout.

## Evidence files

Templates are supplied under `ProjectDocs/ClosedTestEvidence`:

- `closed_test_upload_evidence.template.json`
- `closed_test_health_evidence.template.json`
- `production_rollout_readiness.template.json`

Actual evidence filenames, when genuinely available, are:

- `closed_test_upload_evidence.json`
- `closed_test_health_evidence.json`

Do not create those files just to satisfy the validator. They represent retained external evidence.

## Privacy-minimal evidence

The schemas intentionally retain only candidate/frozen-artifact hashes, version values, release-state enums, aggregate tester counts/days, aggregate blocker counts, and reviewed boolean gates. Tester identities and account/signing credentials are out of scope.

## Unity verification

1. Open the exact Step 6 candidate in Unity 6000.5.9f1 and confirm zero compile errors.
2. Run `Legends of Rus > Tests > Run v1.18 Step 6 Self-Tests`; expected `42/42`.
3. Run complete EditMode and PlayMode suites.
4. Run `Legends of Rus > QA > Print v1.18 Step 6 Closed Test Go-No-Go`.
5. Optionally write the privacy-minimal local snapshot with `Legends of Rus > QA > Write v1.18 Step 6 Sanitized Rollout Snapshot`.
6. Keep all evidence/authorization flags false until the matching real evidence or operator approval exists.
