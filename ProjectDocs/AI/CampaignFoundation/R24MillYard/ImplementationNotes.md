# R24 mill-yard integration

Authored111–120 from the R22 sector outline and R23's recorded next action. `Chapter01Sector03R24` contains the exact four JSONs installed in the existing `CampaignActive` resource owner. First110 levels and every prior story/localization entry remain unchanged. Active resource GUIDs and campaign definition are preserved.

New content uses seeds12111–12120 and existing PrototypeR4 rules, gems, items and obstacles. Recovery111/116, layered central shelves113, uneven triple delivery114, strong edge delivery gates115, open special-making space117, consumed-move survival118, lower layered locks119 and the mixed yard checkpoint120. All brief details are in RoundBriefs.json; design rationale is in DesignReview.md. No generated artwork, animations or runtime service changes.

The after120 `campaign.chapter01.mill_yard` scene returns to ChapterMap and introduces the road to the supply storehouse. Bilingual names and twenty distinct goal-aware reactions are included. No production/minigame mechanic, restored Firebird, chapter finale, reward for watching dialogue or reward on replay.121 and Chapter2 remain unavailable; planned finale remains500.

First-clear111–120:1840gold,6gems,1SeaPearl. Difficulty labels/move budgets are authored hypotheses. Solver evidence shows possible legal completions, not human acceptance or population win rates. Human31–120 and Android remain open. All effect/animation work stays last.

Four existing test files extend catalog bounds and checks: ActiveCampaignIntegrationTests, CampaignBoardIntegrationTests, CampaignCheckpointLifecycleTests and LevelSystemRuntimePlayModeTests. Active tests cover three seeded starts/replays per round; save migration from completed10/40/50/60/70/80/90/100/110; real board input through map→PreLevel→Gameplay→Victory→Dialogue/map for110/111/115/118/120; reward/reload protection. Checkpoint tests cover50–120,19 ordered pending milestones and the121/Chapter2 boundary. Earlier completion history in these tests is an explicit fixture.

Sources includes task-start hashes/content/test snapshots and TestChanges.patch. Reproduce authoring with PrepareBriefs.py then AuthorAndInstall.py. Verify.py runs serial Unity suites against isolated save stores using previously approved process-only Vulkan/descriptor4096. CheckContent/AuditContent/SummarizeWitnesses/Summarize/CleanAndAudit retain content, replay, runtime and scope evidence. See TestResults.md for measured outcomes; source checks alone are not a Unity pass.

Next: bounded121–130 supply-store approach, with recovery121/126, layered lower gates and scattered surface obstacles, preserving1–120. Do not bulk-fill the sector, infer human acceptance or tighten budgets solely from solver lengths. Optional dialogue recap and older repeated reactions remain known UX/editorial work.
