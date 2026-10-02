# 2026-09-24 — detailed reference recheck

**Conclusion: recognizable and closer, but not an exact match.** Compared the archived Friends and Requests originals against the final Verify04 native Russian captures at 941×1672. Inspected current runtime geometry and revision2 active prefab/cutover. No production changes or new generation were made for this review.

| Area | Observed result | Priority |
|---|---|---|
| Main title / roster portraits | Original Russian title and same source portrait subjects/level badges are present. Portraits are slightly shorter within compressed rows. | Mostly retained |
| Energy / Visit / footer lettering | Original Russian lettering, icon subjects and button faces are retained. No obvious text-over-decoration overflow in the inspected roster capture. This does not prove exact alpha contours. | Mostly retained |
| Joined tabs | Reference tabs form a connected ornate strip with a central blue jewel. Current separate rectangular frames omit connecting ornament; type is heavier. Request badge is absent. | High |
| Outer list frame | Reference has larger curled gold corners, decorative rail junctions and a sapphire rail detail on the right. Current borrowed surround is simpler, with different top/bottom treatment. | High |
| Guild flags | Reference uses hanging blue/red/green/purple banners. Current smaller shield crests have different silhouettes, colors and symbols. | High |
| List geometry | Reference first roster portrait/row is around y447; current row starts y490 (43px lower). Reference row pitch is about164px; current is154px. Added search occupies the missing space, so later rows progressively converge. | Layout tradeoff |
| Typography | Native names, guilds, statuses and tabs are visibly bolder and wider than the original. Original source font identity is still unverified. | Medium |
| Row surfaces | Navy family is close. Repeated donor texture, mirrored corners and continued rails differ from original material/ornament. | Medium |
| Lower controls | Sort/remove controls lack the original exact contour/icon/lettering arrangement. Current sample count differs deliberately from reference capacity6/50. | Medium |
| Scenery | Shared Home plate shows a cooler starry scene; roster original has a warm sunset village. Building/tree placement differs. | Large visual difference; shared-art choice |
| Requests view | Original has a wider title, wider ornate cards and substantially larger portraits. Current uses roster title/frame/navigation for consistency. Check/cross and gift subjects are retained, but spacing and scale differ. | Separate refinement |
| Requests order | Stored Requests capture is alphabetically sorted after a test invokes Sort. Its different order is a working sort state, not evidence of wrong identity mapping. | Expected state |
| Header / added UI | Actual shared Home header, saved balances, sample notices, functional search, Back and local Find tab are intentional. Reference roster says Invite. Request reference uses shop-category footer icons; current keeps page navigation across tabs. | Preserve behavior unless explicitly redesigned |

Recommended next: **Friends List R2 — Joined Tabs, Guild Flags and Outer Frame**. Restore those source decorations while preserving search, shared header, original-lettered action buttons and native interactions. Then assess typography/lower controls and Requests-specific geometry separately. Do not copy reference live counts, presence or request notifications as real data.

Prior verification remains **26 targeted Unity tests and15 browser views passed**. These are historical layout/behavior results, not proof of exact visual equivalence. No new Unity run or Scenario call was needed for this visual assessment.

---

# Friends List — reference fidelity, rebuild R1

The page now uses the shared Home player header, with live saved coins, gems and energy. The original Russian title and portraits are retained. Energy, Visit and all five footer controls now retain the original Russian lettering with their artwork. English remains localized using live labels and the existing blank faces.

Source-derived gold/navy row surfaces replace the earlier stretched textured frames. The portraits and rows have more room, and original footer aspect ratios replace uniform sizing. The header stays at the safe top edge and footer controls at the safe bottom edge on tall phones. The clean Home environment replaces the very dark forest; it belongs to the approved art family but is not this reference's exact village.

Remaining differences:

- The reference starts the list immediately beneath the tabs. The implementation retains a functional search field and explicit sample disclosure, so the first row starts lower and rows are slightly shorter.
- Names, guilds and statuses use approved live fonts. Exact source font identity is not verified.
- Guild emblems are approved shared assets, rather than the reference's exact colored hanging flags.
- The row's hidden surface and left/top rail continuation are reconstructed from source donors. Scenario removed some thin rails, which were restored with a local silhouette mask. Fine contours and donor texture are approximate.
- The outer list surround, tab frames, search frame, sort/remove controls and carved footer use approved shared material; their ornamental details differ from the original.
- Back remains beside the title. The third tab is the existing local Find view; the roster illustration labels it Invite.
- Friends, requests, levels, presence and the Events exclamation badge are illustrative examples. Counts report examples rather than an invented online capacity. Social buttons explain that the service is not connected; they send nothing and grant no rewards.
- Request gifts/check/cross art and request portraits retain the accepted earlier extraction and its contour limitations.

This is a native Unity page, not a screenshot with hotspots. Full pixel identity, real online friends and physical-phone usability are not claimed. Recommended next visual refinement: original joined tabs, guild flags and outer list ornaments.

Final Verify04 passed all26targeted Unity checks;15browserviews/comparison passed. These checks establish tested layout/behavior, not full visual identity.
