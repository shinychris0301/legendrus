# v1.20 Step 8.2 — Wave 2 Unity Prefab Runtime QA, Provider-Boundary Verification, and Visual Acceptance

Baseline: **v1.20 Step 8**, confirmed compile-clean by the developer.

This milestone does not activate Wave 2. It adds a Play-Mode runtime-capture harness for 9 candidate pages at 4 target aspects (36 captures), a critical-provider/economy freeze guard, and a runtime visual analyzer. Shop is the only strict approved-reference visual gate. Demo DesignScreens are guidance-only; Inventory and Special Offer remain locked-style candidates without direct approved page references.

## Required Unity order

1. `Legends of Rus > Production UI > Build/Repair v1.20 Step 8 Wave 2 Candidates`
2. `Legends of Rus > Production UI > Validate v1.20 Step 8 Wave 2 Candidates`
3. `Legends of Rus > Tests > Run v1.20 Step 8.2 Provider-Boundary Verification`
4. `Legends of Rus > Tests > Run v1.20 Step 8.2 Wave 2 Runtime QA Readiness Self-Test`
5. `Legends of Rus > Tests > Run v1.20 Step 8.2 Wave 2 Unity Runtime Four-Aspect Capture`
6. `Legends of Rus > Tests > Analyze v1.20 Step 8.2 Wave 2 Runtime Visual Acceptance`

## Provider boundary

The Step 8 accepted `ScreenRouter`, `MonetizationService`, `GameState`, `VerifiedEntitlementGate`, and active production registry are hash-frozen. Candidate Wave 2 prefabs are required to contain **zero `Button` components** and no runtime behavior except the screenshot-free page marker. Therefore candidate artwork cannot independently grant currency, energy, entitlements, rewarded-ad rewards, friend energy, or purchase results.

## Visual preflight

The Step 8 source preview for Shop compared with its direct approved reference yields correlation **0.0752** and MAE **0.1801**. The strict runtime gate is correlation ≥ 0.80 and MAE ≤ 0.10. This preflight therefore remains **NO-GO**, and Step 8.2 intentionally provides no active Wave 2 cutover action.

Actual acceptance must use the Unity-rendered 9:16 capture produced by the Step 8.2 runtime harness; the preflight image is only an early warning.
