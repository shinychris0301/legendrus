# Gameplay R1 — reference fidelity review

Reviewed 2026-09-20. **Verdict: the six regular gems closely reproduce the source artwork, but the complete Gameplay screen is not an exact match.**

Compared the supplied 941×1672 original with the archived actual Unity Russian capture at the same resolution and the576×1280 safe-area capture. The original file matches the archived source hash, and all16current runtime sprite hashes match the recorded delivery. This review does not constitute a fresh Unity run. The earlier38passing checks establish tested behavior/layout contracts, not visual identity.

## Findings

| Area | Assessment | Remaining difference / action |
|---|---|---|
| Six regular gems | Strong source match | Original colors, silhouettes and emblems retained. They render slightly smaller within the cells than the reference; apparent sharpness is limited by the small original crops. Threefold interpolation does not add detail. Tune the cell-relative size before considering any new extraction. |
| Board placement and frame | Close at941×1672 | The outer frame occupies approximately the same reference area and the board remains8×7. Native internal cell spacing and rail treatment are different. |
| Board surface and junctions | Needs refinement | The current horizontal wood grain is much more prominent. The original has quieter, darker cell faces with more dimensional antique-gold edges. Current grid strokes are straighter/brighter and intersections look like small regular studs instead of the reference's ornamental joints. Recover the darker face treatment and joint shape while keeping the interactive grid. |
| Moves/goals/level typography | Approximate | Labels and numbers use live serif text with different letterforms, weight, shading and spacing. Moves28 is lighter; the level caption sits differently within its plaque. Exact source font identity remains unverified. Refine typography/material effects without baking dynamic values. |
| Score panel | Needs refinement | Empty stars are correctly empty at score0, but their outlines are thin and flat compared with the heavier beveled reference stars. The progress trough is more angular. The reconstructed bottom rail is conspicuous behind the stars. Repair the rail/star layering and match the star/progress silhouettes. |
| Booster cards | Frames close; content hierarchy differs | Source outlines and card positions are close. Retained artwork fills more of each card. Long approved names wrap into small captions near the lower ornament; the reference has larger, shorter labels. Adjust icon padding and label placement/size while retaining the approved names and functions. |
| Scenery at reference aspect | Largely aligned in exposed areas | City/river placement is close where original pixels remain. The regions reconstructed behind old UI differ and are not exact source scenery. |
| Scenery on tall displays | High-priority defect | A repeated skyline and horizontal transition appear above the board. Current layout uses one cover-scaled environment sprite, not a tiled background; the composition reveals reconstructed city imagery beneath the original exposed strip. Repair the continuous environment plate and inspect tall-aspect crops. |

## Intentional differences to preserve

- Existing Ivan face and the four established booster artworks/functions/names were explicitly retained.
- The character nameplate remains hidden per the earlier request; do not restore the sample name or the untracked32badge just to imitate the illustration.
- Score0, unearned stars, current round, inventory counts, different board arrangements and actual mission values reflect game state.
- Two missions at the start, increasing to three later, is the approved progression.
- The reference's rainbow piece is a special booster; it is not missing from the six-color regular gem extraction.

## Recommended next refinement

**Gameplay R2 — board rendering, score details and responsive scenery.** Keep the accepted six source gems. First repair the duplicate skyline, then match the quieter board faces and ornate junctions, then refine the score stars/progress and booster-label spacing. Check941×1672 and tall safe-area captures against the original before another art-generation pass. Use existing source pixels for recoverable details; Scenario is appropriate only for a bounded repair where the hidden environment cannot be recovered.

No production assets or code were changed during this assessment. No new Scenario calls or credits were used.
