# Gameplay calmer reference R6 — implemented and verified; visual review pending

[Open the standalone Unity review](Gameplay_CalmerReference_R6_Review.html).

The active Gameplay now has a real **8×7 board**, six calmer emblem gems, a clean twilight village background, separate red moves/parchment objectives/wood score panels, and four wood-and-gold booster cards. Existing Ivan and booster sprites remain byte-for-byte intact. Labels and counters are live RU/EN text. The score meter reflects the existing score fraction and the stars retain actual threshold semantics. Tapping a card forwards to its original booster button; inventory, effect and pause restrictions remain the existing behavior.

The user's final font reply authorized matching the reference's Cyrillic serif appearance with the project font. Gameplay uses restrained cream regular serif text on dark panels and dark ink on parchment. No new font file was added or OS font distributed. Exact typeface identity and long-session/device comfort are not established.

## Staged missions and board data

Rounds **1–5 retain two missions**; **6–10 use three**. The added mission collects green gems: 10 in round 6, 8 in recovery round 7, then 12, 14 and 16. Existing mission targets, move allowances, rewards, seeds, teaching order and difficulty tags remain. These are initial authored targets, not a measured win-rate balance.

All ten authored levels now use 8×7. Fixed cells formerly outside the board moved: round 2 (6,7)→(6,6), round 4 (6,7)→(6,6), round 8 (0,8)→(0,6), round 10 (1,7)→(1,6). Their tile and special identities are retained. The runtime and fallback dimensions agree. New dimensions can change seeded arrangements and difficulty.

Validator and goal tracker now accept up to three objectives; the tracker no longer silently truncates the third. Win gating and snapshots include all slots. The board exposes indexed goal access while retaining its compatibility events. The tutorial's progress detection includes the third objective. Pre-Level now presents the same two/three missions and matching icons; other Pre-Level sections retain their existing design.

The six tile IDs retain matching semantics: 0 ruby teardrop; 1 blue crystal; 2 green emerald; 3 round gold; 4 purple teardrop; 5 orange sun hexagon (previously a shield drawing). No extra tile type or new booster effect was introduced.

## Artwork and ownership

Built-in imagegen produced eleven independent resources: six gems, four blank panels and one clean environment. No screenshot regions or full-screen UI image enter runtime. Resources are saved under `Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned/` with `_r6` names. [Prompts](GenerationPrompts.json), [environment prompt](EnvironmentPrompt.json), [blue-gem revision](SapphireRevision.json), raw `Sources/` and [asset provenance](GeneratedAssetProvenance.json) record the process.

The first blue gem was too narrow. The generated revision had a painted checkerboard; previously authorized local transparency cleanup removed only its connected exterior, retaining artwork RGB. Final blue black/white/magenta proofs were individually inspected. All ten isolated resources were also inspected through three transparency proof sheets (diagnostics, not page implementations). Panel writing surfaces remain opaque. `QA/PrepareArt.py` followed by `QA/CleanupSapphire.py` reproduces the sprite preparation. The environment is copied unchanged.

Gameplay prefab/view/owner agree on revision **6**; Pre-Level agrees on revision **2**. Existing serialized objects and references are upgraded in place. Added native components draw the score meter and forward full-card taps. Historical inactive HUD objects remain serialized to preserve IDs.

## Checks performed

- Two Unity 6000.5.9f1 verification runs: both Gameplay and Pre-Level owning rebuilds/readbacks exited 0. Initial captures exposed the narrow blue gem and faint score bar; the second run validates their corrections and full-card input.
- Final **EditMode 7/7 passed** (Gameplay and Pre-Level fixtures): canonical bindings/controls, repeated-build stability, source-sheet integrity, imports and unaffected-page revisions.
- Final **PlayMode 25/25 passed** across GameplayAlignment, PreLevelAlignment, LevelGoalRuntime and LevelSystemRuntime fixtures. Covers live authored objectives/inventory/moves, pause/input, localization/status, routing/rewards, all-three-goal completion gating and snapshot restoration. Seed test verifies 120 board starts (10 rounds × 12 seeds), legal moves, no starting matches and retained authored fixed pieces. This does not prove every level can be won within its move limit.
- 24 fresh final Unity PNG/JSON pairs: 15 Gameplay and 9 Pre-Level. Directly inspected nine final portraits: Gameplay RU/EN reference size, RU narrow/tall safe areas, round 6 RU, round 10 EN and tall EN status; Pre-Level round 10 RU and round 8 EN. Other captures were produced by passing tests, not all manually reviewed.
- Source audit passed: 20 existing files modified, 26 new production files (11 PNG/meta pairs and two C#/meta pairs), 1,629 baseline files unchanged, all 854 existing metadata files preserved and new GUIDs unique. All 304 original Gameplay IDs and all original Pre-Level IDs remain. Baseline comprises the R5 1,648-file manifest plus the initially clean level catalog (backed up from HEAD before audit). Final manifest contains 1,675 files.
- Protected Home, Splash, Loading, World Map, Chapter Map, frontend art, core board rules/VFX, packages, project settings and Android guards match the baseline. Pre-Level goal UI and goal-tracking services are intentional changes. [Incremental diff](AppliedSourceChanges.diff), [manifest](SourceChangeManifest.json), [audit](FinalSourceAudit.json) and archived commands/XML document exact scope.
- Browser review results are in `QA/StandaloneReviewBrowserChecks.json`: 25 selectable portraits, local links and a 360px layout. This verifies the viewer, not a fresh Unity render.

## Not performed / limitations

No APK/AAB build, physical-phone session, long-session comfort study, performance/memory profiling, full-project/pure-core regression suite, effects video, win-rate simulation or complete human playthrough. Current gems, scenery, ornament and font are independent adaptations, not pixel-identical copies. Existing booster medallions intentionally differ from the reference icons. Board-special artwork, detached match-identity markers and obstacle damage/VFX polish remain unchanged and are still candidates for the next visual pass.

## Existing changed files

- `Assets/LegendsOfRus/Resources/Config/chapter01_levels.json`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/PreLevel.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/Gameplay/LevelData.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/Gameplay/LevelGoalRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/LevelTutorialPresenter.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayArt.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayCell.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayHudStyle.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPreLevelRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPreLevelView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/GameplayAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/LevelGoalRuntimeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/PreLevelAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/GameplayAlignmentEditModeTests.cs`
