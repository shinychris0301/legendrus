# Team Hub R5 — Typography and Season Progress

Implemented the approved typography/progress refinement in the real Team Hub prefab. Guild power heading/value now align left as in the reference. Membership, task-label, member-name and contribution text proportions are tightened, and selected live labels use a restrained one-pixel dark shadow for separation. The approved embedded font assignments are unchanged. A trial reduction in guild-name size was rejected after comparing the first native capture; its previous proportions are retained.

The flat season fill is replaced by the existing approved Loading sapphire texture, rendered as a native horizontal Filled Image beneath the source rail and live count. The count and fill use one explicit pair of sample constants:7560/10000,75.6%. Existing prominent preview notices remain. This does not introduce actual guild progress, timers, reward collection or online services. The old serialized fill component remains bound but disabled to preserve references; the new image is a required binding.

No new artwork, fonts, extraction or Scenario calls/credits were needed. The reused sprite was inspected and its hash recorded in `Sources/AssetProvenance.json`; existing images/metas/fonts and other pages remain unchanged.

## Native changes

`ProductionTeamHubReferenceLayout.cs`, `ProductionTeamHubView.cs`, `ProductionTeamHubPrefabBuilder.cs`, active TeamHub prefab/cutover and two TeamHub test files. Internal revision6 is synchronized. Source title/navigation/motto lettering, shared Home header, roster assets, native footer routes/info panel and sample-state guards remain intact. Exact scope is recorded in `QA/ScopeAndVerification.json`.

## Verification

Final Verify02 Unity6000.5.9f1 narrow rebuild/readback passed; **8 Edit Mode + 10 Play Mode =18passed,0failed/skipped**, including Individual Ranking regression. Verify01 passed before the guild-name size adjustment. Tests cover new fill binding/texture/fraction/order, sample count consistency, retained font, localized actual glyph fit, safe-area and button bounds, repeat rebuild, native navigation/modal blocking and no reward/save mutations.

Eight TeamHub/ten Ranking actual Unity captures were packaged with isolated saves. Final Russian941×1672 and narrow English576×1280 safe-area captures inspected. Reused sapphire viewed over black/white/magenta. Standalone browser review loads ten views and toggles original comparison without overflow/script errors.

No Android build, physical-device check, performance test or full-project suite ran. Test success is not evidence of exact typeface identity or whole-page pixel equivalence.
