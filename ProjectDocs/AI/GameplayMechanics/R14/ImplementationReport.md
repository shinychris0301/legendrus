# Gameplay Mechanics R14 — material impacts

Implemented the next presentation pass after R13 in the PrototypeR4 development sandbox. Ordinary gems retain the approved sprite textures and now break into asymmetric facets with varied spin, travel and a brief contact glint. Surviving obstacles receive a restrained fracture response; final hits release a larger material-specific break.

- Wood: short grain splinters on damage, longer jagged planks on release.
- Ice: small chips and fracture spokes, then translucent highlighted facets.
- Chains: vibrating closed link, then tumbling open links and sparks in the approved antique-gold palette.
- Seals: small chips, then floating gold/sapphire fragments with detached rune strokes.

Obstacle fragments are procedural geometry matched to the material palette and silhouette, not extracted textured fragments. The approved intact/damaged obstacle sprites remain unchanged. R13 contact timing and special-item effects remain in place; the old generic obstacle burst is suppressed for R4 to avoid duplicate visual noise.

## Implementation and bounds

`MaterialBreakView` uses one non-interactive UI mesh clipped to the board, capped at 96 active hit records. It subscribes to existing contact and obstacle events, ignores no-damage hits and hidden ordinary colors on special cells, and clears on recovery/disable. No per-hit GameObjects or gameplay RNG are introduced. Gem shatter retains its existing six texture batches and fixed capacities. Session content is `MechanicsRoundsR4-RulesR4-PresentationR14`.

Changed existing runtime files: `GemShatterView.cs`, `Match3Board.cs`, `PrototypeImpactView.cs`, and `MechanicsPlaytestLog.cs`. Added `MaterialBreakView.cs`, `MechanicsMaterialFeedbackTests.cs`, and their metadata. Runtime-created sandbox components require no prefab rebuild. Task-start hash comparison confirms no gameplay-core, art, prefab, package or project-settings changes; no paths deleted. See [exact scope](QA/Scope.json).

## Verification

Unity 6000.5.9f1, previously authorized Vulkan for the verification process only. Final combined selected PlayMode suite: **69 passed, 0 failed, 0 skipped**. The seven new material tests are included in that total. Focused final run also passed 7/7.

Tests cover actual two-to-one and one-to-zero obstacle interactions, final planned board hashes and move cost, distinct material meshes, original gem texture identity, clipping, a 200-hit capacity stress, expiration, disabled-state handling and recovery. Existing matching, combinations, causal timing, rounds, session capture and audio regression tests also passed. No EditMode suite was rerun in R14; core and timeline source hashes are unchanged. R13's EditMode results are not counted as new verification.

Earlier attempts: Verify01 stopped at a C# multiple-`var` declaration compile error, corrected with an explicit type. Verify02 passed 6/7; its gem test swiped in the wrong grid direction, so no match was accepted. Corrected that fixture; final focused and combined runs passed. The chain fragment palette was also corrected after inspecting the actual approved chain sprite.

## Visual evidence and limits

[Animated review](Gameplay_Mechanics_R14_Review.html) provides nine actual Unity sampled sequences with a frame inspector. First-hit fixtures start with two layers; final-break fixtures are separate setups starting with one layer. They are not one continuous two-move recording. Sample intervals are retained between captured frames, with review holds at the beginning/end and a jump to the final settled state. No generated/interpolated frames, sound recording or device frame-rate claim.

Inspected final chain and gem debris as well as wood, ice and seal captures. Rendering retains the game's existing art and confines the new mesh to the board. This completes the implementation pass; subjective motion acceptance and physical Android performance/audio/haptics remain unverified. Production campaign still uses Legacy behavior.

Next: review these animations, then play the five R4 sandbox rounds and collect updated session exports plus phone performance evidence before production migration or further tuning.

Browser verification: 135 final portraits loaded at both 430px and 1440px (270 checks); nine animations loaded at both widths (18 checks), no JavaScript errors or horizontal overflow. Mobile review inspected.
