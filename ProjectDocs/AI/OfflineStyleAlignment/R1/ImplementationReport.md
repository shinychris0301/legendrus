> Correction: the dedicated original was found among generically named downloads. See [path and hash](QA/OriginalReferenceLocated.json). Missing-reference statements below record the earlier incomplete search; R1 has not yet been aligned to this original.

# Visual Page 32 — Offline / No Connection R1

Implemented the active native OfflineState page using approved navy/gold surfaces, sapphire crest, cloud symbol, emerald Retry, blue Home and red Close. Russian and English text stays live/localizable. Responsive composition respects simulated safe areas and covers the full display with the existing clean scenery.

No dedicated original was found. An optional reference question was sent; work proceeded after allowing time for a reply, using the approved style. The old Wave04 preview was inspected and is explicitly a legacy preview, not an original or current runtime capture. No pixel-perfect reference claim is made.

## Behavior

The old Retry hotspot simply navigated Home. Retry now re-reads Application.internetReachability and refreshes the screen without leaving or modifying save/inventory. No network yields Wi-Fi/mobile-data guidance. A detected network yields a separate state explicitly stating that access to game services is unconfirmed. Changes after returning from device settings are rechecked on application focus. Language changes preserve the state.

This is a **device network-availability check**, not a verified internet/server request. The source contains no configured game-service health endpoint. No arbitrary external URL, fake progress timer, successful cloud sync, currency grant or automatic route change was added. Backend availability checks and service-triggered presentation remain a later integration task. The route is available through the existing ScreenId.OfflineState mechanism; no new global offline interceptor blocks local gameplay.

Back to Home, Close and system Back retain the existing Home destination. Local-play guidance explains that available levels and on-device saving remain usable. Repeated exit clicks and rebinding are guarded. Existing network-only features keep their own service gates.

## Art and scope

Seven existing sprite resources and two embedded font resources reused unchanged. The cloud's offline slash is native UI geometry. **Zero new Scenario calls,0CU, zero image edits.** Existing upstream Scenario provenance remains attached; reuse is not counted as generation. No screenshot backdrop/hotspots and no duplicated Canvas/EventSystem.

Added native ProductionOfflineRuntime/View/Layout, owning ProductionOfflinePrefabBuilder, canonical Pages/OfflineState.prefab and revision1 cutover, plus Edit/Play test fixtures and metas:16 new Assets files. The sole existing production file changed is ScreenRouter.cs for native action binding. All prior metas/GUIDs, artwork, Tutorial, Gameplay, Splash, other pages, Packages, ProjectSettings and Android guards match the5,652-file start snapshot; no deletions.

## Checks performed

- Unity6000.5.9f1 C# compilation/import, prefab rebuild, binding/readback and revision1 cutover passed.
- Verify01: **5 Edit Mode +8 Play Mode =13 tests passed**, zero failures/skips. Six Offline tests and seven Tutorial regressions cover native bindings/dependencies, idempotent runtime construction, offline/network-detected/cellular/lost-network states, focus refresh, language changes, Retry re-probing without false service success, no route/save mutation, rebinding, repeated exits, Home/Close/system Back and safe-area input targets.
- Six actual Offline captures at RU941×1672, safe EN576×1280 and safe RU1080×2400, with injected reachability states. Nine Tutorial regression captures retained separately. RU offline/retry and tall EN network-detected states visually inspected.
- Source/service/legacy-preview inspection; all reused-resource hashes and production scope audited; source whitespace and verification-script syntax checked. Browser390px loaded all6views with no errors, missing links or horizontal overflow; QA/BrowserReview.json retained.

## Not performed

Real airplane-mode/reconnection/captive-portal tests, internet/server health requests, APK build, physical phone/touch validation, full-project test suite, profiling or backend integration. Existing asset alpha quality is inherited from earlier verified pages; no new alpha extraction was performed. Exact source-font identity remains unknown.

## Next action

Review Offline_Alignment_R1_Review.html. Later connect verified game-service availability and real-device network tests when the corresponding service is implemented. The next planned visual page is Maintenance.
