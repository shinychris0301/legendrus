# Campaign R37 — outcome language test results

2026-09-29. **51 Unity tests passed; 0 failed; 0 skipped.** Outcome reactions now use the selected Russian or English language in Victory, Fail and the legacy outcome overlay.

| Suite | Passed | Evidence |
| --- | ---: | --- |
| Native outcomes and legacy overlay | 3 | [XML](QA/Verify01/Outcome/results.xml) |
| Campaign checkpoint lifecycle | 32 | [XML](QA/Verify01/Checkpoint/results.xml) |
| Installed campaign resources and story | 16 | [XML](QA/Verify01/Editorial/results.xml) |
| **Total** | **51** | [Verification](QA/Verification.json) |

Six cases are new. The resource checks include 600 deterministic legal, no-opening-match board starts; these are not 600 completion runs or human difficulty measurements. Tests ran in Unity 6000.5.9f1 with isolated save stores and the previously authorized process-only Vulkan backend and descriptor limit of 4096. Project settings were unchanged.

## Verified behavior

- Explicit English/Russian reaction lookup works for victory and failure on rounds 51, 55 and 60. The existing two-argument API remains Russian by default; unsupported language falls back to Russian. Null level and unsupported outcome guards remain intact.
- Native Victory refreshes its existing caption on EN → RU → EN switches without adding UI objects or changing progress, rewards or other player state. The asynchronous state comparison excludes only the independent energy-clock timestamp.
- Fail preserves its existing hidden reaction state and uses the same area for inspected booster help. Changing language does not replace the help with a story caption.
- A temporary legacy-overlay fixture updates its existing speaker/body on language changes without adding objects or mutating player state. It is not installed over native Victory or Fail.
- Existing checkpoint persistence, story and active/candidate resource checks pass.

## Actual Unity captures

These captures use explicit service-result fixtures, including prior progress. They are not manual playthroughs or evidence of new board wins. Round 51 is a replay fixture so the existing caption is visible instead of the first-clear video-bonus explanation.

- [English Victory, 941](QA/Verify01/Outcome/Journey/Outcome51_EN_941.png)
- [English Victory, 576 with safe area](QA/Verify01/Outcome/Journey/Outcome51_EN_576.png)
- [Russian Victory after switching back, 941](QA/Verify01/Outcome/Journey/Outcome51_RU_941.png)
- [English Fail booster help, 576](QA/Verify01/Outcome/Journey/Outcome55_FailHelp_EN_576.png)

All four captures were inspected. The tested caption fits its existing rectangle and Russian text returns correctly. The existing Victory caption is still very small at phone size; this pass does not establish comfortable readability or acceptance of every outcome string at every resolution. Layout, font and art changes were outside this correction.

## Change and source checks

[Scope](QA/Scope.json): seven existing source/test files changed; zero project files added or deleted; 7,664 baseline project files unchanged. The exact changes are retained in [Changes.patch](Sources/Changes.patch). Runtime changes are in StoryService, ProductionVictoryView, ProductionFailView and ScreenRouter; regression coverage is in CampaignCheckpointLifecycleTests, VictoryAlignmentPlayModeTests and FailAlignmentPlayModeTests.

[Source checks](QA/SourceChecks.json): Python syntax and scoped patch whitespace pass; 3,400 protected resource, candidate, package and settings files are unchanged. No serialized layout changed. Generated linker files were restored and only newly generated test scenes absent from the baseline were removed. No commit was made.

Campaign rounds 1–200 and the R36 installed candidate remain unchanged: no level, difficulty, economy, story-copy, artwork or gameplay-effect changes.

## Remaining verification and next step

Human difficulty for rounds 31–200, current Android build/device validation, the full-project suite and all-string/all-resolution visual acceptance remain pending. The optional returning-player recap is still a proposal. Manual export folders were not rechecked in this pass.

Next: check for current-content manual recordings, then continue the bounded bilingual reaction-copy review for rounds 61–70 while preserving level data and accepted scenes. Keep the small Victory caption as an explicit readability follow-up. Do not expand beyond round 200 automatically. Animations and effects remain the final phase.
