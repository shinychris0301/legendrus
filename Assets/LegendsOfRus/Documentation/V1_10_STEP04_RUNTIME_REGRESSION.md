# Legends of Rus — v1.10 Step 4

## Booster Validation, Combined-Special Balance, and PlayMode Regression

This milestone closes the v1.10 gameplay-stabilization sequence.

### Runtime command safety

Every swap and booster execution is now wrapped in a ten-second command watchdog.
The transaction records:

- board snapshot before the command
- move count
- score
- goal progress
- any consumed booster

If an animation coroutine stops, a state hash fails, or the command times out, the
runtime restores the exact pre-command board and counters, refunds the booster, clears
transient visuals, and returns to Idle.

### Per-wave state verification

The runtime verifies the authoritative board hash:

1. at plan start
2. before each resolution wave
3. after clear
4. after collapse
5. after the complete wave
6. after reshuffle
7. at final plan completion

A mismatch triggers immediate transactional recovery.

### Booster rules

Booster commands use one pure validator. Invalid, outside-board, or empty targets are
rejected without consuming inventory. Valid commands expose deterministic affected-cell
footprints for QA and future preview UI.

### Combined-special balance

The balance profile centralizes clear shapes and score-per-cell values. In particular,
Burst + Burst now uses one deterministic 5x5 footprint around the destination cell rather
than two overlapping 5x5 areas that could erase most of the board.

### QA presets

Deterministic presets are included for:

- Color + normal
- Color + row
- Color + column
- Color + burst
- Color + color
- Row + row
- Column + column
- Row + column
- Row + burst
- Column + burst
- Burst + burst

Additional booster presets cover all targeted boosters and an invalid empty target.

### PlayMode tests

The PlayMode suite calls the actual `Match3CellInput` pointer handlers. It verifies:

- up/down/left/right gestures
- tap then adjacent tap
- pause/resume preservation
- booster cancel without consumption
- invalid target without consumption
- valid target consumes exactly one
- timeout rollback and refund
- final board hash equals the planned final hash

### Unity test commands

1. `Legends of Rus > Tests > Run v1.10 Step 4 Self-Tests`
2. `Legends of Rus > Tests > Open Special Combination Preset Debugger`
3. `Window > General > Test Runner`
4. Run both EditMode and PlayMode test assemblies.

### Visual contract

No approved page, board boundary, gem style, or UI art was changed in this step.
