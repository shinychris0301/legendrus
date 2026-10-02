# R17 well routes

Continues the R16 normal-game integration with the first ten rounds of sector2. Existing ActiveCampaignContent, ChapterLevelCatalog, StoryCatalog, GameState, map, save migration and reward claims load the updated content without runtime code or prefab changes.

- `Resources/CampaignActive/{levels,story,story_ru,story_en}.json`: now1–60; original GUIDs/metas retained.
- `EditorContent/CampaignCandidates/Chapter01Sector02R17/`: exact reproducible content snapshot, four JSONs and Unity metas.
- `Tests/CampaignPlayMode/ActiveCampaignIntegrationTests.cs`:60 rounds ×3 deterministic starts and replay witnesses; previous50 preservation; active/candidate and bilingual validation; completed10/40/50 save migration; reward/replay checks including Firebird; normal routes50/51/55/57/60 and new well milestone.
- `Tests/CampaignPlayMode/CampaignBoardIntegrationTests.cs` and `Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs`: active-catalog count60; existing regressions retained.

No changes to matching/damage rules, active artwork, animations, VFX, startup, services, packages or rendering settings. The new obstacle arrangements reuse the same mechanics and sprites. Chapter1 stays incomplete;61 and Chapter2 stay unavailable. 61–100 in SectorPlan.md are outlines, not playable content.

`Sources/AuthorAndInstall.py` rebuilds the candidate and active JSONs from task-start snapshots. `CheckContent.py` checks preservation and structural constraints. `Verify.py` runs existing Unity suites in isolated storage with the previously approved process-only Vulkan and descriptor4096 limit. `CleanAndAudit.py` removes only task-generated test scenes/linker changes and records the scope relative to BeforeHashes.json.

Normal route tests invoke existing buttons and legal board taps/swaps; they are runtime integration checks, not new pointer hit-area/visual acceptance tests. Earlier progress used to reach later rounds is explicitly a fixture. Save tests perform fixture completions to verify progression and economy, separately from per-round command replay witnesses. Solvers demonstrate a legal solution within budget; they do not establish human difficulty or population win rates. Human31–60 and Android verification remain outstanding. Animation/effect refinement stays last.
