# Gameplay Mechanics R3 — Runic Seal

The next approved step after R2 is implemented in the development sandbox. A two-layer seal occupies a cell, blocks swaps and gem/item placement, and partitions gravity like a crate. Ordinary adjacent matches leave it intact. A direct line, bomb, Spark or line/area inventory-booster footprint damages it once per resolution wave. Overlapping footprints do not double-damage the same seal in that wave; a later wave can hit it again. Only final removal advances a clear-obstacle goal.

Color-only clearing cannot target the colorless seal; it can still activate a special that reaches the seal. Sea Pearl retains its reshuffle behavior. No inventory meaning was changed. The board refills the opened cell after final destruction.

## Integration

- Appended RunicSeal obstacle ID4 and PrototypeR3 rule ID4, retaining existing IDs. R3 includes R1 bombs and R2 Sparks. Earlier rule profiles reject a seal board; authored seal generation requires explicit R3. Campaign startup remains Legacy and the Chapter 1 mechanic rollout is unchanged.
- Snapshot validation, replay decoding, stationary occupancy, swaps, generation, refill and recovery understand the new blocker. Immutable plans record layer changes. R3 has its own deterministic resolution seed; earlier profile seeds are unchanged.
- Level obstacle mapping and clear-obstacle goals support `runic_seal` with a maximum of two layers. Goal progress uses the existing final-destruction contract.
- GameplayCellMechanics and the live goal HUD share RunicSealGraphic: an opaque navy octagonal plate, antique-gold bands and sapphire rune. The first hit breaks/dims the inner band and introduces a crack; the second removes the obstacle. Sapphire hit fragments use the existing impact presenter. This is a code-drawn prototype, not final painted obstacle art or an extracted Royal Kingdom asset.
- Sandbox defaults to R3 with four new fixtures: ordinary-match immunity, two line hits, two Spark hits and direct booster hits. R3 has20 fixtures, R2 has16, older profiles9. Inventory remains temporary. No production prefab, art, routing, save or level-data asset was replaced.

## Verification

**332/332 EditMode +14/14 PlayMode passed**, zero failed/skipped in accepted results. Six browser capture/viewport checks and three local links passed, with no JavaScript errors. See [accepted results](QA/Verification.json) for exact runs and counts and [scope](QA/Scope.json) for changed paths. Core checks cover layer bounds, occupied-cell exclusions, rule gating, ordinary-match immunity, Row/Column/Burst/Spark hits, overlap deduplication, line/area boosters, color immunity, snapshot replay, gravity reopening and recovery. Runtime tests exercise real board commands, move counts, final-only goal progress, intact/cracked graphic meshes, generation across eight seeds and bounded rejection of a fully blocked board. A legal starting move does not establish that a whole level is solvable or balanced.

[Portrait review](Gameplay_Mechanics_R3_Review.html) contains actual Unity captures of intact, cracked and cleared states on the approved gameplay page. Only one capture is displayed at a time. Browser checks cover all three states at430 and1440 widths. Developer controls and fixture labels are not production UI.

Earlier attempts are retained: Verify01 selected the upper line first in its runtime fixture; corrected the test to activate the intended line at(3,3). Verify02 used an unsupported GetMesh overload in a new diagnostic assertion; corrected to the local API. Neither attempt is counted as a successful seal runtime run. Verify03 passes both seal tests and produced the reviewed captures.

The NativeJourney runner wrapper exited143 while Unity continued; both tests completed with passing XML, recovered after Unity finished. The child process exit code was unavailable and is not asserted. Exact pre-run Addressables link.xml and metadata were restored. Scope:15 existing source files changed, three new C# files plus metadata, zero deletions;7,276 existing Assets/Packages/ProjectSettings files unchanged.

No physical Android acceptance, IL2CPP build, full-project suite or human difficulty playtest was performed. The live campaign is not switched to these mechanics. Goal-aware Spark targeting, richer impact choreography, final obstacle art, reduced-intensity tuning and phone profiling remain open.

## Next step

Build a small, representative round-playtest slice combining ice, crates, seals, Sparks and bombs. Review objective clarity, move budgets and effect readability before authoring the held first50 campaign rounds. Keep the slice in the sandbox until balance and rule/save migration are explicitly addressed.
