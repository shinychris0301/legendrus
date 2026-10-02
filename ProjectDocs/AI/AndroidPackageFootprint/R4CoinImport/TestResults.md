# Coin Android import experiment R4 — 2026-10-01

**Rejected both reduced imports and restored the original.** The 512 and 1024 Android variants changed the coin's filtered detail/highlights beyond the appearance-preservation limits defined before testing. There is **no retained production change or new APK saving**.

The final restored 1254×1254 import reproduces the corrected baseline **exactly within all 52 measured coin regions**: mean, 99th-percentile and maximum RGB differences are all zero. All 7,774 baseline Assets/Packages/ProjectSettings files and the R2 APK remain unchanged.

## Scope and method

The user authorized continuing to the next scheduled task: the coin-only Android import experiment from R3. Re-read current handoff/task/instructions, verified Unity 6000.5.9f1 and working-tree state, and snapshotted current files. Preserved existing uncommitted changes and approved Splash/art.

Each candidate temporarily changed only:

- `Assets/LegendsOfRus/Scripts/Editor/LockedDesignImporter.cs`: a name-specific Android size limit inside the existing `ConfigureHomeAlignmentTexture` owner.
- `Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/coin_icon.png.meta`: matching Android max-size override.

The original PNG, GUID, default import limit, RGBA32, sRGB, mipmaps, filtering and all prefab references/layouts remained intact. No blanket folder compression or source-image resize. The existing builder already calls this owner; no parallel builder or prefab hierarchy change was needed. Both files were restored from their exact task-start snapshots after rejection. [Rejected 512 patch](Sources/Rejected512.patch) · [Rejected 1024 patch](Sources/Rejected1024.patch). These are **rejected experiment records, not patches to apply**.

The temporary Play Mode fixture used the existing bootstrap/router/Canvas and isolated per-run `/tmp` save/config directories, with the previously authorized process-only Vulkan option. No new Canvas/EventSystem or startup path. Fixture source/meta were removed after each run. The current coin importer was actually reimported by Unity, and the fixture asserted the expected texture dimensions.

## Coverage and screening

Each measurement run captured 40 native page configurations: PreLevel, unavailable SpecialOffer, and DailyReward in both available day 1 and claimed day 1 states; each in Russian/English at 360×640, 941×1672, 1080×2340, 1080×2400 with safe rectangle (45,100,990,2160), and 1440×3200. DailyReward claimed state was reached through `GameState.ClaimDailyReward` in isolated storage. No commercial services were enabled.

An additional 12 diagnostics rendered the imported sprite at 27, 57, 150 and 230 pixels on black, white and saturated magenta. These are Unity test renders of the existing asset, not newly generated art. Corrected diagnostics assert the actual background pixel colour so an opaque UI backdrop cannot invalidate them silently.

Before running either candidate, the fixture defined conservative screening limits for each coin's rendered mesh bounds plus a 2px margin:

- Mean absolute RGB difference ≤3 on the 0–255 channel scale.
- 99th percentile of each pixel's maximum channel difference ≤24.
- Geometry difference ≤0.02 screen pixels.

These are screening criteria for preserving the approved appearance, not a universal perceptual-quality metric. Visual review also compared the source-detail/edge rendering. Both candidates produced a smoother, differently emphasized coin face; their unchanged geometry and sufficient nominal pixel count did not establish visual equivalence.

| State | Imported size / mip levels | Valid comparisons | Comparisons outside limits | Worst mean RGB difference | Worst p99 channel difference |
|---|---|---:|---:|---:|---:|
| 512 candidate | 512×512 / 10 | 40 native pages | 40 |6.063 |57 |
| 1024 candidate | 1024×1024 / 11 | 40 pages + 12 backgrounds |52 |6.064 |57 |
|Restored original | 1254×1254 / 11 | 40 pages + 12 backgrounds |**0** |**0** |**0** |

Geometry/activation matched throughout. [512 comparison](QA/Candidate512Comparison.json) · [1024 comparison](QA/Candidate1024Comparison.json) · [Restoration comparison](QA/RestoredComparison.json).

The first baseline/512 diagnostic backgrounds were obscured by a native Canvas backdrop outside the page viewport. Those 24 initial diagnostic images (12 per run) are retained as failed fixture evidence but **excluded from alpha-background acceptance**. Their 40 native page pairs remain valid and already rejected 512. Corrected the fixture with an explicit background Image on the existing Canvas plus pixel-colour assertions, then recaptured the original before testing 1024. The final restored run repeated this corrected fixture. No production backdrop or UI was changed.

Inspected original/512 dark-background diagnostics, corrected original and 1024 renders over all three backgrounds, and the 1024 standalone DailyReward reference-size page. Fine texture/highlight differences were apparent. Corrected alpha edges retained a recognizable clean cutout, but that alone did not override the appearance differences. Example final baseline page: [restored DailyReward, actual Unity 941×1672](QA/Restored/DailyReward_941x1672_ru_0_ACTUAL_UNITY.png). All captures are **actual Unity offscreen Play Mode renders**, not physical Android captures or new design approvals.

## Checks and final state

- Five Unity runs completed: first baseline, 512 candidate, corrected baseline, 1024 candidate, restored original. Each executed the same one measurement test successfully: **five passing executions of one distinct test**, zero harness failures/skips. Successful capture execution is not candidate acceptance; the two candidates failed the separate screening shown above. [Run inventory](QA/RunInventory.json).
- Final restored run: **1 Play Mode measurement test passed**, 52/52 valid regional comparisons have exactly zero RGB difference. Reimported dimensions returned to 1254×1254 with 11 mip levels. [Final test results](QA/Restored/results.xml) · [Command/outcome](QA/Restored/summary.json).
- **7,774 source/config files unchanged; 0 changed/added/deleted** after temporary-fixture removal. Approved Splash, PNG bytes, metadata, prefab objects and prior user changes are preserved. [Scope](QA/Scope.json).
- Existing APK remains **838,219,234 bytes**, SHA256 `480cb80ddeea115741fb25b779fed855da0b740cf3c55d88721c5b1dfd12057c`. No new packaging was attempted because neither candidate met the prerequisite fidelity checks.

Authored delivery consists of four experiment/analysis source files, rejected patches and task-start snapshots, captured QA evidence, this report and CurrentTask/SessionLog/DevelopmentSchedule/handoff updates. Raw logs and isolated product data remain under `/tmp/lor-coin-import-r4/`; generated caches are outside the authored patch. No commit or deployment.

Not run: new guarded APK build, Edit Mode suite, broad gameplay regression, physical-phone input/lifecycle/memory/performance verification, temporal shimmer sweep, release signing or backend activation. The theoretical mip-payload savings from R3 were **not delivered**, and no device-memory saving is claimed. Human rounds 31–500 pacing and font provenance remain open.

**Next scheduled local task:** audit a narrow removal of obsolete progress-fill/frame dependencies from disabled UI, including all owning builders, view validation and cloned headers. Establish that no current state or repair path consumes them before changing serialized references or moving Resources assets. Preserve object IDs/GUIDs and visible art; do not delete textures from the visibility result alone. Stop further visible-coin downsizing. Physical-phone work and hosting remain deferred; final gameplay effects remain last.
