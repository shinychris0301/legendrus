# v1.18 Step 8 — Production Access Decision and Launch Candidate Readiness

This checkpoint converts the production-access outcome and frozen closed-test candidate into a fail-closed launch-readiness decision. It does not upload, publish, start a rollout, access Play credentials, or read signing secrets.

## Evidence layers

1. `production_access_decision.json` — reviewed evidence that production access is granted when required, or explicitly reviewed as not required for the account.
2. `CANDIDATE_FREEZE_MANIFEST.json` + SHA-256 — exact frozen candidate and signed-AAB boundary from Step 5.
3. `launch_candidate_readiness.json` — reviewed launch evidence for store/legal declarations, app identity/versioning, Play App Signing/upload-key workflow, API 36+, AAB/ARM64/16 KB verification, release notes, countries/regions, support, monitoring, rollback, and release history.

## Launch decision layers

- **Human launch review ready** requires all external and technical evidence, but not final publication authorization.
- **Production release draft ready** uses the same evidence boundary and remains a manual Play Console activity.
- **Manual production rollout allowed** additionally requires explicit production-release authorization and store-publishing authorization.

## Rollout rule

For the app's first production release, Step 8 requires a 100% production rollout plan because Google Play staged rollout is for app updates, not first publication. For later updates, a reviewed rollout percentage from 1–100% is accepted.

## Privacy and credential boundary

The Step 8 schema excludes tester identity, device identity, Play account credentials, service-account keys, keystore passwords, unrestricted feedback text, and publishing tokens. The runner contains no network client and no `BuildPipeline.BuildPlayer` invocation.
