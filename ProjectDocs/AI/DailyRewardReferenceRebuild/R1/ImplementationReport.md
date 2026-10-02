# Daily Reward reference rebuild R1

Daily Reward native revision 2 now uses the shared Home player header, original Russian title and source-derived card/button/panel borders. The available day has an emerald face, Day 7 has a warm gold surface, and the monthly and next-reward areas have the original ornate frames. The existing seven-day reward schedule and save/claim services are preserved.

Reference: `/home/k2/Downloads/match3final/daily rewards.png`, SHA256 `a3baff5ea0f48a96d1403bb8a97fe0bd487505746497bc972f499712c1ee5565`. It exactly matches the original previously stored in DesignScreens, which remains outside production dependencies.

## Assets and fidelity

Three accepted Scenario jobs, no paid retries: Pixa title alpha 3 CU, Pixa main-button alpha 3 CU, one repair atlas 13 CU; **19 CU total**. Requests, returned billing, job records and unmodified outputs are archived. The atlas moved/resized the requested parts; inspected output coordinates are recorded and material repairs were remapped under bounded masks. Original frames and title RGB are retained outside recorded edits. New header alpha removes attached portrait/resource-bar fragments and restores the intentionally opaque navy face.

The existing clean hall environment, original isolated chest and Back arrow were reused. Day-card, monthly and next-panel perimeters use local contour masks. Hidden faces are reconstructed from Scenario output, not claimed to be original pixels. Eight versioned sprites plus metas were added; old art/metas are unchanged. Alpha reviewed on black, white and magenta.

Static Russian title lettering is source artwork; English title and all changing days, amounts, claim states, progress and rules remain live. The bottom button uses the original frame with a repaired empty face: its source caption means “Great,” while the current action actually claims the reward, so its localized Claim/Claimed wording is preserved. The single working Claim action remains below the cards, not embedded into a day card.

Remaining reference differences: regenerated hall scenery and hidden textures, simplified fine-edge masks, live font identity, existing functional booster illustrations instead of source book/shield examples, repeated milestone chest previews, missing source Day 7 title plaque and some source glow. Amounts, progress, statuses and unavailable monthly payouts intentionally reflect real functionality rather than reference examples. No pixel-perfect identity or user acceptance is claimed.

## Behavior

Full shared header: profile/settings plus coin, gem and life/energy values and existing resource routes. No new startup, Canvas or EventSystem. Available-day treatment follows the actual current day, and claimed/upcoming statuses remain visible. Back and reward-confirmation flow remain intact.

The seven-day schedule is unchanged: 500 coins; 20 gems; up to 1 energy; Living Light ×1; Sea Pearl ×1; Mace ×2; then 1,000 coins +40 gems +Firebird ×1. Local UTC claim policy, missed-day reset, save-failure handling, capacity limit and duplicate prevention are unchanged. Monthly progress is real; milestone rewards remain labelled unavailable. No new economy or server-time feature was added.

## Verification

Unity **6000.5.9f1**, final `Verify02`: compilation, builder/canonical-prefab rebuild and serialized readback passed. **7 Edit Mode +9 Play Mode =16 passed**, including Daily Reward, Shop and Home daily-badge regression. Covers exact schedule totals, cycle reset/wrap, legacy and backward-date cases, month rollover, energy cap, overflow/no partial grant, persisted claim/reopen, failed-save no grant, duplicate rejection, RU/EN and safe areas, actual next-day descriptions and one-time Back navigation.

`Verify01` also passed, but visual inspection found lower status labels near gold ornamentation. Those labels were moved inward and border-clearance assertions added; final Verify02 evidence supersedes the earlier captures. Native Russian Day 7/claimed and tall English captures were inspected, with seven actual Daily Reward captures available. Browser review: **9 views**, no script errors or horizontal overflow.

No Android build, device test, full-project suite, live multi-day elapsed test or server clock validation ran.

## Scope

**9 existing Assets files modified,16 added,0 deleted** against the task-start snapshot. All **333 previous prefab records retained** (352 total). Other page prefabs and prior artwork/metas are unchanged. Shared-header/navigation additions are scoped to Daily Reward; Shop regression passed.

Modified files:

- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/DailyRewardAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/DailyRewardAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionDailyRewardPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionNavigationStyle.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionDailyRewardRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPlayerHeader.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionDailyRewardView.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/DailyReward.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/DailyRewardCutoverState.txt`

New assets: `header_ru_r3`, `header_blank_r3`, `card_r3`, `card_active_r3`, `day7_r3`, `month_r3`, `tomorrow_r3`, `claim_blank_r3`, each with meta, in DailyRewardAligned. Source scripts, masks, provenance and scope audit accompany the review.

Next: user review, then Reward Received or requested Daily Reward refinements. No additional Scenario generation is scheduled.
