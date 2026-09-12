# v1.14 Step 2 — Core Loop VFX and Booster Feedback

## Outcome

Step 2 consumes the immutable Step 1 motion timeline through a pure semantic
mapper, a board-scoped presenter, and a fixed prewarmed UI-effect pool. VFX is a
presentation consumer only: the deterministic board model and resolution plan
remain authoritative for every puzzle outcome.

## Bounded presentation architecture

`CoreLoopVfxMapper` converts a semantic event into an immutable
`CoreLoopVfxRequest` and shares the pure `CoreLoopVfxPresenter.ResolveCue`
mapping. `CoreLoopVfxProfile` owns immutable style values for recipe timing,
color, intensity, easing, and caps. The board-scoped presenter subscribes to
one board and submits each bounded recipe to `PooledUiVfxLayer`; immutable
diagnostics and trace values make the result observable without exposing
mutable slots.

The pool is created and prewarmed outside the per-effect play path. Playing an
effect reuses an existing slot; it does not instantiate a particle prefab or
create a new runtime VFX GameObject. Every UI image has raycasting disabled.
Completion returns the slot, and disable, board teardown, testing reset, and
command recovery clear active presentation without affecting board state.

## Deterministic feedback mapping

| Semantic input | Bounded presentation |
| --- | --- |
| Accepted or rejected swap | Directional trail or restrained rejection pulse |
| Match wave | Magnitude-scaled board rings; no fabricated affected cells |
| Later cascade | Combo-depth response capped by the profile |
| Special creation | Creation ring at the authored board position |
| Row/column activation | Axis-aligned sweep recipe |
| Burst/color activation | Area or board response recipe |
| Major special combination | Explicit combination recipe, never coordinate inference |
| Reshuffle/recovery | Board response or immediate pool cleanup |

Event sequence, board positions, affected count, combo depth, special kind,
combination kind, booster kind, and plan hash are presentation inputs only.
The mapper never reads or writes the mutable board.

## Booster lifecycle contract

`GameplayMotionEvent` carries a `GameplayBoosterKind` value for:

- `BogatyrMaceRow`
- `BogatyrMaceColumn`
- `Firebird`
- `LivingLight`
- `SeaPearl`

An accepted booster publishes `BoosterActivated` only after validation and
inventory consumption. It publishes `BoosterCompleted` only after successful
presentation of its deterministic command. Invalid targets, unavailable
inventory, command contention, and planner failure publish `BoosterRejected`
without consuming inventory. Mace orientation is explicit; no listener parses
Russian UI copy or derives intent from coordinates.

`CoreLoopFeedbackService` maps lifecycle events once per booster command and
retains provider-neutral no-op SFX and haptic defaults. Step 2 adds no audio
module, clip dependency, Android API, or production provider.

## Preserved boundaries

- All 65 approved PNGs are byte-identical to v1.14 Step 1.
- GameplayCore, Chapter 1 configuration and localization, story services,
  input handling, UI geometry, packages, and the Unity version remain
  byte-identical to v1.14 Step 1.
- Match rules, booster outcomes and costs, level values, rewards, story routes,
  save schema, navigation, and approved art do not change.
- Target remains Unity 6000.5.9f1, Android portrait, reference 941×1672.

## Verification boundary

The static verifier parses project JSON, scans C# structure, checks mapping,
pool, raycast, cleanup, booster, and feedback contracts, discovers Unity tests
without executing them, compares protected content against the supplied v1.14
Step 1 baseline, and verifies the exact changed-file manifest.

The completed static pass checks 81 Step 2 source invariants, discovers 164
EditMode and PlayMode tests including 15 new Step 2 tests, counts 22 editor
self-checks, compares all 65 PNGs and 49 protected boundary files, and reports
zero failures.

`ProjectDocs/ARTIFACT_MANIFEST.csv` is the inherited source-delivery record;
the authoritative Step 2 delta is `V1_14_STEP02_CHANGED_FILES.txt`.

Unity Editor import/compilation, EditMode and PlayMode execution, editor
self-tests, visual capture comparison, allocation measurement, frame pacing,
and representative Android devices remain pending because Unity is unavailable
in the packaging environment.
