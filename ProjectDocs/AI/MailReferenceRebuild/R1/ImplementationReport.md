# Visual Page 25 — Mail / Inbox Reference Rebuild R1

Mail's earlier implementation used the old header, dark forest scenery, shared generic row frames and recreated Claim captions. This pass updates the actual native page using the two originals (`mail box.png` and `mail open.png`) archived in Sources.

## Implemented

- Actual shared `ProductionPlayerHeader` component cloned from the approved Shop header, using Home presentation and live account values. Replaced the fragile individual Shop-child cloning.
- Home clean environment and approved carved footer reused. Header anchors to the safe top; footer controls anchor to the safe bottom. Parchment and attachment placement adapt to taller displays; the modal shield covers the safe content area.
- Source message rails/corners at original-like row positions and pitch. Occupied interiors reconstructed from approved navy material; sender intersections in the rails use neighboring source metal. Portraits and arrows reused unchanged.
- Original Russian Claim and Claim All artwork includes the lettering. English uses live text on blank faces. Claim reuses its accepted alpha; Claim All has a new bounded mask. Rejected local inpainting attempts left patches, so the English Claim All face instead uses approved Home green material within a feathered interior mask.
- Approved gameplay sapphire and existing shared booster mapping used for attachments. Coins, book and sender artwork reused. Attachment examples now have visible framed slots.
- Example labels, sample unread state, tab filtering, all five letters and layered Back behavior remain. Sending, deleting and claiming still explain that the mail service is unavailable; no rewards or messages are issued.

Four versioned PNGs plus metadata added. Runtime/view/layout, owning builder, active Mail prefab and cutover now agree on revision 2. Seven existing Assets files changed, none deleted; all earlier PNGs, font files, metadata, Home/Friends/other-page prefabs are unchanged by SHA. No shared header code changes were needed.

## Scenario and source handling

One Scenario `model_pixa-background-removal` request for a sheet containing the prepared original row and Claim All button: **3 CU**, no resubmissions or RGB generation. Original visible RGB is retained under returned alpha. The footer allowed contour removes neighboring rails/backdrop, and intentional inner surfaces remain opaque. Four sprites were inspected over black, white and pink. Input sheet, source bounds, job/billing, repair mask and output hashes are archived. Reconstructed faces, rail continuation and fine alpha are approximate; this is not claimed as pixel-exact extraction of hidden material.

## Verification

Final Verify02: Unity 6000.5.9f1 compilation/rebuild/readback, **4 Edit Mode + 25 Play Mode = 29 passed**, no failures/skips. Includes Mail, Friends List and Home regression checks. Mail coverage includes tabs, empty Sent, every example letter, layered Back, guarded claims/send/delete, save invariance, routing/rebinding, maximum balances, languages, layout and pointer hit tests.

Initial Verify01 passed import/Edit Mode but four Play Mode captures stopped at strict `Rect.Contains` checks on the new shared Profile boundary. The capture assertion now permits one pixel of rounding and reports both rectangles; final responsive layout and all checks pass. Earlier results are retained. Sixteen actual Unity Mail captures retained; default RU and tall EN guild letter visually inspected. Nineteen browser views (16 native, two originals, one earlier implementation) load without script errors or horizontal overflow. Scoped C# whitespace passed.

No Android build, physical phone, full project suite, performance or live-mail-backend verification. Existing font weight, joined-tab ornamentation, parchment material, preview hierarchy/name plaque and full letter typography still differ; see QA/ReferenceFidelityReview.md.

Recommended next pass: original joined tab treatment and parchment preview hierarchy, including portrait/name plaque and letter typography.
