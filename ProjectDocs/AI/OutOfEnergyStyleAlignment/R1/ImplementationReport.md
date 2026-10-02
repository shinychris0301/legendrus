# Visual Page 16 — Out of Energy R1

Implemented 2026-09-17 in Unity 6000.5.9f1. The existing energy gate now uses a native navy/gold panel with a live count, the real refill countdown, clearly unavailable service rows and a working Close button.

[Actual Unity review](OutOfEnergy_Alignment_R1_Review.html) · [Reference comparison](ReferenceComparison.md) · [Changed files](ChangedFiles.md)

## Implementation and existing behavior

- New active `Resources/ProductionUI/Pages/OutOfEnergy.prefab`, `ProductionOutOfEnergyPrefabBuilder`, matching runtime/view/layout revision 1 and `OutOfEnergyCutoverState`. Legacy Wave02 resources retained. Existing registry supports the route; no registry or startup changes.
- Separate panel/emblem/button/icon sprites, live localizable RU/EN text, four native Buttons and the existing raised count badge. No reference screenshot dependency, invisible hotspots, duplicate Canvas/EventSystem or new fonts.
- Modal entry after an unsuccessful Pre-Level start retains/dims the actual live Pre-Level page. Closing preserves its state and allows another Play attempt. Standalone entry from existing resource-header routes remains standalone: clean existing ChapterMap scenery behind the same panel, Close follows existing Back behavior to Home. No routing redesign or copied contextual screenshot.
- Reads `GameState.Save.energy` and `EnergyRemaining()`, and invokes the existing `RefreshEnergyFromClock()` to keep the visible count and countdown consistent. Uses the existing 30-minute interval, capacity and persisted timestamps; no new regeneration rules or timers. At nonzero energy it reports availability, without automatically starting a level. At capacity it shows “Energy is full” and no misleading countdown.
- Three service rows retain purchase/friend/ad callbacks in the router, but are explicitly disabled in the native view. Current commercial ports cannot durably fulfill these offers, no verified price catalog exists, and friend requests are only local demo records. Merely binding a runtime port is insufficient to enable an offer. No hardcoded reference price, simulated purchase, ad reward, or friend energy is presented as operational.
- `CanUseOffer` is an intentional fail-closed UI guard, not live provider readiness discovery. Future feature work must supply a verified display catalog, readiness/fulfillment contract and real friend service before enabling these controls. Existing Failure continuation, monthly rewards and SMS follow-ups remain deferred.
- Existing GameState, monetization services, Android guards, packages, project settings, signing, Gameplay code/art/prefab and all earlier page art/prefabs were not edited. Router changes only replace OutOfEnergy bindings; the existing Pre-Level test helper now presses the native Close button instead of the removed hotspot.

## Scenario / provenance

Five successful Scenario jobs, **33 CU total**: empty panel 12, controls 12, icon isolation 3, panel isolation 3, controls isolation 3. No duplicate paid submissions or corrective regeneration. Original requests, reference upload records, jobs, statuses, outputs and billing are in `Sources/Scenario/`.

The panel was reconstructed from the reference with all lettering/icons/buttons removed while retaining opaque navy surfaces and the four inner decorative frames. The control sheet reconstructed a clean medallion without the baked “0” and two buttons without lettering. Small icons preserve original reference RGB.

Scenario background removal retained navy rectangles around some controls/icons. Local contour masks removed exterior patches and neighboring gold-row fragments, preserving the medallion's opaque center, button surfaces and RGB. The cleanup is documented and reproducible in `Sources/PrepareAssets.py`; final source/output hashes and operations are in [AssetProvenance.json](QA/AssetProvenance.json). All eight final sprites were inspected on black, white and magenta. This is hybrid Scenario processing and local cleanup, not perfect automatic extraction. Some fine silhouette edges remain approximate.

## Checks performed

Final [Unity run2](UnityVerification/run2/): import, C# compilation, native prefab rebuild and serialized readback passed (`OUT_OF_ENERGY_R1_REBUILD_PASS`). All three Unity process exit codes were 0.

- **10/10 Edit Mode passed**, zero failed/skipped: OutOfEnergy 6 (including four countdown boundary cases), RewardReceived 2, Shop 2. Checks native bindings, importer alpha/uncompressed/no-mip settings, screenshot-free dependencies, four buttons, repeat builder identity, countdown rounding and adjacent page regressions.
- **14/14 Play Mode passed**, zero failed/skipped: OutOfEnergy 5, RewardReceived 5, Shop 3, PreLevel energy-gate regression 1. Checks actual energy-gate opening, repeat Close, reopen, blocked underlying controls, inactive service callbacks/no grants, actual saved regeneration, full capacity, no automatic level start, and resource-header standalone entry/return. Existing Pre-Level regression verifies closing the gate and playing after energy restoration spends exactly one energy and no booster stock.
- Eight OutOfEnergy, nine RewardReceived and seven Shop capture/metadata pairs. OutOfEnergy text-fit, pointer-hit and safe-area assertions at 941×1672 RU/EN, 576×1280 simulated safe area RU/EN, 1080×2400 simulated safe area RU. Standalone background bounds/aspect are checked against the full rendered display. These are real Editor Play Mode captures with isolated saves and simulated display dimensions, not Android screenshots.
- Manually inspected final standard RU, tall EN, regenerated RU and standalone RU. Other captures have automated checks. [Run1](UnityVerification/run1/) also passed 10 Edit Mode + 13 Play Mode, but visual inspection caught the low ad caption and standalone background gap. Both were corrected, and the additional existing Pre-Level regression ran in final run2. Earlier evidence retained.
- Scope snapshot: **4,720 original Assets/LegendsOfRus files; 4,718 unchanged; 2 modified; 33 new; 0 deleted**. All existing `.meta` files unchanged. [ScopeComparison.json](QA/ScopeComparison.json) names every file.
- Additional Python syntax, source RGB/hash, review-link and scoped whitespace checks are recorded in `QA/StaticChecks.json`.

## Not performed / remaining limits

No APK/AAB build, physical-device or Android hardware-Back test, full project test suite, browser execution of the HTML review, calibrated color/font validation, performance profiling, real 30-minute wait/background-resume trial, server-clock/anti-cheat work, actual purchases/ads/friend delivery or provider activation. Regeneration was tested by changing isolated saved timestamps and exercising the existing clock service.

This is an implemented R1 awaiting user visual review, not pixel-exact fidelity or final user acceptance. Remaining differences include fine frame engraving, flat live serif lettering, green count badge versus the reference's navy badge, and existing background/portrait differences. The reference's enabled offers/99 ₽ are intentionally replaced by truthful unavailable states.
