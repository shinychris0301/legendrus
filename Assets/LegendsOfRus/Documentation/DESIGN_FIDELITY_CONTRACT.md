# Design Fidelity Contract

## Non-negotiable
The selected approved screen image is the visual acceptance target.

## Vertical-slice implementation
For maximum initial fidelity the full approved design is rendered as the primary screen skin.
Interactive areas are transparent Unity Buttons placed at normalized coordinates on the same
9:16 reference viewport.

This is intentionally different from the previously failed approach of auto-cutting presentation
sheets into contaminated sprites.

## Runtime reconstruction rule
When a baked design area is replaced by dynamic Unity UI:
1. Recreate that component independently.
2. Match the original material, scale, spacing and ornament.
3. Capture the runtime screen.
4. Compare it to the approved PNG using the included comparator.
5. Do not merge the change if visual drift is obvious.

## Responsive rule
- Approved 9:16 composition remains intact.
- The viewport fits inside `Screen.safeArea`.
- Taller devices receive extra surrounding space instead of zooming the approved screen.
- Hitboxes live under the same aspect-controlled viewport, so buttons remain aligned.
- Test 16:9, 18:9, 19.5:9 and 20:9.

## Reference assets
`Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/`

They are not future source material for sprite extraction.
They are acceptance references and temporary exact visual skins for the commercial vertical slice.
