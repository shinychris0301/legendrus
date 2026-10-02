# Special Offer reference rebuild R1

The user confirmed `/home/k2/Downloads/match3final/tuturial.png` as the Special Offer reference. Native revision 2 replaces the earlier Shop-derived page with its original Russian title, gold perimeter, open treasure composition and red Close control. The canonical prefab and owning builder/runtime were rebuilt together.

## Artwork

Two accepted Scenario jobs completed, with no paid retries:

- Pixa background removal: fresh isolation of the source chest, coins, bag, scroll, bottle, energy card and compass; 3 CU.
- OpenAI image model through Scenario: removal of occupied panel content and reconstruction of its village, platform and navy material; 13 CU.

Total: **16 CU**. Requests, outputs, job records and returned billing are archived in `Sources/Scenario`. Original chest RGB is retained; only alpha changed, with a short ribbon-tip restoration and removal of isolated extraction specks. Source warm sparks are a separate transparent layer. The repaired material is confined to recorded interior masks, and the perimeter/crest and Russian title use original art with the earlier approved extraction alpha. The Close control and emerald button reuse the earlier extraction of this same source. English uses a separately cleared title face and live lettering. The repaired surface and hidden corner are reconstructed, not claimed to be original pixels.

The five new sprites were inspected over black, white and magenta. Native Russian and English captures were inspected at source and tall proportions. The chest is isolated from its source backdrop; the opaque panel interior is intentional.

## Behavior and intentional differences

The Home shared player header is a dimmed, noninteractive backdrop with live saved balances. Its energy indicator remains hidden as previously requested. No header input can pass through the modal composition. Close and Shop retain existing navigation and duplicate-click guards.

The offer catalog remains unconfigured. The existing three preview categories (Coins, Gems, Bonuses) are retained with unspecified amounts; the reference's six sample entitlements, prices, discount, savings and expiry are not represented as real commerce. Purchase visibly reads Unavailable and cannot grant rewards. The source bottom quest/navigation sample is replaced by the existing functional Shop return control. English/live fonts approximate the source; tall screens retain aspect-fit spacing. This is a reference-aligned implementation, not pixel-identical reproduction of the illustrative commercial page.

## Verification

Unity **6000.5.9f1**, final `Verify03`:

- Compilation, canonical prefab rebuild and serialized readback succeeded.
- **4 Edit Mode + 6 Play Mode checks passed**, covering Special Offer and Shop regression.
- Checks include revision/bindings, repeat-build stability, live balances, RU/EN text fitting, safe areas, header input blocking, unavailable purchase/no grant, save stability and one-time navigation.
- Five actual Special Offer captures: 941×1672 RU/EN, 576×1280 safe-area RU/EN, 1080×2400 safe-area RU.
- Review browser check: **7 views**, no script errors or horizontal overflow at 390×844.

`Verify01` stopped during rebuild because Unity returned a missing CanvasGroup object through null-coalescing. An explicit Unity null check fixed this; `Verify02` passed. Visual inspection then removed small isolated alpha specks; `Verify03` is the final result. Earlier logs are retained and are not reported as successful runs.

No Android build, physical-device test, full test suite or live purchase validation ran.

## Scope

Compared with the task-start Assets snapshot: **10 existing files modified, 10 new files added, zero deleted**. All **164 prior prefab records remain** (225 records total). Existing art/metas and other page prefabs are unchanged. Shared-header/navigation additions are confined to the Special Offer path; Shop regression passed.

Modified files:

- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/SpecialOfferAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/SpecialOfferAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionSpecialOfferPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionSpecialOfferLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionSpecialOfferRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionNavigationStyle.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionSpecialOfferView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPlayerHeader.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/SpecialOffer.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/SpecialOfferCutoverState.txt`

Added: five sprites and their metas under `Resources/ProductionUI/SpecialOfferAligned` (`panel_ru_r2`, `panel_blank_r2`, `hero_source_r2`, `hero_glow_r2`, `reward_frame_r2`). Hashes, scope audit, masks, scripts and test evidence accompany this review.

Next: user visual review; then Daily Reward or any requested Special Offer refinement. No further Scenario generation is scheduled. Previously deferred paid continuation, ads and SMS tasks remain in feature development.
