# Gameplay Mechanics R15 — gem shattering and booster spectacle

The user found R14's booster, line-clear and gem-breaking effects too simple. This pass strengthens their visual presentation in the PrototypeR4 sandbox while retaining the approved artwork and existing game rules.

Gem breaks combine larger texture-preserving facets with 16 small colored chips/sparkles, short motion trails and localized contact light. Major fragments travel farther and shrink less while fading. Line clears use white-gold travelling heads, tapered luminous trails, curved energy strands and sparks. Bombs use a layered shock front, radial flares and drifting embers.

Inventory boosters have distinct releases: the mace swings into a strike and recoils; the Firebird grows into a wing-shaped flare; Living Light emits sapphire radiance; Sea Pearl releases an ascending spiral. Booster illustrations reuse the approved sprites. R14's material-specific obstacle damage and R13's causal special timing remain active.

## Scope

The existing impact mesh remains bounded at 192 effect records, with a vertex guard below the Unity UI limit. New lights and trails are procedural UI geometry, not new raster art or post-processing bloom. Effects are limited to the board. Gem texture batches retain their prior capacity. All motion is presentation-only and uses unscaled time; no game-state RNG or per-particle GameObjects are introduced.

Modified `PrototypeImpactView.cs`, `GemShatterView.cs`, `PrototypeBoosterView.cs` and `MechanicsPlaytestLog.cs`. Added partial renderer `PrototypeImpactViewR15.cs` and `MechanicsSpectacleTests.cs` with metadata. Runtime-generated components require no prefab rebuild. Session content is `MechanicsRoundsR4-RulesR4-PresentationR15`. Production campaign remains Legacy.

## Evidence

See [verification results](QA/Verification.json), [scope comparison](QA/Scope.json) and the [animated Unity review](Gameplay_Mechanics_R15_Review.html). Eight sequences cover a three-match, horizontal/vertical line items, bomb and each of the four inventory boosters. The tests issue real commands and check input release, board outcomes, move costs, rendering, effect expiration and recovery. Stress testing fills the effect pool and checks finite board-bounded geometry under the UI vertex limit.

The first focused run passed 3/4 tests. Its ordinary-match fixture accidentally formed a four-match, which correctly gathered gems into a newly created item instead of shattering them. Corrected the fixture to isolate a three-match; no gameplay rule was changed.

These are actual Unity captures sampled during automated commands, with an opening hold and a jump to the final settled state. The viewer preserves intervals between sampled frames. It is silent playback, not a frame-rate recording. Physical Android performance, sound/haptics on device, and subjective player acceptance remain unverified. No EditMode rerun is claimed for this presentation-only pass.

Final Unity verification: **73 selected PlayMode tests passed, zero failures/skips** in Verify03, after soft-edge light polish. Verify02 also passed 73/73 before that visual adjustment. Verification used the previously authorized process-only Vulkan override; project graphics settings were unchanged.

A final review-only fixture reset removes misleading round instructions from the gem capture. Verify04 reran all four new tests: 4/4 passed with the same runtime sources as the 73/73 final broad run. Published sequences use Verify04.

Published 123 final portraits across eight sequences. Browser verification passed 246 frame loads and 16 animation loads at 430px/1440px, including full-size link checks; no JavaScript errors or horizontal overflow. Final line, gem and Firebird frames and the mobile review were visually inspected. Scope: four existing runtime files changed, four source/meta paths added, no deletions, 7408 unchanged. Source diff whitespace check passed.
