# Events R2 — Card Frames and Rewards

Implemented the user's approved frame-join and reward-scale refinement in the active Events prefab. Internal runtime/prefab/cutover revision is **5**.

## Implementation

The old hollow frame mask contained opaque rectangular navy regions in its upper-left corner, producing the visible notch. Added `frame_ornaments_r5.png`, refining that previously accepted Scenario mask along the original gilding and a one-pixel dark bevel. RGB is unchanged. The independent illustration window now uses a rounded native stencil and extends toward the rails; foreground/scenery aspect ratios and frame-over-art ordering remain intact. Card positions/heights were adjusted toward the reference's visible rail positions.

Twelve tightly cropped sprites retain exact RGBA from existing Scenario-isolated reward artwork. Each is placed and scaled separately with preserved proportions. This restores the Firebird reward prominence and consistent baselines across the four cards. Hidden legacy group Images remain serialized for compatibility but no longer draw duplicate rewards. The Merchant's prior separate chest/medal overlays are inactive. Reward examples remain illustrative, not authoritative grants.

The shorter Ice Tournament description uses bounded fitting and revised spacing. The source Russian title, shared Home header, event availability, input routing and saved values remain unchanged. Only the Events prefab was rebuilt.

## Workflow and provenance

**Zero new Scenario requests / zero CU.** This refinement reuses the accepted source extractions and changes only alpha, exact crops and native layout. New frame RGB is unchanged; all twelve reward sprites exactly equal their source RGBA crops. Frame and reward transparency were inspected over black, white and magenta. [Asset provenance](QA/AssetProvenance.json) · [Integrity check](QA/SourceIntegrity.json).

## Verification

Unity **6000.5.9f1** final Verify03 rebuilt/imported successfully. **4 EditMode +13 PlayMode =17 passed**, zero failures/skips. Existing Events, Shop and Out of Energy coverage verifies native bindings, rebuild idempotence, RU/EN localization, standard/narrow/tall layouts, live balances, raycasts/routes, Firebird preview and unavailable categories without reward/save mutations. New assertions verify each reward clears its caption, action and lower decorative rail, and that a rounded stencil is present.

Verify01 exposed an Ice Tournament description overflow after the larger reward row. Verify02 showed a remaining one-pixel excess at the minimum text size. The final spacing correction fixes both without weakening the glyph-fit test. Earlier run evidence remains in `/tmp/lor-events-frames-r2`. Final evidence: [Verify03](QA/Verify03).

Eight actual Unity captures are included. Standard RU and narrow EN were visually inspected. Browser verification loaded all10 review choices, checked the comparison toggle and found no script errors or horizontal overflow at430px. [Browser checks](QA/BrowserChecks.json).

Six existing Assets files changed: runtime, view, builder, PlayMode checks, Events prefab and cutover. Thirteen PNGs and their metas were added; no existing PNG/meta replacements or deletions. [Exact scope](QA/ScopeAndVerification.json). Other pages' prefabs, shared booster resources and Gameplay are unchanged.

No Android build, physical-device test or full-suite run. [Remaining reference differences](QA/ReferenceFidelityReview.md) include scenery/glow, tab texture, fine individual frame carving, typography and footer ornamentation.

[Open the implemented page review](Events_CardFramesAndRewards_R2_Review.html).
