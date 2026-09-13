# Reward and progress asset review

The four current R6 sources were inspected directly and rendered unchanged in local Chrome over black, white and saturated magenta. All four have genuine RGBA transparency. The reward frame's wood surface and progress trough are intentionally near-opaque. No baked transparency grid, scenery contamination or objectionable edge matte was found.

| Source | Actual dimensions | Near-opaque core bounds | Core size |
| --- | --- | --- | --- |
| quest_reward_frame.png | 1095×1437 | x189–900, y193–1245 | 712×1053 |
| quest_reward_gem.png | 1230×1278 | x281–945, y262–1058 | 665×797 |
| progress_frame.png | 2172×724 | x47–2127, y255–453 | 2081×199 |
| progress_fill.png | 2172×724 | x14–2161, y290–432 | 2148×143 |

Bounds use alpha≥245. Low-alpha antialiasing and incidental pixels extend farther; use each whole image and compensate its transparent margins in the transform. The actual source dimensions differ from the provisional dimensions in the delegation request. SHA-256, full alpha diagnostics and computed transforms are recorded in the neighboring JSON files.

To place the frame's visible core at reference (612,1488),114×158, the full source rectangle is approximately (581.74,1459.04),175.32×215.62. This changes the independent decorative frame's aspect by about6.7%. Preserving native aspect instead gives a106.83×158 core, centered within the requested114×158 area. Both options are recorded for the implementation owner.

The gem can preserve native aspect: a64.25×77 visible core centered at (671,1535) needs a whole-source rectangle approximately (611.78,1471.24),118.83×123.47. Relative to the exact-target frame rectangle, this is approximately (30.04,12.20),118.83×123.47. No sprite crop or pixel edit is needed.

Checks performed: original PNG and file-header inspection; direct full-source image review; local Chrome decoding, alpha histogram/bounds and three-background rendering; direct review of the resulting diagnostic image; current-source hashes; whole-source placement calculations. An initial Chrome capture rejected a fractional viewport height; rounding the viewport height fixed that diagnostic call. The source display and pixels were unchanged.

No production files, prefabs, metadata or source code were edited by this QA worker. No Unity compile, runtime capture, test suite, APK/AAB or phone test was performed. The candidate transforms still need native Unity placement and live-value clearance verification; this source review does not prove an exact final-page match.
