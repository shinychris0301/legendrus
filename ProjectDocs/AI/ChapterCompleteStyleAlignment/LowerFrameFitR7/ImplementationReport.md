# Chapter Complete R7 — lower frame decoration repair

[Actual Unity review](ChapterComplete_LowerFrameFit_R7_Review.html) · [Pixel checks](QA/PixelRepairAudit.json) · [Change audit](QA/FinalAudit.json)

Confirmed the user's remaining issue: both lower inward gold scrolls on the Chapter Complete Victory/reward panel were cut off by the rectangular wood repair. R6 fixed only the upper corners. R7 restores the lower source scrolls and their opaque backing with traced, softly edged masks that exclude the separate Chapter 2 teaser art/text. The full frame perimeter was inspected to check for remaining straight cutoffs of the outer scrollwork.

Original source RGB is retained in the opaque repair regions. All alpha and every pixel outside the lower repair masks are unchanged, including R6 upper repairs. The wood surface, ribbon, teaser, characters, layout, labels, rewards and routes retain their existing implementations. No Scenario/API call or generation credits were used.

Changed production: `ChapterCompleteAligned/footer.png`, `Pages/ChapterComplete.prefab`, `ProductionChapterCompleteView.cs` (revision7), and `Step20ProductionUiFix01SerializedPrefabBuilder.cs` (validates native footer dimensions as well as aspect/input behavior). All are under `Assets/LegendsOfRus/Resources/ProductionUI/` or `Assets/LegendsOfRus/Scripts/`. No resource GUID changed. Baseline4567files:4563unchanged,4existingchanged,none added/deleted. All2395metas,184prefabobjectIDs and35R6evidence hashes preserved.

## Checks performed

- Unity6000.5.9f1 scoped compilation, prefab rebuild/save/readback.
- ChapterComplete/Pause: **4/4 Edit Mode and6/6 Play Mode passed**, zero failures/skips. Existing checks include bindings/imports, no source-screen dependencies/duplicate UI, rewards/no duplicate grants, acknowledgement/navigation, localization, incomplete states, text fit, safe areas, raycasts, aspect ratios and non-overlap.
- Six ChapterComplete and sixPause PNG/JSON capture pairs saved with commands/logs/XML; isolated test storage used.
- Visually compared original/reference and current lower-corner sprite details. Inspected repaired full frame on black/white/magenta, final941RU/EN lower-panel details and576safeRU full portrait. Other capture variants received automated checks, not separate visual review.
- Pixel audit: original alpha, outside-mask pixels and existing upper repairs unchanged; opaque repaired RGB exactly matches source. Installed PNG hash equals staging. Python syntax, local HTML targets and finalproduction/evidence hashes verified. Scoped whitespace findings recorded in `QA/DocumentChecks.json`.

Not performed: APK/AAB build, physical-phone test, full project suite, separate ChapterFinaleCompletion suite, browser interaction, extended manual play, profiling, exact-font identification or calibrated color comparison. The separate Victory result page was not changed; this fixes the panel on ChapterComplete open in the IDE.

Next: review both corrected lower corners, then phone readability/touch verification. Previously documented scenery/font/material differences remain; this is not an exact-match claim for the entire page.
