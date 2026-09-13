# Home R6 — original-reference reconstruction

Status: Home revision **9** is implemented; final Unity verification passed **36/36 Edit Mode and 21/21 Play Mode**, with zero failures or skips. This is not a claim of exact visual identity or user acceptance. The earlier R5 implementation was rejected as too different.

[Actual Unity portrait](UnityVerification/Home/Home_941x1672_RU.png) · [Standalone portrait and original-reference overlay](Home_ReferenceReplica_R6_Review.html) · [Independent visual review](QA/FinalVisualReview.md)

The latest request explicitly includes Home's background, character, button and all other elements. It supersedes the earlier Home-only winter/Splash-reuse constraint. Home now uses a reconstructed dusk landscape, young red-cloaked Ivan with a perched Firebird, uppercase Russian crest, an emerald ИГРАТЬ/PLAY control, resource HUD, four navigation cards and a connected carved footer. Approved Splash and Loading are protected separately.

## Active implementation

- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab`, revision 9.
- Existing owner: `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`, `RebuildHomeAlignment`; `LockedDesignImporter.cs` consumes the existing runtime contract. These editor files needed no new changes this turn.
- Runtime: `ProductionHomeRuntime.cs`, `ProductionHomeView.cs`, `ProductionHomeLayout.cs`; new Home-only `ProductionHomeGoldText.cs` shades the live Play label while preserving native shadow geometry.
- Resources: `Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/`.
- Focused tests: `HomeAlignmentDeliveryEditModeTests.cs` and `HomeAlignmentPlayModeTests.cs`; existing Splash/Loading tests remain unchanged.

The existing registry, native controls, services, save data and navigation remain in use. There is no parallel startup, Canvas or EventSystem. Loading measures 25 actual Home textures plus its prefab: 26 operations. The retired separate R5 Firebird object remains inactive with its sprite cleared to preserve its existing serialized identity; the new combined character/Firebird occupies the existing Home hero slot.

## Artwork and live UI

The complete original `ApprovedScreens/Home.png` (941×1672) was provided as an actual image input during generation. Twenty-three selected outputs replace Home sprites; the two independently isolated progress sprites are retained. Original tool output, retained source and runtime PNG bytes match. No screenshot regions were cropped or composited into runtime UI, and no generated PNG pixels were edited. The background is a clean environment plate. Character and UI sprites have independently inspected alpha; opaque panel surfaces and the masked portrait are intentional.

Whole-sprite transforms place the new ornamentation around live labels and existing hit regions. HUD and navigation caption bands use measured source bounds. Play has a separate decorative artwork child around its existing hitbox; gold shading affects only live lettering. The Home instance of the existing character fade blends the lower 8 percent of the sprite, without changing the shared Splash fade source or PNG colors. The footer backing begins at the carved base instead of making a flat black strip above it.

Gold, gems, energy, level and daily reward availability use current game state. RU/EN labels remain localizable. The reference's quest/experience progress and reward numbers cannot be supplied by the current services; dashes and an empty progress state remain instead of invented values. The event exclamation is decorative reference artwork expressed through native UI; it does not claim a new event-count provider.

## Verification

Unity **6000.5.9f1 (b57deb96f08d)** compiled, imported, saved and read back Home revision 9 through its existing owner. Four owner invocations succeeded; each paired Edit Mode run passed 36/36. The final Play Mode run passed 21/21. Exact fixtures, invocation settings, cases and outcomes are retained in [Reproduction.json](UnityVerification/Reproduction.json), [ImportRuns.json](UnityVerification/ImportRuns.json), [FinalEditMode.json](UnityVerification/FinalEditMode.json) and [FinalPlayMode.json](UnityVerification/FinalPlayMode.json). Tests used the existing isolated `/tmp/lor-home-r3/test-config` configuration.

| Check performed | Outcome |
| --- | --- |
| Serialized Home owner/revision/resource contract and protected Splash wiring | 36/36 focused Edit Mode cases passed |
| Complete rendered RU/EN captions, Play dimensions/gradient/shadow, hero fade, safe-area bounds, no layout drift, HUD balances, live service refresh and 12 action routes | Included in final 21/21 focused Play Mode pass |
| Splash → Loading → Home, measured resource work, cancellation, failure and retry, responsive Splash/Loading | Included in the same focused Play Mode pass |
| Actual Home portraits | Seven retained original PNGs with fixture JSONs; direct review of RU, EN, narrow safe area, maximum balances and tall safe area |
| Sprite provenance and alpha | 23 new selections plus 2 retained sources inspected; exact tool/source/runtime bytes and black/white/magenta review recorded |
| Standalone review in local Chrome | All 5 portrait selections loaded without horizontal overflow; original overlay loaded, half-opacity and reset/hide behavior passed |
| Read-only decoded pixel comparisons | Splash: 75 changed pixels; Loading: 15; Home RU roundtrip: 2; all differences at most 1/255 per channel. These captures are not pixel-identical. |

The pixel counts each compare 1,573,352 pixels. Splash/Loading compare with retained R5 captures. The two Home roundtrip pixels lie within Play lettering. Exact comparison JSONs are retained in `UnityVerification/{Splash,Loading,LanguageRoundtrip}PixelComparison.json`. These rendering comparisons are separate from protected source-hash checks.

Intermediate failures are preserved in `UnityVerification/InitialPlayMode.json`, `IntermediatePlayMode.json` and `Intermediate3PlayMode.json`. They exposed caption fit, shadow triangle-stream counting in the test and the Russian Play width. Actual diagnostic review also drove corrections to the cloak cutoff, button depth, reward-frame height and footer transition. Functional passes do not establish exact reference fidelity.

## Changed files and preservation

[SourceChangeManifest.json](SourceChangeManifest.json) lists the exact 31 production paths changed from the pre-R6 local working tree: 23 PNG replacements, Home.prefab, five existing C# files and the new Home gold-text C# plus metadata. [AppliedSourceChanges.diff](AppliedSourceChanges.diff) contains the owned text diff; no Git staging, reset, commit or discarded local changes occurred. The existing editor importer/builder remains unchanged in this turn because it already consumes the updated runtime resource/revision contract.

[FinalSourceAudit.json](FinalSourceAudit.json) passed: 1,487 baseline paths, 1,458 unchanged, 29 intended modifications and exactly two additions. All 771 existing metadata files remain byte-identical and retain their GUIDs. The new gold-text GUID is unique and referenced by Home. All 336 previous prefab IDs remain; 33 were added, for 369 total. Protected groups are unchanged: 64 Splash, 30 Loading, 2 package, 25 ProjectSettings and 23 Android build/signing paths. All 39 owned pre-edit backups match the recorded baseline; all 23 selected tool/source/runtime PNG byte checks and both retained progress checks pass. Six changed/new C# files passed direct whitespace checks. [FinalProductionHashes.json](FinalProductionHashes.json) records all 1,489 resulting audited paths.

Documentation/evidence changes are confined to this R6 delivery directory, `ProjectDocs/AI/CurrentTask.md`, `ProjectDocs/AI/SessionLog.md`, and the R5 report's rejection/current-delivery link banner. Earlier acceptance and rejection history remains intact. Generated Unity caches are excluded. No credential, keystore or password material was read, printed, committed or packaged.

## Remaining differences

This reconstruction is substantially closer to the original composition, but the independently painted face, pose, feather paths, engraving, crest letterforms and small color details remain different. The reference's exact font is unidentified and not supplied: local `SplashTitleTypography.asset` still has no assigned RegularFont/StartFont and uses its existing development fallback. No OS/container font was distributed or relabeled as exact. Maximum balances remain complete but small in the narrow simulated phone fixture. The extreme 1080×2400 safe-area fixture adds an unoccupied decorative wood band below the live footer controls so the plate reaches the physical bottom.

No APK/AAB build, physical-phone installation/render/input check, full-project regression, performance/memory suite, physical display-resolution change or release/custom-signing validation was performed in this turn. Existing Android delivery predates this revision. User visual approval has not been inferred.
