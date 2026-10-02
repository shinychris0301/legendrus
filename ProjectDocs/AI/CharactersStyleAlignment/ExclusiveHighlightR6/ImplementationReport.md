# Characters — exclusive reference highlight R6

The earlier implementation reused Ivan’s highlighted source frame for every character. Its yellow illumination was baked into the pixels, so removing an extra outline did not resolve the shared highlight.

Extracted the normal Masha card perimeter from the original reference using one Scenario Pixa background-removal job (3CU). Removed the portrait, text, stars and badges from the interior locally through alpha, keeping source RGB unchanged. Checked the resulting isolated frame over black, white and magenta.

Both the backing and foreground frame of every noncurrent character now use the normal bronze-gold sprite. Locked slots also use it. Only currentIvan retains the original highlighted source border and its subtle glow. Previewing Masha/Firebird or another hero does not transfer the highlight. No added rectangular outline, fake equip action or save change.

Existing Characters prefab and owning runtime/view/builder/cutover updated to revision6. R4 text and surface corrections, R3 scrolling, navigation and unavailable ownership behavior remain intact. Original sprites and all existing GUIDs preserved.

## Checks performed

Unity6000.5.9f1 compilation/import/rebuild/save/readback passed.2Edit+6Play=8tests passed with0failures/skips. Strengthened existing preview test to check both frame layers for all eight characters and ensure only currentIvan uses the highlighted source. Locked-frame assignments also checked. Existing filters, upward drag/full locked-row visibility, localized text, safe-area/raycast targets, navigation, info input blocking and save-preservation checks passed.

Nine fresh actual Unity captures retained; inspected RU initial, Firebird preview and scrolled safeEN. The glow is exclusive to Ivan, and the other character/locked frames are visibly subdued. Browser390px checks all11views and links; results inQA/BrowserCheck.json. Syntax/whitespace/provenance checks inQA/SourceChecks.json; exact file scope inQA/ScopeAudit.json.

Not performed: APK, physical phone/touch/hardwareBack, full-project suite, profiling or hero-service integration. The main scene, enlarged alternative portraits, exact font and footer still have the previously documented reference differences.

[Review R6, previous R5 and original](Characters_ExclusiveHighlight_R6_Review.html).
