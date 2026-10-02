# Chapter Complete R4 — scenery alignment

[Actual Unity portrait review](ChapterComplete_Scenery_R4_Review.html) · [Reference comparison](ReferenceComparison.md) · [Audit](QA/FinalAudit.json)

Replaced the clean village environment with a more closely guided reconstruction of the reference's scene area. The cottage now sits farther left, the upper-right branching canopy is restored and the church sits closer to its reference location. The center remains clean scenery behind the independently rendered Ivan, firebird, crest and interface.

The input `Sources/SceneReference.png` is a reference-only crop at (0,150)–(941,1091); it is never a runtime resource. Scenario removed the actors and interface and reconstructed the hidden village. The installed environment is the unmodified, fully opaque 1024×1024 output. It was inspected without overlays for unwanted text, ornament or actor remnants before integration. No scene pixels were pasted around invisible controls.

One Scenario request succeeded, 12 CU, no retry. [Exact prompt/request](Sources/ScenarioScene/Request.json), output hash, status and billing are saved under `Sources/ScenarioScene/`. Its generic helper filename `booster_medallion_trial01.png` contains the scenery output. The image-generation skill's preservation/review workflow was applied using the user's chosen Scenario provider.

## Production changes

- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/environment.png`: new clean plate, existing GUID retained.
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionChapterCompleteView.cs`: revision 4.
- `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`: verifies the inner scene and full-cover background use the same clean sprite and that the scene cannot intercept input.
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/ChapterComplete.prefab`: rebuilt/read back at revision 4 through its existing in-place upgrade.

The existing scene coordinates and responsive layout are unchanged. No subject, firebird, UI artwork, labels, reward quantities, routing or service behavior changed. No new resource or GUID was introduced. The production audit preserves all 176 prefab object IDs, all 2,395 previous metadata files and all 35 R3 evidence hashes; 4,563 of 4,567 baseline files are unchanged, with four changed and none added/deleted.

## Verification

Unity 6000.5.9f1 compilation/rebuild/readback completed in `run1`. Existing Chapter Complete/Pause checks passed: **4/4 Edit Mode and 6/6 Play Mode**, zero failures/skips. These cover serialized bindings/imports, absence of reference-page dependencies, repeated setup, actual rewards, no duplicate grants, navigation/acknowledgement, incomplete states, localization, safe areas, text fit, raycasts, button aspect ratios and non-overlap. Tests use isolated save storage.

Six Chapter Complete and six Pause PNG/JSON capture pairs, command records, logs and test XML are saved under `UnityVerification/`. Visually inspected final reference-size RU/EN, 576×1280 safe-area RU/EN and incomplete-state portraits against the approved reference and R3. The 1080×2400 RU variant received automated layout checks, not a separate visual review. Verified the installed PNG hash equals the raw Scenario output and its alpha is fully opaque. No new transparent assets were introduced.

Python syntax and all local review HTML targets checked. Final production/evidence hashes rechecked. Scoped whitespace check findings are recorded in `QA/DocumentChecks.json`.

Not run: APK/AAB build, physical Android testing, full project suite, separate ChapterFinaleCompletion suite, browser interaction, extended manual gameplay, performance profiling, exact font identification or calibrated color comparison. The scenery is closer, not pixel-identical, and the page has not been marked user-approved.

Next: review R4, then check readability and touches on a physical phone. Any further typography/material changes should remain a separate focused pass. Failure continuation stays scheduled after UI design completion.
