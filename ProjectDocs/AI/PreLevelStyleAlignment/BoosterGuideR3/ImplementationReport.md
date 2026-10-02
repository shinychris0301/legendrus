# Pre-Level R3 — booster guide and subtle scenery tone

The Pre-Level booster section now describes the four existing **in-game tools**. Tapping an icon opens its native inline guide with the original artwork/name, live stock and actual effect; Back restores the icons. The previous “Choose boosters” heading and route into an unimplemented selection modal are removed from this page. Play remains available from the guide and uses the existing energy gate.

The user's additional request is implemented as a **Pre-Level-only scenery tint: RGB ×0.88 relative to Chapter Map's environment color, with alpha preserved**. The tint is derived from the shared source each time, so alignment does not accumulate darkness. Shared PNGs, character/HUD, panels and booster artwork retain their colors. This scalar is a rendering setting, not a calibrated claim of 12% lower perceived brightness.

[Open the standalone portrait review](PreLevel_BoosterGuide_R3_Review.html). It includes nineteen final actual Unity portraits, a before-darkening R3 portrait, previous R2 and the original reference. Phone views simulate display/safe areas in the Editor; this is not an APK or device capture.

## Behavior

| Tool | Explanation of the existing gameplay action |
| --- | --- |
| Богатырская булава / Bogatyr Mace | Tap a cell for its row; swipe vertically for its column |
| Перо Жар-Птицы / Firebird Feather | Clear a 3×3 area around the chosen cell |
| Живой свет / Living Light | Clear gems matching the chosen gem's color |
| Жемчужина Морского Царя / Sea King's Pearl | Add five moves to the current round |

Inspection changes no save, energy, stock or board state. Empty inventory remains inspectable. Stock/language updates refresh an open guide; entering Pre-Level again resets it to the four icons. These actions remain usable during gameplay with their original costs and rules. **This is an inventory guide, not a newly implemented pre-game loadout.** That choice follows the actual tools, which need a board target or an active round.

## Implementation and preservation

Unity **6000.5.9f1 (b57deb96f08d)**; canonical resource `ProductionUI/Pages/PreLevel`, now native revision **3**. Modified the real prefab and its owning builder/runtime together, adding one inline detail group and a twelfth native button. Existing navigation is retained; only the Pre-Level booster callback changes from modal navigation to inspection. The legacy BoosterSelect screen remains available to its other callers.

Seven existing files changed:

- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/PreLevel.prefab`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPreLevelView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPreLevelRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs`
- `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/PreLevelAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/PreLevelAlignmentPlayModeTests.cs`

[Source audit](QA/SourceAudit.json): **1,726 of 1,733 baseline production files unchanged; all 896 existing metadata files unchanged; all 476 previous Pre-Level object IDs retained** (500 now). No new production files, image generation or image edits. Gameplay R11, matching/booster/energy rules, authored levels, earlier pages, shared fonts/art, packages, signing and Android guards remain unchanged. New small explanatory text uses the existing font with a flat face; the unknown exact font and device fallback limitation remain.

## Verification

Final run4: Unity owner rebuild/save/readback succeeded; **3/3 EditMode and 5/5 PlayMode tests passed**, zero skips/failures. Checks include canonical binding/resource integrity, shared art and page tint, all ten authored rounds and 2/3 objectives, rewards/replay, inventory updates, Back/HUD routing, energy gate and double-start protection. The new guide test exercises all four effects in RU/EN, stock zero and updates, returning to icons, entering Gameplay from details without stock consumption, unchanged move allowance and no armed booster. Captures verify active button center raycasts, safe-area containment, explanatory text line count/height and mesh containment, return lettering clearance and panel separation.

Saved **19 final PNG/JSON capture pairs**. Directly inspected final RU941 default, EN941 mace guide and RU576 pearl guide; inspected earlier RU941 pearl and narrow/EN guide captures to fix button proportions and alignment. Final visual review confirms the scenery darkening and foreground color retention; no exact reference-replica claim.

[Recorded test history](QA/TestHistory.json): run1 rebuild/EditMode passed; PlayMode 4/5 because the new test checked narrow input immediately after activating the detail hierarchy without a frame. Run2 waits for the normal UI lifecycle; 5/5 passed. Visual inspection then found return-art/text misalignment despite logical bounds passing. Centered its aspect-preserved image, shortened the label and added ornamental-clearance assertions: run3 5/5 passed. Run4 applies the user's background tint; final 3/3 and5/5 passed. Each of four owner rebuilds succeeded. No test tolerance was relaxed to accept the earlier visual defect.

The guide keeps inventory consumption unchanged; it does not retest every booster/obstacle combination. An older broad motion fixture still expects the historical `Hotspot_Boosters` page, absent since the earlier native Pre-Level implementation; it was not run or rewritten in this focused pass.

Chrome checks passed for all22portrait selections and3links, with no horizontal overflow at360px; its screenshot was inspected. All1,733final production hashes,49final evidence hashes and75priorR11evidence hashes verified; changed C# whitespace checks pass. Browser viewer/link and source/evidence checks are recorded in QA. No caches are included as production changes. CurrentTask.md and SessionLog.md preserve earlier acceptance decisions and record this work.

Not performed: APK/AAB, physical Android rendering/touch, full-project suite, profiling, prolonged play, exhaustive booster combinations, calibrated color measurement, fresh captures of other pages, or new asset-alpha acceptance (no assets changed). No new visual approval inferred.

Remaining separate audit refinements: localized explanations for non-gem objectives, broader small-text/goal-trim polish and tall-screen spacing. This pass resolves the misleading booster interaction and the newly requested scenery emphasis.
