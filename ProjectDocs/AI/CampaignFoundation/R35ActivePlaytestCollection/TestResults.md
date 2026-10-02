# Campaign R35 — installed campaign playtest collection

**48 distinct Unity tests passed; 0 failed; 0 skipped in the final run.** The current 1–200 campaign is now selectable in twenty recorded ten-round blocks through an Editor-only menu. Normal campaign content and difficulty are unchanged.

[Verification summary](QA/Verification.json) · [Recording evidence](QA/RecordingEvidence.json) · [Implementation](ImplementationNotes.md) · [Playtest instructions](PlaytestInstructions.md).

## Executed checks

Unity **6000.5.9f1**, Verify03, isolated test save stores, previously authorized process-only Vulkan and descriptor limit 4096. Project backend/settings unchanged.

| Suite | Passed | Coverage |
|---|---:|---|
| Active collection | 28 | All 20 installed block fixtures; exact content and reopen parity; 30 energy/zero starting boosters; no setup runs in recordings; invalid/misaligned/unavailable ranges and malformed content rejection; complete content fingerprint; current matching rules/art; recording on close; actual final-round wins at 40 and 200 with correct replay |
| Existing recorder/storage | 16 | Outcome and rating persistence, separate retries/interruptions, paused timing, local write failure/retry, source classification, storage recovery and existing recording contracts |
| Existing completion/tuning | 4 | Historical replay witnesses and unchanged tuning; final-round story/completion/replay; failed-terminal/non-recorded behavior; save-before-exit and write retry |

[Active XML](QA/Verify03/ActiveCollection/results.xml) · [Recorder XML](QA/Verify03/Recorder/results.xml) · [Completion XML](QA/Verify03/Tuning/results.xml).

## Confirmed behavior

- **Twenty block starts (1, 11, …, 191)** resolve the installed CampaignActive data and reopen their privately owned in-memory journal correctly. Prior progress is an explicit fixture. Fixture setup produces no measured starts/wins or earned rewards; normal player progress, balances, inventory and preferences remain unchanged.
- **Round 31** starts with current gameplay rules and current board artwork. Closing its preview persists one interrupted attempt, with its rating and block metadata.
- **Rounds 40 and 200** were completed through actual board commands using retained deterministic witnesses. Victory and the milestone dialogue lead to block completion. No following round starts automatically; no false chapter finale is acknowledged.
- **Replay explicitly selects the block's final round.** At round 40, result acknowledgement can select installed round 41. The replay correction returns selection to 40 before Play. Selection itself spends nothing; the replay start is a separate paid attempt. Its recording survives closure as interrupted.
- Exports include the chosen range, fixture boundary/context and SHA-256 content fingerprint covering **definition + levels + story + RU + EN**. Manual and automated sources remain distinct. The two saved terminal examples are clearly `automated_campaign`, each containing a win and an interrupted replay. Earlier progress inside those test blocks was service-level setup, not board-play or human evidence.
- Malformed installed content fails explicitly, without loading an old candidate. Historical R9/R12/R13/R14 menus remain available.

The first new-test compile missed a namespace import; corrected. The next run's eight failures were teardown assertions against GameState's independent full-energy clock timestamp, not balance/progress mutations. The harness now excludes only that timestamp, still checks all other player fields, and always cleans up before asserting. Full run history is retained in Verification.json and QA/Verify01–03.

## How to access the verified route

At **Home in Play Mode**, open **Legends of Rus → Development → Campaign Active Playtest (Recorded)**. Choose **31–40** for the first current-content pilot, followed by a separate **41–50** session. All installed ten-round blocks are available. The monitor shows the range and fingerprint; use Save/Show recording after rating the outcome. [Detailed instructions](PlaytestInstructions.md).

## Scope and remaining verification

**5 existing source files modified, 4 files added including two .meta files, 0 deleted; 7,653 existing project files unchanged.** Added an Editor-only fixture helper/test suite; updated the existing menu/monitor, optional recorder metadata and development completion/replay behavior. No production content, rules, economy, artwork, page prefabs or project settings changed. [Exact patch](Sources/Changes.patch) · [Scope](QA/Scope.json) · [Source checks](QA/SourceChecks.json).

Python syntax/scoped whitespace checks passed. All **3,044 protected files** checked across production UI, active campaign, GameplayCore, Packages and ProjectSettings remain unchanged. Test-generated scene/linker cleanup completed. No commit.

**Not established:** human difficulty for 31–200, a current Android build/device pass, full-project test-suite pass, fresh visual/pointer acceptance or a manual Stop Play Mode session. The automated destruction/disable tests verify the recording callbacks and durable files; they are not a human interaction recording. The 20 fixture checks are not 200 new board replays. Gameplay effects remain deferred.

**Next:** review new manual exports when available. In parallel, the R34 bilingual reaction draft for 51–60 is ready for a bounded editorial implementation pass, preserving accepted early scenes and all level data. Returning-player recap remains a design proposal, not implemented. Keep campaign content at 200 until these usability/evidence gaps are addressed.
