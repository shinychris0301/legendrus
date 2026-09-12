# v1.18 Step 4 — Controlled Test Build and Release Candidate Foundation

## Status
Implemented in source; Unity/Test Runner/device/signing/AAB/Play evidence remains pending for these exact bytes.

## Current Google Play technical snapshot — 2026-09-09
- New apps and updates submitted after 2026-08-31 must target Android 16 / API 36 or higher.
- New Google Play apps publish with Android App Bundles (AAB).
- Native-code bundles require 64-bit support and must be compatible with 16 KB memory page sizes.
- Android developer verification/package registration requirements become effective 2026-09-30; Step 4 surfaces this as a review warning rather than pretending account-level Play Console evidence exists.

## Implemented
- Engine-free controlled-test build preflight/evidence model.
- Three independent gates: release-candidate plan, signed AAB creation, closed-test upload.
- AAB artifact evidence validator for filename, SHA-256, size, version, API level, signing, ARM64 and 16 KB evidence.
- Local sanitized Unity Editor preflight snapshot; no device/account/signing identifiers.
- Dedicated EditMode tests and Editor self-test.

## Deliberately blocked
- Automatic PlayerSettings mutation.
- BuildPipeline.BuildPlayer invocation.
- Keystore/password/account credential discovery.
- Production application identifier/version approval.
- Play App Signing/upload-key workflow approval.
- Signed AAB creation.
- Closed-test upload and Play publishing.

These gates stay blocked until concrete external inputs/evidence are reviewed and explicit authorization is given.
