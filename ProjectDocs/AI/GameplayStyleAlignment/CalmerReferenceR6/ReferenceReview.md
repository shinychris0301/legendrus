# Gameplay calmer reference R6 — reference and implementation-scope review

2026-09-14. The user rejects the current R5 visual direction and supplies a new reference, requests 8×8, the shown gem family and top/bottom composition, support for two or three missions, and preservation of the existing character and boosters. The user also asks for an opinion. This entry records the assessment; no R6 artwork or implementation exists yet. R5 remains installed, not visually accepted. Latest reference supersedes the earlier Gameplay composition target.

## Visual direction

The uploaded reference uses flatter, emblem-bearing gems with broad facets and localized edge highlights. This reduces repeated bright texture detail compared with R5's highly fractured, luminous gems. Retain distinct silhouettes and color identities: red teardrop, blue elongated crystal, green square, gold round token, purple teardrop and orange hexagon. Reserve pronounced glow for selected/special pieces and brief feedback. This is a visual-design judgment, not a medical claim or a measured long-session comfort result.

Top: existing Ivan portrait on the left with a nameplate below; separate red/gold moves panel; broad parchment mission panel; dark wood score panel with progress and stars on the right; selected-level plaque centered below. All wording, counts and progress must remain live and localizable. Reference name, hero badge 32, level 123 and goal counts 30 are examples, not production data. Verify a real hero-level data binding before exposing such a badge; never substitute selected-level data for hero progression silently.

Bottom: four carved wooden gold-framed cards, stock badges and readable label strips; settings at lower right. Keep current character and booster sprite bytes and real effects. The reference's shield/shuffle labels do not authorize silently relabeling Living Light/Sea Pearl or changing their effects. Match the new card containers around the retained booster designs with accurate existing names. This intentional exception prevents an exact-pixel-identity claim.

The attachment visibly has eight columns and seven rows. The explicit requested target is eight by eight. Add the eighth row and rebalance the open scenery spacing while keeping square cells and readable header/footer. Exact vertical coordinates cannot simultaneously remain identical to the pictured 8×7 layout.

## Verified project implications

- Project root: `/home/k2/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL`; recorded Unity 6000.5.9f1, b57deb96f08d.
- Active prefab: `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab`. Owner: `Step20ProductionUiFix01SerializedPrefabBuilder`; composition/style/view: `ProductionGameplayRuntime`, `ProductionGameplayHudStyle`, `ProductionGameplayView`, responsive geometry: `ProductionGameplayLayout`.
- Board mounts are currently authored as 786×942. Real board dimensions come from the level catalog through `LevelBoardFactory`; changing just visible rows would be incorrect.
- All ten chapter levels currently have two goals. Dimensions: L1 8×9, L2 7×8, L3 8×8, L4 8×9, L5 7×9, L6 8×9, L7 8×8, L8 8×9, L9 8×9, L10 8×9. L8 has a fixed cell at (0,8), outside the target boundary. It must be deliberately repositioned with its tutorial/special purpose retained, followed by legal-move and authored-layout validation. New dimensions can change seeded boards and difficulty even when other cells remain in range.
- `LevelDataValidator` rejects more than two goals; `LevelGoalRuntime` truncates to two; `Match3Board` exposes two slots/events; Gameplay prefab/view requires two slots. Third-mission support must extend validation, tracking, binding, win checks and related Pre-Level presentation/tests coherently. Center two goals; use three evenly spaced columns for three, without a decorative fake third objective or invented missions.
- Current six tile indices map to ruby, sapphire, emerald, amber, amethyst and shield. The new six appearances need a documented one-to-one mapping and consistent goal/tutorial presentation; retain tile identities and rules. The new orange/gold pair must stay easy to distinguish.
- Existing Ivan portrait comes from Home avatar resources. Existing booster order is Bogatyr Mace, Firebird, Living Light, Sea Pearl. Their exact current sprite resources and transactional behavior are preservation targets.
- New reference is visible in the conversation. It has not been saved as a verified local source or registered with a file hash. The historical REFERENCE_PROVENANCE.json covers older Splash/Loading resources, not this attachment. No new isolated gem or panel resources were verified in this review. Recreate independent resources; do not crop screenshot regions or use the screen image as runtime UI.

## Checks performed / not performed

Read root instructions, historical handoff, current task entries, provenance, git status and Unity version; searched relevant owners, layout, art mapping, goal runtime/validation and board factory. Directly compared the uploaded image with the retained R5 941×1672 RU Unity capture. Parsed all ten runtime level definitions, checked fixedCells/obstacles/boardItems against 8×8 bounds, and checked all 1,648 R5 production hashes before and after this documentation-only review: unchanged. Root AGENTS.md was the only project AGENTS.md located by the scoped search.

No production edits, asset generation, local image processing, Unity launch/compilation, new tests, captures, APK build, device session or comfort study. Old passing R5 tests do not verify R6 or three-goal behavior. Only this report, preservation evidence, CurrentTask.md and SessionLog.md changed.

Next focused implementation: establish independent six-gem and blank-panel resources from the new visual target, then update the real 8×8 board/data and adaptive 2/3-goal contract together with the new header/footer. Preserve existing Ivan/booster art, use actual data, and verify seeded legality, all-three-goal completion, RU/EN text and portrait/safe-area captures. The broader R5 VFX/obstacle follow-up is subordinate to this new composition request.
