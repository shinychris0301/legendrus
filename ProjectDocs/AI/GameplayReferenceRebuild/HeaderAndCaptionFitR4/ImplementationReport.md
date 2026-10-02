# Gameplay R4 — header, caption clearance and board colors

The header now reads as one connected panel. Booster lettering clears the lower gold decoration, and the cell rails and joints use a consistent bronze-gold palette sampled from the original reference.

## Findings and changes

- **Separated header:** the three source sprites had independent edge masks that interrupted the shared rails and exposed rectangular dark edges. `JoinedHeaderBacking` now combines their existing blank faces with the original top/bottom rail and divider pixels. The three old panel objects remain as non-rendering layout anchors. Live headings, score, moves, missions, stars and controls remain separate. The repaired outer alpha was inspected over black, white and magenta. Intentional dark recesses remain; exterior extraction padding is clipped.
- **Booster ornaments:** the previous caption rectangle extended to card-local y264, overlapping the filigree beginning near y257. The new well is x17–175, y218–255. Captions use 17px bold live type with bounded line spacing. `ProductionGameplayCaptionFit` optically centers the final glyph mesh, including the shadow, and only reduces scale if necessary. This handles Cyrillic descenders without clipping text or hiding decorations. Existing names, art, stock badges and functions remain.
- **Board color discrepancy:** the extracted outer frame has **zero visible RGB differences** from the original source crop. The native cell rails and joints used different, paler palettes. Both now share source-sampled highlight `(155,104,45)`, face `(101,55,10)` and shadow `(28,17,3)`. The outer frame PNG, board geometry and six gems are unchanged. Native metal geometry remains an approximation of the source's textured metal.

## Assets and implementation

One new `GameplayHeaderR4/header.png` reuses prior Scenario-extracted faces and source rail pixels. No additional Scenario requests, credits, generated art or baked text. See [asset provenance](Sources/AssetProvenance.json), [board palette analysis](Sources/BoardPaletteAnalysis.json), and [reproducible assembly](Sources/PrepareAssets.py).

The active Gameplay prefab and owning builder agree at **revision15**. Seven existing Assets files modified, five files added, zero deleted. All397 previous prefab records remain;405 records total. No other page prefab changed. Existing PNGs, fonts and metadata retain their snapshot hashes. Exact file audit: [ScopeAndVerification.json](QA/ScopeAndVerification.json).

## Verification

- Unity6000.5.9f1 Build05 compiled, rebuilt and read back the canonical prefab successfully.
- Final Verify04: **9 Edit Mode +31 Play Mode =40 passed**, zero failed/skipped. Includes retained bindings/input, gameplay/booster/VFX behavior, goal and large-number containment, board-joint visibility at438px, native RU/EN captures and tall simulated safe areas.
- New caption checks measure actual rendered mesh bounds against the wooden face, including shadow, rather than only the full card bounds.
- Inspected actual Unity941px Russian/English,438px gameplay and576px tall three-mission captures; header continuity and lower caption clearance are visible. Test-injected large numbers/specials are labelled fixtures, not earned gameplay state.
- Initial diagnostic runs caught the reused moves-anchor width and Cyrillic glyph overhang; both corrected. Earlier logs are archived separately from final verification.
- Review page:14 image options load at390px without JavaScript errors or horizontal overflow; see [BrowserChecks.json](QA/BrowserChecks.json).

No Android build, physical-device check or full-project suite ran. The existing shared typography asset still has unassigned RegularFont/StartFont and uses the desktop Liberation Serif development fallback in these captures. Exact source-font identity and Android font parity remain unresolved; no OS font copied. This revision repairs the reported issues, not a claim of whole-screen pixel identity.

Next: user visual review of the standalone native capture page.
