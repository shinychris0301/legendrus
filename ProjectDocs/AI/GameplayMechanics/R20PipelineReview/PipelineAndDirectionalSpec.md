# R20 — Layered VFX production specification

Status: **design/specification, not a new Unity implementation**. Prepared from the user's AI-to-Unity pipeline and inspection of the installed R19 sources. R19's 79 passing tests are historical evidence, not tests run for this specification.

The next vertical slice is the directional item (the guide's “Rocket”), using our approved sapphire-and-antique-gold design. Complete its formation, launch, travel, cell contact, chain reaction and recovery before expanding the same system. This takes priority over broad Firebird/rainbow expansion suggested after R19.

## What the project already does, and what must change

| Area | Installed R19 | Next work |
|---|---|---|
| Gameplay authority | BoardResolutionPlan and BoardPresentationTimeline determine results and contact times | Preserve them; VFX must not select targets or charge moves |
| Formation | Connected gems gather; atlas poses crossfade into existing idle art | Author denser assembly poses with an exact final idle silhouette |
| Directional activation | Four artwork poses combine projectile and flame/trail in each image | Separate body, launch plume, trail, sparks and contact accents |
| Bomb activation | Four fire/smoke artwork poses plus separate shell fragments | Separate flash, flame, smoke and debris lifetimes; preserve actual blast footprint |
| Debris | Six gem-color regions and wood/ice/chain artwork | Add several distinct large fragments per material, not repeated clusters only |
| Authoring | Timings and mesh code in SpecialSequenceView | Reusable effect definition, editable curves and a versioned prefab with matching builder |
| Rendering | MaskableGraphic meshes under RectMask2D in an overlay Canvas | Keep Canvas ordering/clipping; prototype a UI-compatible particle bridge before adopting ParticleSystem |
| Performance | Bounded cues and no per-fragment objects | Profile overlap, fill rate, allocations and sustained mobile frame times |

Reviewed sources: `SpecialSequenceView.cs`, `MechanicsSequenceArt.cs`, `GemShatterView.cs`, `Match3Board.cs`, `BoardPresentationTimeline.cs`, `MechanicsSandbox.cs`, and `ScreenRouter.cs`. The router creates a ScreenSpaceOverlay Canvas. Sandbox initialization attaches the R4 presenters at runtime. There is no need for a second BoosterManager or a new startup path.

## Art direction and asset contract

Use faceted sapphire, antique-gold fittings, warm amber sparks and restrained blue-white energy, matching the approved gems and idle line item. Strong shape and contrast at phone scale; preserve empty space between cells. The guide's generic rounded/cartoon style is not the approved project style. No lettering, scenery, opaque rectangles or baked HUD elements in effect sprites.

Generate separate components, keeping the approved idle item as the reference. Proposed assets below **do not yet exist as separate new R20 resources**:

| Component | Art requirement | Rendering role |
|---|---|---|
| Item/body | Reuse exact approved sprite; no embedded exhaust | Animated sprite/Image |
| Formation | 8–12 authored poses, consistent pivot/bounds; last pose aligned to idle sprite | Sprite sequence + pose curves |
| Launch plume | 6–8 short asymmetrical energy poses, no projectile body | Local flipbook |
| Trail ribbon | Isolated tapering blue-white texture, no body or debris | Variable-length UI mesh |
| Spark set | 3–4 distinct sapphire/amber streaks | Bounded particle emission |
| Contact accent | 6–8 compact fracture/impact poses, no complete gem | Per-contact flipbook |
| Fragments | Distinct large facets, separate from spark/glow | Existing fragment batches |
| Smoke/wisp | Sparse pale-blue wisps, no black backing | Low-opacity particles; first quality layer to reduce |

Prompt template for the asset stage:

> Isolated 2D front-facing [COMPONENT] for the existing Legends of Rus sapphire directional match-3 item. Match the supplied approved item/gems: faceted jewel surfaces, antique-gold fittings where appropriate, painterly crisp highlights, navy-compatible shadows. [POSE/LAYER REQUIREMENT]. Transparent RGBA, no text, no background, no border, no complete projectile in plume/trail assets. Consistent pivot, scale and palette across frames, generous transparent gutters, no frame overlap. Include bright readable focal shapes without filling the whole board with glow.

Produce contact sheets for continuity inspection, then a documented atlas manifest; do not assume AI output uses a perfectly even grid. Check black/white/saturated backings, alpha edges, all frame bounds, downscaled mobile appearance and the last formation pose against the idle item. Existing R18 assets are reference/reusable material, not proof that these separate layers have been generated.

Scenario is an intended asset-stage provider in the supplied workflow. No callable Scenario tool was found in this session; no Scenario generation was run or claimed. A future asset pass can use the available image-generation tool with the same contract. Tool choice does not change the art or acceptance criteria.

## Directional-item timeline

All times are seconds relative to **this item's activation**, including chained activations. Preserve current R4 contact scheduling for the first prototype. Creation has its own clock: existing0.24s gather, hold during wave commit, then0.22s creation. Do not add an arbitrary1s pause to every cascade.

| Time / trigger | Visible action | Owner |
|---|---|---|
| 0–0.07 | Compress body to0.94 length/1.06 width; concentrate light at its core | Pose curve |
| 0.07–0.12 | Snap to1.12 length/0.94 width; brief tip accents | Pose curve + local sprites |
| 0.12 | Release two directional bodies; launch plume lasts about0.10s | Presenter |
| 0.12–0.40 maximum | Heads advance from actual origin; trail length and sparks follow travel | Presentation timeline |
| Contact at cell d | Compact impact and colored fragments; obstacle changes state; chained item begins its own anticipation | Existing contact/obstacle events |
| Each contact +0–0.18 | Existing cell disappearance response; no second damage event | Existing board presenter |
| Up to0.18 after a head exits | Trail fades from its tail; residual sparks fade | VFX only |
| Existing wave commit/refill | Gravity/cascade continues; lingering wisps do not block input or consume moves | Existing command lifecycle |

The directional contact formula in current code is `0.12 + 0.28 * distanceInCells / max(columns, rows)`. On an8×7 board, distances0,1,2,3,4 contact at0.120,0.155,0.190,0.225,0.260s.0.40s is a maximum span window, not a fixed delay before every cell clears. Heads must arrive at cell centers on these times; decorative tails must not determine collision. Any future pacing adjustment must update the shared presentation schedule and its tests, not only the animation curve.

Optional board-only impulse: at most0.03cell for0.10s; reduced-motion setting disables it. Leave header, text and controls stationary. This is a proposed option, not a camera-setting change or a substitute for layered art.

## Unity composition and integration

Proposed reusable prefab (under the existing LegendsOfRus asset tree), with a matching deterministic editor builder and serialized effect definition:

```
DirectionalEffect
  PoseRoot              # Image + Animator or sampled pose curves
  LaunchPlume           # local sprite sequence
  ProjectileHeads       # two oriented bodies
  TrailLayer            # clipping-aware batched mesh
  SparkEmitter          # pooled, Canvas-compatible particles
  ContactLayer          # driven by real cell contacts
  ResidualWisps         # short optional low-opacity tail
```

Use Animator for the body's pose if it improves authoring; parameterized curves remain appropriate for path travel. Neither Animator events nor particle collisions may resolve gameplay. The presentation controller consumes the existing immutable plan/events, synchronizes its visual clock, and returns pooled resources on completion, cancellation, disable, exit and command recovery. Keep randomness cosmetic and separate from the board RNG. Disable raycasts on every effect layer.

A stock scene ParticleSystem is not automatically a clipped uGUI element. ScreenSpaceOverlay renders after the scene. Therefore prototype simulation-to-Canvas particle rendering or retain a clipping-aware UI particle batch for sparks; do not switch the game's Canvas/camera/backend just to make a sample prefab visible. Test both normal overlay play and the capture harness. No new UI-particle package is assumed.

Particle starting values are design targets, in cell-relative units: non-looping launch burst12–20 sparks, lifetime0.16–0.35s, speed2–5cells/s, size0.025–0.07cell, modest tangential variation;0–6 optional wisps per activation. Trail follows the head rather than using free particles to approximate its path. Tune by captures, not particle count alone.

Unity's ParticleSystem exposes duration, lifetime, speed, size and particle limits; choose limits explicitly. These settings do not establish that the effect is mobile-efficient. [Unity main-module documentation](https://docs.unity3d.com/6000.0/Documentation/Manual/PartSysMainModule.html). Animator parameters can drive transitions and blend behavior; gameplay remains separate. [Unity animation parameters](https://docs.unity3d.com/6000.0/Documentation/Manual/AnimationParameters.html). Overlay ordering is documented in [Unity6000.5 ScreenSpaceOverlay](https://docs.unity.com/en-us/engine/6000.5/script-reference/unityengine/rendermode/screenspaceoverlay); the Canvas-compatible integration recommendation above is a project-specific inference.

## Rules that the examples must not replace

The supplied3×3 bomb example is illustrative. Our current presentation uses radius2 for a normal bomb and radius4 for a bomb pair, and actual affected cells come from the resolver. Do not change those footprints to match that example. Row+bomb already has its own planned footprints. Color conversions must use resolved targets and exclude hidden ordinary identities from fragment emission.

The guide's names are categories, not new inventory types: Rocket→Row/Column specials; Bomb→Burst; Color Bomb→Color special; Hammer→BogatyrMace's actual existing row/column action; Shuffle→SeaPearl. LivingLight and Firebird also exist and need their own later effect specifications. Do not replace the mace with a one-cell hammer or introduce a second color-clear rule accidentally.

## Completion gates and sequence

1. Directional item: layer isolation and phone-size art review → Canvas renderer/prefab → real formation/activation/chain captures → interaction/recovery tests → Android profile.
2. Bomb: separate ignition/flame/smoke/shell, preserving the actual footprint and obstacle stage events.
3. Color special: readable target selection, connection arrivals and conversion poses.
4. Mace, SeaPearl, LivingLight, Firebird: independent identity and contact sequence.
5. Combination choreography: compose those reusable effects; add an override only when the combination needs different staging. Avoid fifteen copied standalone systems.

Initial performance **targets, not measurements**: maximum256 cosmetic particles across the board; retain all required projectile/contact cues and reduce smoke then sparks under pressure. Start with at most two1024px effect atlases for the directional slice, reused textures where possible. Measure memory and actual imported dimensions; document every exception. Pool effect instances; require no steady-state per-particle managed allocation after warmup. Profile single row, full-board conversion, large chain and repeated cascades on a named Android device, including sustained frame time and overdraw. Aim for60fps/16.7ms total on the chosen device; the supported minimum tier must be decided from measurements.

Verification must cover both directions/orientations, near-edge origins, all affected cell contacts, chained specials, two-stage obstacles, outcome hashes, exactly-one move charge, cancellation/recovery, pause/resume, input release, safe-area layouts and reduced motion. Test visual degradation at the particle cap without losing gameplay-significant contacts. Final review must include actual Unity footage and final-state evidence; a generated clip is only design reference.

No new runtime art/prefab, Animator, ParticleSystem, gameplay change or Git commit was produced in this specification pass. The next concrete build target is this directional vertical slice.
