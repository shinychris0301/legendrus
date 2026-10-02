# Event Detail R7 — typography and Firebird edges

User selected continued Event Detail refinement. The page now reuses licensed, embedded project fonts for the roles previously chosen by the user: Roboto Serif SemiBold for player names and FreeSerif Bold for the English title/logo/buttons. Both fonts are serialized bindings assigned by the owning runtime/builder and reapplied after localization. Explanatory copy retains its lighter existing typeface; the original Russian title/button images are unchanged. No OS font files were copied or new font binaries imported.

Adjusted heading baselines, name column position/width, availability/objective/premium copy space and small reward-label height. The shared Home header still owns its presentation.

Firebird refinement reuses the earlier accepted Scenario/source mask, constrains segmentation to a narrow edge band and keeps solid feather interiors. Tiny enclosed pinholes and detached dust were removed; a visible skyline window beyond the wing was explicitly excluded from subject/light. Source starbursts remain. Halo was recomputed from the cleaned light mask using the previous colour/radius/intensity. Original visible bird RGB and geometry are preserved; this does not imply exact alpha recovery.

## Scope and verification

Internal revision **8** in owner/builder/prefab/cutover. Seven existing Assets files changed: ProductionEventDetailRuntime.cs, ProductionEventDetailView.cs, ProductionEventDetailPrefabBuilder.cs, EventDetail.prefab, EventDetailCutoverState.txt and the two EventDetail alignment fixtures. Three new Firebird/light PNGs plus their metadata. Existing art/fonts/metas, R6 frame/progress assets, other page prefabs and prior working-tree changes preserved. See QA/ScopeAndVerification.json.

Final evidence: **Verify03**, Unity **6000.5.9f1**. Targeted EventDetail/Events tests and six native Detail/eight Events regression captures use isolated saves. Tests verify font resource bindings/persistence, text fit, shared resources, nonoverlap, safe areas, input/navigation and unavailable data state. Final RU/narrowEN and black/white/magenta alpha inspected. Review selector/comparison checked in Chrome. **4 Edit Mode +10 Play Mode =14 passed**, zero failed/skipped; nine browser views/comparison passed. Exact results are in QA.

Verify01 caught the new font's small reward-label height requirement; bounds were increased. Verify02 passed but the all-semibold body treatment was visually too heavy, so approved fonts were limited to intended roles before Verify03. No new Scenario requests/credits; existing extraction and source pixels reused. No Android build, physical-device test, performance test or full-suite run.

QA/ArtAudit.json records source RGB checks. Sources/ReusedAssets.json and QA/PreservationChecks.json verify approved art/font hashes, retained blue feather eyes and excluded skyline residue. QA/ReferenceFidelityReview.md documents remaining differences and intentional adaptations. Live event systems remain unavailable; no dates, balances, progress or rewards were fabricated.
