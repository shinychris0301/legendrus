# Visual Page 28 — Settings reference rebuild R1

Settings now uses the real shared Home player header through the approved Shop header instance. Its live portrait/name/level and coin, gem and energy presentation are no longer independently reconstructed controls. Page routing remains bound by the Settings view.

The header follows the safe top edge and Back/version follow the safe bottom edge. The central panel composition retains its original aspect ratio. The outer carving uses nine-slice borders to extend its side rails while retaining the corner and bottom-arch proportions, keeping the arch aligned with Back on tall screens. Only its sprite-border metadata changes; the PNG and GUID remain untouched. Overlay shielding covers the full safe area on tall displays. The clean approved Home environment replaces the older heavily darkened forest background.

Preference labels and lower-row labels were reduced to give the source toggles, values and arrows breathing room. Existing approved Mail dividers replace plain preference separator lines. Sign Out gains a truthful unavailable subtitle in the reference's second-line area. All local buttons, including the language modal and cloned header, use the existing map press-feedback component. No new sounds, haptics or separate navigation system were introduced.

## Art and scope

The previous Scenario-isolated Settings title, icons, toggles, parchment, rows, outer carving, Back and language modal are reused. No extraction or generation was needed: **0 new Scenario calls, 0 CU**, no PNG/font edits. Existing source alpha and reconstructed surfaces retain their earlier limitations. This pass does not claim exact font identity or whole-image parity.

The owning builder rebuilds the actual Settings prefab; runtime/view/layout and revision3 cutover state agree. The shared header component/source prefab and other pages are not modified. Existing saved music/SFX/vibration/notification toggles, session language/modal behavior, account navigation and unavailable-service explanations remain intact. The settings notification toggle does not implement OS notification delivery. Sign Out does not delete saves or simulate account authentication.

## Verification evidence

Baseline01 ran the existing Settings tests before edits. Verify01 rebuilt successfully, but its legacy strict Rect.Contains safe-area assertion rejected the shared Profile control touching the safe-area boundary. The assertion now uses inclusive bounds with the same one-rendered-pixel tolerance as other rebuilt pages. Verify02 passed 13 tests and adds standalone Language Select regressions because its runtime reuses Settings modal resources. Visual inspection of Verify02 caught the arch remaining behind after Back moved downward; the nine-slice correction is verified in Verify03. Final results and exact changed paths are recorded in QA/ScopeAndVerification.json; resource hashes in QA/IntegrityChecks.json; browser results in QA/BrowserChecks.json.

Native capture tests cover live values, RU/EN layouts and safe areas, saved toggles, repeated binding, modal blocking, language selection, route commitment, returning to the originating page, and preserving the paused board/hash/moves/score. New checks verify the single shared header, press feedback, unchanged button geometry/preferences, header/footer safe-edge placement and label/value/arrow clearance.

Final **Verify03 passed 4 Edit Mode + 9 Play Mode = 13 tests**, zero failures/skips, covering Settings and standalone Language Select. The review has 15 actual Settings captures plus original/baseline views; all 17 browser views loaded at 430px without errors or overflow. Baseline passed the original 7 Settings checks. Language Select regression captures are retained in QA/LanguageSelectCaptures.

Eight existing Assets files changed: runtime, view, layout, builder, active prefab, cutover, Play Mode fixture and frame sprite-border metadata. None added/deleted. Hash checks preserve 5,614 protected existing art/font/meta/other-page prefab files. The single excluded frame meta changes only spriteBorder; its GUID and all other fields are verified unchanged. Source/cutover/meta whitespace checks pass; generated prefab YAML excluded.

No Android build, physical-device test, full suite, backend integration or performance test was run.

## Next action

Review the complete Settings page and its language modal. The most visible remaining fidelity differences are the locally repaired row textures and the heavier live type. Targeted surface/typography work can follow this review; Visual Page 29 — Language Select is next in sequence.
