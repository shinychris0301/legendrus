# 500-round chapters — gameplay and visual implementation brief

Date: 2026-09-20. User approved 20 chapters × 500 rounds, 10 sections × 50 rounds, a first 50-round representative section, and a scrolling map. User additionally directed consistent current styling and Royal Kingdom as the reference for difficulty, board, boosters, obstacles and effects.

This document records that direction and a concrete implementation specification. It does not add playable rounds or change the gameplay runtime. Exact numerical tuning below remains a development starting point, not a measured result or a claim about Royal Kingdom's private balancing algorithms.

## Fixed visual and gameplay constraints

Retain the approved Russian fairy-tale family: painterly scenery, navy lacquer, antique gold, sapphire, controlled emerald controls, original extracted Russian lettering where approved, localizable live values and approved fonts. Reuse the source-extraction-first Scenario workflow. Generate only assets/hidden artwork that cannot be extracted; archive requests, masks and actual cost. No new generation needed for this specification.

Keep the8×7 board, six current gem designs and restrained saturation. Keep original inventory-booster artwork, names and functionality. Line power-ups indicate direction through horizontal/vertical placement; no tiny companion gem or H/V badge. Gems remain visually calm at rest. Effects must explain an action while leaving the board readable.

Royal Kingdom informs mechanical clarity and progression structure. Our art direction, source panels, character designs, naming and chapter stories remain the accepted Legends of Rus designs.

## Verified reference and limits

Official documentation consulted on2026-09-20:

- [Power-ups](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/177-power-ups/): line-of-four rocket, square-of-four spinner, L/T dynamite and five-in-line color clear. Its rocket points perpendicular to the triggering line. Spinner targeting is described as random, not goal-seeking.
- [Combinations](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/178-power-up-combinations-1777447709/): combining specials increases coverage; line+line, line+area and color+special are distinct patterns.
- [Boosters](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/179-boosters/): distinguishes starting power-ups and inventory tools used during play. In-game tools do not consume a move.
- [Challenging levels](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/180-challenging-levels/): recommends opening the board and combining power-ups; states levels can be beaten without boosters or spending coins. Does not publish exact difficulty curves, seeds, win rates or adaptive rules.
- [Items](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/section/48-items/): examples include adjacent-match damage, layered blockers, power-up-only damage, color requirements and items brought to the bottom.

No frame-by-frame gameplay footage or device timing was measured in this research. Effect timings and difficulty scheduling below are our implementation proposals. Do not describe them as extracted Royal Kingdom values.

## Existing project baseline and gaps

Current playable catalog: Chapter1, levels1–10 only. LevelData already stores board dimensions, seed, fixed cells, objectives, obstacles, moves, difficulty, rewards, star thresholds, story and tutorial triggers. Validation explicitly requires ten Chapter1 levels; catalog/save/story/UI paths include Chapter1 assumptions. Inspect and migrate these together before adding content.

Existing board special kinds: Row, Column, Burst, Color. Existing combo enum covers color+normal/special, line+line, line+burst and burst+burst. A square-match flying special is not present in the special enum and must not be advertised as working.

Existing obstacle configuration supports ice(up to2layers), chain(up to2), crate(up to3); Firebird feather is a drop objective. Do not interpret an artwork depicting stone, webs or metal as proof of a corresponding mechanic. A three-layer crate takes three qualifying damage events under its rules, not necessarily three player moves: cascades/combos must follow the resolver's defined hit deduplication.

Inventory functionality to preserve: Bogatyr Mace clears selected row/column; Firebird clears3×3; Living Light clears a chosen color; Sea Pearl adds5moves. The Firebird inventory action is separate from any future flying match-created power-up.

## Implementation order and completion gates

1. **Chapter Map scale:**500plannedslots perchapter,10sections of50,6–8visible nodes, reused offscreen nodes/scenery, current-level jump, saved-position restoration. Stable ID=(chapterId,levelId); optional global index=(chapterId−1)×500+levelId. Keep published/playable status separate from planned/locked slots. Only data-backed levels can launch. No500prefabs or500live buttons. Later rounds sit above earlier rounds, preserving downward-drag progression. Keep header/title/footer fixed while path/scenery scroll together.
2. **Catalog/progression foundation:** load chapter catalogs through a manifest, separate authored content from planned count, support batches of50, remove hardcoded ten-level completion assumptions, and preserve existing saves. Planned chapter completion is round 500; clearing the last currently published round is a content boundary, not ChapterComplete. Next chapter opens only when completion conditions and published content permit. Reference samples and unbuilt slots never award stars/rewards.
3. **Mechanics/readability audit:** make the current special/combination and obstacle rules explicit, verify current behavior before tuning, complete damage-state artwork, and ensure VFX use resolver events. Add new square-match flying mechanics only with their own deterministic targeting, combo, save and tutorial support; no silent behavior changes to existing inventory tools.
4. **First50rounds:** author data against the curriculum below; preserve existing IDs/progress, review changes to tutorial/story triggers, test every changed starter level. Use handcrafted introductions and milestone puzzles, plus reviewed layout variants for practice. Do not publish automatically generated layouts directly.
5. **Scale to500:** nine further50-round content batches, each validated and playtested. Chapters2–20 follow the approved story/theme list, retain learned mechanics and add chapter-specific combinations rather than resetting the tutorial each chapter.

## Difficulty and board design

Use teaching, practice, normal, hard, recovery and milestone roles. A challenge is followed by breathing room. Increase strategic demands through access to objectives, blocker arrangement, color availability and combo opportunities; lowering moves is one tuning control, not the sole progression mechanism. Avoid making an inventory purchase necessary to satisfy a goal.

All first 50 rounds use the approved8×7 canvas. Early boards are open. Later boards create lanes and pockets with supported blockers. Holes, portals, moving partitions, expansion and multi-stage battle boards require new topology/resolution work and remain later extensions. A proposed mechanic must not appear in level data until it is supported and verified.

Start with two distinct missions. Introduce a third in a dedicated, forgiving tutorial round once prior mechanics are established; retain some later two-mission rounds for pacing. A proposed first three-mission lesson is round21, subject to the starter-level migration review; current data already uses three from round6. No runtime threshold changed by this document.

First-section curriculum:

| Rounds | Objective of the group | Primary mechanics | Mission count | Review gate |
|---|---|---|---|---|
|1–5|Learn swaps, cascades, moves and visible goals|Color collection, open board|2|Clear onboarding without using inventory|
|6–10|Learn deliberate line power-up creation|Four-match, horizontal/vertical placement|2|Player can predict the cleared line|
|11–15|Open blocked areas|Single-layer ice and crates, then practice|2|Hit feedback and goal counts agree|
|16–20|Plan area/color clears|L/T and five-in-line, supported combinations|2|Distinct creation rules and footprints|
|21–25|Introduce a third mission gently|Known mechanics with reduced obstruction|3lesson, then2or3|No simultaneous new blocker introduction|
|26–30|Learn remaining existing blockers|Chains, layered damage states|2or3|Each layer visually distinguishable|
|31–35|Route a collectible to an exit|Feather drop paths and blocking lanes|2or3|Reachable exit and reliable collection|
|36–40|Plan power-up combinations|Mixed known mechanics|2or3|Useful opportunities without forced inventory use|
|41–45|Apply skills with recovery rounds|Mixed objectives, varied layouts|2or3|No sustained difficulty spike|
|46–50|Consolidate section skills|Handcrafted milestone and story/reward beat|2or3|Human playtest and progression handoff|

Each group uses a teach/practice/challenge/recovery sequence as appropriate; round 50 is a section milestone, not chapter completion. Exact move limits, quotas, rewards and target completion rates are authored after simulation and human testing. Do not copy current ten-level targets to490extra levels.

## Power-up and booster specification

| System | Intended behavior/clarity | Status/action |
|---|---|---|
|Line special|Four in a line; unmistakable row/column sweep; orientation conveyed by the art itself|Existing Row/Column mechanics. Audit creation-direction rule against tutorial and reference before any change.|
|Area special|L/T shape produces a visibly distinct area clear|Existing Burst. Make footprint and obstacle hits explicit; tune for8×7rather than importing another game's radius blindly.|
|Color special|Five in a line; clears one color with readable target selection|Existing Color. Audit tap/swap behavior and combos; no attached gem fragment.|
|Flying special|Square-of-four creates a small targeted projectile|Later mechanic candidate; requires new matcher/resolver support. Retain Firebird inventory3×3action.|
|Combinations|Communicate enlarged coverage before the hit; one authoritative damage sequence|Audit all existing combo kinds, edge clipping, overlap deduplication, obstacle layers and score attribution.|
|Inventory tools|Keep current four artworks/functions and quantities; clear targeting/cancel feedback|Do not replace Sea Pearl with a shuffle or Mace with a one-cell hammer merely to mimic reference.|
|No-move recovery|Animated reshuffle, stable obstacles/objectives, valid resulting move|Audit existing path and repair as needed. Distinct from a paid inventory tool.|

## Obstacle progression and artwork

For each obstacle define: occupied layer, whether it blocks swaps/falling, qualifying hit source, damage-event deduplication, layer count, visual state, goal-count timing, interaction with drops/specials, and removal effect. Count goal progress consistently on removal or defined collection events, not every damaged layer unless that goal explicitly measures layers.

First section uses current crates/ice/chains/feathers. Later chapters can introduce sealed relics(power-up-only), colored rune locks(color-qualified hits), source objects(release collectibles), linked barriers and spreading curses. These are our themed proposals informed by reference obstacle categories; they are not implemented and exact counts are not adopted from Royal Kingdom. Introduce one new rule at a time.

Each layer needs original-size readable artwork: visibly changed planks/bands for crates; cracks/missing coverage for ice; removed chain links/locks for chains. Match approved gold/wood/ice materials. Inspect at real cell size and on bright/dark board areas; no blurry enlarged fragments or misleading states.

## Effects and interaction

Use an event sequence: anticipation → clear path/area → impact → damage-state update → fall/refill → goal feedback. State changes come from the deterministic resolver, not particle collisions or animation completion guesses. Pool temporary visuals and cap overlapping effects.

Starting tuning ranges, to measure in our game: ordinary match response roughly0.12–0.20s, special-creation accent0.18–0.30s, line sweep0.20–0.35s, area impact0.25–0.40s, color-clear sequence0.35–0.60s. These are proposals, not observed reference timings; do not stack blocking pauses for every tile. Maintain clear input ownership during resolution.

Use thin warm-gold trails, compact Firebird embers, ice-specific fractures and brief controlled highlights. Remove tiny gem companions, persistent bloom and full-screen white flashes. Show chain reactions in a readable order. Provide reduced-effects behavior for shakes/flashes/trail density without changing resolution. Preserve the user's calmer visual direction.

## Acceptance and measurement

Validate unique stable IDs, contiguous published batches, supported goals/obstacles, board bounds, move budgets, reachable drop exits, valid initial moves, completion handoffs, and exact-once rewards. Test no-match shuffles, invalid swaps, special creation/combos, damage layers and save migration. Repeated board seeds must reproduce a reported defect.

Simulation covers a range of seeds and strategy policies; it can flag deadlocks or suspicious difficulty but cannot prove human fairness or universal solvability. Record completion rate by policy/seed, moves remaining, reshuffle frequency and booster-free outcomes, then conduct human playtests of every teaching/milestone round and representative variations. Separate initial development fixtures from production content and observed metrics from desired targets.

Phone acceptance:6–8legible nodes, smooth scrolling/recycling, selected level remains stable, no stale button callbacks, current-level jump works, and content endpoints cannot launch missing levels. Check low-memory behavior and device frame time; desktop Unity results alone are not phone performance evidence.
