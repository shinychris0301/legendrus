# Event Detail R2 — Original Reference

Rebuilt the active Unity Event Detail page around the dedicated Firebird Festival original. The previous R1 Events-derived composition has been replaced with the original's large title, free Firebird silhouette, five milestone chests, reward cards, leaderboard and premium-pass hierarchy.

[Native capture review](EventDetail_OriginalReference_R2_Review.html) · [Fidelity findings](QA/ReferenceFidelityReview.md) · [Scope and test results](QA/ScopeAndVerification.json)

## Reference and implementation

Original: `/home/k2/Downloads/match3final/ChatGPT Image Aug 25, 2026, 08_59_50 PM (7).png`, copied to [Sources/OriginalReference.png](Sources/OriginalReference.png). Earlier statements that no dedicated original existed were incorrect; this pass uses the subsequently identified image.

The approved shared Home environment and actual `ProductionPlayerHeader` remain, including live energy. Original Russian title, Play and Activate lettering are retained with their artwork. English uses blank surfaces and live labels. Dynamic captions and quantities remain native, localizable text.

Thirty-three sprites separate title/buttons, Firebird and light, panel surfaces, chests, reward icons, rail and diamonds. The page is native UI with independent assets, not a full-screen reference image with hotspots. Intentionally opaque navy/purple panel surfaces remain opaque. Safe-area header and bottom controls were checked at reference and tall-phone sizes.

There is no event service yet. Dates are unconfirmed, balances/thresholds/quantities/ranks use em dashes, rewards and leaderboard rows are labelled examples/previews, and Play/Activate are disabled. Existing Back, profile, settings, shop and energy routes remain. No event reward, purchase or progression is granted.

## Asset workflow

Three Scenario jobs were accepted: Pixa asset atlas 3 CU, Pixa Firebird isolation 3 CU, and hidden-surface repair 13 CU: **19 CU total**, no paid retries. Request/status/billing evidence is under `Sources/Scenario/`.

Initial masks retained backing or omitted details. Original RGB was restored beneath corrected masks; the Firebird tail, hourglass, potion, reward emblem and premium chest received local source-based mask corrections. Complete milestone diamonds were extracted separately. Hidden text footprints use repaired surfaces. A bounded title repair removes baked feather overlap so the independently placed Firebird can overlap it. Source RGB preservation therefore does not imply every title pixel is unchanged.

Transparency was inspected against black, white and magenta. Final native Russian and narrow English captures were inspected after the last art fixes. Fine feather alpha, repaired surfaces and reconstructed light remain approximations.

## Changed project files

Eight existing Assets files changed: EventDetail runtime, view, layout, prefab builder, active prefab, cutover state, Edit Mode fixture and Play Mode fixture. Internal revision 3 agrees across builder, prefab and cutover. Added 33 PNGs, their 33 metas and the containing folder meta under `Resources/ProductionUI/EventDetailAligned/`. No existing PNG or other page prefab was changed; no Assets files were deleted. Full paths are in the scope report, compared against this task's initial Assets hash snapshot.

## Verification

Unity 6000.5.9f1 final **Verify03**: prefab rebuild/import succeeded; **4 Edit Mode and 10 Play Mode tests passed**, zero failures/skips. Checks cover EventDetail and Events regression, shared header/Home parity, hierarchy stability, preview restrictions, navigation, language fit, source asset placement, milestone count and safe edges. Six EventDetail captures and eight Events regression captures were produced using isolated test saves.

Verify01 found English reward-preview text wrapping; the caption was shortened. Verify02 passed before the final mask corrections; Verify03 repeated the relevant checks with final assets. Logs, commands and XML results for all three runs are retained in QA.

Browser review: nine views decoded, original-comparison toggle passed, no script errors or horizontal overflow at 430×900. The HTML displays actual Unity captures, with the original and earlier captures explicitly identified.

No Android build, physical-device check, performance assessment or full-project suite was run. Passing tests establish functionality/layout assertions, not pixel-perfect fidelity. Next action: user visual review; further work, if requested, should target fine Firebird edges and section typography rather than replacing the approved shared header/background.
