# R13: play rounds 21–30

1. Enter Play Mode and reach Home.
2. Choose **Legends of Rus → Development → Campaign Playtest 21-30 R13 Granary (Recorded)**.
3. Read the well scene, then play through21–30 **without inventory boosters**. The preview seeds prior progress through20 using disposable campaign transactions. This leaves **20 of30 test energy and one earned Mace**. Leave it unused; any consumption is recorded and separated from unassisted results. Production progress and inventory are untouched.
4. Rate difficulty, goal clarity and special opportunity in the monitor **before Continue/Retry**. Focus on feather routes23/27, the Color-item opportunity24, and the difficulty changes25→26 and30. Optional notes when sharing: did you understand how to bring feathers to the bottom, and how to create the five-match Color item?
5. After winning30, read the granary story. The completion panel offers Save Recording, Exit Playtest and an explicit optional Replay30. Round31 and Chapter2 are unavailable. Save and use **Show recording** or **Copy recording path** in the monitor, then share the JSON/path.

Do not replay30 unless you want another attempt. Starting a replay consumes normal test energy. First-clear rewards do not repeat. The monitor identifies `CampaignSector01R13-…`; the earlier R9 and R12 menus remain available separately.

Recordings survive Play Mode in `Application.persistentDataPath/CampaignPlaytests/Sessions/manual_campaign`. The R13 analyzer excludes older candidate fingerprints and automated tests:

```bash
PYTHONDONTWRITEBYTECODE=1 python3 ProjectDocs/AI/CampaignFoundation/R13/Sources/AnalyzeSessions.py '/path/to/CampaignPlaytests/Sessions/manual_campaign' --output /tmp/campaign-r13-results.json
```

There are no human R13 results yet. Further animation/effect work remains deferred. After this playtest, review the new delivery and Color-item lessons before authoring31–40 and the riverbank milestone.
