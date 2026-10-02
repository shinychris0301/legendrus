# R17 revision — Design the complete special-item experience

Status: revised proposal, awaiting design review. No Unity implementation or new art assets produced in this revision.

## What the feedback changes

The user finds the prior physical-motion proposal too simple, including its debris, and wants the visual payoff of creating and activating powerful items. R17's Canvas studies are insufficient as a production quality target. They remain historical timing sketches; they are not approved effects. The prior recommendation to implement those three isolated studies first is superseded by a complete special-item sequence prototype.

The central design unit should be **match → formation → ready item → activation → contact → chain reaction → recovery**. Each stage should make the power feel valuable and establish what happens next. More fragments, brighter rings, or faster sprite movement alone do not define this experience.

## Verified local findings

- `Match3BoardPresentationR13.GatherForCreations` already gathers connected matched gems toward a creation position, shrinking/fading them over 0.24 seconds. Creation is not absent from the sandbox; its visual treatment is limited.
- `PrototypeImpactViewR15.RichEffect` builds much of the presentation from glows, rings, stars, diamonds, strokes and trails. These primitives can be useful supporting layers, but the current repetition gives several powers a similar visual identity.
- `GemShatterView` divides a sprite into eight textured triangle pieces with movement, rotation and tint. This is not authored thick fracture artwork. Simply enlarging those pieces does not supply convincing fracture surfaces or material detail.
- `PrototypeBoosterView` animates one Image using position, scale, rotation and alpha. This limits visible changes in shape, pose and volume.
- `Match3Board.Initialize` and `InitializeCampaign` explicitly select Legacy. `InitializeMechanicsSandbox` attaches the prototype effect presenters. The newer presentation remains sandbox-only; it has not been rolled out through normal campaign gameplay.

These are source-review findings, not new runtime verification. They explain both an art/presentation gap and an integration gap. Migrating unfinished effects into campaign would address only the latter.

## Revised art and animation approach

### Formation is a reward

For a line item, visually connect the matched row/column, gather its energy into the chosen cell, reveal the directional body in a short authored transformation, then settle it with a restrained ready-state animation. For a bomb, build pressure as the contributing gems collapse into its core, resolve a recognizable shell and reveal its charged state. Five-match and other supported formations need their own recognizable transformation.

The resulting item must remain legible at cell size. Ready-state motion should be selective and restrained when many specials are present. Preserve the approved item identities and the actual placement/rules; animation does not choose a different destination or introduce a new power.

### Activation needs authored shape changes

Create original animation frames and/or articulated parts for the main action, rather than relying on scaling the idle icon. Examples: a line item opens/releases its directional heads; a bomb shell compresses, ruptures and gives way to an irregular luminous blast; Firebird wings change pose through takeoff, bank and strike.

Use a combination of authored sprite sequences, reusable fragments, trails, masks and synchronized motion. This does not require a new rendering backend, a generic replacement VFX package or a change in game style. Royal Kingdom is a reference for clarity and payoff, not an asset source or evidence that a particular rendering technique is required.

### Design debris as artwork

Create a small family of actual jewel fracture profiles, including shaded side faces and varied thickness, matched to the approved gems. Distinguish the large readable pieces, small fast chips and short residual dust. Wood, ice, chains and seals each require different fracture structure. First-stage damage leaves the damaged object present; final-stage destruction releases the remaining structure.

A line strike drives debris along its contact path; a bomb pushes it away from the source; a mace produces a concentrated impact. Avoid spraying the same fragments in the same radial pattern for every event. Select density and lifetime in the context of a whole cascade so the next move remains readable.

### Treat combinations as distinct sequences

A combination should visibly form or communicate its combined identity before release. A line/bomb combination can stage a forceful multi-directional launch; a carried payload must remain visible until delivery; rainbow conversion must show the new objects before their activations. All resulting footprints, contacts and costs continue to come from the existing rules.

Synchronize sound and optional haptics with the key formation, launch and impact beats. Use an escalating hierarchy from ordinary match through a special to a combination. Visual acceptance should also be tested muted; audio cannot conceal weak animation.

## The next deliverable to review

Prepare **one cohesive board sequence** at the intended final art quality:

1. A supported four-match visibly creates a directional item, which settles and remains recognizable.
2. A supported L/T match creates a bomb with a different formation and ready state.
3. The two powers combine using the existing supported rule, showing a deliberate combined launch.
4. The attack breaks gems and hits a staged obstacle at the correct contacts; the board recovers into a readable state.

If these actions require separate seeded fixtures, label cuts explicitly rather than implying one recorded playable session. Before coding the Unity integration, review final-art key poses and an animated composition using the proposed effect artwork, actual board scale and representative density. An offline composition must be labelled as such. A static mockup, primitive-only Canvas sketch, or more particle count is not sufficient evidence of the intended quality.

Asset deliverables for that sequence: directional formation/launch poses and exhaust, bomb formation/rupture/blast frames, material-specific fracture pieces, and supporting contact/dust layers. Inspect transparent edges over black, white and saturated colors. These assets are proposed; none is claimed to exist yet.

After design acceptance, integrate the approved sequence into the existing sandbox presenters and contact timeline. Verify creation visibility, input recovery, deterministic outcomes, obstacle layers, effect pooling and phone performance. Use continuous normal-speed capture with sound plus a muted comparison. Correct timestamp namespacing before further sampled reviews.

Only then extend the same quality standard to Firebird, rainbow, other boosters and all supported combinations. Schedule a deliberate campaign integration step after gameplay, persistence and performance checks; do not leave the final effects accessible only through the sandbox.

## Reference and review boundaries

Royal Kingdom's official documentation describes creating distinct power-ups from match shapes, then combining them into stronger outcomes. This supports reviewing formation and activation as a connected player experience. It does not specify the internal art-production technique or establish exact animation timings for our game.

- [Official power-ups](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/177-power-ups/?p=all)
- [Official combinations](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/178-power-up-combinations-1777447709/)

This revision is design/source review only. No Unity tests, device tests, asset generation, campaign migration or matching-rule changes were performed. Existing R17 studies are retained as historical sketches, not approved implementation targets.
