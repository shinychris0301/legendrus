# Campaign R41 — reaction text for rounds 81–90

2026-09-29. **49 Unity tests passed; 0 failed; 0 skipped.** Installed 20 distinct reactions per language: **40 localized values changed**. Campaign remains rounds 1–200.

| Suite | Passed | Evidence |
| --- | ---: | --- |
| Installed resources and story | 16 | [XML](QA/Verify01/Editorial/results.xml) |
| Checkpoint lifecycle | 32 | [XML](QA/Verify01/Checkpoint/results.xml) |
| Bilingual Victory caption fit and state preservation | 1 | [XML](QA/Verify01/Caption/results.xml) |
| **Total** | **49** | [Verification](QA/Verification.json) |

[Copy audit](QA/CopyAudit.json) confirms exactly 20 Russian and 20 English reaction changes. Existing keys/speakers and the other 612 entries per locale are preserved. All 632 required bilingual keys remain populated. The new Chapter01EditorialR41 candidate matches installed levels/story/locales exactly; historical candidates remain intact.

All 200 levels, definition and story JSON are byte-for-byte unchanged. No move budgets, seeds, goals, obstacle layers, rewards, milestone order or acknowledgement keys changed. Repeated reaction occurrences beyond the first decreased from 79 to 59 in Russian and 83 to 63 in English. Rounds 91–100 still need editorial review; remaining repetitions elsewhere are not automatically rejected or rewritten.

[Copy review](CopyReview.md) · [Applied values and mechanic evidence](AppliedCopy.json).

The text reflects the two-color goal without obstacles in round 81, central two-layer ice in round 83, three delivery paths with thicker lower ice in round 85, left/right obstacle groups in round 88 and the stronger lowest crates in round 89. That last hint explicitly says “lowest” because the other crates along the feather columns have only one layer. Round 90 preserves the existing hearth milestone and its transition toward connecting the well. No new rule, timer or inventory reward is introduced.

The caption regression checks installed victory strings in both languages for wrapping in the R38 writing area. Native layout/input at 360 × 640, 576 × 1280 and 941 × 1672, unavailable-video status and player-state preservation also pass; only the independent energy-clock timestamp is excluded. The [English phone capture](QA/Verify01/Caption/Journey/Caption_en_576.png) was inspected. Captures use the existing round-51 service-result fixture, not each revised reaction, an Android device or a manual playthrough. Failure captions retain their existing visibility rules and do not replace booster help.

Resource checks include 600 deterministic legal starts without opening matches, not 600 completion runs or human difficulty measurements. Checkpoint tests cover persistence, acknowledgements, language lookup and reward/progress isolation. All three suites passed on the first verification run in Unity 6000.5.9f1, using isolated saves and the previously authorized process-only Vulkan backend/descriptor limit 4096. Project settings were unchanged.

[Scope](QA/Scope.json): three existing project files changed (two active locales and the existing candidate-parity test path); nine candidate/meta files added; zero deleted; 7,686 baseline files unchanged. [Source checks](QA/SourceChecks.json) confirm Python syntax, scoped test patch checking and 3,590 protected runtime/UI/settings/package files unchanged. Generated test scenes/linker output were cleaned against the task-start baseline. No commit was made.

A fresh default-folder check found five valid manual recordings for rounds 11–30 and none for 31–200; external paths were not searched. [Manual evidence](QA/ManualEvidence.json).

**Next:** inspect new current-content manual recordings if available; otherwise continue the bounded reaction-copy review for rounds 91–100. Human 31–200 difficulty, current Android build/device, full-project suite and optional returning-player recap remain pending. Animations/effects remain the final phase.
