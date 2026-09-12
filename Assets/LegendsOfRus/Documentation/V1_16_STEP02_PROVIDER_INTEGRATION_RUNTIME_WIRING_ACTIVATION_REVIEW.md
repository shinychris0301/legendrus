# v1.16 Step 2 — Provider Integration, Runtime Wiring, and Activation Review

## Decision

Implementation status:
`IMPLEMENTED_IN_SOURCE_PENDING_PROVIDER_LEGAL_AND_ACTIVATION`

Runtime candidate status: `PREACTIVATION_DISABLED`

Activation is not authorized. The source candidate provides reviewed seams and
fail-closed defaults, not a live commercial launch. Every external capability
is disabled by local config, manifest approval fields, the v1.15 economy gate,
disabled provider bindings, and a compile-time activation boundary.

The phrase “implemented in source” does not claim that Unity compiled or
executed this Step 2 candidate. No retained Unity 6000.5.9f1 Console log,
validator report, Editor self-test result, EditMode XML, or PlayMode XML is
included. Those evidence fields are `NOT_RETAINED`.

## Runtime topology

`LegendsOfRus.CommercialRuntime` is the only new composition assembly. It
depends on `CommercialCore`, `EconomyCore`, `Runtime`, and `RuntimeEconomy`.
Existing domain assemblies do not depend back on it. Narrow friend access lets
this reviewed composition install the disabled typed-analytics bridge without
making the installer or monetization binder public.

Boot produces an immutable diagnostic snapshot and does not create a GameObject,
touch `GameState`, call `SaveService`, access `PlayerPrefs`, or contact an
external service. Exceptions publish `SafeDisabled` and leave the offline game
usable.

## Activation layers

| Layer | Shipped state | Authority |
|---|---|---|
| v1.15 runtime economy | Inactive | `economy_runtime_activation.json`; all verification/runtime approval fields remain false |
| commercial local config | Disabled | Exact resource id and SHA-256; all providers and capability flags are `disabled`/false |
| remote config | Proposal-only | Exact bundled snapshot and content hashes; may only further disable |
| consent | Restricted | Disabled provider; unknown, child, denied, missing, or failure cannot enable capture/ads |
| provider bindings | Unavailable | Disabled analytics, IAP, ad, purchase-verifier, and ad-verifier implementations |
| commercial manifest | Preactivation | All retained-evidence, review, capability, migration, durable-fulfillment, and final approval fields false |
| build switch | Disabled | `ExternalProviderActivationCompiled == false` |

For the shipped bytes, the expected decision is
`VerificationRequired / COMMERCIAL_V115_ECONOMY_INACTIVE`. Even a valid remote
snapshot and consent grant cannot bypass the economy, manifest, provider, or
build boundaries. Editing either pinned JSON resource changes its SHA-256 and
fails closed.

## Remote-config behavior

`RuntimeRemoteConfigCache` is process-local. Before the first accepted snapshot,
missing, throwing, invalid, authoritative, unbound, or hash-mismatched input
uses `ConservativeFallback`. After a valid bundled snapshot is accepted, a
failed refresh returns the exact prior last-known-good snapshot atomically.

The snapshot cannot grant consent, provider selection, v1.15 economy activation,
save migration, durable fulfillment, or final activation. It cannot change a
reward amount, product identity, or placement identity. No network provider is
present.

## Analytics wiring and privacy boundary

Runtime analytics now uses typed methods only. The legacy free-form event/logger
path is not used for upload. `AnalyticsService` accepts an allowlisted `ScreenId`
or typed domain arguments; the bridge constructs registry-bound drafts. It does
not accept an arbitrary dictionary, receipt, proof, transaction id, account id,
device/ad id, exception text, or persistent player id.

The session id is process-local, events have monotonic sequence/timestamp
ordering, and the shipped bridge is capture-disabled. The offline adapter used
by tests has a bounded in-memory trace and declares no upload capability. The
runtime bridge exposes no flush or upload method.

Level start and terminal transitions are callback-atomic: a null, rejecting, or
throwing adapter cannot partially change funnel or replay state. Gameplay screen
view is emitted before level start. Pause restart/exit cancellation emits no
false terminal event, retains the used attempt key, and lets a distinct attempt
start. Pause, Out of Energy, and Booster Select modal views use exact allowlisted
screen identities.

## Commercial UI and failure paths

Shop purchases, rewarded energy, and Victory double actions call the
`MonetizationService` facade. The facade defaults to an unavailable internal
port. Its binder, port interface, candidate result factory, and committed result
factory are non-public; exceptions and null results become stable unavailable
outcomes.

The inherited `MockPurchase`, `MockRewardedEnergy`, and
`MockRewardedContinue` methods remain compatibility wrappers only. They no
longer mutate a save. An unknown pack no longer defaults to 50 gems. Victory no
longer grants a double before an ad/verification result. Mail Claim All and Ask
Friends energy are explicitly unavailable and dismissible rather than
repeatable prototype grants.

Every unavailable path returns control to the player, shows non-blocking UI
feedback where applicable, and leaves commercial state unchanged.

## Fulfillment boundary

Commercial adapters and verification evidence never supply a reward amount.
`CommercialEntitlementRecipeCatalog` is a closed mapping to an already reviewed
v1.15 economy operation:

| Commercial identity | Step 2 outcome |
|---|---|
| starter IAP | `COMMERCIAL_FULFILLMENT_RECIPE_UNAVAILABLE` |
| hero IAP | `COMMERCIAL_FULFILLMENT_RECIPE_UNAVAILABLE` |
| firebird IAP | `COMMERCIAL_FULFILLMENT_RECIPE_UNAVAILABLE` |
| energy-refill IAP | `COMMERCIAL_FULFILLMENT_RECIPE_UNAVAILABLE` |
| rewarded energy | `COMMERCIAL_FULFILLMENT_RECIPE_UNAVAILABLE` |
| rewarded continue/unknown placement | rejected as unknown |
| Victory reward double | verified, detached candidate only |

The Victory candidate is created from a fresh detached v1.15 coordinator. It
contains the exact catalog-defined bonus and doubled marker while the source
save remains byte-for-byte unchanged. The durable-marker proposal and source
fingerprint are SHA-256 identities; raw receipts, proof payloads, and reward
tokens are not exposed by the candidate.

A candidate is not a grant. It always reports `IsPersisted == false` and
`IsEntitlementCompleted == false`. Step 2 releases the process-local entitlement
reservation and never calls `VerifiedEntitlementGate.Complete`.

Save v6 has no durable commercial fulfillment history. The future atomic-store
request, result, interface, and unavailable implementation are internal, and no
success constructor exists. Until a reviewed save migration can atomically
commit candidate balances and a durable replay marker in one compare-and-swap
operation, live fulfillment must remain disabled.

## Verification inventory

The Step 2 source adds isolated EditMode and PlayMode assemblies. Coverage
includes:

- pinned local config and activation manifest behavior;
- conservative fallback and atomic last-known-good retention;
- inactive economy, disabled providers, and compile-time activation boundary;
- typed analytics schema, session order, modal mapping, atomic funnel failure,
  cancellation, and absent transport surface;
- missing IAP/energy recipes, unknown identities, verifier mismatch, durable
  replay seeding, invalid eligibility, overflow rollback, detached copies, and
  reservation release;
- unavailable runtime actions, exception containment, legacy wrapper neutrality,
  save neutrality, and trusted-boundary visibility.

The Editor command
`Legends of Rus/Tests/Run v1.16 Step 2 Self-Tests` contains 29 source check sites.
The exact test/check inventory is reported by the Step 2 static verifier. Counts
found in source are not Test Runner or self-test execution evidence.

Run the static boundary verifier from the project root:

```text
node Tools/verify_v1_16_step02.mjs --source-only
```

For release comparison, supply the immutable v1.16 Step 1 project baseline as
required by the verifier. A release verifier pass proves pinned files,
allowlisted deltas, assembly/static boundaries, PNG preservation, and manifest
inventory; it still does not substitute for retained Unity execution results.

## Unity verification still required

On the exact frozen candidate, retain and bind all of the following before any
activation review:

1. Unity 6000.5.9f1 compilation with zero errors.
2. Project validator output.
3. Step 2 Editor self-test output.
4. EditMode NUnit XML with zero failed, skipped, or inconclusive tests.
5. PlayMode NUnit XML with zero failed, skipped, or inconclusive tests.
6. Provider selection/package/version evidence and failure-path results.
7. Consent, privacy, legal, product, placement, regional, and security reviews.
8. Representative-device tests for offline, cancel, denial, retry, provider
   exception, replay, process restart, and interrupted commit.
9. An approved save migration and durable atomic fulfillment design.
10. Explicit analytics, IAP, rewarded-ad, and final runtime approvals tied to
    the same candidate and evidence hashes.

Any source or resource change after execution invalidates evidence tied to the
prior bytes.

## Release and artifact authority

`ProjectDocs/ARTIFACT_MANIFEST.csv` is an inherited historical source-delivery
record. It is intentionally not regenerated and is not authoritative for this
milestone. For the Step 2 delta, the authoritative file inventory is
`V1_16_STEP02_CHANGED_FILES.txt`; packaged delivery authority additionally
requires the final archive SHA-256 supplied with the release.

Packaging acceptance requires a single project root, no generated Unity cache
directories, no symbolic links, no traversal or case-fold collisions, no native
provider binaries, and byte-identical reconstruction from the reviewed delta.
The extracted full package, and a clean Step 1 baseline after applying the
hotfix package, must each rerun the Step 2 release-boundary verifier
successfully.
