# v1.20 Step 3.2 current checkpoint

Canonical production UI sprites and deterministic reusable-prefab generation are implemented. Run `Legends of Rus > Tests > Run v1.20 Step 3.2 Self-Tests`. Runtime screenshot cutover remains disabled; Step 3.3 performs the first page replacements.

v1.20 Step 2 is the current source milestone. See README_V1_20_STEP02.md at project root.

# Current milestone: v1.20 Step 1

Start with `README_V1_20_STEP01.md`. The 20-chapter content-pipeline contract, templates, legacy Chapter 1 validation boundary, ownership rules, and QA tooling are implemented in source. Chapter migration, Addressables mutation, runtime cutover, automatic art generation, approved-art overwrite, and production content publication remain unauthorized.

# Current milestone: v1.19 Step 8

Start with `README_V1_19_STEP08.md`. Real reviewed v1.19 chain, operating-baseline, and v1.20 transition evidence remain external. v1.19 closure and v1.20 preparation remain separately unauthorized.

# Current milestone: v1.19 Step 7

Start with `README_V1_19_STEP07.md`. Real reviewed retrospective/archive/next-cadence evidence remains external.

# CURRENT MILESTONE — v1.19 Step 3

Post-Launch Trend, Retention, and Economy Guardrail Review is implemented in source. Run the v1.19 Step 3 self-tests and full Test Runner suites before retaining reviewed aggregate evidence. Automatic analytics upload, economy/reward mutation, level mutation, Play actions, and publishing remain disabled.

# CURRENT MILESTONE — v1.18 Step 8

Production Access Decision and Launch Candidate Readiness is implemented in source. Run the v1.18 Step 8 self-tests and full Test Runner suites before retaining external launch evidence. Production release and store publishing remain explicitly unauthorized in the shipped source.

# CURRENT CHECKPOINT: v1.18 Step 6 — Closed Test Upload, Go/No-Go, and Rollout Readiness

Run the Step 6 self-test and full Test Runner suites before advancing. Closed-test upload, tester rollout, production access, and production publishing remain blocked until real retained evidence and explicit authorization exist.

# Legends of Rus — Commercial Vertical Slice v1

This Unity project begins implementation of the selected locked design as an actual playable game.

## Unity
Target editor: Unity 6000.5.9f1.

## Start
1. Open this folder as a Unity project.
2. Let Unity import all textures.
3. Open `Assets/Scenes/Bootstrap.unity`.
4. Press Play.
5. The runtime bootstrap builds the UI automatically.

## Implemented flow
Loading → Home → Chapter Map → Chapter Intro → Pre-Level → Gameplay → Victory / Fail → Dialogue / Chapter Map / Chapter Complete.

Chapter 1 now contains ten data-driven levels with persisted selection, stars,
unlocks, retry history, first-clear rewards, and story triggers.

The completed v1.12 sequence now activates deterministic ice, chain, crate,
score, collection, bring-down, and survival mechanics across Chapter 1. New
mechanics debut one at a time, scripted tutorial steps pause and resume gameplay
safely, completed tutorial steps persist, and levels 4 and 7 provide recovery
after the two authored difficulty peaks.

Chapter 1 story structure and Russian copy now load from separate validated
resources. The intro runs once, seven post-victory scenes follow deterministic
level mappings, and every released level has repeatable localized victory and
failure character reactions on its result screen.

The level 10 victory now resolves a five-line major finale before Chapter
Complete. Completion remains recoverable until the player leaves that screen,
is acknowledged exactly once, and does not repeat when level 10 is replayed.
The runtime summary displays saved level and star totals instead of the baked
reward numbers in the approved screen skin.

The v1.14 Step 1 gameplay pass now publishes deterministic, ordered motion
hooks for swaps, blocked and invalid returns, match waves, cascades, specials,
falls, reshuffles, and recovery. Provider-neutral SFX and haptic boundaries use
safe no-op defaults. Full-cell raycast roots, non-overlapping board/footer
hitboxes, stable-axis swipe commitment, explicit pointer ownership, and safe
presentation callbacks remove the diagnosed intermittent input-loss and
coroutine-freeze paths without changing puzzle rules or approved artwork.

The v1.14 Step 2 pass consumes those hooks through a deterministic recipe
mapper and a fixed, prewarmed UI-effect pool. Every generated effect is
non-raycast and is returned on completion, recovery, disable, or teardown.
Major special combinations and deep cascades receive bounded response, while
Bogatyr Mace row/column, Firebird, Living Light, and Sea Pearl publish explicit
activated, completed, and rejected feedback. Presentation remains unable to
mutate board state, booster inventory, balance, story, or navigation.

The v1.14 Step 3 pass adds one bounded unscaled-time clock for meta UI motion.
Buttons use repeatable pointer-owned feedback and a per-root action gate;
screens cannot receive input while entering; modal blockers remain active
through exit; and Pause → Settings → Pause preserves the same paused board.
Existing map regions receive local non-raycast pulses, while Victory, Fail,
Reward Claim, and Chapter Complete use distinct presentation-only reveals.
No animation path can grant rewards, change progression, acknowledge story, or
select a navigation destination.

The v1.14 Step 4 pass connects the existing feedback signals to a persistent,
settings-aware audio adapter and a priority-coalesced Android View haptic bridge.
Missing clips and unsupported haptics remain safe no-op conditions; approved or
licensed audio content is not bundled. Fidelity captures now include viewport,
safe-area, route, device, and transition metadata, and the Editor comparator
requires that sidecar and uses an aspect-fit artwork crop to produce explicit
diff metrics. The four-target runner proves capture/layout evidence only;
dynamic Gameplay pixel comparison still needs a canonical fixture or mask.
F6–F9 start persisted development performance scenarios and F10 cancels; CPU
and GPU percentiles are gated, while allocation counters are aggregate-window
evidence. Unity, audible-output, screenshot, Android haptic, and
representative-device performance verification remain pending.

The v1.15 Step 1 static-source pass defines a versioned, proposal-only economy
catalog and pure deterministic offline validation and simulation. Reports
distinguish the inherited unwired economy JSON, runtime hardcodes, and authored
Chapter 1 level rewards before comparing them with the proposal. Booster
activation inventory costs and purchase-price availability are modeled, but
booster purchase prices are undefined and purchase affordability remains
`INCOMPLETE`. Catalog work cannot mutate `PlayerSaveData`, balances, inventory,
progression, saves, or UI behavior and does not make proposed values
authoritative. Store/ad SDKs, analytics, backend, remote config, offer redesign,
and Chapter 2 remain outside this step. Unity compilation, Editor self-tests,
and Test Runner execution remain pending.

The v1.15 Step 2 source candidate adds a detached atomic transaction engine,
copy-on-write version-6 save coordination, configured energy/reward operations,
and an immutable approval gate. It is deliberately not registered with live
gameplay: the catalog remains proposal-only, all Unity-verification and final
activation flags remain false, and the existing runtime paths are unchanged
until the documented activation checkpoint is completed.

The v1.16 Step 1 source candidate adds one engine-free CommercialCore assembly
for typed analytics and funnels, strict proposal-only remote config, immutable
consent decisions, provider-neutral IAP and rewarded-ad contracts, unavailable
production defaults, and explicit process-local development adapters.
Verifier-reported evidence can reserve only a detached idempotency key. The
gate binds both verifier roles immutably at construction, rejects missing
bindings, and does not accept caller-selected verifiers per reservation. A
reservation contains no reward amount or fulfillment callback. Nothing is
registered with live gameplay in Step 1, no provider or SDK is installed, and
no save, balance, inventory, screen, or v1.15 activation flag changes.

The v1.16 Step 2 source candidate adds a hash-pinned, trusted runtime
composition boundary while keeping every external provider and capability
disabled. Runtime screens now call the typed analytics facade, process-local
remote config uses only a validated proposal or conservative fallback, and the
old prototype commercial grants have been removed. Verified victory-double
evidence can produce only a detached economy candidate; no candidate can be
persisted or completed because save version 6 has no approved atomic durable
fulfillment store. The shipped activation phase is
`VerificationRequired / COMMERCIAL_V115_ECONOMY_INACTIVE`, and all provider,
legal, verification, capability, and activation flags remain false. Unity
6000.5.9f1 compilation, Editor self-test, Test Runner execution, retained
evidence, production approvals, and explicit activation remain pending.

The v1.17 Step 1 source candidate adds a separate Editor-only Android readiness
contract. A strict, SHA-256-pinned policy and paired maximum-order
scheduled-options/BuildReport guards define one exact nonproduction Development
APK configuration, but the current compiled
`ResolvedPackageLockApproved=false` gate prevents both Android preflight and APK
build. Release APK, AAB, Gradle export, custom
signing, custom Build Profiles, alternate identity/version/output, persisted or
per-build Android defines, alternate scheduled scenes, extra Build Options,
AssetBundle manifests, native debug symbols, `.androidlib` projects, embedded
packages, managed plugins, manifest overrides, and changed commercial
configuration are rejected before compilation or build. The validator never
starts a build or changes
PlayerSettings and does not read a keystore path, alias, password, environment
variable, or command-line credential. No Android plugin, provider SDK,
manifest override, Gradle template, keystore, or binary is included. Exactly
Unity 6000.5.9f1 must generate/save the missing `ProjectSettings.asset` and
resolve/save `Packages/packages-lock.json`. First run static/baseline verification
on a separate clean extraction, then use a different copy for Unity import.
Import-generated `.meta` files and roots such as `Library`, `Logs`, and
`UserSettings` are execution state outside the sealed 457-file source
fingerprint; do not run the sealed-source verifier in that imported copy. Return
only the two named Unity-generated files, unedited, for a reviewed source
follow-up that approves and SHA-256-pins the lock, flips the compiled gate, and
repins the verifier/evidence. Later Unity evidence binds to the reviewed source
archive hash plus those explicitly retained inputs, not literal post-import
457-file equality. Do not run Android preflight or build the current candidate.
All Step 1 Unity, APK, native-library, 16 KB, and device evidence remains
pending.

The v1.17 Step 2 source candidate keeps `PlayerSaveData` at version 6 and the
primary `LegendsOfRus_Save_v1` identity unchanged while adding independent
storage envelope version 1. An engine-free journal validates deterministic
UTF-8/Base64 framing, byte counts, revisions, payload SHA-256, and envelope
SHA-256; stages and flushes writes; retains a committed backup; never promotes
staging on load; and blocks unsupported future versions. A copy-on-write
migration registry preserves the inherited v1-to-v6 semantics, and explicit
reset replaces recoverable history. Unrecoverable, storage, parse, migration,
and future-version failures make every normal write rescan and block while the
evidence remains, so an in-memory fallback cannot erase it. Clearing that block
requires explicit external remediation. SHA-256 is corruption detection, not
authentication or encryption. No cloud/account recovery, durable commercial
fulfillment, provider, economy authority, or production Android capability is
introduced. Step 2 Unity and device evidence is pending; the inherited Step 1
package-lock approval gate remains false.

Raw current-v6 imports require the complete typed 32-field DTO without
duplicate known fields; unversioned and v1-v5 inputs retain migration
compatibility. Canonical future raw values and canonical differently shaped or
oversized future envelopes block downgrade. A reset whose primary flush is
durable may be adopted through `PrimaryDurablyCommitted` even when later
backup/cleanup work reports failure, without overstating full journal success.
All 12 affected inherited fixtures preserve and isolate primary, backup, and
staging; every other inherited test is unchanged. Legacy GameState mutator
booleans remain gameplay-acceptance signals, not durable proof. Copy-on-write
mutation transactions are required before closed/player-facing testing or any
transactional-durability claim.

The v1.17 Step 3 pre-entry source candidate adds an independent, engine-free
diagnostics core. It accepts only closed incident/subsystem values and reviewed
`ErrorReportCode` / `ErrorReportSiteCode` enums whose wire strings are generated
internally; its public records have no message, stack trace,
exception, arbitrary metadata, identifier, save payload, purchase payload, or
user-entered text field. A fixed-capacity process-memory buffer provides
deterministic fingerprinting, inclusive-window duplicate coalescing,
timestamp-triggered hard retention from first occurrence, capped retry state,
and canonical JSON
inspection. Offline, disabled, network-blocked, not-due, throwing, and malformed
transport cases are contained without mutating gameplay or save state.

This diagnostics core is deliberately detached. It does not subscribe to
Unity logs, create a GameObject, run at bootstrap, persist records, perform
background work, install a crash SDK, or include a network transport. Its public
API cannot inject a transport; the shipped production path is disabled and the
compiled network gate is false. Step 2
Unity and device entry evidence is still not retained, so Step 3 cannot be
called formally entered, Unity-verified, or production-ready.

## v1.17 Step 4 source candidate
Step 4 adds an engine-free optimization policy/evidence core, a bounded runtime
optimization service, low-memory cleanup, retired-screen lifetime control,
bootstrap/route timing, a locked texture audit, and a source texture inventory.
The approved PNG bytes are not modified. Optimization evidence is process-memory
only and contains no player/device identifiers or network/persistence channel.
Unity and representative Android evidence for these exact Step 4 bytes is still
pending, and Addressables/content delivery remains deferred.

Home also links to:
- World Map
- Shop
- Events placeholder
- Characters placeholder
- Mail/reward curiosity hooks

## Design fidelity strategy
The approved design PNGs are used as immutable screen skins in the vertical slice.
Transparent interaction hitboxes are aligned to the visible buttons. This guarantees that
the core page appearance in the first implementation stays identical to the approved screen
instead of being approximated with a different UI kit.

The gameplay board is the first area reconstructed as true runtime content. It overlays the
approved Gameplay design with individually authored gem sprites and working Match-3 logic.

As development continues, baked dynamic areas (currency values, goals, prices, player name,
event timers, etc.) must be replaced by native Unity components while preserving the approved
visual composition.

## Controls
- Mouse/touch: UI and Match-3 board
- Android back / Escape: pause or resume Gameplay, navigate back contextually,
  stay in active Dialogue, and quit from Home
- F12: save a runtime QA screenshot to `Application.persistentDataPath/DesignQACaptures`
- Editor/Development Build F6/F7/F8/F9: persist Gameplay Idle, Gameplay Active,
  Meta Idle, or Meta Transition performance evidence; F10 cancels the run

## Editor tools
- `Legends of Rus/Validate Commercial Vertical Slice`
- `Legends of Rus/Levels/Open Chapter 1 Authoring`
- `Legends of Rus/Tests/Run v1.11 Level System Self-Tests`
- `Legends of Rus/Tests/Run v1.12 Step 1 Self-Tests`
- `Legends of Rus/Tests/Run v1.12 Step 2 Self-Tests`
- `Legends of Rus/Tests/Run v1.12 Step 3 Self-Tests`
- `Legends of Rus/Tests/Run v1.13 Step 1 Self-Tests`
- `Legends of Rus/Tests/Run v1.13 Step 2 Self-Tests`
- `Legends of Rus/Tests/Run v1.13 Step 3 Self-Tests`
- `Legends of Rus/Tests/Run v1.14 Step 1 Self-Tests`
- `Legends of Rus/Tests/Run v1.14 Step 2 Self-Tests`
- `Legends of Rus/Tests/Run v1.14 Step 3 Self-Tests`
- `Legends of Rus/Tests/Run v1.14 Step 4 Self-Tests`
- `Legends of Rus/Tests/Run v1.15 Step 1 Self-Tests`
- `Legends of Rus/Tests/Run v1.15 Step 2 Self-Tests`
- `Legends of Rus/Tests/Run v1.16 Step 1 Self-Tests`
- `Legends of Rus/Tests/Run v1.16 Step 2 Self-Tests`
- `Legends of Rus/Tests/Run v1.17 Step 1 Self-Tests`
- `Legends of Rus/Tests/Run v1.17 Step 2 Self-Tests`
- `Legends of Rus/Tests/Run v1.17 Step 3 Self-Tests`
- `Legends of Rus/Tests/Run v1.17 Step 4 Texture Audit`
- `Legends of Rus/Tests/Run v1.17 Step 4 Self-Tests`
- `Legends of Rus/Tests/Run v1.17 Step 5 Self-Tests`
- `Legends of Rus/Tests/Run v1.17 Step 6 Self-Tests`
- `Legends of Rus/Tests/Run v1.18 Step 1 Self-Tests`
- `Legends of Rus/Tests/Run v1.18 Step 2 Self-Tests`
- `Legends of Rus/Tests/Run v1.18 Step 3 Self-Tests`
- `Legends of Rus/Tests/Run v1.18 Step 4 Self-Tests`
- `Legends of Rus/Tests/Run v1.18 Step 5 Self-Tests`
- `Legends of Rus/QA/Print v1.18 Step 5 Candidate Freeze Preflight`
- `Legends of Rus/QA/Generate v1.18 Step 5 Candidate Freeze Manifest`
- `Legends of Rus/Android/v1.17 Step 1/Validate Development APK Readiness`
- `Legends of Rus/Android/v1.17 Step 1/Show Manual Configuration Checklist`
- `Legends of Rus/Android/v1.17 Step 1/Open Android Player Settings`
- `Legends of Rus/Android/v1.17 Step 1/Open Build Profiles`
- `Legends of Rus/QA/Run v1.14 Step 4 Capture/Layout Matrix (Play Mode)`
- `Legends of Rus/QA/Compare Runtime Capture To Approved Design`

## Important
Store-purchase and rewarded-ad UI paths now fail closed and cannot perform the
old direct prototype grants. No production provider is installed or active.
The only implemented fulfillment mapping is a detached victory-double
candidate, and it cannot reach live state or persistence. Production activation
still requires verified transactions, an atomic durable idempotency boundary,
consent/legal review, provider, product, placement and regional approval, full
Unity verification with retained evidence, and explicit final approval.

The v1.17 Step 1 guard also blocks every production-sensitive Android artifact.
Its defined package and output are development-verification placeholders, not
production identities. The current package-lock approval gate is false: do not
run Android preflight or attempt any APK/AAB build, Gradle export, Play upload,
or signing action in this candidate.

Step 2 local recovery does not remove that Android hard stop and does not make
save data trusted commercial evidence. Do not treat the envelope hashes as
proof of purchase, server authenticity, or anti-cheat protection. Use only
disposable profiles for corruption/interruption tests, retain raw primary,
backup, and staging evidence first, and never call `PlayerPrefs.DeleteAll()` on
a real profile.

Step 3 does not authorize collection or transport. Never pass a Console
message, stack trace, filesystem path, QA replay token, save value, identifier,
commercial evidence, credential, or user text into diagnostics. Complete the
retained Step 2 prerequisite first, then verify the exact Step 3 candidate in
Unity 6000.5.9f1 before any later integration is considered.

Step 4 does not waive the Step 2/Step 3 prerequisite evidence. Do not describe
its memory thresholds as certified device limits until representative Android
measurements are retained. Do not activate Addressables/remote content delivery,
production providers, economy/commercial systems, Android production build, or
publishing from this source candidate.


## v1.17 Step 5 source candidate

Step 5 pins Addressables 2.8.1 and adds deterministic local groups/labels, a
validation probe, content catalog validation, reference-counted Addressables
sprite release, and a fail-closed Resources fallback. Current approved
production Resources are not migrated and all remote delivery/catalog-update
activation remains false. See
`V1_17_STEP05_ADDRESSABLES_CONTENT_DELIVERY_FOUNDATION.md`.


## v1.17 Step 6 source candidate

Step 6 preserves the accepted Step 5 compile hotfixes and adds an engine-free
compliance core, separate local compliance persistence, an explicit local-only
support export, strict non-persistence of recovery codes, and consent gates for
actual runtime analytics and rewarded-ad calls. Final legal documents, legal and
support UI, recovery/deletion backends, production account services, remote
content, commercial activation, production Android signing, and publishing all
remain blocked. No replacement Privacy/Terms/Support art is invented because
those approved locked-style screens do not yet exist in the retained art set.
See `V1_17_STEP06_PRIVACY_TERMS_SUPPORT_ACCOUNT_RECOVERY_FOUNDATION.md`.


## v1.18 Step 1 source candidate

Step 1 adds a five-profile synthetic Android QA matrix, engine-free capability/aspect classification, a 64-entry local-only observation ring buffer, and a fail-closed closed-test preflight evaluator. Device identifiers, remote QA upload, production signing, controlled-test build authorization, final store/privacy approvals, and Play publishing remain disabled. See `V1_18_STEP01_DEVICE_MATRIX_CLOSED_TEST_PREFLIGHT_FOUNDATION.md`.

## v1.18 Step 2 — balance/tutorial review foundation

The current source candidate adds a separate engine-free local balance-review core.
Level starts/wins/fails and tutorial shown/completed events are mirrored into a
bounded process-memory buffer using enums/integers only. A deterministic local
review aggregates win/failure/retry rates, winning move margin, and tutorial
completion drop-off. This is review evidence only: no production analytics
transport, persistent telemetry, automatic balance mutation, tutorial experiment
mutation, signing, controlled-test build authorization, or Play publishing is enabled.

## v1.18 Step 3 — store listing/release checklist foundation

The current source candidate adds an engine-free Google Play store-listing requirements validator, an intentionally empty final-asset staging boundary, and a fail-closed release checklist. Reference design screens remain reference-only and are not counted as runtime screenshot evidence. Final store text/assets, privacy/Data safety, content declarations, production signing, AAB evidence, controlled-test authorization, and Play publishing all remain blocked. See `V1_18_STEP03_STORE_LISTING_RELEASE_CHECKLIST_FOUNDATION.md`.


## v1.18 Step 4 — controlled-test build/release-candidate foundation

Step 4 adds the engine-free controlled-test build evaluator and artifact validation boundary for API 36/AAB/IL2CPP/ARM64/16-KB readiness. It does not invoke a build, read signing secrets, or authorize upload/publishing. See `V1_18_STEP04_CONTROLLED_TEST_BUILD_RELEASE_CANDIDATE_FOUNDATION.md`.

## v1.18 Step 5 — controlled-test artifact/device evidence/candidate freeze

The current source candidate adds exact AAB SHA-256 evidence binding, five-profile privacy-minimized physical-device evidence validation, deterministic source/config/evidence hashes, and a fail-closed local candidate-freeze manifest generator. No AAB, device evidence, signing secret, Play credential, upload, or publishing authorization ships enabled. See `V1_18_STEP05_CONTROLLED_TEST_ARTIFACT_DEVICE_EVIDENCE_CANDIDATE_FREEZE.md`.

### v1.18 Step 6 implementation note
The Closed Test Upload, Go/No-Go, and Rollout Readiness foundation is implemented in source. It validates a verified candidate freeze, Play test-track preparation, privacy-minimal upload/health evidence, conditional 12-tester/14-day production-access requirements for applicable new personal accounts, first-release 100% rollout rules, and rollback/monitoring/declaration approvals. No Play API, network upload, tester identity persistence, signing credential access, or rollout action is implemented.

## v1.19 Step 1 — production launch operations and post-launch observability

Step 1 adds an engine-free, fail-closed launch-operations core. It validates the human launch-operations plan, reviews retained aggregate Android-vitals and incident evidence, and produces Continue/Hold/Halt/Escalate recommendations. It does not call Play APIs, build/sign/upload an artifact, change rollout percentage, halt/resume a rollout, or execute rollback. Current Google Play bad-behavior thresholds are encoded as review signals only. Real production evidence remains absent in the source candidate. See `V1_19_STEP01_PRODUCTION_LAUNCH_OPERATIONS_POST_LAUNCH_OBSERVABILITY_FOUNDATION.md`.



## v1.19 Step 5 — LiveOps cadence/content/experiment readiness

Step 4 adds engine-free cadence, aggregate content-health, and experiment-readiness review. It does not activate production analytics or Remote Config, assign experiment traffic, mutate LiveOps/economy/rewards/difficulty, or publish content. Reviewed evidence belongs under `ProjectDocs/LiveOpsReadiness`; user identifiers, tester identity, credentials and unrestricted raw telemetry do not. See `V1_19_STEP04_LIVEOPS_CADENCE_CONTENT_HEALTH_EXPERIMENT_READINESS.md`.


## v1.19 Step 6 — LiveOps outcome/decision/closeout review

Step 6 is source-only until the exact bytes compile and pass the complete Unity Test Runner suites. Outcome, experiment-decision and rotation-closeout evidence templates are not production evidence. Do not enable production Analytics/Remote Config or live mutation/publishing actions from this checkpoint.
