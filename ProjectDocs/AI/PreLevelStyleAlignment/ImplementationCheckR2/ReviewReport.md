# Pre-Level implementation check — 2026-09-14

**The native Pre-Level page is implemented; pre-game booster selection is still incomplete.** This is a read-only production audit, not a new visual revision or an approval request.

Project: `/home/k2/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL`. Recorded Unity: **6000.5.9f1 (b57deb96f08d)**. Active prefab: `Assets/LegendsOfRus/Resources/ProductionUI/Pages/PreLevel.prefab`, resource `ProductionUI/Pages/PreLevel`, native revision **2**. Its owner is `Step20ProductionUiFix01SerializedPrefabBuilder.RebuildPreLevelAlignment`; presentation/data are owned by `ProductionPreLevelRuntime`, `ProductionPreLevelLayout` and `ProductionPreLevelView`, with existing `ScreenRouter` navigation.

The R1 report and its captures are historical: production now supports three goals and uses the current Gameplay goal assets. The early Loading handoff/provenance does not describe this newer page. No historical art corrections were reapplied.

## What is present

- Live selected-level objectives, move allowance, booster stock and first-clear currency/booster rewards. Two objectives in rounds 1–5, three in rounds 6–10. Replayed claimed rewards display zero and a received status.
- Actual Gameplay collection gems and obstacle/feather/score/survival goal resources, separate navy panels and isolated booster/chest sprites. Accepted Chapter Map scenery/banner and shared Home HUD/Play resources remain in use.
- Native Back, Play, profile, settings, resource and four booster buttons. Play calls the existing energy gate; Back returns to Chapter Map.
- Russian/English text and responsive portrait layout. The saved tests verify safe-area containment, raycasts, panel separation, reward clearance and repeated layout.

The HUD's **12,540 is wallet gold**, read from `save.gold`, not Gameplay score. Reference example values such as 30/30 goals and 500/10 rewards must not replace authored level data. Level 1 correctly shows 12/12 goals, 28 moves and 120 gold/0 gems. The HUD profile level and selected round are different fields.

## Remaining issues, in priority order

1. **Booster selection is promised but not implemented.** `ProductionPreLevelView.BindNavigation` sends all four icons to the same BoosterSelect modal. `ScreenRouter` only wires that modal's Close and Play actions; there is no chosen loadout, selection feedback or pre-game consumption. Existing tests prove opening the modal preserves stock; they do not prove selection works. Recommended next functional action: reconcile this heading/action with the actual in-game booster model before adding any inventory-spending behavior.
2. **Non-gem objectives lack an explanation.** The level-8 portrait shows feather, chain and green gem counts but does not say what action completes them. `GoalDescription()` has localized action strings, yet rendering explicitly sets every description to an empty string. A compact description or accessible detail interaction would clarify these goals; this is a usability gap, not evidence of incorrect counts.
3. **Text and goal-panel padding need polish.** Small gold lettering and quantity badges have strong textured/shadowed edges in the saved portraits. Goal counts sit close to the bottom gold trim; the heading is similarly close to the upper trim. The shared typography asset has no explicit RegularFont or StartFont, so the existing development fallback remains a device-verification limitation. Exact reference font identity remains unknown; no fonts were changed or added.
4. **Tall-screen composition spreads sections apart.** At 576×1280, the moves medal floats between distant panels and the bottom wood strip is conspicuously empty. Controls remain visible in the inspected capture; this is a composition issue rather than a demonstrated clipping failure.
5. **Reference fidelity remains approximate.** The implementation has brighter sunset scenery, heavier polished gold frames, a different red banner, smaller reward presentation, different booster medallions and an added wood footer. The original is darker, with finer antique ornament and a more compact vertical composition. These resources reuse previously accepted pages, so wholesale replacement is not implied by this audit. Current gem silhouettes intentionally follow the later Gameplay decisions and should not be reverted to the old reference gems. Pre-Level circular booster medallions match this page's original concept; the later Gameplay rim-removal change did not update Pre-Level.

## Visual evidence inspected

These are **previously saved, actual Unity R11 verification captures**, not fresh captures from this audit. Each link is one complete portrait:

- [Current RU, 941×1672](../../GameplayStyleAlignment/ReadabilityR11/UnityVerification/PreLevel/PreLevel_941x1672_RU.png)
- [Current EN, 941×1672](../../GameplayStyleAlignment/ReadabilityR11/UnityVerification/PreLevel/PreLevel_941x1672_EN.png)
- [Level 8: three missions](../../GameplayStyleAlignment/ReadabilityR11/UnityVerification/PreLevel/PreLevel_Level08_RU.png)
- [576×1280, simulated safe area](../../GameplayStyleAlignment/ReadabilityR11/UnityVerification/PreLevel/PreLevel_576x1280_safearea_RU.png)
- [Original Pre-Level reference](../../../../Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/PreLevel.png)

## Checks and limits

Performed this audit: instructions/handoff/current-task/provenance/version and local-change inspection; active prefab/runtime/builder/test/routing inspection; original reference and four saved portraits directly viewed; all **1,733 production hashes** and **75 R11 evidence hashes** verified unchanged. Parsed the saved R11 XML: **3/3 Pre-Level EditMode and 4/4 Pre-Level PlayMode tests passed historically**, including all ten authored levels, inventory/replay, energy/double-start protection and navigation. These are not new test passes.

A fresh targeted EditMode run was attempted and exited **1 before compilation or test execution**, because another Unity Editor owns the project lock. The initial process-name search missed its `unityhub-unity-editor-6000.5.9f1` executable; process/lock inspection confirmed the live owner. The editor was not stopped and its lock was not removed. PlayMode was consequently not launched. [Exact verification](QA/Verification.json) and [attempt command](QA/AttemptedEditMode/command.json) retain the limitation.

Not performed: fresh Unity compilation/tests/captures, prefab rebuild, new artwork or alpha acceptance, new browser checks, exact pixel/color matching, full-project tests, profiling, full human playthrough, APK/AAB or physical-phone testing. Existing test coverage does not verify a pre-game loadout feature.

Changed only this audit report/QA records and the appended history entries in CurrentTask.md and SessionLog.md. Production, GUIDs, packages, signing settings and Android guards remain unchanged. Next focused work: resolve booster-selection semantics, then improve goal explanations/readability and tall-screen spacing; rerun focused Unity checks when the Editor is available.
