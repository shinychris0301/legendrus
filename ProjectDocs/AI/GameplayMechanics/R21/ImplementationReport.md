# R21 — Complete item creation and stable idle handoff

The user reviewed the new item animations and found the creation process incomplete and the final stage awkward. This pass addresses that specific transition in the existing R4 sandbox before extending activation VFX.

## Findings and correction

R19 crossfaded four generated formation poses into an idle sprite with a different silhouette. Its creation stage was0.22s and creations within the same wave ran sequentially. RefreshAll could reveal subsequent items before their turn. The gathered pose also held still during the clear/commit interval.

The new `SpecialCreationGraphic` continuously assembles the approved idle sprite itself. Ten textured sectors converge with tangential motion, then become one seamless full-sprite quad. The forming item continues joining while the board clears. A0.40s completion phase joins and settles it; the final Image takes over at its normal pose with the same artwork, direction and aspect ratio. No generated substitute silhouette is used for match creation.

Matched gems now follow gently curved convergence paths over0.32s; the center gem fades into the forming item. The original connected-run membership remains unchanged, including square runs. All creations in one resolution wave now start their completion phase together. A short full-sprite hold precedes handoff; no additional opaque panel or screen flash is introduced.

This applies to match-created row/column, bomb, color and Firebird items. R19 activation art and rainbow conversion presentation are retained. This is a continuous mesh/pose animation fix using approved existing art, not a newly painted8–12-frame asset pack or completion of the R20 layered activation specification.

## Integration

New `Match3BoardCreationR21` partial owns preparation, simultaneous completion and cleanup. Creation graphics live under a board-clipped, non-interactive root and are reused by cell. Allocation is bounded by visited board cells; geometry is at most30 vertices per active assembly, then4 for the completed pose. No per-frame particle GameObjects, game RNG or model mutations are added. Recovery/reset clears assemblies; disable/destruction clears or removes their views; a timeout protects orphaned visual cues.

R4 invokes the new coroutine after the existing after-clear snapshot commit. Earlier rule versions retain the original creation coroutine. Core resolution, special footprints, difficulty, move costs and attack contact times are unchanged. Session presentation version isR21. The sandbox constructs these components at runtime, so no serialized production prefab or separate builder is needed for this correction.

Changed runtime sources: Match3Board.cs, Match3BoardPresentationR13.cs, SpecialSequenceView.cs and MechanicsPlaytestLog.cs; added Match3BoardCreationR21.cs and SpecialCreationGraphic.cs with metadata. Updated MechanicsSequenceArtTests to inspect the new creation renderer as well as R19 attack layers. Added MechanicsCreationFinishTests for simultaneous creation/full-sprite hold, color/Firebird identity and cleanup.

## Verification and review

Final test counts and scope are in [Verification.json](QA/Verification.json) and [Scope.json](QA/Scope.json). [Open the animated review](Gameplay_CreationFinish_R21_Review.html).

Early Verify01 compiled, with2/4 failing because the old tests only counted the R19 atlas renderer's vertices. Updated the assertions to include the new creation graphics; Verify02 passed6/6 focused tests. Visual inspection then prompted a continuously joining middle stage and center-gem fade. Verify03 passed5/7; the two new capture fixtures wrote timestamps before creating their output directory. Corrected the fixture directory setup. Verify04 passed82/82 selected PlayMode tests, zero failures/skips. A final defensive change then hides restored idle sprites before yielding into the completion coroutine; Verify05 passed the focused7/7 creation/art checks on that final source, zero failures/skips. Intermediate runs are not added to the distinct test count.

Verification uses Unity6000.5.9f1 and the previously authorized process-only Vulkan option with descriptor limit4096. No project backend/package changes. The review uses actual576×1280 Unity captures with recorded sample intervals, plus normal and half-speed playback and final-state images. Historical R19 comparisons are separately recorded, not synchronized footage. No frame interpolation, generated gameplay frames, audio recording, physical-phone frame-time measurement or user visual acceptance is claimed.

Synthetic test snapshots may show49/4 in the underlying playtest HUD. This existing fixture-caption mismatch is not a changed move limit. New visual timing can change perceived pace; review/player feedback and Android profiling remain necessary. The next broader task is the separately specified layered directional activation pass, once the creation transition is visually satisfactory.

Review checks:5 sequences,75 actual Unity portraits and10 normal/half-speed WebP animations. Browser430/1440 passed150 frame loads,20 animation loads,4 historical comparisons, full-size links and still controls, with no JS errors/horizontal overflow. Mobile page and final formation frames visually inspected.

Task-start SHA scope:5 existing source/test files modified,6 new helper/test/meta files,0deleted,7427unchanged. Art, GameplayCore, prefabs, packages and project settings unchanged. Original linker state restored; new temporary test scene artifacts removed.184 superseded task-owned PNGs pruned with SHA manifest; final combined/focused captures and all test logs/results retained.
