# Chapter Complete R1 — reference comparison

Compared the approved 941×1672 `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/ChapterComplete.png` with actual Unity R1 captures. This is a first implementation for review, not a claim of exact reproduction or user acceptance.

| Element | Implementation and remaining difference |
|---|---|
| Header and chapter ribbon | Original RGB and fixed Russian lettering, Scenario alpha, reference positions and native aspect ratios. Header extraction ends at the upper title band; the original long side framing is not reconstructed as a separate scenic border. |
| Ivan, firebird and ceremonial crest | Source RGB retained, Scenario alpha with local lower-edge cleanup. One decorative foreground group preserves pose, clothing, crest and bird details. Fine hair/feather edges and the bounded lower/right silhouette are not a complete reconstruction of hidden artwork. Three gold stars are ceremonial crest artwork, not a live three-star level rating; earned chapter stars appear in the numeric summary. |
| Environment | One new clean Scenario plate, framed to keep the cottage and onion-domed church visible. Building placement, clouds, foliage and lighting differ from the original. No old composite screenshots are used. |
| Reward/footer board | Original outer carving, bounded local wood reconstruction and independent controls. Wood grain and some inner dividers differ. No scenery-filled rectangular UI crops. |
| Rewards | Intentional functional adaptation: six live categories—gold, gems and four existing boosters—in place of the four reference samples. Base first-clear amounts come from claimed level rewards; the row grants nothing and is not the current wallet balance. Doubled-reward history is not available and is not inferred. |
| Continue and World Map | Original green/blue source buttons and Russian labels, native proportions and interactive Buttons. Both retain existing acknowledgement and World Map routing. |
| Next-chapter teaser | Original opaque framed forest illustration and Russian lettering, explicitly marked coming soon. No Chapter 2 levels or unlock feature were added. |
| English and dynamic text | Existing project serif font, live localizable text on locally repaired surfaces. The source typeface remains unidentified; embossing, exact face texture and lettering are approximate in English. |
| Responsive layout | Composition uniformly fits the safe area; the clean background covers the remaining display. Tall screens intentionally show scenery above/below the panel. |

Visually inspected final actual Unity reference-size Russian, tall English, reference-size English, tall Russian, and incomplete-state captures. Automated layout/raycast/text checks cover all six captures. Alpha QA sheets show assets on black, white and magenta. No calibrated color comparison, physical-device review or pixel-perfect equivalence claim.
