# Characters — reference hero and sunset R7

Replaced the main Home-derived portrait with the original reference Ivan and Firebird, isolated together through Scenario. The source pose, darker cloak, blue/gold embroidery, face and feather RGB are retained. A separate clean sunset-town plate replaces the forest within the portrait window; only scenery hidden by the original subject was generated. Shared Home artwork and the rest of the page background remain unchanged.

Rebuilt the portrait frame from original border pixels and the existing Scenario alpha, clearing the occupied interior and cool-colored top residue. Removed an attached lower-left frame scroll from the subject alpha. The frame now renders above the portrait; the portrait reaches underneath its lower edge, removing the exposed straight cutoff and black gap. Alternative previews retain their existing art/resolution and are anchored at the bottom so their cropped bases are also concealed.

Preserved the exclusive Ivan highlighted card frame, normal frames on other/locked cards, full locked-row scrolling, R4 text/surface repairs, live RU/EN, saved-state header, unavailable hero controls and all navigation. Updated existing prefab and owning runtime/view/builder/cutover to revision7; no new startup system.

## Art and provenance

One Pixa subject-alpha extraction (3CU) and one clean-scene generation (12CU): **15CU total**, two accepted jobs, no resubmissions. Original subject and frame RGB are preserved; local corrections change alpha only. Source/input/job/output/final hashes and repair masks are recorded. New page-local assets:hero_firebird_r7,portrait_frame_r7,sunset_r7. Black/white/magenta alpha composites inspected; generated plate visually checked for absence of characters/UI.

[Provenance](AssetProvenance.json) · [Scenario jobs/cost](QA/ScenarioAudit.json). The original scene obscured by the character cannot be recovered exactly: generated architecture/cloud placement differs, even though palette and setting follow the source. Source hero resolution is411×513; alternative hero previews are still lower-resolution cutouts. No whole-reference runtime UI.

## Verification

Unity6000.5.9f1 import/compile, native prefab rebuild/save/readback and revision7 cutover passed. Final Verify03:2Edit+6Play=8tests passed,0failures/skips. Verify01 and Verify02 also passed8tests each; visual inspection then prompted bottom anchoring for alternative previews and correction for Image.preserveAspect vertical padding. Existing preview test now checks that every subject base extends below the viewport. Edit checks verify frame draw order, separate subject/scene bindings and hidden current-subject cutoff.

Existing current-hero-only frame assignments, locked-row drag/filter reset, bindings, idempotence, RU/EN text, safe-area/raycasts, navigation/modal/input and save preservation remain covered. Nine final actual Unity captures are provided. RUmain and safeEN inspected; alternativeFirebird preview inspected for its final bottom fit. Browser390px checks11review views; results inQA/BrowserCheck.json. Source syntax/whitespace/input/RGB/hash checks inQA/SourceChecks.json. Scope audit records exact production changes.

## Remaining scope

Title/header typography, circular footer medallions/carved backing, card scene variation and higher-resolution alternative portraits remain separate refinements. Hero ownership/upgrades/stats are still unavailable. This is not a pixel-identical reconstruction of hidden scenery.

Not performed: APK, physical phone/touch/hardwareBack, full-project suite, profiling or hero-backend integration. Next: review R7; the recommended following visual focus is the circular footer/navigation and carved backing.

[Review actual Unity R7, previous R6 and reference](Characters_HeroScene_R7_Review.html).
