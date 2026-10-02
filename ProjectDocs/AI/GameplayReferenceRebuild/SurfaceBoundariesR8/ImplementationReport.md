# Gameplay R8 — scenery seam and booster boundaries

**Subsequent user review rejected the scenery result.** R8 softened the seam but retained the dark patch itself. See [R9](../PortraitSceneryR9/ImplementationReport.md); the visual-completion statement below records the earlier assessment and is superseded. The booster repair remains in use.

The user’s close-up identified the flat, translucent-looking strip directly beneath Ivan. The cause was the rectangular join between the R1 generated portrait-underlay scenery and the original trees/sky. Removing small pieces of the header mask in R6/R7 could not correct that background seam.

## Implemented

- Repaired a 400×400 scenery crop through one Scenario API request, **12 CU**, with no resubmission. Blended the accepted output only around the offending join using the archived repair mask. **1,516,463 environment pixels remain unchanged**, with no edits outside that mask. The result is reconstructed scenery, not recovery of hidden original pixels.
- Restored the original booster card’s inward upper curls and lower carving. The old rectangular wood repair had covered the curls; the reference’s removed quantity badge also left an arc and interrupted divider. Continued the wood locally, completed the divider from original rail material, and replaced the caption’s contaminated center with a clean source wood strip.
- Preserved the R7 portrait and header sprites, their geometry, existing character, fonts, booster names/functions/counts, gems, board and gameplay behavior. No broad header redesign was applied following the user’s clarification.
- Active Gameplay prefab and owning builder now agree on **revision 19** and `gameplay-surface-boundaries-r8`. All **405 existing serialized prefab records** remain.

## Verification

- Unity **6000.5.9f1** batch compile, Gameplay prefab rebuild and serialized readback succeeded.
- **9 EditMode + 32 PlayMode tests passed**. Coverage includes Gameplay/Pre-Level integration, booster transactions, gameplay VFX, localization, caption geometry, input and phone safe areas.
- Extended the native control comparison across the former nameplate region: **8,424 pixels** match the same render with the entire HUD hidden. This excludes an actual UI overlay there; it does not establish scenery continuity on its own.
- Visually inspected the corrected scenery in native 941×1672 and 576×1280 safe-area captures, along with a magnified portrait-underlay crop. The former straight dark boundary is no longer visible in those captures.
- Inspected booster transparency over black, white and magenta and the native card close-up. Restored top carving matches original RGB throughout its recorded support, including **136 gold pixels** previously covered by the wood patch. Source lower curls and divider remain visible around live captions.
- All **18 browser review choices** loaded, with no script errors or horizontal overflow at 390px viewport width.
- Assets scope audit: **5 existing files modified, 5 files added, 0 deleted**. Existing portrait/header sprites, other page prefabs, fonts and art resources were not modified.

## Changed production files

- `Assets/LegendsOfRus/Resources/ProductionUI/GameplaySurfaceBoundariesR8/`: versioned environment/card PNGs and Unity metadata.
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplaySourceControls.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayView.cs`
- `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/GameplayAlignmentPlayModeTests.cs`

Evidence: `Sources/AssetProvenance.json`, `Sources/Scenario/scenery_seam/`, `QA/SurfaceChecks.json`, `QA/ScopeAndVerification.json`, `QA/BrowserChecks.json`, and the archived Unity logs/results.

No Android build, physical-phone run or full-project test suite was performed. Existing exact-font identity and Android font-binding limitations remain. Native captures use isolated test saves; large-value and special-piece views are controlled fixtures. The lower divider behind the original stock badge is reconstructed from a source donor, not pixel-exact hidden artwork.
