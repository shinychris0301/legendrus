# Characters — current hero and collection scrolling R3

The yellow border now identifies the character currently in use (Ivan), independently of the selected preview. The locked row now contains full218×240cards matching the other cards. A905×624clipped vertical collection shows part of the locked row initially; swipe upward to reveal the complete cards. Details, preview-information link and navigation remain fixed. Filter changes reset scrolling and shrink the content extent to avoid blank space.

Implemented in the owning Characters runtime/view, rebuilt existing prefab and revision3 builder/cutover. Existing card art, source extraction, fonts, main scene and routing reused unchanged. No new Scenario requests or image processing:0CU. All existing GUIDs/meta files preserved. Hero ownership remains unavailable, Ivan remains current, and previews do not unlock or equip heroes.

## Checks performed

- Unity6000.5.9f1 compilation/import, prefab rebuild/save/readback and revision3 cutover passed.
-2Edit Mode +6Play Mode tests passed; zero failures/skips. Added a regression covering current-vs-preview outline, locked-card initial clipping, upward pointer drag revealing the complete row, safeEN phone layout, filter scroll reset and save preservation.
- Existing bindings, idempotence, draw order, RU/EN complete text, safe-area/raycast checks, header updates, navigation, info modal and repeated/rebound/disabled callbacks passed. Masked card centers cannot intercept header/footer raycasts.
-9actual Unity captures include initial/scrolled states, RU/EN, narrow/tall safe areas, Firebird preview, Rare filter and info modal. Visually inspected the Firebird preview with Ivan's outline and the narrowEN full locked row.
- Browser review check recorded separately inQA/BrowserCheck.json. Source Python syntax and scoped whitespace checks recorded inQA/SourceChecks.json.
- QA/ScopeAudit.json compares against the pre-R3 snapshot:7existing Assets files changed; no added/deleted production files or art changes.

## Limits

No APK, physical phone/touch/hardware-Back test, full-project suite, profiling or hero-service implementation. The drag test invokes Unity pointer handlers in Play Mode; it is not a physical touchscreen test. The main portrait/Firebird, scenery and footer still differ from the reference, as documented inR2. No exact-replica claim.

[Open review](Characters_CurrentHeroScroll_R3_Review.html). Next: visual review of current-hero indication and collection scrolling; remaining main-scene/footer fidelity is a separate refinement.
