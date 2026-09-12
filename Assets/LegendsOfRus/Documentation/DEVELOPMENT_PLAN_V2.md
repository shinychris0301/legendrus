# Legends of Rus — Commercial Development Plan v2

## Production principle

1. Keep the approved design pages as the visual source of truth.
2. Stabilize the playable Chapter 1 vertical slice before expanding to all chapters.
3. Keep board logic, presentation, content data, economy, story, and monetization separated.
4. Every milestone must have explicit acceptance tests before the next milestone starts.
5. Commercial systems must be transparent, value-based, and compatible with remote configuration.

## Phase A — Commercial vertical slice

### v1.10 — Gameplay Core Stabilization and Regression Tests — COMPLETE
- Separate the pure board model from Unity UI animation.
- Introduce an explicit gameplay state machine.
- Centralize tap/swipe direction mapping.
- Make swaps, invalid swaps, cascades, special creation, and special activation deterministic.
- Rebuild booster targeting, preview, cancellation, execution, and inventory consumption.
- Add seeded boards and reproducible QA replays.
- Add EditMode and PlayMode regression tests.
- Preserve the approved Gameplay page layout and the restored gem presentation.

Exit criteria:
- All four swipe directions pass automated tests.
- Invalid swaps return without spending a move.
- Matches of 3, 4, 5, T and L resolve correctly.
- Row, column, burst, and color specials activate and chain correctly.
- Every booster has correct target rules and is consumed only on successful execution.
- Input is locked during animation/resolution states.
- No dead board remains unresolved.
- Gameplay visual bounds still match the approved page.

### v1.11 — Data-Driven Chapter 1 Level System — COMPLETE
- Replace the single prototype configuration with reusable LevelData assets/JSON.
- Create the first curated Chapter 1 progression set.
- Support board dimensions, move limits, goals, fixed cells, difficulty tags, rewards, and story triggers.
- Build a Unity level-authoring and validation tool.

Exit criteria:
- Chapter 1 levels load entirely from data.
- Designers can add or rebalance a level without modifying C#.
- Level selection, stars, unlocks, rewards, and retry state persist correctly.

### v1.12 — Goals, Obstacles, and Chapter 1 Difficulty Curve — COMPLETE
- Score goal.
- Color collection.
- Ice.
- Chains.
- Crates/boxes.
- Bring-down item.
- Limited-move survival.
- Tutorial scripting for newly introduced mechanics.

Exit criteria:
- Every Chapter 1 mechanic has at least one tutorial level and regression test.
- Difficulty rises gradually and has recovery levels after peaks.

Implementation status:
- Step 1 complete: schema-v2 goal, obstacle, and board-item data foundation.
- Step 2 complete: deterministic obstacle rules, bottom-exit collection, and
  generic two-slot goal progress.
- Step 3 complete: schema-v3 tutorials, one-at-a-time mechanic activation,
  persistent tutorial completion, progression validation, difficulty peaks on
  levels 3 and 6, and recovery levels on levels 4 and 7.

### v1.13 — Chapter 1 Story Vertical Slice — COMPLETE
- Chapter intro.
- Firebird discovery and tutorial dialogue.
- Mid-chapter story milestones.
- Character reactions on victory/fail.
- Chapter boss or major milestone.
- Chapter Complete flow.
- Russian localization data separated from art.

Exit criteria:
- Full story flow can be played from first launch to Chapter Complete without editor intervention.

Implementation status:
- Step 1 complete: separate Russian story data, first-launch Chapter 1 intro,
  localized multi-line Dialogue runtime, and persistent one-shot scene history.
- Step 2 complete: deterministic post-victory scene mapping, major milestones
  at levels 1/3/6/10, and localized victory/failure reactions for all ten levels.
- Step 3 complete: five-line Chapter 1 finale, save-v6 completion
  acknowledgement, relaunch recovery, replay-safe Chapter Complete routing,
  dynamic saved-progress summary, and full first-launch route coverage.

### v1.14 — Premium Feel: Animation, VFX, Audio, Haptics, and Fidelity
- Gem swap, rejection, fall, cascade, special creation, and booster VFX.
- Button, modal, map-node, victory, fail, and reward animations.
- SFX and music buses.
- Haptic feedback settings.
- Pixel-level screen comparison at 16:9, 18:9, 19.5:9, and 20:9.

Exit criteria:
- Stable 60 fps target on representative Android hardware.
- No clipping or scale drift.
- Approved-screen comparison passes for the complete core loop.

Implementation status:
- Step 1 complete in source: deterministic swap, rejection, invalid-return,
  wave, cascade, special, fall, reshuffle, and recovery hooks; corrected
  frame-rate-independent fall interpolation; provider-neutral SFX and haptic
  boundaries; seam-free board raycasts; stable-axis gesture commitment;
  pointer lifecycle and multi-touch guards; and presentation-listener failure
  isolation. Unity 6000.5.9f1 and representative-device verification remain
  pending.
- Step 2 complete in source: pure deterministic VFX mapping; a fixed,
  prewarmed, non-raycast UI-effect pool; board-scoped presentation and cleanup;
  restrained special-combination and deep-cascade response; and explicit
  activated, completed, and rejected feedback for Bogatyr Mace row/column,
  Firebird, Living Light, and Sea Pearl. Board rules, balance, story, approved
  PNGs, responsive UI geometry, and save data are unchanged. Unity 6000.5.9f1,
  visual capture, allocation/frame-pacing, and Android device verification
  remain pending.
- Step 3 complete in source: one fixed-slot, clamped, unscaled-time meta motion
  clock; repeatable pointer-owned button feedback; per-root exactly-once action
  gates; input-safe screen entry; blocker-safe modal entry, exit, replacement,
  and interruption; local map response; and distinct Victory, Fail, Reward, and
  Chapter Complete reveals. The inherited permanent hotspot latch is fixed
  without changing gameplay timing. Approved PNGs, safe-area/reference
  geometry, navigation destinations, rewards/economy, story, save schema,
  puzzle rules, packages, and scenes are unchanged. Unity 6000.5.9f1, visual
  capture, allocation/frame-pacing, and Android device verification remain
  pending.
- Step 4 complete in source: persistent settings-aware audio adapter with four
  prewarmed SFX voices and safe missing-clip diagnostics; priority-coalesced
  Android View haptics with unsupported-platform no-op fallback; shared
  safe-area fit rules; PNG-plus-metadata captures; viewport-aware approved-art
  comparison thresholds; and an opt-in, fixed-capacity development performance
  probe with F6–F10 operation, persisted deterministic reports, and CPU/GPU
  percentile gates. The four-target matrix certifies capture/layout evidence,
  not pixel fidelity; dynamic Gameplay comparison still needs a canonical
  fixture or reviewed mask. No approved or licensed audio clips are bundled.
  Unity compilation, Test Runner, audible output, four-aspect comparison,
  Android haptic behavior, and representative-device performance remain
  pending. Approved PNGs, gameplay, balance, Chapter 1 data, navigation,
  economy, rewards, story, saves, scenes, and project settings are unchanged.

### v1.15 — Economy and Retention Balance
- Energy timer and refill.
- Booster inventory and rewards.
- Daily reward calendar.
- Continue economy.
- Reward doubling.
- Starter offer and chapter-finale offer data.
- Economy simulation and guardrails.

Exit criteria:
- No negative currency states.
- All rewards and costs are remote-configurable.
- Offer presentation is transparent and does not block normal progress.

Implementation status:
- Step 1 complete in source and static verification: a versioned,
  proposal-only economy catalog;
  explicit prototype provenance for unwired declarations, runtime hardcodes,
  and authored level rewards; deterministic offline ledger, refill, cadence,
  earn-spend, and Chapter 1 affordability simulations; idempotent grant and
  atomic failure behavior; and guardrails for negative balance, duplicate
  grants, invalid references or amounts, overflow, unreachable progress, and
  forced purchase. Booster activation inventory costs and purchase-price
  availability are represented, but purchase prices are undefined and booster
  purchase affordability is `INCOMPLETE`. The proposal is non-authoritative and
  does not mutate saves, existing balances, rewards, inventory, progression, or
  UI. Unity compilation, Editor self-tests, and Test Runner execution remain
  pending.
- Step 2 foundation complete in source: an atomic multi-resource transaction
  engine, detached copy-on-write candidate coordination, immutable operational
  policy, and fail-closed activation gate bound to the proposal identity,
  normalized source SHA-256, exact Unity 6000.5.9f1 version, retained
  verification evidence, and explicit final activation approval. Product
  approval is recorded, but compilation, Editor self-test, EditMode, PlayMode,
  and final runtime-activation flags remain false. The catalog is still
  proposal-only and non-authoritative. `GameState`, `ScreenRouter`,
  `Match3Board`, and `RuntimeBootstrap` remain unchanged and do not consume the
  new coordinator or policy, so inherited runtime hazards are not claimed
  fixed. Save version 6 and `LegendsOfRus_Save_v1` remain unchanged; there is no
  schema, migration, balance rebase, automatic grant, or external commercial
  SDK. Unity compilation and execution remain pending.

### v1.16 — Analytics, Remote Config, and Monetization Adapters
- Analytics event schema and funnel tracking.
- Remote config interfaces.
- IAP adapter.
- Rewarded-ad adapter.
- Consent and failure handling.
- Development sandbox remains available.

Exit criteria:
- No store purchase is granted without a verified transaction result.
- Ad unavailability never traps the player.
- Core loop works offline without real-money features.

Implementation status:
- Step 1 complete in source and static boundary verification: one engine-free
  `LegendsOfRus.CommercialCore` assembly now contains a typed analytics schema,
  deterministic validation and bounded memory-only capture, process-local
  funnel tracking, strict proposal-only remote config bound to the reviewed
  economy identity, conservative all-disabled fallbacks, immutable consent and
  audience validation, provider-neutral IAP and rewarded-ad contracts,
  unavailable production defaults, explicit scripted development adapters and
  verifiers, and verifier-reported evidence reservations with immutable
  constructor-bound verifier selection, identity checks, and process-local
  replay checks. Reservation callers cannot substitute a verifier per request,
  and missing bindings fail closed. No production verifier or trusted
  composition authority is present, so store/ad authenticity is not claimed. The foundation is
  detached: it has no Unity, runtime, save, economy-grant, file, network, SDK,
  or backend access and is not registered with live gameplay. Reservations
  contain no reward amount or fulfillment callback. Unity 6000.5.9f1
  compilation, Editor self-tests, and all Test Runner execution remain pending.
  This does not complete the v1.16 exit criteria: production providers, legal
  and consent UI, remote transport, durable receipt reconciliation, live
  wiring, and activation remain deferred.
- Step 2 implemented in source as a pre-activation candidate, pending provider,
  legal, verification, and explicit activation approval. A new trusted
  `LegendsOfRus.CommercialRuntime` composition boundary hash-pins its local
  config, activation manifest, proposal-only remote-config snapshot, and v1.15
  economy identity. The shipped activation decision is deliberately
  `VerificationRequired / COMMERCIAL_V115_ECONOMY_INACTIVE`; the private
  external-provider build switch and every verification, approval, capability,
  and final-activation flag remain `false`. Runtime screens now use the typed
  analytics facade, but the installed provider is disabled and cannot capture
  or upload. Process-local last-known-good remote config remains conservative
  and non-authoritative. Direct prototype store/ad grants, the unknown-offer
  gem fallback, Ask Friends energy, repeatable Mail gold, and unverified reward
  doubling have been removed. A separately verified victory-double result can
  prepare only a detached economy candidate; it is never assigned to live
  state, persisted, or marked fulfilled because save version 6 has no approved
  atomic durable marker store. Commercial unavailability remains dismissible
  and the offline core loop remains available. No provider SDK, network
  transport, receipt upload, production verifier, legal/consent UI, save schema
  change, or runtime commercial activation is included. Unity 6000.5.9f1
  compilation, Editor self-test, complete Test Runner execution, retained XML
  evidence, representative-device failure paths, and all production approvals
  remain pending on the exact Step 2 candidate.

### v1.17 — Android Production Readiness
- Build profiles and signing workflow.
- Save migration and recovery.
- Crash/error reporting.
- Memory, texture, and loading optimization.
- Addressables/content delivery foundation.
- Privacy, terms, support, and account recovery flows.

Implementation status:
- Step 1 implemented in source as an Editor-only, credential-free Android build
  and signing readiness boundary. A strict SHA-256-pinned policy and paired
  maximum-order scheduled-options/BuildReport guards define an exact
  nonproduction Development APK contract, while the current compiled
  `ResolvedPackageLockApproved=false` gate prevents Android preflight and build:
  Unity 6000.5.9f1, Bootstrap only, fixed example debug identity/version/output,
  target API 36, supported minimum API, IL2CPP, ARM64, game category with the
  obsolete `androidIsGame` compatibility alias confirming that same category,
  Portrait, native debug symbols set to `None`, no
  active or stored custom Build Profile, empty persisted and per-build
  Android scripting defines, exact scheduled target/scenes/output, the sole
  `Development` Build Options flag with only normal UI Build's optional
  `ShowBuiltPlayer`, Default compression, no AssetBundle manifest, and Unity default
  debug signing. Release APK, AAB, Gradle export, custom signing, symbol
  packages, alternate identity/version/path, protected commercial drift,
  `.androidlib` projects, manifest overrides, plugins, native SDKs, and every
  production approval remain blocked. The workflow never
  invokes a build, changes PlayerSettings, reads signing credentials, creates a
  keystore, or adds player/runtime payload. `ProjectSettings.asset` and
  `Packages/packages-lock.json` are absent from the source candidate and must be
  generated/saved by exactly Unity 6000.5.9f1, then only those two files must be
  returned unedited for a reviewed source follow-up. Static/baseline verification
  runs on a separate clean extraction before import; Unity-created `.meta` files
  and generated roots in the working copy are execution state outside the sealed
  457-file source fingerprint, and the sealed-source verifier must not run there.
  That follow-up must approve and SHA-256-pin the resolved lock, flip the
  compiled approval gate, and repin verifier/evidence. Later Unity evidence binds
  to the reviewed source-archive hash plus the two explicitly retained inputs,
  not literal post-import 457-file equality.
  Do not run Android preflight or build the current candidate. Unity 6000.5.9f1
  compilation, validator, Editor self-test, complete Test Runner, exact APK,
  merged-manifest, ARM64/16 KB, and representative-device evidence remain
  pending.
- Step 2 implemented in source as a local save migration and recovery
  foundation, pending retained Unity 6000.5.9f1 and representative-device
  evidence. `PlayerSaveData` remains version 6 and the inherited primary key
  remains `LegendsOfRus_Save_v1`; independent envelope version 1 adds
  deterministic byte-count, revision, payload-hash, and envelope-hash checks.
  An engine-free three-slot journal stages and flushes writes, retains a prior
  committed backup, never promotes staging during load, blocks unsupported
  future data, and replaces recoverable history for explicit reset. A
  copy-on-write migration registry preserves the inherited v1-to-v6 semantics,
  while structured load/write results and a journal rescan before every write
  prevent a fallback default from overwriting corrupt or future raw evidence.
  The block clears only after explicit external remediation. SHA-256 is
  corruption detection, not authentication or encryption. Cloud/account
  recovery, durable commercial entitlement, provider SDKs, economy/commercial
  activation, production crash transport, and Android production remain out of
  scope. Step 2 was authorized after a user-reported zero-error run of the
  previous candidate, but retained Step 1 Unity/package-lock/Android evidence
  is not present and `ResolvedPackageLockApproved=false` remains.
  Raw current-v6 imports require the complete typed 32-field DTO, while
  unversioned and v1-v5 inputs retain migration compatibility. Canonical future
  raw data and differently shaped or oversized future envelopes block
  downgrade. `PrimaryDurablyCommitted` distinguishes an adopted, flushed reset
  primary from later redundancy/cleanup failure. Twelve inherited fixtures now
  isolate the primary, backup, and staging keys; every other inherited test
  remains Step 1-identical. Legacy GameState mutator booleans are not durable
  proof, and copy-on-write mutation transactions remain required before
  closed/player-facing testing or any transactional-durability claim.
- Step 3 has been prepared early at the user's direction as a detached
  pre-entry source candidate. A new engine-free `DiagnosticsCore` accepts only
  closed incident/subsystem values and a reviewed `ErrorReportCode` /
  `ErrorReportSiteCode` enum catalog whose wire strings are generated
  internally. Records deliberately have no message, stack, exception, arbitrary
  metadata, identifier, save payload, purchase payload, credential, consent
  snapshot, or user text. A bounded process-memory buffer adds deterministic
  lower-hex SHA-256 fingerprints, inclusive-window deduplication, hard
  retention from first occurrence, fixed-order canonical JSON, and capped
  retry state using an injected clock. Offline, disabled, network-capable,
  not-due, throwing, and malformed adapter cases are fail-closed. No Unity log
  callback, runtime bootstrap, GameObject, persistence, background worker,
  provider SDK, endpoint, credential, or network implementation is included;
  the public API cannot inject a transport, the shipped production path is
  disabled, and the compiled network gate remains false. Because the Step 2
  Unity and device entry evidence is not retained,
  Step 3 remains
  `IMPLEMENTED_IN_SOURCE_PENDING_V1_17_STEP02_PREREQUISITE_AND_STEP03_UNITY_VERIFICATION`
  and must not be represented as formally entered, Unity-verified, or
  production-ready.


- Step 4 has now been prepared early as a source candidate. It adds an engine-free
  optimization policy/evidence assembly, bounded Editor/Development memory
  sampling, low-memory cleanup, retired-screen cleanup after safe route boundaries,
  bootstrap/screen-route timing, and a locked texture audit. Runtime art remains
  Sprite, non-readable, without mipmaps, at max 2048 with the inherited Android
  ASTC 6x6 override. The 65 approved PNG source files are inventoried and must
  remain byte-identical to Step 3. The 47 Resources PNGs represent a 136.78 MiB
  raw RGBA source upper bound, not measured runtime residency. Addressables and
  remote content delivery are deliberately deferred. Step 4 remains
  `IMPLEMENTED_IN_SOURCE_PENDING_V1_17_STEP02_STEP03_PREREQUISITES_AND_STEP04_UNITY_DEVICE_VERIFICATION` until the Step 2/Step 3 prerequisites plus its own Unity and
  representative-device evidence are retained.

### v1.18 — Closed Test / Soft-Launch Candidate
- Device matrix QA.
- Balance telemetry.
- Tutorial and funnel review.
- Store listing assets.
- Release checklist.
- Controlled test build.

## Phase B — Scalable content production

### v2.0 — Chapter Content Pipeline
- Chapter templates.
- Environment and gameplay-skin loading.
- Story and level data authoring.
- Addressables groups.
- Automated chapter validation.

### v2.1–v2.4 — Content Expansion
- Chapters 2–5.
- Chapters 6–10.
- Chapters 11–15.
- Chapters 16–20.

## Phase C — Long-term commercial systems

- Seasonal events and character-rescue events.
- Event pass and event shop.
- Character collection, cosmetics, and frames.
- Rankings.
- Friends and teams.
- Team chest and chat.
- LiveOps calendar.
- Post-Chapter-20 seasonal content.

## Immediate next implementation

The active source milestone is **v1.19 Step 3 — Post-Launch Trend, Retention, and Economy Guardrail Review**. It adds aggregate trend/retention/economy evidence review against an explicitly reviewed project-specific guardrail profile.

The exact Step 3 bytes must compile in Unity 6000.5.9f1 and pass the Step 3 self-test plus complete EditMode/PlayMode suites. Real post-launch evidence and any economy-change approval remain external human-reviewed inputs.

All production-sensitive execution gates remain blocked in source.

### v1.17 Step 6 implementation note
The Privacy, Terms, Support, and Account Recovery foundation is implemented in
source with local compliance/support boundaries and production analytics/ad
consent gates. Final legal copy/UI and all production account backends remain
blocked pending product/legal inputs and Unity verification. The next major
planned milestone after accepted Step 6 verification is v1.18 Closed Test /
Soft-Launch Candidate.


### v1.18 Step 1 implementation note
The Device Matrix QA and Closed-Test Release Preflight foundation is implemented in source. Five synthetic Android QA profiles cover 16:9, 18:9, 19.5:9 and 20:9 across low/mid/high QA capability bands. Runtime capture is coarse and local-only; no persistent device identifier, remote QA transport, production signing, controlled-test build authorization, or Play publishing is enabled.

### v1.18 Step 2 implementation note
The balance/tutorial review foundation is implemented in source with a 128-entry default process-memory buffer and no persistence/upload path. Level starts, wins, failures, retries, winning moves remaining, move limits, scores, and tutorial shown/completed signals are aggregated into local review summaries. Thresholds create review flags only and cannot mutate authored level or tutorial data. Production analytics, remote telemetry, automatic balance changes, tutorial experiments, production signing, controlled-test build authorization, and Play publishing remain blocked.

### v1.18 Step 3 implementation note
The Store Listing Assets and Release Checklist foundation is implemented in source. The requirements snapshot is dated 2026-09-09 and the final store-asset staging folder intentionally contains no publishable graphics. The release checklist separates controlled-test artifact creation from Play upload and fails closed on missing store, legal, declaration, signing, AAB, or publishing evidence.


## Previous implementation — v1.18 Step 4
Controlled-test build/release-candidate preflight foundation is implemented. API 36/AAB/ARM64/16 KB requirements are validated locally; production signing, artifact creation, upload and publishing remain separately gated.

## Current implementation — v1.18 Step 5
Controlled-test artifact/device-evidence/candidate-freeze foundation is implemented. A freeze requires one validated signed AAB, five required physical-device profile records bound to that exact AAB SHA-256, reviewed source/config/store/signing evidence, and separate freeze authorization. The shipped source contains none of the external evidence and cannot upload or publish.

### v1.18 Step 5 implementation note
The Controlled Test Artifact, Device Evidence, and Candidate Freeze foundation is implemented in source. It requires five unique physical-device QA profile records tied to one exact signed-AAB SHA-256 and can generate an immutable local freeze manifest only after every evidence and approval gate is explicitly true. The shipped candidate contains no AAB or physical-device evidence and keeps candidate freeze, closed-test upload, and Play publishing unauthorized.


## Current implementation — v1.18 Step 6
Closed-test upload/go-no-go/rollout-readiness foundation is implemented. It keeps all Play Console network/publishing actions disabled and requires real frozen-artifact, tester, health, account, store/legal, and authorization evidence before any external release action can be considered ready.
## v1.18 Step 8 — Production Access Decision and Launch Candidate Readiness

- retain/review production-access decision when applicable;
- bind launch readiness to the frozen candidate and signed AAB;
- validate store/legal/app-content/signing/versioning/Android release evidence;
- validate first-release full rollout vs update staged rollout policy;
- require support, monitoring, rollback and release-history readiness;
- keep Play publishing as an explicit manual authorization boundary.



## v1.19 — Production Launch Operations and Post-Launch Observability

### Step 1 — Foundation
- Launch-operations preflight and manual authorization boundary.
- Android-vitals stability review using current Play thresholds.
- Privacy-minimal incident summary and launch-window evidence.
- Human Continue/Hold/Halt/Escalate recommendation logic.
- No automatic build, upload, rollout or rollback action.

### Step 2 — Incident Response, Hotfix, and Rollout Control Review
- Privacy-minimal incident register and deterministic triage.
- Hotfix candidate linkage, evidence qualification, and manual release authorization boundary.
- Staged/full-release halt, resume, increase, replacement, and rollback-review validation.
- No automatic build, signing, Play API, rollout, or rollback action.

### Step 3 — Post-Launch Trend, Retention, and Economy Guardrail Review
- Aggregate active-user/session/completion trend review.
- D1/D7/D30 cohort retention review against an approved baseline profile.
- Soft/hard currency, booster, and reward-flow guardrails.
- Human recommendations only; no automatic economy or telemetry mutation.


### v1.19 Step 6 — LiveOps Outcome Review, Experiment Decision, and Rotation Closeout
- Aggregate end-of-cycle outcome evidence review.
- Experiment decision consistency and archive readiness.
- Rotation/reward/content reconciliation and closeout.
- Human/manual closeout authorization only; no automatic mutations.

The exact Step 6 bytes must compile in Unity 6000.5.9f1 and pass the Step 6 self-test plus complete EditMode/PlayMode suites before accepted closeout evidence is considered.


### v1.19 Step 7 — LiveOps Cycle Retrospective, Archive, and Next Cadence Planning
- Retrospective review with lessons/action ownership.
- Cryptographic archive-evidence validation.
- Next-cadence capacity/localization/support/measurement planning.
- Manual cycle-closure and publishing authorization remain separate.
- No automatic LiveOps, experiment, Remote Config, reward/economy, or Play action.


### v1.20 Step 4 — Functionality-first demo game flow

The development priority is now functionality before final art. The current demo design remains acceptable for flow testing. Step 4 fixes deterministic Back navigation (`ChapterMap -> WorldMap -> Home`), appends the missing final-product routes without changing legacy serialized `ScreenId` values, and provides local demo boundaries for social/ranking/login/LiveOps pages while production providers remain unavailable.

The authoritative page function and future asset-resource specification is `V1_20_STEP04_PAGE_FUNCTION_ASSET_PLAN.json/csv/md`. Final production visual reconstruction resumes only after functional QA and edge-case verification.

Next: **v1.20 Step 5 — Functional QA, Edge Cases, and Full Demo Flow Verification**.

### v1.20 Step 5 — Functional QA, edge cases, and full demo-flow verification

Step 5 is the active functionality-first acceptance checkpoint. It hardens the visible Map Back controls, corrects the Friends List parent route to Team Hub, adds PlayMode coverage for actual map Back hotspot dispatch and all extended demo routes, and preserves production-provider fail-closed boundaries. The authoritative 36-page function/asset plan from Step 4 remains unchanged and has been audited complete for 36/36 pages.

Unity 6000.5.9f1 execution is still required: run the Step 5 self-test, complete EditMode/PlayMode suites, the full demo-flow checklist, and Android Back verification. Static verification must not be represented as Unity/device verification.

Next after accepted executable QA: **v1.20 Step 6 — Functional Acceptance Gate and Production Asset Reconstruction Readiness**.


### v1.20 Step 6 implementation note — Functional Acceptance Gate and Production Asset Reconstruction Readiness

Step 6 freezes the Step 5 functionality baseline. Runtime scripts, inherited tests, Packages, ProjectSettings, approved references, canonical UI kit, production page prefabs, and PNG assets remain byte-identical to the Step 5 baseline. The checkpoint adds a source acceptance gate, functional freeze manifest, 36/36 page asset contracts, Waves 0–4, readiness inventory/gap analysis, and Editor/static validation.

Source functional acceptance: **PASS**. Executable Unity/Android acceptance: **PENDING**. Asset authoring preparation may begin, but runtime production-art cutover remains blocked until the exact candidate passes Unity 6000.5.9f1 compilation, both Step 5 and Step 6 self-tests, complete EditMode/PlayMode suites, full demo-flow QA, and Android visible/system Back verification.

Next after executable acceptance: **v1.20 Step 7 — Production Asset Reconstruction Wave 0/1 (Shared UI Foundation + Core Loop)**.


## v1.20 Step 7 — Production Asset Reconstruction Wave 0/1
Wave 0 shared foundation and 12 Wave 1 candidate page asset/layout contracts are authored. Active runtime cutover remains blocked until Step 6 executable acceptance and Wave 1 visual/device signoff.


## v1.20 Step 7.2 checkpoint

Wave 1 candidate prefab serialization and aspect containment are complete. Active cutover is **NO-GO** until reference fidelity, missing direct references, production Cyrillic font binding, Unity four-aspect captures, and Android acceptance are closed. Next: Step 7.3 reference-accurate reconstruction refinement.
