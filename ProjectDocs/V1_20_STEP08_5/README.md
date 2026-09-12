# v1.20 Step 8.5 — Rollback-Safe Shop Development Cutover and Android Regression Acceptance

This milestone authorizes **Shop only** for an explicit development cutover, and only after the actual Step 8.4 Unity runtime evidence is present and passing.

It does not authorize the other eight Wave 2 pages. Direct approved reference authority remains 1/9.

## Safety model

- ProductionCoreLoopPageRegistry remains the accepted ten-route core registry.
- Shop is activated through a separate Resources-backed ProductionMetaPageCutoverRegistry.
- The candidate visual prefab remains presentation-only; ScreenRouter owns all Shop hotspots and purchase navigation.
- MonetizationService / VerifiedEntitlementGate / GameState remain the authoritative purchase/reward boundary.
- Apply is interactive only and cannot run automatically or in batch mode.
- Active Shop prefab is written before the activation-state resource, so an incomplete copy is never intentionally activated.
- A one-time immutable pre-cutover snapshot is created before the first Apply and never overwritten.
- Rollback disables Shop cutover first, then restores/removes the active Shop slot to the exact pre-cutover condition.
- Functional and physical Android acceptance locks are cleared by rollback.

See EXECUTION_ORDER.md and ANDROID_REGRESSION_CHECKLIST.md.
