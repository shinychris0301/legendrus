# Campaign R11 — first human rounds 11–20 review

The supplied recording completes every round in the current R9 candidate. Nine of ten rounds were won on the first attempt. This supports continuing development, with a focused balance and playtest-ending pass before authoring rounds 21–30. It does not establish broad balance acceptance or production readiness.

## Evidence and integrity

One user-supplied `manual_campaign` session, `c883e694fa8043098f9bf4b8850fe8d4`, contains 12 finished attempts, no unfinished attempts and no dropped records. The candidate's four-file SHA-256 matches the recording. All 12 opening hashes match the corresponding R9 round/attempt verification witnesses; budgets, goal-array lengths, outcome phases, settled flags and attempt sequencing are consistent. Repeated rounds retain the same level fingerprint. Unity's per-level serialization hash was not independently regenerated.

There are **10 unassisted wins and one unassisted loss**. The additional round-20 win consumed one inventory booster and is excluded from unassisted outcomes. It follows a win, so it is a replay, not a failed-round retry. The recorder does not identify the booster type. Recorded active play totals approximately 10 minutes, excluding time outside rounds and paused/unfocused time; this is not total session duration or a performance measurement.

[Raw recording snapshot](Sources/Session/playtest-c883e694fa8043098f9bf4b8850fe8d4.json) · [Provenance and SHA-256](QA/Provenance.json) · [Summary](QA/Summary.json) · [Per-attempt evidence](QA/Attempts.json) · [Existing analyzer output](QA/ManualEvidence.json)

## Round findings

Moves used below are budget minus remaining moves, not a count of every input. “Unrated” is missing feedback, not negative feedback.

| Round | Intended role | Attempt | Result | Moves used / budget | Left | Difficulty | Inventory used |
|---|---|---|---|---|---|---|---|
|11|Recovery|1|Win|6 / 32|26|Easy|0|
|12|Teach line item|1|Win|12 / 30|18|Easy|0|
|13|Normal: ice|1|Win|10 / 28|18|Easy|0|
|14|Normal: chains|1|Win|13 / 29|16|Easy|0|
|15|Hard: layered ice|1|Loss|27 / 27|0|Unrated|0|
|15|Hard: layered ice|2|Win|21 / 27|6|Easy|0|
|16|Recovery|1|Win|9 / 32|23|Easy|0|
|17|Normal: split chains|1|Win|11 / 29|18|Unrated|0|
|18|Teach Burst|1|Win|7 / 30|23|Fair|0|
|19|Normal: crate islands|1|Win|6 / 29|23|Unrated|0|
|20|Hard: mixed obstacles|1|Win|21 / 27|6|Easy|0|
|20|Replay after win|2|Win|27 / 27|0|Unrated|1|

- **11 and 16 provide the intended relief.** Their large move reserves are compatible with a recovery role. Keep them provisionally unchanged.
- **13, 14, 17 and 19 are the strongest tuning candidates.** Normal rounds retain 55–79% of their move allowance. Only 13 and 14 were rated easy; 17 and 19 were not rated. This suggests a low-pressure middle section for this player, not proof that each level needs a harder board.
- **15 is a real difficulty peak.** The loss ended with all ten ice objectives cleared and five of the 22 blue gems still missing. The second opening was different and the retry won with six moves left. This is a collection bottleneck in the observed failure, not evidence of an uncleared or broken obstacle. Preserve its current budget for now.
- **20 also requires more moves.** The unassisted win used 21 of 27. Its boosted second attempt reached zero remaining moves on a different opening; a zero-move win is consistent with all goals completed. We cannot attribute the difference to seed difficulty versus player choices without command recordings.
- **12 and 18 need teaching evidence, not automatic difficulty increases.** Round 12's goal/special ratings are absent. Round 18 was rated fair and clear despite finishing quickly. Retain teaching space; ask for a focused line-opportunity check during the next playtest.

## Feedback and what remains unknown

Difficulty: seven easy, one fair, four unrated. Goals and special opportunities: seven clear ratings each, five unrated each. There were no recorded unclear ratings, but blanks are not successful comprehension checks. All effect ratings are unrated, consistent with the decision to defer effects.

The recording contains no move-by-move commands, special-creation/activation counts, blocker damage timeline, screenshots, frame timings, narrative acknowledgements or reward-commit receipts. It cannot verify animation quality, exact match execution, use of the authored teaching moves, story comprehension, durable progression or Android performance. The application version is `0.0.0`; the content fingerprint identifies the candidate, but the file has no exact code revision. One session is not a population success rate.

The earlier solver traces confirm reproducible opening identities and solvability; their short solutions are not human performance benchmarks. No manual results have been substituted with solver results.

## End-of-candidate UX finding

Source inspection shows `CampaignRoundLifecycle.AcknowledgeResult` advances selection only when the next round is playable. After round 20, round 21 is unavailable. `CampaignDevelopmentUI.ShowMap` then displays the selected round's Pre-Level page. Thus the final result/story can lead back to round 20 without a dedicated collection-complete page. The second recorded round-20 run is consistent with this path, but does not prove that the replay was accidental.

Recommended fix: show a clearly labeled **Rounds 11–20 playtest complete** state after the final story, with Save/Reveal Recording, Exit and an explicit optional Replay action. Preserve result/story acknowledgements and reward idempotency. Do not unlock round 21 or mark the chapter complete. This is a next-step proposal, not implemented in this review.

## Recommended next pass

1. Clarify the end of the recorded test as above so completion cannot be confused with progression to a new round.
2. Create a separate, explicitly versioned tuning candidate for normal rounds 13, 14, 17 and 19. Change only move allowances initially, keeping board layouts, seeds, goals and rewards fixed so comparisons remain interpretable. Set conservative values after reviewing intended pacing; this single session does not justify a blanket percentage cut. Preserve recovery rounds 11/16, teaching rounds 12/18 and hard rounds 15/20 provisionally.
3. Run deterministic opening/solvability and progression checks on that candidate. Collect a short second human pass on the four normal rounds plus round 12, with difficulty and special-opportunity feedback before Continue. If the player prefers an easy campaign, retain that preference rather than forcing difficulty from spare moves alone.
4. Then author rounds 21–30 and the granary story milestone using the observed recovery/normal/peak rhythm. Keep production activation and all animation/effect refinement deferred.

## Verification and scope

This task changes documentation and analysis artifacts only. The original recording, game sources, candidate content, player saves, prefabs, artwork and effects were not modified.

- Existing Python analysis suite: **3/3 passed**.
- Supplied-session audit: content match, **12/12 opening hashes**, budgets/goals/phases/ordering and replay classification passed.
- HTML review checked for valid local links and 12 data rows.
- No new Unity compilation, runtime capture, Android test or human replay was run. The review is a data report, not an implemented game page or Unity capture.

Reproduce with `PYTHONDONTWRITEBYTECODE=1 python3 ProjectDocs/AI/CampaignFoundation/R11/Sources/ReviewSession.py /path/to/playtest-c883e694fa8043098f9bf4b8850fe8d4.json`.
