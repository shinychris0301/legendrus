# Team Ranking Reference Rebuild R1 — fidelity review

Compared the dedicated original (`Sources/OriginalReference.png`, 941×1672) with actual native Russian and narrow English Unity captures. **This is an improved implementation, not an exact reproduction.**

## Restored or aligned

- The shared Home player-header component supplies portrait, name, saved coins/gems/energy and native navigation. Its artwork remains the approved shared family.
- Original Russian title, three podium crests, rank medallions and seven guild crests are retained from the existing source-extraction pass.
- Details now uses an isolated original button including the complete Russian caption and top sapphire ornament. Visible RGB is unchanged; traced alpha is approximate. English retains the existing clean button with live lettering.
- Blue reward gems now directly reference the approved gameplay sapphire; no page-specific replacement was generated. Original rainbow rewards and accepted chests remain.
- Table rows are regularly spaced; the player-guild plaque follows its highlighted row. Footer controls follow the safe bottom and the shared header follows the safe top.

## Remaining differences

1. **Table and reward frames:** existing borrowed lacquer/frame sprites have heavier borders, different corner carving and brighter textured faces. The source table is simpler, darker and more continuous. The highlighted guild row also has a different blue surface.
2. **Typography:** retained user-selected fonts are wider/heavier than the source. Several guild names wrap, whereas the original uses single lines. Podium lettering and numeric weights are approximate. Exact source font identity remains unverified.
3. **Podium faces:** previous text removal left reconstructed title surfaces; their texture and lighting differ. The shared sapphire silhouette intentionally differs from the original page-specific currency artwork.
4. **Surroundings:** reused forest background and carved footer differ from the original navy backdrop, side scrollwork, red banners and candles. Back is a functional addition. Tall phones have extra scenery between the table and bottom controls.
5. **Data:** saved header values differ from reference examples. Unknown season dates, example disclosures and non-live membership/standings are deliberate. Local/Global and Week/Season currently show the same sample standings; they are not a live ranking service.

## Recommended next refinement

**R2 — Table Frames and Guild-Name Fit:** restore the original table/reward-slot border proportions and dark faces, then improve single-line name fitting using the selected embedded fonts. Reuse the shared header, gems and accepted crests; retain truthful sample labels. Footer banners and candles can follow as a separate bounded pass.

Actual captures are desktop Unity renders with simulated resolution/safe areas, not Android device screenshots. Automated checks establish layout and interaction behavior, not pixel identity.
