# Gameplay R9 — remaining dark portrait-underlay patch

The user rejected R8 with another close-up. R8 softened the boundary between generated and original scenery but retained the dark area itself. Its report’s claim that the strip had been removed was premature. The native HUD-hidden comparison remains useful for excluding a live overlay, but cannot establish that a baked scenery defect is visually resolved.

## Correction

One additional Scenario API crop edit (**12 CU**, no resubmission) replaces the full affected dark sky area behind the birch trees with blue-violet twilight and cloud lighting continuous with the adjacent scenery. A bounded smooth mask blends this into the existing environment. **87,179 pixels changed; 1,486,173 pixels remain unchanged**, with no changes outside the recorded mask.

Only the environment resource is replaced. The R7 portrait/header sprites and geometry, R8 booster-card repairs, captions, gems, board, controls and gameplay behavior remain unchanged. Hidden scenery is reconstructed, not exact recovery of the original image.

The active Gameplay prefab and its builder now agree on **revision 20**, with proof scope `gameplay-portrait-scenery-r9`.

## Verification evidence

Unity **6000.5.9f1** Build01 compiled, rebuilt and read back revision20 successfully. Verify01 passed **9 EditMode +32 PlayMode tests**. All **18 browser review choices** loaded without script errors or horizontal overflow. Scope audit found **4 existing Assets files modified, 3 added, 0 deleted**; all405prefabrecords retained.

Inspected actual941×1672and576×1280safe-area captures, the enlarged crop and both195×133before/after crops. The dark rectangular area retained in R8 is replaced by visible blue-violet sky/cloud detail continuing through the trees under the portrait. Final visual acceptance remains with the user; the test pass count alone does not establish it.

See `QA/ScopeAndVerification.json` for the final asset scope and Unity test results, `QA/SceneryChecks.json` for mask/control checks, and `QA/BrowserChecks.json` for review-page validation. Actual Unity full-screen captures and matching before/after close-ups are available in the review page. Small close-up choices retain a 195px display width to compare at the scale of the user’s screenshot.

The native HUD-hidden control checks **8,424 pixels** below the portrait. Scenery appearance requires separate visual inspection; a passing control check is not proof of visual fidelity.

Production changes: versioned `GameplayPortraitSceneryR9/environment.png` and metadata; `Gameplay.prefab`; `ProductionGameplaySourceControls.cs`; `ProductionGameplayView.cs`; `Step20ProductionUiFix01SerializedPrefabBuilder.cs`. Previous assets and GUIDs are preserved.

No Android build, physical-device run, full-project test suite or typography migration is included. Existing exact-font identity and Android font-binding limitations remain. Capture saves are isolated; special-piece and large-value examples are controlled fixtures.
