# R16 — Beacon block, rounds 41–50

Continues the user's request to test and implement gameplay into the project. The prior results-only reporting preference remains in force. Existing difficulty budgets are retained; no human R14 results were found. Automated solvability does not establish player difficulty acceptance.

The active package now extends the implemented first 40 rounds with the planned R7 beacon block. `Sources/AuthorAndInstall.py` reproduces candidate `Chapter01Sector01R16` and updates existing `Resources/CampaignActive` JSON files while preserving their metas. The existing runtime loaders, authoring window, progression, saves, rules, UI and art remain in place. Earlier candidate packages and legacy resources remain unchanged.

|Round|Moves / colors|Layout|
|---|---|---|
|41|33 / 5|Open red/blue recovery|
|42|28 / 5|Ten segmented outer crates with openings; red collection|
|43|29 / 6|Ten offset chain cells; green collection|
|44|26 / 5|Six ice cells; survive 16 consumed moves|
|45|26 / 6|Three feather lanes, ten crates including deeper two-layer pockets|
|46|34 / 5|Four small corner ice cells; green recovery|
|47|29 / 5|Open red/blue collection|
|48|29 / 6|Eight crates and eight chains in separate zones|
|49|28 / 5|Survive 18 consumed moves and collect blue|
|50|28 / 6|Three feather lanes through twelve crates, including layered lower cells; red collection|

Seeds 5141–5150, existing 8×7 board. Survival uses consumed moves, not real time. No new fire/defense mechanics, timed events, art, or effects. New first-clear totals: 1840 gold, 6 gems, one Mace and one Sea Pearl. No extra milestone reward.

After 50, Ivan lights the beacon and remains to help protect the village. This completes the first 50-round sector, not Chapter 1. Round 51 and Chapter 2 remain unavailable. The chapter's planned finale remains at 500.

Previous 40 level records, scenes, reactions and localized values are preserved. Normal-game tests cover entry, legal gameplay, results, the beacon scene, save migration from completed 40 to unlocked 41, reload and reward replay protection. Final outcomes are recorded in `TestResults.md` and `QA/Verification.json` after verification.

Animation/effect refinement remains deferred. Device testing and human difficulty acceptance remain outstanding.
