# Events R2 — Card Frames and Reward Scale

Compared the original Events image, R1's final Unity capture and R2's final Verify03 Russian capture at 941×1672. R2 addresses the approved frame/reward pass; it is not a claim of full reference fidelity.

## Corrected

- **Rectangular corner notches:** the old Scenario frame alpha retained opaque navy patches inside its scrollwork. The new overlay clears those patches, preserving the RGB and a narrow dark bevel around the metal. Independent scenery now appears through the openings.
- **Scenery fit:** the artwork window extends four reference pixels closer to the top/bottom rail, with a rounded native stencil to prevent square image corners spilling through the frame. Original foreground aspect ratios are retained.
- **Frame spacing:** the card boxes are closer to the reference rail positions. Tops changed from385/735/1069/1347 to381/729/1064/1343; heights from335/320/266/254 to342/332/277/258. The last card still leaves space for the required Back control.
- **Reward prominence:** twelve individually cropped reward sprites replace four padded group displays. Firebird's gem, chest and medal are approximately28%,16% and19% larger in linear scale than R1. Each icon has its own aspect-preserving fit. Captions, actions and lower rails remain clear.
- **Compact descriptions:** the short fourth card uses bounded native font fitting to accommodate its two-line localized description above the larger rewards. English/Russian captures and glyph-fit tests pass.

[Diagnostic original/R1/R2 details](FrameAndRewardComparison.png) · [Frame alpha](FrameAlpha.png) · [Reward alpha](RewardAlpha.png) · [Source integrity](SourceIntegrity.json).

## Preserved / remaining

The exact source Russian title and shared Home header remain unchanged. Live balances and the energy indicator remain native. Events are still upcoming; reward rows are labelled examples, and only Firebird opens a preview.

The frame still reuses the source third-card ornament family instead of four independently extracted original frames. Fine carving, source scenery/lighting, Firebird glow, selected-tab texture, live typography and the continuous footer surround remain different. This pass improves the visible join defects and reward hierarchy without claiming to finish those separate refinements.

Recommended next visual pass: **Events R3 — Scenery and Firebird Glow**, preserving the title, shared header and corrected frame/reward layout. Source extraction should preserve visible original art; use Scenario only for missing scenery/light where needed.
