# Settings — Responsive Audit R2

Preference switches were approximately 42 pixels high in the narrow-phone simulation, and the language modal Close button was approximately 43 pixels wide. Expanded native switch padding vertically by four reference pixels each side, and modal Close horizontally by three each side. Targets are now 147×77 and 77×76 reference pixels respectively, with their smaller dimension approximately 47 screen pixels. These are simulated pixels, not device dp certification.

Approved artwork, positions, localized labels, carved frame, shared header and Back remain unchanged. Switch touch areas stay separate. The runtime owner, view, builder, active prefab and cutover revision are 4.

New checks cover switch edge taps with save readback/restoration, language selection and expanded Close, tall explanation/modal shields, relocalization and native Back returning to Inventory. Existing tests cover other origins, service explanations, saved preferences, press feedback and paused gameplay board/hash/moves/score preservation. Standalone Language Select regressions also run; its production assets/runtime are unchanged.

No image generation or Scenario calls. No Android/device/backend/performance/full-project testing. Existing repaired row surfaces, fallback typography and tall composition spacing remain. Next recommended page: standalone Language Select responsive/navigation audit.

## Verification and evidence

Unity 6000.5.9f1 Verify01 rebuild/readback passed. All 15 tests passed (4 EditMode, 11 PlayMode), with no failures or skips. This includes standalone Language Select regressions. Produced 30 actual Unity Settings captures; 13 browser review states and the native-pixel toggle passed without overflow or browser errors. Test results, logs and commands are under `QA/Verify01/`; standalone regression captures are under `QA/LanguageSelectCaptures/`.

Compared 15 matching R1 captures: 13 are pixel-identical. `Settings_Language_safe_EN.png` differs by two pixels and `Settings_576x1280_safe_EN.png` by one pixel, maximum channel delta one. This confirms preservation of the prior rendering, not exact fidelity to every detail in the original illustration. Visually inspected the narrow relocalized language modal and tall English Settings capture.

Six existing Assets files changed: `ProductionSettingsRuntime.cs`, `ProductionSettingsView.cs`, `ProductionSettingsPrefabBuilder.cs`, `Settings.prefab`, `SettingsCutoverState.txt`, and `SettingsAlignmentPlayModeTests.cs`. Other 7,190 Assets files unchanged; none added or deleted. No artwork, font or meta changes. Added this review, scripts, report and QA evidence.

Notifications remain a local preference rather than OS notification delivery. Sign-out remains unavailable and saved progress is preserved. Phone sizes and safe areas were simulated; no physical Android device validation was performed.
