# Visual Page 14 — Daily Reward R1

The active DailyReward page now uses a native Unity prefab with independent sprites, live localized text, resource balances and real buttons. The previous screenshot/hotspot page and inactive Wave02 candidates are not runtime dependencies. The user additionally approved implementing a new seven-day schedule.

## Artwork and implementation

Scenario API completed six jobs: clean title plaque (12 CU), clean card (12 CU), reconstructed empty environment (13 CU), fresh reference-chest alpha (3 CU), header alpha (3 CU), card alpha (3 CU): **46 CU total**. Each was submitted once; no replacement generation or automatic retries. Prompts, job/status/billing, original outputs and source provenance are in Sources. Original chest RGB is preserved; header/card/environment are Scenario reference edits, not exact pixel replicas. Header/card retain opaque faces and transparent exterior. Alpha inspected on black, white and magenta.

Layout follows four upper cards, two lower cards plus a wide Day7 chest, monthly progress, next-reward text and claim/back actions. Existing approved portrait/resources/boosters/buttons are reused without changing their source assets. Text is live RU/EN using the project font resolver; exact reference-font identity remains unknown. Repeated builds reuse serialized objects; prefab/view/state revision1 agree. New owner: ProductionDailyRewardPrefabBuilder. Native routing uses the existing meta cutover registry, without a second Canvas, EventSystem or startup path.

## Seven-day schedule

Values are implementation choices consistent with the existing currency/booster types, not the sample amounts in the reference.

| Day | Reward |
|---|---|
| 1 | 500 coins |
| 2 | 20 gems |
| 3 | Up to 1 energy, respecting capacity |
| 4 | Living Light ×1 |
| 5 | Sea Pearl ×1 |
| 6 | Bogatyr Mace ×2 |
| 7 | 1,000 coins + 40 gems + Firebird ×1 |

Once per UTC date, using the existing device clock. Consecutive dates advance; after Day7 the cycle repeats; a missed date restarts at Day1. A date at/before the last claim is blocked. Legacy saves retain their last claim and treat it as Day1; no retrospective gifts or inferred monthly history. This is local persistence, not server-authoritative time or anti-cheat verification.

GameState clones the save, applies the complete reward, writes through SaveService, then publishes it. A blocked write leaves the live save/reward/date untouched. Additive fields record the last cycle day and current month's claim count; existing version6 legacy payload compatibility is retained. Home's daily badge uses the same availability policy. The UI refreshes on state/language/date changes, prevents duplicate listeners/claims, shows next reward identity, and disables claimed rewards. At full energy, Day3 grants zero and still consumes the daily claim, matching the displayed “up to” and capacity rule.

Monthly count/progress is real from this implementation onward. The 7/14/21/28-day chest icons are previews; **no monthly bonus payout is configured or granted**. This is disclosed on the page. Reference reward names/icons without an existing game equivalent were mapped to working boosters (notably Mace replaces the reference book).

## Verification performed

- Unity6000.5.9f1 import/compilation, owner prefab build and serialized readback: passed.
- Final run3 EditMode **7/7** (DailyReward5 + Shop2); PlayMode **9/9** (DailyReward5 + Shop3 + Home daily-badge1); zero failed/skipped.
- Economy checks: exact seven-day totals, duplicate rejection, JSON round-trip, cycle wrap, missed day, legacy fields, backward date rejection, month rollover, capped energy, overflow before mutation, actual SaveService persisted claim/reopen, and blocked-save no-grant behavior.
- UI checks: native bindings/no screenshot dependencies, repeated builder stability, live balances, RU/EN, safe-area/raycast/text fit, every next-reward description in both languages, disabled duplicate claims and single Back navigation.
- Seven actual DailyReward PNG/JSON pairs and seven Shop regression pairs saved. Final run3 claimedRU, Day7RU and576safeEN manually inspected; run2 standardRU/EN,576safeRU/EN,claimedRU/Day7RU inspected before the final preview wording/progress-fill adjustment. Final1080safe and Shop captures checked automatically, not manually inspected this pass.
- Runtime asset hashes, source chest RGB, opaque panel interiors, Python syntax, local HTML links, new C# whitespace and scoped source diff checks: passed.
- Baseline4651 production files:4647 unchanged,4 existing changed(importer,GameState,router,Home badge policy),27 new files;none deleted;all existing metas preserved. Existing page art/prefabs unchanged. No package, Android guard, signing or project-setting edits requested/performed.

Run1 had one wrong test expectation for Back (expectedShop; the existing DailyReward route returnsHome) and a visually narrow heading frame. Corrected both in run2; all14 checks passed. Run3 added explicit blocked-save/preview-fit coverage and refined next-reward wording/progress fill; all16 passed. Earlier evidence is retained.

## Not performed

APK/AAB build, physical-phone test, full-project suite, actual elapsed multi-day device run, server-time/anti-cheat validation, payment/ad/SMS tests, OS storage-failure injection, manual browser execution, extended profiling, calibrated color or exact-font verification. Save-failure test uses the existing unsupported-future-save write guard, not a physical disk failure.

[Review actual Unity captures](DailyReward_Alignment_R1_Review.html) · [Reference comparison](ReferenceComparison.md) · [Changed files](ChangedFiles.md).
