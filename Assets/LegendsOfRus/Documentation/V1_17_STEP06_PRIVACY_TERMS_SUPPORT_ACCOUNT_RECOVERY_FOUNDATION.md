# v1.17 Step 6 — Privacy, Terms, Support, and Account Recovery Foundation

## Status

`IMPLEMENTED_IN_SOURCE_PENDING_UNITY_AND_PRODUCT_LEGAL_VERIFICATION`

This checkpoint adds the local data/security boundaries required before any
production consent, account, support, or recovery UI/provider can be approved.
It deliberately does **not** claim that final legal copy, regional policy,
account backends, deletion endpoints, or store declarations are complete.

The Step 5 baseline used here includes both accepted compile hotfixes:

- `com.unity.addressables` is pinned to `2.8.1` for Unity 6000.5 compatibility;
- the general PlayMode test asmdef directly references
  `LegendsOfRus.ContentDeliveryCore`.

## Engine-free compliance core

`LegendsOfRus.ComplianceCore` has no UnityEngine references and defines:

- explicit `Unknown`, `Denied`, and `Granted` optional-consent states;
- versioned Privacy Policy and Terms acceptance records;
- strict legal-version token validation;
- current-document checks before analytics or ads can be authorized;
- a 32-character lower-hex local support-reference contract;
- strict future recovery-code format validation;
- a hard rule that recovery codes may never be persisted;
- support-export text bounds and control-character rejection.

The gameplay save payload remains **save v6**. Step 6 intentionally stores its
small compliance record under a separate PlayerPrefs namespace
`LegendsOfRus_Compliance_v1` so the already-tested save migration/recovery
journal is not silently expanded or version-bumped during Android-readiness
work.

## Runtime privacy/support service

`PrivacySupportService` is installed on the existing persistent runtime root.
The shipped activation config permits only:

- local compliance-state persistence;
- explicit local construction of a support snapshot.

It keeps all of the following false:

- final legal documents approved;
- legal-acceptance UI approved;
- support UI approved;
- account-recovery UI approved;
- account-recovery backend approved;
- data-deletion backend approved;
- production account services approved.

Therefore a player cannot accidentally create a production-looking legal
acceptance against placeholder copy. Navigation to those future screens fails
closed until the locked visual set gains reviewed Privacy/Terms/Support/Recovery
screens and final product/legal text is supplied.

## Analytics and rewarded-ad consent boundary

The underlying typed `AnalyticsService` remains directly testable, but actual
runtime call sites now use `PrivacyAwareAnalytics`. It returns the existing
`ANALYTICS_CAPTURE_NOT_ALLOWED` result unless the local compliance state has:

1. current Privacy and Terms acceptances, and
2. explicit analytics consent = `Granted`, and
3. final legal documents approved by activation.

Rewarded-ad methods in `MonetizationService` similarly fail closed with
`MONETIZATION_AD_CONSENT_REQUIRED` unless the ads-consent boundary is satisfied.
Purchases are not conflated with advertising consent; existing commercial/IAP
activation gates remain false.

## Support export

Support data is generated only when `TryBuildSupportExport` is explicitly
called. It is not uploaded, queued, emailed, copied to clipboard, or persisted
as a diagnostic report by this checkpoint.

The export DTO is a fixed allowlist containing:

- local support reference;
- application and Unity version;
- platform;
- device model and operating system;
- system/graphics memory sizes;
- coarse save-load status;
- content-catalog readiness and local Addressables activation state.

It deliberately excludes:

- `SystemInfo.deviceUniqueIdentifier`;
- advertising identifiers;
- email/name/account ID;
- passwords/secrets;
- save payload contents;
- recovery codes;
- auth/session tokens;
- free-form error/stack/user text.

## Account recovery and deletion

Step 6 defines format and UI/backend boundaries only. A candidate recovery code
is validated in memory and then discarded. It is never logged, hashed, copied,
serialized, or saved. No recovery network transport is compiled.

Account deletion similarly returns a stable `DELETION_BACKEND_UNAVAILABLE`
result. Local compliance state can be explicitly reset, but that operation is
not represented as server/account deletion.

## UI/art boundary

No new production screen art is invented in this checkpoint. The approved game
uses a locked historical Russian fairy-tale visual language, and the existing
asset set does not contain final Privacy, Terms, Support, Account Recovery, or
Data Deletion screens. Step 6 therefore supplies fail-closed navigation/data
contracts while leaving those production screens unapproved rather than adding
visually inconsistent temporary UI.

## Unity verification

1. Open the exact candidate in Unity `6000.5.9f1`.
2. Confirm Package Manager resolves Addressables `2.8.1` and compilation reaches
   **0 errors**.
3. Run `Legends of Rus > Validate Commercial Vertical Slice`.
4. Run `Legends of Rus > Tests > Run v1.17 Step 6 Self-Tests` and expect
   **20/20**.
5. Run the complete EditMode and PlayMode suites, including the new
   `LegendsOfRus.ComplianceCore.Tests` and
   `LegendsOfRus.ComplianceRuntime.PlayModeTests` assemblies.
6. Verify Home → Pre-Level → Gameplay → Victory/Fail → Home is unchanged.
7. Confirm analytics remains blocked and rewarded ads remain unavailable with
   the shipped Step 6 compliance state/config.
8. In a disposable profile, deny analytics consent through the test/API path,
   recreate the service, and confirm the denial persists without changing the
   gameplay save journal.
9. Build a local support export and confirm it contains only the documented
   allowlist and no unique device identifier, credentials, save values, or
   recovery code.
10. Retain Unity/Test Runner evidence before treating Step 6 as runtime-verified.

## Hard stops

This checkpoint does not authorize final legal copy, production consent UI,
regional/age policy, identity SDKs, cloud save, account recovery, account
deletion, backend support upload, remote content delivery, production analytics,
production ads/IAP, Android production signing/AAB, Play Console Data safety
submission, or publishing.
