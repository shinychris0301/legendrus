# Failure R9 — offer-card proportions

Implemented the next approved visual refinement in the canonical Failure prefab and its owning builder/runtime at revision 9. [Open actual Unity portraits](Fail_OfferCards_R9_Review.html).

## Design changes

- New independent red and blue card artwork replaces the R4 offer sprites in the active page. Green faces are deeper, with clipped corners, simpler gold borders and no projecting top crest.
- Both cards use 324×140 bounds. The existing mesh fitter preserves border/corner and lower green-face details at uniform scale, extending only the plain upper spans. This deliberately replaces R7's whole-sprite aspect fitting: the two newly generated upper sections have different heights. No full-image stretch or screenshot crop is used.
- The attached green panels render approximately 50px tall including borders, versus roughly 30px previously. They remain somewhat wider/shallower than the reference, with different engraved details.
- Increased the live currency gem from26 to34px, price text24 to28px and play symbol22 to32px. Their bounds fit inside the green faces. Repositioned the ad title/status stack into the new upper area; existing localized wording and R8 label style remain.

Main/title/goal framing, character, background, header, retry, booster identities/counts and existing actions are preserved. Both offers remain disabled, with unavailable labels and an unknown-price dash. No purchase/ad behavior was added; that remains scheduled in [FAIL-CONT-01](../../DeferredFeatures/FAIL-CONT-01.md).

## Assets and prompts

Generated through the user's selected **Scenario API**, using `model_openai-gpt-image-2`, medium quality, one output per card. Two jobs succeeded, costing **24 compute units total**. No replacement jobs were submitted.

- [Paid-card runtime sprite](../../../../Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/paid_r9.png):998×345.
- [Ad-card runtime sprite](../../../../Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/ad_r9.png):990×394.
- [Complete generation prompts and source-reference provenance](Sources/ScenarioRequests.json).
- Original unmodified1024×512 outputs, sanitized job/output/billing records: `Sources/Scenario/paid_r9/` and `Sources/Scenario/ad_r9/`.

Used the imagegen skill's asset workflow with the user's Scenario provider preference. Existing authorization covered local transparency cleanup: alpha≤4→0, alpha≥235→255 and transparent-padding trim only. Retained RGB is byte-identical to the source within trimmed bounds. Both sprites were inspected over black, white and magenta; colored interiors remain opaque. Requests use the whole original reference solely as style guidance, never as runtime UI. See `QA/AssetPreparation.json`, `QA/AssetChecks.json`, `QA/ScenarioCosts.json`.

## Exact production changes

Six existing files: `Resources/ProductionUI/Pages/Fail.prefab`; `Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`; `Scripts/Runtime/UI/ProductionFailRuntime.cs`; `Scripts/Runtime/UI/ProductionFailView.cs`; `Tests/PlayMode/FailAlignmentPlayModeTests.cs`; `Tests/ProductionCoreLoopUiEditMode/FailAlignmentEditModeTests.cs` (all under `Assets/LegendsOfRus/`). Four new files: the two PNGs above and their .meta files. Existing R4 assets remain intact.

Of1,827 baseline files,1,821 are unchanged. All944 existing .meta files,509 old prefab IDs and100 R8 evidence hashes are preserved. Two mesh-fitting components bring the prefab to511 objects/components. New GUIDs are unique. Final production manifest contains1,831 files. See `AppliedSourceChanges.diff`, `FinalProductionHashes.json` and `QA/SourceAudit.json`.

## Verification performed

- Unity6000.5.9f1 compile/import and Failure-only rebuild/save/readback succeeded.
- **10/10 EditMode and18/18 PlayMode passed**, zero failures/skips, with isolated test saves/config. Failure, Victory and Pre-Level filters ran. Updated offer tests cover actual source-art landmarks, protected fitting and currency/price/play bounds within the green faces. Existing localization, safe-area, hit-target, route, goal-data, disabled-state and prefab-reuse checks passed.
- Saved9 Failure+12 Victory+20 Pre-Level PNG/JSON pairs, XML, commands and logs. Visually inspected941RU,576safeRU, three-goal576EN and booster-guideRU. Other captures were generated but not individually reviewed.
- R8/R9 actual941RU comparison:83,801 changed pixels, all within the offer region x103..837,y1147..1293. Zero pixel changes outside it. The initial mask estimate missed the final two rendered edge rows; the diagnostic uses observed bounds. This confirms scope, not reference fidelity.
- Verified source/prepared hashes and RGB preservation, alpha diagnostics, new GUID uniqueness, source whitespace, runner syntax, review image/link targets and final production/evidence hashes.

Not performed: browser render/dropdown interaction, APK/AAB build, physical Android test, full project suite, manual gameplay/loss/animation/prolonged-play check, performance profiling, live ad/payment verification, exact-font identification or calibrated color/blur comparison. No packages, settings, signing or Android guards changed.

Next: review R9 on the supplied portraits. Phone readability/touch verification is the next practical check; exact engraving, green-face proportions and font identity remain approximate. Visual approval of R9 is not assumed.
