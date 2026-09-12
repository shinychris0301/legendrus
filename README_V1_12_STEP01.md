# Legends of Rus — v1.12 Step 1

## Goal and Obstacle Data Foundation

This increment extends the verified v1.11 data-driven level system without
changing approved artwork, released Chapter 1 balance, or current gameplay UI.

Implemented:

- Chapter catalog schema v2 with backward-compatible schema-v1 loading.
- Goal contracts: `collect_tile`, `score`, `clear_obstacle`, `bring_down`, and
  `survival`.
- Obstacle authoring records for ice, chains, and crates with validated layers.
- Bring-down item records for the Chapter 1 Firebird feather.
- Deterministic board layers for stationary obstacles and movable objective items.
- Versioned v2 snapshot/replay encoding with v1 replay decoding preserved.
- Chapter 1 authoring controls for goals, obstacles, and board items.
- EditMode, PlayMode, and editor self-test coverage for the new foundation.

The ten released Chapter 1 levels still use their v1.11 collection goals and
empty mechanic arrays. Persistent damage rules, goal evaluation, obstacle art,
and mechanic tutorials are the next v1.12 implementation step.

## Unity verification

1. Open this complete folder in Unity 6000.5.9f1.
2. Run `Legends of Rus > Tests > Validate Test Framework Setup`.
3. Run `Legends of Rus > Tests > Run v1.10 Step 4 Self-Tests`.
4. Run `Legends of Rus > Tests > Run v1.11 Level System Self-Tests`.
5. Run `Legends of Rus > Tests > Run v1.12 Step 1 Self-Tests`.
6. Run every EditMode test, including `BoardMechanicStateTests`.
7. Run every PlayMode test, including `LevelSystemRuntimePlayModeTests`.
8. Run `Legends of Rus > Validate Commercial Vertical Slice`.
9. Open `Assets/Scenes/Bootstrap.unity` and verify the existing ten-level flow.

Expected result: all tests pass and the current Chapter 1 flow looks and behaves
exactly as it did in v1.11.
