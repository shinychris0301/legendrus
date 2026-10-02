# Dormant progress dependency audit R5 — 2026-10-01

The scheduled dependency audit is complete. **Do not remove or relocate the two textures yet.** Sprite-only removal works on unsaved copies of all six installed consumers, but fresh construction still depends on the resources. The audit also reproduced three existing fresh-builder defects. No production change was retained and no size saving was delivered.

## Current dependencies

Targets are only `Resources/ProductionUI/HomeAligned/progress_fill.png` and `progress_frame.png`. GUID/import settings/source hashes are in [Assets.json](QA/Assets.json); source inventory and serialized component IDs are retained alongside it. These are different resources from Loading's `LoadingReferenceAligned/source_progress_fill`, canonical-kit progress art and shared ornaments; those remain outside this cleanup.

| Active prefab | Revision | Dormant Image references | Origin |
|---|---:|---:|---|
| Home | 22 | 3 | Profile frame, quest frame and measured fill |
| WorldMap | 6 | 1 | Home HUD clone |
| ChapterMap | 5 | 1 | Home HUD clone, independently of WorldMap |
| PreLevel | 6 | 1 | ChapterMap HUD clone |
| Fail | 14 | 1 | PreLevel HUD clone |
| Gameplay | 22 | 4 | Goal/star frames and fills created directly |

Unity `AssetDatabase.GetDependencies` confirmed the textures in all six prefab dependency sets. All 11 Images are disabled or under inactive objects. Current `HasRequiredBindings`, serialized revision checks and owning `PageNeedsBuild` accept all six installed prefabs. On disposable instances, clearing only the 11 sprite assignments and invoking current in-place presentation/build paths retains valid bindings, the same ordered Transform object references and zero restored target sprites. These are in-memory checks, not serialized edits or new visual acceptance.

The current source-composition path hides Home's old Graphics. `ProductionPlayerHeader` supplies visible header art; Gameplay's style hides its old bars and uses `ProductionGameplayScoreMeter`. Gameplay still writes `GoalProgress.fillAmount` and `StarProgress.fillAmount`; the latter feeds the visible score meter. Removing either Image binding invalidates Gameplay. Preserve those components and their references even when their sprites are eventually cleared.

Home's preparation list excludes the targets, but its separate legacy `TextureNames`/`LoadSprites` path requires both for a fresh build and old-layout migration. Returning null for either target through the real loader delegate causes the expected missing-standalone-sprite exception. Home's legacy `HasReferenceForegroundBindings` also names both resources; no callers were found in Assets C# sources, and it is not the current `HasRequiredBindings` contract. It still needs an explicit consistency decision in any cleanup.

Fresh Home, Gameplay and Fail construction succeeded and recreated 3, 4 and 1 target references respectively. Map/PreLevel construction copied a target reference before failing for the reasons below. Fresh cloning can therefore reintroduce the dependency even after an installed prefab is cleaned. Clearing references alone also leaves the textures under Resources; it establishes no APK saving.

## New repair-path defects

These failures reproduced on unchanged production sources with both inactive and active fresh roots. All six installed prefabs and their existing in-place build paths passed; this is not evidence that installed gameplay navigation is broken.

1. **WorldMap fresh build:** `ProductionWorldMapRuntime.BuildInto`, line 68, looks for `GoldResource/GoldValue`. The accepted Home header has moved the balance labels under `CapsuleArtwork`. The cloned `ProductionPlayerHeader` retains correct serialized references, but the builder dereferences the obsolete path first. ChapterMap has the same defect at line 74. Use cloned header bindings rather than another ambiguous name search: all three capsule parents share the name `CapsuleArtwork`.
2. **PreLevel fresh build:** the final validation throws `Pre-Level composition is incomplete`. `Header`, `TitlePlaque` and `SourcePlay` are null, and the fresh branch never applies `ProductionPreLevelSourceControls.Apply`, although its existing-content branch does. Applying that existing composition to the failed disposable instance made `HasRequiredBindings` true. This is a diagnostic intervention only; no production fix was installed.

See [active-root diagnostics](QA/FreshActiveProbe/summary.json), per-page `FreshFailure_*.json` records and [source trace](QA/OwnerTrace.txt).

## Verification actually run

- Unity **6000.5.9f1**, Edit Mode, batch/nographics, isolated `/tmp/lor-progress-dependencies-r5/` configuration/storage; no scene or prefab save.
- Main audit: **16 cases, 13 passed, 3 failed, 0 skipped**. [XML](QA/EditorFinal/results.xml), [summary](QA/EditorFinal/summary.json). The failures are the fresh WorldMap, ChapterMap and PreLevel cases above, not accepted passes.
- Active-root confirmation: the six fresh-builder cases were rerun; **3 passed, the same 3 failed**. This does not add distinct coverage. It also recorded missing fields/current paths and the successful unsaved PreLevel composition intervention.
- Initial temporary-fixture compilation failed because Unity 6000.5 rejects `GetInstanceID()`. Corrected the audit to compare actual Transform references. No production change was needed; the compile attempt is excluded from test totals.
- Temporary fixture and generated metadata removed after every invocation. Final source/GUID scan rerun without the fixture; [scope](QA/Scope.json) verifies all **7,774** baseline Assets/Packages/ProjectSettings files unchanged, zero changed/added/deleted.
- Existing R2 APK unchanged: **838,219,234 bytes**, SHA256 `480cb80ddeea115741fb25b779fed855da0b740cf3c55d88721c5b1dfd12057c`.

No Play Mode run, rendered capture, gameplay progress-state exercise, Android build/device test, importer change, texture relocation, commit or deployment. R3's visibility samples remain historical supporting evidence, not exhaustive state coverage. This audit does not claim the failing fresh paths are fixed or that future dynamic callers are impossible.

## Next bounded implementation

Repair the three fresh-construction defects through the existing runtime builders first, reusing serialized cloned header bindings and the existing PreLevel composition. Extend the retained audit into appropriate permanent regression tests; check fresh versus installed controls and revision agreement without rebuilding accepted pages unnecessarily.

Then remove the two legacy resource keys/assignments from Home and Gameplay owners, clear precisely the 11 dormant sprite references in the six active prefabs while retaining object/component IDs and progress state, and make fresh/in-place paths agree. Cover the old Home migration/validation contract explicitly. Verify actual native renders and gameplay score/goal updates before accepting removal.

Only after all references and constructors pass, move the two PNGs with their metadata/GUIDs intact to a non-runtime archive, preserving their bytes and import contract. `LockedDesignImporter` currently recognizes the HomeAligned path; its fallback applies 2048/ASTC and disables mipmaps, so a naive move would silently alter imports. An archive path and narrowly scoped preservation rule must be part of that step. Finally measure a guarded fresh APK; the previous approximately 16 MiB raw mip estimate is not compressed APK or device-memory savings.

Approved Splash/Title and coin imports remain unchanged. Human rounds 31–500 pacing, exact font provenance and physical Android lifecycle/performance/readability remain pending; phone execution and hosting deferred, final effects last.
