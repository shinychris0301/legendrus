# Campaign Foundation R4 — round lifecycle transactions

Implemented a development-only lifecycle boundary on the existing `CampaignSaveSession`. It persists round entry, retries, results and presentation acknowledgement through the same journal as selection and progress. No live router, board, GameState, prefab or save provider is replaced.

## Behavior

- **Start:** require an available, unlocked selected round, calculate energy using the existing 30-minute refill rule, charge one energy, increment the attempt count and pin a snapshot of the validated level. Commit these changes together before returning success.
- **Retry:** require the same round and a prior running or failed attempt. Charge and create the next attempt atomically. Restarting an unfinished attempt records its abandonment as a failure; retrying an already failed attempt does not count the failure again. A failed energy check leaves the prior run untouched. Retry counters follow the existing game convention of attempts after the first, including replays.
- **Result:** match the current run sequence. Successful results require all goal targets to be met; stars and rewards come from the pinned level, not caller-supplied reward amounts. Save score, completion/failure counts, stars, first-win claims, currency and booster deltas together. Replayed levels can improve records but cannot repeat first-win rewards.
- **Acknowledge:** mark a persisted terminal receipt as presented. This operation grants nothing. A normal subsequent start requires acknowledgement; the retry path can directly dismiss a failure.
- **Duplicate/stale input:** an exact duplicate start or result is a no-op. Conflicting results and callbacks from older runs are rejected. The caller must retain the expected sequence for delivery retries; a new run consumes the next sequence.

Checked arithmetic prevents a counter or balance overflow from committing part of a result. Caller mutation of goal arrays does not mutate the stored receipt. A disappeared package does not erase a run or prevent finishing against its pinned snapshot. A new start still requires installed validated content. Selection changes are blocked during a running attempt.

The session is a synchronous single-writer Unity-side boundary. Result facts are expected from the trusted local board controller; goal checks are not a server-authoritative or replay-based anti-cheat mechanism. Only the latest receipt is retained, alongside cumulative level records. Interrupted boards are not serialized here: a future integration must explicitly abandon/retry or implement board-state recovery rather than imply mid-round resume.

## Save upgrade and failure handling

Version 8 adds a persistent run sequence and receipt. `CampaignPlayerSaveV7` preserves R3's exact old field shape. The explicit migration entry point accepts canonical v7, validates it through the new campaign semantics and commits v8 without changing player progress/economy. Direct v6 copy migration now targets v8. R2 rehearsal candidates remain invalid campaign player data. Older version-7 readers block downgrade writes to v8.

Development journal keys remain `lor.campaign.dev.primary`, `.backup` and `.staging`. No live save is discovered or migrated. Both v7 and v8 retain bounded compression and the original archived v6 provenance. Existing development saves require `MigrateLegacy()` before lifecycle access; accepted current v8 saves make this operation a no-op.

If a primary flush or readback cannot be confirmed, the session latches an uncertain-storage state and refuses further reads/actions. This prevents a volatile in-memory write from being treated as durable success on a second call. Recovery requires a new session over a restarted/reloaded durable store. Primary commits that succeeded before a later staging cleanup failure remain committed. Caller code must use `Committed`, not only the journal cleanup status.

## Verification

Unity 6000.5.9f1 verification commands, logs and XML are retained under `QA/Verify01`. New tests cover first-win/replay rewards, failure and paused restart, duplicate/conflicting/stale callbacks, insufficient energy, refill and backward clock handling, checked overflow, missing content, malformed receipt data, and v7 upgrade/downgrade protection.

The five lifecycle write tests each inject all eight journal mutations: 40 interruption scenarios for start, win, failure, retry and acknowledgement. Each discards unflushed data, reopens a fresh session, retries the same command and confirms repeated delivery does not charge or grant twice. V7 migration is also interrupted at all eight points. These are deterministic memory-store crash simulations, not physical-device power-loss tests.

The selected EditMode suites include all prior campaign/pipeline tests. Two existing native PlayMode journeys exercise the unchanged legacy win and fail/retry flow. They are regression checks, not evidence that the campaign lifecycle is wired into the active board. No whole-project suite, Android/IL2CPP campaign execution, backend/provider, or new authored campaign content is claimed.

## Files

All source paths below are under `Assets/LegendsOfRus`:

- New `Scripts/Runtime/Services/CampaignRoundLifecycle.cs` plus metadata.
- Modified `Scripts/Runtime/Services/CampaignPlayerSave.cs` for v8 and strict v7 upgrade.
- Modified `Scripts/Runtime/Services/CampaignSaveSession.cs` for upgrade, active-selection gating and uncertain-storage latch.
- New `Tests/CampaignAdapterEditMode/CampaignRoundLifecycleTests.cs` plus metadata.
- Modified `Tests/CampaignAdapterEditMode/CampaignSaveSessionTests.cs` to retain R3 coverage against v8 and share fixtures.

Additional documents/scripts/QA evidence are under this R4 directory. `QA/Scope.json` records the exact before/after asset/settings hashes. Approved visuals and live services remain unchanged.

## Next step

Campaign Foundation R5: connect these transactions to an explicit development gameplay coordinator, so board construction follows a committed start and result navigation follows a committed finish. Cover navigation failure, duplicate callbacks and application restart with an unfinished run. Keep this composition isolated until concrete story mapping is reviewed: preserve the accepted ten-level opening arc and author a distinct round-500 finale. The first 50-round sector still requires narrative, difficulty and reward authoring; synthetic tests are not that content.

## Final results

Verify01 passed **195/195 EditMode** (16 R4 cases, including looped interruption scenarios, plus 179 previous cases) and **2/2 native PlayMode journeys**, zero failures/skips. Retained 36 actual Unity captures and inspected English Pause. Browser review disclosures, containment at 430/1440 widths and four local links passed without JavaScript errors. Only comment wording changed after the Unity run.

Before/after audit: 7,241 existing Assets/Packages/ProjectSettings files unchanged, three expected files modified, four new source/test/metadata files, no deletions. Restored Addressables link.xml/meta from exact pre-run backups after automatic Editor cleanup.
