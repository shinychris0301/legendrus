# Special Offer R3 — Store removal and full resource header

The Store button is removed from the displayed page, following the user's latest instruction. The original Close control remains functional. The shared player header now shows coins, gems and the saved life/energy value.

## Implementation

The energy indicator was not missing from the shared component: Special Offer explicitly hid capsule 2 in both its header attachment and content refresh. Those two overrides are removed. This supersedes the earlier request to hide the upper-right indicator. The existing lightning icon and live energy/max counter are reused; no separate life balance is invented. The dimmed header remains noninteractive behind the modal.

The Store button GameObject is inactive in the canonical prefab and on every content/localization refresh, preserving its serialized reference for compatibility. It renders no image or text and receives no pointer input. The native revision and builder/cutover revision are 4. The Close route back to the entry page is preserved.

A Store plaque extraction was prepared before the user changed the request. It is archived only as `Sources/UnusedStoreExtraction.png`; it is not a runtime asset or displayed element. No new Scenario requests or credits were used. Existing button/title lettering and purchase availability are unchanged.

## Verification

Final `Verify03`, Unity 6000.5.9f1: compilation, prefab rebuild and readback succeeded; **4 Edit Mode + 6 Play Mode = 10 checks passed** across Special Offer and Shop. Checks cover hidden Store in both languages, visible shared energy, saved 7/30 counter, header input blocking, live balances, safe-area text/controls, disabled purchasing/no grants, repeat-build stability and Close navigation exactly once.

Five actual native captures are provided. Russian 941×1672 and English 576×1280 safe-area captures were visually inspected: no Store control appears, and the third resource is visible. Browser review: **7 views**, no script errors or horizontal overflow. Earlier Verify01/02 pre-removal checks are archived as historical evidence, not the delivered state.

No Android/device/full-suite/live-payment verification ran. Existing repaired surfaces, live English typography and tall aspect-fit margins retain their prior limitations.

## Scope

8 existing Assets files modified; 0 added; 0 deleted against the task-start snapshot. All 225 original prefab records retained. Other page prefabs and existing artwork/metas unchanged. Shared-header change is confined to the Special Offer attachment.

- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/SpecialOfferAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/SpecialOfferAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionSpecialOfferPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionSpecialOfferRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionSpecialOfferView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPlayerHeader.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/SpecialOffer.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/SpecialOfferCutoverState.txt`
