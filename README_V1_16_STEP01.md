# Legends of Rus — v1.16 Step 1

v1.16 Step 1 adds a detached, provider-neutral foundation for analytics,
remote configuration, consent, in-app purchases, and rewarded advertisements.
The new code lives in one engine-free `LegendsOfRus.CommercialCore` assembly so
its validation and failure behavior can be tested without a device, network,
store, advertising provider, analytics provider, or Unity scene.

This is a source foundation, not live commercial activation. Nothing in Step 1
registers with `RuntimeBootstrap`, replaces the existing `AnalyticsService` or
`MonetizationService`, changes a screen route, writes a save, grants currency or
inventory, or contacts an external service. The existing development purchase
and rewarded-ad methods remain inherited prototype behavior and are not claimed
safe for production.

The foundation provides:

- a typed, allowlisted analytics schema, deterministic validation and
  serialization, a bounded memory-only adapter, a disabled default, and
  process-local funnel tracking;
- strict, immutable remote-config snapshots bound to the reviewed economy
  proposal identity, with a conservative all-disabled fallback and a detached
  local deterministic provider;
- immutable consent snapshots, strict audience and policy validation, disabled
  and explicit in-memory development providers, and independent eligibility
  decisions that also require external-services activation;
- provider-neutral IAP and rewarded-ad interfaces, exact product and placement
  allowlists, unavailable defaults, explicit scripted development adapters and
  verifiers, and a process-local verified-entitlement reservation gate; and
- reason-coded failures that return control immediately instead of trapping the
  offline core loop.

The verified-entitlement gate creates only a detached reservation. It contains
no balance amount, reward amount, save adapter, or grant callback. A purchase or
ad result cannot mutate game state through this assembly.

The gate keeps one purchase verifier and one rewarded-ad verifier as immutable
constructor bindings. Neither reservation API accepts a caller-selected
verifier, and a gate missing either binding is inoperable. Step 1 provides no
production verifier or trusted runtime composition authority: it proves
contract and identity consistency for tests, not store-receipt or ad-proof
authenticity. Only a later reviewed integration may construct the gate with
approved production verifiers.

The four purchase pairs merely close over the current legacy UI ids for later
migration; they define no reward recipes. Rewarded placements cover only energy
and Victory reward doubling. The detached chapter-finale presentation switch has
no Step 1 product mapping, and no new chapter-finale product is invented.

## Preserved v1.15 boundary

The v1.15 Step 2 economy activation checkpoint is still incomplete. The economy
catalog remains proposal-only and non-authoritative, and the activation manifest
continues to record these values as `false`:

- `compilationVerified`
- `editorSelfTestsPassed`
- `editModeTestsPassed`
- `playModeTestsPassed`
- `runtimeActivationApproved`

Save version 6, `LegendsOfRus_Save_v1`, existing balances and inventory, all
approved PNGs, current runtime routes, and existing project packages are
unchanged.

## Required Unity verification

Unity execution is not available in the source-packaging environment. Open this
folder directly in exactly Unity 6000.5.9f1 and complete the following before
any live v1.16 integration is reviewed:

1. Allow the project to import and compile with zero Console errors.
2. Run `Legends of Rus > Validate Commercial Vertical Slice`.
3. Run `Legends of Rus > Tests > Run v1.16 Step 1 Self-Tests`.
4. Run every EditMode test in Unity Test Runner.
5. Run every PlayMode test in Unity Test Runner.
6. Retain the Console report and both Test Runner result XML files.

Passing v1.16 tests does not change any v1.15 activation flag. Those flags can
only be updated by their own retained-evidence and approval workflow.

Implementation details are in
`Assets/LegendsOfRus/Documentation/V1_16_STEP01_COMMERCIAL_ADAPTERS_SANDBOX_FOUNDATION.md`.
