# Visual Page34 — Error Dialog R1

Implemented the located original as a native modal: navy/gold header, parchment panel, warning emblem, blue Retry, green Close and smaller Support control. Live RU/EN text. The existing current page remains underneath a full-canvas dimmer; it is not rebuilt or replaced by a screenshot. Input stays blocked through the modal exit animation.

## Behavior

`ScreenRouter.ShowErrorDialog(Action retryFailedOperation = null)` opens the modal over an idle meta page. It returns false if a screen/modal transition or another modal is active. A supplied callback is the actual operation to retry; it runs once **after** close, only if the route has not changed. Navigation before close finishes cancels the pending callback. The callback may start an asynchronous service request, but the dialog does not invent network success or own the service's error handling.

Without a callback, Retry is visibly disabled and the localized message explains how to retry from the page. Close/systemBack preserves the original page and its instance. Support is disabled and labelled unavailable; no contact address or outbound message is invented. `Show(EmptyErrorState)` uses this same no-callback modal instead of the old full-page placeholder/Home shortcut.

Gameplay integration is deliberately deferred: `ShowErrorDialog` rejects Gameplay, preserving the user's scope restriction. No automatic global exception handler, service interceptor, new gameplay pause flow, purchases or save mutations were added. Existing services still need to call the dialog with their real failed-operation callback when integrated.

## Artwork

Five Scenario Pixa source-alpha jobs cost **15CU**: panel, emblem, Retry, Close, Support. No new RGB generation or paid resubmission. Retry output retrieval disconnected once; same accepted output recovered. Original RGB outside recorded masks preserved. Original source, immutable prepared inputs, masks, requests, job/billing records and raw outputs are retained.

Occupied navy/parchment faces use source texture with blended repairs; button lettering cleared for live labels. An underlying Home notification badge contaminated the lower frame: its area was replaced with the mirrored corresponding left border. Opaque panel surfaces retained. Source colors outside final masks and input hashes verified; transparency inspected on black/white/magenta.

The background is the current implemented page, which differs from the source Home composition. Existing selected serif fonts remain approximate; exact source font or embossed lettering is not claimed. Restored hidden paper/navy texture is reconstructed. Support/Retry disabled states intentionally differ when their required functionality is unavailable.

## Verification

- Unity6000.5.9f1 compilation/import, canonical prefab rebuild/readback and revision1 cutover passed.
- Final Verify02: **6 Edit Mode +13 Play Mode =19 tests passed**, zero failures/skips. Seven Error Dialog checks plus12Offline/Maintenance regressions.
- Five actual Error Dialog captures: RU/EN941×1672, safeEN576×1280, safeRU1080×2400 and callback-enabled Retry RU. Ten adjacent-page regression captures.
- Checked prefab bindings/idempotence/no screenshot dependencies, localization, truthful disabled actions, native pointer Close/systemBack, unchanged save and underlying Home instance, delayed single Retry, navigation cancellation, duplicate/busy rejection, modal input blocking and portrait safe-area/input/text rendering.
- Initial Verify01 passed19tests, but visual inspection caught clipped fallback instructions. The fallback copy was shortened and complete-character rendering assertions added before Verify02.
- Browser390px: all6views loaded without errors, missing links or horizontal overflow. Source-script syntax and scoped whitespace checks passed.
- Source RGB/mask/input hashes, alpha backgrounds and production scope audited. Existing assets/prefabs/metas/GUIDs, Gameplay/Splash/other pages, packages/settings and Android guards unchanged.

Not performed: APK build, physical phone/touch, real service/support requests, Gameplay error integration, async backend failure/retry integration, full-project tests or profiling.

## Changed production files

- `Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/ErrorDialogAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/ErrorDialogAlignmentEditModeTests.cs.meta`
- `Assets/LegendsOfRus/Tests/PlayMode/ErrorDialogAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/ErrorDialogAlignmentPlayModeTests.cs.meta`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionErrorDialogPrefabBuilder.cs.meta`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionErrorDialogPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionErrorDialogView.cs.meta`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionErrorDialogView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionErrorDialogLayout.cs.meta`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionErrorDialogRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionErrorDialogRuntime.cs.meta`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionErrorDialogLayout.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/ErrorDialogAligned.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/EmptyErrorState.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/EmptyErrorState.prefab.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ErrorDialogAligned/panel.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ErrorDialogAligned/emblem.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ErrorDialogAligned/retry.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ErrorDialogAligned/emblem.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ErrorDialogAligned/close.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ErrorDialogAligned/support.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ErrorDialogAligned/panel.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ErrorDialogAligned/retry.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ErrorDialogAligned/support.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ErrorDialogAligned/close.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/EmptyErrorStateCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/EmptyErrorStateCutoverState.txt.meta`
