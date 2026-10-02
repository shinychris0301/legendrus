# Friends List R4 — Requests cards and portraits

The Requests tab now uses its original reference's wide cards and larger portraits. Requests has a dedicated source-derived navy card frame and carved outer enclosure with its bottom sapphire. Portraits, gift boxes and Accept/Decline art are reused unchanged. Live names, request descriptions and action hit areas follow the wider layout; the Friends and Find rows retain their existing layout.

The shared header, search, tabs, source title, lower controls and footer routes are retained. Social data/actions remain labelled examples; Accept and Decline show the existing explanation without changing saves. This pass does not add a live friends service.

## Assets and provenance

One Scenario `model_pixa-background-removal` request for the two-frame sheet. Original RGB is retained after local preparation and alpha cleanup. Obscured left trim is mirrored from the original right side; interrupted rails use source donors. The navy interior is reconstructed from a clean original patch. A preparation error that covered the upper bottom-jewel area was repaired from the source after submission, without another request. Both submitted and refined inputs are preserved.

Two new sprites: `request_card_r4.png` and `requests_surround_r4.png`, plus Unity metas. Existing portraits, gifts, actions, fonts and approved page artwork remain unchanged. Reconstructed hidden surfaces and alpha contours are approximate. Black/white/pink inspection is provided in QA.

## Native implementation

The runtime builds Requests with 886 px wide rows (previously 747), 186 × 186 portrait rectangles (previously 157 × 180), names at x242 and gift/action columns at x538/640/750 within each row. Five rows use a 178 px pitch. Portrait aspect ratios are preserved; the source portraits' ornamental tips extend beyond the flat row rectangle. The dedicated enclosure expands with taller safe areas. Tab switching restores the Friends/Find geometry.

Owning runtime, view, layout, builder, actual prefab and cutover use revision 5. Tests cover geometry switching, item separation, localized phone layouts, actions, routes and saved data. See the QA results for actual completed checks. No Android/device/keyboard, performance, full-suite or live-backend verification is claimed.

## Final verification

Unity 6000.5.9f1, final Verify02: native prefab rebuild/readback succeeded; **10 Edit Mode + 16 Play Mode = 26 tests passed**, no failures/skips. Includes Friends List, Team Hub and Team Chat with isolated saves. Thirteen Friends List native captures are archived. Default-order Russian Requests and narrow English Requests were visually inspected. **16 review views** and the comparison toggle passed with no script errors or horizontal overflow.

Verify01 also passed, but visual inspection showed the surround sapphire vertically compressed/partly covered. Corrected the source-density slicing and reduced Requests row pitch from 184 to 178 px, retaining larger 186 px portraits. Final Verify02 confirms the corrected layout. No additional Scenario call was required.

Three Friends roster regression captures (reference RU/EN and narrow EN) are pixel-identical to R3. Asset hash checks confirm ten portraits/gifts/action sprites were reused unchanged; existing art, metas, fonts and other page prefabs are preserved. Two PNG/two metas added; seven existing Friends List files modified; no deletion. Scoped C# whitespace checks passed. Scenario used **one successful request / 3 CU / no resubmissions**.
