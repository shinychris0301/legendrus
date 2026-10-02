# CONT-01 — failed-round recovery: test results

Implemented the approved next foundation: normal failed rounds retain their settled board and attempt in the existing save. Relaunch restores the Failure decision; Retry/Exit finalizes the loss once. Existing free retries, approved UI/art, matching rules and effects are preserved. Paid/ad continuation is still disabled; no moves, price, spend or provider success is invented.

## Final verification — Unity 6000.5.9f1

| Run | Passed | Failed / skipped |
|---|---:|---:|
| Full Edit Mode suite | 1,917 | 0 / 0 |
| Combined affected runtime suite | 104 | 0 / 0 |
| Separate-process save → recover/decline → restart after decline | 3 | 0 / 0 |
| **Distinct total** | **2,024** | **0 / 0** |

[Machine-readable results](QA/Verification.json) · [Integration XML](QA/Integration/results.xml) · [Edit Mode XML](QA/EditMode/results.xml). Final restart phases: [write](QA/RestartWriteFinal/results.xml), [read](QA/RestartReadFinal/results.xml), [cleared](QA/RestartClearedFinal/results.xml). These are actual Unity test runs, not source-only checks. Tests used isolated `/tmp/lor-retained-round-r1/` saves and the previously approved process-only Vulkan/descriptor-4096 setup. Project backend/settings and the normal player save were not changed.

The 104-case runtime run comprises 21 new retained-round cases, 6 native Failure cases, 11 save migration/recovery cases, 10 level-system cases, 10 story/milestone cases and 46 campaign checkpoint cases. Restart tests execute in three separate Editor processes with one isolated store; they are not simulated by replacing a memory object.

## Verified behavior

- Exact checkpoint round trip: tile arrays, all five special-item kinds, obstacle kinds and one/two/three layers, feathers, score, goal progress, zero moves, content/rule contract and attempt identity. Reconstructed board remains input-locked with no second attempt registration.
- A real legal swap from a controlled one-move fixture settles its cascade and enters the production save/failure route. Synthetic board/near-failure fixtures are functional tests, not human difficulty evidence.
- Cold-process recovery reproduces the stored attempt and Failure score. Decline persists once and stays cleared after the next restart. No extra gold, completion or energy charge; duplicate native actions do not create duplicate attempts.
- Storage failures retain the pending in-memory decision, leave failure counters unchanged, and allow native actions to be retried after storage recovers. Change listeners observe the committed state.
- Stale identities, changed rules/content, malformed arrays/goals/enums/layers and bad hashes cannot restore. Invalid optional checkpoint data preserves the rest of player progress. Legacy saves without the field still load.
- The next resolution plan/hash depends on restored snapshot plus action/rules, not a hidden random cursor. Existing inventory is preserved in the player save and is never rewound by restoring the board.
- Existing native Failure layout, bilingual labels, disabled paid/ad offers, routing, level rewards and story/checkpoint behavior remain covered. The actual [576×1280 Russian Unity capture](QA/Integration/Captures/LOR_FAIL_CAPTURE_DIR/Fail_576x1280_safe_RU.png) was inspected. This is not a new visual design or physical-device acceptance.

## Corrections found during verification

The first harness run stopped before compilation because its copied project-root path was wrong. Intermediate runs exposed Unity's null-inline-object serialization both during load and migration; both paths now normalize the cleared checkpoint. A temporary retry charging change was reverted to preserve the existing free-retry policy. The old failure test fixture was corrected to execute the end-state coroutine and leave its tutorial pause before simulating failure. All failures in this task's final selected coverage are resolved; earlier logs remain as evidence and repeated passes are not double-counted.

## Scope and remaining work

[Scope audit](QA/Scope.json): five existing project files changed, ten source/meta files added, none deleted; 7,711 task-start files unchanged. Production changes are GameState, SaveService, Match3Board and ScreenRouter plus three small source additions. Tests comprise one updated fixture and two new fixtures. [Task-only patch](Sources/Implementation.patch) and [implementation notes](ImplementationNotes.md) retained. Temporary test scenes/linker output cleaned; no commit or deployment. CurrentTask, SessionLog, development schedule and deferred continuation status updated.

This implements **failed-round recovery**, not playable paid/ad continuation or arbitrary mid-round recovery. CONT-02 must atomically bind an entitlement/move grant to this attempt, integrate approved authoritative provider contracts, and verify interrupted spend/grant, duplicates, resumed win/re-fail and Android behavior before offers activate. Current Android/device, live services, human Chapter 1 difficulty/pacing and the previously recorded broad Play Mode regression debt remain unverified. The full Play Mode suite was not rerun here. Animation/effect work remains last.
