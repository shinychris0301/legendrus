# Failure R1 — reference fidelity review

Reviewed 2026-09-15 at the user's request. This is a review of Visual Page 09, not approval or a new implementation.

## Verdict

The page uses the intended navy, antique gold and emerald material family and follows the broad HUD / Ivan / outcome / goals / actions / boosters sequence. It is **not yet a close visual match** to the original Failure reference. Functional test passes do not establish visual fidelity.

## Evidence

Original: `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Fail.png` (941×1672).
Retained actual Unity captures inspected in `../UnityVerification/Fail/`: `Fail_941x1672_RU.png`, `Fail_Level10_ThreeGoals_RU.png`, and `Fail_Level10_ThreeGoals_576_EN.png`. These are prior Unity captures, not fresh renders or generated mockups. The three-goal capture avoids treating the early two-goal mission rule as a design defect.

All 1,799 recorded production hashes and all 131 recorded evidence hashes match the current files, with zero changed or missing entries. This establishes continuity with the retained verification run, not a new runtime test.

## Differences

| Area | Observed difference and implication |
| --- | --- |
| Ivan and scenery | Ivan's face, red cloak and blue tunic are substantially brighter; his hair has a hard pale/orange outline. The reference has deeper body shadows with more selective warm rim light. Pose, costume details, castle placement and smoke/ember distribution differ. The current hero reads more like an isolated illustration than a figure integrated into the burning scene. |
| Outcome title and frame | The reference has thicker engraved gold lettering, a larger sapphire crest and a more sculpted title silhouette. R1's live title is flatter and paler, with a straighter, simpler panel. The tall outer frame also has different ornamentation and a more exposed textured interior. |
| Goals | R1's icons are smaller, its heading sits close to the upper carving, and the extra level/score line changes the hierarchy. Even in the three-goal capture, the reference's larger icons and distinct heading band are not reproduced. Keep actual failed-goal types/progress rather than copying sample gems or counts. |
| Retry | The emerald control is appreciably taller and more ornate than the reference's slim action. Source inspection confirms preserved aspect ratio: this is a source-shape mismatch, not sideways stretching. A future correction needs a naturally proportioned Failure-specific surface; do not distort or replace the accepted shared Victory R7 asset. |
| Secondary actions | R1 has a chapter-map return and story line. The reference instead has two substantial extra-moves/ad continuation offers. This is an unimplemented feature difference, not simply missing decoration: the existing Failure flow does not support those transactions/resume paths. Do not add misleading controls merely to imitate the reference. |
| Boosters | R1's four illustrations are much smaller and unframed; quantity badges appear detached and labels lack the reference's framed plaques. The reference's larger circular medallions create a stronger, more coherent lower row. Current runtime presentation fits art to 102×102 design units, explaining its weak visual weight. Keep the existing booster artwork, actual stock, names and functions. |
| Narrow-phone readability | The EN576 capture exposes very small booster names/story text and a Russian story fallback within the English page. The lower label row sits close to the outer carving. Main title and counts remain legible in this capture; there is no basis here to claim a font-import or texture-blur defect. |
| HUD | Broad structure is close, but portrait/frame, resource shapes and proportions differ. Saved level, energy and wallet values must remain truthful. The displayed 12,540 is wallet gold, not the failed round's score. |

## Recommended next focused refinement

Start with **Failure R2 — booster row and lower-panel readability**: larger coherent navy/gold medallion frames around the existing illustrations, quantity badges anchored to their frames, readable live label plaques, and adequate clearance from the bottom carving. Fit this within the current functional layout; preserve retry/chapter routes and existing booster identities. This addresses the strongest lower-section mismatch without introducing purchases or changing game rules.

Follow separately with title/crest depth and a naturally slimmer Failure-specific retry surface, then Ivan/scene integration. An exact visual match is not claimed, and user acceptance remains pending.

## Checks and limits

Performed: inspected project instructions, current task/historical handoff/provenance and working-tree state (206 existing status entries); confirmed project root and recorded Unity 6000.5.9f1; visually compared original and three retained captures; checked active Failure presentation source and canonical builder entry points; verified the 1,799 production and 131 evidence hashes. No nested AGENTS.md found. Historical handoff centers on Splash/Loading; the latest task record and local Failure R1 files establish the current page.

Not performed: new Unity compilation, prefab rebuild, EditMode/PlayMode tests or captures; browser interaction; APK/AAB or physical Android checks; manual animation/gameplay; calibrated color/blur measurement; exact font identification; inspection of every retained capture; new image generation or editing. The previous 10/10 EditMode and 18/18 PlayMode results remain historical.

Only this review, CurrentTask.md and SessionLog.md were changed. Production files and existing acceptance decisions were preserved.
