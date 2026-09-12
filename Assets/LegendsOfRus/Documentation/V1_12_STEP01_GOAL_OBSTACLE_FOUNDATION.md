# v1.12 Step 1 — Goal and Obstacle Data Foundation

## Data contract

`chapter01_levels.json` now uses schema version 2. Every level has explicit
`obstacles` and `boardItems` arrays. Schema version 1 remains readable so old
level files and replay investigations do not fail abruptly.

Supported goal records:

| Goal type | Parameter | Meaning |
| --- | --- | --- |
| `collect_tile` | `tileType` | Clear a number of gems of one type. |
| `score` | none | Reach an absolute score above `startingScore`. |
| `clear_obstacle` | `obstacleType` | Clear authored ice, chain, or crate cells. |
| `bring_down` | `itemType` | Bring authored objective items to an exit. |
| `survival` | none | Complete the authored number of moves. |

Obstacle types are `ice` (one or two layers), `chain` (one or two layers), and
`crate` (one to three layers). The first board-item type is
`firebird_feather`, matching Chapter 1 story content.

## Deterministic state

`BoardModel` now stores three additional deterministic layers:

- stationary obstacle kind;
- obstacle durability/layers;
- movable objective item kind.

Obstacle layers remain fixed when gems swap or fall. Objective items occupy a
cell instead of a gem and move with cell collapse. Snapshot hashes include the
new layers only when mechanic state exists, preserving v1.11 hashes for legacy
boards. Replay tokens use `v2|` only when mechanic state is present, while the
decoder continues to accept the original token form.

## Scope boundary

This step establishes contracts, validation, authoring, persistence in board
snapshots, and factory mapping. It does not activate obstacles in released
levels. Damage rules, blocking rules, goal progress, exit collection, runtime
visuals, and tutorials belong to v1.12 Step 2.
