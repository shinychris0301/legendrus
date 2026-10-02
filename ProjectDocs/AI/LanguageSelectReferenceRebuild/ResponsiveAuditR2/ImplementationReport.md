# Language Select — Responsive Audit R2

The standalone language modal still had a 71×76 reference-pixel Close target, approximately 43×47 screen pixels in the narrow-phone simulation. Added three reference pixels of native raycast padding on each horizontal side, producing a 77×76 target (approximately 47×47 simulated screen pixels). The visible button remains unchanged. This is consistent with the Settings language modal.

The runtime builder sets the padding explicitly so it does not depend on incidental inherited padding when cloning Settings controls. Runtime creation, view, owning prefab builder, active LanguageSelect prefab and cutover revision agree at revision 3. Existing routing and language persistence are retained.

Added a native-raycast interaction test for RU/EN selection from Splash, Home and Settings, saved language readback, unchanged gameplay save, expanded Close-edge taps and single return navigation. Existing tests cover repeated binding, preference reload, shared Settings selection, input isolation and press feedback. Every capture now checks minimum touch dimensions, whole panel/button safe bounds, full-screen corner shielding and complete source sprites. Captures are written before assertions for failure evidence.

No artwork generation, Scenario calls, font changes or source image edits. Approved Splash art is reused as the existing inactive backdrop. This audit preserves that rendering; it does not claim exact source-font identity. Safe areas and display sizes are simulated; no physical Android/device, backend, performance or full-project test suite is included.

## Results

Unity 6000.5.9f1 Verify01 rebuild/readback passed. All 16 tests passed (4 EditMode + 12 PlayMode), no failures or skips, including the existing Settings regression suite. Produced 18 actual Language Select captures; 11 browser review views and native-pixel toggle passed without overflow or errors. Logs, commands and XML results are under `QA/Verify01`; Settings regression captures are under `QA/SettingsCaptures`.

All six comparable R1 captures are pixel-identical, including Russian/English narrow views and button-press state. Visually inspected the narrow English modal after selection from Home and tall English modal from Settings. Panel and Close ornaments remain complete.

Six existing Assets changed: `ProductionLanguageSelectRuntime.cs`, `ProductionLanguageSelectView.cs`, `ProductionLanguageSelectPrefabBuilder.cs`, `LanguageSelect.prefab`, `LanguageSelectCutoverState.txt`, `LanguageSelectAlignmentPlayModeTests.cs`. Other 7,190 Assets files are unchanged, none added/deleted. No changes to the approved Splash assets or Settings production files. Added review, scripts, reports and QA evidence.

Next recommended page: Login / Account responsive/navigation audit.
