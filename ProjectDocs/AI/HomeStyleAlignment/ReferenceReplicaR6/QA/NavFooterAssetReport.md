# Home R6 navigation and footer sprite delivery

Six selected standalone PNGs are saved in `../Sources/`: map, shop, events, characters, mail and reward. Each built-in generation call used the complete original `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Home.png` as an actual reference image. No screenshot crop, manual pixel edit, source recolor or code-generated replacement was used. `../NavFooterPrompts.json` records every prompt, input/output path, dimensions, hashes and selected/rejected status.

All six final files are 1254×1254 RGBA. Their project copies match the original tool output bytes. Final navigation cards share the same repainted frame geometry; their blank caption centers agree within one source pixel. The mailbox retains ample blue around a small parchment envelope. The gift is a closed domed wood/gold/blue chest in three-quarter view. Live labels and badges are absent from the sprites.

Performed: original whole-reference inspection; direct output inspection; file-header alpha checks; read-only PNG decoding for caption geometry; local Chrome display and alpha diagnostics; direct inspection over black, white and saturated magenta; original-output/project-copy SHA-256 and byte equality checks. The opaque navy interiors are intentional. Final sprites have clean outer transparency, without a baked grid, scenery contamination or an objectionable matte.

Initial attempts that produced RGB with a baked transparency grid were rejected. One wide helmet and two cards with inconsistent frame geometry were superseded. The first local Chrome attempt timed out in the restricted sandbox; the authorized local Chrome run completed. A `python` command was unavailable, then the script preparation used `python3`. Pillow was unavailable for a supplemental read-only check; the existing Node PNG decoder completed the measurements. No dependency was installed.

## Layout guidance

`NavCaptionGeometry.json` contains conservative per-card caption rectangles. All four share approximately x251–1004, y913–1048 with center (627,981). Source main frames are approximately x84–1167, y185–1130. Top jewels reach y49; side ornaments approximately x32–1221. The same full-source transform can now align the row and captions. The implementation owner received these measurements.

The broad rectangle includes the caption's inward angled corners. A second read-only check, `NavGlyphBandGeometry.json`, restricts measurement to y940–1025, a candidate actual glyph-height band. Bounds are map x204–1050, shop x202–1052, events x204–1051 and helmet x203–1050. At224×255 this is approximately151–152px wide and17.3px high. Actual Unity glyph extents must choose the usable band; the broader bound must not cause unnecessary shrinking. One requested caption-widening trial returned RGB/grid and was rejected. Further widening was paused for this read-only audit and actual glyph testing; selected sources remain unchanged.

At uniform full-source size224×224, caption center174 requires y≈−1.24 inside the existing203×222 hit region, x−10.5. This produces a shorter main panel than the original reference. A224×255 display with y≈−25.49 better matches the original main panel shape but vertically stretches the independent repaint by about14%. This is a visual tradeoff for full-page review, not proof of exact reconstruction. Labels must fit their measured bounds in actual Unity, including Cyrillic glyph extents and shadow.

## Limits

The generated details are close reference-guided repaints, not pixel-exact assets. Fine filigree, map markings, helmet engravings, chest fittings, inset texture and highlights still differ. The selected gold is somewhat brighter and cleaner than the original aged reference in places. The shared frame includes a top sapphire on every card; the source reference does not show every corresponding ornament identically. Source alpha QA alone does not establish page fidelity.

No production files, prefab, metadata, C#, service, route, Android setting, package or signing setting was edited by this asset worker. No Unity compile, runtime capture, Play/Edit Mode test, APK/AAB build or physical-phone test was performed by this worker. Parent integration and actual portrait review remain necessary.
