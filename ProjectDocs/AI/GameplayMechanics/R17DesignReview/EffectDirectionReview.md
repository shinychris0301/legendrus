# R17 — Gameplay effect direction review

Status: **Design review only; no Unity implementation.** Requested September 27, 2026. User reports that R16 improved substantially but still feels understated and too classic, and explicitly requests reviewing the direction before implementation.

[Interactive portrait review](Gameplay_EffectDirection_R17_Review.html)

## Recommendation

Keep the approved gems, items, obstacles and Russian-fairy-tale setting. Replace the prevailing ring/star/ribbon vocabulary with more physical action: forceful moving bodies, irregular thick fragments, explosive volume and carefully staged impacts. Brighter versions of the existing geometry alone are unlikely to address the user's concern.

First approve three hero effects: ordinary gem fracture, directional strike and bomb detonation. Implement and assess those in R4 sandbox play before extending their language to all other actions. This is a presentation redesign, not a difficulty or matching-rule revision.

## Evidence reviewed

- Existing R16 Unity sample sequences and current presentation sources: `GemShatterView`, `PrototypeImpactView` and its R15 partial, `MaterialBreakView`, `SparkFlightView`, `PrototypeBoosterView`, and R13 contact sequencing. The previously reported R16 verification is historical evidence, not a new test run.
- Official Royal Kingdom Rocket, Dynamite, Electro Ball, carried-Rocket and Rocket–Dynamite help animations. Downloaded and visually inspected temporal samples from each; URLs and image metadata are recorded in [ReferenceSources.json](QA/ReferenceSources.json). These are official help demonstrations, not a new hands-on play session or a phone performance recording.
- Three new code-native Canvas motion studies using this project's existing approved PNGs. These illustrate staging and particle volume only. They do not reproduce the game's board resolver, refill, final art, sound or performance.

Royal Kingdom's official examples visibly use travelling power-up bodies, substantial colored debris and staged activation/contact/recovery. Those qualities are useful reference targets. The GIFs also contain instructional holds and pointer motion; their total durations must not be treated as engine timings. [Official power-up examples](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/177-power-ups/?p=all), [official combination examples](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/178-power-up-combinations-1777447709/).

No claim of exact parity or of having recovered Royal Kingdom's implementation. Its sprites, interface, sound and effects are not production assets for this project.

## Why R16 still feels dated

These are visual assessments, distinguished from verified defects.

| Current evidence | Perceived limitation | Proposed change |
| --- | --- | --- |
| Repeated additive rings, pointed stars, radial flares and white-gold contact accents | Many powers share the silhouette of a ceremonial spell; more light does not create more mass | Assign a unique object motion and shape to each power; reserve repeated accents for a supporting role |
| Gem mesh wedges retain source textures but largely separate as thin pieces | Jewels can read as flat cutouts rather than hard objects | Uneven fracture topology, shaded side faces, a few substantial chunks and much smaller rear chips |
| Directional clears emphasize drawn energy trails | Travel and collision are less memorable than the beam itself | Strong leading projectile bodies, exhaust and precisely timed cell contact |
| Bomb core and circular shock front dominate the image | A luminous disc communicates radius but little explosive material | Brief compression, shaped flame/smoke volume, shell fragments and asymmetric pressure release |
| Firebird and booster artwork is primarily scaled, translated or rotated | Good item design can still move like a UI sticker | Articulated or authored pose changes, depth/shadow cues and distinct strike/recovery poses |
| Broad contact light on adjacent cells | Dense clears flatten jewel color and reduce hierarchy | One dominant attack; smaller local responses, preserved saturated facets and short light exposure |
| R13 provides causal contact and conversion sequencing | Strong foundation, but visual choreography is not yet equally expressive | Retain event-driven contacts and immutable outcomes while improving the presentation between them |

## A separate review-playback defect

Both `MechanicsMaterialFeedbackTests` and `MechanicsSpectacleTests` append to `Gems_Frames.tsv`, despite using different screenshot prefixes. R16's timestamp file contains an eight-frame segment followed by a ten-frame segment, each starting at index zero. Its review metadata reuses the Spectacle prefix for both segments, repeating sample indices 00–07.

The R17 page uses a **review-only reconstruction** from the final ten-frame segment and the existing Spectacle PNGs. [Reassembly manifest](QA/BaselineReassembly.json), [reproducible script](Sources/BuildBaseline.py). No invented or interpolated frames; the review still adds opening/closing holds and jumps to the settled state. Earlier R16 evidence is preserved.

This is a capture-indexing defect, not proof of a Unity animation stutter. Before the next implementation capture, namespace timestamp files by suite and run, truncate at recording start, and reject non-monotonic frame/time sequences. Do not judge subtle timing exclusively from these sparsely sampled captures.

## Proposed effect language

### 1. Ordinary gems and special creation

A matched jewel briefly compresses, then releases into four to six irregular main pieces, plus sparse chips. Use its actual texture and dominant color. Vary depth, spin, speed and size; keep fracture motion recognizably associated with its original cell. One tight contact accent supports the break. Avoid a large cross flare on every jewel.

On special creation, pieces draw inward into a visibly forming object. Let the new special settle once before another action takes focus. Creation should communicate transformation, not accidentally resemble destruction.

### 2. Directional items

The item orients/loads, then launches two readable heads along the actual cleared axis. Short tapered exhaust trails the heads. Each jewel reacts only when the head reaches it; fragments trail that contact rather than appearing simultaneously across the row. Columns reuse the same directional logic. No thick opaque beam lingering over the empty cells.

### 3. Bomb

A small compression and lift establishes a solid object. Release a compact hot core, irregular expanding flame and smoke lobes, shell fragments, then jewel debris. Use brief board-local recoil with a rapidly decaying amplitude; keep HUD and touch coordinates stable. The true model footprint determines affected cells. The Canvas example's illustrative area is not a proposal to change bomb radius.

### 4. Firebird and carried payloads

Give the bird takeoff, bank, dive and landing poses, with wing articulation. Its shadow and size convey lift. Leave clear space around the body; a ribbon is secondary. A carried bomb/line item remains visible and triggers its own effect only after arrival. Preserve destination selection and the existing causality chain.

### 5. Rainbow and conversion

Use a clear selection acknowledgement on the actual targets before discharge. Keep connections sparse and time-offset, with focus on the targets rather than dense intersecting arcs. Conversion must visibly produce the resulting special, then its subsequent activation uses that special's own effect. A large color celebration belongs to the release, not the whole action.

### 6. Inventory boosters

| Booster | Physical signature | Constraint |
| --- | --- | --- |
| Bogatyr Mace | Anticipation swing, heavy downward contact, short rebound and grounded splinters | Use existing hit footprint and inventory/move costs |
| Firebird | Wing-driven lift, curved flight, decisive target strike and feather embers | Keep the approved bird recognizable and destination readable |
| Living Light | Protective faceted form unfolds, then radiance propagates across its actual affected cells | Visual metaphor only; do not introduce a new shield mechanic |
| Sea Pearl | Water lens gathers visual energy, pieces rotate through the existing shuffle, release settles cleanly | Preserve existing shuffle/additional-move behavior and resulting model state |

### 7. Obstacles and combinations

Wood should split along plank structure; ice should crack and shed translucent plates; chains should tension and release connected links; a seal should fracture its face and retain its runic identity. A first hit is smaller and leaves the approved damaged sprite clearly visible. Final destruction breaks the remaining material. Do not bring back numeric obstacle badges.

For every supported special pair, stage a brief fusion followed by a recognizable combined attack. Line+bomb can emphasize a heavy central launch and multiple moving heads; carried payloads need flight then contact; rainbow combinations need conversion then activation. Use the actual resolved footprint, contact order and current rule version. Complex combinations deserve greater volume than ordinary matches, not longer unskippable spectacle on every cascade.

## Timing and asset proposals

The following are **our initial design targets**, not measured competitor timings, and are subject to phone review. No runtime constants were changed.

| Effect | Anticipation | Main action after anticipation | Residual tail |
| --- | --- | --- | --- |
| Ordinary break | 50–80 ms | 80–130 ms | 220–350 ms |
| Line | 80–120 ms | 180–260 ms to travel the board | 150–250 ms after local contact |
| Bomb | 100–160 ms | 120–220 ms pressure release | 300–450 ms |
| Firebird | 100–150 ms takeoff | 300–450 ms travel; 80–120 ms landing | 180–260 ms |
| Rainbow / combinations | 120–200 ms acknowledgement/fusion | Sequence-dependent, preserving real contacts | 200–350 ms after last contact |

Tails may overlap refill when they are decorative and no longer obscure it; input release must still respect actual board settlement. Avoid extending action locks merely to wait for smoke to disappear. The Canvas loops include extra review holds and simplified timing; they are not a production timing contract.

Needed original assets: a compact shaped blast/smoke flipbook, tapered exhaust, a few physical fragment profiles or shaded meshes, articulated Firebird poses, booster attack/recovery poses and material-specific break pieces. Retain current idle sprites. Prefer shared atlases, reusable pooled geometry and existing rendering infrastructure; no package, backend or color-space change is proposed. Inspect new transparent art over black, white and saturated backgrounds to eliminate dark halos and rectangular matte remnants.

## Approval and implementation sequence

1. User reviews this direction and the three illustrative motion studies. No Unity implementation before that review.
2. Build gem, line and bomb in the existing R4 sandbox. Correct capture timestamp namespacing. Preserve core result hashes, round difficulty, move/booster costs and approved idle art.
3. Capture continuous normal-speed motion plus slow motion. Compare the same deterministic fixtures against R16, then play real cascades. Tune art and timing before expanding the set.
4. Extend to creation, rainbow, Firebird/payloads, all inventory boosters, obstacle layers and supported pair combinations. Preserve causal event ordering and recovery on interruption.
5. Test physical Android hardware, including long cascades, before any production migration. Production remains Legacy until separately accepted.

## Acceptance checks for the implementation phase

- At normal speed, a reviewer can distinguish each power by motion and silhouette without reading a label; ordinary clears remain visibly smaller than special combinations.
- Jewel colors and unaffected targets remain recognizable at peak impact. No long white wash or smoke sheet hiding the board. Review overlapping cascades, not only isolated fixtures.
- First and final obstacle hits differ clearly; damaged artwork persists after a non-final hit. Every break uses the correct material response.
- Fracture/contact, carried-payload landing and special conversion align with their real events. No premature clear, duplicate explosion, hidden-color fragments or delayed stale effect after reset/navigation.
- Repeat determinism, all supported combination, cost, recovery and input-unlock checks. Compare relevant plan/state hashes with the prior rules corpus.
- Profile a representative Android device at its selected frame-rate target. At 60 fps the whole frame budget is 16.7 ms; measure VFX CPU/GPU cost, allocations, overdraw and peak active objects before fixing final particle limits. No performance claim is made from this browser study.
- Reduced-motion presentation removes recoil and broad flashes while retaining direction, contact and state changes. Sound/haptics should add weight with distinct contact accents, respect settings and be reviewed on device; this study is silent.

## Verification for this review

Browser checks at 430 px and 1440 px cover all three concept modes, corresponding R16 baselines, pause/scrub/replay, asset loads and horizontal overflow. See [BrowserReview.json](QA/BrowserReview.json). Images are browser screenshots of the motion study, not Unity evidence.

Unity compilation, runtime tests, audio/haptics, device performance and final visual parity are not tested in this design-only task. Game-file hash comparison is recorded in [Scope.json](QA/Scope.json).
