# Team Ranking — Reference Rebuild R1

User requested proceeding after Team Hub. Implemented Visual Page 22 using the dedicated original Team Ranking reference and approved shared assets.

## Implementation

- Migrated the active TeamRanking prefab to `ProductionPlayerHeader`, with saved energy as well as coins/gems. Typed adapter preserves existing serialized controls and routes.
- Added an idempotent page-specific reference layout. The builder now updates existing bindings instead of destroying/recreating the page hierarchy. View, builder, prefab and cutover agree on internal revision 2.
- Extracted the original Details button together with Russian lettering; original visible RGB retained, alpha traced/refined locally. English uses the previously approved blank button and live caption.
- Bound ten blue reward-gem images directly to the approved gameplay sapphire. Retained original title, crests, rainbow rewards, chests and selected embedded fonts.
- Refined count sizes, regularized table spacing/player plaque, reused the approved carved footer, and installed native press feedback.
- Top-aligned the body/header to safe bounds; Details and Back follow the safe bottom. Information overlay blocks input across the expanded safe area.
- Standings/rewards remain explicit examples. No online season, membership or reward-granting service was fabricated.

## Art workflow

No Scenario generation/API calls or credit use. Existing approved resources were available; the only new sprite is source-extracted button lettering. `Sources/AssetProvenance.json` records original source SHA and extraction bounds. `QA/AssetAlpha.png` was inspected over black, white and magenta. Existing art, fonts and metas remain unchanged.

## Verification

Final evidence: `QA/Verify03`; Unity 6000.5.9f1. Prefab rebuild/readback succeeded. **11 Edit Mode + 15 Play Mode tests passed**, including Team Ranking, Team Hub and Individual Ranking. Coverage includes complete native bindings, idempotency, shared asset references, original-lettering localization, routes committing once, safe-edge alignment, hit targets, text bounds, read-only filter/info behavior and save preservation.

Verify01 caught a test-only reference to a nonexistent header property; corrected to the actual balance binding. Verify02 passed all 26 checks. Verify03 adds explicit safe-top/safe-bottom and localized-button assertions and verifies the final patch. Isolated configuration/capture directories protected normal local saves.

Nine native Team Ranking captures cover RU/EN, safe areas, Details, filters and maximum balances. Team Hub and Individual Ranking regression captures are retained. Standalone review includes original and historical comparison views. Browser results are recorded in `QA/BrowserChecks.json`.

No Android build, physical-device, performance or full-suite testing was performed. See the [fidelity review](QA/ReferenceFidelityReview.md) for remaining visual differences; exact matching is not claimed.

## Scope

Nine existing Assets files modified; one sprite+meta and one layout script+meta added; zero deletions. Detailed changed paths and preservation assertions are in `QA/ScopeAndVerification.json` and `QA/PreservationChecks.json`. Existing PNGs/metas/fonts, all other page prefabs and prior edits were preserved.
