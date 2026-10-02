# Error Dialog — Reference rebuild R1

Continued to Visual Page 34 after user instruction. Compared the existing modal with its dedicated original reference and checked the earlier ParchmentToneR2 repair.

## Implemented

Restored original Russian title, blue Retry and green Close lettering using source RGB and existing accepted extraction alpha. Added three versioned sprites; retained all existing art. The repaired parchment below title row 174 remains byte-identical. English uses the existing blank controls and live localizable text.

Added established pointer press feedback. Disabled controls retain a darkened base tint and cannot respond to presses. Retry is enabled only when a caller supplies its retry action. Support remains disabled with a truthful availability label; the reference’s contact wording was not restored because no service is configured.

The existing modal shade continues to block input to the real underlying page, including its shared header. No duplicate header, background screenshot or new routing system. Existing callback deferral, cancellation, Close/Back and save-preservation behavior remain.

Updated ProductionErrorDialogRuntime/View, owning builder, active EmptyErrorState prefab and cutover to revision 2, with test expectations in agreement. Full production scope: QA/ScopeAndVerification.json.

## Verification

Unity 6000.5.9f1, isolated saves. Baseline01: 13 checks passed. Final Verify01: rebuild/readback succeeded; 4 Edit Mode + 10 Play Mode tests passed (14 total, zero failures/skips), including Maintenance regressions. Six actual Error Dialog captures: RU/EN standard, two tall safe-area layouts, callback-enabled Retry, and pressed Close. Eight browser review views passed without script errors or horizontal overflow.

Tests cover modal blocking, source/localized art switching, disabled actions, press/cancel feedback, complete visible text, safe-area raycasts, Close/Back preserving the underlying Home instance, single deferred Retry, route cancellation and unchanged saves. Source RGB, alpha and untouched parchment assertions passed. 5,665 existing art/meta/font and other page prefab files unchanged. Alpha inspected over black, white and magenta; native RU, retry-enabled RU and tall EN captures inspected. Scoped whitespace checks passed.

No new generation or Scenario calls. No Android build, physical-device/backend/support check or full project suite. Exact source-font identity remains unverified. Remaining differences are in QA/ReferenceFidelityReview.md.
