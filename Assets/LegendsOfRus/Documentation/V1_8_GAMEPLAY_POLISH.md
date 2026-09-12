# v1.8 — Gameplay Input, Match Animation and Booster Polish

## Input

The previous implementation decided swipe direction only on pointer-up using raw screen pixels.
That made vertical gestures less reliable on the fitted portrait board.

v1.8 resolves movement while the pointer is actually dragging and normalizes X/Y movement by
the real screen-space width and height of each cell. A movement of roughly 20% of one cell is
enough to commit the swipe. A fast flick that skips the drag callback is still handled on release.

Tap-select + tap-adjacent remains supported.

## Movement animation

Valid swaps now slide the two gem visuals between fixed ornate cells over ~130 ms.
Invalid swaps slide across and return rather than snapping.

## Match feedback

- normal matches pop and fade
- cascades fall into place
- 4 horizontal -> row special
- 4 vertical -> column special
- 5+ -> color special
- T/L intersection -> burst special
- special creation receives a pulse/glow animation
- triggered specials chain into each other

## Boosters

Bogatyr Mace:
- tap a target -> clear its row
- vertical swipe on a target -> clear its column
- animated gold/blue line sweep

Firebird:
- target a cell -> clear 3x3
- fire pulse animation

Living Light:
- target a gem -> remove every gem of that color
- warm board flash

Sea Pearl:
- immediate +5 moves
- blue water/light flash

A targeted booster is consumed only after a valid target is chosen. Tapping the same booster again cancels it.

## Visual fidelity

The temporary prototype geometry was replaced with transparent runtime gems reconstructed from clean examples
on the approved Gameplay page, placed over a stable dark-gold cell well derived from the same design language.
