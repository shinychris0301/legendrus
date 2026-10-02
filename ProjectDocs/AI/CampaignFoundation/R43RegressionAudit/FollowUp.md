# Remaining runtime audit work

Keep the failed cases visible. Source triage is not a passing rerun. Do not restore deprecated artwork, obstacle numbers, screen hotspots or a ten-round finale just to satisfy historical tests. No campaign expansion or effects redesign is part of this follow-up.

1. Fix the 23 standalone board/input/motion fixture failures first: they initialize from the current 8×7 catalog but load 8×9 BoardQAPresetFactory snapshots. Bind the disposable test board to explicit preset dimensions and review its intended rule version. Keep current production rules and verify the assertions after setup succeeds.
2. Separate the five old CoreLoopVfxBoosterFeedback checks from the approved current effects path. Existing mechanics tests passed in the broad run. Explicitly test legacy compatibility where intended; do not reintroduce removed rings or duplicate VFX pools.
3. Update the current-content UI checks: replaced obstacle-stage digits and special sprites; active milestones at round 10; no premature chapter completion; 200-round/600-star map; select/unlock fixtures that cover all rounds; native map controls instead of invisible hotspots. Keep actual runtime assertions, not relaxed counts.
4. ChapterComplete save equality includes lastEnergyUtcTicks, which advances independently. Compare the gameplay-owned fields while retaining energy amount and reward/idempotency assertions.
5. Historical candidate tests: Granary/Riverbank completion messages correctly name their last recorded round, while tests still demand the next unavailable round number. The old sector test compares the active StoryCatalog to a ten-round finale; explicitly load the historical fixture for that assertion.
6. Repeat affected fixtures, then a combined sweep to verify ordering. The R43 focused run confirms the corrected balance fixture preserves ScreenRouter and GameState; a complete post-fix combined Play Mode sweep has not yet been run.

See QA/RuntimeFailureTriage.json and QA/LatestRuntimeStatus.json for exact cases and evidence. Human difficulty, Android device/build verification, broad visual acceptance and optional returning-player recap remain separate outstanding work. Animations/effects stay last.
