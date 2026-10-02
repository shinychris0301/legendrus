# R18 far-street block

The existing normal-game campaign now loads rounds1–70. Only61–70 are new. `Resources/CampaignActive/{levels,story,story_ru,story_en}.json` match the new `EditorContent/CampaignCandidates/Chapter01Sector02R18` snapshot. Existing resource metas/GUIDs are retained. The runtime loader, authoring window, GameState, save service, router and map already read this package; no parallel services or prefab edits were needed.

DesignReview.md records the scenario and puzzle review. RoundBriefs.json records targets, budgets and intent. Sources/AuthorAndInstall.py creates the exact boards from task-start snapshots, seeds7161–7170, and adds the bilingual far-street milestone after70. Previous story, locale entries and levels remain intact. New rewards total1840gold/6gems/1Living Light. No scene reward or chapter completion;71 and Chapter2 stay unavailable.

Test changes:

- ActiveCampaignIntegrationTests validates active/candidate equality, first60 preservation, bilingual story,70rounds ×3 legal deterministic starts and no-inventory completion replays. Completed60 migration is added alongside10/40/50. Reward/replay checks now include Living Light, completing coverage of all four inventory types. Normal route checks cover60/61/65/67/70 and both well/far-street scenes.
- CampaignBoardIntegrationTests and LevelSystemRuntimePlayModeTests expect70 active levels; historical legacy fixtures remain explicit.

Normal route tests invoke bound buttons and board tap/swipe commands, not physical pointer hit areas. Earlier progress is fixture data. Fixture completions in migration tests validate economy/progression separately from actual board replay witnesses. Automated completion is not human difficulty acceptance.

Verification uses isolated storage and previously authorized process-only Vulkan/descriptor4096. Project graphics settings, packages, approved art, animations and effects are unchanged. No Android/full-suite/visual acceptance claim. Sources contain reproducible verification, content-preservation and cleanup scripts; QA retains logs, XML and replay witnesses. Animation/effect work remains the last phase.
