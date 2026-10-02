# Gameplay R7 — band beneath the portrait decoration

The remaining band came from the unused bottom tail of the reconstructed header wing behind the portrait. That tail included a second thin rail and partially transparent dark pixels below the portrait ornament. A small dark residue also remained on the portrait's lower edge.

Removed the wing tail (local x0–114/y173–185) and trimmed12 lower-portrait matte pixels outside its gold contour. The portrait jewel and corner scrolls, main header rails, layout and live values remain intact. This is an alpha-only repair:192 header pixels and12 portrait pixels changed, with zero RGB changes or changes outside the recorded support masks. [Asset provenance](Sources/AssetProvenance.json).

Two new versioned sprites reuse the existing Scenario-derived artwork; no new API requests or credits. Old artwork remains unchanged. The actual Gameplay prefab and its owning builder agree at **revision18**. Five existing Assets files modified, five added, zero deleted; all405 prefab records retained. [Scope audit](QA/ScopeAndVerification.json).

## Verification

Unity6000.5.9f1 Build01 compiled, rebuilt and read back the canonical prefab. Final Verify02: **9 Edit Mode +32 Play Mode =41 passed**, zero failed/skipped. The new regression check renders the scene both normally and with the header backing hidden, then compares the exposed scenery beneath the portrait. All616 sampled pixels match exactly; the previous R6 capture differed at62 pixels. [Pixel result](QA/BandPixelCheck.json). The control uses the same Unity rendering and texture sampling; it does not compare directly against raw texture pixels.

Inspected black/white/magenta alpha previews and the actual Unity portrait-edge detail. The review includes before/after native crops and full Russian/English/small/tall captures. All16 browser views load without errors or horizontal overflow at390px.

No Android build, physical-device verification or full-project suite ran. Existing typography and reconstructed-art limitations remain. Other page prefabs, booster captions/art, gems and board colors are unchanged. Next: user visual review of the corrected portrait edge.
