# Chapter Map — current implementation assessment

Reviewed 2026-09-20 against `/home/k2/Downloads/match3final/chater map.png`, with fresh Unity6000.5.9f1 captures from the active canonical ChapterMap prefab (revision1). The approved reference copy in Assets is visually the same composition but has a different file hash. The Downloads original is copied into `Sources/OriginalReference.png` for this review.

**Verdict: functioning, but it does not yet match the original or the rebuilt Home/World Map presentation.** This is an assessment; production files were not changed.

| Area | Current evidence | Recommended correction |
|---|---|---|
| Top navigation | Older portrait/frame/resource presentation and fonts. No ProductionPlayerHeader component. Still uses the legacy ProductionNavigationStyle branch. | Bind existing controls to the shared Home/WorldMap header; retire the older layout override for this page. Preserve Profile/Settings/resource routes. |
| Chapter title | Recreated symmetrical red banner with different rolled ends, crest, lower ornament and bevelled live lettering. | Extract original banner together with its Russian lettering. Provide a clean face/live translated caption for English; later chapter names need dynamic/individual variants. |
| Level markers and stars | Recreated circular frames, thin empty-star outlines and different filled-star facets. Small lock badges and weak selected-level halo. | Extract original medallion and full/empty stars; keep numbers and earned-star state live. Make selected, completed and locked states clearly distinguishable. |
| Density and path | All ten authored levels are compressed onto one static screen, roughly85reference-pixel vertical steps; no ScrollRect. Reference displays seven larger-spaced markers. Responsive node spacing and background cover scale independently. | Use a scrolling path showing about six or seven nodes at once, starting near the selected level. Keep nodes and painted path registered together when scrolling/resizing. |
| Village scenery | Church, windmill, houses, stream and lighting are recreated with substantially different positions and scale. | Preserve visible source scenery and use Scenario only to clean/reconstruct areas covered by original UI; create a clean environment plate. |
| Play, Back and footer | Different emerald button proportions/filigree, generated Back medallion, and a shallow dark footer strip. Center ornament reaches the bottom edge more tightly than the reference. | Extract original Play lettering/button, Back medallion/arrow and carved footer with correct alpha and proportions. |

## Functional checks

Final targeted run: **5/6 passed** —2Edit Mode passed,1Edit Mode failed, all3Play Mode passed. The failed `HomeHudResourcesAndTypographyAreSharedWithoutChangingHome` assertion expects Home's `source_header` sprite but ChapterMap still uses `header`. Do not weaken the test to conceal this discrepancy; migrate the header and rerun it. Other same-header assertions later in that test were not reached.

Passing Play Mode tests exercised real unlock state, locked taps without save mutation, earned stars, selected-level persistence, Back to WorldMap, Play to story/PreLevel, live resource values and five header actions. Captures checked visible button raycasts, responsive bounds and star-row/frame spacing. RU, EN, RU roundtrip, two tall-safe layouts and earned-progress fixture were captured; RU/reference,576×1280safe and earned-progress were directly inspected.

Chapter1 has **ten authored levels**. The example's labels10–16 must not be treated as proof those levels exist. A fresh save correctly opens level1 with no stars; the progress fixture completes level1 and selects level2. Later chapter content is not supplied by this screen.

No asset generation, Scenario API requests, prefab rebuild or production-code edit occurred during this assessment. All Assets hashes are unchanged (`QA/ScopeAudit.json`). No APK, device or full-suite test.

## Next implementation

Start **Chapter Map Reference Rebuild R1 — Shared Header and Original Controls**: migrate the shared header; extract the original red banner/lettering, Play/Back/footer, medallion and stars. Then align a scrollable path and clean village plate while preserving the ten-level catalog and existing story/PreLevel behavior. Use the approved extraction-first Scenario workflow for needed artwork, avoiding regeneration of intact original UI elements.
