# 2026-10-03 — Settings recovery R1: source delivery

The user approved the next task after the round lifecycle APK refresh. This delivery repairs reproduced preference-saving and native callback failures in the existing Settings page. Two production files, two fixtures and four continuity documents were backed up before editing. Human rounds 201–210 remain unrecorded from the user's prior confirmation; their review remains pending.

[Reproduction](QA/Reproduce/summary.json) records **12 primary failures**, zero skips, Unity launcher exit 2: eight legacy toggles mutated the live preference despite rejected or already locked saving; standalone and paused native switches displayed that uncommitted state; a Changed callback switched twice; a retired binding listener still acted. [Focus](QA/Focus/summary.json) passes the same 12 after repair. [Boundaries](QA/Boundaries/summary.json) passes 12 additional checks. No production or test corrections were required after the focused fix.

`TryTogglePreference` validates the index, honors the persistence lock, clones the current save, changes exactly one boolean and delegates to the existing acknowledged commit helper. Music, SFX, vibration and notification legacy wrappers delegate to this API. Rejection leaves the live object, serialized journal, events and feedback preferences unchanged. Successful commitment adopts the candidate before Changed observers run; tests also verify that pending booster receipt identity and the transient failure result survive. Existing durable-primary adoption and post-write cleanup policy remain covered by the journal regression suites. No schema, audio asset, notification permission or platform scheduling change is introduced.

The actual native Settings view consumes the acknowledgement, refreshes from the committed preference and shows its existing InfoPanel with the existing Russian/English nonretry error wording on failure. Close or Back dismisses the message on the exact original Settings root. Paused Settings retains its board, moves and input lock, then returns to Pause through the existing route. The error panel blocks unrelated controls and refreshes correctly in both languages. A binding-owner token rejects callbacks captured before rebind, and a scoped busy guard prevents Changed callback reentry. The existing Settings Revision 5 prefab, builder, serialized bindings, art and layout are unchanged.

Final actual Unity **6000.5.9f1 (b57deb96f08d)**, isolated storage per launcher:

| Group | Mode | Passed | Failed | Skipped |
|---|---|---:|---:|---:|
| Core | PlayMode | 197 | 0 | 0 |
| Settings | PlayMode | 41 | 0 | 0 |
| Navigation | PlayMode | 21 | 0 | 0 |
| Retained | PlayMode | 52 | 0 | 0 |
| Edit | EditMode | 85 | 0 | 0 |

**311 Play + 85 Edit = 396 distinct final passes**, zero failures/skips, all five final launchers exit 0. Focus and Boundaries are subsets counted once. [Reconciliation](QA/TestReconciliation.json) · [Source validation](QA/SourceValidation.json) · [Final validation](QA/FinalValidation.json). Raw Unity/launcher logs and results XML are retained. These targeted suites cover Settings controls, feedback configuration, Pause/navigation, native win/fail journeys, retained rounds/continuation, earlier transaction recovery, journal loading/writing and migration. They are actual Unity runs, not source-only checks or a full-project/device acceptance claim. The repaired-journal test creates a fresh GameState in the same Unity process; the old instance remains locked and only the fresh instance can commit. No physical/process restart was tested.

Two actual isolated Unity captures were visually inspected: [Russian portrait](QA/Boundaries/Captures/LOR_SETTINGS_CAPTURE_DIR/Settings_Rejected_RU.png), 941×1672, and [English portrait](QA/Boundaries/Captures/LOR_SETTINGS_CAPTURE_DIR/Settings_Rejected_360_EN.png), 360×640. The existing dark panel and readable localized error text are visible over the unchanged Settings page with the committed switches. [Capture review](QA/CaptureReview.json) records hashes and provenance. Automated layout, input shielding, font/glyph and touch-target checks passed. These are Editor captures, not phone or newly approved design evidence; audibility, physical haptics and notification delivery were not measured.

Changed source/fixtures:

- `Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionSettingsView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/SettingsAlignmentPlayModeTests.cs`

The task-local [patch](Sources/TaskChanges.patch) reverses cleanly. Exactly four of 7,839 inputs changed, **7,835 unchanged**, none added/deleted. GameState outside preference toggles, Settings outside acknowledgement/callback ownership/error handling, and all earlier fixture checks are preserved. Existing approved Splash, art, metas/GUIDs, actual prefabs/builders, fonts, settings, packages, backend, Android guards and prior user edits remain intact. Four continuity documents retain earlier history and acceptance decisions.

Current development APK remains **766,031,502 bytes**, SHA256 `71e042f7eaef9edba33c16916c843aa28737b5432666f166aec1a59a0b3b3984`, with unchanged sidecar. Inspected packaged metadata lacks TryTogglePreference: **this APK does not yet include Settings recovery**. Source validation is not an Android compilation. No APK build, Git commit/push, personal-journal or credential access, phone/hosting action or store upload occurred.

**Next:** refresh the Android APK with Settings recovery through the existing guarded workflow, then update artifact pins and evidence. General Persist/reset/load lifecycle paths and the separate language preference remain outside this narrow patch. Human 201–210 recordings/ratings/review remain required before tuning; no rescan or tuning here. All 18 physical rows remain pending and phone/hosting deferred. Real paid/video settlement, RuStore production readiness and measured profitability remain unverified; stock-max remains restricted and effects stay last.
