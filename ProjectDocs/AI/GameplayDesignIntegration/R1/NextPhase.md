# Next phase — campaign and story progression

The user has explicitly moved all further animation and gameplay-effect work to the final development step. R25 is the retained sandbox effects baseline. Do not resume effect polish as the next recommended task. The user also explicitly selected adoption of the newer matching rules. Current artwork and the stable PrototypeR4 rule profile now become the default for new normal/campaign rounds; existing R25 effects are reused without further refinement.

## Current boundary

- Existing native pages are implemented; this pass adopts current board artwork in normal gameplay and the campaign UI preview.
- Normal gameplay adopts PrototypeR4 matching (the existing version identifier is retained for deterministic replay). Historical Legacy remains selectable in the sandbox. Existing records/reward claims are preserved; no unfinished board is migrated.
- Campaign Foundation R1–R6 supplies versioned catalog/save/transaction foundations and an opt-in UI preview. It is not an active multi-chapter production campaign.
- R7 has story and first-50-round drafts. Rounds 11–50 are briefs, not playable authored boards. The expanded round-500 finale and replacement round-10 beat have not been activated.

## Next implementation milestone

**Campaign Foundation R8 — reliable story handoff in the existing campaign preview.**

Connect the approved ten-level story to the campaign preview's Pre-Level and result flow using persistent, idempotent scene acknowledgements. Keep the ten-level bridge's existing finale meaning; do not silently move already acknowledged finale scenes or turn unreviewed round briefs into live content.

Required behavior:

1. Pre-level intro/story appears only when eligible and unacknowledged.
2. A committed win opens Victory. Continue acknowledges the result once, then opens its pending story before the next Pre-Level.
3. A scene is acknowledged only when the player advances it. Closing/reopening before acknowledgement must offer that same pending scene again.
4. Scene acknowledgement never grants a reward, spends energy, registers an attempt or replays a completed result grant.
5. A failed/storage-uncertain transition uses the existing recovery UI. The preview must remain independent from the live player's save.
6. RU/EN dialogue/reaction/completion keys are complete for the actually connected route; use existing approved dialogue UI and art.

Acceptance checks: intro, win, fail/retry, repeated Continue, interruption at each boundary, storage reopen, scene acknowledgement, completed-player replay and locale switch. Keep deterministic campaign state/receipts and inspect actual native captures.

## Following milestones

1. Author and validate the first 50-round sector using the adopted rules after its story mapping is settled and balance has been validated. Existing legacy achievements retain their IDs and claims.
2. Complete chapter progression and Chapter 2 package continuity; distinguish installed/available content from planned chapters.
3. Complete the selected product systems (identity/save ownership before connected social/economy services). Preview screens stay clearly labelled until services work.
4. Perform integrated Android, persistence, content/balance and navigation acceptance.
5. **Final polish:** animations, gem shatter, special creation/activation, combinations, boosters, obstacle hit/break effects, sound/haptics synchronization and performance tuning. Resume from the preserved R25 effects, not from an older revision.

This document schedules work; it does not claim campaign story handoff, new rounds, backend services or device acceptance have been implemented.
