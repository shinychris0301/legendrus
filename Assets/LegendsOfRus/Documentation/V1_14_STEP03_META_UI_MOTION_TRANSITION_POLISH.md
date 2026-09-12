# v1.14 Step 3 — Meta UI Motion and Transition Polish

## Boundary

Step 3 changes presentation only. It does not modify approved PNGs, reference
geometry, safe-area behavior, navigation destinations, Chapter 1 data, puzzle
rules, rewards, economy, story order, save schema, packages, or scenes.

## Runtime architecture

- `MetaUiMotionPresenter` owns the only Step 3 `Update` clock. It uses clamped
  `Time.unscaledDeltaTime` and a fixed 128-slot registration array so motion
  continues while gameplay is paused and never allocates a collection per frame.
- `MetaUiTransition` drives bounded CanvasGroup, scale, and offset recipes. Opaque
  full-screen screens and modals use alpha-only recipes; only native runtime
  overlay panels use small offsets or scale response.
- `AnimatedHotspot` no longer starts a coroutine per tap. It resets its latch
  before callback dispatch, restores on pointer exit/disable, owns one pointer,
  and can be tapped repeatedly when the underlying action is repeatable.
- `MetaUiActionGate` allows one delayed action per screen or modal root. Rapid
  sibling taps and Escape cannot dispatch competing routes or rewards.
- `MetaUiMapNodeFeedback` prewarms one non-raycast diamond pulse on the two
  inherited map interaction regions. It follows the accepted pointer without
  reading or changing selected, unlocked, completed, or rewarded state.

## Screen and modal lifecycle

Outgoing screens are deactivated before deferred destruction. Incoming screen
hotspots remain non-raycast until their entry completes. A generation token
prevents a stale Loading coroutine or interrupted modal continuation from
changing a newer route.

Each supported modal has two layers:

1. A full-screen blocker that remains raycast-active from open request through
   exit completion.
2. An alpha-animated visual/interaction group whose hotspots are disabled during
   opening and immediately disabled when closing begins.

Only Pause, Settings-from-Pause, Out of Energy, Booster Select, and Reward Claim
are accepted as modal routes. Missing art rolls construction back atomically.
Pause stays paused through Pause → Settings → Pause, while Resume, Restart, and
Exit continuations run once after the blocker finishes exiting.

## Outcome and reward isolation

- Level completion/failure is persisted before Victory/Fail presentation.
- Daily reward is granted before Reward Claim opens; closing that modal grants
  nothing.
- Chapter Complete acknowledgement remains in the existing story service and is
  never called by an animation callback.
- Motion types contain no `GameState`, `StoryService`, `MonetizationService`,
  `Match3Board`, `PlayerPrefs`, or mutable gameplay dependency.
- The inherited Mail development reward stays one-shot per visible Mail root;
  repeatable, state-guarded reward controls still revalidate their business rule.

## Map limitation retained by the boundary

The approved map screens are baked full-screen skins and the baseline exposes
one broad interaction region per map, not individually authored runtime nodes.
Step 3 therefore adds local tap response to those exact existing regions without
fabricating levels, changing selection, or painting potentially contradictory
locked/completed stars over the approved art. Truthful per-node state and
individual node hitboxes require a later native map reconstruction milestone.

## Verification

Static verification covers deterministic styles/traces, bounded durations,
single-clock registration, no motion coroutines, non-raycast flourishes,
exactly-once gates, transition cleanup, modal blocking, presentation dependency
isolation, exact release delta, and protected-file hashes. Unity compilation,
Unity Test Runner, screenshots, allocations, and Android frame pacing remain
pending because the Unity Editor is not available in this environment.
