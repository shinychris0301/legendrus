# Login / Account — Reference Rebuild R1

2026-09-25. User authorized the next page after Language Select: Visual Page 30 Login / Account.

The real Login prefab now has clearer native form spacing, less crowded provider messaging, gold live button captions with restrained dark relief, and the same press feedback as other rebuilt pages. Russian and English remain live/localizable; the approved title/body font assets are reused. Sign In and Guest captions use the approved body face instead of the heavier title face. The availability note is smaller and separated from the provider line; field placeholders and legal-link type are reduced to better fit their source bounds. Link-account lettering was enlarged within its plaque. No artwork or font files changed.

The existing repaired Login panel/subject silhouettes, logo, scenery, provider icons and controls are preserved. This reference has a logo and account panel rather than a player-resource header, so none was added. No new Scenario request was needed: 0 jobs / 0 CU, reusing the accepted Scenario extractions and PanelEdgesR2 repairs.

## Implementation

Seven existing Assets files changed; no additions/deletions:

- `ProductionLoginRuntime.cs`: native type/spacing/press treatment and idempotent presentation refresh.
- `ProductionLoginView.cs`: revision 2.
- `ProductionLoginPrefabBuilder.cs`: rebuilds its owned native hierarchy while retaining prefab root/GUID; writes revision 2.
- `Pages/Login.prefab` and `CutoverState/LoginCutoverState.txt`: actual rebuilt revision 2.
- `LoginAlignmentEditModeTests.cs` and `LoginAlignmentPlayModeTests.cs`: native feedback assertions and held-press/cancel test.

Full paths and task-start comparison are in [ScopeAndVerification.json](QA/ScopeAndVerification.json). Current Login layout/scaling is unchanged.

## Validation

Unity 6000.5.9f1. All runs used isolated XDG_CONFIG_HOME / test save storage, without the user's progress.

- Baseline01: 4 EditMode and 10 PlayMode checks passed (Login and Settings).
- Verify01: native prefab rebuild/readback succeeded; 4 EditMode and 11 PlayMode checks passed, no failures/skips.
- Checks cover native bindings, runtime-builder idempotence, localized layouts, control raycasts, password show/hide and clearing, unavailable services/legal state, modal input blocking, single guest commit, progress preservation and return routes from Splash/Settings.
- Added held-press test covers Sign In, Guest, Link, Eye, VK and Back: visible tint, release on pointer exit, no unintended route/guest commit and unchanged save. Captured Guest pressed.
- Eight actual Login Play Mode captures: reference-size RU/EN, tall safe-area RU/EN, larger safe-area RU, unavailable RU/tall EN, and pressed RU. RU and tall EN inspected directly.
- Browser: 10 review views loaded at 430px; no page errors or horizontal overflow.
- SHA-256 confirms 5,615 protected existing image/meta/font/other-page-prefab files unchanged. Scoped source/test/cutover whitespace passed.

No Android/device/soft-keyboard, full-suite, performance, real authentication, network-provider, SMS or fresh-process persistence testing. Account services remain unavailable and explained honestly; guest uses existing local progress. Legal links do not record acceptance.

See [Reference fidelity review](QA/ReferenceFidelityReview.md) for remaining differences. Native screenshots are evidence, not the production page implementation.
