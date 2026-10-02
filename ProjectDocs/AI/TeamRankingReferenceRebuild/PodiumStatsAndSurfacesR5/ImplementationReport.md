# Team Ranking R5 — Podium Stats and Surfaces

Implemented the user's report that members/scores for ranks 1–3 overflow and that podium background colors do not match.

## Cause and correction

The old text boxes were biased right; bronze's score box reached x866 beyond the inner rail near x848. Score boxes extended to y688, below the reward enclosure's y684 top. Member/score text now follows the original lanes, uses live left-aligned 24px text (minimum20 when needed), and clears the source pictograms, divider and rewards. Approved project fonts are retained.

Old extraction erased numbers with rectangular navy donor patches from the ranking table, including on bronze's brown lacquer. New podium variants restore each card's original opaque stat surface and pictograms; bounded numeric glyph masks remove the baked counts and their embossed shadows. Gold's redundant native member pictogram is disabled because its complete original is restored in the sprite. Original dividers and reward rails are protected. No Scenario generation or credits were needed.

24,991 visible source pixels outside the numeric repair masks have zero RGB mismatches before resizing; 11,201 masked pixels are locally interpolated. These are reconstructed hidden surfaces, not recovered original pixels. Existing isolation alpha and every pixel outside the stat-restoration areas are unchanged. R3 ribbons, crests, approved rewards and R4 table/name fit are retained.

## Production changes

- Runtime owner: `ProductionTeamRankingReferenceLayout.cs`; view revision6.
- Matching builder, real active `Resources/ProductionUI/Pages/TeamRanking.prefab`, and cutover revision6 rebuilt and read back.
- Three new podium PNGs and metas; seven existing Assets files modified, zero deletions. Existing PNGs/metas/fonts and every other page prefab are unchanged according to the before/after SHA audit.
- Shared header, live resource values, sample ranking disclosure, filters, Details/Back/navigation and saves are unchanged.

## Verification

Final **Verify03**, Unity6000.5.9f1: rebuild/readback succeeded; **13EditMode +15PlayMode =28passed**, zero failures/skips. Includes TeamHub and IndividualRanking regression checks. Verify01/02 also passed. Visual inspection prompted expanded numeral-shadow cleanup, then protection of the adjacent bronze pictogram edge before final Verify03.

Actual CanvasRenderer glyph bounds are checked in source coordinates for all six statistics: complete single line, original-lane containment, divider separation and reward clearance. Existing checks cover rewards/row highlighting, localization, safe-edge placement, input, routes and save preservation. Nine actual TeamRanking captures cover Russian/English and standard/narrow/tall safe-area displays. Final RU and narrow EN plus alpha over black/white/magenta were visually inspected. Review browser:11 image views and comparison toggle passed at430×900, no script errors or horizontal overflow.

No Android build, physical-device check, performance run or full test suite was performed. The page still has font, reconstructed texture, tab/status, footer and scenery differences from the original.

[Open review](TeamRanking_PodiumStatsAndSurfaces_R5_Review.html) · [Fidelity](QA/ReferenceFidelityReview.md) · [Scope/checks](QA/ScopeAndVerification.json) · [Provenance](Sources/AssetProvenance.json)
