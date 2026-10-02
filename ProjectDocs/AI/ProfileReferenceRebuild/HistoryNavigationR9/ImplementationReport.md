# Profile R9 — History Navigation

Profile History previously rendered Previous and Next as available at both boundaries, even when activating one could only clamp to the current page. R9 dims and disables the unavailable direction. A single-page history disables both directions; empty history retains its existing empty state without a pager.

Disabled controls retain their opaque original source faces and use a muted frame/text color through native Graphic colors. Press feedback ignores them. Click callbacks also check interactability, so a direct event invocation cannot activate an unavailable direction.

`ChangeHistoryPage` now applies only in the History section, computes a clamped target without integer overflow, and skips content rebuilding when that target equals the current page. Available navigation and saved record ordering are preserved. The page counter reuses the approved numeral font with bounded fitting.

View, builder, active prefab and cutover use revision10. No new artwork or font resources were needed:0 Scenario calls/0CU. Original images, frame geometry, header, records and other Profile sections are preserved.

## Verification

Final run `/tmp/lor-profile-history-r9/Verify01`, Unity6000.5.9f1. Import/rebuild succeeded;3 EditMode+10 PlayMode=13passed,zero failures/skips. New boundary coverage includes0,1,6,7 and12 records; disabled pointer response, direct invocation, preserved record instances on unavailable moves, extreme deltas, first/last-page controls and unchanged saves. Existing press feedback, localization, safe-area, text fit, routing, progress and collection tests ran.

42 actual Unity Play Mode captures;44 browser views decoded at430px without errors or horizontal overflow. Visually inspected first-page Russian and last-page tall-English history captures. Captures use isolated test saves and simulated dimensions/safe areas.

SHA checks preserve5,575 existing protected artwork/font/meta/other-page files. Standard ProfileRU/EN and ItemsRU captures are pixel-identical toR8. The matched HistoryRU capture changes only inside pagination controls. Scoped source whitespace checks passed.

No Android/device/full-suite/backend/performance testing. History has no dedicated original screenshot; its existing approved materials are retained, while this pass refines behavior and enabled/disabled presentation.

## Changed Assets files

- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/ProfileCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Profile.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionProfilePrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/ProfileAlignmentPlayModeTests.cs`

Six existing Assets files changed; none added/deleted. Review evidence/scripts and handoff docs accompany the implementation.

[Scope and Unity results](QA/ScopeAndVerification.json) · [Integrity](QA/IntegrityChecks.json) · [Browser](QA/BrowserChecks.json) · [Fidelity](QA/ReferenceFidelityReview.md)
