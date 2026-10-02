# Characters — Panel and Cards R2

Implemented the user's approved next refinement: restore the reference's detail-panel hierarchy and collection-card design. [Review actual Unity captures, R1 and original](Characters_PanelAndCards_R2_Review.html).

## Changes

- Restored a source blue rarity plaque, four/five concept stars, empty hero-level track, stat symbols and separators, four source ability medallions with live unavailable level dashes, and the team-bonus heading/emblem.
- Replaced the flat detail-panel face with clean source-derived texture. Removed the biography from the stat area; availability remains explicit.
- Restored taller cards:218×240, starting at y850, compared with R1's208×218 at y882. Added source class/affinity emblems and concept rarity stars. Eight character portraits remain the R1 extracted originals.
- Rebuilt each card from independent clean scenery, portrait, fade, border, symbols and live text. The approved clean Gameplay town plate is reused as an asset; the Gameplay page and its artwork are unchanged. Portrait lower edges blend into the name/star area instead of ending abruptly against a flat face.
- Added a restrained selected-card perimeter and taller future slots with silhouettes and locks. Condensed the information action into a labelled collection-preview link.
- Preserved RU/EN localization, existing filters/navigation, current Ivan and unchanged saves. Reference rarity/stars/emblems/abilities are explicitly previews. No ownership, hero levels, combat stats, ability levels, bonuses or upgrades are granted. Upgrade/Select remain unavailable.
- Rebuilt the existing Characters prefab and owning builder/runtime/view with revision2; preserved prefab/meta GUIDs and serialized bindings. No new startup or routing system.

## Scenario and asset workflow

Ten accepted Pixa source-alpha jobs: rarity plaque, star, four corner emblems and four ability medallions. **30CU**, no RGB generation or duplicate accepted jobs. Initial star submission disconnected; the job history accounted for all nine other accepted requests with no unmatched job before the replacement was submitted. Recovery evidence is retained in `Sources/Scenario/star_r2/Recovery.json`.

Scenario alpha was combined with original reference RGB. Bounded lettering/level masks remove baked text; local contour cleanup removes attached frame fragments and preserves intentionally opaque medal faces. Final medallions restore original rim pixels before clearing their numeric centers. Initial repeated-donor lettering artifacts on the rarity face were replaced by a coherent blue face. Submitted inputs remain immutable.

Five local derivatives/utilities: transparent card perimeter, soft selected border, textured detail face, portrait fade, and alpha-padding-trimmed empty progress track. R1 PNGs and all prior GUIDs remain unchanged. Reused stat icons, main hero, typography, header and navigation. No complete screenshot or contaminated screenshot region is used as runtime UI.

[Provenance](AssetProvenance.json) · [Input/source integrity](QA/AssetChecks.json) · [Scenario cost](QA/ScenarioAudit.json). Final alpha inspected on black, white and magenta.

## Verification

Unity6000.5.9f1 import/compilation, builder execution, prefab save/readback and revision2 cutover passed. Final Verify02: **2 Edit Mode + 5 Play Mode tests passed**, zero failures/skips. Seven actual Unity captures: RU/EN941×1672, safeEN576×1280, safeRU1080×2400, Rare/Masha, Firebird and information overlay. [Results and history](QA/TestSummary.json).

Checks cover complete bindings/sprite references, screenshot-free dependencies, idempotence, frame/caption draw order, real header updates, eight previews and filters, selected border state, unavailable hero/ability values, disabled actions, save preservation, native raycast targets, full rendered text and safe-area bounds, info input blocking/Back, navigation/currency categories and repeated/rebound/disabled callbacks.

Verify01 also passed seven tests, but visual inspection found frame overlays obscuring captions and a track shortened by texture padding. Corrected ordering/padding and recaptured in Verify02; strengthened the draw-order regression assertions. No failures are hidden or substituted with an offline rendering.

Source checks: ten immutable inputs and original RGB outside repair masks verified; R1 asset hashes unchanged; local frame RGB preserved outside declared repairs. Four Python sources parsed and scoped C# whitespace checked. Browser review evidence: `QA/BrowserCheck.json` (390px viewport, nine selectable views, links/images/overflow).

## Scope and remaining differences

[Scope audit](QA/ScopeAudit.json): seven existing Assets files changed—the Characters prefab, cutover, builder, runtime, view and two test files. Added15PNGs and their metas. No deletions; all existing metas/GUIDs preserved. ScreenRouter, layout component, existing R1 artwork, other pages, Gameplay/Splash, packages/settings and Android guards unchanged against the pre-task5803-file hash snapshot.

This is not an exact replica. R2 deliberately leaves the main Ivan/Firebird scene, title/header and footer composition for the next refinement. Their pose, brightness, setting, lower portrait cutoff, typography and square footer icons still differ. Card scenery is a reusable clean town plate rather than the obscured original; source portraits/emblems have limited resolution. Source-derived frames and repaired faces retain small texture/shape differences. Additional preview labels and inactive hero data also differ intentionally from the reference concept.

Not performed: APK/build, physical-phone/touch/hardware-Back testing, full-project test suite, profiling, real hero ownership/upgrade/backend integration, or exhaustive unrelated-page visual regression. No new reward/economy behavior.

Next: user visual review of R2. Recommended subsequent visual focus remains original main hero/Firebird and sunset scenery, followed by round navigation medallions; do not interpret this as implementation of that later scope.
