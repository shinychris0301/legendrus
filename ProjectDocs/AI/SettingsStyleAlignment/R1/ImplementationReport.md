# Visual Page 28 — Settings R1

Implemented in Unity 6000.5.9f1 following the user's approval. Dedicated `6.settings.png` matches the existing DesignScreens reference byte-for-byte. The historical Splash/Loading handoff is superseded by the verified Inventory milestone; approved earlier pages and local edits are preserved.

## Native page and behavior

New active `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Settings.prefab`, owning `ProductionSettingsPrefabBuilder`, runtime/view/layout and revision-1 cutover state. ScreenRouter binds native controls in both full-page and Pause-modal paths. Old Wave04/reference files remain unchanged. No duplicate Canvas, generic HUD, legacy demo shortcuts or screenshot page.

Music, Sound, Vibration and Notifications use the existing GameState toggles and save persistence. Source switches show a green right-hand knob when on and a gray left-hand knob when off. State changes refresh immediately; music/SFX/vibration gating is observed through the existing feedback service. Notification preference is saved; this pass does not implement OS permission requests or notification delivery.

Language opens a native RU/EN selector and updates live text in place. It uses the established session language state; persistence across app restarts is not added. Original Russian title artwork retained, English uses the selected embedded title font. Other text uses the established player-name serif. Exact source-font identity remains unverified. Version uses Application.version.

Cloud honestly reads “Local save”. Account, Cloud, Support, Privacy and Sign Out display explanatory unavailable states. No authenticated account, sync, contact transmission, policy acceptance, consent change, logout or save deletion is fabricated. Existing PrivacySupportService boundaries were inspected and preserved. Header resources remain live; Profile and resource buttons use existing routes/categories on the standalone page. In paused gameplay they explain that Settings must be closed first, retaining the current board. The header gear represents the current page.

Back dismisses an internal explanation first. Standalone Settings returns to the page that opened it, including Splash, Profile and Inventory. Settings opened from Pause returns to Pause with the same board/hash/moves/score and input still paused; Resume then restores input. Repeated route clicks commit once, repeated binding does not duplicate preference listeners. Informational overlays block underlying controls.

## Artwork and Scenario

18 accepted source-alpha isolation jobs, **54 CU** total; zero new RGB generations. Original title, ten icons, toggle, arrow, Back, paper/row/sign-out frames and outer carved border isolated. One oversized frame POST returned HTTP413 without an accepted job. A smaller, separately recorded native-size input succeeded; no automatic paid resubmission. All records and immutable inputs retained.

20 runtime PNGs including a blank English title and off-switch derivative. Original RGB is retained outside recorded local repair masks. Reused approved Inventory parchment restores the hidden paper face; a clean source navy donor restores row faces, with a red tint for Sign Out. Outer-frame masking removes all original content; individual rows/text/controls are separate objects. Source resampling adds no original detail. [Provenance](AssetProvenance.json), [Scenario records](QA/ScenarioAudit.json), [source workflow](Sources/README.md) and [reused art](ReusedArt.json).

## Checks performed

- Unity C# compilation/import, scoped owner rebuild and prefab serialized readback passed.
- **6 Edit Mode + 14 Play Mode = 20 checks passed**, zero failures/skips, covering Settings and Inventory/Pause regressions. Settings-specific 2 Edit + 5 Play checks cover bindings/idempotency, every preference persisted independently and refreshed after reopening, existing feedback gating, language/overlay input, unchanged economy/consent, origin return, header routes/categories, duplicate clicks/listeners and paused board preservation.
- Initial import caught an unnecessary direct compliance-assembly dependency in the test; corrected to inspect the existing boundary without changing assemblies. First full passing captures exposed five legacy debug shortcuts outside the native prefab; removed both full-page and modal copies and added a router-level assertion. Final broad rerun passed all 20 checks; after the final tiny icon-alpha cleanup, focused 2 Edit + 5 Play Settings checks passed again. 20 distinct latest checks. Earlier run evidence preserved.
- 10 final actual Settings captures: RU/EN reference size, 576×1280 and 1080×2400 simulated safe areas, all-off/mixed preference states, language dialog, cloud explanation and Settings from Pause. 20 Inventory/Pause regression captures. Isolated save storage used; state restored after tests.
- Visual comparison of final reference-size RU, phone EN, switches and explanation/modal states; black/white/magenta alpha inspection. Fixed title edge clipping, icon-neighbor debris and English title accent remnants during preparation.
- Verified original RGB outside repair masks, immutable submitted input hashes, runtime hashes, Python syntax and scoped whitespace. Scope: 57 new Assets files including metas; two pre-existing source files changed (ScreenRouter and Pause regression fixture). All other pre-existing Assets/GUID/meta/package/ProjectSettings hashes unchanged; no deletions. [Exact scope](QA/ScopeAudit.json).

Browser verification at 390px loaded all 11 views without script errors, missing links or horizontal overflow. Evidence is recorded in `QA/BrowserReview.json`.

## Limits and next action

Not performed: APK build, physical-phone touch/audio/haptics/notification permissions, performance profiling, full test suite/all-page regression, live account/cloud/support/legal integrations, calibrated color/pixel comparison or exact-font identification. This is not an exact replica: [reference differences](QA/ReferenceFidelityReview.md).

Next: review the actual Settings portraits and both switch states. Post-UI feature work: authenticated account/sync/sign-out, configured support/legal destinations and consent controls, notification permission/delivery and persistent language preference. Preserve existing SMS and Failure-continuation scheduling.
