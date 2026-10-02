# Maintenance — Reference rebuild R1

Continued to Visual Page 33 after user approval of the sequence. Compared the existing native page with the original `ChatGPT Image Aug 25, 2026, 09_01_26 PM (5).png` (copy in Sources/Reference.png).

## Implemented

- Restored original Russian title lettering and the full red title surface into a new panel variant, preserving the accepted transparent outer mask and existing parchment cleanup.
- Restored the original completion-plaque caption and green UNDERSTOOD button lettering. The time field remains live and displays “Время не объявлено” / “Not announced”. English uses the retained blank artwork and live localized text.
- Reused the clean Home environment for visual continuity with the rebuilt screens. Preserved the approved shared logo and isolated gift preview. The reference uses a logo above the panel, so this page retains that arrangement.
- Added established press feedback to the action without changing its hit area or routing.
- Updated runtime/view, owning builder, active MaintenanceState prefab and cutover to revision 2. Existing GUIDs and prior art retained.

No maintenance service, countdown or compensation grant was added. Understood and system Back return Home through existing navigation. Compensation remains explicitly unconfirmed and the gift is a preview.

## Verification

Unity 6000.5.9f1 with isolated saves and simulated display/safe-area dimensions. Baseline01 passed 13 tests. Final Verify01: rebuild/readback succeeded; 4 Edit Mode + 10 Play Mode tests passed (14 total, zero failures/skips), including Offline regressions.

Checked localization, complete visible text, source-art state switching, pointer press/cancel feedback, safe-area hit testing, repeated/disabled/rebound input, save preservation and Home return. Five actual Maintenance captures cover RU/EN 941×1672, tall EN 576×1280, tall RU 1080×2400, and the pressed state. Seven review views load without script errors or narrow viewport overflow.

Exact source RGB in restored regions, unchanged alpha and unchanged remaining rows were asserted. Existing art/meta/font files and other page prefabs are protected by hashes; results in QA/IntegrityChecks.json. Alpha inspected on black, white and magenta. Full modified/added paths and test counts: QA/ScopeAndVerification.json.

No Android build, physical-device test, live backend/service test or full project suite was run. No Scenario jobs or credits used; the existing accepted source-alpha extraction was reused.

## Limits

The live body typography remains heavier than the original. English cleared surfaces and the prior parchment/timer repairs retain texture differences. The Home plate and approved logo differ from the exact original image. Reference sample countdown and reward promises are deliberately not shown without real service data. See QA/ReferenceFidelityReview.md.
