# v1.20 Step 8.4 — Wave 2 Unity Runtime Capture, Approved Shop Gate, and Conditional Cutover Readiness

## Scope

Step 8.4 is an evidence/readiness milestone. It does not reconstruct artwork again and does not replace active production UI.

- Nine Step 8.3 refined Wave 2 candidates are captured in Unity Play Mode at four target aspect ratios (36 captures).
- Shop is the only strict approved-reference visual gate because it is the only Wave 2 page with direct approved authority.
- Characters, Daily Reward, Profile, Out of Energy, Booster Select, and Reward Claim remain demo-guided/informational.
- Inventory and Special Offer remain style-system-derived without direct page authority.
- Provider/economy authority is re-verified against the accepted frozen boundary.
- A passing Shop runtime gate may establish `ELIGIBLE_SHOP_DEVELOPMENT_CUTOVER_READINESS`; it does not apply a cutover.
- Full Wave 2 cutover remains unauthorized (approved reference coverage 1/9).

## Unity execution order

1. `Legends of Rus > Production UI > Build/Repair v1.20 Step 8.3 Refined Wave 2 Candidates`
2. `Legends of Rus > Tests > Run v1.20 Step 8.4 Source Readiness Self-Test`
3. `Legends of Rus > Tests > Run v1.20 Step 8.4 Wave 2 Unity Runtime Four-Aspect Capture`
4. `Legends of Rus > Tests > Analyze v1.20 Step 8.4 Approved Shop Runtime Gate`
5. `Legends of Rus > Tests > Run v1.20 Step 8.4 Provider-Boundary Gate`
6. `Legends of Rus > Production UI > Evaluate v1.20 Step 8.4 Conditional Shop Cutover Readiness`

## Acceptance rules

Shop runtime acceptance uses the 941x1672 Unity render compared with the direct approved Shop reference. Thresholds remain correlation >= 0.80 and MAE <= 0.10. Six demo-guided page comparisons are informational only.

The distributed Step 8.4 package must keep Shop outside the active production registry and must keep the accepted 10-route core-loop production registry unchanged. No purchase/ad/reward action is invoked by capture or visual analysis tooling.

Any later Shop cutover must be a separate explicit rollback-safe milestone followed by full functional regression and physical Android acceptance.
