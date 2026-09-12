# v1.20 Step 7.3 — Reference-Accurate Wave 1 Reconstruction Refinement and Typography Finalization

## What changed

Step 7.3 replaces the generic Step 7/7.2 visual reconstruction recipe with reference-measured Wave 1 candidate geometry. The nine pages that have direct approved references are measured against the approved 941×1672 compositions; approved full-screen images remain reference-only and are never referenced by candidate prefabs.

The refinement introduces `Assets/LegendsOfRus/Art/Production/Wave01Refined/`, containing clean environment candidates, reference-locked sectional components, live-text erase patches, and style-derived assets for pages without a direct approved reference. The section approach is intentionally candidate-only: it is used to lock geometry/fidelity without activating a full-screen screenshot in runtime.

## Fidelity result

Eight of the nine directly referenced pages now pass the strict full-page source proxy. Gameplay is evaluated differently because the live board must remain composed from individual cells, gems, specials, and gameplay state. Its non-board shell passes the dynamic-content-safe proxy while the full screenshot score remains lower by design.

See `V1_20_STEP07_3_REFERENCE_FIDELITY_METRICS.csv` and `ProjectDocs/V1_20_STEP07_3/VisualQA/DirectReference_vs_Step73_Refined.jpg`.

## Typography

Localized runtime copy remains live `UnityEngine.UI.Text`. Step 7.3 finalizes the hierarchy and effects used by the candidate builder: gold display/heading styles, HUD style, parchment body style, dark-panel body style, outline, and shadow treatment.

No font binary is shipped. The first Unity `Font` imported under `Assets/LegendsOfRus/Art/Fonts/Production/` becomes the production candidate font; otherwise the builder uses Unity's legacy built-in font as a development fallback. Final release signoff requires the project owner to import a properly licensed Cyrillic face and rerun the candidate build/capture.

## Cutover

Active runtime cutover remains **NO-GO**. The reasons are not the old generic visual mismatch anymore; the remaining blockers are three missing direct references, actual Unity/Android execution, final font-face signoff, and dynamic map-node/star decomposition.

## Unity commands

1. `Legends of Rus > Production UI > Build/Repair v1.20 Step 7.3 Reference-Accurate Wave 1 Candidates`
2. `Legends of Rus > Tests > Run v1.20 Step 7.3 Reference Accuracy + Typography Self-Test`
3. `Legends of Rus > Tests > Run v1.20 Step 7.3 Four-Aspect Refined Candidate Visual QA`
4. `Legends of Rus > Production UI > Evaluate v1.20 Step 7.3 Wave 1 Cutover Decision`

Do not alter the active `ProductionUI/Pages` registry until the Step 7.3 decision blockers are closed.
