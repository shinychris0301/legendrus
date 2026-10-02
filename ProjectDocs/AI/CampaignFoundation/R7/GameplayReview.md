# Campaign Foundation R7 — round design and gameplay reference review

Status: **design proposal, not installed gameplay**. User clarified that “rotation” means **rounds/levels**, not rotating or swapping gems. The new priority is round variety, obstacles, match-created power-ups and booster effects. The first 50-round briefs are retained as a provisional baseline; their move limits, targets and rewards must be rebalanced after the mechanic prototype. Existing accepted screens and artwork remain the visual baseline.

## What was actually reviewed

Inspected the local ten authored levels, story/RU/EN files, deterministic match resolver, combination balance profile, board input, animation code and pooled VFX implementation. Consulted Dream Games’ official Royal Kingdom help articles linked below on 26 September 2026. These establish documented rules; no Royal Kingdom device session or frame-by-frame animation measurement was performed. Proposed timing, targeting and effect choreography below are our design choices, not measured Royal Kingdom values. The official cinematic trailer is not evidence of board animation timing.

## Concrete differences and proposed rules

| Area | Current local implementation | Proposed development prototype |
|---|---|---|
| Round variety | Ten authored 8×7 boards; color collection, score, ice, chain, crate, feather descent and move-based survival. Later map positions are not authored rounds. | Hand-author short teaching/practice/mastery/recovery sequences around distinct board shapes and target access; prototype 10 representative boards before balancing 50. Keep approved board frame and gem art. |
| Four in a straight line | Creates a Row special for a horizontal match and Column for vertical. | Prototype a directional **Thunder Arrow**: opposite orientation to match, following Royal Kingdom's documented line rule. This is a rule change, not just new art. Keep the legacy resolver contract until the candidate is verified. |
| Four in a square | Match scanner detects straight runs of 3+; no square special exists. | Add **Firebird Spark**, a small seeking flame created by 2×2 matches. Separate silhouette from the full Firebird inventory icon. Deterministically target a useful remaining objective, with stable tie-breaks. This targeting preference is our proposal; the reference help describes a random item. |
| L/T match | Burst exists, with default radius1 (3×3 footprint). | Prototype **Thunder Stone**, radius2 (candidate 5×5 square clipped to board); a visible charge, radial shockwave and separate hit events. The help states a two-tile radius; exact footprint shape here is explicitly our choice. |
| Five in a straight line | Color special exists; board taps select cells rather than directly activate specials. | **Living Light** with colored links to affected gems and a clear convergence/clear sequence. Add explicit tap activation: choose most common color with deterministic tie-break; swap selects partner color. |
| Two lines | Same-orientation combinations activate two lines; mixed orientation activates each participating line. | Unified cross centered at the destination, regardless of the two line orientations. |
| Line + Burst | Three rows OR three columns plus a local Burst. | Three rows AND three columns; preview and VFX must cover the exact resolver footprint. |
| Burst + Burst | Configured radius2 (5×5) combined footprint plus source activations. | Prototype radius4 footprint, clipped at board edges; expect a major balance impact on 8×7 boards. |
| Inventory boosters | Mace clears selected row/column; Firebird clears3×3; Living Light clears chosen color; Sea Pearl adds5 moves. Campaign charge/refund receipts now exist. | Preserve these owned effects/IDs/counts in the first pass; make targeting and impact distinct. Do not silently turn Sea Pearl into a shuffle or the shield artwork into a different paid item. Review any behavior/icon reassignment separately. |

Royal Kingdom documents line-four, square-four, L/T and line-five as different power-ups, rather than making every match of four or more the same bomb. Its combination guide also distinguishes crosses, wider blasts, carried power-ups and color transformations. [Power-ups](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/177-power-ups/), [combinations](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/178-power-up-combinations-1777447709/).

In-game boosters are distinct from match-created pieces in the reference: they spend inventory and do not use a board move. Our first pass preserves existing ownership/economy and teaches that distinction clearly. [Boosters](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/179-boosters/).

## Round-design direction

Design around **where the player needs to act**, not just larger collection counts. Use open training boards, two pockets connected by crate corridors, feather lanes, layered center targets and broad recovery boards. Initially build these with existing stationary obstacles; irregular holes, conveyors, new gravity and multi-board combat require explicit engine work and are deferred.

Candidate ten-board slice, independent of stable campaign IDs:

1. Open collection board: evaluate gem readability, ordinary shatter and cascade pacing.
2. Line-four opportunity: teach direction, then provide another practical line target.
3. Isolated square-four opportunity: create and activate the seeking Spark.
4. L/T opportunity near crates: show blast reach and layer removal.
5. Straight-five setup: tap versus swap targeting, visibly distinct from a normal gem.
6. Line + bomb combination: compare the wider cross to either power-up alone.
7. Two-layer ice: first hit changes state; second removes it.
8. Chains + feather descent: expose why the path must be opened before collection.
9. A power-up-only **runic seal**: demonstrate why ordinary adjacent matches cannot damage it; ensure enough free space to create the required special without inventory.
10. Mixed mastery round followed by a low-pressure replay variant. No compulsory inventory use, purchase or external bonus.

These are QA/authoring fixtures, not ten additional shipped rounds. Evaluate ordinary swipes, special creation/activation, all combinations, each booster, fail/retry and restart recovery. Use a deterministic move trace for correctness, multiple seeds for robustness, and actual player sessions for difficulty. A solver win is not a human difficulty rating.

## Obstacles: behavior and readable damage

Keep existing ice, chains and crates, but give every remaining layer a distinct visible state. Ice cracks and sheds pale shards; a chain link snaps before the gem becomes movable; crates lose planks and reveal the opened gravity lane. The current resolver already has obstacle interactions and damage rules; it needs a clearer visual response, not a claim that damage logic is absent.

Add one new obstacle family first: **runic seal**, fixed on a cell, initially two layers. Ordinary matching does not damage it; a direct special/booster hit removes one layer per resolution wave. It neither spreads nor generates other pieces. Its blocker/occupancy behavior, goal counting on final destruction, replay serialization and legal-start constraints must be specified in the engine patch. Proposed choice: occupies its cell like a crate, blocks falling, requires direct power-up contact, and cannot contain a gem/feather. Do not place it at mandatory exits without a proven attainable removal sequence.

The reference offers clearly different interaction classes: boxes respond to adjacent matches; snow responds to matches on its cell and exposes ice; Magic Cards initially need power-up hits. These motivate distinct rules and staged surfaces rather than recolored copies of one blocker. [Box](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/194-box/), [Snow](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/195-snow/), [Magic Cards](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/509-magic-cards/).

Later generators/multi-stage enemies need their own specification and narrative placement. For example the reference's Golem splits into smaller enemies and introduces ivy pressure; that is a separate encounter system, not a skin for our crates. Defer it beyond this first village slice. [Giant Golem](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/451-giant-golem/).

## Effects specification

The existing VFX pool creates untextured UI Images and animates color, size, rotation and bounds. Match3Board additionally animates gems, special creation, activations and obstacle interactions. There is useful infrastructure, but pulses/fades alone do not communicate weight, material or the path of a hit. Preserve the pool, immutable resolution plans and watchdog/rollback boundaries.

| Action | Proposed visible sequence | Initial timing target, not measured reference |
|---|---|---|
| Normal match | Small compression, color-matched shards, brief sparkle; goal token travels separately | 100–180ms clear; trails may finish without blocking the next legal command |
| Special creation | Matched gems draw toward the creation cell; distinct silhouette appears and settles | 160–240ms; overlapping independent creations, not one long serial wait per special |
| Directional line | Short charge → two opposing trails → impacts ordered along the actual cleared line | 60–100ms charge +150–240ms sweep |
| Thunder Stone | Tight charged core → clipped expanding ring → gem shards and obstacle-specific debris | 80–120ms charge +180–300ms impact/release |
| Firebird Spark | Takeoff hits orthogonal neighbors → arcing flame trail → one selected target impact | 300–450ms total; destination chosen in the immutable plan |
| Living Light | Link source to exact target-color cells → synchronized clear → short residual light | 300–450ms total; avoid an opaque full-board flash |
| Mace inventory | Ghost of affected row/column while armed → hammer strike and directional shockwave | Target preview is immediate; impact follows committed reservation |
| Firebird inventory | Preview exact3×3 → wingbeat/fire dive → nine-cell impact and sparks | Different scale and silhouette from the seeking Spark |
| Living Light inventory | Preview selected color, confirm through target selection, then color links | Uses its own inventory transaction; not a free on-board activation |
| Sea Pearl inventory | Water/light spiral travels to moves HUD → explicit +5 count increment | No explosion or implication that it destroys tiles |

Reuse gameplay gems, approved booster icons, Firebird motifs, dark wood, antique gold and sapphire. New special/obstacle states and VFX textures need a small asset review against those sources. Do not reuse Royal Kingdom's artwork. Keep the ornate board frame and HUD clear of effects. Respect existing audio/haptics settings and support reduced intensity. Adopt a measured pool budget on the target Android device; current32/maximum128 UI slots are source facts, not a proven final particle budget or60fps guarantee.

## Engine constraints that must be designed before coding the complete set

- Keep old special enum values/save/replay formats intact; append new kinds under an explicit version and preserve legacy golden fixtures.
- A 2×2 detector must participate in match validity, hint/no-move search, reshuffle, cascades, creation priority and deterministic replay—not only the visual scan.
- Resolve overlapping shapes once per connected match group with reviewed priority: Color > Burst > Spark > Line. Define destination/intersection placement and stable tie-breaks; preserve unrelated existing special activations.
- Add an explicit tap-special command to the resolver, board, replay and input paths. Candidate rule: one move per valid on-board activation/combination, zero moves for inventory boosters, none for rejected input. This is our contract, not a verified reference move-cost claim.
- Define every Spark combination: two Sparks produce three targeted flights; Spark+Line/Burst carries its effect; Color+Spark transforms a chosen color group; unique useful targets first, stable fallback when none remain. Targeting is deterministic and effects cannot mutate game state.
- Do not change a saved running receipt's rule set after an update. A candidate gameplay-rule version must be pinned along with its level snapshot. R6 currently pins level JSON but not a selectable resolver rules version; this is required new work.
- Expanded blast ranges and new objective rules invalidate assumed balance. Preserve old first-clear claims; never fund a tutorial by silently refilling paid inventory.

## Delivery order

**Next implementation: Gameplay Mechanics R1 — development rule profile and representative round sandbox.** Start with line/Burst/Color activation, explicit tap command and material-specific hit effects; add square Spark and runic-seal support with full resolver/replay/hint coverage in subsequent bounded passes. Keep the legacy mode selectable for side-by-side comparison and regression tests. Then capture short Unity clips of identical actions and review effect clarity before balancing content.

After those mechanics are accepted, revise the held [50-round draft](Plan/First50Rounds.json), author concrete boards11–20, validate and playtest them, then extend to21–50. Preserve [story mapping](Plan/StoryMapping.json) and the round500 completion boundary. Do not expand all20 chapters against mechanics the user has rejected.

## Evidence from this pass

[33 static checks](QA/Validation.json) and [110 browser interactions](QA/BrowserReview.json) passed. [Scope audit](QA/Scope.json):7,269 existing Assets/Packages/ProjectSettings files unchanged; zero additions/deletions there. The review page and plans were checked, but no Unity compilation, new mechanic runtime test, balance test or Android test ran in this documentation-only pass.
