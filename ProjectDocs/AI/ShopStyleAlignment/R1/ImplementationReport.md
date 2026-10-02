# Shop R1 — implementation and verification

Visual Page 12 now uses `Resources/ProductionUI/Pages/Shop.prefab` through the existing meta-page cutover registry. Replaced the previous reference-screen/hotspot Shop route with native controls. No new startup system or Canvas. Existing `RequestPurchase`, offer-view analytics, commercial entitlement services and other page routes remain intact.

## Delivered

- Four separate product subjects from the original Shop reference, retaining original RGB with Scenario alpha isolation; original title plaque and independent UI boundaries, repaired blank faces for live labels, clean empty product-card material, carved wood environment, native HUD and bottom navigation.
- Live saved gold/gems/energy/player level; approved Home Ivan portrait/ring and resource art reused without changing those resources.
- Russian static original Shop heading, English live heading; live localized labels throughout the remainder. Exact font identity remains unverified; existing typography resolver reused and no OS font files copied.
- Native Offers/Coins/Bundles preview filtering; explicit unavailable Gems/Boosters/VIP categories; profile/settings/back/special-offer/energy routes; no synthetic notification badges or claimed discounts.
- Three existing offer IDs remain mapped to the original purchase callback. All four purchase controls are disabled because neither an approved product-display catalog nor IAP entitlement recipes exist. Merely binding a runtime port does not enable them. The fourth reference product has no invented commercial ID. No balances or rewards are granted by viewing/filtering the page.
- Chapter Complete real SMS phone verification scheduled in `DeferredFeatures/CHAPTER-SMS-01.md` and DevelopmentSchedule after UI design. No SMS backend/provider/OTP flow implemented now.

## Scenario and image preparation

Six accepted jobs; **27 CU** recorded in billing receipts: five alpha requests at3 CU each, one clean empty-card generation at12 CU. No automatic retries or extra generations. The title alpha retained surrounding wood and was not installed; a tighter original-pixel contour was used. Product RGB stayed byte-identical to its source crops. Nav notification overlaps were locally repaired from opposite arcs. Price/quantity text was replaced with clean material, never presented as authoritative amounts. Transparent sprites inspected over black, white and magenta. Crops remain reference staging; runtime dependencies contain neither DesignScreens nor ReferenceOnly textures.

Reproduction/provenance: `Sources/ScenarioAssets.py`, request/job/billing records, `Sources/PrepareArt.py`, `Sources/Provenance.json`, `Sources/PreparationManifest.json`. Reused prior approved Scenario navy/wood material. New Shop-only importer uses RGBA32/uncompressed, bilinear/no-mipmap sprites.

## Checks actually performed

Final `QA/run5`, Unity6000.5.9f1:
- Compile, import, owning builder rebuild, native prefab/cutover readback: passed.
- Edit Mode **4/4**: Shop2 + ChapterComplete2. Native dependencies, required bindings, no extra Canvas/hotspots, imported art settings, repeated-build stability and disabled unverified purchase state.
- Play Mode **6/6**: Shop3 + ChapterComplete3. Shop live HUD update, RU/EN localization, disabled purchase invocation, repeated binding, no save mutation during browsing, category filtering/empty state, profile/settings/special-offer/energy/back navigation exactly once. Existing chapter reward/progression/localization/navigation regression checks also passed.
- Seven Shop PNG/JSON pairs:941×1672RU/EN,576×1280safeRU/EN,1080×2400safeRU,CoinsRU,GemsUnavailableRU. Six ChapterComplete regression PNG/JSON pairs. Capture checks include button safe-area bounds/raycast targets and live-text fit.
- Visual review of actual Shop captures and source comparison recorded in ReferenceComparison. Source RGB/alpha, prepared/runtime hashes, preserved files/GUIDs, Python compilation and local review links checked separately in QA.

Earlier evidence is retained: run1 caught missing no-mipmap Shop import policy; run2 caught wrapped bottom-navigation text. Both fixed. Run3 passed automated checks but visual review found square portrait corners, clipped title scrolls and material seams. Run4 passed; visual review identified a small original coin remnant and distorted empty-state panel. Final run5 includes those fixes.

## Change scope

Start-of-task production snapshot: **4567** files. **4565 unchanged**, **2 existing files changed**, **63 new files**, **0 deleted**. All **2395 existing meta files** retained byte-for-byte. Only existing production changes this task are ScreenRouter's Shop binding/HUD ownership and the Shop-only texture-import branch. New runtime/view/layout, prefab owner, tests, cutover state, sprites/prefab and their metas are listed in ChangedFiles. Previous pages, Android guards, packages, signing and project settings were not intentionally changed.

## Not performed

No APK/AAB build, physical-phone test, payment/provider sandbox test, SMS-provider integration/test, full-project test suite, browser execution, performance profiling, calibrated color measurement or exact-font verification. Do not treat automated test success as exact reference fidelity or user acceptance.

Next: review Shop R1 and check readability/touch targets on a physical phone. Finish remaining UI pages before the scheduled continuation/SMS implementation stage; real Shop sales require verified catalog data and entitlement integration.
