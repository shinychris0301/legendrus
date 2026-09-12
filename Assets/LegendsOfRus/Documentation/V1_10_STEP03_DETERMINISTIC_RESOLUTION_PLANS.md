# Legends of Rus — v1.10 Step 3

## Deterministic Resolution Plans and Special-Combination Commands

Step 3 removes the remaining puzzle-rule decisions from Unity animation coroutines.

The pure GameplayCore layer now produces one immutable `BoardResolutionPlan` for every accepted board command. The runtime board consumes that plan visually, step by step.

## New GameplayCore files

- `BoardResolutionPrimitives.cs`
- `BoardResolutionPlanner.cs`

## A resolution plan contains

- command type: swap or booster
- accepted, invalid-swap, or rejected status
- source and destination cells
- deterministic resolution seed
- board snapshot before the command
- board snapshot after the visual swap
- one or more cascade waves
- matched runs and matched cells
- special-gem creation instructions
- special-gem activation instructions
- exact cleared cells, including their tile and special type
- cleared count by tile type for goals
- score delta for each wave
- board snapshot after clear / special placement
- exact falling-cell commands
- exact spawned-cell commands
- board snapshot after collapse
- dead-board reshuffle result
- final board snapshot
- deterministic plan hash

## Runtime rule

`Match3Board` no longer decides which cells should clear or which special should be created while an animation is running.

Runtime sequence:

1. Request a deterministic plan.
2. Animate the attempted swap.
3. Apply the plan's after-swap snapshot.
4. Animate each special activation and clear command.
5. Apply the after-clear snapshot.
6. Animate special creation.
7. Apply the after-collapse snapshot.
8. Animate the supplied move/spawn instructions.
9. Apply the final snapshot.

This keeps board rules independent from frame rate, coroutine timing, button timing, or VFX duration.

## Supported special combinations

- Color + normal gem
- Color + row special
- Color + column special
- Color + burst special
- Color + color special
- Row + row
- Column + column
- Row + column
- Row + burst
- Column + burst
- Burst + burst

Special effects can trigger other specials. The plan records every activation separately so the view can play the correct effect without recalculating the rule.

## Booster plans

The following targeted boosters now use the same planning pipeline:

- Bogatyr Mace — row
- Bogatyr Mace — column
- Firebird Feather — area
- Living Light — selected color

The booster inventory is consumed only after the planner accepts the target.

Sea Pearl remains a move-counter command because it does not mutate board cells.

## Determinism and QA

The planner never changes the source board while creating a plan. It creates a private working model from the current snapshot and a resolution seed derived from:

- board dimensions
- current board state hash
- command signature

The same board snapshot plus the same command produces the same:

- plan hash
- clear cells
- special commands
- falling commands
- spawned tiles
- final state hash

F12 QA logging now includes the last resolution-plan hash together with board seed, board hash, and board replay token.

## Editor tools

### Self-test

`Legends of Rus > Tests > Run v1.10 Step 3 Resolution Plan Self-Tests`

Expected result:

`19/19`

### Resolution Plan Debugger

`Legends of Rus > Tests > Open Resolution Plan Debugger`

The debugger accepts a board replay token and can inspect either a swap or booster command. It displays every wave and all state hashes.

### Test Runner

Run the EditMode assembly:

`LegendsOfRus.GameplayCore.Tests`

The new `BoardResolutionPlannerTests` cover invalid swaps, valid waves, deterministic plan hashes, four-match creation protection, special combinations, booster commands, source-board immutability, and exact final-snapshot application.

## Visual contract

This step does not change:

- approved Gameplay page
- 941 x 1672 design coordinate system
- board bounds
- selected v1.9 gem presentation
- UI navigation art
- button art

It changes only the ownership and sequencing of puzzle rules.

## Step 3 acceptance gate

- Invalid swap returns without a move cost.
- Valid swap produces at least one atomic wave.
- Planner does not mutate the source board.
- Same snapshot + command gives the same plan hash.
- Special creation cells are protected from their originating clear.
- Special combinations work without requiring a normal match.
- Booster plans do not consume a move.
- Goal deltas come from the plan, not from runtime cell queries.
- Falling and spawn animation use planner instructions.
- Every applied stage is hash-checked in the runtime, and the final runtime board hash equals the plan's final snapshot hash.
