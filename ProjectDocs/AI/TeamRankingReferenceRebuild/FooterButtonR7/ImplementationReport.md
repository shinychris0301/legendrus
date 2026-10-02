# Team Ranking R7 — Footer Button Extraction

The user reported that the footer button was not extracted correctly. Inspection found that the Russian sprite used a smaller581×142 source crop and a hand-traced brightness-threshold mask, which removed dark detail and cut through ornate side curls. English already used a cleaner Scenario-derived isolation registered to593×150, but its runtime rectangle stretched it into the smaller Russian proportions.

Re-extracted the original Russian button and lettering at(180,1458)–(773,1608), retaining source RGB after uniform3×upsampling and reusing the existing approved English alpha exactly. Both languages now use593×150 native bounds. The existing English blank sprite is unchanged; its live caption is centered vertically within the full face. The button sits64referencepixels above the safe bottom and remains clear of Back. No new Scenario requests or credits: the correct source-registered isolation already existed.

## Production scope

Seven existing Assets files changed: native owner/reference layout, view, builder, actual active TeamRanking prefab, cutover and two existing test files. Internalrevision8 agrees across builder/view/prefab/cutover. One PNG+meta added; zero deletions. Existing PNGs/metas/fonts/otherpageprefabs preserved by SHA audit. R6 tabs, R5 podiums, shared header, approved rewards, table and routing/save semantics are unchanged.

## Verification

Unity6000.5.9f1 **Verify01**: rebuild/readback passed; **13EditMode +15PlayMode =28passed**, zero failures/skips, including TeamHub/IndividualRanking regression. Checks include identical sprite/layout aspect ratio in RU/EN, safe-bottom clearance, button raycasts, Details modal/back/input shielding, routes and save preservation, plus earlier podium/table/tab constraints.

Nine actual TeamRanking captures were recorded at reference/narrow/tall simulated sizes, in RU/EN and interaction states. Final Russian/English footer close-ups and narrow Russian page inspected; alpha inspected over black/white/magenta. Browser review:11views/comparison passed at430×900 without errors/overflow. Source RGB and reused alpha equality asserted in the extraction script.

No Android build, physical-device check, performance or full-suite run. The reused isolation is not claimed to be a mathematically exact contour. English face/text and unrelated footer scenery/banners/candles remain different from the original. This pass corrects the button extraction only.

[Open review](TeamRanking_FooterButton_R7_Review.html) · [Fidelity](QA/ReferenceFidelityReview.md) · [Alpha](QA/AssetAlpha.png) · [Scope/checks](QA/ScopeAndVerification.json) · [Provenance](Sources/AssetProvenance.json)
