# R6 HUD and footer source delivery

Ten independently painted sprites are saved in `Sources/`. Every one of the 17 built-in generation calls received the complete original `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Home.png` as an actual image input. No screenshot pixels were cropped, pasted or postprocessed. Selected files exactly match the original tool output hashes in `HudFooterPrompts.json`. Five RGB/checkerboard attempts were rejected; two earlier RGBA panel candidates were superseded for proportions.

All ten selected PNGs have real RGBA alpha. Direct browser inspection over black, white and saturated magenta found clean outer isolation and a genuinely empty avatar opening. Navy/wood/green surfaces retain intentionally near-opaque alpha. Small antialiasing and alpha253 surfaces are retained as generated; existing opaque native backings should remain where appropriate.

| Sprite | Source size | Near-opaque bounds, inclusive |
| --- | --- | --- |
| `hud_frame.png` | 2176 × 723 | x22–2153, y198–472 |
| `resource_capsule.png` | 2172 × 724 | x29–2142, y101–617 |
| `quest_panel.png` | 2206 × 713 | x21–2186, y56–656 |
| `footer_base_generated.png` | 2143 × 734 | x4–2139, y346–707 |
| `avatar_ring.png` | 1104 × 1425 | x95–1007, y3–1336 |
| `coin_icon.png` | 1254 × 1254 | x35–1215, y31–1216 |
| `gems_icon.png` | 1254 × 1254 | x22–1231, y94–1166 |
| `energy_icon.png` | 1254 × 1254 | x360–945, y121–1158 |
| `resource_plus.png` | 1254 × 1254 | x142–1111, y127–1091 |
| `settings_medallion.png` | 1254 × 1254 | x27–1226, y9–1228 |

The avatar transparent opening is x181–921, y151–903 in1104×1425, measured through a connected alpha<8 region. It contains448,934 pixels and is not connected to the exterior. The native portrait mask should sit behind the complete frame; no cropped portrait texture is needed.

Use complete source textures and compensate their transparent margins in the image transform. HUD artwork is already a narrow visible bar. Keep icon and footer ornament proportions where possible; the footer source is a low carved strip suited to bottom alignment over an opaque native backing. The final quest visible ratio is3.604:1, closer to the reference target3.14:1 than the rejected4.37:1 candidate, but still requires modest geometric compensation to occupy the reference panel. No exact shape identity or pixel-perfect color match is claimed.

Visual details remain independently painted: coin relief, diamond facets, avatar pennant heraldry and small frame carvings differ from the source reference. This delivery establishes clean assets and measured geometry, not final page fidelity or user acceptance. The whole Unity portrait must be reviewed after integration.

Checks performed: original whole-reference view and SHA-256 verification;17 built-in generation calls with recorded full prompts/input path/input hash/output path/output hash; PNG dimensions/color-type checks; selected original/output byte identity; final local Chrome/CDP alpha histogram, bounds and three-background captures; direct final capture inspection; avatar opening flood fill. Final diagnostic execution exited0. An initial diagnostic run failed on a fractional CDP viewport height; the QA script was corrected to round the height upward before the successful runs.

Not performed by this subtask: production PNG or metadata installation, C# or prefab edits, Unity import/compilation/capture/tests, APK/AAB build, phone testing, exact original font/color identity verification. Root owns the four progress/reward source slots and the remaining page art; the R6 owner handles native integration.

Evidence: `HudFooterPrompts.json`, `QA/PanelAlphaChecks.json`, `QA/AvatarOpeningGeometry.json`, `QA/PanelAssetsAlphaReview_panels.png`, `QA/PanelAssetsAlphaReview_small.png`, and the reproducible `QA/CaptureHudFooterAlphaReview.mjs`.
