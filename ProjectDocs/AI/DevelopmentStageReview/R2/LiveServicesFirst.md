# First milestone: connected commercial and social vertical slice

User selected **live commercial and social features first** on 2026-09-29. This sets the priority; it does not identify a backend, store, auth provider, ad network, published policies, product prices or deployment environment. The August Bible's vendor suggestions are not decisions. Provider clarification was requested; none was supplied when this document was prepared.

## First work package completed

Inventory the current service boundaries and run the existing commercial/runtime checks. See [TestResults.md](TestResults.md). Preserve the native UI, offline game, existing save data and false activation flags. No provider was connected and no real purchase/chat operation was performed during the review.

## Decisions required before provider integration

| Decision | Why it affects implementation | Concrete deliverable |
|---|---|---|
| Target Android store(s), initial territories and audience | Determines available purchase/sign-in/ad options, identifiers, consent and operational requirements | Written launch matrix; current debug package is not automatically the release identity |
| Backend and deployment ownership | Identity, authoritative wallet, ranking, friends and membership share player IDs | One selected backend/environment and non-secret endpoint/config ownership; no parallel authorities |
| Sign-in methods and guest upgrade | Existing progress must survive linking and collisions | Provider list plus account-link/merge/recovery/deletion rules; SMS purpose decided separately |
| IAP and rewarded-ad providers | SDK/ad-verification and store transaction lifecycle differ | Selected adapters and sandbox accounts/config process; approved product/placement IDs |
| Economy | Current offer visuals and old mock values are not grant recipes | One approved starter product, one ad placement, grant quantities, currencies, continuation price/limits and replay policy |
| Social slice | Avoid implementing five unrelated backends simultaneously | Proposed order: account profile → individual ranking + friends → teams + moderated chat; team rewards/mail later |

No secrets should be supplied in chat or checked into the project. Actual service configuration and deployment must use the chosen environment's approved mechanism. This is a required dependency of the requested live work, not an additional permission gate for local implementation.

## Dependency and implementation sequence

| Work package | Reuse / implement | Exit criterion |
|---|---|---|
| LIVE-01 account and authority contract | Existing Login routes, local save/journal, compliance and commercial interfaces. Specify stable player ID, account session, server wallet, content version and recoverable client cache | Conflict cases and guest migration documented; no client-provided balance or sample ranking accepted as server truth |
| CONT-01 resumable failed attempt | Existing board/core/Fail bindings; add suspended-attempt snapshot at a settled board, stable attempt ID, tile/special/obstacle/goal/move/score/booster/RNG state | Same board restored after interruption; decline/retry terminates once; no extra energy or early reward; no paid/ad availability implied |
| LIVE-02 identity and cloud vertical slice | Implement chosen identity adapter/backend and bind existing Login/Profile; preserve local guest play | Two devices sign into one account; link collision, stale save, logout, token expiry, reconnect and recovery preserve progress correctly |
| COM-01 durable entitlement commit | Extend candidate-only fulfillment through one authoritative ledger and recovery protocol; define approved recipes | Verified transaction + grant committed once; duplicates, reordered/late callbacks, failure/restart and wrong account cannot double-grant or lose a paid entitlement |
| COM-02 one purchase + one rewarded placement | Bind selected provider sandboxes into existing Shop/Victory/OutOfEnergy as applicable | Real sandbox purchase/ad verification and recovery; no grant on open/close/cancel/error; localized availability/price/state |
| CONT-02 paid +5 and rewarded continuation | Connect CONT-01 to COM-01 and configured pricing/placement, retaining retry/quit | Exactly one charge and five paid moves to the same recoverable attempt; stale/duplicate ad callback cannot resume another board |
| SOC-01 ranking and friends | Replace samples via paged async service adapters; server-validated score/progress, invitations and relationships | Two accounts see consistent rank/friend state; unauthorized updates rejected; offline/empty/error/loading/long-name UI passes |
| SOC-02 teams and chat | Membership/roles, durable message IDs, paging, moderation/report/block, reconnect | Multi-client join/leave/roles/chat tests; no local-only send represented as remote success; no reward claims without membership authority |
| LIVE-03 operations and acceptance | Remote configuration can restrict capabilities; service diagnostics, rollback and account-bound replay tests | Integrated Android provider/device/lifecycle tests and regression suite pass; explicit staged activation, observable failures and recovery |

These are ordered delivery packages, not calendar estimates or claims of completed code. Identity and durable-authority design precede provider-backed social/economy writes. CONT-01 can proceed locally while vendor decisions are resolved and preserves the older FAIL-CONT-01 implementation commitment.

## Transaction contract to settle before COM-01 implementation

- Separate a client request ID, provider transaction/event ID, player ID, approved product/placement ID and (for continue/double) attempt or reward-opportunity ID. Bind all identities; do not trust a boolean “success” callback.
- A verified entitlement must resolve to an approved versioned recipe. One authoritative ledger records the entitlement and applied grant atomically. The local save is a recoverable projection, not a second independent wallet authority.
- Persist pending and completed state sufficiently to recover after process death between verification, ledger commit, client refresh and store acknowledgement. Retry the same identity; never create a new grant merely because the client timed out.
- Purchase acknowledgement/consumption follows the selected store's actual contract after durable fulfillment. Refund/revocation and reconnect reconciliation need explicit policy; provider-specific details are deferred until selection and official documentation review.
- Account switches cannot move pending rewards silently. A continuation targets one suspended attempt and selected content/rules version; an incompatible or expired attempt needs a defined recovery/refund path before selling that entitlement.
- Economy values, timestamps, scores, membership and gift limits come from trusted service state. The current local daily/energy/first-clear rules must be migrated deliberately, not uploaded as unlimited currency authority.

## Minimum failure matrix

For each live mutation: success, rejection, timeout-before-server, timeout-after-commit, double tap, duplicate callback, reordered response, app kill/relaunch, offline/reconnect, expired session, account switch, stale content/config, insufficient balance and persistence failure. Assert both the server record and the client's resulting state. For social features add blocked/deleted account, role change, removed membership, message duplication and pagination consistency. For rankings add tie rules, season rollover and rejected unverified scores.

Existing fail-closed tests are valuable but are not proof of this matrix against a real backend. Keep unavailable labels until the relevant package passes.

## Deferred but retained

SMS verification (CHAPTER-SMS-01), monthly bonuses, event progress/pass/rewards, mail attachments, heroes/skins/relic ownership, full story choices/restoration and later chapter mechanics remain planned. They use the same identity/authority foundation. Effects/animation redesign remains final. See the [canonical schedule](../../DevelopmentSchedule.md).
