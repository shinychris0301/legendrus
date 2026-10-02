# Characters — Safe Edges R3

User continued after accepting original-lettering R2. Implemented the recommended tall-phone layout refinement in the real Characters page.

## Changes
- Shared header is anchored at the safe-area top. Footer, Back, collection-info control and lower corner ornaments follow the safe-area bottom, retaining their original internal spacing.
- Extra height expands the collection viewport instead of adding space above and below the whole composition. Tall phones can reveal the full future row; reference/shorter displays retain drag scrolling.
- Collection content height and scroll offset are recalculated and clamped after resize/filter changes. The source-width card and portrait dimensions are unchanged.
- Narrow carved side trims extend to the new lower corners. Corner ornaments, title, header, portraits, cards and footer medallions retain their proportions.
- Info panel remains centered in the safe region; its input shield expands to cover both the top and bottom controls.
- Runtime/view, builder, active prefab and cutover agree on revision15. Russian source lettering, English labels, Ivan-only highlight, disabled upgrade/selection, balances and existing routes preserved.

## Scope and artwork
Six existing Assets files modified, none added/deleted. Exact paths: `QA/ScopeAndVerification.json`. No image generation, Scenario jobs, credits, PNG/meta/font changes. All5,701 protected existing files, including other page prefabs, remain unchanged. The prior R2 captures provide the before view.

## Verification
- Unity6000.5.9f1 import/compile/rebuild/readback passed.
- Verify01:3Edit+8Play passed; the new same-frame modal capture hit the underlying collection before the newly activated overlay had settled. Updated the test to wait for rendered UI, matching the established modal capture tests.
- Final Verify02: **3Edit+9Play=12tests passed**, zero failures/skips. Retained both runs.
- New geometry assertions cover safe top/bottom attachment and collection/footer separation on every capture. Added short941×1412 and tall576×1280 views, and tall1080×2400 modal coverage/hit tests. Verified resizing preserves the selected preview/current hero and saved data.
- Existing tests cover RU/EN text, original lettering states, press cancellation, disabled actions, filters, scrolling, modal blocking and routes.
- Twelve actual Unity captures; tall RU, short RU, tall modal inspected. Fourteen browser review views passed at430px without JS errors, decode failures or horizontal overflow.
- Reference-size pixel comparison and protected-file results are in QA. Scoped whitespace passed after formatting-only normalization of generated YAML.

No Android build, physical device, hardware Back, backend, full suite, performance or fresh-process save test was run. Simulated safe areas do not certify physical device behavior.

## Remaining differences
The reference is a fixed-aspect concept. On taller displays all available cards may fit, leaving intentional spare collection space below them rather than stretching the portraits. Earlier reconstructed scenery, repaired surfaces, live detail fonts and unavailable hero progression remain unchanged. Side-trim texture is stretched vertically; major ornaments are not. See `QA/ReferenceFidelityReview.md`.

Next: user review of responsive layout. A broader navigation and responsive-layout audit across the completed page sequence is a useful next step after this review.
