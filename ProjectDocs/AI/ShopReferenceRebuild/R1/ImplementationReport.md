# Shop reference rebuild R1

Implemented 2026-09-21 following the user's instruction to continue after Chapter Complete. Active Shop prefab, runtime and cutover now use native revision 2.

## Delivered changes

- Uses the same `ProductionPlayerHeader` component and Home source assets as Home, World Map and Failure. Original button instances, saved resource values and navigation callbacks are retained. The previous reconstructed header graphics are inactive.
- Original gold product-card perimeter replaces the earlier generated perimeter. A Scenario-repaired parchment surface fills the occupied interior; the four existing source-isolated product subjects remain unchanged.
- Russian product names, category captions and footer captions extracted from the source. English remains live and localizable. Existing source Russian Shop heading retained. Raster lettering uses its native dimensions.
- Original carved arch restored behind the title. The portrait-occluded part of the arch uses a bounded opposite-side donor. Title crest renders above the shared header, including after repeated builds.
- Source navigation medallions preserve their original aspect and fuller rims; original notification badges are omitted. Their hidden rim segments use mirrored source donors. English captions use a wide blank plaque rather than the narrow category tab. Selection still updates the active category and caption tint.

Reference: `/home/k2/Downloads/match3final/8.shop.png`. SHA-256 `6b0ae5f9f9de74864b3e06f2fa9791a5e1d2abbaea825c4c91a7713b717119cb`.

## Asset workflow and provenance

**One accepted Scenario request, 12 CU, no paid retries or duplicate requests.** `model_openai-gpt-image-2` removes the bag, coins, sample words, amount and price controls from a single card face. Only its repaired interior is composited back; the original border is retained. Request, prompt, raw output and billing are under [Sources/Scenario/card](Sources/Scenario/card). Preparation and exact asset hashes are in [Sources/PrepareAssets.py](Sources/PrepareAssets.py) and [Sources/AssetProvenance.json](Sources/AssetProvenance.json).

The 30,245 pixels outside the card repair mask have unchanged original RGB (zero mismatches). Local lettering alpha, medallion contours and caption surfaces are recorded in the preparation script. Intentionally opaque card/nav faces are retained. New assets were inspected against black, white and magenta. These extractions still have source-resolution and fine-alpha limitations; repaired hidden surfaces are not exact recovered pixels.

## Behavior and deliberate reference differences

Existing Offers/Coins/Bundles filters, unavailable-category state, Profile/Settings/Back/Special Offer/Energy routes and purchase callback mappings are preserved. Original reference prices, reward amounts, promotion ribbons and notifications are illustrative and are not asserted as live catalog data. Purchases remain disabled because there is no verified display catalog or entitlement integration. Browsing does not grant rewards or mutate balances. XP remains unavailable in the shared header rather than displaying fabricated progress.

The approved shared header differs slightly from the Shop reference's own portrait/resource proportions. Existing aspect-fit layout continues to add wood margins on taller screens. Live English typography, fine alpha, reconstructed blank surfaces and hidden arch material remain approximate. Source selected-tab blue styling and commercial content are not fully identical. No pixel-perfect match is claimed.

## Verification

Unity 6000.5.9f1, final **Verify05**: importer, owning builder and prefab readback passed; **4 Edit Mode + 6 Play Mode tests passed** (Shop and Chapter Complete). Coverage includes serialized bindings/import rules, stable repeated builds, crest layer order, live balances, RU/EN selection, disabled purchases, no save mutation, category filtering, routes invoked once, safe-area/raycast checks and text fit. Seven actual Shop captures cover RU/EN, tall safe areas, 1080×2400, Coins and unavailable Gems; six Chapter Complete regression captures also recorded. Nine browser review choices load without errors or horizontal overflow. Source scripts parse successfully.

Verify01 exposed an edge-boundary test using exclusive Rect.Contains maximums; replaced with explicit inclusive bounds and half-pixel rounding tolerance. Later visual inspection found title/header layering and medallion scaling defects, then the narrow English footer plaque. Each was corrected and verified; intermediate evidence retained. Black/white/magenta alpha and native full-size/tall/empty views inspected.

Not run: full-project suite, Android build, physical-phone testing, IAP sandbox or provider tests. Existing payment, ad continuation and SMS development remains outside this visual task.

## Scope

9 existing Assets files changed; 44 added (22 PNGs and metas); 0 deleted. All 380 prior prefab records retained; 451 total after new source lettering and header bindings. Other page prefabs and existing artwork/metas match the task-start snapshot. Detailed scope: [QA/ScopeAndVerification.json](QA/ScopeAndVerification.json).

- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/ShopAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/ShopAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionShopPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionShopView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionNavigationStyle.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionShopRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPlayerHeader.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Shop.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/ShopCutoverState.txt`

[Open review](Shop_ReferenceRebuild_R1_Review.html). Next: user visual review, then Special Offer or a requested Shop refinement.
