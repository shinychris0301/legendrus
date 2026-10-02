# Shared Booster Border Cleanup and Antique Gold R4

The user reported original metal remnants inside the right edge of the extracted boosters, then requested darker metal matching the original. Both corrections are applied to the shared resources.

## Changes

The old source-backing mask extended into the original right-hand diamond ornament and inner gold arcs. A tighter central mask and explicit side-ornament exclusions remove this residual metal. Original source subjects are restored at their original pixel coordinates above the repaired opaque navy backing, beneath the complete R3 rim.

The recreated metal is uniformly tone-corrected as a separate layer. Sampled source-gold median RGB is103/66/23; the same R3 metal sample changes from141/74/17 to107/65/23. This is a palette comparison, not proof of pixel-exact fidelity. The source sampling region and transform are documented in `Sources/Compose.py`, `Sources/Provenance.json` and `QA/MetalPalette.json`. Cool sapphire pixels and the ring alpha are unchanged; no geometry was regenerated.

Eight `_framed.png` and eight `_rim.png` files in `Assets/LegendsOfRus/Resources/ProductionUI/SharedBoosters/` change in place. Dimensions, GUIDs, importer settings, runtime loaders, prefab references, stock badges and functions remain unchanged. The eight standalone subjects are byte-identical. 87,792 centrally retained source pixels have zero RGB mismatches. Previews remain unavailable.

## Asset workflow

Reuses R3 Scenario generation and Pixa alpha. **Zero new API jobs and zero new credits.** This pass corrects extraction masks and the existing metal palette locally; it does not require another generated rim. No new generation prompt was sent. Reproduction script: `Sources/Compose.py`; previous resources are archived under `Sources/Before/`.

## Verification

Unity6000.5.9f1 final Verify02 import succeeded;13EditMode+21PlayMode=34passed, zero failures/skips. Browser7views+8comparisons passed. Final results and exact changed-file list are in [ScopeAndVerification](QA/ScopeAndVerification.json). Existing targeted Unity tests cover shared resource bindings, Booster Selection, Pre-Level, Pause and Failure. Verify01 tested residue cleanup; Verify02 tests the final darker metal as well. Separate test save directories protect normal local progress.

Alpha reviewed over black, white and saturated magenta. Magnified right-edge before/after comparisons show the removed inward metal tip and extra arc; the intended new outer diamond remains. All eight rim alpha channels and standalone subject files are preserved; cool sapphire RGB checks pass. Ring variants remain identical to each other.

Actual Unity page captures and browser checks are in the review. No Android build, physical-device test or full-suite run is claimed. Hidden backing pixels remain locally reconstructed; fine metal geometry is the previously created recreation, not exact original pixels. Enlarging the low-resolution source subjects does not add detail.

[Asset comparison](AssetReview.html) · [Actual Unity page review](SharedBooster_InnerBorderCleanup_R4_Review.html) · [Right-edge closeups: before above, after below](QA/RightEdgeBeforeAfter.png)
