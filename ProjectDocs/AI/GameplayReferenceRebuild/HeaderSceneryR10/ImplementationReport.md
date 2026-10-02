# Gameplay R10 — scenery beneath adjacent header panels

The user reported that the stain remained beneath the panel beside the portrait. R9 repaired the portrait area only; the old generated-background repair also covered visible sky below the neighboring header panels and around the level plaque.

## Change

Restored original visible sky below the moves, goals and score panels and around the level plaque. **42,615 pixels in the fully restored region match the original RGB exactly.** The original level plaque and its shadow are excluded; only their hidden background is reconstructed using the already accepted R9 Scenario sky, matched to its surrounding sky through gradient blending. No new Scenario requests or credits.

The final composite changes **28,074 environment pixels**, preserves **1,545,278 pixels**, and changes nothing outside its recorded repair mask. R9 portrait-area repair, all header/portrait sprites, R8 booster-card corrections, typography, layout, gems, board and gameplay behavior remain unchanged.

Active Gameplay prefab and owning builder agree on **revision 21**, proof scope `gameplay-header-scenery-r10`. Original assets and GUIDs remain in place.

## Verification

Unity **6000.5.9f1** Build01 compiled, rebuilt and read back revision21. Final Verify03 passed **9 EditMode +32 PlayMode tests**. Verify01 had a prolonged startup delay; interruption was requested, but its result files subsequently showed passing tests as well. Verify02 encountered a transient closing-project lock. Both are archived as diagnostics; Verify03 is the clean final validation run.

All **16 browser review choices** loaded without script errors or horizontal overflow. Inspected the full-width before/after header strip and native941×1672/576×1280safe-area captures. The exposed strip’s **36,382pixels** match the corresponding HUD-hidden control; the original visible-sky comparison passes for42,615pixels. The old dark background patch around the level plaque is replaced with sky matched to the source surroundings. User visual acceptance is still pending.

Assets scope: **4 existing files modified, 3 added, 0 deleted**. All405prefabrecords retained; other page prefabs and prior art assets unchanged.

Actual Unity capture/test results and Assets scope are archived in `QA/ScopeAndVerification.json`. `QA/SceneryChecks.json` verifies original visible-sky pixels, bounded repair scope and native exposed-header-strip comparison against a HUD-hidden control. The review includes full native Russian/English and safe-area views plus before/after crops across the entire header strip.

Control comparisons establish whether a UI layer darkens the exposed scenery; they do not substitute for visual inspection. Hidden background reconstruction is not exact recovery of original pixels. No Android build, physical-device check or full-project suite is included. Existing exact-font identity and Android font-binding limitations remain.

Production changes: `GameplayHeaderSceneryR10/environment.png` and Unity metadata; `Gameplay.prefab`; `ProductionGameplaySourceControls.cs`; `ProductionGameplayView.cs`; `Step20ProductionUiFix01SerializedPrefabBuilder.cs`.
