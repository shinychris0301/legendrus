# v1.20 Step 8.3 — Reference-Accurate Wave 2 Reconstruction Refinement

## Purpose
Refine the nine Wave 2 meta/progression/economy candidates without changing the accepted runtime authority or active production UI.

## Reference authority
- **DIRECT APPROVED:** Shop only.
- **DEMO DESIGN GUIDANCE:** Character Collection, Daily Reward, Profile, Out of Energy, Booster Select, Reward Claim.
- **STYLE-SYSTEM DERIVED:** Inventory, Special Offer.

Only a DIRECT APPROVED page may pass a strict production-reference fidelity gate in this milestone. Demo-guided comparisons are informational and must not be promoted to approved status.

## Reconstruction policy
- No `ReferenceOnly`, `DesignScreens`, or `ProjectDocs` asset may be a prefab dependency.
- No full-screen reference-derived runtime screenshot is emitted under `Wave02Refined`.
- Direct/demo reference pages are decomposed into semantic sectional production components (HUD/header/content/cards/footer), then assembled by the Unity prefab builder.
- Primary headings, player/resource values, product quantities/prices, and CTAs are restored as live Unity `Text` where the Step 8.3 masks remove them.
- Inventory and Special Offer remain style-system-derived until direct page references are approved.
- Wave 2 candidate prefabs stay presentation-only; they do not contain purchase/reward/ad execution paths.

## Source preflight
Shop strict source proxy now passes the Step 8.2 thresholds:
- correlation: **0.8272** (threshold >= 0.80)
- MAE: **0.0221** (threshold <= 0.10)

Demo-guided informational correlations:
- Character Collection: 0.9558
- Daily Reward: 0.9362
- Profile: 0.9506
- Out of Energy: 0.8572
- Booster Select: 0.9221
- Reward Claim: 0.9402

## Cutover decision
**NO_GO_FULL_WAVE02_CUTOVER** remains in force because only 1/9 pages has direct approved visual authority and no Step 8.3 Unity runtime/Android acceptance has been executed by this package.

Shop is now ready to proceed to actual Unity runtime fidelity validation. This does not itself authorize active Shop cutover.

## Unity verification
1. Build/Repair v1.20 Step 8.3 refined Wave 2 candidates.
2. Run the Step 8.3 self-test.
3. Run Step 8.3 four-aspect candidate QA (36 captures).
4. Re-run the Step 8.2 provider-boundary verification.
5. For runtime acceptance, run the Wave 2 Play-Mode capture/analyzer against the rebuilt Step 8.3 candidates.
6. Keep active production UI unchanged until a later explicit conditional cutover milestone.
