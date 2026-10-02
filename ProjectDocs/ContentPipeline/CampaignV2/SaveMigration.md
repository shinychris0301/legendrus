# Save migration design — v6 legacy slice to expanded campaign

## R1 boundary

`CampaignSaveMigration.Plan` is a pure, non-writing planner over an explicit projection of legacy progress. It is not a PlayerPrefs reader, a replacement save codec, or an activated migration. It rejects unsupported versions and unknown records instead of discarding them. It never calls currency, inventory, energy or reward services.

The existing v6 save journal remains the sole live save. The eventual target save schema number must be assigned with its codec change, not declared migrated by this document. The campaign content contract's version 2 is independent of the player's save schema.

## Field rules

| Existing state | Expanded campaign rule |
|---|---|
| Gold, gems, booster counts, energy and refill clock | Preserve exactly through the existing codec/transaction; do not recalculate from progression or award a welcome bonus |
| Chapter 1 level records 1–10 | Keep integer IDs, scores, stars, attempts, retries, failures, completions and rewardClaimed; add an explicit campaign-ID mapping without rewriting source IDs |
| Completed tutorials | Preserve every existing ID; new teaching steps use distinct versioned IDs |
| Completed story scenes | Preserve in a legacy story ledger; map to reviewed expanded milestones explicitly, never assume identical narrative meaning |
| Acknowledged Chapter 1 completion | Preserve as legacy demo acknowledgement; **do not** promote to campaign Chapter 1 completion |
| Selected chapter/level | Retain valid legacy selection; derive newly accessible content from contiguous wins and available packages; never auto-start a level or charge energy during migration |
| Highest unlocked level | Treat as legacy data, not authority to unlock unplayed or missing campaign rounds; calculate from validated per-chapter completion records |
| Daily rewards, settings, language and last reward state | Preserve their owning store and existing behavior; language is separately persisted |
| Unfinished board | No board snapshot exists in v6. Do not claim to recover it; a resumable-round design is a separate feature |
| Unrecognized version, record or future content | Block migration and retain the untouched source; do not “repair” by deleting unknown progress |

## Future commit protocol

1. Read through the current journal and codec; retain exact original bytes/checksum in the existing recovery scheme.
2. Validate source and required campaign package versions. Construct a candidate new payload on a copy. R1's planner returns only progress mapping, not a complete save payload.
3. Apply an explicit, reviewed story-scene mapping. The accepted ten-level finale needs a product/narrative decision before this step can be finalized.
4. Assert unchanged economy, inventory, settings, reward claims, daily state and tutorial records. Check unique progress IDs, chapter selection, package availability and completion invariants.
5. Validate new payload with the new schema codec; stage, verify, commit atomically using the existing persistence architecture. Preserve the old recovery payload until successful readback.
6. Store a migration identity/source version in the target envelope to prevent repeating migration. Restart after every simulated interruption point and prove one resulting state, no duplicate grants and no truncated journal.
7. A newer save opened by an older app must remain protected by the existing future-version write guard. Do not roll it back by normalizing it to the old ten-level catalog.

## Acceptance before activation

Fresh profile; partial legacy chapter; completed legacy chapter; duplicate first-clear attempts; gaps in completion; unsupported future save; corrupt primary with valid backup; blocked writes; app termination between stage/commit/readback; missing Chapter 2; content revision mismatch; repeated migration; profile totals and story state after relaunch. Test against isolated fixtures only, never a user's actual save.

R1 tests establish immutable copying, complete counter/reward preservation, legacy/campaign finale separation, repeatable planning and rejection of unknown/duplicate records. They do not prove an atomic v7+ save migration, because no new save codec or writer is installed yet.
