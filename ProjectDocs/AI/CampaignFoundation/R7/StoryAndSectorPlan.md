# Chapter 1 story and first-sector draft

Status: proposed, not applied. The user's later feedback prioritizes **round mechanics, obstacles, power-ups and effects**. [Gameplay review](GameplayReview.md) now governs the next implementation. First50 numbers below are held until that prototype is accepted and balanced.

## Proposed story decision

Preserve the accepted intro and six pre-finale scenes verbatim at their existing beats: before1, after1/3/4/6/8/9. Retain Dobrynya's early mentor appearance as an existing adaptation, without declaring Ilya or the full trio recruited before their later chapters. The raven remains a mysterious spy: do not assert that it is Chernobog, Koschei or the Nightingale in disguise. The narrative bible connects its role to Chernobog's spies, while the accepted dialogue leaves the identity implicit.

Keep integer round IDs1–10, seeds, board layouts, goal counts, historical stats and first-win reward claims. Their gameplay is the legacy comparison baseline, not proof the user accepts its mechanics going forward. Rule-profile changes require a separate versioned candidate. Never replace existing saved achievements with newly generated levels under the same IDs.

For the expanded candidate, replace the **round10 story trigger** with a new local hearth-recovery scene. Preserve the original finale text and scene source in the legacy package; propose its campaign counterpart after round500. Replace the round10 “finale” presentation/reaction wording in the candidate so it does not claim the chapter is finished. First50 ends with a beacon checkpoint, not departure from the village. The hero leaves for the Dense Forest only after500 and acknowledged finale, with Chapter2's package validated and installed.

The full story contains an apple tree blooming after the first matches. Proposed short after-round2 beat restores that omission. Additional draft beats after10/20/30/40/50 keep the first sector meaningful: hearth, well, granary, riverbank, beacon. [Six new bilingual scenes](Plan/NewSceneDrafts.json) are draft text, not approved copy or installed scenes. They promise local improvement; Firebird remains weakened, and the World Tree is not healed in Chapter1.

## Remaining chapter structure (coarse proposal)

| Rounds | Village task / narrative purpose | Production state |
|---|---|---|
|1–50|First hope: garden, hearth, well, granary, riverbank, beacon|50 briefs;10 baseline boards exist; mechanics/balance pending|
|51–100|Keep the wells and household fires alight|Outline only|
|101–150|Recover the mills and provision routes|Outline only|
|151–200|Restore orchard and fields; villagers begin helping|Outline only|
|201–250|Follow the river's shadow to old village wards|Outline only|
|251–300|Understand the stolen sparks; raven pressure increases|Outline only|
|301–350|Protect scattered homes during a renewed fading|Outline only|
|351–400|Reconnect the village wards using learned mechanics|Outline only|
|401–450|Gather the final sparks and prepare the old seal|Outline only|
|451–500|Break the seal, restore village dawn, depart for Chapter2|Outline; existing finale text retained as source|

These sectors are our proposed expansion, not50-round detail supplied by the original scenario. Each needs a distinct puzzle proposition before authoring; if a sector would only repeat quota inflation, redesign it before adding boards. No new boss system, relationship branch, hero customization or restoration-state storage is implied by this outline.

## Localization and migrated story state

Fresh inspection finds23 Russian keys absent from the English story file:20 reactions and3 completion strings. [Localization draft](Plan/LocalizationDraft.json) supplies English candidates while retaining exact Russian sources. Existing scene translations remain unchanged. Two legacy round10 reactions belong only to the old finale; the new campaign hearth beat needs local replacement reactions. Proposed copies:

- Victory RU: «Один очаг спасён. Теперь сбережём остальные огни деревни.» EN: “One hearth is safe. Now we must protect the village's other fires.”
- Failure RU: «Этот очаг ещё во тьме. Я попробую другой путь.» EN: “This hearth is still in darkness. I will try another way.”

New rounds11–50 need short reactions and tutorial keys only after mechanic terminology is fixed; they are not falsely marked localized here. Distinguish on-board Living Light from its inventory counterpart and Firebird Spark from the Firebird consumable. Current shield/potion pictures versus LivingLight/SeaPearl behavior remain a naming/art-semantic decision for the mechanics review.

Proposed migration: explicitly credit completed legacy intro/non-finale scenes to corresponding campaign beats without rewards. Keep the old finale acknowledgement in the legacy ledger; it must never acknowledge the campaign finale or unlock Chapter2. New apple/hearth beats for returning players can appear in one optional recap, not force replay or repeat currency. Persist scene acknowledgement after user advance; closing before advance reoffers the scene. No story ledger mutation is implemented in this pass. R6 has no campaign dialogue route; that work remains necessary.

Proposed sequence after a first win with a story beat: durable result → Victory → Continue/acknowledge → pending Dialogue → scene acknowledgement → map/selected Pre-Level. A scene read never grants a reward. Failed rounds show an optional reaction inside the existing result presentation only after a layout review; no automatically stacked dialogue modal. Finale uses its separate completion acknowledgement. Test app interruption at each boundary before activation.

## Round brief and economy limits

[First50Rounds.json](Plan/First50Rounds.json) stores baseline rounds1–10 plus40 individual proposed briefs, each with a puzzle intent, difficulty label, move/color targets, goals and first-clear reward. There are **no authored placements/seeds for11–50**, so it is not loadable level content. All candidate numbers are unbalanced design targets. Use [RewardBudget.json](QA/RewardBudget.json) to review the aggregate; it is not a grant instruction.

Baseline1–10 totals:1,850 gold,40 gems and two of each existing booster. Proposed11–50 totals:7,320 gold,24 gems, two Maces and one each of the other boosters. First50:9,170 gold,64 gems,4 Maces,3 Firebirds,3 Living Lights,3 Sea Pearls. Proposed replay and extra scene/sector grants are zero; existing reward claims remain authoritative. No bonus for moving the finale. R6's one-energy start/retry rule remains; no economy change accompanies this document.

Initial briefs insert a recovery round after each new hard round (15→16,20→21,25→26,30→31,35→36,40→41,45→46); round50 requires round51 recovery when that content exists. They use current goal types only. They must be rewritten where new Spark/seal/expanded-blast rules change the intended puzzle. Don't present this baseline as an approved Royal Kingdom-style balance.

## Acceptance before content activation

Validate unique stable IDs, contiguous available content, board dimensions, blocker layers, reachable feather exits, attainable goals, no illegal initial automatic matches, legal opening moves and deterministic command/replay hashes. Account for square matches in legal-move detection once added. Test every affected special/combination and obstacle damage event; a two-layer obstacle contributes to the objective only at the defined destruction point.

For balance, use at least three candidate seeds per proposed board and scripted no-inventory completion traces as a screening step. Then conduct human playtests and record first-attempt completion, moves remaining, retries, abandoned sessions and mechanic comprehension. Suggested initial bands are hypotheses: teach/recovery85–95%, normal65–85%, hard45–65% among new players without inventory. Report sample size and uncertainty; do not infer population win rates from a handful of testers or from solver results. Revisit quotas, moves, colors and layout together.

Approval of the previous foundation did not activate this story mapping or40 extra rounds. This is the concrete plan for review. Immediate next work follows GameplayReview.md: a versioned mechanics sandbox and representative rounds, before bulk content authoring.
