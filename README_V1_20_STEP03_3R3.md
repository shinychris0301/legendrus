# v1.20 Step 3.3R.3 — Pre-Level / Gameplay / Pause Visual Rebuild

This checkpoint continues the reference-match correction wave for the three session-flow surfaces: Pre-Level, Gameplay, and Pause.

## What changed

- Added 24 page-specific reference-derived production components under `Assets/LegendsOfRus/Art/Production/CoreLoop/ReferenceMatchR3`.
- Rebuilt Pre-Level from 12 components that reconstruct the approved 941x1672 reference with zero source-side pixel error.
- Rebuilt the fixed Gameplay shell from five components while leaving the live Match-3 board rectangle (`x=79, y=357, w=786, h=942`) untouched and runtime-driven.
- Rebuilt Pause as a true runtime modal over the live Gameplay page. The central modal is decomposed into seven components and the gameplay background is dimmed at runtime rather than replaced with a captured Pause screen.
- Corrected Pre-Level and Pause interaction hotspots to the geometry of the pinned references.
- Added `ProductionReferenceMatchR3PageMarker`, a deterministic R3 builder/validator, EditMode tests, and a 137-check Editor self-test.
- The base Step 3.3 builder re-applies both R2 and R3 visual-fidelity corrections after any generic rebuild.

## Visual proof boundary

The approved source archive available in this project contains flattened references rather than original layered production source. This checkpoint therefore performs a reference-derived component reconstruction. It does not claim that independent original character/environment source layers exist.

The source-side proof is deliberately scoped by runtime behavior:

- **Pre-Level:** exact full-page component reconstruction at 941x1672; 0 different pixels and 0 max channel error.
- **Gameplay:** exact fixed-shell reconstruction outside the live Match-3 board; 0 different pixels within the fixed-shell scope. The board is deliberately excluded because it is dynamic and must remain interactive.
- **Pause:** exact reconstruction of the central modal crop (`x=150, y=295, w=640, h=1125`) with 0 different pixels. The page behind it is the actual live Gameplay page plus a runtime dim layer.

This is a source/component proof only. Unity import/compression, live board rendering, dynamic numeric masks/text, safe-area scaling, modal animation, and physical-device comparison still require Unity execution before final visual sign-off.

## Screenshot boundary

No generated R3 production prefab may depend on:

- `Assets/LegendsOfRus/Resources/DesignScreens/*`
- `Assets/LegendsOfRus/Art/ReferenceOnly/*`

The source reference images are retained only for QA and visual comparison. Screenshot deletion remains unauthorized until all page waves pass review.

## Runtime behavior preserved

The Match-3 board, swap/match/cascade rules, goals, moves, score, boosters, rewards, saves, analytics boundaries, and chapter data remain intact. Only the Pre-Level and Pause interaction rectangles are corrected to align with the approved visual geometry.

## Unity verification

1. Open the exact package in Unity 6000.5.9f1 and allow imports to complete.
2. Run `Legends of Rus > Production UI > Build v1.20 Step 3.3R.3 Pre-Level Gameplay Pause Reference Match`.
3. Run `Legends of Rus > Production UI > Validate v1.20 Step 3.3R.3 Pre-Level Gameplay Pause Reference Match`.
4. Run `Legends of Rus > Tests > Run v1.20 Step 3.3R.3 Self-Tests`.
5. Expected self-test result: `137/137`.
6. Run the complete EditMode and PlayMode suites.
7. Capture Pre-Level, Gameplay, and Pause at 941x1672 plus 16:9, 18:9, 19.5:9, and 20:9 portrait targets.
8. Confirm the live Match-3 board remains fully interactive and Pause resumes/restarts/settings/exits correctly.

`finalReferenceMatchSignoffGranted` remains false until those Unity/device captures are reviewed.

## Next checkpoint

`v1.20 Step 3.3R.4 — Victory / Fail / Chapter Complete Visual Rebuild`
