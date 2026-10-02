# Tutorial / Help — Responsive Audit R6

The shared header instance on Tutorial had 44×47 reference-pixel resource targets, approximately 27×29 screen pixels on the narrow simulated phone. Enlarged each native target to 76×76, preserving the visual sprites and positions. Coins and gems expand equally on both sides; energy expands only leftward to avoid the adjacent Settings target. This change belongs to Tutorial's header instance; it does not modify other pages' shared-header instances.

Overview builder, owning view, active TutorialOverlay prefab and cutover revision agree at 7. The owning prefab builder rebuilds the actual page and verifies its serialized bindings. The approved title, isolated gift medallion, full button silhouettes, Russian source lettering and current Gameplay gem/booster resources remain intact.

New interaction coverage includes native header edge taps with matching resource routes, short-screen native scroll events exposing the complete reward-panel bottom, lesson heading taps, detail Next/Previous/Close, and native Done returning to Home in Russian and English. Layout assertions check minimum targets, non-overlapping header actions, full source sprites, safe-top header and safe-bottom footer. Offscreen lesson headings are intentionally clipped by the existing scroll viewport and excluded from visible-target assertions.

Existing tests cover the four topics, original gem/booster resource identity, first-level routing without spending energy, repeated bindings, button press feedback and returning to the same paused board with moves/score/hash intact. Display sizes and safe areas are simulated; no Android/device, performance, backend or full-project suite testing. Existing font and reconstructed surface differences remain. No image generation, Scenario calls, artwork or font changes are needed.

## Results

Unity 6000.5.9f1 Verify01 rebuild/readback and all 13 tests passed (3 EditMode + 10 PlayMode), with no failures or skips. Produced 29 actual Unity captures. Twelve browser review views and native-pixel toggle passed without overflow or errors. Logs, commands and test XML are retained under `QA/Verify01`.

Compared 14 matching R5 captures: 13 are pixel-identical. The short Russian overview differs by 19 pixels, maximum channel delta one, in a single line of text. Fresh short-screen scrolled Russian and tall English overview captures were inspected: complete reward-panel bottom and button ornaments remain visible, with the existing tall scenery gap preserved.

Seven existing Assets changed: `ProductionTutorialOverview.cs`, `ProductionTutorialView.cs`, `ProductionTutorialPrefabBuilder.cs`, `TutorialOverlay.prefab`, `TutorialOverlayCutoverState.txt`, `TutorialAlignmentPlayModeTests.cs`, `TutorialAlignmentEditModeTests.cs`. Other 7,189 Assets files unchanged; none added/deleted. No image/font/meta changes. Added review, scripts, reports and QA evidence.

Next recommended page: Offline responsive/navigation audit.
