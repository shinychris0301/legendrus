# Characters / Hero Collection — Reference Rebuild R1

Continued to Visual Page 36 after Dialogue. Replaced the old individually copied header elements with the actual shared player-header component, and added consistent button press feedback. Prior Characters R1–R12 visual refinements remain installed.

## Implemented
- Builder clones the current Shop `ProductionPlayerHeader`, whose backing, avatar and three resource capsules use the same approved source sprites as Home. It no longer searches for obsolete separate resource/icon/backing objects.
- Characters binds Profile, Settings and resource actions to the existing router. Saved values refresh through the shared component; page font refresh skips header descendants.
- Native buttons use established press tint. Pointer exit restores tint without selecting a hero. Disabled Upgrade/Select retain explicit dimmed faces/captions.
- Runtime/view, builder, active Characters prefab and cutover agree on revision13. Only Ivan remains highlighted as current; browsing other cards is a preview. Filters, locked-card scrolling, modal blocking and routes retained.

## Scope / assets
Zero generation/Scenario calls or credits. Preserved all existing PNG/meta/font files, portraits, reconstructed scenery, carved frame and footer. Seven existing Assets files changed, none added/deleted. Exact paths: `QA/ScopeAndVerification.json`. Reference provenance: `Sources/AssetProvenance.json`.

## Validation
- Baseline01:2Edit+6Play passed.
- Verify01:3Edit+4Play passed;3Play failed because old Rect.Contains checks excluded buttons touching the safe-area maximum. Corrected inclusive bounds with1px numerical tolerance.
- Final Verify02:Unity6000.5.9f1 import/compile/prefab readback passed; **3Edit+7Play=10tests passed**, no failures/skips.
- Coverage: shared artwork/bindings against Home, idempotent build, saved balances, localization/text fit, input targets, filters, Ivan exclusivity, dragging locked cards into view, modal blocking, resource/page routes, disabled actions and callback cleanup. New press-cancel test verifies no hero/progress/balance mutation.
- Nine actual Unity captures; full RU, tall EN and scrolled tall EN inspected against original/prior page. Eleven review views checked in Chrome at430px; no JS errors, image decode failures or horizontal overflow.
- 5,675 protected existing files unchanged. Scoped whitespace passed after generated YAML whitespace normalization; serialized values unchanged.

No Android build, physical device/hardware Back, full project suite, backend/ownership service, performance or fresh-process save test. Existing texture-decompression and unverified-source-font warnings remain outside this focused change.

## Limits / next
Not a pixel-identical recreation: live glyphs, reconstructed scenery and repaired surfaces differ. Preview stats/ownership/upgrades remain unavailable; concept numbers are not fabricated. Existing centered composition fitting is preserved on tall phones. Next recommended refinement: safe-edge header/footer layout, then title/tab typography. See `QA/ReferenceFidelityReview.md`.
