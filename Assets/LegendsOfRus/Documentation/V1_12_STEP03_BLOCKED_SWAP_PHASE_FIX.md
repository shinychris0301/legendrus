# v1.12 Step 3 — Blocked-Swap Phase Correction

## Symptom

Swiping through a chain, crate, Firebird feather, or empty cell correctly
returned a rejected resolution plan. The runtime had already entered
`GameplayPhase.Swapping`, but that phase did not allow a direct transition back
to `Idle`. `EnsurePhase` therefore reported a recovered invalid transition even
though the board was not corrupted.

## Correction

- `Swapping → Idle` is now an explicit state-machine transition for preflight
  rejection or cancellation before mutation.
- The rejected-plan branch records `LastRejectedCommandReason`, invokes
  `CommandRejected`, and returns to `Idle`.
- The branch no longer writes a warning for normal blocked input.
- No command transaction starts, no animation runs, no move is consumed, and
  the board snapshot remains unchanged.

## Regression coverage

- EditMode verifies the direct rejected-swap phase transition.
- PlayMode loads a deterministic empty-cell preset, attempts the blocked swap,
  and verifies `Idle`, available pointer input, no active command, identical
  move count, identical board hash, and the expected rejection reason.
- Existing obstacle tests continue to verify that chains, crates, objective
  items, and empty cells block swaps while ice remains swappable.

No level data or image asset changed in this correction.
