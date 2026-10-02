# Friends List Reference Rebuild R1

The older copied header, recreated Russian button captions and cramped textured rows diverged from the original. This pass installs the shared Home header, restores original Russian Energy/Visit/footer lettering, introduces source-derived navy row panels and gives portraits/content more space. The header and bottom navigation follow the safe screen edges. Approved Home scenery and carved footer material are reused.

Search, sorting, tab selection, navigation, localization and explanatory social-action dialogs remain native. This is still a sample Friends page: no live presence, requests, visits or energy transfers are implied. Existing routes and saved balances are preserved.

## Art and provenance

Seven original-lettered sprites reuse the accepted Scenario-derived alpha of the existing Energy, Visit and five footer sprites. Their original RGB is restored without another service request. English keeps the accepted blank faces and live text.

One new Scenario background-removal job isolates a row prepared from original rails and a clean navy source donor: **3 CU, no resubmission**. Its thin gold rails needed local silhouette restoration after Scenario removed them. Intentionally opaque panel material remains opaque. Hidden rail/surface reconstruction is approximate. Eight new sprites and metas are added; existing art and GUIDs are preserved.

See [asset provenance](Sources/AssetProvenance.json), [Scenario audit](QA/ScenarioAudit.json) and [black/white/magenta inspection](QA/AssetAlpha.png).

## Active implementation

- `ProductionFriendsListRuntime.cs`: shared header setup, approved clean scenery, restored Russian captions, source row panels, larger portraits, footer geometry/material.
- `ProductionFriendsListView.cs`: revision 2, shared-header binding and presentation, Russian sprite/English text switching.
- `ProductionFriendsListLayout.cs`: safe-edge header/footer anchoring, expanded list surround and full modal input shield on tall displays.
- `ProductionPlayerHeader.cs`: Friends List adapter only; existing adapters retained.
- `ProductionFriendsListPrefabBuilder.cs`: regenerated active prefab and cutover, both revision 2.
- Existing Friends Play Mode tests extended for shared header, safe-edge geometry, lettering mode and row/footer separation.

## Verification

[Scope and final test results](QA/ScopeAndVerification.json) record the actual files and checks. Tests use isolated saves and include Team Hub and Team Chat regressions. Verify01 was interrupted after an Edit Mode result; its incomplete evidence is retained. Verify02 exposed an older strict safe-boundary assertion, corrected to the half-pixel tolerance used on other pages. Verify03 then caught large-balance overflow because the older Friends refresh bypassed shared-header sizing; calling the shared refresh fixes that defect. The initial Scenario matte also needed rail restoration, performed without another request.

Final Verify04: Unity6000.5.9f1 rebuild/readback, 10 EditMode + 16 PlayMode tests passed, zero failed/skipped. Twelve native Friends List captures were produced. Russian, narrow English and Requests captures and transparent assets were inspected. The standalone review provides native captures, earlier implementation and the original reference. All 15 browser views and the comparison toggle passed without script errors or horizontal overflow. Scoped C# whitespace checks passed.

No Android build, physical-device/keyboard, performance, full-suite or live-backend test was performed. [Remaining differences](QA/ReferenceFidelityReview.md) distinguish source artwork, reconstructed surfaces and intentional behavior differences.
