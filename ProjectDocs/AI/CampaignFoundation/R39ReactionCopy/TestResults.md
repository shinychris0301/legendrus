# Campaign R39 — reaction text for rounds 61–70

2026-09-29. **49 Unity tests passed; 0 failed; 0 skipped.**

Installed 20 distinct outcome reactions per language for rounds 61–70: **40 localized values changed**. The normal campaign remains rounds 1–200.

| Suite | Passed | Evidence |
| --- | ---: | --- |
| Installed resources and story | 16 | [XML](QA/Verify01/Editorial/results.xml) |
| Checkpoint lifecycle | 32 | [XML](QA/Verify01/Checkpoint/results.xml) |
| Bilingual Victory caption fit and state preservation | 1 | [XML](QA/Verify01/Caption/results.xml) |
| **Total** | **49** | [Verification](QA/Verification.json) |

Source/data checks confirm:

- Exactly 20 Russian and 20 English reaction values changed. Existing keys, speakers and the other 612 entries in each locale are preserved; all 20 revised texts are distinct within each language.
- All 632 required bilingual keys remain populated. The new Chapter01EditorialR39 candidate matches installed levels/story/locales exactly; historical candidates remain intact.
- All 200 levels, campaign definition and story JSON are byte-for-byte unchanged. No move budgets, seeds, goals, obstacle layers, reward values, milestone order or acknowledgement keys changed.
- Repeated reaction occurrences beyond the first decreased from 119 to 99 in Russian and 123 to 103 in English. Repetition in rounds 71–100 remains; this pass does not claim complete editorial acceptance.

[Copy review](CopyReview.md) · [Applied text and exact mechanic evidence](AppliedCopy.json) · [Copy audit](QA/CopyAudit.json).

The text was reviewed against each round's goals, crate/chain durability, delivery columns and surrounding story scenes. Round 66 correctly has no obstacles; round 67's survival goal uses consumed moves, not a timer. The accepted scene after round 70 remains unchanged. Narrative hints do not add new mechanics or guarantee a winning strategy.

The caption regression checks installed victory strings in both languages for wrapping in the R38 writing area, then verifies native layout/input at 360 × 640, 576 × 1280 and 941 × 1672, including unavailable-video status. Player state is preserved, excluding only its independent energy-clock timestamp. Captures in [Journey](QA/Verify01/Caption/Journey) use the existing round-51 service-result fixture; the English phone capture was inspected. They do not visually demonstrate every new round-61–70 reaction. Failure text retains its existing display rules and does not replace booster help.

The resource suite includes 600 deterministic legal starts without opening matches. These are not 600 new solution runs, manual playthroughs or human difficulty evidence. Checkpoint tests cover persistence, acknowledgements, language lookup and progress/reward isolation.

Unity 6000.5.9f1 used isolated saves and the previously authorized process-only Vulkan backend/descriptor limit 4096. No project settings were changed. All three suites passed on the first verification run.

[Scope](QA/Scope.json): three existing project files changed (two active localization JSONs and the existing candidate-parity test path); nine candidate/meta files added; zero deleted; 7,668 baseline project files unchanged. [Source checks](QA/SourceChecks.json) confirm Python syntax, scoped test patch checking and 3,590 protected runtime/UI/settings/package files unchanged. Generated test scenes/linker files were cleaned against the task-start baseline. No commit was made.

A fresh check of the default manual-export folder found five valid recordings covering only rounds 11–30 and no recordings for 31–200. External paths were not searched. [Manual evidence](QA/ManualEvidence.json).

**Next:** review new current-content manual recordings if they arrive; otherwise continue the bounded reaction-copy review for rounds 71–80. Human 31–200 difficulty, current Android build/device, full-project suite and optional returning-player recap remain pending. Animations/effects remain the final phase.
