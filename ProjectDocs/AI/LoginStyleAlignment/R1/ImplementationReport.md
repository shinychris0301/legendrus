# Visual Page 30 — Login / Account R1

Native Login/Account UI follows the dedicated `4.login.png` reference: source Ivan and Firebird, ornate wood/parchment panel, emerald sign-in control, sapphire guest control, four provider icons, account-link control and password-eye icon. Approved Home logo and clean FrontEnd dusk scenery are reused. A visible Back control is added.

The page opens from Splash Account and the standalone Settings Account row. Back returns to the originating screen, including Settings' previous origin. Account in Settings opened over a paused round retains its explanatory state, so that route cannot discard the paused board.

## Current functionality

- Live RU/EN input placeholders, captions and unavailable-state descriptions; the original Russian panel heading is preserved, with an English blank-heading derivative.
- Native email/phone and masked password fields, character limits and working visibility toggle. Values exist only in the UI; they are cleared when leaving/hiding the page and before guest entry. No credential logging, storage or transmission is added.
- Guest entry uses the existing local guest callback and opens Home without resetting progress. Repeated binding/clicks commit only once.
- Sign-in, VK, OK, Mail.ru, Google and linking controls explain that online services are unavailable. The page displays that limitation beside the form. It does not simulate authentication or cloud protection.
- Terms/privacy controls explain that published destinations are not connected. The reference checkbox is shown empty and is not a legal-consent toggle; no consent is recorded or required to continue as a local guest.

Real authentication, provider configuration, account linking/sync and published legal destinations remain post-UI work, recorded in `ProjectDocs/AI/DeferredFeatures/AUTH-ACCOUNT-01.md`. Existing compliance approval guards are unchanged.

## Scenario and provenance

**18 accepted Scenario source-alpha jobs, 54CU total; no RGB generation.** One full-border output was rejected because its dark ornamentation disappeared. Three bounded corner isolations replaced it; the upper-right ornament mirrors the accepted upper-left. Seventeen accepted isolations plus the blank-header derivative produce 18 runtime PNGs.

Original RGB is retained outside declared local repair masks. Form captions are removed locally; the occupied paper interior uses a clean cropped donor from the approved Settings paper. The empty consent face uses source green material. Final alpha removes neighboring scenery/logo fragments and keeps deliberately opaque panel faces. Ivan/Firebird pixels hidden by the form remain absent and are covered by the separate panel, rather than being presented as recovered anatomy.

Submitted inputs are immutable and hash-verified. Assessment.json, masks, API requests/job IDs/billing/download hashes, rejected output and final provenance are retained under Sources. Enlargement by two is interpolation, not added source detail. No complete screenshot or scenery-contaminated rectangular crop is used as runtime UI.

## Implementation and checks

Active `ProductionUI/Pages/Login.prefab`, owning builder/runtime/view/layout and cutover revision1 created. Three existing files changed: ScreenRouter.cs, ProductionSettingsView.cs and SettingsAlignmentPlayModeTests.cs. Fifty-three new Assets files including metas. All pre-existing metas/GUIDs, Splash/Settings/Gameplay prefabs, earlier artwork, packages and ProjectSettings remain unchanged from task start. Exact paths: QA/ScopeAudit.json.

Unity6000.5.9f1 import, compilation, prefab rebuild/readback and idempotency passed. Broad Verify02: **8 Edit Mode +15 Play Mode tests passed**, zero failures/skips, across Login/Settings/LanguageSelect/Pause. Final Verify03 reran **2 Edit +4 Play Login tests**, all passed, after fixing the information-panel fill and full-screen dimming. These are **23 distinct latest checks**, not 29 distinct tests.

Verified pointer/raycast targets, input shielding, password visibility and clearing, explicit unavailable actions, unchanged game/consent state, guest single-commit behavior, Splash/Settings entry and Back origins, repeated navigation, RU/EN rendering and safe areas. Final seven actual Login captures at941×1672,576×1280 and1080×2400;25 neighboring Settings/LanguageSelect/Pause captures retained. Initial captures exposed corner scenery fragments, a wrapped link caption and letterboxed info panel; these were corrected before final verification. Final RU page and tall EN information state visually inspected.

Source RGB/input/runtime hashes, Python syntax, scoped source whitespace and production-file scope checks passed. Alpha resources inspected on black, white and magenta. Browser at390px loaded all eight views without errors, missing links or horizontal overflow (QA/BrowserReview.json).

Not performed: APK build, physical Android phone or keyboard/IME testing, real authentication/OAuth/SMS/cloud/legal integrations, full project suite, performance profiling or exact-font identification.

## Remaining differences

The clean background is reused approved scenery, so buildings, lighting and water placement differ from the source. Original character/Firebird artwork is retained, but edge cleanup and covered regions are not a layered original. Some fine corner details, rebuilt paper/blank button textures and live font weight differ. The upper-right corner is mirrored. Added Back/unavailability messages and inactive legal acceptance intentionally reflect the current implementation. Exact replication is not claimed.

Next: user review of Login/Account R1. Real account services follow the UI stage; do not enable provider or compliance flags as a substitute for integration.
