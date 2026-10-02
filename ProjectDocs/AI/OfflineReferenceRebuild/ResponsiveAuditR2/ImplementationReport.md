# Offline — Responsive Audit R2

The Offline header's resource plus controls had 44×47 reference-pixel targets, approximately 27×29 pixels on the narrow simulated phone. Expanded their native raycast targets to 76×76 while preserving artwork and placement. Energy expands leftward only to avoid Settings. This applies to Offline's shared-header instance, not other pages.

Retry and Home visual rectangles overlapped vertically by three reference pixels. Inset the Retry hit area's bottom by four reference pixels, leaving a one-pixel gap between the hit rectangles without changing either sprite or layout. Retry remains 467×138 reference pixels.

Runtime/view/builder/active OfflineState prefab and cutover revision agree at 4. Retry still uses device network reachability; detecting a device connection does not claim game-service availability or navigate away. Existing focus refresh, localization, shared resource categories and duplicate-exit protection remain.

## Verification

Unity 6000.5.9f1 Verify01 rebuild/readback and all 10 tests passed (2 EditMode + 8 PlayMode), zero failures/skips. Added native Retry tests for disconnected, connected and disconnected-again states, RU/EN relocalization, single Home return, header edge taps and correct resource categories. Assertions cover saved progress, probe/check counts, complete connectivity messages, safe bounds, full sprite presentation and non-overlapping targets. Existing focus, binding, press-feedback and balance tests passed.

Produced 19 actual Unity captures. Eleven browser review views and native-pixel toggle passed without overflow/errors. All six matching R1 captures are pixel-identical. Fresh tall English and narrow Russian network-detected captures were inspected: full panel ornaments and buttons remain visible and message text fits. Existing repaired surfaces, fallback typography, header backing ornament and tall-screen scenery spacing remain. Pixel identity establishes preservation of R1, not exact original-source fidelity.

Seven existing Assets files changed: ProductionOfflineRuntime.cs, ProductionOfflineView.cs, ProductionOfflinePrefabBuilder.cs, OfflineState.prefab, OfflineStateCutoverState.txt, OfflineAlignmentPlayModeTests.cs and OfflineAlignmentEditModeTests.cs. Other 7,189 Assets files unchanged; none added/deleted. No artwork/font/meta changes or image-generation/Scenario calls. Added review/scripts/reports/QA evidence.

Screen sizes, safe areas and network states are simulated. No physical Android/device, live server/network, backend, performance or full-project tests were run.

Next recommended page: Maintenance responsive/navigation audit.
