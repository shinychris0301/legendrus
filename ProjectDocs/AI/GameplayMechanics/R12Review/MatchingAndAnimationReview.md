# Matching and animation review after R11

The awkwardness has identifiable causes in both interaction rules and presentation. The approved R10 artwork should stay. More glow or sound alone will not resolve these issues.

## Evidence and scope

Reviewed current core/runtime source, executed **30 diagnostic cases against the actual GameplayCore sources** using Unity's bundled C# compiler/Mono, inspected existing R11 Unity captures, and viewed frame sequences from three official Royal Kingdom help animations (rocket, spinner carrying rocket, rocket+dynamite). The first standalone compile needed the netstandard reference; the corrected runner completed. This is not a new Unity Editor/Android runtime verification, new manual playtest, or claim of exact parity with Royal Kingdom.

[Reproducible probe](Sources/RulesProbe.cs) · [Runner](Sources/RunProbe.py) · [Observed results](QA/RulesProbe.txt) · [Source hashes](QA/SourceHashes.json).

**Two different rule paths are live:** normal campaign initialization still explicitly selects Legacy; the five-round mechanics sandbox selects PrototypeR3 and receives R9–R11 presentation. A player entering through the regular campaign will not experience the reviewed sandbox rules/effects. Preserve saved campaign compatibility while correcting and validating the candidate.

## Priority findings

### 1. Inconsistent special + ordinary-gem gestures — confirmed

On the same nonmatching adjacent board, tapping Row, Column, Burst, Spark and Color resolves. Swapping with an ordinary gem returns `InvalidSwap` for Row/Column/Burst but resolves for Spark/Color. This is not a pointer threshold problem: the core planner itself rejects those moves. If a hidden-color match forms, a bomb/line can activate instead, making the response depend on information the player cannot see.

Cause: `BoardResolutionPlanner.IsSpecialCombinationSwap` / `DetermineCombination` only recognize ordinary-gem partners for Color and Spark. Recommendation: make a deliberate, consistent contract for all candidate specials—tap activates in place; drag into a valid neighboring ordinary tile activates at the destination; blocked targets remain rejected without cost. This consistency recommendation is ours; the help text does not fully specify every gesture edge case.

### 2. Invisible gem colors still influence matches — confirmed

`BoardModel.FindRuns` compares tile IDs and does not exclude specials, including during square detection. The probe forms an ordinary match through a bomb with a matching hidden tile ID. `Match3Board.Refresh` hides the ordinary gem behind every special, so this color is not communicated. It also affects color targeting, counts and accidental cascades.

Recommendation: explicitly decide whether specials are standalone pieces or colored match pieces. For the current single-color R10 art, standalone pieces are clearer. Exclude their hidden color from ordinary run/square matching and ordinary-color frequency selection in a **new candidate rule version**. Continue activating specials through taps, swaps, combinations and blast chains. This is a design correction based on our visibility mismatch, not a claim that the official documentation specifies Royal Kingdom's internal match detector.

### 3. Rainbow + special chooses the wrong color for the requested reference — confirmed

`ResolveColorSpecialCombination` uses `partner.TileType`; Spark+Color likewise uses the Spark's hidden tile ID. Probe: ordinary color 2 is most common, partner color 5 occurs once; all forced activations still select color 5. Thus two visually identical specials can produce very different combination strength.

Royal Kingdom documents its Electro Ball combinations as converting the most common match-item color. Change candidate combinations to a shared, deterministic eligible-color selector; retain chosen-neighbor color for rainbow+ordinary-gem. Decide tie handling explicitly. [Official combinations](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/178-power-up-combinations-1777447709/).

### 4. Payload and transformation results are calculated but not shown — confirmed

Spark+line/bomb instructions contain `Payload`; the corresponding blast footprint is applied in `ResolveSparkFlight`. However, `SparkFlightView.Present` ignores Payload entirely. Motion events do not carry it, and the impact renderer always shows the same small Spark arrival blast. A full line or bomb area disappears without a visible carried item or the correct landing effect.

Similarly, rainbow combinations enqueue forced activations directly. They do not visibly transform the selected ordinary gems into new specials before those specials activate. The official spinner+rocket and combination demonstrations make these phases visibly distinct. Recommendation: show merge → visible payload/conversion → travel/charge → property-specific activation → affected cells.

### 5. Visible attacks and affected-cell timing are disconnected — confirmed

`AnimateResolutionActivations` starts every activation in its flattened list together, holds for 0.32 s (or 0.58 s if any Spark exists), then starts a single clear wave. `AnimateClear` scales/fades all selected cells together. The travelling line head does not determine when each cell breaks. The planner sorts activations spatially, so the list is not an animation dependency order; a secondary special hit by a flight/blast can appear to activate before the parent visibly reaches it.

Recommendation: derive a bounded presentation schedule from attack origin, footprint and parent activation. Let a line break cells as its front reaches them; let bombs break outward from the center; trigger a chained special when its parent impact arrives. Preserve the plan's final snapshot, move cost, obstacle hit count and input lock through settlement. Do not accidentally damage a multi-stage obstacle once for every animated particle/beam.

### 6. Formation and combination emphasis is too generic — confirmed

Creation happens after other matched cells clear and currently uses a 0.22 s scale/alpha pulse plus a generic charge ring. There is no visible gathering of the matched gems. Pair combinations generally play individual activation effects together, without a merge or a clearly larger combined object. Double-bomb logic clears a radius-four footprint, but the visual still shows ordinary radius-2.3-cell blast drawings at the two origins; its scale does not explain the combined coverage.

Recommendation: add short convergence and materialization for creation, and distinct combined wind-up/footprint visualization. Avoid simply lengthening all waits.

### 7. Obstacle feedback and debris remain generic — confirmed

Obstacle impacts are published after the gem clear animation, then a common 0.12 s tile-background pulse plays before the post-clear snapshot changes the art. `PrototypeImpactView.Obstacle` shares ring/shard geometry across materials, mainly changing color and radius. Intact/damaged R10 sprites work, but impacts do not visibly crack ice, split wood, snap links or fracture a rune rim in material-specific ways.

`GemShatterView` uses the same eight triangular sectors, movement pattern and timing for every gem; the inspected line/bomb frames show a repeated geometric pattern. It also emits ordinary-gem fragments from a special's hidden tile ID. Recommendation: controlled deterministic variation, correct source material per piece, brief localized hit deformation, and the stage sprite change at impact. Keep the approved art and no-number obstacle presentation.

## Complete rule comparison

Reference facts below come from the official [Power-Ups](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/177-power-ups/?p=all) and [Combinations](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/178-power-up-combinations-1777447709/) pages. Matching means the documented broad rule agrees, not every undocumented edge case.

| Case | PrototypeR3 observation | Assessment |
|---|---|---|
| Four horizontal / vertical | Creates perpendicular Column / Row | Aligned; keep |
| Square | Spark/Firebird | Aligned; keep |
| L or T | Burst/bomb | Aligned; keep |
| Five straight | Rainbow | Aligned; keep |
| Overlapping shapes | Five > intersection > square > four; one per connected group | Local policy; reference priority undocumented |
| Creation location | Preferred destination if eligible; otherwise ordered middle; L/T limited to intersection | Can feel displaced; review consistent placement, do not claim exact reference rule |
| Line tap | Whole row/column | Aligned |
| Bomb tap | Two-tile square radius, clipped at board edges | Broad radius agrees; exact corner geometry not fully specified in help |
| Spark tap | Launch cross + target; obstacle-first scan order | Footprint broadly agrees; target is deterministic priority rather than help's described random item |
| Rainbow tap / ordinary swap | Most common stored tile ID / partner ID | Broad behavior agrees; hidden-special counting needs correction |
| Line+line (all three orientations) | Cross at swap destination | Aligned coverage |
| Line+bomb (either orientation) | Three rows + three columns at the post-swap bomb position | Aligned breadth; center differs from line+line and depends on which ingredient moved |
| Bomb+bomb | Radius four at destination plus source activations | Broad radius agrees; visual under-represents it |
| Spark+Spark | Three distinct targets when available | Aligned count; target policy differs |
| Spark+line/bomb | Payload effect at landing, not shown in flight | Core concept aligned; presentation incomplete |
| Rainbow+line/bomb/Spark | Uses hidden partner color | Confirmed documented-reference mismatch |
| Rainbow+rainbow | Board-wide clear, one obstacle interaction per affected obstacle in the wave | Broad behavior aligned |
| Chain obstacle hit by several effects in one wave | Deduplicated to one layer loss | Existing balance policy; preserve unless separately reconsidered |

All 15 unordered pairs of Row, Column, Burst, Spark and Color resolved in the probe. That checks acceptance of each pair, not full rule parity, visual quality, or all board edge/obstacle cases. Six shape examples and five tap/swap gesture comparisons were also executed.

## Recommended implementation order

1. **R12 interaction rules:** introduce a new sandbox rule version; fix special+normal gestures, hidden-color participation and rainbow combination color selection. Add explicit checks for both drag directions, blocked neighbors, overlap priority, corners, taps, cost, replay hashes, no-move detection and each pair. Preserve Legacy and existing PrototypeR1–R3 replay behavior.
2. **R13 causal animation:** add matched-gem gathering, combination merge, rainbow conversion, visible Firebird payloads and property-correct landing effects. Schedule clears/chain activations at visible contact and verify sound synchronization again.
3. **R14 material polish:** directional/radial fragments, wood/ice/metal/rune impact behavior, controlled debris variation and mobile performance checks.

Keep current move budgets, level layouts and art. Rule fixes can nevertheless change effective difficulty; replay the five-round witnesses and collect player feedback rather than silently retuning levels. This review changes no production or sandbox gameplay code/assets. Implementation is the next step; no improved animations are claimed yet.
