# Campaign R40 — reaction text for rounds 71–80

2026-09-29. **49 Unity tests passed; 0 failed; 0 skipped.** Installed 20 distinct reactions per language: **40 localized values changed**. Campaign remains rounds 1–200.

| Suite | Passed | Evidence |
| --- | ---: | --- |
| Installed resources and story | 16 | [XML](QA/Verify01/Editorial/results.xml) |
| Checkpoint lifecycle | 32 | [XML](QA/Verify01/Checkpoint/results.xml) |
| Bilingual Victory caption fit and state preservation | 1 | [XML](QA/Verify01/Caption/results.xml) |
| **Total** | **49** | [Verification](QA/Verification.json) |

[Copy audit](QA/CopyAudit.json) confirms exactly 20 Russian and 20 English reaction changes, with existing keys/speakers and the other 612 entries per locale preserved. All 632 required bilingual keys remain populated. The new Chapter01EditorialR40 candidate matches installed levels/story/locales exactly; historical candidates remain intact.

All 200 levels, definition and story JSON remain byte-for-byte unchanged. No move budgets, seeds, goals, obstacle layers, rewards, milestone order or acknowledgement keys changed. Repeated reaction occurrences beyond the first decreased from 99 to 79 in Russian and 103 to 83 in English. Rounds 81–100 still need editorial review.

[Copy review](CopyReview.md) · [Applied values and mechanic evidence](AppliedCopy.json).

Round 73 distinguishes cracking two-layer ice from fully removing it; the goal runtime counts destroyed obstacles. Round 75 mentions the stronger chains beneath the feathers. Round 78 checks all three obstacle types. Round 79 avoids describing ice as an immovable crate. The existing courtyard milestone after round 80 is preserved. These hints do not add mechanics or guarantee a winning strategy.

The caption regression checks installed victory strings in both languages for wrapping in the R38 writing area. It also verifies native layout/input at 360 × 640, 576 × 1280 and 941 × 1672, unavailable-video status and state preservation, excluding only the independent energy-clock timestamp. The [Russian phone capture](QA/Verify01/Caption/Journey/Caption_ru_576.png) was inspected. Screenshots use the existing round-51 service-result fixture, not every revised reaction, a manual playthrough or an Android device. Failure captions retain their existing visibility rules and do not replace booster help.

Resource checks include 600 deterministic legal starts without opening matches, not 600 completion runs. Checkpoint tests cover persistence, acknowledgements, language lookup and reward/progress isolation. All three suites passed on the first verification run in Unity 6000.5.9f1, using isolated saves and the previously authorized process-only Vulkan backend/descriptor limit 4096. Project settings were unchanged.

[Scope](QA/Scope.json): three existing project files changed (two active locales and the existing candidate-parity test path); nine candidate/meta files added; zero deleted; 7,677 baseline files unchanged. [Source checks](QA/SourceChecks.json) confirm Python syntax, scoped test patch checking and 3,590 protected runtime/UI/settings/package files unchanged. Generated test scenes/linker output were cleaned against the task-start baseline. No commit was made.

A fresh default-folder check found five valid manual recordings for rounds 11–30 and none for 31–200; external paths were not searched. [Manual evidence](QA/ManualEvidence.json).

**Next:** inspect new current-content manual recordings if available; otherwise continue the bounded reaction-copy review for rounds 81–90. Human 31–200 difficulty, current Android build/device, full-project suite and optional returning-player recap remain pending. Animations/effects remain the final phase.
