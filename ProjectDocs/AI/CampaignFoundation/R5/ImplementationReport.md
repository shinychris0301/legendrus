# Campaign Foundation R5 — development gameplay coordinator

The development campaign now has a coordinator connected to the actual `Match3Board` through an explicit host. Board creation follows a durable R4 start transaction; its completion callback supplies real score and objective counts to R4 result persistence. Result presentation is attempted only after that transaction commits.

## Implemented behavior

`CampaignGameplayCoordinator` owns a supplied session, board host and routing interface. It has no global instance, startup hook, storage provider or scene creation. `Recover()` reads the saved receipt and presents the map, pending result or interrupted-round choice. It never replays an unfinished board automatically.

A fresh start commits energy/attempt state before creating the board. Duplicate start delivery does not construct another board or reset the current one. Board creation failure leaves the committed attempt recoverable. Explicit retry is a new paid attempt and records unfinished-attempt abandonment according to R4. Insufficient energy preserves the existing board/run.

Callbacks carry a coordinator generation and run sequence; stale, duplicate and disposed callbacks cannot grant rewards or navigate again. Synchronous construction callbacks are deferred until construction completes. The coordinator snapshots result arguments before use. The completed board is disabled/disposed even if result storage fails; an uncertain session cannot keep playing or pretend a volatile result is committed.

Routing failures or exceptions leave durable results intact. `Recover()` retries presentation without granting rewards. Result acknowledgement commits before returning to the map; if that navigation fails, a later recovery opens the map from the already-acknowledged state.

On restart with a running receipt, the routing interface receives an interrupted-round choice. Explicit abandonment saves a failure with zero recovered score/objective counts, preserving the spent energy. Explicit retry creates a new attempt. No board-cell state or partial result is invented. This is not mid-round resume.

## Real-board integration and isolation

`CampaignMatch3BoardHost` uses an existing caller-supplied RectTransform, creates only its owned board child, and disables it immediately on stop before destroying it. It creates no Canvas, EventSystem or parallel startup system. It passes the attempt already committed by R4 into the new explicit `Match3Board.InitializeCampaign` entry point.

The legacy Initialize overloads retain their existing GameState registration. Campaign initialization bypasses that registration, preventing duplicate attempt counts and writes to the live save. Board seeds continue to use the same level/attempt formula. Goals, special gems, resolution, animations and win/fail callbacks use the actual existing board implementation.

Inventory boosters are deliberately unavailable through the campaign entry until consumption/refunds use campaign transactions. Campaign calls cannot consume the separate live GameState inventory, including the watchdog test helper. The legacy game booster path is unchanged. This limitation is exposed as a command rejection, not a working booster claim.

Routing remains an injected interface: tests supply a recording route sink. This milestone does not install the coordinator into ScreenRouter or present a new native campaign HUD/result page. Production-style visual routing, campaign booster consumption, story mapping and activation remain separate work.

## Verification

EditMode tests cover transaction ordering, duplicate starts, board creation failure/exception, route failure/exception, acknowledgement followed by navigation failure, reopened pending/running receipts, explicit abandonment, stale/disposed callbacks, invalid results, synchronous callbacks, empty energy, and unconfirmed start/result storage. Fault tests inject each of the six mutations up to and including the primary flush for both start and result, then reopen durable state. R4 retains all eight-boundary lifecycle coverage.

Two new PlayMode tests construct the real board in a test-owned container and execute legal swipes selected by a read-only planner. They wait for real board completion/failure callbacks, inspect saved campaign records and route receipts, and verify energy is charged once. The failed run retries with a new committed attempt. Both check duplicate entry preserves the same board/hash, pause/resume works, campaign inventory boosters are rejected, and the separate live GameState snapshot remains unchanged.

The verifier also runs the existing native win and fail/retry journeys as regressions. A dedicated PlayMode test assembly adds no production dependencies. All stores used by the new campaign tests contain synthetic player state; no actual player save is imported. The runner isolates its configuration directory.

Commands, XML and logs are under `QA/Verify01`; final aggregate results are in `QA/Verification.json`. Native regression screenshots, if produced, show the existing native flow, not the campaign routing interface. CampaignWin/CampaignFailRetry JSON are test-state receipts, not user saves or UI captures. No whole-project suite, Android/IL2CPP campaign execution, physical-device power-loss, new authored levels, backend activation or live cutover is claimed.

## Change scope

- Modified `Scripts/Runtime/Gameplay/Match3Board.cs`: explicit committed-attempt initialization and campaign inventory isolation.
- Added `Scripts/Runtime/Gameplay/CampaignMatch3BoardHost.cs` and metadata.
- Added `Scripts/Runtime/Services/CampaignGameplayCoordinator.cs` and metadata.
- Added `Tests/CampaignAdapterEditMode/CampaignCoordinatorTests.cs` and metadata.
- Added `Tests/CampaignPlayMode/` with a dedicated test assembly and real-board integration tests, including metadata.

These paths are under `Assets/LegendsOfRus`. No new save schema or R4 transaction change is required. Existing art, page prefabs, resource catalogs, startup, activation and Android settings remain outside scope. The before/after hash report is `QA/Scope.json`.

## Next step

R6: provide a deliberate development UI composition for the coordinator using existing gameplay/result presentation, including an honest interrupted-round choice and retryable save/navigation errors. Add campaign inventory transactions before enabling its booster controls. Keep live startup untouched until story mapping is reviewed: the accepted ten-level opening is not the round-500 finale. Authoring the first 50-round sector and physical Android acceptance remain pending.

## Final results

Unity Verify01 passed **208/208 EditMode** (13 R5 cases +195 prior) and **4/4 PlayMode** (two new campaign real-board journeys +two existing native journeys), zero failures/skips. Actual campaign tests reached victory and failure/retry through legal board swipes and preserved the separate live GameState snapshot. Retained 36 native regression captures and two campaign test-state JSON files; English native Pause capture inspected. Browser disclosures, 430/1440-width containment and four local links passed without JS errors.

Hash audit: 7,247 existing Assets/Packages/ProjectSettings files unchanged, only Match3Board.cs modified, 11 new source/test/assembly/metadata files, zero deletions. Exact Addressables link.xml/meta backups restored after automatic Editor cleanup. No approved prefab/art/startup/activation/settings changes.
