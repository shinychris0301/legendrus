# v1.15 Step 1 — Economy Configuration and Simulation Foundation

## Boundary

Step 1 defines and evaluates proposed economy values. It does not activate
them in the player runtime. The current `GameState`, `SaveService`, UI routes,
development monetization mocks, save key, and save schema remain authoritative
for the existing vertical slice and are not mutated by catalog load,
validation, comparison, or simulation.

Implementation statements in this document mean present in source and covered
by static verification. They do not claim Unity compilation, Editor execution,
or Test Runner results.

No store or rewarded-ad SDK, analytics provider, backend, cloud service,
remote-config client, offer redesign, Chapter 2 content, or automatic player
balance migration is part of this milestone.

## Versioned proposal catalog

The catalog is one versioned proposal source for:

- soft and premium currency identifiers;
- starting resources used by simulations;
- energy capacity, level-entry cost, and refill cadence;
- the four released booster identifiers, booster activation inventory costs,
  and purchase-price availability;
- inactive continue price and move-count comparison values;
- reward-source identifiers, grant contents, cadence, and replay policy; and
- Chapter 1 progression inputs consumed by bounded deterministic simulations.

Its authority state is explicit. A valid proposal is not runtime authority, and
catalog version changes do not alter an existing save.

Catalog validation rejects malformed versions, duplicate identifiers, missing
references, unsupported reward or cost resources, invalid cadence, negative
amounts, invalid capacities, and arithmetic that cannot be represented safely.

No booster purchase price is defined in the Step 1 proposal. Purchase prices
must not be guessed from starting inventory, level requirements, reward grants,
or development shop packs. Consequently, booster purchase affordability is
explicitly `INCOMPLETE` with price unavailable; only activation inventory
requirements are evaluated.

## Prototype provenance

The inherited prototype has multiple value sources. Reports must preserve their
provenance instead of treating the legacy economy JSON as active runtime data:

| Provenance | Meaning |
| --- | --- |
| `declared_unwired_config` | A value declared by the inherited economy JSON but not consumed by the current runtime. |
| `runtime_hardcode` | A value currently enforced by `GameState`, `ScreenRouter`, or a development-only monetization mock. |
| `authored_level_data` | A first-clear reward authored in the Chapter 1 level catalog. |

The prototype snapshot includes the current five-energy capacity, one-energy
entry cost, 30-minute refill cadence, 20-gem continue declaration, five rewarded
continue moves, starting balances and booster inventory, daily and mail rewards,
development pack grants, the absence of booster purchase prices, and all ten
Chapter 1 first-clear rewards. A comparison is evidence only; it cannot write
those values back to gameplay or saves.

## Deterministic ledger and simulations

The offline ledger applies earn and spend transactions atomically. Unknown
resources, invalid quantities, insufficient balances, duplicate grant keys,
and overflow are rejected without partial mutation. Repeating an idempotent
grant cannot award it twice.

Time is supplied as explicit input. Refill and cadence evaluation must not read
the wall clock internally. The same catalog, scenario, and explicit tick inputs
therefore produce the same ordered events, balances, violations, and report.

Required scenario coverage includes:

- representative earn-spend loops;
- exact energy refill boundaries, multi-interval refill, capacity, and clock
  rollback;
- repeatable and one-shot reward cadence;
- Chapter 1 progression affordability; and
- a no-store/no-ad path using starting resources, authored rewards, and natural
  refill within a declared bounded horizon.

Guardrails report stable machine-readable violations for negative balance,
duplicate grant, invalid identifiers or references, invalid amount, arithmetic
overflow, unreachable progression, and a configuration that forces a purchase.
Invalid fixtures must demonstrate that each guardrail fails closed.

## Non-mutation guarantee

Step 1 has no authority to:

- read or write `PlayerPrefs` as part of validation or simulation;
- call player grant, spend, progression, daily-claim, or save methods;
- increment the save version or reinterpret an existing player balance;
- change Chapter 1 reward data, level difficulty, navigation, or UI copy; or
- dispatch store, ad, analytics, backend, or remote-config work.

The proposal becomes eligible for runtime integration only after its reports
are reviewed and a later milestone is explicitly approved.

## Verification status

Static verification covers project JSON parsing, C# structural scans, proposal
and deterministic-report contracts, source-level guardrails, protected hashes
against v1.14 Step 4, and the exact release delta.

Static verification does not prove Unity compilation or Unity test execution.
Unity 6000.5.9f1 compilation, Editor self-tests, and EditMode/PlayMode Test
Runner execution remain pending in this environment.
