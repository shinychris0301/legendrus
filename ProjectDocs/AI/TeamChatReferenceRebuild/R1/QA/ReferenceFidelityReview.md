# Team Chat R1 — detailed reference recheck, 2026-09-23

## Verdict

The page follows the original composition and reuses several original assets, but does **not** match it exactly. This is a fresh visual comparison of the archived original and the latest stored Verify04 Russian Unity capture, both 941×1672, plus inspection of current runtime geometry and revision-2 cutover. No Unity rerun or production changes were made for this assessment.

## Close matches

Original Russian title/plaque, six character portrait subjects, chest artwork, composer face, and Russian Send/Gifts/Help Requests/Stickers lettering are retained. The footer button positions and source aspect ratios are close. The Home shared header supplies the expected portrait and three resource groups. These are source-derived assets with estimated alpha, not a claim of pixel-identical edges.

## Remaining differences, in priority order

| Priority | Area | Observed discrepancy | Recommended correction |
|---|---|---|---|
| High | Chest enclosure | Original is a taller dark navy enclosure joined to the page frame; current separate textured panel begins lower. Guild crest is smaller and different. Chest sits lower; original information medallion is absent. | Restore the source enclosure/crest geometry and information ornament, preserving sample labelling and native controls. |
| High | Chat surround and system message | Source uses one continuous dark inset with carved side rails. Current separate table-derived rectangle has a brighter repeated texture and different corners. System crest/frame and heading position differ substantially. | Recover source rails and system-row frame, preserving an intentionally opaque dark interior and scroll mask. |
| High | Message spacing and parchment | Current portraits are vertically smaller in several rows. Later messages sit higher, leaving excess space beneath the last row. Parchment has conspicuous smooth repaired areas instead of source grain. | Align row/portrait/bubble geometry to reference landmarks; refine only the repaired parchment areas. |
| High | Missing message artwork | Large blue energy medallion, small gold lightning, crossed swords and flexed-arm illustration are absent. | Reuse approved matching icons first; isolate missing source artwork only where necessary. |
| Medium | Typography and Help buttons | Player names, roles, counts and messages use a heavier/wider serif. Line breaks differ. Help buttons are shorter with different proportions/lettering than the original. | Fit the approved fonts to source geometry and extract original fixed Russian Help-button lettering with its frame; retain live English. Exact source font remains unidentified. |
| Medium | Footer and outer scenery | Source has continuous dark carving, corner rails and a central blue jewel. Current reused carved texture has different motifs; the central source jewel is absent. Tree scenery differs from the source surround. | Restore the page-specific footer/outer ornaments while retaining shared navigation and a clean environment plate. |

## Intentional behavior differences

Saved account values and unavailable XP differ from the illustrated values. Sample chat/role/chest labels and the local-session notice are deliberate. Back replaces the original chat symbol. Send is darker in the empty-input capture because it is disabled; this is a state difference, not missing original lettering. Tall displays expand the history viewport between safe-edge controls. Chat is local-only; presence dots and Help badge are fixed examples, not live services.

## Recommendation

Proceed with **Team Chat R2 — Chest and Message Frames**: correct the chest enclosure/crest, continuous chat surround and system row, then align message/portrait spacing. Preserve the accepted shared header, source-lettered footer controls and local-only behavior. Follow with missing message icons and bounded parchment/typography cleanup.

The earlier 28 passing Unity tests establish tested behavior/layout constraints; they do not establish visual identity. No new Scenario calls, image edits, Unity tests or Android/device checks were performed during this recheck.
