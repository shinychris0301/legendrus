# Chapter Complete R6 — Victory frame overflow fix

[Actual Unity review](ChapterComplete_VictoryFrameFit_R6_Review.html) · [Pixel audit](QA/PixelRepairAudit.json) · [Production audit](QA/FinalAudit.json)

The reported overflow was reproduced in the Chapter Complete page open in the IDE. Beside the Victory ribbon, the footer's rectangular wood repair covered the inner gold scrolls at both upper corners. The ribbon's position/size was already aligned with the reference; moving or shrinking it would not fix the damaged footer pixels.

Restored only the original corner ornaments and their opaque wood backing, softly blended at clean wood boundaries. Fully restored pixels match the original reference RGB exactly. The complete existing alpha channel and every pixel outside the repair mask are unchanged. No source text, actor or scenery region was added to the runtime panel. This uses the previously approved local extraction/cleanup workflow; **no Scenario/API generation or credits**.

Changed production files:

- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/footer.png` — two bounded upper-corner repairs.
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionChapterCompleteView.cs` — revision6.
- `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs` — validates the reward frame keeps native aspect and does not intercept input.
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/ChapterComplete.prefab` — existing in-place upgrade/rebuild/readback at revision6.

No layout, ribbon artwork, lettering, rewards, routing, other page, settings, packages or Android guard/signing changes. Baseline4567files:4563unchanged,4changed,none added/deleted. All2395metadatafiles,184existingprefabobjects and35R5evidence hashes preserved; no new GUIDs.

## Verification performed

Unity6000.5.9f1 compilation/rebuild/readback; **4/4 Edit Mode and6/6 Play Mode ChapterComplete/Pause checks passed**, zero failed/skipped. Existing checks cover bindings/imports, source-page exclusion, duplicate UI, reward/no-repeat-grant behavior, navigation/acknowledgement, localization, incomplete states, text fit, safe areas, raycasts, native button ratios and non-overlap. Isolated test storage used.

Six ChapterComplete and sixPause PNG/JSON capture pairs plus commands/logs/XML saved in `UnityVerification/`. Visually inspected before/reference/after941RU/EN Victory-area details and final576safeRU portrait. Remaining variants were captured and automatically checked, not individually visually reviewed this pass. Final footer inspected on black, white and magenta. Pixel-array audit checks alpha preservation, exact source RGB in opaque restored regions and unchanged pixels outside the mask. Installed sprite hash matches staging. Local HTML targets, Python syntax and finalproduction/evidencehashes checked; scoped whitespace findings recorded in `QA/DocumentChecks.json`.

Not run: APK/AAB/device/full suite/separate ChapterFinaleCompletion suite/browser interaction/extended manual gameplay/performance profiling/calibrated color or exact-font identification. Scope is the Chapter Complete Victory ribbon/reward-frame boundary; the separate Victory result page was not modified or tested specifically.

The reference's intended central ribbon-over-frame overlap remains. General R5 limitations (reconstructed scenery/materials, approximate live font and intentional real reward categories) remain. Next: review the corrected corners and continue phone readability/touch verification.
