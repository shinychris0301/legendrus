# Characters R6 — reference fidelity review

**Verdict: the exclusive Ivan highlight is now correct; the page as a whole is not yet a close replica.** The two-column composition and collection/detail structure are substantially closer than R1, but prominent scene, typography and navigation differences remain.

Compared the original941×1672reference with the actual R6 Verify01 RU capture at the same size, plus the scrolled safeEN576×1280capture. Checked active frame assignments, extraction provenance and recorded verification. No production changes or fresh Unity run in this review.

## Resolved or substantially improved

- Only currentIvan has the bright source-frame illumination. Other heroes and locked slots use the normal frame extracted from Masha. The added rectangular outline remains removed. Source style and state distinction now agree with the reference.
- Rarity plaque, stars, empty level track, four stat rows, circular ability medallions and team-bonus grouping are present. These were major R1 omissions.
- Four-column cards have source portraits, corner emblems and stars. Their first row now starts around the reference position and uses comparable proportions.
- Main title and detail-button lettering are better aligned. The former flat title repair boundary and repeated detail-face texture are reduced, although the repaired surfaces still do not reproduce the original's full material detail.
- Full-height locked cards can be revealed by upward scrolling. Partial visibility at rest is the user's requested behavior, not a remaining clipping defect.

## Remaining differences, in priority order

| Priority | Area | Current versus reference |
|---|---|---|
| High | Main Ivan/Firebird and scene | Reused Home artwork has a different pose, smaller face relative to the window, much brighter red cloak/blue tunic and orange bird. The original has a closer, darker subject against a warm sunset town. Current dark forest changes the entire composition. A straight horizontal artwork cutoff and black strip remain above the portrait frame bottom; a colored source remnant is also visible along the top inside edge. |
| High | Footer and ornamental backing | Current square Home navigation frames differ clearly from the large circular source medallions. The helmet differs from the reference guild crest. Dense carved backing/perimeter decoration is largely absent, leaving broad dark areas. The source event badge should only appear if supported by real notification state. |
| Medium/high | Typography | Current title, tabs, names and stat labels are heavier and flatter; the reference uses slimmer shaded gold lettering. The title's bevel/depth is missing. Long names and preview labels crowd the lower card area. Existing approved fonts are approximations; exact source-font identity remains unverified. |
| Medium | Main panel placement and detail controls | Current main-panel top is lower (runtime323 versus source region around309), while its bottom remains near843, shortening the area. Buttons are also lower and shorter (runtime y775,height60 versus source extraction y758,height67). Their lower brightness is partly intentional disabled styling. The rarity badge/star grouping and ability/bonus spacing still differ. |
| Medium | Collection artwork and materials | Source portrait identities match, but one clean town plate is reused across cards, unlike the varied source backgrounds. Portrait blending and the dark name band differ. Smaller corner emblems and stars, simplified normal-frame inner corners, repaired faces and the added tiny preview captions change the detail/texture balance. |
| Medium | Top header | Broad avatar/resources/Settings layout matches, but the small experience strip under the player name is missing. Avatar/banner details and type weight differ. Currency, level and energy values must continue to come from actual save state; source values are not fixed design requirements. |

## Intentional data and interaction differences

Combat stats, hero levels, ability levels, team bonus and ownership/upgrades are not implemented. Em dashes, preview explanations and disabled controls should remain until their services exist. Reference stars/abilities are concepts, not granted progression. CurrentIvan remains active while browsing. The collection information link and scrollable full locked row are accepted implementation choices; they should not be removed merely to imitate a static screenshot.

## Recommended next refinement

**R7 — main hero scene and portrait-frame cleanup.** Use Scenario to isolate the reference Ivan/Firebird artwork, reuse visible source pixels, and reconstruct only obscured sunset scenery. Remove the remaining frame-edge residue and fit the subject so no straight lower cut is exposed. Use page-local resources; do not modify shared Home artwork. Keep the now-correct exclusive highlight, scrolling and unavailable hero states.

Then refine the circular footer medallions/carved backing and typography. No R7 implementation or approval is claimed here.

## Checks actually performed

- Visual comparison of original and R6 RU941×1672; scrolled safeEN capture inspected.
- Original copy matches the Downloads source hash;34recorded runtime sprite hashes match their R1/R2/R4/R6 provenance.
- Relevant code/prefab/cutover modification times precede the recorded R6 test completion, supporting use of those captures; this is not a new runtime verification.
- Read back existing2Edit+6Play=8passing results, with0failures/skips.
- Inspected workspace changes, recorded Unity6000.5.9f1 and owning runtime frame assignments.

Not performed this turn: new Unity import/build/tests/captures, new browser run, APK, physical-phone/touch tests, Scenario requests, image edits or production modifications. Only this review/check record, its HTML link and CurrentTask/SessionLog were updated. [Machine-readable evidence](ReferenceReviewChecks.json).
