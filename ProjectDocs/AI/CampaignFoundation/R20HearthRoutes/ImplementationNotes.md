# R20 household routes

Normal gameplay loads1–90 through the existing CampaignActive package. Only81–90 are new. The four active JSONs match `EditorContent/CampaignCandidates/Chapter01Sector02R20`; original active metas/GUIDs are preserved. Existing catalog, authoring window, GameState, save/routing and map behavior require no new runtime system or prefab edits.

DesignReview.md records the scenario/puzzle review. RoundBriefs.json describes budgets, targets and intent. Sources/AuthorAndInstall.py reproduces exact layouts/seeds9181–9190 from task-start snapshots, bilingual names/reactions and the after90 hearth-routes scene. It is a local checkpoint, not a new fire/timer/restoration service. Chapter1 remains incomplete;91/Chapter2 stay unavailable. First-clear budget1840gold/6gems/1Sea Pearl, no scene/replay bonus. All prior80 content remains intact.

Three existing test files change. ActiveCampaignIntegrationTests covers90×3 deterministic legal starts and no-inventory completion replays, previous80 preservation, active/candidate equality and RU/EN validation. It adds completed80 migration alongside10/40/50/60/70, retains all four inventory reward/replay checks, and exercises normal routes80/81/83/85/90 with courtyard/hearth dialogue. CampaignBoardIntegrationTests and LevelSystemRuntimePlayModeTests expect90 active rounds. Explicit legacy fixtures remain unchanged.

Normal UI tests invoke bound buttons and legal board commands, not physical pointer hit-area tests. Earlier progress is fixture data. Save/economy fixture completions are separate from actual replay witnesses. Solvers establish a legal completion, not human difficulty acceptance or population win rates.

Verification uses isolated storage and previously approved process-only Vulkan/descriptor4096. Runtime matching, art, animation/effects, project graphics and packages remain unchanged. Logs/XML/witnesses and source/preservation/scope evidence are retained in QA, with reproducible scripts in Sources. No Android/full-project-suite/new visual audit claim. Human31–90 remains outstanding; animation/effect work stays last.
