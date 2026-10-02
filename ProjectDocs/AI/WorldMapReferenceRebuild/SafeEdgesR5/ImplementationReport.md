# World Map Safe Edges R5 — 2026-09-25

The installed World Map now positions its shared header/title at the safe top and Back, chapter action and bottom navigation at the safe bottom. Its masked chapter viewport expands into the extra height instead of keeping a centered fixed-height composition. Runtime, owning Home/World Map builder and active WorldMap.prefab agree at revision6.

The original card dimensions, decorations, source title/buttons, twenty chapter images, names/order, locked presentation and border-only current-chapter highlight are unchanged. Later chapters remain above; dragging downward reveals them. Resizing preserves normalized scroll progress, including the initial Chapter1 end, intermediate positions and the Chapter20 end. This preserves relative list position, rather than anchoring one specific card pixel across every aspect change.

At1080×2400 with a990×2160 safe area, the viewport grows from1006 to approximately1387 design units. Roughly five complete cards plus part of another can be visible, depending on scroll position and current-card height. Partial cards at the mask edges are intentional scrolling behavior; their source sprites have not been cropped. Header/footer controls stay outside the mask.

## Validation

- Verify01:3Edit Mode and20Play Mode checks passed (7World Map +13Home). Native Unity6000.5.9f1 compilation, prefab/header checks, routes, live balances, lock notices, pointer hit tests, chapter drag direction, border highlight, localization, safe-edge distances and viewport growth.
- New resize test covers normalized positions0,0.5and1 through simulated display changes and restoration; confirms later chapters above. Added tall English capture. Reference941×1672RU render is pixel-identical to the previous audit capture:0changed pixels.
- 19actual World Map Unity captures and Home regression captures retained;9review views decoded in Chrome with no script errors/horizontal overflow. Inspected tallRU, tallEN and intermediate scroll captures. Tests simulate941×1672,576×1280and1080×2400safe areas with isolated saves.
- 7161existing Assets unchanged;5existing files modified,no Assets added/deleted. All existing artwork, fonts, metadata and other page prefabs—including HomeR8—unchanged. No new generation/Scenario jobs.
- Generated YAML empty-field whitespace normalized after testing without changing serialized values; scoped whitespace check passed.

## Changed files

- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/WorldMap.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionWorldMapLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionWorldMapView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/WorldMapAlignmentPlayModeTests.cs`

## Limits and next step

No Android build/physical-device, backend, performance or full-game suite run. Original body/font and scenery approximations remain. This is a responsive-layout repair, not a new exact-reference fidelity claim. No separate World Map cutover-state file exists; installed revision/bindings are checked by the owning builder.

Next recommended fix: the Friends List right-hand decorative frame join identified in the navigation audit.
