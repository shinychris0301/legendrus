# Inventory — Reference Rebuild R1

Inventory now uses the shared Home player-header component, including energy, and the approved Shop footer medallions and Russian label artwork. English captions remain live and localizable. Header and footer follow the simulated phone's safe edges; the detail group centers within the extra height on taller screens. The information overlay extends its input shield through that space.

The selected item receives an empty-center gold perimeter through the existing map border component, instead of a stretched filled tab. Only one item is selected. Four owned boosters reuse the approved circular metal-rimmed shared art and display exact saved quantities on wide framed value panels. Quantities reuse the approved numeral font and remain readable for large values. No boosters are consumed by viewing or selecting items.

Approved Home scenery, Profile wood behind the title and carved-footer material bring the page into the existing visual family. All20 original framed collectible examples and title/feather/parchment resources are retained. Tabs, uppercase item headings and detail copy have clearer spacing and restrained font sizes; item availability remains truthful. Local buttons reuse existing map press feedback without adding sound or haptics. Disabled Use is visibly muted.

This is a native interactive page, not a reference image with hotspots. Collectible ownership, crafting and inventory use remain unavailable and labelled previews. Reference artwork, source-font identity and reconstructed surfaces are not claimed exact. No Scenario calls were needed:0calls/0CU; no production image or font files were edited.

## Verification

Baseline01 passed2EditMode/6PlayMode tests before changes. Verify01 rebuilt and passed existing checks but the new immediate press capture missed the rebuilt control in the raycast registry. The test now waits for the native controls to settle before pressing. Final Verify02 succeeded: prefab rebuild/readback,2EditMode+7PlayMode=9passed,0failed/skipped, Unity6000.5.9f1.

Checks cover live shared-header values, all20 collectible selections, unique selection perimeter, shared booster sprite identities, saved quantities including zero/negative-clamped/maximum values, no consumption, navigation/category routing, repeated binding, modal input blocking, press feedback, localized text fit, safe-area bounds and top/bottom edge anchoring.

15 actual Unity Play Mode captures were produced.17 review views decoded in a430px browser without errors or horizontal overflow. Visually inspected default Russian, standard boosters, pressed booster and tall English layouts. Captures use isolated saves and simulated phone dimensions/safe areas; no physical-device approval is implied.

SHA checks preserve5,575 existing artwork/font/meta/other-page files. No production art/font additions or edits. Source/cutover whitespace passed; Unity's generated YAML formatting was retained. View/builder/prefab/cutover agree on revision2. No Android, device, full-suite, backend or performance validation.

## Changed Assets files


- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/InventoryCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Inventory.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionInventoryPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionInventoryLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionInventoryRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionInventoryView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/InventoryAlignmentPlayModeTests.cs`

Seven existing Assets files modified; none added/deleted. Review evidence and handoff records accompany the implementation.

[Scope/results](QA/ScopeAndVerification.json) · [Integrity](QA/IntegrityChecks.json) · [Browser](QA/BrowserChecks.json) · [Fidelity review](QA/ReferenceFidelityReview.md)
