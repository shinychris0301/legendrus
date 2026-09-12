# v1.16 Step 1 — Commercial Adapters and Sandbox Foundation

## Delivery boundary

Step 1 introduces engine-free contracts and deterministic local implementations
for analytics, remote configuration, consent, IAP, and rewarded ads. It is a
detached foundation. No new type is registered with the current player runtime,
and no external provider or SDK is present.

Implementation statements in this document mean present in source. They do not
claim Unity compilation, Editor execution, Test Runner execution, network or
store certification, device execution, or live commercial activation.

## Assembly boundary

`LegendsOfRus.CommercialCore` has no assembly references and sets
`noEngineReferences` to `true`. Its four domain namespaces are:

- `LegendsOfRus.CommercialCore.Analytics`
- `LegendsOfRus.CommercialCore.RemoteConfig`
- `LegendsOfRus.CommercialCore.Monetization`
- `LegendsOfRus.CommercialCore.Consent`

The assembly does not reference Unity, Runtime, EconomyCore, RuntimeEconomy,
GameplayCore, `PlayerPrefs`, `GameState`, `SaveService`, `ScreenRouter`, or
`RuntimeBootstrap`. This prevents the source foundation from mutating live state
or creating a dependency cycle with the inherited
`RuntimeEconomy -> Runtime` assembly edge.

## Analytics contract

Analytics uses a versioned registry of canonical event definitions and typed,
allowlisted attributes. Events are validated before they are offered to an
adapter. Validation and deterministic serialization reject unknown events,
unknown or duplicate attributes, type mismatches, invalid identities and
sequences, and disallowed free-form or sensitive data.

The default adapter is disabled. The offline adapter stores only a bounded,
process-local copy and performs no upload. Funnel state is process-local and a
rejected transition cannot partially change it. Adapter failures are converted
to stable result codes and cannot interrupt gameplay.

This pipeline is not connected to the inherited debug-only
`AnalyticsService`; current runtime events therefore retain their legacy
behavior until a separate wiring review.

## Remote-config contract

Remote config accepts a caller-supplied local JSON string through a deterministic
provider. Parsing is strict: malformed JSON, duplicate or unknown fields,
unsupported schema, invalid identity, incomplete allowlists, type mismatches,
authority claims, unsafe enablement, and economy-binding mismatch fail closed.

The accepted Step 1 snapshot must remain proposal-only and non-authoritative and
must bind to the reviewed v1.15 economy proposal id and SHA-256. Every commercial
enablement remains false. Failure returns one immutable conservative snapshot;
there is no prefix merge, disk persistence, network fetch, background refresh,
or runtime application.

Remote config cannot set or bypass any field in
`economy_runtime_activation.json` and is not a second source of economy
authority.

## Consent contract

Consent snapshots contain purpose decisions and coarse audience state without a
name, date of birth, account identifier, advertising identifier, or provider
identifier. Missing, invalid, future-schema, wrong-policy, unknown-audience, and
child-audience records resolve to restricted behavior.

Analytics and rewarded-ad eligibility are independent decisions. Consent alone
is never sufficient: the requested external capability must also be separately
activated. The production default provider is disabled. The development
provider is explicit, process-local, and unable to bypass validation or external
activation.

## IAP and rewarded-ad contract

IAP and rewarded ads expose provider-neutral request, availability, evidence,
verification, and result types. Exact allowlists bind known offer/product pairs
and rewarded placements. Production defaults report unavailable immediately.

The purchase allowlist is a closed migration record for the four current legacy
UI ids only: `starter`, `hero`, `firebird`, and `energy`, each paired with one
explicit product id. It contains no reward recipe, amount, price, or grant. The
remote-config `starter_offer` and `chapter_finale_offer` switches are detached
presentation proposals; Step 1 does not map the chapter-finale flag to a product
and does not invent a chapter-finale product id. The rewarded allowlist contains
only the existing energy and Victory reward-doubling opportunities. Any broader
offer or placement mapping is deferred and has no runtime authority.

Scripted development adapters require explicit scripts and evidence. A completed
adapter result is not an entitlement. The reservation gate invokes the purchase
or rewarded-ad verifier bound immutably at gate construction and accepts only a
verified, identity-matched result; neither reservation method accepts a
caller-selected verifier, and replaying a reserved key fails. A gate without
both verifier bindings is inoperable and fails with `verifier_unavailable`.
Rejected or failed evidence remains retryable where its reason code allows it.

Step 1 supplies no production verifier and no trusted runtime composition
authority. The pure gate establishes result/identity consistency,
constructor-bound verifier selection, and process-local replay protection; it
does not independently authenticate a store receipt or an ad-provider proof. A
reviewed production integration must construct it with approved verifier
instances behind a trusted composition boundary. Until that boundary exists, a
reservation is test/sandbox evidence, not a production transaction entitlement.

Reservations deliberately contain no reward amount, economy resource, save
operation, or fulfillment callback. Step 1 cannot grant or persist anything.
Durable store receipt reconciliation and one-commit fulfillment require a later
reviewed runtime boundary.

## Offline and failure guarantees

- Analytics, IAP, ads, offers, and the development sandbox are disabled by
  conservative defaults.
- Provider absence, cancellation, rejection, unavailability, malformed evidence,
  and exceptions return reason-coded results without trapping the player.
- No network, file, thread, timer, SDK, credential, endpoint, or cloud dependency
  is added.
- The match-3 loop, story flow, local save, and purchase-free progression remain
  independent of every v1.16 interface.

## Preserved v1.15 activation and save boundary

Step 1 does not complete or bypass the v1.15 Step 2 activation checkpoint. The
economy catalog bytes and activation-manifest bytes are unchanged. Product
approval remains recorded, while compilation, Editor self-test, EditMode,
PlayMode, and runtime-activation flags all remain false.

The following remain unchanged:

- `PlayerSaveData.saveVersion = 6`;
- `LegendsOfRus_Save_v1`;
- existing balance, energy, inventory, progression, and claim meanings;
- the detached v1.15 transaction engine, policy, coordinator, and activation
  gate;
- live `GameState`, `RuntimeBootstrap`, `ScreenRouter`, `Match3Board`,
  `AnalyticsService`, and `MonetizationService` behavior;
- project packages, project settings, scenes, and approved art.

## Deferred work

Step 1 does not include:

- production analytics transport or dashboard configuration;
- network remote config, polling, cache persistence, or backend authority;
- a store, billing, advertising, analytics, consent-management, backend, or
  cloud SDK;
- consent UI, legal copy, privacy policy, regional rules, or account-age flow;
- store product metadata, regional prices, signed-receipt server validation, or
  trusted verifier composition, or durable receipt reconciliation;
- ad-unit identifiers, production availability, or device certification;
- runtime registration, screen wiring, save mutation, economy grants, or
  migration away from legacy development mocks; or
- any change to economy values, story, levels, gameplay, balance, art, or save
  schema.

## Exact Unity 6000.5.9f1 verification workflow

1. Open this project directly in Unity 6000.5.9f1 and retain a clean Console
   capture showing compilation completed with zero errors.
2. Run `Legends of Rus > Validate Commercial Vertical Slice` and retain its
   passing report.
3. Run `Legends of Rus > Tests > Run v1.16 Step 1 Self-Tests` and retain the
   passing report.
4. Run all EditMode tests and retain the Test Runner XML.
5. Run all PlayMode tests and retain the Test Runner XML.
6. Review source, results, consent/legal requirements, store/ad provider choices,
   and the still-pending v1.15 activation checkpoint before proposing any live
   wiring.

Unity is unavailable in the source-packaging environment, so steps 1–5 remain
pending and no runtime activation is claimed.
