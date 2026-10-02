# Victory caption readability — R38 test results

2026-09-29. **14 Unity tests passed; 0 failed; 0 skipped in the final runs.**

[Open the actual Unity capture review](Victory_Caption_R38_Review.html).

| Verification | Result | Evidence |
| --- | --- | --- |
| Canonical prefab, art and idempotent rebuilding | 3 passed | [Edit Mode XML](QA/Verify03/EditMode/results.xml) |
| Full Victory runtime suite | 11 passed | [Play Mode XML](QA/Verify05/Victory/results.xml) |
| Serialized identities | All 362 preserved; revision 11 | [Source checks](QA/SourceChecks.json) |
| Scoped patch reverse check | Passed | [Exact patch](Sources/Changes.patch) |

The caption font increased from 14 to 34 reference pixels. Reaction, reward-bonus explanation and status text reuse a larger wrapping area above Continue, with pale text over navy. On the 576-pixel-wide phone capture the type increases from about 8.6 to 20.8 pixels. The smallest verified 360 × 640 safe-area fixture retains about 11.3 pixels. These are rendering measurements, not physical-device readability certification.

Verified both Russian and English at 360 × 640, 576 × 1280 and 941 × 1672. All installed victory reactions were checked for vertical text fit in both languages. Native button raycasts, safe areas, resource/booster containment, unavailable-video status, reward idempotency, current score, replay and language switching pass. The new caption test preserves player state, excluding only the independent energy-clock timestamp.

Final captures include [English phone](QA/Verify05/Victory/Journey/Caption_en_576.png), [Russian small phone](QA/Verify05/Victory/Journey/Caption_ru_360.png) and [English status](QA/Verify05/Victory/Journey/Caption_Status_en_360.png). Final English 576 and Russian 360 captures were inspected, along with earlier final-layout Russian 576, status and full reward cases. Captures use Unity service-result fixtures and simulated display/safe areas; they are not Android screenshots or manual wins. Existing runtime coverage also includes a real board-win callback.

Initial verification caught and resolved a duplicate background on rebuild and touching footer button bounds under fractional scaling. An existing test captured the last authored round before its star reveal and still called that image round 10; it now waits for the reveal and labels the capture correctly. No animation code was changed. Failed-run evidence is retained in QA/Verify01–Verify04; final passing evidence is EditMode Verify03 and Victory Verify05.

[Scope](QA/Scope.json): six existing project files changed, zero added/deleted and 7,665 baseline files unchanged. The active prefab and its owning style/runtime builder are updated together. Source and serialized revision agree at 11. Existing sprite assets, fonts, wording, campaign data, services and settings are unchanged. Generated test scenes/linker output were cleaned against the task-start baseline. No commit was made.

Unity 6000.5.9f1 used isolated save stores and the previously authorized process-only Vulkan backend/descriptor limit 4096. No Android build/device run or full-project suite was performed. Difficulty testing and gameplay effects were outside this caption correction.

[Implementation details](ImplementationNotes.md) · [Verification summary](QA/Verification.json).
