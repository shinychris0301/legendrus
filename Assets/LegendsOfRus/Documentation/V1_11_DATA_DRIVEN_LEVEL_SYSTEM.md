# Legends of Rus — v1.11 Data-Driven Chapter 1 Level System

## Runtime data contract

`chapter01_levels.json` is the Chapter 1 source of truth. `LevelData` records
contain stable identity, board dimensions, gem count, moves, deterministic seed,
difficulty, goals, authored starting cells, first-clear rewards, three score
thresholds, and story triggers.

The v1.11 goal adapter supports one or two `collect_tile` goals because the
approved Gameplay HUD has two goal slots. New goal kinds and persistent board
obstacles belong to v1.12.

## Board creation

`LevelBoardFactory` builds a deterministic board for the level and attempt,
applies authored cells, then rejects any initial board with a match or no legal
move. Authored cells are starting placements, not immovable obstacles.

The visual board rectangle remains x=79..865 and y=357..1299 in the 941×1672
design space. Row and column counts scale only the cells inside this boundary.

## Progression and rewards

Save schema v3 keeps a per-level record with best stars, best score, attempts,
retries, failures, completions, and first-clear reward state. Completion unlocks
the next catalog record and selects it. Replays can improve best score and stars
but cannot grant the first-clear reward again.

Legacy v1/v2 saves migrate their chapter, next level, and aggregate stars into
the v3 shape. Level selection and progression are clamped to the current
Chapter 1 catalog.

## Authoring and validation

Open `Legends of Rus > Levels > Open Chapter 1 Authoring`. The editor window
loads the JSON, exposes every v1.11 field, validates stable IDs and progression,
and only saves a valid ten-level catalog.

Run `Legends of Rus > Tests > Run v1.11 Level System Self-Tests` for catalog,
lookup, deterministic board, initial-match, and legal-move checks. PlayMode tests
cover dynamic dimensions, selection, stars, unlocks, retries, first-clear reward
idempotency, save reload, and data-driven story routing.

## Visual contract

No approved page, board boundary, gem style, or UI art changed in v1.11.
