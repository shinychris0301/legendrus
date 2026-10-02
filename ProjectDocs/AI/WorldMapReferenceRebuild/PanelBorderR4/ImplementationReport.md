# World Map R4 — panel-border highlight

The active chapter now has a continuous chamfered gold perimeter with a soft halo. This is native UI geometry following the **chapter panel boundary**, independent of the extracted frame alpha. The old ornament-shaped glow is disabled. The original frame sprite, painting, text, chapter state and layout remain unchanged.

`WorldMapPanelBorderHighlight` draws only a narrow perimeter: no center fill, no raycast target, and normal ScrollRect masking. The owning composition migrates existing cards without deleting their serialized art records, and creates the same perimeter for fresh builds. Canonical prefab/view revision3 rebuilt and read back successfully.

Verification: Unity6000.5.9f1 compile/rebuild; **9 Edit Mode +16 Play Mode =25 passed**, no failures/skips, including Home regressions. Native RU/reference and tall-phone captures inspected. Tests cover the new geometric border/CanvasRenderer, disabled former glow, real state, drag scrolling, localization, routes and responsive layouts. On/off captures differ only inside the active card's narrow perimeter; the interior rectangle x118–823,y1056–1273 has zero changed pixels. See `QA/HighlightComparison.json`.

The first run caught a missing CanvasRenderer and an ambiguous reflection test method lookup. Both were corrected; initial result XML retained under `QA/InitialFailures`. Final results/logs/captures are under `QA/`. No Android APK/device test.

Scope: four existing Assets files changed (WorldMap prefab, source composition, view, Edit Mode tests); new border component and its meta only. No artwork PNGs or other pages changed. Zero Scenario calls/credits: this is a native UI outline, not new bitmap artwork.

Next: review the active panel border; the next screen in the approved sequence remains Chapter Map.

Scoped whitespace check passed; after verification, only Unity-serialized empty-name trailing spaces were normalized (no value changes).
