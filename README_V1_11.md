# Legends of Rus v1.11 — Data-Driven Chapter 1 Levels

Open this folder with Unity 6000.5.9f1.

## What changed

- Ten curated Chapter 1 levels now load from
  `Assets/LegendsOfRus/Resources/Config/chapter01_levels.json`.
- Each level controls board dimensions, move limit, two color-collection goals,
  authored starting cells, difficulty tag, deterministic seed, score thresholds,
  first-clear rewards, and story triggers.
- The runtime board scales its rows and columns inside the approved, unchanged
  Gameplay board rectangle.
- Save version 3 persists selected level, highest unlock, per-level best stars,
  best score, attempts, retries, failures, completions, and reward-claim state.
- Story routing reads the completed level's trigger data.
- `Legends of Rus > Levels > Open Chapter 1 Authoring` edits and validates the
  JSON catalog without C# changes.

Authored `fixedCells` are deterministic starting cells. They participate in
normal matching and falling after the board starts; v1.12 introduces persistent
obstacle mechanics.

## Unity verification

1. Run `Legends of Rus > Tests > Validate Test Framework Setup`.
2. Run `Legends of Rus > Tests > Run v1.10 Step 4 Self-Tests`.
3. Run `Legends of Rus > Tests > Run v1.11 Level System Self-Tests`.
4. Open Test Runner and run all EditMode tests.
5. Open Test Runner and run all PlayMode tests, including
   `LevelSystemRuntimePlayModeTests`.
6. Run `Legends of Rus > Validate Commercial Vertical Slice`.
7. Open `Assets/Scenes/Bootstrap.unity`; play through level completion, retry,
   reward, dialogue, and Chapter Complete routing.

The approved page art, board bounds, gem style, and UI artwork were not changed.
