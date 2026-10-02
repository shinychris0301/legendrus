# Characters — Original Lettering R2

User requested extraction of titles and buttons together with their text. Implemented source Russian artwork in the active Characters prefab, preserving English localization and existing interaction rules.

## Included
- Main ПЕРСОНАЖИ title and complete УЛУЧШИТЬ / ВЫБРАТЬ button artwork, including original lettering, gold relief and colored faces. Original source RGB with prior accepted extraction alpha.
- ВСЕ / РЕДКИЕ / ЛЕГЕНДАРНЫЕ tabs: source faces and lettering with traced outer silhouettes. Alternate selection faces are bounded navy/blue color variants; warm gold lettering is preserved.
- КАРТА МИРА / МАГАЗИН / СОБЫТИЯ / ГИЛЬДИЯ footer captions. Map uses source RGB with accepted alpha; the other source text faces use the existing shared caption frame to avoid adjacent icon/background remnants. Existing medallions remain unchanged.
- Russian live labels are hidden where source lettering supplies the caption. English restores original cleared faces and live labels. Character names, preview information, counters, stat headings and other changing content remain live.
- Upgrade and Select remain disabled/dimmed. Original ВЫБРАТЬ wording does not enable ownership or hero selection; current Ivan is still identified by his exclusive card highlight/status.

Runtime/view, builder, active prefab and cutover agree on revision14. Header, routes, portraits, scenery, collection scrolling and preview semantics preserved.

## Extraction and scope
`Sources/Extract.cjs` records the reproducible extraction, source bounds and state variants. `Sources/Reference.png` is copied from the existing approved Characters original. No image generation, Scenario jobs or credit use. Thirteen new PNG/meta pairs; seven existing Assets files modified, no deletions. Exact paths: `QA/ScopeAndVerification.json`. Existing artwork/meta/fonts/other-page prefabs unchanged (5,675 protected files).

Inspected transparency over black, white and magenta. Source-pixel assertions verify original RGB for title/buttons/default tabs/map caption, original text faces for remaining footer captions, accepted title/action alpha, and unchanged warm lettering in alternate tab states.

## Verification
- Prior R1 Verify02 supplied before captures and passed10tests. No redundant new baseline run.
- Verify01: import/compile passed; one Edit Mode expectation still required the old blank footer-caption sprite. Updated that expectation to the new Russian variants; remaining2Edit tests passed. Play Mode was not run after that failure.
- Final Verify02: Unity6000.5.9f1 rebuild/readback,3Edit+8Play=**11tests passed**, no failures/skips.
- Checks include RU/EN sprite/live-label switching, every tab selection state, disabled actions, saved balances, exclusive Ivan highlight, scroll reveal, preview safety, press cancellation, modal blocking and routes.
- Nine actual Unity captures; inspected default RU, Rare RU and tall EN. Chrome checked all11review views at430px without JS errors, decode failures or horizontal overflow.
- Source-pixel/integrity checks and scoped whitespace passed. Generated YAML whitespace normalization after testing did not change serialized values.

No Android build, physical device test, backend/ownership integration, full suite or performance run.

## Remaining differences
The main Russian title and action letters now match source pixels. The page as a whole is not pixel-identical: English/live detail typography, reconstructed scenery, repaired surfaces and centered tall-screen layout remain. Alternate tab selection colors are derived states absent from the reference; three footer caption frames deliberately reuse the approved shared shape. These are documented in `QA/ReferenceFidelityReview.md`.
