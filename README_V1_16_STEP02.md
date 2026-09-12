# Legends of Rus — v1.16 Step 2

## Provider Integration, Runtime Wiring, and Activation Review

Status: `IMPLEMENTED_IN_SOURCE_PENDING_PROVIDER_LEGAL_AND_ACTIVATION`

Runtime candidate status: `PREACTIVATION_DISABLED`

v1.16 Step 2 connects the detached Step 1 commercial contracts to a trusted,
fail-closed runtime boundary. No production provider, SDK, endpoint, credential,
store product registration, or ad-unit identifier is included. Every shipped
provider and capability remains disabled, and the normal offline game remains
available when commercial actions are unavailable.

### What is wired

- `LegendsOfRus.CommercialRuntime` loads two exact, SHA-256-pinned local
  resources and composes disabled consent, analytics, IAP, ad, and verifier
  implementations.
- The typed analytics facade replaces free-form runtime event logging. It uses
  allowlisted event and screen identities, one process-local session, atomic
  level-funnel transitions, and no upload or persistence surface.
- The bundled remote-config snapshot is proposal-only and non-authoritative. A
  valid snapshot can be retained process-locally as last known good; invalid or
  missing input fails to an all-off fallback and can never grant authority.
- Shop and rewarded-ad UI actions delegate through an unavailable runtime port.
  The compatibility `Mock*` methods remain only as non-granting wrappers.
- Commercial fulfillment is copy-on-write. All four known IAP products and the
  rewarded-energy placement have no approved economy recipe. Only the catalog's
  existing Victory reward-double action can prepare a detached candidate after
  verification.

### What is deliberately unavailable

- analytics upload or a production analytics adapter;
- live IAP or rewarded-ad providers and verifiers;
- any network fetch or provider SDK;
- save mutation from a commercial callback;
- a public runtime-port binder or forgeable committed result;
- durable fulfillment, entitlement completion, or replay history in save v6;
- activation by editing JSON or remote-config flags.

The future atomic-store types are internal and the shipped implementation can
only reject. A candidate reports `IsPersisted == false` and
`IsEntitlementCompleted == false`; its entitlement reservation is released.
Live fulfillment must remain blocked until one reviewed save migration can
commit both candidate balances and a durable marker atomically.

### Current activation decision

The commercial manifest and local config are pinned with every approval and
enablement set to `false`. The build-time provider-activation switch is also
`false`. With the inherited v1.15 runtime-economy manifest still inactive, the
expected decision is:

- phase: `VerificationRequired`
- reason: `COMMERCIAL_V115_ECONOMY_INACTIVE`
- analytics, IAP, rewarded ads, and durable fulfillment: disabled

### Verification supplied in source

- isolated EditMode fixtures for config, activation, remote-config LKG,
  analytics atomicity, fulfillment mapping, copy isolation, replay seeding, and
  the unavailable atomic boundary;
- isolated PlayMode fixtures for bootstrap composition, save-neutral commercial
  actions, disabled defaults, trusted-boundary visibility, exact screen mapping,
  gameplay event order, and cancellation;
- Editor menu command:
  `Legends of Rus/Tests/Run v1.16 Step 2 Self-Tests`;
- static verifier: `node Tools/verify_v1_16_step02.mjs --source-only`.

Source test discovery and self-test check-site counts are inventory only. They
are not proof that Unity executed them. No Step 2 Unity Console log, validator
report, self-test report, EditMode XML, or PlayMode XML is retained in this
candidate, so Unity execution evidence remains `NOT_RETAINED`.

### Required activation review

Before any external capability is activated, retain evidence for Unity
6000.5.9f1 compilation, the project validator, the Step 2 Editor self-test,
EditMode and PlayMode Test Runner runs, provider selection, consent and privacy
review, product/placement registration, security review, regional behavior,
representative-device failure paths, a save migration, and durable atomic
fulfillment. Record explicit capability and final runtime approvals against the
same frozen candidate bytes.

The machine-readable sequence and hard stop are recorded in
`Assets/LegendsOfRus/Documentation/NEXT_CHECKPOINT_V1_16_STEP02_ACTIVATION.json`.

Detailed boundary and verification notes are in
`Assets/LegendsOfRus/Documentation/V1_16_STEP02_PROVIDER_INTEGRATION_RUNTIME_WIRING_ACTIVATION_REVIEW.md`.
