# Gameplay Mechanics R16 — energy and impact redesign

The user accepted that R15 improved the effects but requested a substantially stronger redesign across the gameplay animation set. R16 changes the lighting/compositing method as well as motion, scale and effect shapes in the PrototypeR4 sandbox.

## Implemented

A dedicated additive UI energy shader supplies the light pass. It brightens the scene through the effect's alpha, preserves destination alpha and supports UI stencil/rect clipping. It is a local material on the existing impact renderer, not a project graphics-backend or post-processing change. The owned material is destroyed with its board.

- Gems: contact squash, rapid release into larger spinning approved-texture facets, colored light cores and longer sparkling wakes.
- Horizontal/vertical items: broad tapering wakes, sustained white-gold cores, curling strands and bright moving heads.
- Bombs: larger hot core, expanding shock front, radial flare tongues and glowing debris.
- Creation: converging sparks and a contracting gold charge.
- Rainbow/conversion: bright jagged links and a multicolor radial release.
- Firebird items: larger banking flight silhouette, continuous energy ribbon replacing square trail dots, and retained visible row/column/bomb payloads.
- Inventory boosters: stronger mace swing/strike/recoil, larger Firebird wing release, sapphire light surge and ascending pearl spiral.
- Obstacles: stronger light at actual contact, larger wood/ice/link/rune fragments, more wood and ice debris, with first damage distinguished from the final break. No numeric obstacle overlays.

The approved gem/booster/item artwork is reused. The light pass and ribbons are code-native shader/mesh work; no new raster illustrations were generated. Existing pool and vertex bounds are retained. Effect changes are driven by the established events and immutable plan; puzzle outcomes, difficulty, move budgets and contact timing constants are unchanged. Production campaign remains Legacy.

## Scope and evidence

Modified runtime sources: `PrototypeImpactView.cs`, `PrototypeImpactViewR15.cs` (the existing partial renderer), `GemShatterView.cs`, `MaterialBreakView.cs`, `SparkFlightView.cs`, `PrototypeBoosterView.cs`, `Match3BoardPresentationR13.cs` (clear pose only), and `MechanicsPlaytestLog.cs`. Added the resource shader and two shader/lifecycle PlayMode checks with metadata. Runtime-created sandbox components require no prefab rebuild. Session content: `MechanicsRoundsR4-RulesR4-PresentationR16`.

See [verification](QA/Verification.json), [task-start scope](QA/Scope.json), and the [animated review](Gameplay_Mechanics_R16_Review.html). All graphics verification uses the previously authorized process-only Vulkan override. Android graphics compilation/performance is not inferred from desktop success.

The first focused run passed 4/4 behavioral tests, but visual inspection exposed an incorrect shader color-write mask that omitted red. Corrected it to RGB and added an actual pixel test for warm color channels, destination alpha and RectMask2D clipping. The second added test checks R4 material ownership/disposal and that R3 keeps its previous material.

The review uses actual Unity sampled captures from real commands. It covers the eight primary effects, six causal/combination sequences, and eight separately initialized obstacle damage/final-break cases. A single portrait can switch to R15 playback for the eight main effects; these are independent recordings, not synchronized footage. The viewer preserves captured sample intervals, adds beginning/end review holds and jumps to the final settled state. Playback is silent and is not a frame-rate recording.

Remaining validation: the user's assessment during play, and physical Android performance/audio/haptics. This pass does not claim production campaign migration or device readiness.

Final Unity run: **75 selected PlayMode tests passed, zero failures/skips**. This includes the two new rendered-pixel/material checks, all previous effect/causal/combination tests and round/session/audio regressions. No EditMode rerun is claimed; gameplay-core source hashes are unchanged.

Browser verification: 374 portraits across 22 sequences; 748 still-image checks, 44 animation checks and 16 R15 comparisons at 430/1440px passed. No JavaScript errors or horizontal overflow; full-size links checked. Final line, bomb, conversion frames and mobile review visually inspected. Task-start scope: 8 existing sources changed, 5 source/shader/metadata files added, no deletions, 7408 unchanged. Source diff whitespace check passed.
