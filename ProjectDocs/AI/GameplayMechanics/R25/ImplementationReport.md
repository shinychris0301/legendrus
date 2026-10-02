# R25 — Tiny gem scatter, large shards and sparkling contact

The requested effect is implemented in the R4 sandbox: each ordinary gem shatters into eight irregular source-textured shards and sixteen tiny chips. This replaces the six coarse rectangular chunks. Row/column items use the existing cell-contact schedule, making the scatter advance along the clearing line. Ordinary matches and other ordinary-gem clears receive the same material treatment.

The chips have varied angles, reach, rotation, size and lifetime. Eight additive light accents follow those same chip paths, with short tapered trails. A compact colored impact flare lasts 0.13 seconds. Tiny fragments fade between 0.38 and 0.545 seconds; larger shards tumble and fall for up to 0.62 seconds. There is no expanding ring or circular board overlay. [Timing/design](EffectDesign.md).

## Integration and bounds

`GemShatterView` owns material fragments in the six existing reusable texture batches. Each color retains at most sixteen active gem breaks, with at most 88 vertices per break (1,408 per color batch). `PrototypeImpactView` and `PrototypeImpactViewR15` reuse the existing masked additive mesh for contact light and chip glints. The renderer cap remains 192 events; the new gem-light branch uses at most 184 vertices per event. These are code budgets, not measured device performance.

Trajectories are deterministic functions of cell coordinates and particle index; they do not consume gameplay random values. Original approved gem textures supply shard and chip facets. No generated bitmap assets, extra particle GameObjects, physics bodies, shaders, material assets or dependencies. This Canvas-based particle implementation fits the existing UI board. Recovery/disable cleanup and masking remain in place. Item-creation choreography and other activation silhouettes are unchanged; this is a focused shatter improvement.

The owning effects are created by the existing runtime sandbox composition. No prefab/builder regeneration is required. `MechanicsPlaytestLog` identifies PresentationR25. Campaign remains Legacy; score, matching, damage, moves, gravity and contact timing are unchanged.

## Evidence

[Actual Unity review](Gameplay_GemScatter_R25_Review.html) includes row/column launch, bomb, combined clear and creation regression clips with R24 comparison, normal/half speed and frame inspection. Captures are timestamped samples without interpolation or audio, not an Android frame-rate measurement. Synthetic HUD captions can still show 49/4 moves, an existing fixture mismatch.

See [verification](QA/Verification.json), [browser checks](QA/BrowserVerification.json), and [task-start file scope](QA/Scope.json). Final run results are recorded after verification completes. Physical Android performance and user visual acceptance remain outstanding.

Final Unity PlayMode verification: **33/33 passed**, zero failures/skips (Verify03/Scatter), including row/column/bomb/combination outcomes, all inventory boosters, ordinary matches, chip direction and lifetime, material texture identity, bounded geometry, masking, recovery and input release. An initial compiler name collision was fixed. The next run passed 27/33 and exposed an empty-active-light interval: sparkle selection omitted the longest-lived chips while the event remained active. The final change selects all lifetime groups and expires the light at the longest chip lifetime; all six failures pass on the final run. Tests were not weakened to hide the interval.

Verification used Unity 6000.5.9f1 with the previously approved process-only Vulkan and descriptor limit 4096, isolated test configuration. No EditMode run or physical Android test. Source diff whitespace checks passed. Scope: six modified runtime/test files, no added/deleted files under Assets/Packages/ProjectSettings, 7,439 unchanged. No project settings, packages, textures or prefabs changed. No Git commit.

Browser verification at430/1440 passed594still loads,36animations and18R24comparisons with zero JavaScript errors/overflow. Mobile HTML and final row/column Unity captures inspected.
