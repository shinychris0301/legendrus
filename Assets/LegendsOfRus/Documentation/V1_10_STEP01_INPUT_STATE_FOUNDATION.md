# v1.10 Step 1 — Input and Gameplay-State Foundation

## Status

Implemented as the first gated substep of v1.10 Gameplay Core Stabilization.

## Why this step comes first

Recent device tests exposed recurring input-direction and overlapping-command defects.
Before the board model is extracted, the project needs one authoritative direction
mapping and one authoritative interaction state.

## Implemented

### Authoritative board direction

`BoardDirectionResolver` is now the only place that converts a normalized gesture into:

- Left
- Right
- Up
- Down

Board row zero is at the top, therefore:

- Up = `(0, -1)`
- Down = `(0, 1)`

Both continuous drag and fast-flick fallback use the same resolver.
Pointer positions are converted into each cell's local RectTransform coordinates before
normalization, so Canvas scaling and phone aspect ratio cannot silently reverse axes.

### Gameplay state machine

The board now tracks explicit phases:

- Idle
- Selected
- BoosterTargeting
- Swapping
- InvalidSwapReturning
- ResolvingMatches
- CreatingSpecials
- ActivatingSpecials
- Falling
- Refilling
- Paused
- Completed
- Failed

Pointer input is accepted only during Idle, Selected, and BoosterTargeting.
A second tap/swipe cannot enter during swap, cascade, special activation, or falling.

### Pause protection

Pause is accepted only while the board is in an interactive stable phase.
If a swap/cascade is active, the player is asked to wait for the move to finish.
The Pause modal now has a full-screen raycast blocker, so board cells cannot receive
input through unused areas of the modal.

Opening Settings from Pause no longer rebuilds the Gameplay screen. Returning from
Settings restores the same paused board.

### Tests

Two test paths are included:

1. Unity Test Framework EditMode tests under:
   `Assets/LegendsOfRus/Tests/EditMode`
2. A dependency-free Editor menu self-test:
   `Legends of Rus > Tests > Run v1.10 Step 1 Self-Tests`

The self-test validates all four directions, top-origin row deltas, and the main state
transition/input-lock sequence.

## Unity validation procedure

1. Open the project in Unity 6000.5.9f1.
2. Wait for script compilation.
3. Run:
   `Legends of Rus > Tests > Run v1.10 Step 1 Self-Tests`
4. Expected result:
   `v1.10 Step 1 self-tests passed: 14/14`
5. Optionally open:
   `Window > General > Test Runner`
6. Run the EditMode tests in `LegendsOfRus.GameplayCore.Tests`.
7. Open `Assets/Scenes/Bootstrap.unity` and test:
   - tap selection
   - tap adjacent cell
   - swipe left/right/up/down
   - invalid swap return
   - pause/resume
   - booster targeting pause/resume

## Acceptance gate

Step 1 passes when:

- all 14 self-tests pass,
- all EditMode tests pass,
- no second input enters during a swap or cascade,
- up/down/left/right mapping is correct,
- Pause cannot be clicked through,
- Pause/Settings/Resume preserves the same board.

## Next substep

v1.10 Step 2 will extract the tile grid, swap validation, match detection, gravity,
refill, and legal-move detection into a deterministic BoardModel independent of the
Unity view and animations.
