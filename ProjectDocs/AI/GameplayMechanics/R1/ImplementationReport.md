# Gameplay Mechanics R1 — development sandbox

This pass implements the first bounded mechanics prototype approved after Campaign Foundation R7. “Rounds” refers to level design; no group-rotation mechanic was introduced. The live campaign continues using legacy rules.

## Candidate behavior

| Action | Prototype R1 |
|---|---|
| Straight four | Horizontal match creates a column clear; vertical match creates a row clear |
| Tap an existing special | Activates Row, Column, Burst or Color for one move |
| Tap a Color special | Selects the most frequent tile color; lowest color ID breaks ties |
| Bomb | Radius two, a clipped 5×5 footprint |
| Two lines | One row and column through the swap destination |
| Line + bomb | Three rows and three columns through the bomb's post-swap position |
| Two bombs | Radius four through the swap destination, clipped to the board |
| Inventory boosters | Keep their existing meanings, stock accounting and zero-move cost; Sea Pearl adds five moves |

These are our candidate rules, informed by the [R7 reference review](../../CampaignFoundation/R7/GameplayReview.md). This is not an exact reproduction of Royal Kingdom's animations or a claim of measured reference timing.

`GameplayRuleVersion` leaves Legacy as the default. A development replay record pins the rule version, board snapshot, action and expected plan hash. The resolver produces immutable plans; animation cannot alter their result. Candidate no-move handling recognizes a tappable special as a valid action. Saved campaign receipts are still legacy-only; a rule-version migration is required before any production activation.

## How to try it

1. Open the existing startup scene and enter Play Mode.
2. Reach Home and close any modal.
3. Choose **Legends of Rus → Development → Gameplay Mechanics Sandbox**.
4. Use **Rules** to compare Legacy and Prototype R1, **Next** for the next fixture, **Reset** to replay, and **Exit** to restore Home.

The sandbox clones the approved Gameplay prefab inside the existing safe area. It creates no additional production Canvas, EventSystem or startup route. Its stock and board are disposable; it never reads or writes live progress. The developer control strip is not proposed production artwork.

Nine synthetic fixtures cover line activation, bomb with layered ice, color activation, line+bomb, two lines, two bombs, chains, crates and creating a line-four. These are correctness fixtures with intentionally oversized goals, not nine balanced campaign rounds. Drag the ordinary gem at column 4, row 4 upward into row 3 in the final fixture. Board-coordinate `down` means decreasing row index; the visual gesture is upward.

## Effects and remaining scope

The candidate adds a bounded procedural mesh layer for color-matched gem shards, blast rings and material-colored ice/chain/crate hit feedback. It uses the actual clear/obstacle instructions and is non-interactive. Existing approved sprites, board rails and HUD remain in use. Existing booster effects are retained alongside the new shared clear/hit feedback.

This is an initial implementation, not final effects polish. Full directional trails, targeted flight, hammer/firebird choreography, move-HUD flight, reduced-intensity tuning and Android performance acceptance remain outstanding. The square-created Firebird Spark and power-up-only runic seal are deferred to separate passes. Existing obstacle art remains in use; no new obstacle sprite set was generated.

## Evidence

Unity 6000.5.9f1 passed **297/297 EditMode and 11/11 PlayMode tests**, with no skips in those selected suites. EditMode, campaign UI (5), campaign board (2) and native win/fail journeys (2) passed in Verify03; the final effect-rendering correction and both sandbox tests passed in Verify06. The new core mechanics contribute 12 EditMode cases.

See [verification](QA/Verification.json), [scope](QA/Scope.json), and [interactive Unity capture review](Gameplay_Mechanics_R1_Review.html). Captures are actual Unity renders at 576×1280. The sampled sequence is for visual inspection and does not measure real-time frame pacing.

Initial validation exposed an old collapse-test expectation that replaced an occupied bottom tile. The test now correctly expects the falling special above that tile; production collapse logic was not changed. A subsequent sandbox test used the wrong coordinate direction in its four-match fixture; this was corrected. The visual audit also caught a missing CanvasRenderer on the new impact graphic; the final runtime check verifies active effects produce mesh vertices. One diagnostic run used an incorrect GetMesh overload and was corrected. Earlier unsuccessful runs are retained in QA.

No phone, IL2CPP build, full-project test sweep or human difficulty test is represented by the automated checks. First-50-round authoring remains held until the mechanics are reviewed.

## Next bounded pass

Implement the 2×2 Firebird Spark with deterministic target selection and complete match, hint/no-move, reshuffle, cascade, combination and replay coverage. Review its flight/impact on the same approved board before introducing the runic seal or authoring additional campaign rounds.
