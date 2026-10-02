# Home Safe Edges R8 — 2026-09-25

Home's actual source-composition layout now anchors the shared header and crest to the safe top and the lower interaction group to the safe bottom. The active prefab, runtime revision22 and owning builder agree. No World Map, Friends List or Splash changes in this pass.

The hero and Firebird extraction, Play, social shortcuts, navigation cards and reward/footer group move together, preserving the Play-shaped opening in the original hero sprite. No art was resized, regenerated or recolored. The additional tall-screen height is filled by the existing clean scenery between the crest and hero. Reference-size rendering is pixel-identical to the audit baseline:941×1672,0changed pixels.

The source builder now serializes three shortcut RectTransform references in the layout; its required-bindings check validates them. The existing Home-only rebuild retains the original prefab GUID and button identities. Revision validation uses ProductionHomeView.CurrentRevision; Home has no separate cutover-state file.

## Verification

- Final Verify02:6Edit Mode and20Play Mode tests passed. Includes active prefab/builder validation, save-backed balances, Home routes/social returns, pointer raycasts, language changes, loading preparation and cancellation.
- New assertions require the header and footer to meet safe edges, preserve navigation/footer and Play/navigation joins, and keep the hero opening aligned with Play. Existing repeated-layout checks cover drift. Added tall English capture.
- 11actual Unity captures retained with state/geometry sidecars;9browser review views decoded without errors or horizontal overflow. Visually inspected final tall RU/EN captures. Simulated displays include941×1672,576×1280with insets,1080×2340and1080×2400with side/top/bottom insets.
- Verify01 passed its initial26functional checks but visual inspection found an exposed hero cutout after moving Play independently. Corrected the entire overlapping group; retained the rejected tall capture underQA/Verify01/VisualFinding and added the alignment assertion before Verify02.
- 7160existing Assets files unchanged. Six existing files modified, no Assets added/deleted. Every existing PNG, font, meta and other page prefab is unchanged. Zero generation/Scenario calls.
- Scoped whitespace check passed. Generated YAML empty-field whitespace was normalized after verification without changing serialized values.

## Changed files

- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeSourceComposition.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/HomeAlignmentPlayModeTests.cs`

## Limits and next action

These are native Unity Editor captures and simulated safe areas, not physical Android-device tests. No Android build, backend, performance or full-game test run. Existing typography and extraction artifacts are not an exact-source fidelity claim. Tall displays intentionally show more sky between crest and hero while retaining original artwork sizes.

Next recommended page: World Map safe-edge header/footer and expanded scrolling area. Friends List's right-frame join remains a separate follow-up from the audit.
