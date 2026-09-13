# Direct visual review — final R8

Inspected the complete original Home reference, accepted R7 Unity portrait and final R8 Unity portraits at 941×1672 RU/EN, 576×1280 safe area and 1080×2400 safe area. Also inspected the standalone viewer's magnified Play rendering and original-reference lettering.

The new lighting gives the upper/left letter contours a narrow gold highlight and the opposite contours a bronze recess. Text stays centered and readable inside the existing emerald button in both languages and at narrow/tall portrait sizes. The reference retains different serif shapes, painted gold texture and broader sculpted highlights; R8 is not a claim of exact font or pixel identity. The existing font and complete button PNG are deliberately preserved in this focused pass.

The first render was too subtle, so the sample width was increased from two to 3.5 local text units (approximately 1.5–1.75 design pixels after the retained text scale). Independent roundtrip comparison exposed unstable sampling at font-atlas boundaries; continuous per-glyph boundary coverage corrected it. Final actual Russian images are pixel-identical before and after the combined display-size and language sequence.

Final Home versus accepted R7: 4,904 changed pixels, zero outside the 321×66 Play-ink review rectangle at (311,1093). Other artwork, gold HUD, footer, badges and scenery are unchanged. Splash/Loading reference-size comparison also reports zero changed pixels.

The standalone page shows one selected complete portrait. Its optional magnifier fits the whole word within narrow windows, up to 2× on wider windows. Browser screenshots are viewer evidence; the separate UnityVerification PNGs remain unaltered Unity renders. No raster assets were generated or edited, and no new transparent sprite required acceptance.

This is an implementation review, not user approval or physical-phone evidence. Android GPU appearance and performance remain untested.
