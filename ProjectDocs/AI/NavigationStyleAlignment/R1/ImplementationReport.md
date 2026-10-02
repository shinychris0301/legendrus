# Navigation R1 — implementation and verification

The shared navigation revision is implemented on Home, WorldMap, ChapterMap, PreLevel, Failure, Shop, SpecialOffer and DailyReward. User authorization covered headers and controls; Gameplay was subsequently excluded and remains unchanged.

[Portrait Unity review](Navigation_Alignment_R1_Review.html) · [Comparison / remaining differences](ReferenceComparison.md) · [Changed files](ChangedFiles.md)

## Implementation

- `ProductionNavigationStyle` applies a common reference-sized frame, resource icons, plus/settings controls and player-label spacing to the existing native hierarchy. Player portraits are reused. Shop and DailyReward portraits now use the full Home-size frame/pendant instead of their smaller previous treatment.
- WorldMap, ChapterMap, PreLevel, Shop and SpecialOffer use the verified Scenario-isolated circular Back arrow. Native button instances and actions remain in place. DailyReward already used this arrow.
- The large wooden strip behind the WorldMap/ChapterMap/PreLevel lower controls is replaced with a slim ornamental footer. Shop lower navigation captions use gold-bordered navy plaques. Existing illustrated destinations and primary actions remain functional.
- Existing page views and the Home/WorldMap/ChapterMap composition owners apply the same configuration. `ProductionNavigationPrefabBuilder.Rebuild` modifies and reads back the eight active prefabs without touching excluded pages. Home's artwork contract advances from revision14 to15; its actual preload manifest includes all seven new navigation resources, increasing from26 to33 textures. Loading artwork is unchanged and its progress still represents real preparation.
- Balance labels use double-density live glyphs and best-fit within the measured space between the icon and plus button. All digits remain visible at maximum integer balances. No names, resource values, XP, sample currencies or prices are baked into the new header.

## Scenario and artwork

Three successful Scenario jobs, **18 CU total**: blank-header reference edit12, header alpha3, icon-sheet alpha3. No retries or extra generations. Requests, outputs and billing are recorded in `Sources/Scenario`.

The five icons retain original DailyReward-reference RGB. Scenario's first alpha retained attached header fragments; authorized local silhouette cleanup removed those fragments. Black/white/magenta alpha review performed for icons and the header. The intentionally opaque navy panel face is preserved. The header frame is reconstructed and not an exact pixel extraction. Capsule artwork is reused from the existing isolated Home sprite; Back reuses DailyReward R2's previous Scenario extraction. Exact font identity remains unknown.

## Checks performed

- Project root and Unity6000.5.9f1 verified; root instructions, handoff, current task, provenance and initial dirty working tree inspected.
- Unity import, compilation, eight-prefab rebuild and binding readback passed (`QA/import4/unity.log`). Home's serialized revision is15.
- Final `QA/run4`: **22/22 EditMode and31/31 PlayMode**, zero failures/skips. Includes native bindings/no-reference-screen dependencies; repeated owner builds; RU/EN labels; safe-area containment and pointer raycasts; Back/profile/settings/resource navigation; Home resource preparation and actual balances including int.MaxValue; chapter/pre-level routes; Failure retry/exit/booster flow; Shop/SpecialOffer availability; DailyReward schedule/persistence/failed-save handling.
- Fresh Unity capture files: Home14, WorldMap8, ChapterMap6, PreLevel20, Failure10, Shop7, SpecialOffer5, DailyReward7. Standard941×1672 RU captures for all eight pages manually viewed, plus DailyReward576×1280 safe-area EN and Home576×1280 maximum balances. Other captured variants checked by their automated assertions. Home diagnostics are explicitly named diagnostic captures.
- Source icon RGB equality, alpha range, reference hash, Python syntax, review links and changed-C# whitespace checked.
- Start-of-task production baseline4680 files:4656 unchanged,24 modified,19 added,zero deleted. No existing `.meta` changed. No Gameplay files changed. Home's baseline matched HEAD; all385 original serialized object IDs remain, with no additions/deletions. See `QA/ScopeChecks.json` and `QA/HomeObjectIdentity.json`.
- Splash/Loading, Victory, Pause and ChapterComplete code/art/prefabs remain unchanged. Their existing captures were reviewed for scope; no fresh regression run for those pages is claimed. Existing services, routes, reward schedules, deferred continuation/SMS work, Android guards, packages, signing and project settings were not edited.

Earlier verification was not wholly successful: run1 exposed the old Home sprite contract, profile safe-area boundary and old null-sprite test assumptions; run2 exposed hard-coded old revision expectations and numeric/icon spacing; run3 exposed ordinary balance wrapping after density changes. These issues were corrected, keeping the behavioral assertions. The complete final run4 passed. XML results from all runs are retained.

## Checks not performed

No APK/AAB build, physical-phone test, full repository test suite, browser execution, performance profiling, calibrated color matching or exact-font identification. No payment, rewarded-ad or SMS integration was implemented. No exact whole-page visual match or user acceptance is claimed. Fine header engraving, portrait details and older lower-navigation illustrations remain different from the references as documented in the comparison.
