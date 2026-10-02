# Out of Energy reference rebuild R1

Implemented Visual Page 16 after the user approved continuing from Reward Received. The active OutOfEnergy prefab, owner and cutover state now agree on revision 2.

## Result

- Restored the original Russian НЕТ ЭНЕРГИИ lettering, three offer headings and original ПОПРОСИТЬ / СМОТРЕТЬ button captions.
- Restored original gold outer and inner frame perimeters. Scenario isolated the original panel and energy medallion; existing approved blank navy material supplies the cleared interior.
- Replaced the generic green energy badge with the original navy/gold face. The displayed count remains live. Repositioned the emblem, timer, icons, action captions and controls to source bounds.
- English remains live/localizable. After real energy regeneration, the static Russian heading hides and the live ENERGY AVAILABLE message appears. Close still returns safely without spending energy or automatically starting a round.
- Purchase, friend delivery and rewarded video services remain unavailable. Controls are disabled with availability explanations. The unconfigured 99 ₽ price is removed; no currency charge or ad reward is simulated.
- As a modal, this page dims the real Pre-Level page and its shared player header. The existing standalone route and environment remain supported. No duplicate header, full-reference backdrop or invisible screenshot hotspot was introduced.

## Asset workflow and provenance

Reference `/home/k2/Downloads/match3final/energy .png` is byte-identical to the existing `Resources/DesignScreens/OutOfEnergy.png`. Archived as `Sources/OriginalReference.png`.

Two new Scenario Pixa background-removal jobs: panel 3 CU and medallion 3 CU, **6 CU total, no paid retries**. Only alpha is used from these outputs; original RGB is retained. Existing Scenario panel/green/blue materials are reused for hidden blank surfaces. The count's baked zero is removed under a small local inpaint mask; the live count covers that area. Source button outlines and lettering are retained separately. Panel alpha excludes the independent Close control and unrelated lower scenery.

Black, white and magenta composites were inspected. `QA/OriginalRGBCheck.json` verifies **122,420 visible original pixels with zero RGB mismatches** across eight source assets. Repaired panel/badge/button interiors are intentionally excluded from that unchanged-RGB claim. Source and reused material hashes are in `Sources/AssetProvenance.json`.

## Verification

Unity **6000.5.9f1** rebuilt and read back the active prefab. Final **Verify03: 8 EditMode + 10 PlayMode = 18 passed** across Out of Energy and Reward Received regression tests. Earlier commentary incorrectly stated 19; XML evidence confirms 18. Baseline and intermediate logs are retained.

Tests cover energy gating, modal input blocking, real clock progression, persisted regeneration, full capacity, unavailable offers producing no service calls/grants, repeated binding/building, Close routing and RU/EN source/live text switching. Native captures cover 941×1672, 576×1280 and 1080×2400 with simulated safe areas, regenerated/full energy and standalone routing. Final Russian standard and English tall captures were visually inspected. Initial repair texture stretching and repeated donor ornament fragments were corrected before Verify03.

The standalone HTML review decoded all **10 views** with no JavaScript errors or horizontal overflow. Targeted source whitespace checks passed. No Android build, physical-device test, live purchase/ad test or full project suite was run.

## Scope

Task-start comparison: **8 existing Assets files modified, 26 added, none deleted**. All **121 prior prefab records retained**, now 141. Existing GUID/meta files, old artwork and other page prefabs remain unchanged by this task. Exact list: `QA/ScopeAndVerification.json`.

Modified implementation: ProductionOutOfEnergyRuntime.cs, ProductionOutOfEnergyView.cs, ProductionOutOfEnergyLayout.cs, ProductionOutOfEnergyPrefabBuilder.cs, OutOfEnergy.prefab and its cutover state. Existing EditMode/PlayMode tests were extended; 13 versioned sprites and metas were added.

## Remaining differences / next step

Actual energy capacity/count, the real countdown, availability copy and current underlying page differ deliberately from illustrative reference values. English and regenerated-state typography use project fonts; source font identity remains unknown. Hidden navy/green material, fine alpha edges and the small repaired badge face remain approximate. Tall screens preserve proportions with additional scenery margins. This is not a claim of pixel-perfect reconstruction.

Next: user visual review, then **Visual Page 17 — Booster Selection** if continuing. No additional Scenario jobs are scheduled.
