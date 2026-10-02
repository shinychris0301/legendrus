# Playtest rounds 11–20

The collection tools are ready. No human campaign sessions have been collected yet, and the candidate's moves, goals, rewards and artwork remain unchanged from R9. Animations and effects remain deferred.

1. Open Unity, enter Play Mode and reach **Home**.
2. Select **Legends of Rus → Development → Campaign Playtest 11-20 (Recorded)**.
3. Read the hearth dialogue, then play rounds11–20 normally. This disposable test starts with **30 energy and zero boosters**. The historical1–10 progress is explicitly a test fixture, not your live progress.
4. After each win or loss, use the **Campaign playtest** editor window to rate **Difficulty**, **Goal clarity**, and **Special opportunity**, before pressing Continue or Retry. Leave Special opportunity unrated when not relevant. On12, look for a line-item opportunity; on18, look for the T-shaped match that creates a bomb. Do not read the solver paths before trying the rounds.
5. Retry naturally. Stopping early is useful evidence too. You can stop Play Mode whenever you finish; recordings are saved automatically, while disposable game progress resets.
6. Copy the JSON path from the monitor and share that file/path for analysis. The monitor is also available under **Legends of Rus → Development → Campaign Playtest Monitor**.

Automatic save happens at attempt start, after settled commands, at most every2seconds during active play, on focus loss/pause and at round/preview end. The monitor reports save failures and provides **Save now** to retry. An unclean crash can leave an `ongoing` snapshot; it is not counted as a defeat. This is local recording, not an automatic upload.

On this Linux machine the default folder is:

`/home/k2/.config/unity3d/DefaultCompany/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL/CampaignPlaytests/Sessions/manual_campaign/`

Each session gets one `playtest-<sessionId>.json` checkpoint and an optional `.bak` previous copy. Share the main `.json`; retain the backup if recovery is needed. Use the path shown in the monitor on other machines.

## What to watch for

- Do rounds11 and16 feel like a useful rest after the harder boards?
- Are the goals on15 and20 clear, including two-stage ice?
- Can you recognize the optional line/bomb opportunities on12 and18 without the solution?
- Does any loss feel caused by an unclear rule, or any win happen before you understand the puzzle?

Write any specific comments alongside the JSON when sharing it. The editor controls store categorical ratings; they do not record spoken/free-text feedback.

## What is recorded

Exact candidate fingerprint; application and matching-rule versions; level fingerprint; chapter/round; committed attempt and run IDs; starting/final board hashes; move budget/remaining moves; score/goals; active time excluding pause/background; outcome; inventory usage; ratings. Wins/losses describe the board result, not proof that a campaign reward save committed. Retry/exit interruptions remain distinct. No command replay is captured by this observer; these files are outcome evidence, not new solver witnesses.

Automated runs are always labeled `automated_campaign`, including any batch process. Old mechanics-sandbox sessions are kept separate. Sessions do not represent unique people. Unknown/used inventory runs are excluded from the unassisted summary.

## Analyze the returned files

Run from the project root:

```bash
python3 ProjectDocs/AI/CampaignFoundation/R10/Sources/AnalyzeSessions.py /path/to/CampaignPlaytests --output /path/to/CampaignPlaytestSummary.json
```

The analyzer accepts only manual sessions matching the current candidate's four-file fingerprint, deduplicates session snapshots, excludes automated/old-content records, and separates unfinished/abandoned attempts from losses. It reports invalid files rather than inventing missing outcomes. A few sessions cannot establish population win rates. Review actual play and these results before tuning R9 or adding rounds21–30.
