# Offline / Connection — Reference rebuild R1

Continued from the accepted Tutorial R5 to Visual Page 32. The original Offline reference already existed in `OfflineStyleAlignment/ReferenceAlignmentR2`; this pass reuses its panel and crossed-plug extraction.

## Changes

- Restored original Russian title pixels into a separate panel variant and restored the original lettering on Retry and Home. Source RGB and accepted alpha are retained; full gold ornaments remain visible.
- Replaced the older independently cloned header pieces with the actual `ProductionPlayerHeader` used by Home/Shop. Coins, gems, energy, profile and settings retain existing routes and saved values. Header follows the safe top edge on tall screens.
- Reused Home’s clean environment plate, with the page’s existing dark tint. No new scenery generation or full-screen screenshot UI.
- Added the established press feedback to interactive controls. Russian network-detected heading and all English labels remain native text; no stale NO CONNECTION image is shown after connectivity is detected.
- Preserved Retry’s device-reachability probe and existing navigation guards. No server status is inferred from device connectivity. The displayed support availability remains honest.
- Fixed the clipped Russian network-detected description found during visual review and added a full-message visibility assertion.

The runtime, layout, owning builder, active `OfflineState.prefab` and cutover state agree at revision 3. Eight existing Assets files changed and three PNG/meta pairs were added; the complete list is in `QA/ScopeAndVerification.json`.

## Verification

Unity 6000.5.9f1. Isolated test saves and simulated phone safe areas/network availability; no user save modified.

- Baseline01: 18 targeted tests passed.
- Verify01: rebuild and 19 tests passed. Visual inspection subsequently found a clipped Russian network-detected message; this was corrected before the final run.
- Final Verify02: rebuild/readback succeeded; 5 Edit Mode + 14 Play Mode tests passed (19 total, zero failed/skipped). Details: `QA/ScopeAndVerification.json`.
- Actual Unity captures: standard 941×1672 RU, retry, network-detected RU; 576×1280 safe-area EN offline/network-detected; 1080×2400 safe-area RU.
- Navigation, retry/focus refresh, duplicate-input guards, save preservation, resource bindings, shared artwork, pointer feedback, safe-edge layout and text visibility checked. Tutorial regressions included.
- Review browser: all 8 views load; no script errors or narrow viewport overflow. 5,649 protected existing art/meta/font and other page prefab files are unchanged. New extraction inspected over black, white and magenta; exact RGB/alpha checks recorded in `QA/IntegrityChecks.json`.

No Android build, physical-device network check, backend availability test or full project suite was run. No Scenario jobs or credits used. Source extraction and masks reuse earlier approved work.

## Remaining differences

See `QA/ReferenceFidelityReview.md`. This is a functional reference-aligned page, not a pixel-identical recreation of sample account values or unavailable services.
