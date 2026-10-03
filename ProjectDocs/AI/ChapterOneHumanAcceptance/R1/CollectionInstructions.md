# Chapter 1 — next manual recording

The next concrete action is **record rounds 201–210 in Unity with the existing Active recorder**. Do this with human moves and ratings. The audit found no manual outcomes for 31–500 in the normal recorder folder or project archives; all 47 ten-round blocks remain open. This is a collection requirement, not a demonstrated balance defect.

1. Open this project in **Unity 6000.5.9f1**, enter Play Mode and reach Home with no modal open.
2. Open **Legends of Rus → Development → Campaign Active Playtest (Recorded)**. Select **201–210**, then **Start installed-campaign recording**. Avoid historical candidate menus.
3. The existing fixture supplies **30 energy and zero starting inventory boosters**, using an in-memory campaign journal. Its earlier completions are setup data. Normal player progress is not imported. Inventory boosters can be earned during the block; keep actual use in the recording.
4. Play without inventory boosters first. Keep genuine losses, free retries and interrupted attempts. Do not substitute solver commands, test harness wins or invented ratings. If you choose a booster-assisted attempt, retain it with its recorded usage.
5. Before Continue or Retry, set the existing monitor's **Difficulty** (`easy`, `fair`, `hard`), **Goal clarity** (`clear`, `unclear`) and **Special opportunity** (`clear`, `unclear`). Assess opportunities to create/use line, bomb, Color and combinations. “Clear” should reflect understanding, not merely winning. The monitor does not expose an effects-rating control; effects remain deferred.
6. Finish the block's dialogue and use **Save now / Copy recording path / Show recording**. Exit from playtest completion or stop Play Mode. No upload occurs. Export each block separately; the recorder retains at most 200 completed attempts per session and reports dropped history.
7. Record a short observation alongside the session: why a hard round caused difficulty, whether the following recovery relieved pressure, what goal/special was unclear, and whether the story checkpoint felt coherent. Use [BlockObservations.csv](BlockObservations.csv); enter the actual session ID and path. Blank fields are intentional and are not feedback.

On this workstation the normal folder is:

`/home/k2/.config/unity3d/DefaultCompany/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL/CampaignPlaytests/Sessions/manual_campaign/`

This is `Application.persistentDataPath/CampaignPlaytests/Sessions/manual_campaign/`; other machines can have a different persistent root. JSON checkpoint, `.bak` and repeated exports from one session must not be counted as separate players. The audit itself uses isolated Editor storage, not this normal player folder.

The installed content fingerprint is:

`CampaignActive-87f504a454cad5e4b52755755e07af856af32b51be9b9413a9f14a0e22003a80`

The native audit also records the exact per-round `JsonUtility` fingerprints. A changed content fingerprint requires a new native audit; do not relabel an older session as current. `manual_campaign` is recorder provenance and does not independently establish player identity. Reviewed recordings inform acceptance and any bounded tuning; coverage alone does not grant acceptance.

## Collection order

| Priority | Blocks | Purpose |
|---|---|---|
| 1 | 201–210, 211–220, 221–230, 231–240, 241–250 | First unverified late sector; compare authored hard peaks and recovery rounds. |
| 2 | 491–500 | Observe 495 hard → 496 recovery → 497–499 normal → 500 hard/finale, with the whole ten-round context. |
| 3 | 291–300, 341–350, 391–400, 441–450, 481–490 | Sample later sector endings and the final approach. |
| 4 | Every block 31–200 | Close the earlier human-evidence gap. |
| 5 | Remaining blocks 251–490 | Complete systematic coverage; no skipped rounds are implied by sampling. |

In **201–210**, authored hard rounds are **205/210**, recovery rounds **201/206**. These are content labels, not measured human difficulty. The per-round matrix includes the actual move limits, goals, obstacle types and story scenes. Compare the six late sectors rather than judging all late content from one final checkpoint.

An isolated block does not establish continuous 470-round pacing or cumulative economy. Keep that limit in the review. The development recorder ends on **PLAYTEST COMPLETE** after the selected block's dialogue, including at 500; normal-game Chapter Complete → World Map and locked Chapter 2 remain separate integration checks with historical R6 evidence. Interrupted attempts do not restore their board or establish same-attempt continuation. Phone execution and hosting remain deferred.

## Refreshing the report

For additional exports in the documented normal folder or `ProjectDocs/AI`, run `python3 ProjectDocs/AI/ChapterOneHumanAcceptance/R1/Sources/AuditCoverage.py`, then `python3 ProjectDocs/AI/ChapterOneHumanAcceptance/R1/Sources/BuildReview.py`. The scanner reads sessions and writes only this report directory. Current recordings still require review; unrated, stale, interrupted, automated and truncated evidence is reported separately from eligible outcomes.

If installed content changes, first regenerate the native audit with the supplied temporary Editor helper, after checking Unity is closed. For example, pass `--output ProjectDocs/AI/ChapterOneHumanAcceptance/R1/QA/NativeAudit02` to `RunNativeAudit.py`, then pass `--native ProjectDocs/AI/ChapterOneHumanAcceptance/R1/QA/NativeAudit02/InstalledContent.json` to `AuditCoverage.py`. Use a fresh output directory; the launcher deliberately refuses an existing directory and the original evidence must not be overwritten. Do not change campaign content merely to make a historical fingerprint match.

## 2026-10-03 — review the first201–210export

[Recording review R2](../R2RecordingReview/README.md) adds an explicit-path, read-only review command for the installed201–210block. Final30Python checks pass; the current five historical recordings provide0qualifying outcomes. The user confirmed no new201–210recording yet. Collect actual human moves/ratings and block observations as above, then use a fresh R2output directory. Exit2with needs_human_recording is an intentional pending result; exit0only means ready for human review, never automatic acceptance. R5development APK refresh is complete; phone execution and hosting remain deferred.
