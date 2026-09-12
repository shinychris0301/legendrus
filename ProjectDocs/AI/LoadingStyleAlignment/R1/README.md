# Loading style alignment R1 — offline design review

Status: prepared for user review, not approved and not implemented in Unity.

Open `Loading_Alignment_R1_OFFLINE_NOT_UNITY.png` for the single standalone 941 x 1672 page. This is a headless Chrome capture of `Loading_Alignment_R1.html`, not a Unity capture. Open the HTML for its live text and preview notes; `?capture=1` renders the exact reference canvas without review notes outside the page.

## Composition and provenance

| Element | Actual source | Treatment / limits |
|---|---|---|
| Logo | `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/legends_of_rus_ornate_crest.png` | Direct relative HTML image reference to the canonical source. No copied/redrawn logo, color filter, or opacity change. Original aspect ratio retained at width 550. Source SHA-256 `8f7a333b29cf0d27fc7c11be6f75f55aef37cbb1a0bb93f965b124dcf034c3dc`. |
| Journey illustration | `journey_scene_refined.png` | New built-in imagegen illustration using current Ivan, Firebird, winter environment and logo material references. Scene and characters are combined in this design layer; it is not a clean runtime environment plate or isolated character set. Natural dimensions 941 x 1671, displayed with CSS cover on the 941 x 1672 canvas. |
| Earlier illustration | `journey_scene_generated.png` | Superseded within this review pass because character silhouettes overlapped the reserved lower UI area. Preserved as generation lineage, not another final page. |
| Tip frame / paper | `tip_panel_generated.png` | Generated isolated RGBA artwork, 2041 x 770, plus a CSS opaque backing behind the writing area. Native interior alpha is near-opaque, not fully opaque; do not promote this sprite as production-ready. No scenery or lettering is embedded in the panel. |
| Tip text | HTML text nodes | `СОВЕТ` and `Совет: объединяйте особые самоцветы, чтобы очищать тьму быстрее.` preserved from the old journey concept. The historical Wave01 builder instead used `поле`; that variant was not adopted. This wording remains part of the proposed visual for review. |
| Loading label | HTML text node | `Загрузка...` retained from active FIX01 source. |
| Loading track | CSS navy/gold track with stationary sapphire segment | Visual example of an indeterminate state; no percentage, timer, measured-progress claim, or application transition. |
| Typography | Existing development fallback stack by name | Times New Roman, Liberation Serif, Noto Serif, Droid Serif, serif. `fc-match` resolves the first two requests to local Liberation Serif. Source font identity and exact Unity rendering remain unverified. No fonts copied or distributed. |

The actual approved Splash PNGs, prefab, icon sizing, layout, GUIDs, and font assignments were not modified. Materials were matched through the source artwork and visual inspection; this is not user approval.

## Generation and checks

All raster generation/editing used the built-in `image_gen.imagegen` tool. Full prompts and input roles are in `PromptSet.json`. Original tool outputs remain under the built-in generated-images directory; selected artwork and the first scene iteration are copied here. A panel opacity-edit attempt returned an RGB image with a baked checkerboard background; it was rejected and was not copied into this preview. The preview therefore keeps the first RGBA panel with a code-native opaque paper backing. Production alpha cleanup is still required.

The final page was visually inspected. Browser checks found all three referenced images loaded, exact canonical logo reference with filter `none` and opacity `1`, correct Russian text without body overflow, and no JavaScript exceptions. The initial title/ornament overlap was corrected before final capture. The JSON next to the final PNG contains the actual browser measurements.

`QA/Panel_Black`, `Panel_White`, and `Panel_Saturated` HTML/PNG/JSON files record inspection of the panel component with its opaque writing-area backing against black, white and saturated magenta. These are component diagnostics, not additional game-page designs. They demonstrate the current preview treatment, not acceptance of the raw PNG alpha. Raw panel measurements: 369344 alpha-zero pixels, 43 alpha-255 pixels, 1202183 partial-alpha pixels, center alpha 253, four outer corners alpha 0.

Headless Chrome initially failed inside the sandbox (`setsockopt: Operation not permitted`); its local capture was then allowed through automatic escalation review and completed using a temporary browser profile. No external website, deployment, Unity launch, or user browser session was involved.

`ProvenanceAndChecks.json` records hashes, selected artifacts, and unchanged protected source checks. No Unity compilation, Edit/Play Mode tests, Android build, application navigation test, actual Unity capture, 9:19.5/9:20 layout validation, or safe-area validation ran. The browser's fit-to-window display is not a Unity responsive-layout test.

## Next action

Obtain user review of this one standalone page. Following visual approval, produce and verify a clean environment plate and isolated journey sprites, resolve panel alpha, then implement the approved layout in the active `Pages/Loading.prefab` and its actual FIX01 ownership path. Reuse live localized text and existing services; resolve real loading readiness/error behavior before claiming functional progress or navigation acceptance. Do not use this complete page PNG or its combined journey illustration as a substitute for the required production layers.
