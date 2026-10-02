# Characters R1 — reference fidelity review

Reviewed 2026-09-18 at the user's request. **The broad composition matches; the implemented page is not yet a close visual match.** Functional verification does not establish reference fidelity.

## Evidence examined

Original `Sources/OriginalReference.png` (941×1672), compared visually with actual Unity Verify04 `Characters_941x1672_RU.png`; existing safe-area `Characters_576x1280_safe_EN.png` also inspected. Read current owning runtime, view/builder records, extraction/cleanup script, provenance and prior verification report. Original copy SHA256 matches the Downloads original; all14runtime PNGs match their recorded R1 hashes. Relevant code, prefab and cutover timestamps are no later than the recorded Play Mode run. These support use of existing captures but are not a fresh runtime test. [Recorded checks](ReferenceReviewChecks.json).

## Differences

| Area | Observed difference | Significance / next correction |
|---|---|---|
| Main Ivan and Firebird | Reference has a closer, darker portrait in a warmly lit sunset town. Current page reuses a smaller-faced Home hero, brighter red/blue clothing and a bright orange Firebird over a dark forest. The subject ends in a conspicuous straight horizontal cutoff above the frame bottom. | High. Separate the original visible hero/Firebird through Scenario and obtain a clean sunset environment only where extraction cannot recover it. Fit the subject to the portrait window so its lower cutoff is hidden by the frame. Do not alter shared Home assets. |
| Detail-panel structure | Five stars, blue rarity plaque, level/progress strip, stat icons/dividers, circular skill medallions and team-bonus structure are absent. A biography and preview messages take their place. Current panel face is a much flatter brown field. | High. Restore the reference's visual grouping and textures, with explicitly preview/unavailable states for unimplemented data. Existing boosters are not equivalent to character abilities. Do not display invented levels, earned stars, stats or bonuses. |
| Collection cards | Portrait identities largely match because they come from the source, but the reference's scenic interiors, corner class/emblem badges, stars and selected-card lighting are missing. Repeated near-black/brown surfaces make the collection look flatter. Busts terminate in straight cutoffs. | High. Rebuild cards as independent background/frame/portrait/emblem/text layers; blend or conceal the bust baseline within the card. Any rarity-star examples must be labelled concept previews, not ownership or upgrade levels. |
| Card proportions | Source extraction records put the first card at approximately y848 with a222×241region. Current builder places it at y882 and208×218. The added collection label contributes to the lower start. Source locked cards extend roughly150px vertically; current future cards are104px high, lack silhouettes, and leave room for an extra information bar absent from the reference. | High. Restore taller card faces and the original vertical rhythm. Source figures are approximate visual/source-region bounds; current figures are exact builder dimensions. Move preview/help disclosure into a less intrusive presentation without hiding it. |
| Footer | Reference has large round medallions, a sculpted ornamental backing and a guild crest. Current footer uses smaller square Home frames and a helmet for Guild/Team. It sits lower and leaves a relatively empty dark band. | High. Extract separate source navigation medallions/backing and preserve existing real routes. Only show an event alert badge when supported by state; reference decoration is not evidence of a real notification. |
| Typography and title | Reference lettering is more slender, with gold shading/bevel on the title. Current lettering is heavier, flatter and cream-colored throughout; names and explanatory labels crowd the available hierarchy. Title face is largely uniform navy after text clearing, and surrounding carved scrollwork is missing. | Medium/high. Keep live localizable text and the approved font choices, tune size/weight/spacing/color and restrained shadow treatment; restore clean surrounding ornament. Exact source font remains unverified. |
| Header and controls | Shared header keeps the broad avatar/resources/Settings arrangement but omits the small experience strip under the player name. Avatar/frame details and font weight differ. The source Upgrade/Select controls are taller and brighter; current controls are smaller, lower and dimmed by their unavailable state. | Medium. Restore layout/decoration locally where data permits; disabled styling is intentional. A live progress strip requires real defined progression, or an explicitly unavailable visual state. |
| Scene/frame finish | Overall navy/gold family is present, but the source has dense carved perimeter ornament and textured panel faces. Current clean forest and simplified brown panel surfaces create a different material balance. Some narrow colored remnants remain along the portrait/card frame's upper edge after interior cleanup. | Medium. Inspect source-derived borders at full resolution and repair those bounded remnants; replace broad flat fills with coherent clean texture without importing neighboring scenery or text. |

## What already matches

The two-column hero/detail arrangement, three-tab order, eight-character order, four-column collection grid, gold frame language, blue accent selection and major navigation destinations follow the reference. Individual source portrait identities and visible source border pixels were retained. This is useful structural progress, but it does not offset the major artwork and hierarchy differences above.

## Intentional functional differences

The reference's level28, currency totals, energy25/30, combat stats, skill levels and team bonus are concept data. Current header values come from a saved test profile. Hero ownership/upgrades/combat stats do not exist, so disabled actions, em dashes and preview disclosures remain appropriate. Restoring visual structure does not require activating those features. Existing deferred hero-service task remains separate; this review does not authorize new gameplay rules.

## Recommended next revision

**R2 — detail-panel and collection-card fidelity.** Restore the blue rarity plaque, stat symbols/dividers and ability-section grouping; restore card proportions, textured faces, source emblems and a clearer selected border. Keep all unsupported values and abilities explicitly unavailable or labelled as concepts, and retain live text. Reduce repetitive disclosure text and integrate the information action more discreetly while keeping it discoverable. Use the established Scenario extraction-first workflow for the source elements that are not already available; only rebuild obscured surfaces.

Then address the main hero/sunset plate and round footer medallions. These are the next largest differences and should be completed before describing the page as a close replica. This is a recommendation, not a claim that R2 is already implemented.

## Checks performed / not performed this turn

Performed: original-versus-RU capture visual comparison; existing narrow safe-area EN capture inspection; relevant layout/art-source inspection; source copy hash;14runtime asset hash comparisons; code/prefab/cutover timestamp check against existing capture run; prior2Edit+5Play result readback. Project root/version and current dirty tree inspected.

Not performed: new Unity import/build/tests/capture, new browser run, APK, physical-phone/touch checks, Scenario calls, image edits or production changes. Earlier7passing tests remain historical evidence and are not claimed as rerun. Only this report/check JSON and CurrentTask/SessionLog were updated.
