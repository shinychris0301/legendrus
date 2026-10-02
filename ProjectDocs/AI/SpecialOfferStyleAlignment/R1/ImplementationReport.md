# Visual Page13 — Special Offer R1

Replaced the active DemoFunctionalPageRuntime/hotspot SpecialOffer surface with `Resources/ProductionUI/Pages/SpecialOffer.prefab`, a native page bound through the existing meta cutover registry. The historical Wave02 prefab remains untouched and inactive. The owning `ProductionSpecialOfferPrefabBuilder` creates or updates the same prefab and cutover state outside Play Mode.

## Presentation and behavior

Shop-style dark wood, navy/gold title, parchment offer card, emerald purchase control, separate live labels and native Back/Shop actions. HUD reflects the saved gold/gems/energy, subscribes to GameState changes and responds to RU/EN language selection. One body composition scales uniformly within the safe area; clean wood covers the whole display. No new Canvas/EventSystem/startup path.

Reuses the clean approved PreLevel isolated chest (original RGB/alpha/GUID), Shop title/button/material assets and ChapterComplete reward tiles. The main card is a separate862×982 derivative of the original1024×1152 Scenario empty-card source, avoiding enlargement of Shop's smaller431×491 sprite. Alpha checked over black/white/magenta. No new generation/API calls or credits. Original Shop/PreLevel art is unchanged. Provenance and deterministic preparation are in Sources.

The original `starter` purchase identity and `RequestPurchase` service boundary remain connected. Purchasing is disabled: no verified price, entitlement recipe, discount or expiry data exists. The three decorative preview categories use unspecified amounts (`—`), accompanied by a clear message that contents are unannounced. No price/discount/countdown is invented and no rewards are granted by this page. Binding a provider alone cannot enable buying. Back preserves the router history and Shop returns to the existing native Shop.

Exact source font identity remains unverified; the existing local typography resolver is reused with live localized text. No OS fonts distributed. ChapterComplete SMS and Failure continuation stay scheduled after UI completion; neither feature is implemented by this task.

## Reference status

There is **no approved standalone SpecialOffer screenshot** locally. The historical Step83 source-candidate is explicitly style-system derived and contains screenshot fragments/sample commercial values. It is shown only as historical evidence in the review, never installed as runtime art. The result follows the approved visual family and Shop structure; no exact-match claim.

## Checks performed

Final `QA/run3`, Unity6000.5.9f1:
- Compilation, owner rebuild, serialized prefab/cutover readback: passed.
- **4/4 Edit Mode**: SpecialOffer2 + Shop2; required bindings, native controls, clean dependencies, no Canvas/hotspots/screenshot dependencies, repeated-build stability, original commercial identity and unavailable values.
- **6/6 Play Mode**: SpecialOffer3 + Shop3; live balance update, RU/EN text, safe-area bounds/raycast targets/text fit, chest/card/button aspect ratios, disabled purchase invocation after repeated binding, no save mutation while viewing, Shop entry/Back/Shop exactly-once navigation; existing Shop category/navigation checks also passed. Zero failed/skipped.
- Five SpecialOffer PNG/JSON capture pairs:941×1672RU/EN,576×1280safeRU/EN,1080×2400safeRU. Seven actual Shop regression pairs. Captures use an isolated save; the SpecialOffer fixture adds23gold to prove the live counter.
- Final actual941RU/EN and576safeRU/EN visually reviewed;1080safe checked automatically. Alpha QA, reused-source hashes, final production hashes, Python syntax, local HTML links and scoped whitespace checks are recorded in QA.

Earlier run1/run2 automated checks passed; visual inspection caused refinement. The enlarged Shop chest exposed a pre-existing quantity-plaque remnant. A local cleanup trial was rejected because the lower edge was clipped; the final page uses the complete high-resolution PreLevel chest instead. The smaller Shop card was replaced with a derivative of its full-resolution source. The rejected trial remains documentation-only, with no runtime reference.

## Scope preservation

Baseline **4630** production files: **4628 unchanged**, **2 existing files changed**, **19 new files**, **0 baseline files deleted**. All **2427 existing .meta files** retained byte-for-byte. Existing changes are limited to SpecialOffer router binding/HUD ownership and its texture-import branch. New view/runtime/layout/owner, native prefab/cutover state, frame and tests/metas are listed in ChangedFiles. Previous pages, services, packages, project settings, Android guards and signing stay unchanged.

## Not performed

No APK/AAB build, physical-phone check, store/provider/payment test, SMS integration, full-project suite, browser execution, extended manual gameplay/profiling, calibrated color or exact-font verification. UI availability is not a completed commercial feature. User visual acceptance remains pending.

Next: review the page and verify readability/touch targets on a phone. Real prices, contents, expiry and transactions require the later verified commercial catalog/entitlement stage.
