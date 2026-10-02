# R2 fidelity recheck — 2026-09-23

**Verdict: the main composition is substantially closer, but this is not yet a faithful finish.** Compared the original and the stored final Verify03 Russian Unity capture at the same 941×1672 size; inspected the Firebird black/white/magenta alpha sheet and current runtime placement code. This was a read-only implementation audit, not a new Unity capture or test run.

## Priority findings

| Priority / area | Visible discrepancy | Recommended correction |
| --- | --- | --- |
| High — Firebird extraction | Left feather strands have broken holes, rough dark/light outlines and missing blue eye details. A straight cutoff remains beneath the body/feet, just above the countdown. The strongest original starburst highlights are also weaker. | Repair the alpha against the original feather contours, retaining non-gold feather colours. Remove the straight exposed cut and align the bird/countdown overlap. Restore original emission highlights on a separate layer. Do not regenerate the whole bird. |
| High — countdown and feather balance | The countdown is a nearly rectangular navy bar instead of the ornate shaped gold plaque. The separate gold-bordered feather-balance inset has become an almost unframed navy area. | Extract the original borders independently and preserve opaque faces, with live unavailable-date/count text inside. |
| High — premium-pass reward enclosure | The gold inner frame enclosing the chest/gem/feather/pass items is absent. The chest is smaller and the examples float on the purple surface. | Restore the inner enclosure and original relative item scale/spacing, retaining the preview notice and disabled purchase. |
| High — footer | The continuous dark carved surround behind the bottom controls is missing; exposed Home scenery replaces it. Side circles are smaller and use different rims/art. | Restore a separately isolated carved footer and source circle proportions. Keep functional Back instead of introducing an unsupported mail action; retain the passive reward example. |
| Medium — leaderboard | Portraits are smaller; row surfaces/borders are different, and the separate preview note pushes the row block down. The repeated Vasilisa portrait does not reproduce the original Lyudmila portrait. Rank medallion silhouettes are absent. | Restore source row art, portrait proportions and appropriate sample portraits; integrate the preview notice without losing row hierarchy. Any rank-medallion examples must remain explicitly labelled previews. |
| Medium — chest/reward sizing | Chest presentation is normalized into similar boxes. The fifth ornate chest loses the stronger size emphasis in the original; potion and Firebird-medal artwork occupy less of their cards. | Measure individual source bounds and restore their relative sizes instead of applying the same visual scale to all assets. |
| Medium — typography and disabled buttons | Section headings are lighter/thinner than the reference. The Activate face is much darker and flatter; Play is dimmed and carries an added preview caption. | Tune live heading weight/position. Preserve clear disabled semantics while retaining more of the original gold/green face detail. Do not enable unimplemented actions to achieve a visual match. |

The title/logo plaque and original Russian Play/Activate letterforms are the closest areas. Major horizontal section positions and the five-stage reward structure follow the source. However, a good source sprite is not sufficient by itself: runtime tint, scale, mask quality and surrounding ornament also affect the result.

The Home environment/shared header are approved intentional differences. Actual saved resource values, unknown event dates/counts, empty progress, preview text, and disabled participation are data/function constraints, not reasons to copy the reference's example statistics into production. English remains live/localized; the exact original font is not established.

**Recommended next pass: Event Detail R3 — Firebird Edges and Panel Ornamentation.** First repair the bird silhouette/overlap, countdown plaque, feather-balance frame and premium inner frame. Then restore the carved footer and refine leaderboard/reward proportions. This recheck authorizes no new Scenario spend or runtime edits by itself.

The earlier 14 targeted Unity tests establish behavior/layout assertions, not image fidelity. In particular, the earlier note that the countdown fully conceals the bird cutoff was too optimistic: the visible straight edge above it remains a defect.

---

# Event Detail R2 — Reference fidelity

Compared the dedicated [original](../Sources/OriginalReference.png) with final Verify03 native Russian at 941×1672 and English at 576×1280 with safe-area insets. The page now follows the original's main composition, but is **not an exact pixel match**.

| Area | Result and remaining difference |
| --- | --- |
| Header and environment | Actual shared Home header/background retained by user preference; these deliberately differ from the original Event Detail scenery and example balances. |
| Title | Original Russian lettering, logo and plaque proportions restored. Fine alpha and a small repaired feather-overlap area are approximate. English is live typography, not an exact source font. |
| Firebird | Large free silhouette replaces R1's framed event scene. Original RGB and independent sparks/soft halo are used. Tail restored after initial isolation omitted it; fine feather edges remain uneven in places. Source occlusion at the countdown is concealed by the independent plaque, not presented as recovered hidden pixels. |
| Countdown/objective | Correct visual order and surfaces; live unconfirmed-date text replaces the reference's promotional countdown. Feather count stays unknown. |
| Milestones/rewards | Five original chest designs, complete rail diamonds and separate framed reward icons follow the original hierarchy. Empty progress, unknown thresholds and quantities are intentional until event configuration exists. |
| Leaders/pass | Original two-column layout and navy/purple surfaces restored. Sample leaderboard and pass are explicitly previews. The original inner pass-item frame and carved footer surround are not fully reproduced. Native labels/rows differ from source lettering and example values; no real ranking or purchase is implied. |
| Bottom controls | Original Russian Play/Activate art and text retained; both actions disabled. Existing functional Back and a passive reward example replace the reference's side controls. The right circular rim is reused approved art rather than an exact extraction of that reference control. |
| Tall phones | Header follows the safe top; Back/Play/reward follow the safe bottom. Extra vertical space shows the approved Home scenery. This is responsive behavior, not a stretched reference screenshot. |

Final native captures show the restored hourglass, potion, emblem and premium chest without the earlier incomplete masks. Frame interiors retain their intended opaque surfaces. There is no separate rectangular backing around the Firebird, though its fine feather alpha needs further polish for a closer match.

All 14 targeted Unity checks passed; the review's nine image views and comparison toggle passed. These checks do not establish exact visual identity. No Android/device verification was performed.
