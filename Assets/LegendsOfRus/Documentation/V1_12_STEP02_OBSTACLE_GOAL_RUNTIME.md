# v1.12 Step 2 — Deterministic Obstacle Resolution and Goal Runtime

## Resolution contract

Obstacle effects are calculated in `BoardResolutionPlanner` against a cloned
working board. The authoritative runtime board remains unchanged until
`Match3Board` applies each immutable wave snapshot.

| Mechanic | Swap rule | Damage rule | Gravity rule |
| --- | --- | --- | --- |
| Ice | Gem remains swappable | One layer when its cell is cleared | Gem falls normally |
| Chain | Wrapped gem cannot be swapped | One layer when its cell is cleared | Does not divide the column; replacement gems may fall into the wrapper |
| Crate | Cell cannot be swapped and contains no gem | One layer from a direct clear or an orthogonally adjacent match | Stationary; divides its column into gravity segments |
| Firebird feather | Item cannot be swapped or cleared as a gem | Not damaged by clear effects | Falls with gravity and is collected only through the bottom exit |

An obstacle position can receive no more than one hit in a wave. A direct hit
takes precedence over an adjacent-match hit when both apply. Interactions are
sorted by board position before mutation, included in the plan hash, and exposed
as `BoardObstacleInteractionInstruction` records.

## Crate-aware collapse

Crates split a column into independent segments. Cells fall to the bottom of
their current segment and cannot pass a surviving crate. Only the segment that
is connected to the top of the board receives new gem spawns. Destroying a
crate reconnects the adjacent segments before that wave's collapse.

Objective items in the bottom-connected segment exit only when no gem or other
cell blocks them below. Each exit produces a
`BoardObjectiveItemCollection` record and removes the item before refill.

## Goal runtime

`LevelGoalRuntime` evaluates the same one-or-two goal records validated by the
schema-v2 level catalog.

| Goal | Progress source |
| --- | --- |
| `collect_tile` | `BoardResolutionWave.ClearedByTileType` |
| `score` | Absolute runtime score after the wave |
| `clear_obstacle` | Matching obstacle interactions whose final layer reached zero |
| `bring_down` | Matching bottom-exit collection records |
| `survival` | Successfully accepted swaps that consume a move |

The gameplay HUD now consumes generic slot-one/slot-two remaining values.
`RedRemaining`, `BlueRemaining`, and `GoalsChanged` remain as compatibility
aliases for existing tests and integrations. Runtime command transactions save
and restore a goal snapshot together with board, moves, score, and consumed
booster state, so a watchdog recovery cannot retain partial progress.

## Visual and content boundary

No image asset is added or changed. Runtime-only cell tints distinguish prototype
ice, chain, crate, and objective-item states, while public interaction events are
available for later approved VFX. The ten current Chapter 1 records are not
activated or rebalanced in this increment.

## Next development step

v1.12 Step 3 will author the Chapter 1 mechanic rollout, tutorial scripting,
difficulty peaks, and recovery levels using the completed deterministic runtime.
