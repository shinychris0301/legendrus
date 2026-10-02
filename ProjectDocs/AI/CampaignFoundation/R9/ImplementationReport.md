# Campaign Foundation R9 — playable rounds11–20 candidate

Authored ten new boards and connected the first village sector's story through round20. The expanded candidate replaces round10's chapter-ending story with a local hearth recovery, adds the missing apple-blossom beat after2, and introduces the well-to-granary handoff after20. Existing legacy content remains unchanged.

[Actual Unity review](Campaign_Foundation_R9_Review.html) · [Story/content decisions and playtest](StoryAndContent.md) · [Verification](QA/Verification.json) · [Screening results](QA/BalanceScreening.json).

## Implemented assets and entry points

Four explicit candidate JSON assets live in `Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector01/`: levels, story, RU and EN localization. They are loaded only by the explicit editor menu/test paths, not a Resources scan or production activation. ContentRevision remains the existing campaign contract revision1; R9 is an authoring candidate, not a published revision. SHA256 fingerprints are retained in [CandidateHashes.json](QA/CandidateHashes.json).

The existing CampaignDevelopmentPreviewMenu now offers both a fresh1–20 candidate and a clearly labeled **Start at11 (Test Progress)** shortcut. The latter seeds disposable claimed-reward/completion fixtures for1–10, acknowledges prior scenes and opens the new hearth scene before11. Both reuse CampaignDevelopmentUI, the approved prefabs and current PrototypeR4 rule/art implementation. Their in-memory progress is discarded on leaving Play Mode. No native production save is imported or modified.

New boards have explicit obstacle placements, layers, deterministic seeds, goal quotas, move limits and first-clear rewards. Two optional authored swaps reliably create a line item and Burst under the current rules. All R7 budget values are retained as provisional; no new mechanics, inventory requirements, artwork or VFX are introduced. Levels1–10 retain exact gameplay fields/rewards; only candidate story triggers and round10 presentation labels differ. The original Resources level/story/localization files are untouched.

The partial candidate has no chapter completion or round500 finale. Round21 and Chapter2 remain unavailable. Expanded scene IDs remain separate from legacy acknowledgements; production migration/recap decisions are not activated. Reactions and round-name keys are localized data; they are not claimed as a newly implemented result overlay or name-display feature.

## Verification and findings

Unity6000.5.9f1, using the previously approved process-only Vulkan workaround and descriptor limit4096 with isolated test configs. Project backend/settings/packages unchanged.

- Final candidate Verify05:14/14 PlayMode checks passed. Includes all20 boards ×3attempts for deterministic legal starts without initial matches; line/Burst opportunities across3seeds; ten round tests each producing3inventory-free completion witnesses; legacy field preservation; exact round10 result acknowledgement → selected11 → pending hearth; duplicate reward protection and content boundaries; actual UI rendering and round11 play-to-victory/Continue-to12.
- Regression Verify04:109/109 campaign EditMode and10/10 campaign board/UI PlayMode checks passed. **133 distinct checks passed, zero failed/skipped in their final runs.** This is not a full repository test run.
- All30 completion witnesses replay with their expected plan hashes and final board hashes. Search uses width8 and a14000-action cap; no inventory, no state changes outside the resolver. Actual animated round11 execution also matches the witness and spends exactly one energy before selecting12.
- Search solutions range2–8moves against27–32move budgets. This is a reachability screen using full deterministic information, not evidence of player win rate or accepted difficulty. Human review of generous budgets is still needed.
- Actual round15/20 board and RU/EN story captures inspected; later-round/story views use declared test progress. The unchanged Dialogue page currently has no dedicated elder portrait, so the elder scene uses scenery and localized speaker text. No substitute portrait was generated.
- Browser430/1440:18image loads across nine capture options; zero JavaScript errors or horizontal overflow. Mobile review inspected. Source diff whitespace check passed. No Android/device or full-campaign balance test.

Earlier runs are retained: Verify01 compilation failed on incorrect test API member names; fixed. Verify02 passed11/14: neighboring random gems could alter the intended line creation, and two tests incorrectly tried to import later rounds through the ten-level legacy schema. Added authored neighboring guards and changed fixtures to use campaign transactions after10. Verify03 passed14/14; then strengthened the round10 handoff test and added the explicit jump menu, with final Verify05 passing. No production validators were weakened to accommodate test data.

## Change scope and remaining work

[Scope.json](QA/Scope.json): one changed existing file (CampaignDevelopmentPreviewMenu.cs),13added files (four JSON assets, test source and metas/folder metas), zero deletions,7448task-start files unchanged under Assets/Packages/ProjectSettings. Task-owned test scene/linker artifacts cleaned; existing local work preserved. No prefab/builder revision changes are needed because the existing runtime composition and serialized bindings are unchanged. No Git commit.

Next: use the jump menu and the [playtest instructions](StoryAndContent.md) to assess11–20 without inventory. Collect outcomes and mechanic comprehension before tuning difficulty or authoring21–30/granary. Production activation, returning-player story migration, the remaining first50, chapter continuity and Android acceptance remain outstanding. **All animation/gameplay-effect refinement stays deferred to the final step.**
