# Gameplay R3 — header typography and mission readability

Implemented the approved refinement in the canonical Gameplay prefab and its builder at revision 14. [Open the review](Gameplay_HeaderReadability_R3_Review.html).

## What changed

- Moves, score, level and headings use stronger serif lettering. Live gold text has a restrained cream-to-gold gradient and a smaller dark shadow. Parchment labels and mission counts remain dark ink without gold shading.
- Mission icons now occupy 60×64 reference pixels and align above their counts. Counts moved up 13 pixels, giving the parchment's lower border more room. Two missions use balanced 130-pixel spacing; three use 100-pixel spacing. The icon and count rectangles are separate and share a center line.
- Booster captions increased from 17 to 18 reference pixels and use bold lettering, retaining the approved wording, two-line layout and existing artwork. Inventory numbers have stronger weight.
- Numeric font fitting handles longer values. A small Gameplay-only mesh effect contains glyph overhang and shadows after Unity's normal best-fit calculation. It applies to moves and score, only adjusting geometry when it exceeds the inset text area; regular values retain their normal size. It does not change displayed values or game state.

All text remains live and localizable. The six gems, board rendering, scenery, character, booster artwork/functions and saved gameplay behavior are unchanged. No new raster assets, font files, Scenario calls or credits were needed.

## Verification

Unity 6000.5.9f1 Build02 compiled, rebuilt and read back revision 14. Final Verify02: **9 EditMode +31 PlayMode =40 passed**, zero failed or skipped. Existing gameplay, Pre-Level, input, inventory, reshuffle and VFX checks remain covered.

Added checks verify rendered header glyph bounds, mission icon/count separation, containment within the parchment, and matching center lines. A controlled UI fixture exercises 999 moves, 9 999 999 score, level 500, a five-digit mission count, a completion checkmark and three missions in both Russian and English at 438×780 and 576×1280 with a simulated safe area. These are deliberately injected display values; the captures do not claim a playable level 500, a real score award or matching star progress.

Verify01 caught a three-digit move value extending roughly 1.47 screen pixels beyond the text area's left edge at 438 pixels, exceeding the test's one-pixel allowance. The numeric overhang fitter corrected this; the same checks pass in Verify02. Diagnostic results are retained.

Actual Unity standard, three-mission, English and tall-phone captures were inspected. The standalone review's 14 image choices load at 390-pixel browser width without script errors or horizontal overflow. No Android build, physical-device run, full-suite execution or performance profiling was performed.

## Scope

[Scope audit](QA/ScopeAndVerification.json): six existing Assets files modified, two added, zero deleted. All 395 old prefab records are retained; 397 total, including the two numeric-fit components. Only Gameplay.prefab changed. Existing image, font and metadata hashes are unchanged: [artwork audit](QA/UnchangedArtwork.json).

Modified: ProductionGameplayHudStyle, ProductionGameplaySourceControls, ProductionGameplayView, the owning Step20 prefab builder, Gameplay.prefab and GameplayAlignmentPlayModeTests. Added ProductionGameplayNumberFit.cs and its metadata. Reapplication checks pass without duplicate objects or effects.

## Font limitation

The shared typography configuration still has no RegularFont or StartFont assigned. This Linux Unity session resolves the existing **OS Liberation Serif development fallback**; the log reports that explicitly. This pass retains that existing resolver and does not redistribute an OS font or mark its identity as exact. The rendering shown here is therefore not proof of Android font parity. A project-bundled, approved font must be assigned and verified before release typography can be considered complete. Existing bundled ranking fonts are available for a separately reviewed binding decision.

The live typography remains an approximation of the reference. Two-line booster names and centered mission counts also intentionally differ from its shorter captions and painted layout. No pixel-exact claim is made.
