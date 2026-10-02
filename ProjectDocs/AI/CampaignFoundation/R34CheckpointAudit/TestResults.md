# Campaign R34 — 200-round checkpoint audit

**29 Unity tests passed; 0 failed; 0 skipped.** Normal gameplay remains at rounds **1–200**. This pass audits the checkpoint and adds regression coverage; it does not add levels or claim completion of the remaining UX/device work.

## Newly executed verification

Unity 6000.5.9f1, isolated save store, previously authorized process-only Vulkan and descriptor limit 4096. [Test XML](QA/Verify01/Checkpoint/results.xml) · [Verification summary](QA/Verification.json).

- **3 new regression cases passed:** interruption after the first line of a multi-line catch-up scene restarts the unread scene without skipping or duplicate acknowledgement; sparse read history resumes only unread milestones in order across reloads; unlocked/last-completed markers cannot substitute for actual milestone completion.
- **26 existing checkpoint cases passed:** single acknowledgement through round 200, failed/retried checkpoint persistence, reward/unlock protection, ordered 27-milestone history and unavailable round 201/Chapter 2.
- Fixtures exercise story/progress services. They are not human playthroughs or native navigation/pointer captures.

## Fresh audit results

| Check | Result |
|---|---|
| Installed content | 200 contiguous rounds; four active JSON files exactly match R33 candidate |
| Scenario references | 28 scenes including intro, 27 milestones, 400 outcome reactions; no orphan scenes or missing required keys |
| Bilingual coverage | 632 nonempty required RU/EN keys, including speakers, dialogue, reactions and round names 11–200 |
| Final chapter boundary | No installed Chapter 1 finale or round 201; planned finale remains 500 |
| Repeated reaction copy | **RU: 6 duplicate groups / 139 repeated occurrences beyond first. EN: 9 groups / 143 repeated occurrences** |
| Largest repetition | Rounds 51–100 repeat one victory sentence and one failure sentence, 50 times each |
| Returning-player backlog | Up to **27 scenes / 33 lines**, excluding intro; navigation rechecks pending scenes one at a time |
| Manual evidence | Five valid exports in the default folder, covering only 11–30; zero files covering 31–200 |
| Current recording route | Existing recorded menu stops at **31–40 R14**, reading the older candidate; no normal-gameplay recorder attachment found |
| Android device | Fresh ADB query succeeded; **0 attached / 0 authorized** devices; no build/install attempted |

[Content](QA/ContentAudit.json) · [Editorial duplication with every affected key](QA/EditorialAudit.json) · [Backlog](QA/BacklogAudit.json) · [Manual exports](QA/ManualEvidence.json) · [Device query](QA/DeviceEvidence.json).

**Backlog clarification:** after each scene the router returns directly to Chapter Map. Entering the map or requesting a level can open the next scene. The code does not automatically play 27 scenes consecutively, but repeated interception can prevent a returning player from reaching the selected level promptly. Ordering/persistence passing does not resolve this UX issue.

## Retained evidence, not rerun

R33's four result XMLs were inspected: **302 passed**, no failures/skips. All **600 replay witness files** remain present and active content still matches that candidate. This is retained automated solution evidence, not 600 new replays or human win rates. [Evidence record](QA/RetainedEvidence.json).

## Prepared follow-up

The highest-priority implementation gap is current-content playtest collection. [Next step](NextStep.md) specifies an isolated recorded block selector using CampaignActive and matching rules/art parity, before further expansion. The existing 31–40 R14 menu must not be described as testing the full installed R33 campaign.

[Returning-player flow](ReturningPlayerFlow.md) defines optional reading/recap/later behavior and persistence safeguards; **not implemented or user-validated**. [Editorial draft](EditorialDraft.md) provides **20 bilingual replacement reactions for 51–60**, reviewed against their actual goals/layouts; **draft only, not installed or visually accepted**. Accepted early scenes are preserved. Remaining repeated copy still needs bounded editorial passes.

## Change scope and limits

Only project-source change: `Assets/LegendsOfRus/Tests/CampaignPlayMode/CampaignCheckpointLifecycleTests.cs` (three regression cases). **7,657 existing project files unchanged; 0 added/deleted** under Assets, Packages and ProjectSettings. Production code, content, art, rules, economy and settings unchanged. [Scope](QA/Scope.json) · [Exact test patch](Sources/TestChanges.patch) · [Source checks](QA/SourceChecks.json). Python syntax and scoped patch whitespace checks passed. Test-generated scene/linker cleanup complete. No commit.

**Still unverified:** human difficulty for 31–200, current Android build/device, full-project suite and fresh native visual/pointer acceptance. The older Android UI-audit build is historical, not evidence of current campaign delivery. Default-folder absence does not rule out external playtests. Recap/story-history UX and editorial drafts require implementation and validation. Animations/effects remain the final phase. Historical R22 results are preserved as historical evidence.
