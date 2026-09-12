# v1.14 Step 1 — Core Loop Motion and Feedback Foundation

## Outcome

The deterministic board model and resolution planner remain the rule authority.
`Match3Board` now converts an accepted `BoardResolutionPlan` into an ordered,
immutable presentation timeline. Listeners can add audio, haptics, particles,
camera response, or telemetry without mutating puzzle state or depending on
coroutine frame order.

## Intermittent gem-input diagnosis

The reported “gem sometimes does not move” symptom had multiple interaction
shell causes rather than a nondeterministic board rule:

- Each old cell input rectangle was visually inset on all four sides. Across an
  8×9 board, those seams left approximately 6.2% of the nominal board surface
  without a cell raycast target.
- Booster hitboxes started at design y=1290 while the board ended at y=1299,
  so the bottom nine design pixels of the final row were intercepted.
- Drag committed at 0.20 cell without requiring a stable dominant axis. Small
  diagonal movement could choose a blocked or unintended neighbor.
- A gesture marked itself sent before `Match3Board` confirmed command
  acceptance, and pointer ownership had no cancellation, focus-loss, pause, or
  multi-touch lifecycle.
- Display-only dynamic-value masks remained raycast targets and could intercept
  UI input.

The input root now covers the full nominal cell while the approved visual inset
is preserved in a non-raycast `CellContent` child. Booster hotspots begin below
the board. Drag and release share one dominance rule, the sent latch follows the
board's Boolean acceptance result, and board-wide pointer ownership is released
on pointer-up, cancel, disable, focus loss, and application pause. A touch begun
while a cascade finishes may commit only after the board returns to an
interactive phase; resolution commands still never overlap.

## Deterministic presentation timeline

| Board stage | Motion hooks |
| --- | --- |
| Accepted swap | `SwapStarted`, `SwapCommitted` |
| Blocked swap | `SwapRejected` plus deterministic nudge, no mutation or move cost |
| Invalid swap | `SwapReturnStarted`, `SwapReturned` |
| Resolution wave | `MatchWaveStarted`, `MatchWaveCompleted` |
| Later wave | `CascadeStarted` with increasing combo depth |
| Special | `SpecialActivationStarted`, `SpecialCreationStarted` |
| Collapse/refill | `FallStarted`, `FallCompleted` |
| Dead-board recovery | `ReshuffleStarted`, `ReshuffleCompleted` |
| Runtime recovery | `CommandRecovered` |

Every event carries a monotonic sequence, primary and secondary board
positions, combo depth, affected count, special type, and resolution-plan hash.
The fall animation captures immutable start offsets and alpha values before its
first frame, removing the previous frame-rate-dependent interpolation path.

## Feedback and failure boundaries

`CoreLoopFeedbackService` exposes replaceable `ICoreLoopSfxSink` and
`ICoreLoopHapticSink` interfaces. The shipped sinks are no-ops; no audio module,
clip, Android API, or device vibration call is required. SFX and vibration
settings are read for every signal and gate their providers independently.

Later cascade waves emit one wave-level feedback cue rather than duplicate
cascade and match pulses. Special activations aggregate their provider request
per deterministic activation batch.

Provider exceptions, feedback observers, motion observers, command-rejection
observers, and legacy score/move/goal/obstacle/item callbacks are isolated per
listener. A failing presentation consumer cannot strand an active command.
Planner failures before mutation return to a deterministic interactive state,
and watchdog recovery removes tracked transient line/flash objects before
restoring the authoritative snapshot.

## Preserved boundaries

- All 65 approved PNGs are byte-identical to v1.13 Step 3.
- GameplayCore, balance, level, story, localization, package, and project-version
  boundary files are byte-identical to v1.13 Step 3.
- No match rule, level value, reward, story route, save schema, or approved art
  asset changed.
- The project remains Android portrait, Unity 6000.5.9f1, reference 941×1672.

## Verification

Static verification parses every project JSON file, scans all C# sources for
balanced structure, checks Step 1 source contracts, discovers 149 EditMode and
PlayMode tests, counts 23 new targeted tests and 19 editor self-checks, compares
the 65 PNGs and 38 protected boundary files against v1.13 Step 3, and verifies
the exact changed-file manifest.

Unity Editor import, compilation, EditMode, PlayMode, device frame pacing, and
visual capture comparison remain pending in Unity 6000.5.9f1 because the editor
is not available in the packaging environment.

