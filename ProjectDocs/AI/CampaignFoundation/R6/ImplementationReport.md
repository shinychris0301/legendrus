# Campaign Foundation R6 — development UI and booster transactions

The campaign foundation now has an opt-in UI composition using the approved Pre-Level, Gameplay, Pause, Victory, Fail and Error Dialog prefabs. Four gameplay boosters use campaign inventory transactions. The live startup/router and live player-save format remain unchanged.

## Open the implemented preview

Enter Play Mode, reach Home, then choose **Legends of Rus → Development → Campaign UI Preview (In Memory)**. Play opens the selected authored round. The Pre-Level Back button returns to the previously visible Home UI. The menu uses the existing Canvas, safe area and EventSystem; it does not create a scene or install another startup path.

This preview imports a **synthetic default player**, not your live save. Its journal is in memory and is discarded when the preview ends. It loads the existing validated ten-level Chapter 1 bridge. There is no new 500-level content or full campaign chapter-map selector in this milestone. Returning from a result presents the selected Pre-Level page; after a win the next playable round is selected.

## Implemented behavior

- Energy/attempt commitment precedes board creation; result persistence precedes Victory/Fail presentation.
- Existing presenters accept explicit campaign data/count providers. Campaign values drive the shared resource header, moves, score, goals, result rewards and booster stock without registering the campaign with GameState.
- Pause supports resume, paid retry and an interrupted-attempt decision. Settings and unrelated navigation/provider actions are disabled in this isolated composition.
- Interrupted runs offer a paid restart or an explicit end to the attempt. Ending records zero recovered score/progress and retains spent energy; it does not pretend to restore cell state.
- Storage uncertainty stops the owned board and offers reopen/recovery. A throwing reopen callback remains retryable. UI uses the approved error panel with localizable Russian/English copy, including the restart cost.
- Victory's external bonus/ad action is disabled. Continue acknowledges the result without granting it again. Fail supports retry and return.

## Booster transaction boundary

The development payload advances from version 8 to **9**, adding a monotonic booster receipt. Frozen version 7 and 8 shapes support explicit upgrades; direct legacy v6 copy import remains available. This does not change the live v6 save service or automatically migrate real users.

A booster reserves one item in the journal before its effect starts. Completion marks that reservation applied. Runtime command recovery restores board/moves/score/goals and refunds the reservation once. A lost board refunds only an unsettled reservation; an already applied booster remains spent. Pending reservations block round starts/results. Unconfirmed writes latch inventory input until storage is reopened. Idempotent receipts prevent duplicate delivery from charging or refunding twice.

Bogatyr Mace, Firebird, Living Light and Sea Pearl use this path. The actual existing board effects are reused; no new sprites or effects were generated. Campaign boards suppress legacy ScreenRouter toasts so they cannot display messages through the underlying live page; the approved gameplay HUD continues to show armed-booster state.

## Verification

See [machine-readable verification](QA/Verification.json), [scope audit](QA/Scope.json) and the [review page](Campaign_Foundation_R6_Review.html). The review displays actual Unity captures individually, not generated mockups.

Initial runs are retained: Verify01 exposed a test API typo; Verify02 exposed a null live-state lookup in Pre-Level, which was fixed. Verify03 completed EditMode but the combined PlayMode Editor process terminated with native signal 11 and wrote no test XML. That run is not counted as passed. Visual inspection also exposed recovery-text clipping and a shared-header refresh that overwrote campaign energy; both were corrected before the final split-suite run. Splitting the suites avoids sharing one Editor process across all campaign and native journeys; it does not establish the cause of the native crash.

Final **Verify04 passed 219/219 EditMode and 9/9 PlayMode**, zero failures/skips. PlayMode ran in three separate Editor processes: five campaign UI tests, two real campaign board tests and two existing native win/fail journeys. Tests cover all four booster effects, timeout rollback/refund, repeat use, interrupted-run recovery, throwing storage reopen, shared-header energy and HUD/result values. EditMode adds 11 cases, including eight journal mutation boundaries for reserve/apply/refund, v8-to-v9 upgrade and duplicate/stale/conflicting receipts. Six campaign UI screenshots and 36 native captures are retained. Visually inspected Pre-Level, Gameplay, Victory, recovery, Fail and save-error views.

## Scope and limits

Approved artwork, serialized page prefabs and GUIDs remain unchanged. Presenter providers and the owning opt-in composition supply runtime data and modal geometry; no prefab revision bump or asset rebuild is required. No backend, actual-player migration, purchase/ad integration, live startup registration or Android setting changes.

Tests invoke button callbacks and real board commands. They do not establish physical touch hit targets, Android/IL2CPP acceptance, process-kill durability, complete safe-area coverage or full-project regression coverage. The preview is a development integration, not a production campaign cutover. It does not restore an interrupted board cell-for-cell or author/balance new rounds. Story reactions remain outside this development flow pending the approved opening-arc/finale mapping.

## Recommended next step

Prepare a concrete **Chapter 1 opening-arc and first 50-round sector specification** for review: map the existing ten-level story beats into the expanded chapter, separate the old level-ten finale from the round-500 finale, list objective/difficulty/obstacle/booster introductions, rewards and Russian/English dialogue coverage. Review that content plan before changing accepted scenes or activating expanded progression. Physical Android acceptance remains pending an available device.
