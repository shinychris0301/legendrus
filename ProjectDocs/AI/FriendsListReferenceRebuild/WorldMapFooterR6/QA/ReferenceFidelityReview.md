# Friends List R6 — reference fidelity review

## Verdict

The overall art family and five destination meanings match, but the result is **not an exact visual match**. World Map metalwork is genuinely reused; the center composition, selection appearance and sizing need another pass.

Reviewed the saved Verify06 Unity capture at 941×1672 against `../../R1/Sources/OriginalReference.png`, and inspected the footer enlarged 2× in a browser. The enlarged image is a crop of an existing native capture, not a fresh Unity run or newly generated artwork. Inspected the current runtime source and its World Map sprite references. No production edits or new Unity run in this review.

## Findings in priority order

1. **Map still looks partly selected.** Its outer frame is the gold World Map guild frame, but its inserted map face retains a bright blue inner arc. Friends is the only destination assigned the active outer frame in code, yet the screenshot does not communicate that distinction cleanly. The previous statement that only Friends is highlighted was incomplete visually.
2. **Center symbols fit too tightly.** The event scroll approaches the upper ring, and the character helmet's upper tip is clipped by the inset. These are crops of existing complete medallions, not newly isolated complete emblems. The source rectangles are mapped to the same 144×130 opening with different aspect ratios, so symbol proportions also change slightly. Preserve whole silhouettes at their own aspect ratios, with space beneath the metalwork.
3. **Footer scale and spacing differ.** World Map uses 236-reference-pixel-high full medallions; Friends scales them to 174 (about 74%). That preserves each frame's aspect but makes the circles and caption plates smaller and increases gaps between them. This was a fit choice, not an exact reproduction of World Map's footer geometry. Any enlargement must clear the Sort/count/remove controls above it.
4. **Captions differ from the reference.** They remain live/localizable and fit within their plates, but the installed title font is heavier and the longer Map/Characters captions become smaller than the short captions. Exact source typeface remains unverified.

## What matches / is intentionally adapted

- Five distinct destinations are now visible: map, shop, event scroll, friends and helmet. The duplicate guild-crest rendering failure is resolved in the reviewed capture.
- Metal frames reference the actual World Map `nav_guild_blank`, `nav_shop_blank` and `nav_map_blank` assets. Shop retains its full original face. The Friends footer's original metal frames were deliberately replaced at the user's request; restoring those original Friends rims would contradict that direction.
- The original Friends title plaque, portrait family, ornate panels and green/blue action buttons remain recognizable.
- Preview wording, sample counts, search/Find behavior and saved resource values differ from the mockup for functional reasons. They should not be replaced with invented live data to reproduce the image.
- Across the rest of the page, the darker Home scenery, heavier live typography and shifted row geometry still differ from the warm original Friends reference. R6 did not change these areas.

## Recommended next pass

**Friends List R7 — Footer symbol isolation and sizing.** Keep the approved World Map metalwork. Remove the copied Map selection arc; fit complete scroll and helmet symbols inside the frames without stretching or cropping; balance circle size, spacing and captions within the available footer. Retain all routes, localization and safe-edge placement.

The earlier 15 passing Unity checks cover native bindings, layouts and navigation; they do not establish pixel-level fidelity. New visual acceptance should inspect the complete footer and enlarged edges in both languages.

[Enlarged native footer inspection](FooterInspection2x.png)
