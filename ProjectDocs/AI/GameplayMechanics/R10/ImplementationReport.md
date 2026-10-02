# Gameplay Mechanics R10 — Board art and damage readability

The R9 review found that dense ornament and small textured highlights separated the new pieces from the broad facets of the approved gems. The user approved a cohesive board-art pass and replacement of obstacle numerals with visible damage stages.

## Implemented

Twelve original image-tool sprites in `Assets/LegendsOfRus/Resources/ProductionUI/MechanicsBoardArtR10/`:

- Amber bomb, compact directional sapphire, board-scale Firebird and rainbow prism.
- Crate, ice, chain and runic seal, each with a separate intact and damaged image.

Generated using the actual approved sapphire/gold gems as style inputs and prior sprites as edit targets. Damage variants use the new intact sprite as their input to retain placement and palette. Final prompts and original tool-output paths are in [FinalGeneration.json](Sources/FinalGeneration.json); the directional piece received a further [proportion correction](Sources/CompactLineRevision.json) after viewing the first Unity capture. Production PNG pixels/alpha are copied unchanged. No procedural repaint, source-gem replacement or new shared inventory icon was introduced.

Crate loses its reinforced bands and gains split planks. Ice opens a large hole in its shell. Chain has a snapped top link and cracked padlock. Seal has a missing corner and broken rune. The remaining-hit counter still lives in the puzzle model; the prototype no longer creates an `ObstacleLayers` text object. Header objective counts stay live. A one-layer obstacle starts with the final-removable artwork, matching the single-/two-hit mechanics.

`PrototypeObstacleArt` selects by actual layer count. The previous generic fissure overlay is retired for these sprites. Intact ice uses UI alpha0.52 to reveal its underlying gem; damaged ice uses0.8 with a genuinely open center. Goal icons use the intact art at full opacity. Both chain states have real transparent centers.

`PrototypeBoardArt` centralizes the new resource root. Match3Board uses the new four on-board special sprites; SparkFlightView uses the same board Firebird for flight. Specials receive a modest 8% larger drawing rectangle to compensate for transparent margins. Ordinary gem sprites and shared inventory-page icons remain unchanged. No animation timing, deterministic rule, layout, move budget, booster cost or campaign progression change was made. Future session exports identify the presentation as `MechanicsRoundsR4-PresentationR10`.

This remains the existing non-Legacy development sandbox. Legacy campaign art/routes are preserved. Runtime-created components require no production prefab rebuild; owning runtime code and resource mappings are updated together.

## Inspection and verification

All twelve sprites inspected over black, white and magenta backgrounds: [alpha measurements](QA/AlphaAudit.json), diagnostic composites under Art/. No clipping or opaque rectangle accepted around the silhouettes. Source alpha is retained; the frost panel itself is intentionally opaque and receives transparency in Unity. Approved gem import settings were reused, with512 texture limits and unique metadata/GUIDs.

Verification uses Unity6000.5.9f1, prior user-authorized command-line Vulkan for this process only, and the previously established child-only file descriptor limit4096. No project graphics settings, host limits or packages are changed.

- Verify01 and Verify02:23/24 passed. New damage-state test incorrectly assumed a complete activation would always stop after one layer. The fixture's later waves can remove the second layer in the same command. Existing gameplay tests passed.
- The new test now observes the resolved2→1 event and the actual damaged sprite during resolution, then verifies final removal and plan-state agreement. It follows real cascades rather than changing game rules to fit the assertion.
- Verify03 focused run:5/6 passed. The crate cleanup step aimed a mace at a crate-occupied cell; the existing booster validator requires a filled target. Changed the test to target a filled cell in the same row, and assert the command is accepted.
- Verify04 focused rerun:6/6 passed. Combined with the other23 successful cases in Verify02, the latest result for all24 unique selected tests is Passed. Runtime and artwork were unchanged between those runs; only the new test sequence was corrected.
- Final results are recorded in [Verification.json](QA/Verification.json). Existing tests cover commands, five-round outcomes, inventory semantics, replay consistency, animation cleanup, Spark/seal behavior, feedback and local session storage.

The review presents actual Unity captures one portrait at a time, including intact/damaged/cleared sequences, the mixed board and special pieces. These are automated fixture captures, not human acceptance, phone performance measurements or a production build.

## Next acceptance

Replay the five sandbox rounds and inspect the material stages at normal phone-scale display size. Confirm whether the new family feels integrated and the no-number stages remain readable. Difficulty is intentionally preserved. Android performance, audio/haptics and campaign rollout remain separate outstanding work.

## Final evidence and scope

The review presents27 selected portrait states from Verify02 and Verify04, with capture provenance recorded in QA/CaptureProvenance.json. Mixed board, rainbow piece, intact/damaged obstacles and revised directional artwork were visually inspected. Browser checks passed54 capture/viewport combinations (430/1440),72 asset/state/backdrop combinations, state navigation, native-pixel containment and local links; zero JavaScript errors.

Task-start comparison:7 existing source/test files changed,27 new paths (12 sprites plus metadata, folder metadata and PrototypeBoardArt source/metadata),0 deletions;7,325 pre-existing files unchanged. No existing art file, prefab, GameplayCore rule, round content, package or ProjectSettings file changed. Exact scope in QA/Scope.json. Own generated test scenes/linker artifacts restored to task-start state. Runtime visual improvements are not a claim of player acceptance or phone performance.
