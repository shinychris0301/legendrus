# Maintenance — Responsive Audit R3

The existing Maintenance layout and Continue button already fit the tested safe areas. The 532×134 reference-pixel control is approximately 326×82 screen pixels on the narrow simulated phone. No production code, prefab, revision or artwork change is required. Runtime/view/builder/active prefab/cutover remain at revision 3; the approved R2 parchment surfaces are preserved.

Added native raycast tests for a disabled Continue button, language changes while the page is open, narrow/short/tall displays, button-edge taps and single return to Home. Assertions cover complete source images inside safe bounds, the gift clearing the button, non-interactive gift preview, full text visibility and unchanged gameplay saves. Existing tests cover system Back, repeated binding, press feedback and no invented countdown or reward grant.

This is an audit of the active prefab, not a prefab rebuild or source-art regeneration. Only the PlayMode test source is expected to change under Assets. Display sizes/safe areas are simulated; physical Android/device, backend, performance and full-project tests are outside this run. Existing reconstructed parchment and fallback-font differences remain; no exact source-font claim is made.

## Results

Unity 6000.5.9f1 Verify01 passed all 7 tests (2 EditMode + 5 PlayMode), zero failures/skips. Active prefab bindings and idempotent runtime setup passed; no production rebuild was performed. Produced 13 actual Unity captures. Eleven browser states and native-pixel toggle passed without overflow/errors. All five matching ParchmentSurfaceR2 captures are pixel-identical. Short Russian and tall English captures were visually inspected: full ornaments, readable localized messages and complete button silhouette remain visible.

Only `Assets/LegendsOfRus/Tests/PlayMode/MaintenanceAlignmentPlayModeTests.cs` changed under Assets. The other 7,195 Assets files are unchanged, with no additions or deletions. Added this review, scripts, reports and QA evidence. No image generation or Scenario calls.

Next recommended page: Error Dialog responsive/navigation audit.
