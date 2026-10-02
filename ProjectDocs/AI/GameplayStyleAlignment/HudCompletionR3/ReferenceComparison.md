# Gameplay R3 — reference comparison

Compared the canonical [Gameplay reference](../../../../Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Gameplay.png), the existing R2 portrait and actual R3 captures. The supplied gem/booster/obstacle/state/variant sheets remain the art source; their production mappings and original resources were preserved. R3 improves header/footer completeness and feedback readability. It is not an exact match or a new acceptance decision.

| Reference element | R3 result and remaining difference |
|---|---|
| Upper lacquer and gold border | Now a dedicated opaque panel with layered rails and carved ends. The generated carving is heavier and more regular than the reference's fine, distressed filigree. |
| Ivan/name/level | The existing portrait is larger and the labels are grouped beside it. Source artwork is preserved; costume, framing and facial treatment still differ from the reference. |
| Moves and stars | Centered R2 medallion and larger cream moves count; existing live star and progress bindings retained. Medallion filigree and inactive-star detailing remain different. Empty progress at a new level is actual state. |
| Goal slots | Larger supplied gems and lower-right counts are more legible. Score and survival now have distinct symbols. Reference sample counts 15/18 are not forced onto the authored level's 12/12 goals. |
| Booster row | The four supplied medallions remain; emerald stock badges now have larger cream faces with a short shadow. Their artwork/lighting still differs from the older page reference. |
| Gear and score panel | Dedicated round gear and sapphire-ended carved score plaque replace the earlier sparse treatment. R3's larger leaves, blue oval ornaments and rounded bronze well are new interpretations; the reference has finer branching filigree and diamond center ornaments. |
| Messages | A single slot above the board prevents older stacked toasts from obscuring boosters. This is a functional addition rather than an element visible in the idle reference. |
| Board and scenery | Outside the changed HUD/footer regions the reference-size R2/R3 captures are byte-identical at pixel level. Existing scenery composition and gem/metal brightness still differ from the reference. |
| Action lighting and obstacles | This pass does not implement the reference's central golden activation burst or new booster/obstacle effects. Existing rectangle/wash VFX, mixed directional special families, and incomplete ice/chain/crate damage artwork remain the next work. |

The reference depicts an activation moment; the idle capture intentionally shows the live starting board. Extra brown shields/round tokens are drawings in the reference, not evidence of supported new tile mechanics. Exact font, carving, source-lighting and pixel identity remain unverified.

Review [one whole portrait at a time](Gameplay_HudCompletion_R3_Review.html), including safe-area and live-goal variants. The [state specification](ComponentAndStateSpecification.md) covers the next supported booster/special/obstacle pass and keeps unsupported mechanics separate.
