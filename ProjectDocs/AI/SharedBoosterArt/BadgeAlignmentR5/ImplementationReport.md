# Shared Booster Badge Alignment R5

The user reported that the badge number was too low and its colors differed from the original Booster Selection image. The shared live badge now uses a dark forest-green face, restrained antique-gold rim and pale-gold numerals. Visible digit geometry is centered rather than relying on font ascent/descent. The R4 residue cleanup and darker medallion metal are retained.

## Implementation

`ProductionPreLevelBadge.AlignCount` normalizes the count rectangle at double raster density, uses the existing local `RankNumbers` font, enables `SplashTitleTextGeometry` and disables conflicting text effects. A centered72%-width text area and best-fit limits keep multi-digit inventory counts within the circular face. Labels remain live and raycast-free. The source font identity is still unverified.

The procedural badge face is changed from bright glossy emerald to dark green with a restrained highlight. Metal shoulders/recesses use less saturated gold/brown. Original badge face pixels were inspected directly; no reference screenshot or baked sample number is used at runtime.

Updated owners: Booster Selection, Pre-Level source controls, Pause, Failure and Gameplay source controls. Pause counts are parented under their own badge without changing the serialized Text reference. `ProductionSharedBoosterArtBuilder.RebuildBadges` updates only the five corresponding active prefabs. Existing GUIDs, revision checks and gameplay functions remain intact. Preview tools still have no stock badges.

R5 scope: **7 existing C# files + 5 existing prefabs**, no added/deleted Assets files, no PNG/meta changes. The earlier R4 pass separately changed16PNGresources for residue removal/darker rims. See [exact R5 scope](QA/ScopeAndVerification.json).

## Verification

Unity6000.5.9f1 final Verify03 rebuild/import succeeded. **19EditMode +31PlayMode =50passed**, zero failed/skipped. Existing tests cover shared resources and five page flows, including0/7/10/999 staying on one line within the circular rim at narrow-phone size.

Verify01 detected10 crossing the rim; a smaller font cap fixed two-digit counts. Verify02 then detected999 crossing the rim; the final width constraint resolved that failure without weakening the test. Earlier run evidence remains under `/tmp/lor-booster-badge-r5`; final logs/NUnit outputs are copied to `QA/Verify03`.

Actual Unity Booster Selection, Gameplay and narrow Pre-Level stock captures were visually inspected. Original/before/current badge enlargements are in [AssetReview](AssetReview.html); original has7 while the test inventory has12. Browser verification loaded8native/reference views and3badge comparisons with no script errors or horizontal overflow at430px. Source colors/native gradients and font shapes remain an approximation, not an exact source-text extraction. NoAndroid build, physical-device test or full-suite run.

**Zero new Scenario requests or credits.** This pass updates native drawing/text geometry and retains the previously generated/extracted resources.

[Actual Unity page review](SharedBooster_BadgeAlignment_R5_Review.html) · [Badge comparison](AssetReview.html) · [R4 border cleanup and darker metal](../InnerBorderCleanupR4/AssetReview.html)
