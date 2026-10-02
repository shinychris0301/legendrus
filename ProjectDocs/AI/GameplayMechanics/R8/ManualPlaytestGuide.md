# Gameplay Mechanics R8 — Manual playtest handoff

The five rounds and local feedback recorder are ready. **Manual playtesting is pending.** No session files were found in this project's default MechanicsPlaytests folder on 2026-09-26. R7's 18 automated passes establish test coverage, not player difficulty or satisfaction.

## Launch the rounds

1. Open the existing project in Unity 6000.5.9f1. Enter Play Mode and navigate through Start to **Home**.
2. Close any modal. Choose **Legends of Rus → Development → Gameplay Mechanics Sandbox** in the Editor menu. This command is enabled only at Home in Play Mode, with no modal or other development sandbox open.
3. The sandbox starts directly at **Round 1 of 5**. Keep it in round mode; the **Fixtures** button switches to isolated effect demonstrations and ends the current round attempt.
4. Play with adjacent gem swaps and board specials. The inventory boosters have zero stock intentionally. These rounds test earned specials and obstacles without spending campaign inventory.

No solution moves are included here. Read each round's own instruction, then try it before consulting any automated witness or solution review.

## Play and record

Make one first attempt at each round. Use **Retry** for additional attempts when useful; retries are recorded separately. Use **Next** to advance after entering feedback. After Round 5, use **Exit**—Next wraps back to Round 1.

| Round | Name | What to observe |
| --- | --- | --- |
| 1 | Ледяная тропа | Is the line special's direction and effect on ice clear? |
| 2 | Под ледяной коркой | Are layered ice damage and bomb reach easy to understand? |
| 3 | Полет искры | Can you follow the Spark from activation to its target? |
| 4 | Рунные врата | Can you distinguish the seal's remaining layer and recognize effective hits? |
| 5 | Испытание мастера | Do combinations and mixed obstacles remain readable during cascades? |

After a win, loss or decision to leave an attempt, cycle the three feedback controls **before** Retry, Next or Exit:

- **Difficulty:** easy / fair / hard. Leave unrated if undecided.
- **Goals:** clear / unclear.
- **Effects:** readable / busy.

Use **Save now** and check **Saved locally**. If it reports a failure, retry and retain a separate **Export JSON** if possible. Normal sandbox exit and Play Mode shutdown also checkpoint locally. A crash retains only the last checkpoint; it does not resume the board or invent a win/loss.

A move budget exhausted is a loss; leaving an unfinished round is an interrupted attempt. Both are useful observations. Do not keep replaying solely to turn every record into a win.

For specific visual defects, record a short separate note using [ObservationNotes.md](ObservationNotes.md). The JSON stores ratings and round state, not screenshots, video, complete move replays or free-text comments. Note the round and attempt number, what triggered the issue, and what was hard to see.

## Find and review the results

The runtime **Copy location** control copies the most recent explicit export path, or otherwise the current checkpoint path. An explicit export is a snapshot; it will not acquire later feedback automatically. Export again after your final ratings if sharing that file.

Default checkpoint directory for the current Linux project settings:

```text
/home/k2/.config/unity3d/DefaultCompany/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL/MechanicsPlaytests/Sessions/manual_sandbox/
```

Each new sandbox session creates its own JSON. The accompanying `.bak` is the previous checkpoint, not a separate session. Use one latest valid snapshot per session, preserving older copies for recovery. Automated records under `automated_test` must not be counted as manual attempts.

[Open the existing local JSON viewer](../R6/Gameplay_Mechanics_R6_Review.html). Provide the saved folder path and any observation notes for the next review; nothing is uploaded by the recorder.

## Decision after the first pass

First inspect actual remaining goals/moves, first attempts versus retries, and explicit clarity ratings. Reproduce specific reports in the relevant fixture, then make the smallest indicated adjustment and recheck affected rounds. One person's attempts are qualitative feedback, not a population win-rate estimate. Ongoing/interrupted attempts and unrated fields stay separate from completed outcomes and explicit ratings.

The first pass covers short sandbox rounds only. It does not approve campaign-wide mechanics migration or the first 50 production rounds. Additional players and phone testing remain necessary before wider balance decisions.

## Verification limits

This handoff was checked against the current Editor menu, sandbox, feedback, round and storage source files. No Unity run, new gameplay change or human session was performed for R8. [R7 verification](../R7/Gameplay_Mechanics_R7_Review.html) passed all 18 selected PlayMode tests with captures using the approved process-only Vulkan override. Default OpenGL capture instability remains unresolved; project graphics settings have not been changed.
