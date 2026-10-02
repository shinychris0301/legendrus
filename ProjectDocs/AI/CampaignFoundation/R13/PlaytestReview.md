# R12 human playtest review — 28 September

The newest local recording, `309d572a061640adaf6bff53a560d98a`, matches the exact R12 content fingerprint. It records ten first-attempt wins on rounds11–20, no inventory usage, no dropped attempts and no unfinished attempts. All ten opening hashes match the existing first-attempt reference witnesses; goals, outcomes, phases, budgets and attempt ordering are consistent. This is one repeat-player session, not a population success rate.

|Round|R9 first attempt: moves used / budget|R12: moves used / budget|R12 moves left|
|---|---|---|---|
|11|6 / 32|7 / 32|25|
|12|12 / 30|12 / 30|18|
|13|10 / 28|13 / 25|12|
|14|13 / 29|5 / 26|21|
|15|27 / 27, loss|23 / 27, win|4|
|16|9 / 32|10 / 32|22|
|17|11 / 29|3 / 26|23|
|18|7 / 30|9 / 30|21|
|19|6 / 29|6 / 25|19|
|20|21 / 27|15 / 27|12|

The R9 retry on15 and boosted replay on20 are deliberately excluded from this first-attempt comparison. Opening identities match between revisions. Different choices and learning remain confounders; lower move counts do not prove the allowance changes caused better play.

Round15 still creates pressure, finishing with four moves remaining. Round13 consumed more of its adjusted budget. Rounds14/17/19 remain generous despite tuning. Retain the R12 allowances provisionally while moving to the next authored block, rather than repeatedly cutting them from a single repeat player's outcomes.

All difficulty/goal/special/effect fields in the JSON are **unrated**. In follow-up conversation the user answered “yes. the difficulty is normal.” to the question about difficulty and recognizing round12's line-item rule. Record this as overall qualitative feedback: difficulty normal and line rule understood, not as invented per-attempt ratings. This supports retaining R12 while authoring21–30; effects remain deferred. The recorder does not prove that the final story/completion screen was seen, since its outcome is captured at board-result time. It also contains no command stream, exact booster type, screenshots, frame timings or reward-commit receipts.

[Raw snapshot](Sources/Session/playtest-309d572a061640adaf6bff53a560d98a.json) · [SHA-256/source provenance](QA/Provenance.json) · [Summary](QA/Summary.json) · [Attempt details](QA/Attempts.json). Original recording is unchanged.
