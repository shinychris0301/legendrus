# v1.20 Step 3.3R.2 — Home / World Map / Chapter Map Visual Rebuild

This checkpoint corrects the first three high-impact pages against the pinned approved 941x1672 references.

## What changed

- Added 14 page-specific production PNG components under `Assets/LegendsOfRus/Art/Production/CoreLoop/ReferenceMatch`.
- Rebuilt Home from six semantic production components: top HUD, logo scene, hero scene, Play component, illustrated navigation component, and quest footer.
- Rebuilt World Map from four components: top HUD, title plaque, painted world-map plate, and bottom navigation.
- Rebuilt Chapter Map from four components: top HUD, Chapter 1 ribbon, painted level-path plate, and bottom controls.
- Added `ProductionReferenceMatchPageMarker` and a deterministic Unity Editor builder/validator.
- The older Step 3.3 builder re-applies the R2 versions after any manual base rebuild, preventing accidental regression to the generic pages.
- Added a source-side reconstruction tool and exact static pixel proof.

## Important implementation boundary

The approved design archive contains flattened references rather than original source-layer artwork. To guarantee visual correspondence without using a single full-screen runtime screenshot, this checkpoint decomposes the approved references into role-specific production components. No production PNG is 941x1672, and generated prefabs are forbidden from referencing `Resources/DesignScreens` or `Art/ReferenceOnly`.

This is a reference-derived component reconstruction, not a claim that original independent painter/character source layers suddenly exist. Future art-source replacement can swap these component assets without changing the prefab/runtime contract.

## Static reference proof

The 14 components reassemble to the pinned references with zero pixel/channel error at 941x1672 for Home, World Map, and Chapter Map. This proves the source-side component decomposition only. Unity rendering, dynamic HUD masks/text, device scaling, and interaction overlays still require Unity/device review before final reference-match signoff.

## Runtime behavior preserved

`ScreenRouter` interaction hotspots, gameplay, saves, economy, rewards, chapter data, and production provider gates are not changed by this checkpoint. Home / World Map / Chapter Map remain part of the Step 3.3 production cutover; only their production visual prefabs are rebuilt.

## Unity verification

1. Open in Unity 6000.5.9f1 and allow import to finish.
2. Run `Legends of Rus > Production UI > Build v1.20 Step 3.3R.2 Home-Map Reference Match`.
3. Run `Legends of Rus > Production UI > Validate v1.20 Step 3.3R.2 Home-Map Reference Match`.
4. Run `Legends of Rus > Tests > Run v1.20 Step 3.3R.2 Self-Tests`.
5. Expected self-test result: `85/85`.
6. Run complete EditMode and PlayMode suites.
7. Capture Home, World Map, and Chapter Map at 941x1672 plus 16:9, 18:9, 19.5:9 and 20:9 portrait targets and compare to the approved references.

Final `finalReferenceMatchSignoffGranted` remains false until those Unity/device captures are reviewed.

## Next checkpoint

`v1.20 Step 3.3R.3 — Pre-Level / Gameplay / Pause Visual Rebuild`
