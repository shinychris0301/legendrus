# Daily Reward — original buttons R2

Implemented in the active native DailyReward prefab, revision 3. The original Russian **ПОЛУЧИТЬ** button appears on the available day's card and grants that day's existing reward. The original **ОТЛИЧНО** bottom button returns Home without claiming. Back remains available. After claiming, the card shows its live claimed status and hides Claim; the reward receipt flow is preserved. English uses live CLAIM/GREAT labels over blank button faces.

## Extraction

Original: `/home/k2/Downloads/match3final/daily rewards.png`. One new Scenario Pixa alpha job, **3 CU**, no paid retries. Segmentation retained adjacent card rails, so those were excluded locally. The lower button reuses R1's Scenario alpha. Source RGB and Russian lettering are unchanged: **0 mismatched RGB pixels across 59,519 visible pixels** in the two assets. English reuses the existing Scenario emerald material. Alpha reviewed over black, white and magenta. Three PNGs and their metas added; old art unchanged. See Sources/AssetProvenance.json and QA/OriginalRGBCheck.json.

## Implementation and verification

Updated ProductionDailyRewardRuntime, ProductionDailyRewardView, ProductionDailyRewardPrefabBuilder, native prefab and cutover state together. Kept existing Claim binding, added a separate Confirm binding and preserved all 352 prior serialized prefab records (361 total). Source Claim follows all seven available-day states. Day 7's chest/amount positions make room for it. No reward schedule or service changes.

Unity **6000.5.9f1**, final OriginalButtonsR2Verify02: rebuild/readback succeeded; **7 Edit Mode + 10 Play Mode tests passed**. Includes all-day RU/EN placement, localizable button labels, claim once/reopen, Day 7 receipt, close-without-grant, single navigation, save failure, schedule/energy limits, safe-area hit testing, Shop and Home regression checks. Tests use isolated storage. Initial run had two full-save equality failures because normal full-energy clock refresh advanced lastEnergyUtcTicks; test snapshots now allow only that monotonic timestamp change while requiring all other fields to match. Production energy behavior unchanged. Initial failure evidence retained.

Actual Unity RU/EN, tall safe-area, Day 7 and claimed views reviewed. Review HTML: **9 views decoded, no browser errors or horizontal overflow** at 390×844. Task-start asset audit: 8 existing files modified, 6 added, 0 deleted; other page prefabs and shared header unchanged. Shop change is test-only clock handling.

No Android build, physical-device test or full-suite run.

## Reference fidelity

[Detailed comparison](QA/ReferenceFidelityReview.md). The page is not an exact match: Day 7 chest/plaque/glow, monthly milestone icons/count badge, scenery and live type remain different. Saved balances and reward values intentionally reflect the working game. Recommended next visual pass: Day 7 and monthly reward hierarchy.
