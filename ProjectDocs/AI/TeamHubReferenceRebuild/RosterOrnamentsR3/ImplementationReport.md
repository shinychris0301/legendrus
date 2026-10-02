# Team Hub R3 — Roster Ornaments

Continued the recommended roster refinement after the user's “continue to next.” The native page now uses original guild rank badges, carved row ends and bronze/navy roster surfaces. Ranks4–5 are plain live numerals without circular backplates. Contribution shields reuse the approved `TeamHubAligned/power` sprite at larger source-aligned dimensions, with left-aligned live amounts.

Three badge PNGs retain original source RGB and fixed numerals; two blank row sprites preserve visible original carving and reconstruct hidden faces/portrait-occluded rails from the same source row. The existing portraits, shared header, fonts, chest, sapphire, potion and R2 task/summary art are unchanged. No Scenario calls or credits were needed for these visible source details.

Also repaired an existing repeated-build ordering issue: inserting an already preceding decoration at its content's old index moved it above that content. A common ordering helper now keeps chest/task/count decorations behind content on every rebuild; assertions cover repeated rebuilding.

## Changed files

Six existing Assets files: `ProductionTeamHubReferenceLayout.cs`, `ProductionTeamHubView.cs`, `ProductionTeamHubPrefabBuilder.cs`, active `Pages/TeamHub.prefab`, `CutoverState/TeamHubCutoverState.txt`, and `TeamHubAlignmentEditModeTests.cs`. Five new PNGs and metadata under `TeamHubAligned`. Native view/builder/prefab/cutover agree on internal revision4.

Exact paths/hashes: `QA/ScopeAndVerification.json`, `Sources/AssetProvenance.json`, `QA/AssetChecks.json`. Existing PNGs/metas/fonts, prior Event Detail/Ranking changes and other page prefabs are unchanged relative to the start-of-task snapshot. No deletions or backend/reward/save changes.

## Verification

Final **Verify03**, Unity6000.5.9f1: rebuild/readback passed; **6 Edit Mode + 10 Play Mode = 16 passed, zero failed/skipped**, including Individual Ranking regression. Existing tests check safe-area positioning, RU/EN variants/text fit, button routing, modal blocking, shared assets and sample-state immutability. New assertions check source badges/plain lower ranks and stable decoration order across rebuilds.

Verify01 detected the existing repeated-build ordering issue. Verify02 stopped on a variable-name collision in the added test; corrected before Verify03. Logs are retained, not overwritten.

Eight TeamHub/ten Ranking actual Unity captures with isolated saves were packaged. Russian941×1672 and narrow English576×1280 with simulated safe area were visually inspected. Five new sprites were inspected over black/white/magenta; source-medal RGB and opaque numeral centres verified. Browser review loads ten views, toggles original comparison and checks errors/overflow.

No Android build, physical-device verification, performance test, full project suite or online guild integration was run. Full-page pixel identity is not claimed; remaining differences are in `QA/ReferenceFidelityReview.md`.
