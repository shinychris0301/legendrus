# Maintenance — Parchment surface R2

The user reported that the panel background did not match the reference. Inspection confirmed the old text-removal process filled the interior with a mirrored 241×49 parchment sample. The repeated pattern and tonal bands remained visible behind the live text.

## Repair

Used the built-in imagegen tool for a continuous parchment reconstruction based on the current panel and original reference. A second focused edit removed the generated divider so the original brown separator could be retained without doubled edges. Only the repaired interior is integrated into two new RU/EN panel sprites; the original title, frame outline, jewels and alpha remain from the existing assets. The separator ink is protected by a narrow contour mask. Texture color is calibrated against four untouched parchment sample areas in the reference. Soft boundaries blend into existing edge shading.

New assets: `Assets/LegendsOfRus/Resources/ProductionUI/MaintenanceAligned/panel_ru_surface_r3.png` and `panel_en_surface_r3.png`. Prior assets remain unchanged. View, builder, active prefab and cutover now agree at revision 3. English/Russian captions, buttons, gift, scenery and maintenance behavior are unchanged.

Tool: built-in imagegen, two edit calls; no Scenario calls and no CLI image generation. Exact prompts: Sources/ImagegenPrompt.txt and Sources/ImagegenFollowupPrompt.txt. Original generated outputs are copied into Sources; Sources/IntegrateSurface.cjs records bounded integration and color calibration. The generated full panel is not used as the production page.

## Checks

Final Verify03 passed 4 Edit Mode + 10 Play Mode tests (14 total, no failures/skips), with five actual Maintenance captures and seven browser views passing. Final Unity results and actual changed files: QA/ScopeAndVerification.json. Targeted Maintenance and Offline tests include navigation, localization, complete text, press feedback, unchanged save and simulated safe-area layouts. Earlier verification runs passed functional checks but visual review identified separator double edges/tonal banding; the final texture addresses those findings.

Original alpha and pixels outside the repair mask are asserted unchanged. 5,661 existing art/meta/font and other page prefab files remain unchanged. Final alpha inspected against black, white and magenta. Actual Unity captures and original/prior comparisons are available in the HTML review.

No Android build, physical-device/backend test or full project test suite. Texture behind removed content is reconstructed, not recovered exactly. Existing typography and English completion-plaque repair differences remain outside this panel-background fix.
