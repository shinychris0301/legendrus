# Legends of Rus — v1.12 Step 3

## Chapter 1 Mechanic Tutorials and Difficulty Curve

This increment completes v1.12 by activating every finished goal and obstacle
contract across the ten Chapter 1 levels. The approved screen skins, gameplay
board bounds, gem presentation, and image assets remain unchanged.

Implemented:

- schema-v3 `difficultyRating` and `tutorialSteps` level data;
- one-at-a-time mechanic debuts for matching, score, ice, chains, crates,
  Firebird-feather bring-down, and survival;
- deterministic tutorial triggers for level start, first move, first goal
  progress, first obstacle hit, and first objective-item collection;
- a gameplay tutorial overlay that pauses input, preserves the approved layout,
  and resumes only after the current queue drains;
- save-version 4 tutorial completion so dismissed steps do not interrupt replays;
- local peaks on levels 3 and 6 followed by recovery levels 4 and 7;
- a Chapter 1 progression validator enforcing rollout order, tutorial coverage,
  recovery drops, move relief, and finale difficulty;
- Unity authoring support, PlayMode regression coverage, and a Step 3 editor
  self-test suite.

Runtime correction:

- blocked chain, crate, objective-item, and empty-cell swap attempts now return
  directly from `Swapping` to `Idle` without logging a recovery error;
- rejected attempts spend no move, start no command transaction, and preserve
  the board hash;
- `CommandRejected` and `LastRejectedCommandReason` expose expected rejection
  feedback without treating normal input as a fault.

## Unity verification

1. Open this complete folder in Unity 6000.5.9f1.
2. Run `Legends of Rus > Tests > Validate Test Framework Setup`.
3. Run the v1.10 Step 4, v1.11, v1.12 Step 1, and v1.12 Step 2 self-tests.
4. Run `Legends of Rus > Tests > Run v1.12 Step 3 Self-Tests`.
5. Run all EditMode tests.
6. Run all PlayMode tests, including `Chapter01MechanicCurvePlayModeTests`,
   `LevelGoalRuntimeTests`, and `LevelSystemRuntimePlayModeTests`.
7. Run `Legends of Rus > Validate Commercial Vertical Slice`.
8. Open `Assets/Scenes/Bootstrap.unity` and play levels 1–10 in order.
9. Confirm each new mechanic's start tutorial and triggered follow-up, the
   easier recovery pacing on levels 4 and 7, and Chapter Complete after level 10.
10. Compare the core screens at 16:9, 18:9, 19.5:9, and 20:9.

Expected result: the project compiles without errors, all automated tests and
self-tests pass, the ten-level progression reaches Chapter Complete, and the
approved artwork remains pixel-identical to the prior milestone.
