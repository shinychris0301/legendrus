# Detailed Failure R6 reference review

## Verdict

**The implementation matches the overall composition and art family, but it is not yet a close reproduction of every reference element.** R6 corrected the title footprint. The most actionable remaining problems are the footer overlap, uneven painted offer-card baselines, missing goal-heading enclosure and the crowded booster/nameplate area. Do not restart the page or undo the accepted title/main-panel distinction.

## Evidence examined

Original941×1672 `Art/ReferenceOnly/ApprovedScreens/Fail.png`; R6 actual Unity captures `Fail_941x1672_RU`, `Fail_Level10_ThreeGoals_RU`, `Fail_Level10_ThreeGoals_576_EN` and `Fail_BoosterGuide_RU`. The guide capture was used to inspect explanation placement, not as a stable-color comparison because it captures a different interaction state. Inspected active Failure runtime/view/panel-fitting source, prefab ownership and existing Unity results. Current source/prefab/assets match all1,827 recorded R6 production hashes; all98 prior evidence hashes match. The reference hash also matches recorded provenance.

Numerical current rectangles below come from implementation/source dimensions. Reference painted bounds and relative visual offsets are approximate observations, not exact segmentation. No percentage fidelity score or calibrated color/blur claim is justified.

## Findings

| Area | What matches | Remaining discrepancy | Priority |
|---|---|---|---|
| Title panel | Centered691×240 including crest closely follows the estimated690×240 reference footprint. Top sapphire and navy/gold family fit. | Different clipped corners, crest/scroll shapes and texture. Live title lettering appears narrower/lighter than the painted reference. Title-side removal was explicitly requested; do not restore those ornaments merely to copy the original. | Low for size; medium for typography/details |
| Main panel | Original ornate panel is restored; large enclosing navy surface and scrollwork belong to the reference family. | Individual curls/jewels differ; material looks somewhat brighter/bluer in broad areas. The main frame should be preserved during subsequent fixes. | Low/medium |
| Goals heading/inset | Two/three goals are centered with visible space below the top border; icons and counts no longer touch. | Reference has an outer heading frame and separate inset; current heading floats above one panel. Current inset643×185 is wider/taller than the visually estimated reference inner area near600×165. This removes the nested structure and changes the amount of empty space. | High for fidelity |
| Goal icons/counts | Current level's actual goal types/counts render truthfully. Icon/count separation is visibly improved. | Current88px maximum icons are smaller than the roughly100px reference examples. Exact icon designs differ because they use the existing Gameplay art. Different mission symbols and counts are not missing designs or bugs. Do not replace real missions with reference sample gems. | Medium styling; data differences intentional |
| Retry button | Size/position are close; slim emerald face, gold ends and live text work. | Emerald is more saturated and the faceted face more pronounced than the reference; bevel/lettering differ. This is a visual judgment, not a measured universal brightness excess. | Medium/low |
| Paid/ad offer pair | Correct red/blue family, green lower faces and left/right grouping. Live titles and symbols are present. | Painted bodies and green button faces do not share a baseline. Equal324×136 slots fit paid art to118.108px tall and ad art to132.574px. The ad crest further offsets its face. The ad title/status stack is crowded and less prominent than the reference. Align actual visible content at native proportions, not only equal RectTransforms. | High |
| Offer state and price | Explicit unavailable labels avoid false functionality. | Reference shows price40 and available continuation; current price is a dash and both buttons are disabled. This is a deliberate pending-feature state, not an accidental fidelity defect. Integration remains scheduled in FAIL-CONT-01 after UI design completion. | Deferred behavior |
| Booster heading/row | Heading is now inside its border. Four established illustrations and attached stock badges remain. | Medallions are166px versus reference circles around180–190px visually; the row starts roughly20px lower. Long names use two lines at19px rather than the reference's shorter single-line labels. Small captions become difficult at phone width; lower plates approach the outer carving. Retain current booster identities/names/functions. | High for phone readability |
| Bottom chapter-map action | Existing route remains accessible. | Its panel visibly covers the lower center ornament/crest. The original has a clean continuous footer and no button in that location. This is the clearest remaining overlap. Reposition the existing action without deleting it or redesigning the main frame. | Highest |
| Ivan/environment | Hero, ruined village, castle, dark navy night and warm rim light establish the intended scene. | Face/pose, cloak folds, embroidery/pendant and building placement differ. Current cloak/face are more evenly lit; the reference has stronger localized shadows. The castle sits farther right in the implementation. These are asset-level differences, not rect-alignment fixes. | Medium/later art pass |
| Header | Correct portrait/name/resource/settings structure and single sun coin. | Portrait pendant, plus signs and fine framing differ. Empty XP rail lacks the reference blue fill because there is no backed XP value. Current level/energy are actual state; reference28 and0/30 must not be hard-coded. | Medium details; data differences intentional |
| Tall screen | Inspected EN safe-area capture has no hard scene seam or clipped main controls. | Long labels and offer availability text shrink substantially. This is a simulated Unity display, not proof of physical-device readability. | Device verification pending |

## Recommended next focused refinement

1. Resolve the chapter-map panel's collision with the lower ornament; preserve the main frame and current route.
2. Align the two offer bodies and green faces using their actual painted bounds, and give the ad title/status enough vertical separation. Keep both disabled until the scheduled integration.
3. Restore the goal section's nested heading/inset structure, keeping data-driven icons centered.
4. Rebalance booster-row space and long labels, then assess small text on a phone. Retain names/artwork and correct badge counts.

Only after those layout/readability corrections should a separate typography/material pass be considered. Avoid global darkening: bright emerald/Firebird elements and dark structural panels need independent judgment. Do not regenerate Ivan or the main panel automatically.

## Checks this review performed and did not perform

Performed now: instruction/task/source/reference inspection; four saved Unity portraits viewed;1,827 production and98 evidence hash checks; source/PNG dimension comparison for offer scaling; inspection of existing R6 results (10/10 EditMode,18/18 PlayMode). Added this review and geometry/evidence JSON; updated CurrentTask/SessionLog. A first dimension-check invocation lacked Pillow in the default interpreter; retry with system `/usr/bin/python3` succeeded without installing anything.

**No new Unity run, build, test suite, capture, production edit, art generation or image alteration.** No APK/AAB/device test, browser interaction, manual gameplay/animation check, calibrated image metric, exact-font identification or live ad/payment check. Existing test passes validate their specified contracts, not full visual fidelity: they do not establish painted-card baseline alignment or footer ornament clearance.
