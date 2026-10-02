# Chapter Complete R3 — scene frame and footer

[Actual Unity portrait review](ChapterComplete_SceneFrame_R3_Review.html) · [Reference comparison](ReferenceComparison.md) · [Audit](QA/FinalAudit.json)

Restored the reference's narrow carved side rails between the header curves and reward footer. Both are individual transparent sprites with original source RGB and locally traced contours, positioned at source coordinates behind the existing composition. The footer now uses a continuous dark wood material instead of repeated repair patches. Its existing frame and complete alpha channel are preserved.

One Scenario generation succeeded: 1024×1024 opaque wood material, 12 CU, no retries. Only a clean wood sample was uploaded. The output was uniformly scaled/cropped and adjusted toward the source sample's mean RGB; it was not stretched or used as a whole page. Requests, response metadata, billing and output are under `Sources/ScenarioWood/`. The helper's generic `booster_medallion_trial01.png` filename contains the wood output, not booster artwork.

The active ChapterComplete prefab, runtime, serialized view and owning editor builder agree on revision 3. The upgrade preserves existing objects and adds two non-interactive images. Existing rewards, text, controls, localization, page layout and services are unchanged.

## Changed production files

- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/footer.png` — interior material only.
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/scene_rail_left.png` and `scene_rail_right.png`, with two new `.meta` files.
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/ChapterComplete.prefab`.
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionChapterCompleteRuntime.cs` and `ProductionChapterCompleteView.cs`.
- `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`.

No previous asset GUID changed. Baseline audit: 4,563 files, 4,558 unchanged, five existing files changed, four new files, none deleted. All 2,393 previous metadata files and 168 existing prefab object IDs remain; final prefab has 176 objects. All 35 R2 verification files retain their hashes. Two new GUIDs are unique. Other page assets/prefabs and prior user changes are preserved.

## Checks performed

- Unity 6000.5.9f1 compilation, scoped prefab rebuild, save and readback in `run1`.
- Chapter Complete/Pause: **4/4 Edit Mode and 6/6 Play Mode passed**, zero failed or skipped. Existing tests cover bindings/import settings, no screenshot dependencies or duplicate UI, real rewards, no duplicate grants, acknowledgement/navigation, incomplete states, localization, text fit, safe areas, raycasts, native button aspect ratios and non-overlap.
- Six Chapter Complete and six Pause PNG/JSON capture pairs saved with commands/logs/XML in `UnityVerification/`; tests used isolated save storage.
- Visually compared approved reference with final 941×1672 RU/EN, 576×1280 safe-area RU/EN and incomplete-state captures. The 1080×2400 RU capture was checked by automated layout assertions, not separately visually reviewed.
- Three asset hashes match installed files. Both rail RGB arrays match source crops exactly; footer alpha and all pixels outside the interior repair rectangle match the baseline.
- All three final assets visually inspected over black, white and magenta. Python syntax and local HTML resource paths checked.

Not performed: APK/AAB build, physical Android test, full project suite, repeat of the separate ChapterFinaleCompletion suite, browser interaction, extended manual gameplay, profiling, calibrated color comparison or exact font identification. Existing English blank-panel texture remains patterned. This is a reviewable refinement, not an exact-match or user-acceptance claim.

Next: visual review of R3, then physical-device readability and touch verification. Further village reconstruction should be treated as a separate focused pass. Failure paid/ad continuation remains scheduled after UI design completion.

Final hash recheck matched all 4,567 production files and 35 evidence files. Scoped `git diff --check` reported only Unity-serialized empty `m_Name` trailing spaces in ChapterComplete.prefab; these were retained, not claimed as a clean whitespace check.
