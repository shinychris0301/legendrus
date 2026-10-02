# Daily Reward — reward hierarchy R3

Native revision 4 restores the original Day 7 chest and red/gold Russian title plaque, adds a repaired bronze/gold sparkle surface, and replaces repeated monthly chests with four distinct source chests. The monthly count now sits on a sapphire shield and drives a gold progress rail. Original R2 claim/confirmation buttons, shared header, actual seven-day rewards and receipt/navigation flows remain intact. Monthly milestone payouts remain unavailable and labelled.

## Asset workflow

Original: `/home/k2/Downloads/match3final/daily rewards.png`. Two accepted Scenario jobs: subject atlas alpha (3 CU) and bounded surface repairs (12 CU), **15 CU total**, no paid retries. Alpha output retained background around the pasted crops; local contours removed surrounding rails/numbers/material. Chest and Russian plaque RGB match **112,307 visible source pixels with zero mismatches**. Original outer panel and badge borders retained; hidden bronze, burgundy and blue material repaired. Gold rail uses a tiled exposed source segment; fill is a separate original-material sprite driven by real progress.

Black/white/magenta alpha sheets inspected. Eleven new PNGs and metas added. No existing artwork overwritten. Source lettering retained for Russian; English remains live over a repaired blank plaque.

## Implementation

ProductionDailyRewardRuntime, View, owning prefab builder, active prefab and cutover state updated together. Native plaque/badge images are noninteractive. All 361 prior prefab records retained (369 total). Tight chest dimensions center the source sprite without aspect-fit left alignment. Repeated builds preserve layer ordering, keeping live English title and monthly count above their backgrounds.

## Validation

Unity **6000.5.9f1**, final **RewardHierarchyR3Verify03** rebuild/readback succeeded. **7 Edit Mode + 11 Play Mode tests passed** with isolated saves, including Day 7 claim, duplicate protection, receipt/close, RU/EN, safe-area hits, all available days, source sprite assignments, centered chest, monthly counts 0/5/28/31, progress clamping, repeated-build layer ordering, schedule/persistence and Shop/Home regressions.

Earlier captures exposed left-aligned aspect-fit chest placement and, on the second rebuild, backgrounds drawing over live text. Both corrected; explicit regression assertions added. Earlier evidence retained. Final native RU/EN and month-28 captures visually checked. Review has **10 selectable views**, all decoded with no browser errors or horizontal overflow at 390×844.

Task-start audit: 7 existing Assets files modified, 22 added, 0 deleted; other page prefabs, existing art and shared header unchanged. No Android build, device testing or full suite.

## Limits and next step

[Reference comparison](QA/ReferenceFidelityReview.md). Day 7 chest is intentionally smaller than the static reference to fit actual reward descriptions and claim controls. Hidden textures/light and alpha edges remain approximations; monthly icons leave room for the availability notice. Live type, functional reward values/art, tomorrow composition and hall scenery still differ. Next recommended page after review: Reward Received.
