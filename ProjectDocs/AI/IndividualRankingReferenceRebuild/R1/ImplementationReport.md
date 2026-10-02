# Individual Ranking — Reference Rebuild R1

Continued to Visual Page 20 after Event Detail R7. The native Individual Ranking page now uses the shared Home player/resource header, safe-edge navigation, the original Russian Rules button lettering, and approved sapphire reward icons. The existing source title, top-three medals, portraits, chests and user-selected fonts are preserved.

## Implementation

- Added an Individual Ranking adapter to `ProductionPlayerHeader`, retaining the page's existing button bindings and routing. Coins, gems and energy come from saved state; the experience field remains unknown rather than fabricating progress.
- Header is anchored to the safe top. Back, Rules and a reused carved footer are anchored to the safe bottom. The ranking table retains its source composition; taller phones show more scenery below the table. The Rules modal is centered in the safe viewport with a full-height input shield.
- Restored the wider Week tab and source-relative spacing. Adjusted tab and English Rules caption baselines.
- Directly reused `ProductionGameplayArt.Gem(1)` in all three prize cards. Original page artwork and the approved Roboto Serif SemiBold, FreeSerif Bold and Bitter SemiBold resources are unchanged.
- Added `rules_original_ru.png`: original 450×81 source RGB including lettering and button, with the accepted isolated button alpha reduced to native size. The Russian semantic label remains, but its rendering is disabled to avoid overprinting. English uses the existing blank button and live localized text.
- Reused `EventDetailAligned/footer_r4` with subdued bronze tint. This is an adaptation, not a claim that this reference's footer was extracted exactly. Reused native press feedback without adding route listeners.
- Rebuilt the actual `IndividualRanking.prefab`. Owner, builder and cutover are revision 7. Updated fresh construction to clone the current shared header instead of looking for obsolete Shop header objects.

## Art workflow

No new Scenario jobs or credits. Existing approved assets satisfy this pass; the only new sprite restores already-visible source lettering. Source bounds, SHA and alpha provenance are in `Sources/AssetProvenance.json`. Source RGB equality was asserted. `QA/RulesAlpha.png` was inspected on black, white and magenta. Fine inherited alpha remains estimated.

## Verification

Final `Verify03`, Unity 6000.5.9f1: rebuild/readback succeeded; **2 Edit Mode + 6 Play Mode tests passed**, zero failed or skipped. Ten actual native captures cover Russian, English, tall safe areas, maximum balances, all period tabs and Rules. Final Russian 941×1672 and English 576×1280 safe-area captures were visually inspected.

Checks include source-title/button locale switching, selected fonts, shared-header/resource identity, safe-edge positions, text fit, row separation, raycasts, routing once, Rules blocking/dismissal, preview state without save/reward mutation, and repeated rebuilds keeping carving behind navigation. The Profile-to-Ranking regression now uses the current Profile information panel and native route button.

Verify01 exposed old test assumptions about exclusive `Rect.Contains` upper edges and a removed Profile hotspot. Verify02 used the native route but initially omitted opening its containing information panel. These tests now follow actual navigation. Visual inspection also caught footer sibling ordering that could cover Back after rebuilding twice; this was fixed and covered by a rebuild assertion before final verification.

Review browser checks load all 12 views, toggle the original comparison, and check for script errors and horizontal overflow. No Android build, physical-device test, performance profiling, live-service verification or full-project suite was run.

## Scope

Ten existing Assets files changed and one PNG plus its meta were added, with no deletions. Exact paths are recorded in `QA/ScopeAndVerification.json`. All pre-existing art, fonts and meta files, other page prefabs, and the earlier Event Detail work retain their starting hashes. `ProductionPlayerHeader` only gains the new typed adapter; its existing page adapters and presentation are unchanged.

See `QA/ReferenceFidelityReview.md` for remaining differences. Standings and prizes remain disclosed examples, not implemented online rankings.
