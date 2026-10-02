# CONT-01 — retained failed-round foundation

The approved next step is same-attempt failure recovery before activating continuation purchases or ads. This change installs the provider-independent checkpoint and terminal-failure lifecycle. It does not grant moves or make either offer available.

## Installed behavior

A normal round reaches zero moves only after its command/cascade settles. The owning router saves an optional `retainedRound` in the existing v6 player-save journal before retiring the board. This carries its GUID identity, chapter/level and attempt number, content fingerprint, rule version, seed, board hash, all tile/special/obstacle/layer/objective-item arrays, score, zero moves and goal progress. No alternate startup, save file or routing system is introduced.

Opening Home after relaunch recovers the existing Failure page. Opening settings/profile/shop retains the pending decision. Retry or an explicit departure from Failure to the map/Home finalizes one loss and clears the checkpoint durably. Retry then creates a new attempt using the existing free-retry energy policy. Recovery does not consume energy, stock or currency; no completion/reward is granted. The existing disabled paid/ad controls and approved native layout remain unchanged.

`InitializeRetained` reconstructs a fresh, input-locked board with identical state and the original attempt number. It does not register another attempt. It is the restore seam for CONT-02, not an enabled zero-move continuation action. Normal UI currently restores the failure decision screen; a playable continuation still requires the future entitlement operation.

## Persistence and compatibility

A failed checkpoint write keeps an in-memory copy and displays a localized save warning. A failed decline/retry write keeps the pending decision and rebuilds the native Failure controls so the user can try again. Neither failed write increments the loss count. A successful primary commit remains adopted even if secondary journal cleanup reports failure, following the repository's existing persistence contract.

Missing identities normalize to no checkpoint after load because Unity inline JSON serialization can rehydrate a null serializable object as an empty instance. Existing saves without this optional field remain supported. Unknown schemas/rules, changed level fingerprints, stale attempt ownership, malformed arrays/enums/goals and mismatching hashes cannot restore a board. Invalid optional checkpoint data does not reset the rest of the save; a new attempt clears it. A valid pending attempt blocks level switching and direct attempt registration until finalized.

Inventory belongs to the existing player save, including already-consumed boosters. A settled failed board has no armed command to replay. Restoring never rewinds the player's current inventory to an old copy. The board planner already derives resolution randomness from the snapshot plus action and rule version; the checkpoint declares `snapshot-action-v1`. Resolution plan/hash parity is checked using different initial random seeds. No runtime RNG algorithm or matching rule is changed.

## Scope boundaries

This is not active-mid-round recovery, device acceptance, cloud synchronization, a paid/ad entitlement, or a proof of exactly-once external analytics delivery. Failure telemetry is invoked once after durable local finalization; delivery across a process crash remains the responsibility of the later telemetry/transaction service. Campaign sandbox/coordinator sessions remain managed by their existing collector.

CONT-02 still needs the authoritative transaction/entitlement contract, selected provider sandbox, approved pricing and ad limits, atomic spend plus recoverable move grant, stale/duplicate callback protection, resume/win/re-fail coverage and Android interruption tests. Effects remain last. Human Chapter 1 difficulty/pacing and the previously documented broad runtime regression debt remain outstanding.
