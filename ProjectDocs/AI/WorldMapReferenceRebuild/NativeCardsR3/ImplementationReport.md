# World Map — native reference cards R3

Implemented 2026-09-20 in the canonical WorldMap prefab and its existing runtime/builder. This is a native Unity page, not a screenshot with hotspots.

## Result

- Twenty confirmed chapter names and accepted Scenario region paintings; four complete cards initially. Later chapters are above the current chapter, revealed by dragging downward.
- Separate opaque lacquer surface, proportion-preserving region painting, extracted ornamental frame, lock and progress plaque. The R2 frame separation repair is retained.
- The current chapter highlights **only the decorative border** with an independent frame-alpha glow. The panel face, painting and text are not tinted by selection. Buttons use no whole-panel color transition.
- Original Russian lettering on title, Back, Continue, map action and five navigation medallions. English uses locally repaired blank faces and live labels; dynamic chapter names, availability and progress remain live.
- Bottom Back and Home / Map / Tasks / Guild / Shop controls use existing routes. Header reuses Home portrait/resource/settings sprites and the repaired joined backing, with saved balances and level.
- Actual content gating retained: Chapter 1 has ten authored levels, so its star maximum is 30. Chapters 2–20 are visibly inactive and locked. No sample 15/15 or 9/15 progress and no invented unlocks. Untracked XP is a dash.
- Existing task dialog, startup/router/services and save behavior retained. World Map view revision 2; prefab builder rebuild and serialized readback passed.

## Highlight evidence

Actual Unity captures with glow off/on differ at 31,846 pixels, entirely within the current card (x92–848, y1030–1299). The measured interior rectangle x170–779, y1090–1239 has **zero changed pixels**. No pixels outside the current card changed. The glow texture center alpha is zero. See `QA/HighlightComparison.json` and paired `QA/WorldMap/WorldMap_BorderGlow_*.png` captures.

## Verification

Unity 6000.5.9f1 import/compile and WorldMap-only prefab rebuild passed. Final Verify02: **9 Edit Mode + 16 Play Mode = 25 passed**, zero failures/skips, including existing Home regression coverage. Native tests cover bindings/import settings, chapter availability, earned stars/completion, route callbacks, pointer drag scrolling, locked chapter behavior, header balances, RU/EN round trip, visible button raycasts and portrait/safe-area captures.

Inspected actual Unity RU/EN, tall-safe, final-chapter and border-highlight captures, plus transparent sprites over black/white/magenta. Initial iterations corrected a compiler variable collision, unwanted adjacent-card rail at initial scroll position, stretched terrain crops, English navigation lower-rail damage and caption positioning. Final logs/results and captures are under `QA/`.

No Android APK, physical-device, full test-suite or backend verification was performed.

## Scope and assets

Eight existing Assets files changed: WorldMap prefab, view/runtime/layout, shared navigation's WorldMap-only bypass, owning builder and two WorldMap test files. Added 139 files including resources/metas and two C# files/metas; no deletions. `QA/ScopeAudit.json` compares against the task's initial full Assets hashes. All other existing Assets—including Home, Splash, Loading and Gameplay art/prefabs—are unchanged.

Existing Scenario extractions and accepted paintings were reused. **Zero new Scenario jobs or credits** were needed for native integration. Local operations: English blank-face repair, derived border glow, center cover-crops and painting alpha taper. Source origins and hashes are in `Sources/AssetManifest.json`, the R2 extraction package and R1 paintings package.

This is not a pixel-identical whole-screen claim: canonical story names/paintings, reversed scrolling order, actual saved values, live-font metrics, adapted card heights, fine extraction edges and repaired English surfaces differ from the supplied example. The exact source font remains unidentified. English caption faces have some soft repaired texture. Future chapters need authored playable content before being enabled.

Next: review the native World Map, then continue the approved sequence to Chapter Map.

Final source checks: eight review capture choices decoded at390×844 with no JavaScript errors or horizontal overflow. Scoped git diff whitespace check and Python source parsing passed. After runtime verification, only 582 Unity-serialized empty-name trailing spaces were normalized; no serialized values or runtime behavior changed.
