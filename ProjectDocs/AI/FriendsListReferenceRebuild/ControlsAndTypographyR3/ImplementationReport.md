# Friends List R3 — lower controls and typography

Continues the accepted R2 recommendation. The generic lower controls are replaced with the original Sort/Remove button contours, metal rails, navy faces, people pictogram and Russian lettering. Their reference proportions are retained. English and the active sort state use live captions on repaired blank faces; the original people pictogram stays visible.

Body captions now use the existing embedded Title face, while player names retain the approved Names face. Tab, guild, presence and counter sizing has been adjusted; presence includes a colored dot. Exact source-font identity remains unverified. The shared header, R2 tabs/flags/outer frame, portraits, row actions, footer, scenery and native routes remain intact.

## Extraction

One Scenario `model_pixa-background-removal` request isolated a sheet containing both original controls: **3 CU**, no resubmission. Source RGB is retained under the returned alpha with bounded corner cleanup and opaque face restoration. Blank caption faces use smooth interpolation from adjacent original navy strips. Hidden material and tiny contour accuracy remain approximate. Source coordinates, request, response, billing and scripts are in `Sources/`.

Four new sprites plus their Unity metas are installed in `FriendsListAligned`: `sort_ru_r3`, `sort_blank_r3`, `remove_ru_r3`, `remove_blank_r3`. Existing approved artwork is preserved. Alpha was inspected over black, white and saturated pink.

## Implementation

The actual Friends List prefab, owning runtime/view/layout, builder and cutover are rebuilt together at internal revision **4**. Sorting, filtering, language switching, safe edges and local-only social explanations remain operational. Sort toggles its live selected-state caption; Remove opens the existing explanation and does not alter saved data.

Verification results are recorded in [QA](QA/ScopeAndVerification.json), with native Unity captures in the review. No Android build, physical-device/keyboard, performance, full-suite or live-backend testing is claimed.

## Final verification

Unity 6000.5.9f1, final `Verify03`: prefab rebuild/readback succeeded; **10 Edit Mode + 16 Play Mode = 26 tests passed**, zero failures/skips. Checks cover Friends List, Team Hub and Team Chat with isolated saves. Twelve native Friends List captures are archived; final Russian, narrow English and Requests images were visually inspected. All **15 browser review views** and the comparison toggle passed without script errors or horizontal overflow.

Two initial verification launchers ended after successful Unity import/shutdown logs, before tests ran. The detached final launcher completed all phases. The blank-face gold smear was corrected before final verification; no additional Scenario request was made. Scoped C# whitespace checks passed.

Source RGB checks found zero mismatches for 217,258 visible Russian-button pixels; this is color provenance, not proof of exact contours. Four PNG/four metas were added; seven existing Friends List files changed; nothing was deleted. Existing artwork/metas/fonts and other page prefabs remained unchanged by task-start SHA comparison.
