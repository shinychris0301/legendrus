# v1.20 Step 3.1 — Placeholder Audit and Replacement Matrix

This checkpoint audits the current screenshot-backed runtime UI before reconstruction begins.

Key findings:
- 22 full-screen PNGs are active under `Assets/LegendsOfRus/Resources/DesignScreens`.
- `ScreenRouter.BuildDesignLayer` loads those screenshots through `DesignScreenRegistry` and `Resources.Load<Sprite>`.
- Current interaction is layered through invisible hotspots plus selected runtime overlays.
- There are no Unity `.prefab` files in the Step 2 baseline.
- The target product inventory contains 36 surfaces plus one legacy Chapter15 special-case route.
- 14 target product surfaces do not yet have a current ScreenId/screenshot implementation.
- The 22 runtime screenshot placeholders represent about 132.04 MiB of raw RGBA source pixels if all were resident simultaneously.

This checkpoint does not remove screenshots and does not cut over any runtime route. Approved screens remain reference/QA sources until their reconstructed prefabs pass visual and interaction verification.

Primary artifacts:
- `Assets/LegendsOfRus/Documentation/V1_20_STEP03_1_PLACEHOLDER_AUDIT_AND_REPLACEMENT_MATRIX.md`
- `Assets/LegendsOfRus/Documentation/V1_20_STEP03_1_PLACEHOLDER_REPLACEMENT_MATRIX.json`
- `Assets/LegendsOfRus/Documentation/V1_20_STEP03_1_UI_KIT_DEPENDENCY_INVENTORY.json`
- `ProjectDocs/ProductionUI/placeholder_replacement_matrix.csv`

Editor audit:
`Legends of Rus > Production UI > Run v1.20 Step 3.1 Placeholder Audit`

Next checkpoint:
**v1.20 Step 3.2 — Canonical UI Kit Reconstruction**.
