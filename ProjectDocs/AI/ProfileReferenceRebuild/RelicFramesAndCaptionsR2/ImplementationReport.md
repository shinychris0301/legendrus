# Profile — Relic Frames and Captions R2

User continued the recommended Profile refinement after R1. The native page now encloses relic cards and selected-relic information in the original carved frame, adds original gold heading ornaments, separates preview notes from section titles and fits live relic names on a single line with inner clearance.

The shared Home header, original portraits/relic artwork, saved statistics, approved booster assets and guarded actions remain. No ownership, combat-stat, XP or upgrade behavior was added.

## Assets

Three new PNG/meta pairs in `Assets/LegendsOfRus/Resources/ProductionUI/ProfileAligned/`: `relic_collection_r2_frame`, `heading_left_r2`, `heading_right_r2`.

Source is the original Profile reference retained under `../R1/Sources/Reference.png`. The frame preserves its original boundary; a recorded mask replaces old content with a clean navy donor. Heading flourishes keep original RGB; local gold segmentation removes the navy behind them.

One Scenario `model_pixa-background-removal` sheet request, 3 CU, no retries and no RGB generation. Scenario alpha is bounded outside the frame and its intentionally opaque interior is restored. The returned ornament rectangles needed local navy removal. Evidence: `Sources/Scenario/relic_frame/`, `Sources/Sheet.json`, `Sources/FrameRepairMask.png`, `Sources/Provenance.json` and `QA/AlphaBackgrounds.png`.

## Implementation

- `ProductionProfileRuntime.cs`: source enclosure, localized heading/note composition and measured one-line relic captions.
- `ProductionProfileLayout.cs`: extends the relic enclosure to the footer while maintaining corner geometry; measures captions after applying the current phone scale.
- `ProductionProfileView.cs`: native revision 3.
- `ProductionProfilePrefabBuilder.cs`: source sprite border settings and cutover revision 3.
- Active `Pages/Profile.prefab` rebuilt; `CutoverState/ProfileCutoverState.txt` revision 3.

- `ProfileAlignmentPlayModeTests.cs`: adds font-size/pixels-per-unit diagnostics to the existing width assertion; limits remain unchanged.

Seven existing Assets files changed, six added, none deleted. No existing image, font, metadata or other-page prefab changed by SHA. Exact scope and validation are recorded in `QA/ScopeAndVerification.json` and `QA/IntegrityChecks.json`.

## Verification

Final **Verify03**, Unity **6000.5.9f1**: prefab rebuild/readback and **3 Edit Mode + 6 Play Mode tests passed**, zero failures/skips. View, owner, prefab and cutover use revision 3. Fifteen actual Unity captures cover RU/EN, 941×1672 and tall 576×1280 / 1080×2400 simulated safe areas, collection tabs, quantities, history, milestones, modal behavior and large values. Routes, duplicate-action guards, saved statistics and preview no-grant behavior remain tested.

Seventeen review choices loaded in headless Chrome at 430 pixels without page errors or horizontal overflow. Alpha backgrounds and final RU/tall EN/Relics captures were inspected. Visible RGB equals the documented source/donor composites. 5,549 existing artwork/font/metadata/other-page-prefab files remain unchanged. RU and EN captures' top 677 pixels and footer from y1527 are identical to R1; checked Items and History captures are fully pixel-identical. Scoped C# whitespace check passed.

Verify01/02 rejected caption widths. Build-time font metrics changed after phone scaling; final iterative measurement occurs after responsive scale is applied, with extra inner clearance. Earlier logs are retained. Native captures are simulated phone sizes/safe areas, not physical-device screenshots.

No Android build, device, backend, online-profile, full-suite or performance test was requested or performed. Remaining approximations are described in `QA/ReferenceFidelityReview.md`.
