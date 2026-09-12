# v1.18 Step 6 — Closed Test Upload, Go/No-Go, and Rollout Readiness

## Status

Implemented in source. Unity 6000.5.9f1 compilation and Test Runner execution for these exact bytes remain pending. No Play Console upload, closed-test rollout, production-access application, or production rollout is performed by this source package.

## What this checkpoint adds

- engine-free `LegendsOfRus.ClosedTestRolloutCore`;
- fail-closed closed-test release preparation and rollout readiness evaluation;
- privacy-minimal closed-test upload evidence validation;
- privacy-minimal closed-test health evidence validation;
- explicit go/no-go gate after a release is available to testers;
- conditional production-access readiness for personal accounts created after 2023-11-13;
- first-production-release protection against staged rollout percentages;
- update-release staged-rollout readiness support;
- rollback, monitoring, managed-publishing, Play declaration, and outstanding-release gates;
- sanitized local readiness snapshot generation;
- 32 EditMode tests and a 42-check Editor self-test.

## Current Google Play policy snapshot

Policy assumptions in this foundation were rechecked on 2026-09-09 against official Play Console Help:

- closed testing uses a managed tester cohort and opt-in flow;
- developers need permission to release apps to testing tracks;
- new personal developer accounts created after 2023-11-13 must complete a closed test with at least 12 testers continuously opted in for at least 14 days before applying for production access;
- staged rollouts are available for app updates, not the first production release;
- outstanding releases must be resolved before starting another release.

The account-type rule is conditional. The source package does not assume the developer account type and keeps that review gate false.

## Security / privacy boundary

No tester emails, Google Group membership, opt-in URLs, Play credentials, service-account keys, keystore secrets, device identifiers, or free-form logs are stored by the Step 6 schemas. No `BuildPipeline.BuildPlayer`, Play API, HTTP client, upload endpoint, rollout action, halt/resume action, or credential access exists in Step 6.

## Shipped state

All local validators are enabled. All evidence and production-sensitive authorizations remain false, including candidate-freeze retention, closed-test upload, closed-test publishing, go/no-go, production access, production rollout, and store publishing.
