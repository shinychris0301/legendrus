# Gameplay original-reference rebuild R1 and fresh gem extraction

Implemented in the canonical Gameplay prefab at revision 12. Source: `/home/k2/Downloads/match3final/077-fantasy_russian_match_3_adventure.png` (941×1672), SHA-256 `bfa56b2fe39373e958afa963fff554c74263374bc184958594c5c4646f0e675a`.

[Standalone review](Gameplay_ReferenceRebuild_R1_Review.html) · [Gem transparency](QA/GemsAlpha.jpg) · [Source integrity](QA/GemSourceIntegrity.json) · [Scope and results](QA/ScopeAndVerification.json)

## Fresh gems

Six regular gems were isolated again through Scenario: red teardrop, blue crystal, green square, gold coin, purple teardrop and orange hexagon. Original source RGB is retained under the returned alpha. Source crops are enlarged threefold by interpolation, which adds no recovered detail. No saturation adjustment was applied. Black, white and magenta inspection backgrounds show their transparent edges. Source-integrity checks confirm that every output RGB pixel equals its input crop and that all six inputs reproduce from the original image.

ProductionGameplayArt now resolves these sprites for the board and shared mission icons, including Pre-Level. The visual footprint is slightly larger while retaining aspect ratio, cell hit areas and breathing room. Existing gem identities, matching rules and mission accounting are unchanged. Existing rainbow/bomb/line-special artwork remains intact; those are boosters, not the six regular gems requested for re-extraction.

## Surrounding approved Gameplay rebuild

Source controls supply the portrait ring, moves frame, parchment goals, score frame, round plaque, board perimeter, wood booster cards and gear. Native interactive controls and live localized values remain active. The original exposed city scenery is preserved, with Scenario repair behind removed UI. There is no full-screen screenshot with invisible hotspots.

The board remains 8×7. Score starts at zero. Two/three missions use actual level definitions. Existing Ivan portrait and four booster artworks, names and functions remain intact; no character name or sample level-32 badge is shown. Existing native grid junctions, obstacles, oriented specials without companion fragments, reshuffling and effects are retained.

## Extraction and repairs

16 accepted Scenario jobs, 67 recorded CU: fourteen alpha isolations at 3 CU each (including six gems =18 CU), one hidden-scenery reconstruction at13 CU, and one material repair sheet at12 CU. Requests, accepted job IDs, outputs and billing are under Sources/Scenario. No duplicate submissions.

The automatic goals and board masks selected the contents rather than the perimeter, so those masks were replaced locally. Original RGB remains on the recovered ornamental frames. Occupied panel faces required clean lacquer/parchment/wood repairs; a generated material sheet supplies these bounded areas and the opaque board substrate. The score's star-obscured bottom rail is reconstructed from its reflected original top rail; sample star fragments are removed. The portrait's example badge is removed by a bounded reflected-rim repair. AssetProvenance.json records these operations. 358,423 fully exposed scenery pixels are unchanged.

## Verification

Unity 6000.5.9f1 compiled, rebuilt and read back Gameplay revision12. Final Verify05: **8 EditMode +30 PlayMode =38 passed, 0 failed/skipped**. Covers Gameplay/Pre-Level alignment, RU/EN, safe areas, authored objectives and obstacles, twelve resolved moves at438px, specials without companion fragments, button/cell raycasts, status placement, VFX pools, booster activations, inventory transactions and recovery cleanup. VFX captures use a controlled fixture, not a normal production round.

Build03 subsequently corrected only reference provenance metadata and rebuilt/read back the same layout. Its log is retained. The native screenshots are from Verify05. Earlier failures exposed an obsolete Home revision assertion, long booster-label overflow, and a floating-point boundary excess smaller than0.0001px. These were corrected; the size assertion now permits0.001px calculation tolerance while maintaining the90% constraint.

Source RGB/reproducibility/transparency checks also pass. Panels, gems and board frame were inspected over black, white and magenta. This is targeted verification, not the full project suite, an Android build, physical-device validation or performance profiling.

## Scope and remaining differences

9 existing Assets files modified, 35 new files, 0 deleted relative to the task-start snapshot. All395 prefab records retained. The canonical Gameplay prefab and its builder agree. No existing artwork or .meta files were changed; new sprites have independent GUIDs. Pre-Level's prefab remains unchanged but its shared gem lookup intentionally uses the new set.

This is not pixel-exact overall. Live serif typography, native score stars/progress and grid junctions differ from the painted reference. Existing Ivan and boosters intentionally differ. Repaired surfaces and hidden scenery are inferred. The existing tall-aspect environment extension repeats some architecture, visible in the safe-area captures; that background treatment remains a refinement item. Fine source detail is limited by the original image resolution. Player values, objectives and piece arrangements correctly follow the live game rather than the illustration.
