# Special Offer R2 — original button and explicit header

User asked whether the background header is shared, reported a missing title, and requested the button with its original text.

## Findings and change

Yes: the dimmed player/resource bar uses `ProductionPlayerHeader`, also used by Home and Shop. This component and its source assets were not modified in this pass. Its noninteractive CanvasGroup and hidden energy indicator are preserved.

The previous Russian screenshot already displayed the title baked into `panel_ru_r2`, while the separately bound `Header` object was inactive. A completely missing title was not reproduced in that archived image. The request was treated as referring to the modal title; an optional clarification about a background page title was sent. Native revision 3 now activates the bound Header explicitly with separate Russian/blank title sprites. The original Russian lettering, crest and divider are retained; English remains live. The corresponding top region of the panel body is transparent so the edge is not composited twice. Tests now check header visibility and the correct title sprite in both languages.

The emerald button was extracted from source rectangle `(173,1263)-(770,1388)` of the approved `tuturial.png`, retaining the complete **КУПИТЬ СЕЙЧАС** lettering. The existing Scenario-extracted alpha from the same button is reused. All 74,625 RGB pixels match the original crop (zero mismatches). No generated lettering or new Scenario request: **0 CU this pass**. English shows live **BUY NOW** on the existing blank button face.

The button remains disabled because a verified offer catalog has not been configured. The live availability message below it and price-unavailable label remain visible. This visual change does not enable billing, change prices or grant rewards.

## Verification

Unity 6000.5.9f1 `Verify01`: compilation, native prefab rebuild and readback passed. **4 Edit Mode + 6 Play Mode = 10 checks passed**, covering Special Offer and Shop regression. Includes RU/EN header/button states, safe-area and text bounds, shared-header input blocking, unavailable purchase/no grant, live balances, repeat-build stability and one-time navigation.

Black, white and magenta alpha previews inspected. Actual Unity 941×1672 Russian and 576×1280 safe-area English captures inspected; five native captures available in the review. Browser: **7 views**, no script errors or horizontal overflow. No Android/device/full-suite/live-payment verification.

Scope against task-start hashes: **7 existing Assets files changed, 8 added, none deleted**. All **225 prefab records retained**. Existing artwork/metas, shared header code and other page prefabs are unchanged.

Modified files:

- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/SpecialOfferAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/SpecialOfferAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionSpecialOfferPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionSpecialOfferRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionSpecialOfferView.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/SpecialOffer.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/SpecialOfferCutoverState.txt`

Added: four sprite/meta pairs under `SpecialOfferAligned`: `buy_lettered_r3`, `header_ru_r3`, `header_blank_r3`, `panel_body_r3`. Provenance, scripts, alpha previews, scope audit and native test evidence accompany this report.

Remaining limits: English fonts and previously repaired panel material approximate the source; tall aspect-fit margins remain. If the missing title refers to a separate page title behind the modal, that is distinct from the verified modal heading and requires the pending clarification. No visual acceptance is inferred.
