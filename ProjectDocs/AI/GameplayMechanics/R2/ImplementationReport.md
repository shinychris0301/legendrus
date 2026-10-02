# Gameplay Mechanics R2 — square-created Firebird Spark

The approved next step adds a 2×2 special and a deterministic seeking flight to the development sandbox. The live campaign remains on Legacy. PrototypeR1 retains its rule ID and replay seed; PrototypeR2 is a new explicit rule version.

## Rules

A same-color 2×2 square creates a Spark. Overlapping match runs form one group, with one creation in the order **five-line Color → L/T Burst → square Spark → four-line directional special**. The preferred swap cell is used when it belongs to the winning shape and is eligible; otherwise placement uses a stable sorted choice. Existing specials and locked cells are not overwritten by a new creation.

Square detection participates in swap validity, cascades, legal-action queries, generation and reshuffling. `TryFindLegalAction` returns a pair of cells for a swap, or equal endpoints for a special tap. This is the shared core hint query, not a new player-facing hint button. A board where a square is the only legal move is kept rather than reshuffled.

| Activation | Effect |
|---|---|
| Tap Spark or swap it with a normal gem | Consume one move, clear source and orthogonal neighbors, fly to one target |
| Two Sparks | One move; three targeted flights, preferring previously uncovered targets |
| Spark + directional special | Carry the row/column clear to the landing cell |
| Spark + Burst | Carry the radius-two blast to the landing cell |
| Color + Spark | Transform the selected color group into planned Spark flights |
| Spark reached by another effect | Activate it in the same immutable resolution wave |

Targets prefer obstacles, then existing specials, then ordinary gems. Equal priorities use row/column order. The resolver avoids cells already covered by the wave before using a stable occupied-cell fallback. This is an explicit prototype policy, not an assertion of Royal Kingdom's proprietary targeting. It does not yet rank targets by outstanding collection goals or future board value. A crate can be targeted without a gem; objective items are not treated as ordinary gems.

Every Spark activation carries its destination and optional payload in the immutable plan and plan hash. The runtime never chooses a target. Replay uses the R2 rule version, before-snapshot, action and expected hash. Existing enum values are preserved; Spark is appended. Legacy/R1 planners reject Spark-containing boards rather than silently applying incomplete rules. No running campaign receipts have been migrated to R2.

## Presentation and sandbox

The existing approved isolated Firebird icon supplies both the board special and the flying bird. No source artwork or prefab was regenerated. A short arcing flight and gold embers stay within a masked board layer. The source icon disappears at takeoff; the landing ring and gem clear follow the planned flight. The old generic Spark flash and R2 settle glints are suppressed. Legacy and R1 cues remain unchanged. Existing inventory Firebird still means its original area booster; the new board special is earned through matching and uses no inventory.

Open **Legends of Rus → Development → Gameplay Mechanics Sandbox** in Play Mode at Home with no modal. Rules cycles Legacy, PrototypeR1 and PrototypeR2; Next cycles nine fixtures for the older rules and sixteen for R2. Fixtures 10–16 cover square creation, targeting, two Sparks, line payload, bomb payload, Color transformation and a chained Spark. Reset restores the fixture; Exit restores Home. These are synthetic correctness boards with oversized goals, not balanced campaign rounds.

In fixture 10, drag the ordinary gem at column 5, row 4 left to column 4, row 4. In fixtures 12–15, swap the adjacent specials in that row. Tap the Spark in fixtures 11 and 16. Columns/rows in this paragraph are one-based visual coordinates.

## Evidence and limitations

**Verification passed: 315/315 EditMode and 12/12 PlayMode tests**, with zero skips in the accepted runs. The final runs are Verify04 EditMode, Verify06 Spark, Verify07 baseline mechanics, and Verify03 campaign UI/board plus native win/fail regressions. The new Spark suite adds 18 EditMode cases; the runtime case exercises all seven Spark fixtures. Cascade-created Spark coverage is confirmed on generated seed 25. Browser checks passed 40 capture/viewport checks at 430/1440, playback at both widths, four local links, and zero JS errors.

[Test evidence](QA/Verification.json), [changed files](QA/Scope.json), and [capture review](Gameplay_Mechanics_R2_Review.html) record the verification. Captures are actual Unity renders, not generated mockups. The HTML sequence consists of sampled screenshots and does not measure real-time frame pacing.

The first EditMode run exposed accidental pre-existing matches in two test fixtures, which changed the shape priority. The fixtures were corrected; the priority rules were not weakened. The visual pass identified the duplicated takeoff icon and leftover generic flashes; these were corrected before the final capture. A later combined PlayMode capture run terminated with native Editor signal11 and no XML. It is not counted as passing; the cause is unconfirmed. The final capture suites were rerun in separate Editor processes. Only crash-generated InitTestScene files absent from the before-hash manifest were removed.

The presentation is an initial prototype. Goal-aware targeting, production rule migration, richer carried-payload choreography, effects intensity preferences, phone profiling, human difficulty testing and final special-silhouette differentiation from the inventory icon remain open. Existing crate/ice/chain artwork and damage rules remain unchanged. No new obstacle family, authored campaign round, live save change or Android release is included.

## Next step

Review this Spark behavior, then implement the bounded **runic seal** obstacle: two layers, damaged by direct specials/boosters, with deterministic blocker, goal, serialization and replay behavior. Keep first-50-round content production held until the mechanics and obstacle interaction are accepted.
