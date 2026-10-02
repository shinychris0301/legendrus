# Team Ranking — Responsive Audit R9

The existing Back arrow measures 72×69 reference pixels, about 44×42 screen pixels in the narrow-phone capture. Expanded its native raycast area by four reference pixels on each side and six above/below, giving an 80×81 target (about 49×50 screen pixels). These are simulated pixel measurements, not device dp certification.

The original arrow, complete Guild Details button, large side banners, candles and central carving retain their appearance and placement. Back remains above the footer as previously approved. The extra touch area stays inside the safe viewport and clears the standings and Guild Details control. No texture/font/color changes.

The reference-layout owner, view, builder, active prefab and cutover revision are 10. Sample standings, unavailable leaderboard service and unchanged saved-state behavior are retained.

Validation covers safe-top header, safe-bottom ornaments, complete sprite bounds, Back target edges, reward/stat containment, localized text, native filter/Details/Close/Back clicks, modal shielding over the full safe height, and language changes with the panel open. New captures include tall English, local/week filters and tall Details in both languages.

Verify01 rebuild/readback and all 11 targeted Unity tests passed: five EditMode and six PlayMode, zero failures/skips. Retained 20 native captures; all 12 browser views and native-pixel viewing passed without overflow/errors. Eight matching R8 captures are pixel-identical; tall Russian differs by only two text pixels with maximum channel delta one. Narrow English and tall Russian Details captures were visually inspected.

Six existing Assets files changed (reference-layout owner, view, builder, prefab, cutover and PlayMode tests); 7,190 unchanged, none added/deleted. Exact scope is recorded in QA/ScopeAndVerification.json. Native captures are distinct from this static HTML viewer. No image generation or Scenario calls were required. No Android build, physical-device test, backend integration, performance check or full-project test suite was run.

Known visual differences remain from R8: extra scenery space on tall phones, tighter ranking rows, approximated source fonts/panel surfaces and reconstructed transparent ornament edges. This audit preserves the accepted art; it does not establish pixel identity to the original reference.

Next recommended page: Team Chat responsive/footer audit.
