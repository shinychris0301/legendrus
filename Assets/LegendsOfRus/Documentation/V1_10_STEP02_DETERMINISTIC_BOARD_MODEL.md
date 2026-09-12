# Legends of Rus — v1.10 Step 2

## Deterministic BoardModel Extraction

This step separates the authoritative Match-3 rules and state from Unity UI, touch input,
animations, scenes, economy, story, and save systems.

## New gameplay-core files

- `BoardPrimitives.cs`
  - `BoardPosition`
  - `BoardCellData`
  - `SpecialGemKind`
  - `BoardMatchRun`
  - collapse movement/spawn records
  - snapshots and replay tokens
- `BoardRandom.cs`
  - seeded random source
  - deterministic sequence source for tests
  - runtime seed utility
- `BoardModel.cs`
  - board generation
  - swaps
  - run detection
  - special creation
  - special-chain expansion
  - clear operations
  - gravity and refill
  - legal-move detection
  - dead-board recovery
  - snapshots and state hashes
- `BoardReplayCodec.cs`
  - replay-token decoding and validation

## Runtime integration

`Match3Board` remains the Unity controller/view layer, but it no longer stores the
logical tile or special-gem arrays. It delegates the puzzle state and rules to
`BoardModel`.

The approved Gameplay page, board position, gem presentation, special overlays,
and animation timings were not redesigned in this step.

## Deterministic QA

Each runtime board logs:

- random seed
- board state hash

A board snapshot can be encoded as a replay token containing:

- width
- height
- tile type count
- every tile
- every special type
- state hash

Open:

`Legends of Rus > Tests > Open Deterministic Board Debugger`

The debugger can generate a board from a seed, display its state hash, copy a replay
token, and validate that the token restores the exact same state.

## Automated tests

Run the immediate self-test:

`Legends of Rus > Tests > Run v1.10 Step 2 BoardModel Self-Tests`

Expected:

`22/22`

Run the full EditMode suite through Unity Test Runner. The new tests cover:

- same-seed determinism
- generation without starting matches
- guaranteed legal move
- non-mutating legal-move queries
- valid and invalid swap behavior
- horizontal four
- vertical four
- five in a line
- T and L intersections
- row/column special chains
- burst bounds
- color-special clearing
- clear metadata
- gravity
- refill
- special movement during gravity
- snapshots
- replay restoration
- dead-board reshuffling

## Acceptance criteria

Step 2 passes when:

1. The Step 2 self-test reports `22/22`.
2. All `LegendsOfRus.GameplayCore.Tests` EditMode tests pass.
3. A seed generated in the Board Debugger produces the same board and hash after reopening the tool.
4. Gameplay still uses the approved art and board bounds.
5. Swaps, cascades, special creation, gravity, refill, and reshuffle continue to work in Play Mode.
6. No Unity Console compiler errors are present.

## Next step

`v1.10 Step 3 — Deterministic Resolution Plans and Special-Combination Commands`

Step 3 will make the model return complete atomic resolution commands for the Unity
animation layer, including special + special swaps and reproducible cascade sequences.
