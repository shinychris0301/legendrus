# v1.9 — Vertical Input and Runtime Gem Art Fix

## Vertical swipe correction

The runtime board stores row `0` at the top and increases row indices downward.
Unity pointer coordinates increase upward.

Therefore:

- finger moves **up** → board delta `(0, -1)`
- finger moves **down** → board delta `(0, +1)`

v1.8 used Unity's `Vector2Int.up/down` names directly and therefore inverted vertical swaps.
v1.9 applies the board-coordinate mapping in both the drag path and the fast-flick fallback.

## Gem art restored

The v1.8 transparent reconstruction is no longer used for ordinary runtime gems.
The game again uses the compact faceted set from the earlier gameplay build:

- `Gameplay/Gems/Gem_Ruby`
- `Gameplay/Gems/Gem_Sapphire`
- `Gameplay/Gems/Gem_Emerald`
- `Gameplay/Gems/Gem_Amber`
- `Gameplay/Gems/Gem_Amethyst`
- `Gameplay/Gems/Gem_Topaz`
- `Gameplay/Cell_DarkGold`

This set is now treated as the locked runtime gem presentation, not a temporary prototype.
The v1.8 movement, match-4/match-5 special creation, cascade animation, and booster animations remain enabled.
Special overlays still use the approved row/column/burst/color artwork.

## Regression test

In Gameplay, test a gem away from the board edges:

1. Swipe upward: the selected gem must exchange with the cell visually above it.
2. Swipe downward: it must exchange with the cell visually below it.
3. Left/right behavior should remain unchanged.
4. Create four in a row and four in a column to verify special orientation.
5. Use the Mace with an up/down target gesture to verify column targeting.
