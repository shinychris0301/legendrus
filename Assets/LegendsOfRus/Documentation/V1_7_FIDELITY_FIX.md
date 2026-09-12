# v1.7 — Runtime Fidelity Fix

## What the screenshot exposed

The portrait design image was correctly fitted inside the Game view, but fixed-size
runtime UI elements were scaled using the whole wide Canvas. The static approved
screen therefore stayed small while the runtime Match-3 board grew much larger.

## Fix

The CanvasScaler now follows the same limiting dimension as the fitted portrait viewport:
- wide editor / landscape container -> match height
- tall portrait phone -> match width

The Match-3 board no longer uses GridLayoutGroup fixed pixel cell sizes.
Every live cell is anchored proportionally inside the exact approved board rectangle.

The approved Gameplay screen contains an 8-column x 9-row board, so the runtime board
now matches that exact structure.

## Button response

Invisible interaction rectangles remain aligned over the baked approved controls, but
they now provide a restrained premium response:
- ~3% press compression
- subtle antique-gold flash
- 65 ms tactile press hold
- 140 ms screen fade-in

The approved underlying artwork is never recolored or replaced during these animations.

## Visual tile fidelity

The temporary flat prototype gem icons are replaced by runtime cell sprites reconstructed
from clean cells on the approved Gameplay page:
Ruby, Sapphire, Emerald, Amber, Amethyst and ornamental Shield.
