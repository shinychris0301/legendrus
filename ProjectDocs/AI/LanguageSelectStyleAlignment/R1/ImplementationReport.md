# Visual Page 29 — Language Selection R1

The separate LanguageSelect route now presents the approved Settings R2 modal over the existing Splash artwork: navy-and-gold panel, sapphire crest, original Russian heading, blue active-language face and red Close. It uses native Buttons and live language labels. The backdrop is a non-interactive hierarchy of the actual Splash resources, not a screenshot.

RU/EN selection is immediate and keeps the selector open. Close and Back return to the page that opened it. Returning through Settings also preserves Settings' own previous destination. The existing Splash button/guard still navigates once to LanguageSelect.

## Persistent language

User selection on either Settings or LanguageSelect writes the independent device preference `LegendsOfRus_Language_v1` and flushes PlayerPrefs. A fresh process reads it before any selection. Missing/unsupported values fall back to Russian; English is normalized case-insensitively. Game-progress save data, journal version, rewards and account state are not altered by this preference.

`DemoFeatureState.SetLanguage` remains an in-memory override for existing previews/tests. User-facing controls call `SelectLanguage`, which persists the choice; resetting demo-session state reloads the saved preference. This adds persistence to currently localized pages, not complete localization of every legacy screen.

## Assets and scope

No new Scenario calls or credits: five approved Settings R2 sprites are reused unchanged. Their earlier alpha extraction, repair history and original reference remain linked through AssetProvenance.json and Sources/README.md. Original title lettering is retained; live labels/English heading remain approximate. The reference shows Settings beneath the modal; this separate route uses the requested Splash context.

New native LanguageSelect prefab, owning builder/runtime/view/layout, cutover revision1 and LanguagePreference service. Existing changed files: ScreenRouter.cs, DemoFeatureState.cs, ProductionSettingsView.cs and SettingsAlignmentPlayModeTests.cs (restores the independent preference after tests). All existing metas, Splash assets/prefab, Settings prefab, Gameplay, packages and ProjectSettings preserved. Exact paths and hashes: QA/ScopeAudit.json.

## Checks

Unity 6000.5.9f1 import, compilation, prefab rebuild/readback and idempotency checks performed. Final broad run: six Edit Mode tests passed; twelve of thirteen Play Mode tests passed across LanguageSelect, Settings, Pause and Splash. The only failure is the pre-existing strict Splash round-trip image equality assertion: one pixel differs by one channel level. No assertion was relaxed and no Splash art changed. The follow-up isolated Splash run passed both tests, including exact round-trip pixel equality. Latest results therefore cover **6 Edit Mode + 13 distinct Play Mode tests passing**, plus the two fresh-process persistence phases. The broad-run single-pixel failure remains recorded; its cause is not proven.

LanguageSelect checks cover actual pointer selection, selected-face state, input shielding, disabled backdrop buttons, RU/EN portrait and safe-area layouts, Close/Back, repeated listener binding, Settings/origin return, fallback values, session reload and unchanged progress. Settings/Pause regression checks pass.

Two additional fresh Unity processes share isolated storage: the first writes English through the production setter; the second confirms English is loaded before interaction and displayed on Splash. Both passed.

Five actual LanguageSelect captures at 941×1672, 576×1280 and 1080×2400 simulated safe areas; fourteen Settings, six Pause and seven Splash captures retained. RU and tall EN selector captures visually inspected. Source reuse hash audit and scoped source-whitespace audit passed. Browser at 390px loaded all six views without errors, missing links or horizontal overflow (QA/BrowserReview.json).

Initial builder attempt failed on Unity's missing-component null behavior; explicit component validation fixed it before successful rebuild. Earlier evidence is retained.

Not performed: Android APK build, physical-phone/cold-device restart test, full project test suite, performance profiling, online account/cloud synchronization or expanded localization. Separate desktop Unity-process persistence is verified; Android storage behavior has not been device-tested.

Next: user review of LanguageSelect R1, followed by Visual Page 30 — Login / Account UI.
