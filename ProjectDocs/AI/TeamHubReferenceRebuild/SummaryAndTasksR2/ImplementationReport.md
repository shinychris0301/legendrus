# Team Hub R2 — Guild Summary and Task Frames

Implemented the approved refinement in the active native Team Hub prefab. The guild chest now has its original circular enclosure and Russian nameplate; the member count has the source group pictogram. The three task cards sit within a continuous original outer frame/header, with independent dark count plaques and better aligned reward plaques, approved sapphires and live amounts.

Russian fixed chest/section lettering is retained from the source as previously authorized. English switches to blank variants and live labels. Semantic localized text remains assigned in both languages. Existing chest, shared Home header, gem, potion, fonts, navigation and sample-state behaviour are preserved. No Scenario jobs or credits were needed: these pieces use source pixels and existing approved lacquer for hidden faces.

## Implementation

- `ProductionTeamHubReferenceLayout.cs`: summary icon, chest enclosure draw order, task shell/card geometry, count/reward plaques and reward alignment.
- `ProductionTeamHubView.cs`: revision3 and localized source/blank variants.
- `ProductionTeamHubPrefabBuilder.cs`, `Pages/TeamHub.prefab`, `CutoverState/TeamHubCutoverState.txt`: revision3 rebuilt and read back successfully.
- Two existing Team Hub test files: source/blank locale behaviour, enclosure ordering, reused chest identity, independent count plaques and existing layout/input regression checks.
- Seven new PNGs and their metadata under `Resources/ProductionUI/TeamHubAligned`; extraction bounds, repairs and SHA256 are in `Sources/AssetProvenance.json`. Existing art and metadata were not replaced.

Exact changed files and preservation results: `QA/ScopeAndVerification.json`, `QA/PreservationChecks.json`. Seven existing Assets files changed, fourteen added, none deleted relative to the start-of-task snapshot. Prior Event Detail/Individual Ranking changes, other page prefabs, existing PNGs/metas and fonts remain byte-identical.

## Verification

Unity6000.5.9f1, Verify01: narrow rebuild/readback succeeded; **5 Edit Mode + 10 Play Mode = 15 passed, 0 failed/skipped**. Includes Individual Ranking regression. Tests cover repeated rebuilding, shared header and resources, RU/EN variants, actual button routes, modal blocking, absence of reward/save mutations, safe bounds and text fit.

Eight Team Hub and ten Ranking captures were produced with isolated saves. Russian941×1672 and narrow English576×1280 with simulated safe area were visually inspected. All seven new sprites were inspected over black, white and saturated magenta. Intentional opaque lacquer is preserved. Review HTML provides one portrait at a time with original/before comparison; browser checks load ten views, toggle comparison and check errors/overflow.

No Android build, physical-device check, performance test, full project suite or live guild backend validation was performed. Functional test success does not establish pixel identity. See `QA/ReferenceFidelityReview.md` for remaining differences.
