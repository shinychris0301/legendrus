# Shared texture audit R3 — 2026-10-01

**The next bounded candidate is the Android import size of `HomeAligned/coin_icon.png`, tested at 512×512 while preserving its source PNG, RGBA32 format and mipmaps.** No import change or further APK reduction was made in this audit. Current approved art and the 838,219,234-byte R2 APK are unchanged.

The coin is imported at **1,254×1,254** but its largest measured visible mesh is **229.54×229.54 pixels**, on Daily Reward at a simulated 1440×3200 display. The similarly oversized progress fill/frame were **not visible in any sampled state**; they remain referenced by disabled legacy objects and runtime builders. They are not safe deletion candidates merely because they are hidden.

## Current project review

The repeated request to resume after chat loss was resolved against current files, not the original 2026-09-12 handoff alone. Reviewed AGENTS.md, latest CurrentTask, current DevelopmentSchedule, updated handoff, local changes and R2 evidence. Unity version remains **6000.5.9f1**. R2 packaging was already complete and was not repeated. This task follows the recorded immediate next action: complete-use and largest-size audit before import changes.

The core-first schedule, approved Splash, 500-round Chapter 1 and recent UI/gameplay work remain in place. No phone execution, hosting/service activation or final-effects work was initiated. Previous acceptance/test totals remain historical; the checks below are this task's actual work.

## Reference audit

Scanned **5,498 text files** under Assets: 936 C# sources, 72 prefabs, one scene, 23 assets, 428 JSON files, 4,032 metadata files, three shaders and three materials. Recorded **23 GUID-reference lines**, **24 literal source-reference lines**, and **20 serialized Image components**. Traced constructed Resources paths, shared header replacement, runtime style refresh and builder ownership in source. The runtime-only `Resources.LoadAll<Sprite>` usage is confined to gameplay atlas sheets, not these Home assets.

| Texture | Source/imported size | Serialized image references | Current observed use |
|---|---:|---:|---|
| coin_icon | 1254×1254 | 9 across Home, DailyReward, PreLevel, SpecialOffer, Victory | Visible on DailyReward, PreLevel and SpecialOffer. Home's old icon is disabled; shared headers use separate reference art. Victory's old header icon and unused reward slots are inactive; live coin rewards switch to VictoryReferenceR1/coin. Shop replaces its original header icon too. |
| progress_fill | 2172×724 | 3 across Home and Gameplay | Home's old quest fill and Gameplay's old goal/star fills are inactive. Gameplay now presents its score through the current score-meter component. |
| progress_frame | 2172×724 | 8 across Home, Gameplay, Fail, PreLevel, ChapterMap, WorldMap | Retained old profile/quest/gameplay frames are disabled or inactive. Cloned headers account for several references absent from direct literal searches. |

[GUID references](QA/GuidReferences.json) · [source references](QA/SourceReferences.json) · [serialized hierarchy/activation inventory](QA/SerializedImages.json) · [coverage and limits](QA/ScanCoverage.json).

This covers the current known literal/GUID paths and sampled routes. It is not a proof about every future dynamic path, all gameplay/reward states, or every possible device resolution. In particular, hidden serialized references still hold dependencies, and old builders could restore their content unless changed together. No GUID, component, prefab or Resources entry was removed.

## Actual Unity measurements

The final Play Mode measurement fixture used the **existing RuntimeBootstrap, ScreenRouter and Canvas**, with isolated `/tmp` product storage and the previously approved process-only Vulkan option. It visited **10 routes × 2 languages × 5 display configurations = 100 configurations**, recording **200 matching image observations**. Routes: Home, PreLevel, Gameplay, Victory, Shop, SpecialOffer, DailyReward, Fail, ChapterMap and WorldMap. Captures/measurement use each current page's native layout. Page visits are layout fixtures, not completed gameplay journeys.

Display configurations: 360×640, 941×1672, 1080×2340, 1080×2400 with safe rectangle (45,100,990,2160), and 1440×3200. Values below measure the actual CanvasRenderer mesh projected through Unity's capture camera, rather than assuming the whole RectTransform is painted.

| Coin location | Reference 941×1672 | 1080×2340 | 1440×3200 |
|---|---:|---:|---:|
| PreLevel reward | 70.00 px | 80.34 px | 107.12 px |
| SpecialOffer preview | 99.00 px | 113.62 px | 151.50 px |
| DailyReward day 1 | **150.00 px** | **172.16 px** | **229.54 px** |

The DailyReward rectangle is 150×157 reference units, but the square coin's painted mesh is 150×150. PreLevel's active prefab uses 70×70; relying on an older 42×42 source adjustment alone would have understated this. RU/EN observations agree on these asset sizes. At 360×640 the respective coin widths are 26.78, 37.87 and 57.39 pixels.

The coin had **30 visible observations out of 90**. Progress fill had **0/30**, and frame **0/80**. Visibility required an enabled, active, non-culled graphic with a nonempty mesh and positive inherited alpha. No assumption that zero observed visibility means an asset is unused or unloaded.

[Final measurement test](QA/RuntimeFinal/results.xml) · [run command/outcome](QA/RuntimeFinal/summary.json) · [all observations](QA/RuntimeFinal/RuntimeMeasurements.json) · [summary](QA/RuntimeSummary.json).

Ten standalone 941×1672 Russian page captures were saved by the final run. These are **actual Unity offscreen Play Mode captures**, not Android screenshots or generated design references. Inspected the DailyReward, SpecialOffer and Gameplay captures from the initial successful run and the final PreLevel capture. Example: [DailyReward page](QA/RuntimeFinal/DailyReward_941x1672_RU_ACTUAL_UNITY.png). No new visual approval or all-page design acceptance is claimed.

## Candidate and acceptance requirements

Current importer owner is `LockedDesignImporter.ConfigureHomeAlignmentTexture`, also called by `Step20ProductionUiFix01SerializedPrefabBuilder` during Home art preparation. It deliberately retains RGBA32, sRGB, full-rect sprites, alpha transparency, clamp wrapping, box mipmaps and trilinear filtering for foreground controls. Android currently overrides max size to 4096. **Do not apply the generic ASTC rule or shrink the entire HomeAligned folder.**

The next experiment should change only the coin's Android max size to 512 through that owner and its existing `.png.meta`; keep default/editor source resolution, PNG bytes, GUID, sprite border, layout and all other assets unchanged. No prefab reference/geometry revision is needed for a pure platform import setting, but owning importer and serialized metadata must agree. This is a candidate to test, not an already accepted quality change.

RGBA32 full-mip arithmetic estimates **8,384,072 bytes currently versus 1,398,100 bytes at 512**, a **6,985,972-byte (6.66 MiB) raw texture payload reduction**. This is neither measured compressed APK savings nor measured device memory. The actual import dimensions, mip chain and build delta must be measured.512 gives about 2.23 source texels per displayed pixel at the largest sampled use; that ratio alone does not establish visual equivalence.

Before retaining the change:

1. Compare the actual original versus512 Unity import on DailyReward, SpecialOffer and PreLevel at all measured sizes, both languages, with native standalone captures and coin-region error measurements. Include active/inactive DailyReward card state and normal unavailable SpecialOffer state; keep services unavailable.
2. Inspect alpha edges over black, white and saturated backgrounds using the imported sprite. Check gold ornament detail, thin outlines, highlight clipping and mip shimmer. Preserve opaque coin interior. Do not regenerate, crop or recolor the source PNG.
3. Run affected page/importer checks; verify all serialized references, names and geometry still resolve. Retain existing Home source-pixel assertions for its separate HomeReferenceAligned assets rather than weakening them.
4. If visual checks pass, use the existing guarded Android build and fresh packaging procedure; compare measured artifact bytes, asset payload delta, signing/identity/permissions and ZIP/ELF alignment. Archive the current R2 artifact first. Revert only the experimental importer/meta changes if fidelity fails.

## Verification, changes and limits

**Final measurement run: 1 Play Mode test passed, 0 failed/skipped.** It exercised the 100 configurations above; those are not 100 independent tests. An earlier seven-page pass succeeded, then the GUID scan identified three additional cloned-header consumers, justifying the final expanded pass. An initial temporary-fixture compile error called an outdated CanvasRenderer.GetMesh overload; corrected to the installed API. No production fix was necessary, and the failed compilation attempt is not counted as a test pass.

The fixture was temporarily installed in the existing PlayMode test assembly, then removed with its generated `.meta` after each run. Its source and runner are retained under this task's Sources directory. No extra startup or Canvas was installed. [Cleanup](QA/TemporaryFixtureCleanup.json).

**All 7,774 baseline Assets/Packages/ProjectSettings files are unchanged; zero changed/added/deleted.** The current APK hash is unchanged from R2: `480cb80ddeea115741fb25b779fed855da0b740cf3c55d88721c5b1dfd12057c`. [Scope](QA/Scope.json). Authored changes are this report, three audit/fixture source files, evidence/captures, and CurrentTask/SessionLog/DevelopmentSchedule/handoff updates. Raw logs remain under `/tmp/lor-shared-texture-r3/`; generated caches are not part of the patch.

Not run: importer optimization, original/512 visual comparison, new APK build, Edit Mode suite, full gameplay regression, physical Android input/lifecycle/performance tests, release build or backend deployment. No source-font provenance resolution. Human rounds 31–500 pacing remains open. Phone work and hosting remain deferred; final gameplay effects remain last.

**Next action:** run the bounded coin-only Android 512 import experiment and its native visual/alpha checks; retain it only if those checks pass. Keep the dormant progress resources intact pending a separate dependency-cleanup task.
