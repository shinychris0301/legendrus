# Login / Account — Responsive Audit R2

Enlarged the password-eye target to 76×76 reference pixels, input fields to 539×76, and Link Account to 289×76. Existing artwork and input geometry remain unchanged; raycast padding provides the additional space. The account-link target grows downward to avoid Guest above it.

Terms and Privacy were short, tightly stacked text links. They now occupy separate columns below Link Account with 76-reference-pixel-high targets, leaving a gap between controls. Localized labels wrap within 48-pixel areas on the parchment. The existing unselected consent ornament moved down 15 reference pixels to align with them; it remains decorative and records no consent. Other panel, button, scenery, logo and character artwork are retained. At the narrow tested size, 76 reference pixels are about 47 screen pixels; this is not a device-dp certification.

Runtime builder, view, active Login prefab and cutover are revision 3. Existing routing, guest behavior and unavailable authentication explanations remain. No credentials are sent; leaving Login clears the form as before.

Added native raycast tests for the enlarged Eye and Link targets, both legal links, tall explanation shields, RU/EN relocalization while open, native Close and return to Settings. Geometry checks cover safe target bounds, minimum dimensions, non-overlapping button targets and the complete account panel. Existing tests cover guest commit-once behavior, unchanged progress, other origins, provider explanations, legal state and field clearing.

No image generation, Scenario calls, fonts or image assets changed. Safe areas/display sizes are simulated. No physical Android/device, soft-keyboard, backend, performance or full-project suite testing. Existing source/font/surface approximations remain.

Verify01 rebuilt successfully, but its EditMode check still expected the old revision 2. Updated the test to use the owning view's CurrentRevision before rerunning. Earlier evidence is preserved.

## Final verification

Verify02 passed rebuild/readback and all 17 Unity tests (4 EditMode + 13 PlayMode), with no failures or skips, including Settings regressions. Produced 26 actual Login captures. Twelve browser review views and native-pixel toggle passed without overflow or errors. Evidence for both verification runs is under `QA/`; Settings regression captures are retained separately.

Compared eight matching R1 captures. Every pixel difference is confined to the repositioned legal links and their decorative checkbox. At reference size the combined changed bounds are x253–725, y1495–1554; main panel, buttons, scenery, character, logo and other lettering remain pixel-identical. Narrow Russian and tall English captures were inspected: legal labels remain within the parchment, with no decorative frame obscured.

Seven existing Assets files changed: `ProductionLoginRuntime.cs`, `ProductionLoginView.cs`, `ProductionLoginPrefabBuilder.cs`, `Login.prefab`, `LoginCutoverState.txt`, `LoginAlignmentPlayModeTests.cs`, `LoginAlignmentEditModeTests.cs`. Other 7,189 Assets files unchanged; none added/deleted. No image/font/meta changes. Added review/scripts/reports/QA evidence.

Next recommended page: Tutorial / Help responsive/navigation audit.
