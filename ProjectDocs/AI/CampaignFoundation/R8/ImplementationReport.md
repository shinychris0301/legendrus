# Campaign Foundation R8 — story handoff

The opt-in campaign preview now offers eligible intro and milestone scenes through the approved Dialogue page. Victory → Continue commits result acknowledgement and next-round selection together, then offers pending dialogue before Pre-Level. Closing an unfinished scene reoffers it from its first line. Final Continue commits the scene acknowledgement before navigation.

## Implementation

- CampaignStoryFlow derives pending scenes from completed rounds and the acknowledgement ledger. It does not grant currency, spend energy, create attempts, or unlock chapters.
- Scoped `bridge-story:<chapter>:<scene>` and `campaign-story:<chapter>:<scene>` keys use the existing completedStoryScenes list. No save-schema change. The immutable legacy archive credits scenes only for the unchanged bridge package; it cannot suppress an expanded-campaign scene with the same ID.
- CampaignGameplayCoordinator has an optional story route and Story state; starting a round through this coordinator cannot bypass pending dialogue. Existing non-story integrations retain their contract.
- CampaignDevelopmentUI supplies campaign-owned story data to ProductionDialogueView, independent of live StoryService. Runtime delegates are assigned to the existing cloned Dialogue prefab before activation. Serialized fields, prefab assets and builder revision remain unchanged by this task.
- RU/EN use existing speaker and dialogue keys, checked across all eight bridge scenes. Locale switching retains the current line. Unused reaction/completion copy is not newly connected; its previously documented localization gaps are still outstanding.
- Acknowledging the bridge's round-ten finale does not acknowledge the expanded chapter completion or unlock Chapter 2. The expanded round-ten/round-500 mapping remains a separate content task.

## Verification

Unity 6000.5.9f1 Verify02: **121 passed, zero failed/skipped** — 109 campaign EditMode, 10 campaign board/UI PlayMode, and 2 normal win/fail journey PlayMode checks. Prior approved process-only Vulkan and descriptor limit4096, isolated test configuration; no project backend/settings changes.

New story coverage includes idempotent acknowledgement, invalid keys, journal faults at mutations 1–7 for both acknowledgement and result/selection, restart recovery, currency/energy/attempt preservation, all eight scene locales, and legacy/expanded scene-ID separation. UI checks cover intro, actual win to pending dialogue to next Pre-Level, fail/retry, partial-scene restart, RU/EN line switching, storage failure/reopen, and isolation from live StoryService. Existing disk-store campaign checks also passed; new story interruption tests use simulated durable stores and do not establish OS power-loss guarantees.

Verify01 stopped at compilation because the new test used the wrong helper method name. Corrected to Restart; the final suites passed. Actual RU/EN intro and post-victory captures inspected. Browser review at 430/1440 pixels: 16 image loads, no JavaScript errors or horizontal overflow. No physical Android run, full repository suite, or full-campaign balance claim.

## Scope and boundaries

Six existing sources/tests changed; two new C# files with their metas; no deleted files, and7439 task-start files unchanged under Assets/Packages/ProjectSettings. See [scope](QA/Scope.json) for exact paths. Existing dirty work preserved. Temporary test scenes/linker artifacts cleaned; no Git commit.

The editor menu **Legends of Rus → Development → Campaign UI Preview (In Memory)** remains an isolated preview. Its journal survives reopen within the preview, but leaving Play Mode discards that in-memory store. Production save ownership and activation are not part of this milestone. No new artwork, animation/effect refinement, reward tuning or level content was added.

## Next milestone

Author the first campaign sector in reviewable batches, beginning with settling the expanded round-ten beat and authoring rounds11–20 from R7 briefs under the adopted rules. Validate goals, deterministic starts, progression and difficulty before activation. Continue toward the first50 and chapter continuity, then product systems/device acceptance. **All further animation and gameplay-effect polish remains the final development step.**
