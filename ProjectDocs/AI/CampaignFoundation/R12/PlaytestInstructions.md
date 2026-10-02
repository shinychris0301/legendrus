# R12 follow-up playtest

1. Enter Play Mode in Unity and reach Home.
2. Choose **Legends of Rus → Development → Campaign Playtest 11-20 R12 Tuning (Recorded)**. The older menu without “R12 Tuning” still loads the unchanged R9 candidate.
3. Play rounds 11–20 without inventory boosters. Starting test allowance is 30 energy and zero boosters; progress is disposable. Expect roughly ten minutes based on the previous session, with variation for retries and reading.
4. Before Continue or Retry, rate difficulty, goal clarity and special opportunity in the Campaign Playtest Monitor. Focus on **12, 13, 14, 17 and 19**. On 12, note whether you recognized and used the chance to create a line item; mention this when sharing results, since individual commands are not recorded.
5. After round 20, finish the well story. The completion panel offers **Save Recording**, **Exit Playtest**, and optional **Replay Round 20**. Use the monitor's **Show recording** or **Copy recording path** to find the JSON. A save failure keeps Exit on the completion panel so you can retry.
6. Share that file path. You do not need to replay round 20. If you do, it becomes a separate attempt and uses the normal energy cost when you press Play. First-clear rewards are not granted again.

## What changed

| Round | R9 moves | R12 moves | Reason |
|---|---:|---:|---|
|13|28|25|Small first adjustment to the normal ice round|
|14|29|26|Small first adjustment to the normal chain round|
|17|29|26|Small first adjustment to the normal split-chain round|
|19|29|25|Slightly larger adjustment to the fastest normal round|

The earlier successful attempts used 10, 13, 11 and 6 moves respectively. These allowances retain substantial headroom; they are deliberately conservative experimental values, not established balance targets. Board seeds/layouts, goals, rewards and all other rounds are unchanged. Recovery 11/16, teaching 12/18 and hard 15/20 retain their prior budgets.

This second pass keeps the full sequence to preserve the difficulty rhythm and first-attempt seed comparisons. The focused questions concern the five rounds above. Previous playtest experience may make this pass easier, so one repeat does not establish new-player balance.

## Recording identity

The monitor displays a `CampaignSector01R12-…` content fingerprint. Original R9 recordings are kept intact and excluded from R12 totals. Files use the same local CampaignPlaytests/Sessions/manual_campaign folder with unique session IDs. Automated verification is tagged automated_campaign and is not human evidence.

To analyze only matching R12 recordings:

```bash
PYTHONDONTWRITEBYTECODE=1 python3 ProjectDocs/AI/CampaignFoundation/R12/Sources/AnalyzeSessions.py '/path/to/CampaignPlaytests/Sessions/manual_campaign' --output /tmp/campaign-r12-results.json
```

Live save and production content remain untouched. All animations/effects are deferred to the final development phase. Next after this evidence review: rounds 21–30 and the granary story milestone.
