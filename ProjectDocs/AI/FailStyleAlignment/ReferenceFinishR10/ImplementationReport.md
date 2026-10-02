# Failure reference refinement R10 — partial implementation

[Actual Unity portrait review](Fail_ReferenceFinish_R10_Review.html) · [Reference comparison](ReferenceComparison.md)

The completed part is implemented in the canonical Failure prefab and owning runtime/builder. **The full requested pass is not complete:** Scenario completed six independent assets, then repeatedly rejected the next job with HTTP429. The latest user-requested single retry captured the explicit API reason: the plan’s creative units limit has been reached. No reset/retry header was provided; the numerical allowance and reset time remain unverified. No further requests are running. User subsequently chose to retain Scenario and wait for restored access. Generation is paused; no fallback provider or background monitoring has been started.

## Implemented

New independent scenery, isolated Ivan, ornate main frame, clean-sided title, goal frame and nameplate. Their navy surfaces and fine gold detail follow the supplied reference. The title/main bounds and navigation remain intact. Fortress placement and hero framing changed together in runtime and adaptive layout. The title text moved upward and its live bevel strengthened. Exact font identity remains unknown; no font files were imported or distributed.

User-approved Failure captions are «Молот грома», «Феникс», «Щит дружины», «Зелье силы». Existing quantities, inventory identifiers and gameplay functions remain unchanged. Guides explain row/column clearing,3×3 clearing, selected-color clearing and+5 gameplay moves respectively. English captions have additional wrapping and side padding. Existing R9 booster illustrations and offer cards remain in use; runtime has no references to missing generated assets.

The original reference is a style/composition input only. No screenshot-region crops or complete screenshot are runtime art. All six completed Scenario model_openai-gpt-image-2 outputs were generated once, medium quality,12CU each (72CU total). Original sources, requests, job/status/billing records and output hashes are retained under Sources. Twelve asset request bodies were prepared; only six jobs were accepted. The remaining request statuses are in QA/GenerationStatus.json.

Authorized local preparation changed alpha endpoints and trimmed transparent padding only. RGB preservation passed for all retained pixels; environment copied byte-for-byte. All five isolated sprites (including Ivan) were examined over black/white/magenta. The apparent orange background in the raw preview was hidden RGB outside alpha; no extra segmentation or regeneration was used. Six new PNGs and fresh GUID-bearing metas were added, with old assets retained.

## Verification performed

- Unity6000.5.9f1 import/compilation, Failure-only prefab rebuild and revision10 readback passed.
- Final run3:10/10 selected EditMode and18/18 selected PlayMode tests passed, no failures/skips. Covers Failure, Victory and Pre-Level, native controls, safe-area bounds, text fit, goal data, unchanged guide functions/stock and navigation.
- Run1 passed all Failure checks but one existing Pre-Level test failed on lastEnergyUtcTicks changing during a whole-save comparison (17/18 PlayMode). Run2 repeated PlayMode successfully (18/18) without changing that fixture or energy behavior. Run3 rebuilt after caption padding/wrapping adjustment and passed all28 checks. Earlier logs/XML/commands retained under UnityVerification/Attempts.
- Saved9 Failure,12 Victory and20 Pre-Level PNG/JSON pairs from final Unity run3. Viewed final941RU and576px three-goalEN captures; additionally viewed run1 normalRU, safeRU, three-goalEN and booster-guideRU. Not every capture was manually inspected.
- Six source/RGB checks; transparent sprites examined over three contrasting backgrounds. No calibrated color/blur scoring.
-1,831 baseline production files audited:1,824 unchanged,7 existing files changed,12 new PNG/meta files. All946 existing metas and all511 prior prefab object/component IDs preserved. Prior93 R9 evidence hashes verified unchanged after correcting a report-copy destination mistake; QA/EvidenceRecovery.md records the recovery.
- Python source parsing, final resource/meta GUID checks and local review-link validation performed. Final production/evidence hash manifests retained.

## Checks not performed

No APK/AAB build, Android install/phone test, manual full loss/playthrough, browser rendering/dropdown interaction, performance/profiling, full test suite, exact-font identification or live ads/payment continuation. No packages, Android guards, signing settings or other page implementations were changed. Prior R9 APK is not an R10 build.

## Remaining work

Generate and integrate paid_r10/ad_r10 plus mace_r10/firebird_r10/shield_r10/potion_r10. Their requests are prepared, but no accepted jobs or usable outputs exist. Recheck offer-face containment, whole-medallion scale, transparency and localized captions after integration. The current scenery, portrait, font and engraving remain interpretations rather than exact pixels. Visual acceptance is pending.

Paid+5 moves and rewarded-ad continuation remain disabled and scheduled in [FAIL-CONT-01](../../DeferredFeatures/FAIL-CONT-01.md) after UI completion. The ordinary booster that adds5 moves retains its existing gameplay function; it does not implement a paid Failure continuation.

## Existing production files changed

- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Fail.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionFailLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionFailRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionFailView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/FailAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/FailAlignmentEditModeTests.cs`

## New production files

- `Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/environment_r10.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/environment_r10.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/ivan_r10.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/ivan_r10.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/panel_r10.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/panel_r10.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/title_r10.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/title_r10.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/goals_r10.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/goals_r10.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/nameplate_r10.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/nameplate_r10.png.meta`
