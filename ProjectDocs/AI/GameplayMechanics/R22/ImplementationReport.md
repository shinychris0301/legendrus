# R22 — Distinct item creation choreography

The user asked for more impressive item animation after R21. The main remaining defect was that every item assembled from the same detached triangle pattern. R22 replaces that pattern with motion specific to the approved item artwork and separately animated, textured accents.

- Directional items: the two end sections approach the growing central crystal, with the entire choreography oriented to the row/column direction.
- Bomb: a compressed silhouette charges and expands, with paired amber ignition plumes.
- Color item: a turning, widening prism with opposing prismatic energy sweeps.
- Firebird: a connected mesh unfolds the wings; feather-shaped flame trails separate outward during the reveal.

Matched-gem gathering remains R21's curved motion. Completion lasts 0.52s (previously 0.40s): 0.32s to finish the pose, followed by a 0.20s seamless full-sprite hold. The independent accents fade through completion. Same-wave items finish together. The final body is a single quad with the exact approved sprite, size and orientation; no different generated item replaces it.

## Artwork and runtime integration

Built-in imagegen generated one transparent atlas using the approved sapphire line item and amber Firebird as style references. The output was copied unchanged to `Assets/LegendsOfRus/Resources/ProductionUI/MechanicsEffectsR22/CreationAccents.png`. The original generated file remains in Codex's generated-images directory. [Exact prompt/provenance](Sources/ArtPrompt.txt). No Scenario or CLI generation was used.

The 1254×1254 RGBA atlas has four independently sampled regions with measured unequal gutters. Transparency was inspected on black, white and saturated pink in [AssetReview.html](AssetReview.html); no opaque scenery/backplate was found. Approved item sprites were not edited.

`SpecialCreationGraphic` owns a reusable `CreationAccentsGraphic` sibling behind its body. Both are Canvas-compatible and use the existing board RectMask2D; neither intercepts input. Each active effect has two accent quads (8 vertices); bodies use 12 directional vertices or up to 256 connected-pose vertices, dropping to 4 for the final hold. Finish, reset, disable and destruction clear the layer. These are bounded implementation counts, not a measured Android performance result.

`Match3BoardCreationR21` passes the actual special kind to the choreography. `MechanicsPlaytestLog` records PresentationR22. Existing runtime-created sandbox composition remains the owner, so there is no serialized production prefab/builder to regenerate for this pass.

## Scope and limitations

Implemented for match-created PrototypeR4 directional, bomb, color and Firebird items. Core matching, affected cells, chain rules, scoring and move budgets are preserved. Campaign remains Legacy. Activation/combination and rainbow-conversion effects retain their preceding presentation. The separate activation pipeline described in R20 is not finished by this creation pass.

This is a new visual candidate, not a claim of Royal Kingdom parity or user acceptance. Android profiling and a deliberate campaign rollout remain outstanding. The review contains sampled actual Unity portraits, normal/half-speed animated WebP, and all five R21 comparisons. The sampled intervals are retained; there is no frame interpolation, audio capture or phone frame-rate claim. Synthetic fixture HUDs can show 49/4 moves, a pre-existing fixture-caption mismatch.

## Verification

Unity focused art run: 7/7 passed. Final combined PlayMode run: 83/83 passed, zero failures/skips. Browser430/1440: 164 frame loads, 20 animations and 10 R21 comparisons passed, no JavaScript errors or overflow. Review uses 82 actual Unity portraits and ten animations. No EditMode or physical Android run.

See [QA/Verification.json](QA/Verification.json) for actual Unity results and [QA/BrowserVerification.json](QA/BrowserVerification.json) for review checks. Verification uses Unity 6000.5.9f1 with the previously authorized process-only Vulkan override and descriptor limit 4096. Project graphics settings are not changed.

The new lifecycle test checks all five item orientations/types, bounded clipped accent meshes, one reused layer, exact final four-vertex art and clearing on finish/disable. Existing creation tests cover same-wave completion, recovery, color/Firebird identity, final state hashes, moves and restored input.

[Scope against task-start inventory](QA/Scope.json) records actual changed files, preserving unrelated local edits. Next visual slice: independently authored directional activation body/plume/trail/contact using the R20 timing contract, after reviewing this full creation transition.

Final task-start scope: four modified source/test files, five new asset/source/meta files, zero deleted, 7434 unchanged under Assets/Packages/ProjectSettings. Cleaned only test-created scene/linker artifacts. Pruned405 unreferenced broad-regression numbered PNG samples with SHA256 manifest; retained dedicated review sequences, before/after regression evidence, logs and results.
