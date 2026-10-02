# Event Detail R4 — Footer and Leaderboard

Restored the carved footer and source-based leaderboard rows, enlarged portraits, and restored blank rank-medal silhouettes. The footer gift directly reuses the Home screen resource, per the user's instruction during this pass.

[Native capture review](EventDetail_FooterAndLeaders_R4_Review.html) · [Fidelity review](QA/ReferenceFidelityReview.md) · [Scope/test evidence](QA/ScopeAndVerification.json) · [Unchanged reused resources](Sources/ReusedAssets.json)

## Result

The footer now has a continuous dark carved backing anchored to the safe bottom. Its left control retains working Back navigation, with the existing approved arrow separated from its old ring and placed on the source-style sapphire medallion. The right side references `ProductionHomeSourceComposition.ArtRoot + "source_gift"` directly, at the Home sprite's original proportions. Its separate R3 rim is inactive, avoiding double rims. The gift is still a passive reward example, not a newly implemented reward action.

Leaderboard rows now begin directly below the heading and use the original bronze and sapphire surfaces. Three sample portraits were extracted from the dedicated reference, replacing the smaller substitutes; the fourth row retains the established Ivan portrait. Original medal outlines have blank centres and live em-dash rank placeholders. Unknown scores remain em dashes. The section heading explicitly says “ЛИДЕРЫ • ПРИМЕР” / “LEADERS • PREVIEW”, allowing the rows to match source spacing without losing the preview disclosure.

Shared Home header/background, live energy, original Russian title/buttons, R3 Firebird/panel repairs and existing unavailable actions remain. Existing gem, potion, Firebird reward, feather and chest resources were reused unchanged; no new gem, booster or mail icon generation was requested.

## Scenario and reuse instruction

One Scenario edit atlas repaired hidden footer/row surfaces, empty ring faces and medal-number footprints: **12 CU**, no retries. That request had already completed when the user requested using Home's gift and reusing existing resources. No subsequent generation request was submitted.

The newly prepared right ring/closed chest were removed from Assets before Unity import and archived only under `Sources/UnusedBeforeReuseInstruction/`. They are not referenced by the game. The actual Home gift and 12 existing reward sprites were hash-checked unchanged; the runtime test asserts direct Home gift resource identity.

Source RGB is preserved outside bounded repairs. Hidden carved wood, row interiors, cleared numerals and ring faces are reconstructed rather than claimed as exact original pixels. Original sample portraits retain source RGB beneath locally traced alpha. Source art and transparency were inspected on black, white and magenta; portrait row-background residue was trimmed before import. The footer is intentionally opaque carved artwork, not an environment screenshot.

Prompt, input atlas, unmodified output, cost evidence, source boxes, masks and reproduction scripts are retained in `Sources/`. Reuse existing approved assets first in future work; only missing artwork or necessary hidden-surface repairs should require new generation.

## Project changes

Seven existing Assets files changed: EventDetail runtime, view, layout, builder, active prefab, cutover state and Play Mode fixture. Eleven PNGs and their eleven metas were added: footer, two row surfaces, Back rim/arrow, three portraits and three blank rank medals. Existing PNGs/metas and other page prefabs are unchanged; no deletions. Internal revision **5** is aligned across owner, prefab and cutover. Full paths and the initial-hash comparison appear in the scope report.

## Verification

Unity **6000.5.9f1**, final **Verify02**: import/rebuild succeeded; **4 Edit Mode + 10 Play Mode tests passed**, zero failures/skips. Verify01 also passed; visual review then prompted a small vertical correction to rank placeholders, verified in Verify02.

Checks include existing EventDetail/Events behavior, shared header, localization, safe edges, no duplicate bindings, no unsupported reward grants, footer draw order, non-overlapping leaderboard rows, and exact Home gift sprite reuse without an extra rim. Six EventDetail captures and eight Events regression captures use isolated test saves. Final Russian and narrow English captures were visually inspected.

Browser review: nine views and comparison toggle passed at 430×900, with no script errors or horizontal overflow. These are actual Unity captures. No Android build, physical-device check, performance assessment or full-project test suite was run.

Remaining differences: individual reward/chest scale, section typography, disabled-button tint, fine Firebird alpha and reconstructed hidden ornament. The Home gift and functional Back intentionally differ from the dedicated source's side-control artwork/behavior. Recommended next refinement: **Reward Proportions and Typography**, using existing resources.
