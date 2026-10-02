# Original Booster Rims R2

The user identified that badge removal changed the original circular metal band. The R1 repair mirrored whole image patches across the lower circles; this could copy parts of the icon face into the ring and alter its apparent engraving and thickness.

The band is now extracted as a separate transparent annular sprite for all eight icons. The centre artwork is excluded from that layer. Visible original metal, side ornaments and sapphires are retained. Repair is confined to the two badge footprints: corresponding source rim pixels supply the missing arcs, while hidden navy faces use separate local source inpainting with metal excluded from its donor pool. This prevents the whole-icon mirrored patches used in R1. Outer residue is removed with a constrained silhouette.

The eight shared `*_framed.png` assets now incorporate these corrections. Their existing GUIDs and dimensions are preserved, so Booster Selection, Pre-Level, Pause and Failure use the updated appearance through their existing shared bindings. Eight `*_rim.png` files with transparent centres are also supplied independently. Tool-only icons, counts, functionality and preview availability are unchanged.

The flattened reference does not contain the band pixels hidden by the stock/plus badges. Those small arcs remain reconstructed; an exact recovery of the hidden pixels is not claimed. **154,066 visible pixels outside the repair footprints have zero RGB differences from the source.** This verifies retained color, not pixel-perfect alpha or missing-region reconstruction.

The previously accepted Scenario alpha is reused. **0 new API calls / 0 CU**; no subject regeneration. The alpha resources were inspected over black, white and magenta. The final native Booster Selection capture was inspected, and native Pre-Level, Pause, Failure and tall/English views are supplied in the review.

Verification: Unity6000.5.9f1 import passed; **13 EditMode + 21 PlayMode = 34 targeted tests passed**. Browser checks passed for seven page/reference views and eight asset comparisons. No Android/device or full-suite run.

Scope against the start-of-task hash snapshot: eight framed PNGs modified, eight rim PNGs plus eight metas added, no files deleted. Runtime code, prefab bytes, existing metas, tool-only PNGs and gameplay data are unchanged. Existing builders continue referencing the same canonical sprites; no serialized revision change is needed for this texture-only update.

[Native review](SharedBooster_OriginalRims_R2_Review.html) · [Original / isolated band / corrected icon](AssetReview.html). See Sources/ExtractRims.py and Sources/Provenance.json for the reproducible masks and measurements, and QA/ScopeAndVerification.json for exact paths and test outcomes.
