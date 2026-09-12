# v1.15 Step 2 — Runtime Economy Transactions and Energy Integration

## Delivery boundary

Step 2 implements a reviewable runtime-economy foundation in source. It does
not activate that foundation in the current player flow.

Present in source:

- an atomic transaction engine for the supported wallet, energy, and booster
  resources;
- detached snapshots and copy-on-write candidate coordination so rejected work
  cannot partially alter caller-owned state;
- a frozen legacy fallback policy;
- an immutable catalog-derived policy that excludes starting balances;
- a fail-closed activation gate bound to a catalog id, schema, normalized
  source SHA-256, manifest identity, and exact Unity version; and
- source-level EditMode and PlayMode coverage for transaction and gate
  contracts.

Not connected to the source foundation:

- `GameState` balance, energy, daily, reward, and booster mutations;
- `ScreenRouter` level entry, reward, shop-mock, mail, and retry routes;
- `Match3Board` booster debit and recovery routes; and
- `RuntimeBootstrap` registration or policy selection.

Those live files remain unchanged from Step 1. Existing prototype behavior and
known hazards therefore remain until an explicitly reviewed activation-wiring
change is implemented and verified. Merely loading or evaluating the catalog,
manifest, policy, engine, or detached coordinator cannot update a player save.

Implementation statements in this document mean present in source. They do not
claim Unity compilation, Editor execution, Test Runner execution, device
execution, or runtime activation.

## Atomic transaction contract

The pure transaction engine accepts one stable transaction key, a detached
snapshot, costs, and grants. It stages all supported resource changes on a
candidate copy and publishes the complete candidate only after the full request
passes validation.

The supported resources are gold, gems, energy, Bogatyr Mace, Firebird, Living
Light, and Sea Pearl. The engine:

- rejects null or invalid snapshots and requests;
- rejects empty or unstable keys, unknown resources, negative amounts,
  duplicate resource entries, insufficient balances, and overflow;
- caps an energy grant at the supplied capacity;
- leaves the prior snapshot unchanged after every rejected request;
- records a successful transaction key process-locally and rejects its replay;
  and
- leaves failed keys retryable.

Process-local transaction keys are not represented as durable idempotency.
Persistent one-shot behavior still belongs to existing domain state such as
first-clear reward markers and daily claim dates. Step 2 adds no transaction
history to the save schema and makes no claim that a process-local key survives
a restart.

The detached copy-on-write coordinator provides a candidate boundary for later
runtime integration. It does not register itself with the current game and does
not write `PlayerPrefs`. An activation change must still define and verify the
single-commit save adapter and stable domain keys for each live call site.

The detached entry contract authorizes a zero-cost retry only after the same
coordinator instance has successfully staged an attempt for that level and the
supplied candidate contains that attempt. This process-local entitlement is
deliberately fail-closed after a restart; live wiring must add a reviewed
domain-session lifecycle before activation rather than infer a free retry from
caller input alone. Normal and retry entry both reject clock rollback.

First-clear grants require an unlocked level with existing completion evidence.
Their amounts are frozen from the SHA-bound economy proposal when the policy is
built; mutable or fallback `ChapterLevelCatalog` reward fields are never used as
the grant source. A missing policy reward fails closed.

## Immutable policy and fail-closed gate

The Step 1 catalog remains a proposal:

```json
{
  "proposalOnly": true,
  "authoritative": false
}
```

The gate never converts that proposal into a usable catalog policy unless all
of these bindings and approvals match:

1. the proposal passes catalog validation;
2. the detached catalog exactly matches the supplied source JSON;
3. catalog schema and id match the supported immutable identity;
4. the normalized source SHA-256 matches both source and manifest bindings;
5. the manifest schema and id match;
6. the Editor is exactly Unity 6000.5.9f1;
7. product approval is recorded;
8. compilation, Editor self-tests, EditMode tests, and PlayMode tests are each
   recorded as passed; and
9. final runtime activation is explicitly approved.

Failure at any boundary returns a reason-coded legacy fallback. Evaluation has
no registration, logging, save, `PlayerPrefs`, or `GameState` side effect. The
catalog-derived policy contains operational energy, daily-reward,
reward-doubling, and first-clear progression values; it intentionally excludes
starting balances and cannot authorize an existing-save rebase.

## Current activation manifest

The shipped manifest represents the current evidence honestly:

| Field | Value | Meaning |
| --- | --- | --- |
| `productApproval` | `true` | Step 2 foundation implementation was requested. |
| `compilationVerified` | `false` | Unity compilation has not run here. |
| `editorSelfTestsPassed` | `false` | Editor self-tests have not run here. |
| `editModeTestsPassed` | `false` | EditMode Test Runner has not run here. |
| `playModeTestsPassed` | `false` | PlayMode Test Runner has not run here. |
| `runtimeActivationApproved` | `false` | No live wiring or runtime authority is approved. |

The verification flags are evidence records, not switches to set in advance.
The final approval flag must remain false until the live call-site wiring is
reviewed, implemented, and reverified.

## Save and compatibility boundary

Step 2 preserves:

- `PlayerSaveData.saveVersion = 6`;
- the PlayerPrefs key `LegendsOfRus_Save_v1`;
- all existing save fields and serialized meanings;
- current player balances, energy, booster inventory, progression, and claim
  markers on load;
- Chapter 1 authored first-clear rewards; and
- the current UI routes and presentation.

There is no schema addition, migration, balance rebase, automatic grant, or
catalog-version rewrite. Policy activation, when approved later, must consume
the current save as its opening snapshot rather than replacing it with proposal
starting values.

## Deferred live hazards

Because production call sites remain unchanged, Step 2 does not claim to have
fixed split energy-debit/attempt persistence, split booster debit/refund,
unchecked inherited `int` arithmetic, repeatable development mail or pack
grants, unknown development pack behavior, wall-clock reward edge cases, or
the lack of durable transaction receipts. These are activation-wiring
acceptance items, not completed behavior in this inactive foundation.

Store/IAP, rewarded-ad, analytics, backend/cloud, and remote-config SDKs are
not added. Development monetization mocks remain development mocks and are not
promoted to verified purchase or entitlement flows.

## Exact Unity 6000.5.9f1 activation workflow

1. Open this project directly in Unity 6000.5.9f1 and retain a clean Console
   capture showing compilation completed with zero errors.
2. Run `Legends of Rus > Validate Commercial Vertical Slice` and retain its
   passing report.
3. Run the v1.15 Step 2 Editor self-tests from `Legends of Rus > Tests` and
   retain the passing report.
4. Run all EditMode tests in Unity Test Runner and retain the result XML.
5. Run all PlayMode tests in Unity Test Runner and retain the result XML.
6. Review the retained evidence, then change only the corresponding four
   verification flags from false to true. A failed or skipped suite keeps its
   flag false.
7. Review a narrowly scoped live-wiring change for `GameState`,
   `ScreenRouter`, `Match3Board`, and `RuntimeBootstrap`. It must define stable
   domain keys, one candidate save commit, failure recovery, and legacy-save
   behavior while preserving save version 6 and the existing key.
8. Keep `runtimeActivationApproved` false while that wiring is implemented and
   tested. Re-run steps 1–5 after the wiring change.
9. Obtain explicit final runtime-activation approval. Only then set
   `runtimeActivationApproved` true in the reviewed release candidate.
10. Re-run the complete verification set once more after the manifest change;
    reject the release if the gate does not return the intended policy or if
    any live economy flow regresses.

Unity is unavailable in the source-packaging environment, so none of steps
1–10 is reported as complete by this milestone artifact.
