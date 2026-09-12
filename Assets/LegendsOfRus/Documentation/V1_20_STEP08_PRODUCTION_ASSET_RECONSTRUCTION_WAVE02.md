# v1.20 Step 8 — Production Asset Reconstruction Wave 2

## Scope
Wave 2 covers nine meta/progression/economy surfaces:
Characters, DailyReward, Profile, Inventory, Shop, SpecialOffer, OutOfEnergy, BoosterSelect, RewardClaim.

The accepted Step 7.4 Compile Fix 01 runtime/gameplay/navigation baseline is frozen. This milestone adds production-candidate art and Editor-only candidate build/QA tooling. It does not edit `Resources/ProductionUI/Pages`, `ProductionCoreLoopPageRegistry`, gameplay logic, save data, economy transactions, IAP fulfillment, rewarded-ad fulfillment, social fulfillment, Packages, or ProjectSettings.

## Reference classes
- Shop: **DIRECT_APPROVED_REFERENCE** (`Art/ReferenceOnly/ApprovedScreens/Shop.png`) used only for measurement/QA.
- Characters, DailyReward, Profile, OutOfEnergy, BoosterSelect, RewardClaim: **DEMO_DESIGN_REFERENCE** (`Resources/DesignScreens`) used only as reconstruction guidance.
- Inventory, SpecialOffer: **STYLE_SYSTEM_DERIVED** from the locked Russian-fairy-tale UI language and existing Wave 0 primitives.

No candidate prefab may depend on `Art/ReferenceOnly`, `Resources/DesignScreens`, or `ProjectDocs`.

## Art result
- 9 candidate page contracts
- 36 new page-specific PNGs
- 65/65 Wave 2 functional asset requirements mapped
- shared Wave 0 buttons, panels, tabs, resource icons, modal frames, ornaments and VFX reused instead of duplicated
- live Unity text retained for dynamic/localized values
- no font binary bundled; the Step 7.3 production-font slot remains the integration point for a separately licensed Cyrillic font

## Candidate output
On Unity import the Editor builder generates parallel prefabs under:
`Assets/LegendsOfRus/Resources/ProductionUI/Wave02Pages/`

Menu commands:
- `Legends of Rus > Production UI > Build/Repair v1.20 Step 8 Wave 2 Candidates`
- `Legends of Rus > Production UI > Validate v1.20 Step 8 Wave 2 Candidates`
- `Legends of Rus > Tests > Run v1.20 Step 8 Wave 2 Asset Reconstruction Self-Test`
- `Legends of Rus > Tests > Run v1.20 Step 8 Wave 2 Four-Aspect Source QA`

## Economy / monetization rule
The Shop and SpecialOffer candidates are display surfaces only. They do not grant currency, items, entitlements, energy, or rewards. Production fulfillment stays behind the existing verified provider/receipt boundary. Rewarded-ad and friend-request rows likewise remain display/interaction surfaces routed to the accepted functionality layer.

## Cutover state
**NO ACTIVE WAVE 2 CUTOVER IN STEP 8.**

Step 8 is an authoring milestone. Wave 2 activation requires a later runtime capture/fidelity pass, interaction smoke test, provider-boundary regression test, four-aspect Unity render QA, and physical Android verification. This is especially important because only Shop currently has a direct approved reference.
