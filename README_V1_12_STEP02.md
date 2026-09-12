# Legends of Rus — v1.12 Step 2

## Deterministic Obstacle Resolution and Goal Runtime

This increment turns the v1.12 Step 1 mechanic contracts into deterministic
gameplay behavior while preserving the approved artwork and the current ten
released Chapter 1 level records.

Implemented:

- chain, crate, objective-item, and empty-cell swap blocking;
- ice and chain durability loss when their cell is cleared;
- crate durability loss from direct clear effects or adjacent match cells;
- at most one obstacle hit per cell in each resolution wave;
- crate-separated gravity and refill segments;
- Firebird-feather collection only through a bottom board exit;
- immutable obstacle-hit and objective-item-collection resolution events;
- plan hashing that includes mechanic interactions;
- generic one-or-two-slot runtime progress for `collect_tile`, `score`,
  `clear_obstacle`, `bring_down`, and `survival` goals;
- atomic goal-progress rollback with board, moves, score, and booster state;
- generic gameplay HUD slot events with legacy red/blue accessors preserved;
- non-asset prototype tints and runtime events for mechanic presentation;
- EditMode, PlayMode, and editor self-test coverage for the new behavior.

The released Chapter 1 JSON remains balance-identical to v1.12 Step 1: its ten
levels still have empty obstacle/item arrays and existing collection goals.
Mechanic activation, tutorial sequencing, and difficulty-curve tuning are the
next development step.

## Unity verification

1. Open this complete folder in Unity 6000.5.9f1.
2. Run `Legends of Rus > Tests > Validate Test Framework Setup`.
3. Run `Legends of Rus > Tests > Run v1.10 Step 4 Self-Tests`.
4. Run `Legends of Rus > Tests > Run v1.11 Level System Self-Tests`.
5. Run `Legends of Rus > Tests > Run v1.12 Step 1 Self-Tests`.
6. Run `Legends of Rus > Tests > Run v1.12 Step 2 Self-Tests`.
7. Run all EditMode tests, including `BoardObstacleResolutionTests`.
8. Run all PlayMode tests, including `LevelGoalRuntimeTests` and
   `LevelSystemRuntimePlayModeTests`.
9. Run `Legends of Rus > Validate Commercial Vertical Slice`.
10. Open `Assets/Scenes/Bootstrap.unity`, play the existing ten-level flow,
    and confirm its visuals and balance are unchanged.

Expected result: the project compiles without errors, every automated test
passes, Step 2 self-tests report success, and the released Chapter 1 flow still
matches the approved presentation.
