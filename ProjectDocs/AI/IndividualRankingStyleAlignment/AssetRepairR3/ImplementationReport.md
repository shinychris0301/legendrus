# Individual Ranking — Asset Repair R3

Implemented the user-approved R3 repairs in the existing native Individual Ranking page. The canonical prefab, view, owning builder and cutover now agree on revision3. Source artwork and all existing asset GUIDs were preserved; Gameplay and all other page prefabs/art were excluded.

## Changes

- **Title:** replaced the complete old lettering footprint, including its dark shadows, with clean source lacquer texture. Retained crest, rails and separate live localized heading.
- **Panels:** restored connected dark metal borders and corner ornaments on the reward panel, table and prize cards. Removed the former color-threshold mask that discarded metal shadows. Repaired small blue corner contamination and the lower table corner obscured by Back. Updated three sprite border definitions; retained their GUIDs. Main panels keep source proportions; the Rules surface tiles its texture to avoid vertical stretching.
- **Portraits:** repaired missing lower caps for Solovey, Vodyanoy and Zmey. Their original faces, eyes and upper artwork remain unchanged outside explicit repair masks. A small amount of missing beard/neck/rim artwork is reconstructed, not exactly recovered.
- **Rows:** native-size portrait/reward sprites are rendered on a separate non-interactive foreground layer, preventing row backings from cutting off overlaps. Purple chests and short-row portraits no longer shrink with row height. Power values share a right edge; gap dots moved up into the separators.
- **Preserved behavior:** live RU/EN text, real wallet/level values, clearly labelled sample standings, period controls, Rules overlay and existing routes. No new leaderboard, countdown, XP or prize grants.

## Scenario and asset workflow

Source assessment and local repair came first. Local portrait-cap extension produced streaks and was rejected before installation. One Scenario generation request produced a repair strip for the three portraits: **12CU** recorded. Only the missing lower caps were composited into source portraits; the generated full portraits are not runtime resources. No new extraction/generation requests were made for recoverable title or panel pixels.

[Request/status/billing/output](Sources/ScenarioPortraitCaps/Request.json) · [repair bounds](Sources/Repairs.json) · [provenance](AssetProvenance.json). Existing R2 Scenario work remains upstream provenance, not new R3 spending. R3 updates seven existing PNGs and adds no new runtime image files. Enlargement is interpolation, not additional original detail.

## Verification performed

- Unity6000.5.9f1 import/compile and prefab rebuild/readback passed twice.
- Run1 passed4 EditMode+11 PlayMode tests (8 Ranking checks and7 Event Detail regressions), zero failures/skips. Manual review then prompted the small corner repair and Rules texture tiling.
- Final focused run2 passed2 Ranking EditMode+6 PlayMode tests, zero failures/skips. Every latest result across15 distinct targeted checks passed. Existing relevant tests were reused; no artificial implementation-mirroring tests were added.
- Ten final Ranking capture pairs cover standard RU/EN, tall/safe-area RU/EN,1080×2400 safeRU, maximum balances, Day, Week and Rules. Six Event Detail regression capture pairs from run1 retained. Native controls, raycasts, safe areas, text fit, background aspect/coverage, no-save mutation, localization/live balances, routing and Rules blocking/Back behavior checked.
- Manually reviewed original reference, repaired title/panel textures, Scenario input/output, alpha sheets over black/white/magenta, run1 RU/tallEN/RulesRU, and final RU/Rules safeEN. Other final captures checked automatically.
- Seven independent original-RGB/mask audits passed; all three original upper portraits are unchanged before interpolation. Runtime hashes, Scenario output hash/billing, all41 asset GUIDs, prefab identities, source AST, scope, whitespace and review links checked. See `QA/`.

Commands and test XML are in `UnityVerification/run1` and `run2`; complete local Unity logs remain in `/tmp/lor-ranking-r3/`. Only focused build-result excerpts are copied into the review package.

## Not performed and remaining limits

No APK build, physical Android test, full-project suite, browser execution, profiling, calibrated color measurements, exact-font identification or live leaderboard integration. Passing tests do not prove exact visual fidelity.

Panel interiors and missing portrait caps are repaired approximations. Original-source resolution and fine edge limitations remain. Medal centers/laurels, typography, full footer decoration, shared header details and regenerated R2 scenery still differ from the reference; those are outside this focused R3 pass. No claim of exact match or user acceptance. Review R3 next; the suggested following visual pass is medal relief and typography, followed by the full footer base.
