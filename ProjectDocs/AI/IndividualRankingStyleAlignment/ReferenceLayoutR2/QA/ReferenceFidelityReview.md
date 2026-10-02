# Individual Ranking R2 — detailed reference-fidelity review

The composition is substantially closer than R1, but R2 does **not** fully match the original. The earlier description of the remaining differences as “slight” understated visible extraction and finishing defects. This is a review of saved actual Unity R2 captures against the supplied original, not a new runtime capture or a claim of user acceptance.

## Evidence checked

- Original: `/home/k2/Downloads/match3final/individual ranking.png`,941×1672; SHA25656ec682e5e5982688789ce43afa05bbd4c51768a943829ecb2331d37b9164611. Its stored copy matches.
- Actual Unity run3 captures:941×1672 Russian and English. Capture metadata identifies Unity6000.5.9f1/OpenGLCore and an isolated save.
- Current canonical prefab revision2, owning builder, page reference-layout helper, view and cutover state.
- All41 installed PNG hashes match R2 provenance. Inspected the stored magenta alpha sheet and the runtime title sprite directly.
- Reviewed the local preparation masks/processing to distinguish extraction damage from layout problems. No new API calls were needed.

## Findings, in priority order

| Priority | Element | Visible discrepancy | Supporting implementation evidence / recommended correction |
|---|---|---|---|
| High | Outer reward and table panels | Gold borders and corner scrollwork are much weaker and partly fragmented. The reward section reads as a flat rectangular face instead of the framed reference panel. | `PrepareFinal.py` keeps only warm-colored border pixels outside a12px opaque inset. That can discard dark metallic shading as well as unwanted scenery. Restore the full ornamental silhouette and its dark support, preserving the opaque surface; review mask quality before rebuilding. |
| High | Title surface | The supposedly blank title texture still contains dark silhouettes of the original Russian lettering. These are visible when inspecting `title.png`, independent of native text. | Original label removal targeted bright gold strokes and left shadows. Repair the complete glyph footprint, including dark shadows, without touching the crest/rails; keep live localizable text separate. |
| High | Portrait edges | Solovey and Vodyanoy visibly end in horizontal cuts. Zmey also has an explicit bottom cutoff. Several lower portraits look reduced within their rows. | The final alpha script explicitly zeros pixels below source cut lines. Reconstruct only the missing contour/portrait-frame area when needed and normalize visible subject size; do not stretch or regenerate every portrait. |
| Medium–high | Rank medals | First/second/third centers and numbers look flat; laurel edges are thin and incomplete compared with the reference’s sculpted metal. | Central discs were repainted to remove numbers and laurels were selected by color thresholds. Preserve more relief/shadow around the live number and refine metallic edges. |
| Medium | Row assets and alignment | Purple reward chests are smaller than the reference examples. Six-digit power values finish farther left than seven-digit values. Gap dots sit too close to the highlighted row and the final row border. | Portrait/reward image heights depend on row height while preserving aspect, which reduces their visible width in short rows; power text uses `MiddleLeft`. Adjust per-sprite visible bounds, right alignment and separator vertical placement. |
| Medium | Typography | Heading and row lettering are broader/flatter and less engraved than the original; this changes density even where positions agree. | Existing live serif typography remains an approximation; exact font is still unidentified. Tune metrics, weight and restrained relief after asset repairs. Do not distribute an unlicensed substitute or bake dynamic values into images. |
| Medium | Footer | Original has a full-width carved dark decorative base around Back/Rules. R2 exposes forest and uses a narrow reused ornament strip with a central sapphire. | Layout reuses `ShopAligned/footer_ornament` at774×32 and omits the composite source footer. Extract/repair independent footer ornamentation without importing scenery/UI contamination. |
| Lower priority | Header and scenery | The header portrait, capsule treatment and absence of an XP bar differ. Birch-tree placement and lighting also differ. | Shared header was intentionally retained; no real XP field supplies a valid fill. Scenery is a separately generated clean plate. Preserve live balances; do not fabricate progress or claim original hidden scenery was recovered. |

## What already follows the reference

The major vertical arrangement, broad title, Day/Week/Season tabs, three top prize cards, four table columns, fourteen illustrated rows, distinct character/reward identities, blue highlighted sample Ivan row and positions of Back/Rules are recognizable and broadly aligned. Sample prize quantities and ranking values follow the illustration. These improvements do not eliminate the defects above.

## Intentional data differences

Live wallet/level values differ from the reference’s example numbers. The preview label and unannounced-season note replace the illustrated countdown because no authoritative leaderboard/season exists. These are truthful state differences, not defects to fix by hard-coding reference values. An actual countdown/XP bar/reward service belongs to feature development.

## Recommended next revision

**R3 — extracted-asset repair:** clean the title’s residual lettering, restore reward/table frame silhouettes and dark metal shading, and repair cut portrait edges. Match row portrait/chest visible size and separator placement in the same focused layout check. Retain approved source artwork and live text; use Scenario only for bounded missing regions that cannot be recovered locally. Do not regenerate the whole screen.

A later pass can refine medal relief, typography and the full footer. Compare fresh Unity RU/EN captures after changes. No revision was implemented by this review request.

## Checks performed / not performed

Performed: project instructions/current task/handoff and dirty-tree inspection; recorded Unity version check; original/copy hashes;41 current asset hashes; saved Russian/English Unity capture comparison; alpha-sheet and title-texture inspection; prefab/builder/cutover/layout/view/processing review; read historical test XML. Production Assets remained unchanged during this audit.

Historical results remain run2:4 EditMode+11 PlayMode passed; final focused run3:2 EditMode+6 PlayMode passed. They establish prior behavior/layout checks, not visual equivalence. No tests were rerun today for this review.

Not performed: fresh Unity launch/compile/rebuild/capture, APK build, phone test, full suite, browser execution, profiling, calibrated color measurements, exact-font identification, Scenario requests or credit spend. No production images/code/prefabs/settings changed. Updated only this review, its index link, CurrentTask and SessionLog.
