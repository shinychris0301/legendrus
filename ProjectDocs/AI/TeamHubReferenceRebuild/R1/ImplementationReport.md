# Team Hub — Reference Rebuild R1

Continued to Visual Page 21 after Individual Ranking. The actual native Team Hub now uses the shared Home header, restored source navigation cards, revised guild summary/progress and roster layout, approved shared gems/potion, and safe-edge navigation.

## Source and assets

Reference: `/home/k2/Downloads/match3final/ChatGPT Image Aug 25, 2026, 09_00_51 PM (3).png`, the dedicated Guild Hub original. The original title, banner, portraits, chest, scenery and existing approved assets retain their previous files and GUIDs.

Four navigation controls preserve original Russian button faces, icons, lettering and visible engraving. Their exterior alpha is traced, and their source RGB was verified unchanged. English variants retain the original icons/borders and replace only the label area with existing approved lacquer for live localized labels. Both locales retain native buttons and semantic labels; Russian live lettering is disabled to prevent overprinting.

The power panel uses original visible rail material with approved blank lacquer; the chest-occluded bottom-right rail is continued from a visible source segment, and its right cap is mirrored from the left to remove scenery residue. The progress rail has a transparent channel, with baked blue-fill fragments cleared; native background/fill/text render within that channel. Hidden surfaces and traced alpha remain approximations.

Task reward gems directly reference `ProductionGameplayArt.Gem(1)`; the potion directly references `ProductionBoosterArt.Icon(3)`. The second chest reuses the existing Team Hub chest. The footer reuses `EventDetailAligned/footer_r4` with a subdued tint. No new Scenario requests or credits were needed.

## Native implementation

- Added the Team Hub adapter to `ProductionPlayerHeader`, preserving existing action bindings. Coins, gems, energy and level use saved state. Existing shared-header adapters are unchanged.
- Header and footer follow the safe top/bottom edges. The information modal stays centered and its input shield covers the safe viewport.
- Added a dark guild-name backing, corrected power/progress geometry and two-chest placement, and restored original navigation-card widths and positions.
- Restored larger roster portraits and source-relative row spacing. The first sample member no longer uses the unrelated blue personal-ranking highlight.
- Reused embedded Roboto Serif SemiBold for member names and FreeSerif Bold for captions/headings. Exact source font identity remains unverified. No font files were added or copied from the OS.
- Reused native button press feedback. Existing Profile, Settings, Shop, Energy, Chat, Team Ranking, Friends and Back routes remain. Tasks/donation/chest show their existing unavailable explanation without spending or granting rewards.
- Added `ProductionTeamHubReferenceLayout`. The owning runtime and builder migrate the existing prefab objects rather than destroying/recreating the old view and content. Builder, view, actual prefab and cutover use revision 2.

## Verification

Final Verify03: Unity 6000.5.9f1 rebuilt/read back the prefab successfully. **4 Edit Mode + 10 Play Mode tests passed**, zero failed/skipped: Team Hub 2+4, Individual Ranking regression 2+6. Eight native Team Hub captures and ten Ranking captures were produced with isolated saves. Final RU 941×1672 and EN 576×1280 safe-area captures were visually inspected.

Checks cover required bindings, shared header/resource identity, source-button locale switching, full background coverage, top/bottom safe edges, text/glyph fit, row separation, raycasts, routing once, World Map entry, maximum balances, unavailable-action/save invariance, modal blocking/dismissal and repeated builds retaining correct layering. Navigation assets were inspected over black, white and saturated magenta; panel/rail cleanup was inspected separately and in final native captures.

Verify01 import stopped because the existing general typography asset has no regular font assigned; caption binding was corrected to an existing embedded project font. No tests ran during that failed import. Verify02 passed all 14 tests; subsequent visual inspection identified power-edge residue and a progress-fill fragment. Verify03 validates their final correction. Earlier logs remain in QA.

The standalone review loads ten views, including original and previous implementation, and supports original comparison. Browser checks confirm no image-load/script errors or horizontal overflow. No Android build, physical-device test, performance profiling, live guild service or full-project suite was run.

## Scope and limits

Nine existing Assets files changed; ten PNGs with metas and one layout script with meta were added. No deletions. Exact changed files are in `QA/ScopeAndVerification.json`. Existing PNGs, fonts, metas, other page prefabs and prior Event Detail/Individual Ranking edits retain their task-start hashes.

Guild membership, progress, tasks, roster and rewards remain labelled examples. This pass does not implement online guild features or claim pixel-exact matching. See `QA/ReferenceFidelityReview.md` for remaining discrepancies.
