# Visual Page26 — Profile R1

Implemented in Unity6000.5.9f1. Dedicated original located among generic filenames, copied unchanged and registered. User explicitly approved real saved gameplay statistics and labelled hero/relic previews. Existing local changes and accepted earlier pages preserved.

## Native implementation

The active page is `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Profile.prefab`, owned by `ProductionProfilePrefabBuilder`, with view/runtime/layout and cutover revision1. ScreenRouter binds native actions, dismisses the explanation first on Back and suppresses the duplicate generic HUD. The legacy Wave02 prefab is preserved but superseded by the registered native page.

Original Russian title, framed Ivan portrait, five framed hero illustrations, six relic cards, six stat symbols, pencil and footer icons are independent sprites. Other labels remain live RU/EN using existing selected embedded fonts. Actual source typeface remains unverified. Approved shared account header, clean environment, tab/frame surfaces and booster artwork are reused without changing their files.

Header balances and level update from GameState. Six statistics show best score, cleared levels, attempts, stars, chapter and highest unlocked level. First-chapter progress uses authored catalog IDs. Records are copied, filtered, deduplicated and clamped; long totals avoid attempt-count overflow. No player-save writes from this page. Saved-level history is ordered by chapter/level, not falsely presented as timestamped sessions.

Heroes/Collection show a combined collection preview; Relics focuses on six selectable relics. Example star art is explicitly illustrative, collectible levels are em-dashes. Items shows the actual four booster quantities. Achievements contains three informational progress milestones with no rewards. History has empty and six-record pagination states. Identity editing and collectible upgrades explain their unavailable state. Ranking, Team Hub, Inventory and Account routes remain accessible through Profile Actions; Back returns Home. No online identity or collection ownership is invented.

## Assets and Scenario

25 successful `model_pixa-background-removal` jobs cost75CU. Two tight footer crops were rejected and replaced with wider crops;23 accepted source isolations plus an English blank-title derivative yield24 runtime sprites. No RGB generation and no repeated paid submission of an existing job. All submitted inputs hash-verified and retained unchanged. Scenario supplies exterior alpha; bounded local repair restores intentionally opaque framed-card faces, clears captions/levels, preserves source stars, removes a neighboring title baseline and restores complete source contours. The main portrait's original pencil is locally cleared and added as a separate interactive sprite. Clean navy caption donor and masks are recorded. RGB outside final repair masks and all runtime sprite hashes verified. See [provenance](AssetProvenance.json), [repair notes](Sources/LocalRepairNotes.md) and [billing audit](QA/ScenarioAudit.json).

## Performed verification

- Unity import, C# compilation, scoped prefab rebuild and serialized readback passed.
- Broad7 Edit Mode +18 Play Mode checks across Profile/Mail/FriendsList passed. Final focused3 Edit +6 Play Profile checks passed after visual corrections.25 distinct latest checks pass; no failures or skips.
- Checked native bindings, idempotent builder, no runtime screenshot dependencies, copied/sanitized duplicate records, real stats and overflow, safe areas, visible glyphs/text fit, centered button raycasts, native routing/repeated-click protection, modal input guards, RU/EN, collection selection, real booster counts, milestone/history pagination and save preservation.
-15 final actual Profile Unity captures at941×1672,576×1280 and1080×2400, with simulated safe areas, plus28 neighboring-page captures. Tests use isolated storage, restore state and do not overwrite real progress.
- Visually reviewed main RU/EN, phone-safe layout, relic detail, items, history and milestones. Final corrections include missing card borders, clipped footer art, original caption remnants, footer label margins and progress-fill aspect behavior.
- Inspected final alpha over black, white and magenta. Source Python syntax, submitted-source/runtime hashes and scoped whitespace checked. Scope audit:65 new Assets files including metas; only pre-existing ScreenRouter changed. No prior Assets, existing GUID/meta files, packages or ProjectSettings changed besides that router.

Browser390px loaded all16 review views without script errors, missing links or horizontal overflow; evidence in `QA/BrowserReview.json`.

## Not performed / limitations

No APK build, physical Android phone, performance profiling, full test suite/all-page regression, live account/backend, collection upgrades or calibrated color/pixel comparison. The small Edit control needs a physical touch review. Reference differences are documented separately; this is not an exact replica.

Feature phase after UI: authenticated profile/name/avatar editing, defined hero/relic ownership and upgrade economy, authoritative milestone/reward rules and synchronization/conflict handling. Existing deferred SMS and Failure continuation work remain deferred and unchanged.
