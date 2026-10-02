# Events — Reference Rebuild R1

Implemented the user's approved next screen, Visual Page 18 — Events, in the existing active Unity page. Internal Events revision is now **4**; this is the first EventsReferenceRebuild review, following the earlier EventsStyleAlignment R3.

## Changes

- Added an Events attachment to the existing `ProductionPlayerHeader`. Home artwork, portrait, live coins/gems/energy and native navigation are shared. Existing serialized button/text references are retained. The Events fresh-construction path now clones the current shared Shop header instead of searching obsolete child locations.
- Re-extracted the complete original Russian title and its upper crest through Scenario. Retained original RGB and separate alpha. English uses a live localized heading on a matching blank plaque; its cleared face reuses the earlier approved repair.
- Doubled the raster density of native card/tab/action/status/reward captions. A candidate heavier typeface was reviewed and rejected; the established regular face was retained for reference similarity. Shared header typography remains controlled by the shared component.
- Darkened/cooled the independent environment and heading scenery. Existing isolated actors, separate clean scenery, reward-example icons and hollow frame overlays remain in use.
- Anchored the composition to the safe top and Back/footer to the safe bottom. This keeps the header and title together on tall phones while preserving the four-card stack's proportions.
- Updated runtime, owning prefab builder, serialized Events prefab and cutover revision together. Gameplay and other page prefabs were not changed.

## Asset workflow

One new Scenario `model_pixa-background-removal` job: **3 CU**, no retries. The larger title input repairs the older crop's clipped crest. Visible RGB comes from the original image, not the removal service output: **62,857 visible pixels, zero RGB mismatches**. Alpha was inspected over black, white and magenta. The blank English face is a documented reconstruction and is excluded from this exact-RGB claim. Existing Scenario-generated scenery and extracted subjects are reused.

[Request, job and billing](Sources/ScenarioTitle) · [Title provenance](QA/TitleProvenance.json) · [Alpha inspection](QA/TitleAlpha.png).

## Verification

Unity **6000.5.9f1** final Verify04 import/rebuild succeeded. **4 EditMode + 13 PlayMode = 17 tests passed**, zero failures or skips. Coverage includes Events prefab/rebuild idempotence, live balances, localization, standard, narrow and tall simulated screen/safe-area arrangements, source-aspect preservation, label fit, input raycasts, Home/header routes, Firebird preview, unavailable-category behavior, unchanged save/claim state, plus Shop and Out of Energy regression flows. The top-edge assertion allows half-pixel numerical tolerance and separately requires the header to reach the safe top.

Verify01 revealed the old `Rect.Contains` test rejected controls exactly on the safe area's exclusive maximum edge. That assertion was corrected to inclusive bounds with half-pixel tolerance. Verify02 passed; visual inspection prompted the regular-font/density adjustment. Verify03 passed; tall-phone inspection then prompted keeping the full stack top-aligned to the header. Verify04 validates the final implementation. Earlier run evidence remains in `/tmp/lor-events-rebuild-r1`.

Eight actual Unity Events captures are provided; standard Russian and narrow English views were visually inspected, along with the new title alpha. The standalone review offers those eight captures, the original reference and a historical R3 capture. Browser verification checked all **10 views**, comparison toggle and a 430px viewport with no script errors or horizontal overflow.

[Exact changed Assets files and final counts](QA/ScopeAndVerification.json) · [Final evidence](QA/Verify04).

No Android build, physical-phone test or full-suite run was performed. Backend event scheduling/participation/reward delivery remain unimplemented: Upcoming, preview and labelled reward examples are preserved. [Remaining visual differences](QA/ReferenceFidelityReview.md) are documented; this is not a pixel-identical result.

[Open the implemented page review](Events_ReferenceRebuild_R1_Review.html).
