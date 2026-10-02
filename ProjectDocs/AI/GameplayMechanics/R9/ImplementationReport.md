# Gameplay Mechanics R9 — Impact, power-ups and obstacle art

The supplied manual export has been reviewed attempt by attempt: 20 records, including 12 restarts before a move was spent, eight completed attempts, three wins and five losses. Goal confusion appears across rounds 2–5; Spark was explicitly rated busy. The user's separate written feedback identifies weak impact and poor booster/obstacle designs. See [the detailed session review](ManualSessionReview.md) and [structured analysis](QA/ManualSessionAnalysis.json).

## Implemented presentation

Changes run in the existing development mechanics sandbox. Core puzzle rules, board layouts, move budgets, inventory costs and campaign progression remain unchanged.

- New generated navy/gold bomb and sapphire directional power-up sprites replace the old prototype Burst and elongated gem imagery. Ordinary gems remain the approved gameplay gems.
- Ordinary clears now split the actual approved gem texture into eight rotating triangular fragments, with outward motion, gravity and a short fade. Six persistent texture batches cap active pieces and are masked to the board.
- Special activation now precedes clear fragments. A 0.12-second windup compresses the special, followed by a brief overshoot, directional beam with moving impact heads or local blast core, expanding wave and faceted debris. Ordinary clears stay smaller.
- Spark uses an eased 0.52-second flight, a restrained trail and a visible destination ring. Arrival triggers impact before clear feedback.
- Color specials and Living Light link the source to cells actually cleared by the resolution plan. Sea Pearl uses rising blue renewal motes for its +5 moves; it does not show a destructive blast or imply a shuffle.
- Inventory booster use animates the approved tool artwork: mace swing, Firebird dive or magical-tool reveal. Consumed inventory, move accounting and plan targets stay owned by Match3Board.
- Zero-stock sandbox booster illustrations retain 0.8 brightness at full opacity instead of 0.4 brightness/0.8 opacity. Counts and disabled interaction remain authoritative. This is a presentation correction, not free booster inventory.
- Four new obstacles: carved reinforced crate, crystalline ice, chain lock and sapphire runic seal. Board and goal icons use the same new artwork. Final-layer fissures are drawn as live geometry; ice opacity distinguishes its two layers while revealing the gem beneath it. Cleared obstacles hide both image and fissures.
- Russian round instructions name the actual objectives, layer rules and activation gesture more explicitly. They do not reveal solution coordinates.

There is no camera shake or full-screen white flash. Effects use one capped impact mesh (192 retained effect records, under the 16-bit UI vertex budget), six persistent gem-fragment batches (at most 128 pieces per color), and one reused inventory-illustration object; they do not consume gameplay RNG. Recovery/disable clears presentation. This is a bounded presentation design, not proof of phone performance.

## Royal Kingdom reference

Reviewed Dream Games' official [power-up explanations and animated demonstrations](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/177-power-ups/?p=all), [combination guide](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/178-power-up-combinations-1777447709/), and [game page](https://www.dreamgames.com/games/royal-kingdom). The official Rocket/Dynamite GIFs were downloaded to a temporary study folder and inspected as frames. They show distinct directional and area power-ups; the guide documents greater combination reach. No reference art was imported into the project.

The inference for this game is to separate charge, travel, impact and aftermath, and use recognizable silhouettes. R9's exact timings and Slavic art are our implementation choices; they are not measured Royal Kingdom timings or copied assets. Audio/haptics have not been added in this pass.

## Generated assets and provenance

Built-in `image_gen.imagegen` generated six isolated PNGs, now installed under `Assets/LegendsOfRus/Resources/ProductionUI/MechanicsPowerupsR9/`: bomb, line, crate, ice, chain and seal. [Power-up prompts](Sources/ArtPrompts.json) and [obstacle prompts](Sources/ObstaclePrompts.json) record the requests. Source pixels and alpha are preserved; importers cap textures at 512, disable mipmaps and preserve transparency. Existing art, GUIDs and shared inventory icons remain intact.

All six sprites were inspected over black, white and saturated magenta. See [alpha evidence](QA/AlphaAudit.json) and the diagnostic composites in Art/. Opaque crate and seal interiors are intentional. The final chain is an open ring and padlock with a transparent center, preserving the playable gem underneath. The first solid chain plate and first thin directional design were rejected and replaced before acceptance; final revision prompts are also saved in Sources/. Ice transparency on the board is applied in the UI; the generated frost itself is mostly opaque. Damage fissures are code-native overlays, not separate painted damage variants.

## Verification history

- Verify01: blocked by another Unity Editor holding the project; no test result.
- User confirmed Unity closed.
- Verify02: native Mono startup assertion about file descriptors before test execution. This differs from R7's OpenGL rendering failure.
- Verify03: 21/22 passed; the new fissure graphic was missing its required CanvasRenderer. Added the required component and reran.
- Verify04 and Verify05: 22/22 passed, including revised directional artwork and the open-chain sprite.
- Verify06: compilation caught an incorrect BoardCellData constructor in the newly added fragment test. Corrected the test; no test result was claimed for this run.
- Verify07: all 23 PlayMode tests passed together, zero failures/skips. This is the final run with actual-texture fragments and differentiated effects. Results: [QA/Verification.json](QA/Verification.json).

Verification uses the already approved command-line Vulkan override and a child-process-only soft open-file limit of 4096. No host/system limit or project graphics setting is changed.

Existing gameplay/replay, five-round, storage, feedback, Spark and seal tests remain in the selected suite. Five new tests cover event order and settled input, inventory presentation, goal/board artwork consistency and damage-state cleanup, bounded impact geometry/disable cleanup, and actual approved gem textures with fragment expiry/recovery cleanup. The existing seal visual assertion now checks the new fissure graphic rather than the replaced flat glyph.

No production prefab rebuild is needed: these components are created by the existing sandbox/board path and retain all serialized production UI references. The goal-view change is conditional on a non-Legacy board. Default campaign rules/art behavior remains Legacy.

## Remaining acceptance

This pass does not establish that the experience is now satisfying to the player. Review the actual Unity captures, then replay the five rounds to compare impact and clarity. No human session with the new visuals, physical Android run, audio/haptic acceptance or frame-time benchmark has occurred. Default OpenGL capture reliability remains unresolved. Broader campaign migration remains pending review.

## Final review and scope

The review contains 103 actual 576×1280 Unity captures; 53 selected states are presented one at a time across obstacle, bomb, directional, Spark, inventory and round groups. Selected final bomb, directional and Sea Pearl frames were inspected, together with intact/cracked seal and mixed-board states. Sampled playback is an inspection aid, not a real-time video. Browser checks passed all 106 selected capture/viewport combinations at 430/1440, navigation, native-pixel mode, playback start/stop, local links and 18 sprite/background combinations; zero JavaScript errors.

Task-start hash comparison: nine existing source/test files changed; 23 paths added (six sprites and metadata, four presentation classes and metadata, one test class and metadata); zero deletions; 7,300 pre-existing files unchanged. No prefab, puzzle-core, package or ProjectSettings file changed. Test-created scenes and Addressables linker artifacts were removed/restored to the task-start state. See [exact scope](QA/Scope.json).

Next action: replay these visuals in the existing Home → Development → Gameplay Mechanics Sandbox, first the five rounds and then stocked Fixtures for each inventory booster. Judge impact and damaged-obstacle readability before campaign rollout. The user's acceptance of the current difficulty takes precedence over earlier mixed difficulty ratings; no balance change was made.
