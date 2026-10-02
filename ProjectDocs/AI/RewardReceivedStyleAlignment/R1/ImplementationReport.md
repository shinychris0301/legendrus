# Visual Page 15 — Reward Received R1

Implemented 2026-09-16 in the existing Unity 6000.5.9f1 project. A successful Daily Reward claim now opens a native «ПОЛУЧЕНА НАГРАДА» confirmation with the actual saved quantities. The user can close it with «ЗАБРАТЬ» without receiving a second grant.

[Actual Unity portrait review](RewardReceived_Alignment_R1_Review.html) · [Reference comparison](ReferenceComparison.md) · [Changed files](ChangedFiles.md)

## Implementation

- Active prefab: `Assets/LegendsOfRus/Resources/ProductionUI/Pages/RewardClaim.prefab`; owner `ProductionRewardReceivedPrefabBuilder`; matching revision 1 and existing meta-page cutover registry/state. Historical Wave02 prefab retained.
- Separate opaque navy/gold panel, isolated original chest, existing reward tiles/icons and emerald button. Live RU/EN text uses the existing project's typography. No full-reference runtime dependency, invisible screenshot hotspots, duplicate Canvas/EventSystem, new startup system or font distribution.
- One actual reward type appears on Days 1–6, three on Day 7. The view supports all seven existing currency/booster types in centered rows. The seven-item capture is a synthetic presentation fixture, not a newly configured payout.
- `GameState.ClaimDailyReward(out DailyRewardGrant)` returns the balance differences only after the existing candidate save succeeds. The parameterless API remains compatible. Existing seven-day amounts, date rules, capacity caps, save schema and persistence guards are retained.
- ScreenRouter opens the confirmation after a successful claim, binds the receipt, blocks underlying navigation during the modal, and discards the receipt on close or route change. Repeat taps do not regrant. Reopening without a new receipt shows an honest empty state. Direct/debug page routing binds that same empty state.
- Energy at capacity displays an actual increment of 0 with an explanatory message; the existing daily schedule still records the day as claimed. No false +1 is displayed.
- The confirmation receipt itself is transient. A restart after a saved claim retains the inventory and claim date, without replaying the receipt. This adds no new transaction or offline inbox.

## Scenario and asset provenance

Five successful Scenario jobs, **33 CU** total: panel reconstruction 12, chest isolation 3, first panel isolation 3, corrective panel generation 12, repaired panel isolation 3. The first panel clipped the upper finial and was rejected. Exactly one corrective generation was made; no automatic resubmissions. Requests, job/status/output and billing records remain in `Sources/Scenario/`.

The chest retains the source reference RGB and Scenario-generated alpha. The repaired panel retains its generated RGB and Scenario-generated alpha, cropped only around transparent exterior margins. Its intentionally opaque navy center is preserved. Both final sprites were manually inspected over black, white and magenta. Source/output dimensions, pixel bounds, RGB checks and SHA-256 values are recorded in [AssetProvenance.json](QA/AssetProvenance.json). The source chest crop is input to isolation, not a scenery rectangle used as UI. External chest sparks removed by isolation are a documented remaining difference.

## Verification performed

Final [run2](UnityVerification/run2/): Unity import, C# compilation, prefab rebuild and serialized readback passed (`REWARD_RECEIVED_R1_REBUILD_PASS`). Exit codes for import and both test runs: 0.

- **9/9 Edit Mode passed**, zero failed/skipped: Reward Received 2, Daily Reward 5, Shop 2. Covers bindings/import settings/no screenshot dependencies/no duplicate native controls, repeat owner builds, existing seven-day schedule totals/reset/wrap/caps/legacy behavior and Shop regression checks.
- **14/14 Play Mode passed**, zero failed/skipped: Reward Received 5, Daily Reward 5, Shop 3, Home daily-reward badge 1. Covers committed receipts, once-only claims, duplicate confirmation, persistence readback, hidden-page input blocking, route disposal, reopening without receipt, capped energy and rejected persistence. The failed-save test exercises the future-version write guard, not an OS disk-failure simulation.
- Real Unity capture rendering and pointer/text/safe-area assertions at 941×1672 RU/EN, 576×1280 with simulated safe areas RU/EN, and 1080×2400 with simulated safe area RU. Nine Reward Received capture/metadata pairs, plus seven Daily Reward and seven Shop pairs. Rendering backend and dimensions are in each JSON.
- Final Reward Received Day 7 RU, tall EN, Day 1, capped energy and seven-type diagnostic were manually inspected. Other captures received automated layout/input checks. These are Editor Play Mode captures, not Android screenshots.
- First [run1](UnityVerification/run1/) retained: 9 Edit Mode passed, 12 Play Mode passed, 2 failed. Fixed two test timing assumptions (entrance gating and normal energy-clock maintenance) and visually identified seven-item spacing. Final run includes the spacing fix and safe-area offset clamp.
- Source scope comparison: **4,699 original files; 4,696 unchanged; 3 modified; 21 new; 0 deleted** under Assets/LegendsOfRus. Existing `.meta` files unchanged. Existing page artwork/prefabs, Gameplay implementation, Android guards, packages, signing and project settings were not edited. [Scope details](QA/ScopeComparison.json).

Additional static checks are recorded in `QA/StaticChecks.json` after documentation assembly.

## Not performed / limits

No APK/AAB build, physical-phone verification, full project test suite, performance/memory profiling, browser execution of the review, calibrated color comparison, exact-font identification, real elapsed multi-day trial, payment/ad/SMS work, or OS storage-fault injection. Direct/debug full-page routing and hardware Android Back were source-reviewed but not separately exercised by these new tests. Existing monthly bonuses and other deferred features remain deferred.

This is an implemented and tested R1 for user review, not a claim of pixel-exact reference matching or user acceptance. Next: review the actual portrait; prioritize original-style lettering/button engraving or a separate controlled chest-spark layer only if further fidelity work is requested.
