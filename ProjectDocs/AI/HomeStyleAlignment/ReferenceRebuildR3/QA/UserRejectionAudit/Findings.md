# Home reference mismatch — user rejection confirmed

The current revision 6 Home page does not match the original reference. Direct comparison by the root and an independent reviewer found substantial differences in composition, scale, silhouettes, materials and typography. This goes beyond minor finishing differences. The prior functional and containment checks do not establish visual fidelity.

Compared the complete original `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Home.png` with the existing actual Unity `UnityVerification/Home/Home_941x1672_RU.png` retained under ReferenceRebuildR3. Both are 941×1672. All 52 production files still match the delivered source manifest, and the registry/owner/prefab point to Home revision 6. The retained capture is therefore supported by unchanged delivery files; no new Unity render was taken in this review. The issue exists in the delivered Unity result and cannot be attributed solely to viewing the old R1 HTML.

## Confirmed discrepancies

| Area | Original reference | Current Unity Home |
| --- | --- | --- |
| Environment / lighting | Navy-violet twilight, close timber towers, warm windows and amber village lighting | Bright blue/pale-gold sky, extensive white snow and distant white castle; different overall contrast and palette |
| Ivan | Smaller figure with more visible torso and surroundings | Head and torso too large; face/hair, cloak fastening and embroidery differ; waist is much lower |
| Firebird | Smaller shoulder bird, finer orange-red strands and narrower flowing tail | Larger body, long curled crest and broad yellow-orange tail dominate the right side and lower composition |
| Crest | Taller upper and lower lettering, specific black plaque and tightly layered gold carving | “ЛЕГЕНДЫ” and “ТРИ В РЯД” are compressed into thin bands; different plaque, scrolls and sapphire; crest sits lower |
| Top HUD | Textured navy/wood, brighter layered gold rims, pale plus signs, blue experience fill | Flatter black band, different bronze rims/gear/portrait, gold plus signs and empty experience meter |
| Play | Curved ornate ends, fine brighter filigree, different emerald surface and substantial beveled lettering | More rectangular bronze frame with large side diamonds; main body sits higher; thin flat live lettering |
| Navigation | Thick layered clipped-corner frames, specific shop/angled scroll/helmet, tall luminous labels | Thin rectangular borders and different illustration proportions, angle and metal treatment; event marker absent |
| Footer | Bright cyan measured bar and reward value, specific mail/chest carving, ornamental border reaching bottom | Empty bar/dashes, different icon/frame proportions and dark bronze carving; plain dark strip below the footer |

Approximate visual anchors, using top-left origin on the 941×1672 images: Ivan's head spans roughly x300–500 in the reference versus x240–520 currently; the reference belt is near y1000 versus around y1210 currently. Crest apex is near y115 in the reference versus y150 currently. These are visual estimates, not automatic measurements. Serialized current rectangles are exact implementation values: hero (50,446,891,1188), crest (210.5,145,520,390), Play (179,980,585,240). Matching these current rectangles in a structural test proves implementation consistency, not agreement with the painted reference landmarks.

## Why the previous result diverged

The generation records describe fresh independent text-only reconstruction. Separate outputs changed subject proportions, ornament shapes, lettering hierarchy and exposure. Their matching filenames, hashes, alpha and import settings verify that the chosen assets are installed; they do not verify that the chosen artwork resembles the reference closely enough. The generated character/Firebird pair was then placed as a large combined sprite, preserving its incorrect internal proportions. The enlarged caption plaques solved legibility but also simplified the reference's frame design.

The background was deliberately preserved under the earlier foreground-only scope. That historical constraint explains its presence; it does not remove the visible mismatch. A complete dusk-scene match cannot be claimed while the bright winter plate remains. Its preservation constraint remains in force during this inspection; no background changes were made.

Different numeric balances alone are not art defects: resource values must reflect actual state. The missing blue-gem quest/experience measurements are an existing service gap. Their reference values/fills must not be fabricated to disguise that gap. Exact source-font identity also remains unresolved.

## Next focused correction

Establish an explicit reference landmark and material specification for the complete portrait before another replacement pass. Foreground priority: reduce Ivan/Firebird to the reference scale, restore the crest's lettering hierarchy and vertical placement, then correct Play/navigation/footer silhouettes and material contrast against that same specification. Asset isolation and functional tests remain necessary checks; visual correspondence needs its own acceptance criteria. The preserved-background constraint and unavailable quest state must remain explicit until separately resolved.

## Checks and changes in this review

Read instructions/current task/handoff/provenance; inspected working changes, active registry/runtime/owner/prefab; viewed both complete images directly; inspected generation provenance and the existing test scope; checked 52 source hashes plus reference/capture hashes and metadata. Independent visual review corroborated the findings. Details and hashes are in `Verification.json`.

Only review documentation and task/history status were changed. No production/asset changes, generation, image cropping, pixel manipulation, fresh Unity run, compilation, tests, APK/AAB, browser execution or phone testing occurred. Earlier 30/30 Edit Mode and 21/21 Play Mode results remain historical functional evidence. R3 is now explicitly visually rejected, with correction required; no approval is inferred.
