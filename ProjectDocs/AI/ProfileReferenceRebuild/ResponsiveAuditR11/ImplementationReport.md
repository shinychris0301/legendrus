# Profile — Responsive Audit R11

Expanded the native touch areas for the portrait Edit icon, collection action, collection tabs and history pager. Edit now has a 76×77 reference-pixel target; the other controls have at least 76 reference pixels of height, approximately 47 screen pixels on the tested narrow phone. These are simulated pixel measurements, not physical-device dp certification.

Visible artwork, frames, lettering, placement and press feedback stay unchanged. Edit clears the tabs. Collection actions clear the footer. Footer artwork remains anchored to safe edges and uses complete approved icons. Live saved statistics and clearly labelled collection previews retain their behavior.

Runtime owner, view, builder, active prefab and cutover agree at revision 12. New verification covers target edge raycasts, safe-bottom footer/Back placement, complete footer symbols, native section changes, tall profile actions, relocalization, Close and Inventory navigation with unchanged saved state. Existing selection/history/progress/max-value/feedback tests also run.

Verify01 rebuild/readback and all 15 targeted Unity tests passed (3 EditMode + 12 PlayMode), zero failures/skips. Retained 61 native captures; 14 browser review states and native-pixel toggle passed without overflow or script errors. All 44 comparable History Rows R10 screenshots are pixel-identical. Narrow Russian collection and tall English action-panel captures were visually inspected.

Six existing Assets files changed: ProductionProfileRuntime.cs, ProductionProfileView.cs, ProductionProfilePrefabBuilder.cs, Profile.prefab, ProfileCutoverState.txt and ProfileAlignmentPlayModeTests.cs. Other 7,190 Assets files unchanged, none added/deleted. Exact scope/results are in QA/ScopeAndVerification.json. Source/documentation whitespace checks passed.

No image generation or Scenario calls. No Android/device/backend/performance/full-project tests. Existing source-font, panel-surface and collection-preview differences remain. Next recommended page: Inventory responsive/navigation audit.
