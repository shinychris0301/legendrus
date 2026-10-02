# Shop R1 reference comparison

Source: `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Shop.png` (941×1672). Current evidence: actual Unity captures in `UnityVerification/Shop`.

The four-card composition, original product silhouettes/colors, Russian title artwork, dark wood/navy family, antique-gold boundaries and emerald purchase-control layout are represented in native UI. The main product/card positions use source coordinates. This is not an exact-pixel replica.

| Area | Implemented / remaining difference |
|---|---|
| Title | Original plaque/lettering, traced silhouette; tiny edge contours still approximate. English uses the existing live serif fallback. |
| Products | Original source RGB plus Scenario alpha. Fine edges/shadows and uniform fitting can differ slightly from the reference. |
| Card faces | Scenario rebuilt empty parchment and frame; engraving/corner details differ. |
| HUD | Live resources/player level and approved Home portrait/icon family; portrait crop, outer frame and resource capsules differ from the Shop reference. No fabricated experience fill. |
| Prices/contents | Intentional unavailable/coming-soon state. Reference prices, quantities and discount/hit badges are not approved catalog data. |
| Categories | Offers shows all four illustrative designs. Coins/Bundles filter the preview; Gems/Boosters/VIP show honest empty states. Initial Offers selection differs from the reference's highlighted Coins tab. |
| Footer | Original isolated medallion faces and separate live caption plaques; notification overlaps repaired, no fictitious unread counts. Additional real Back/Special Offer controls preserve navigation. |
| Materials/type | Dark wood/navy generated material and Cyrillic serif fallback approximate the reference. Price-face/amount-face blanking is repaired material, not original untouched pixels. |
| Tall phones | Uniform composition scale within safe area; clean wood covers the full display. Larger top/bottom breathing room preserves proportions. |

Visual inspection covered actual941RU,941EN,576safeRU/EN and empty-category view; Coins and1080safe captures additionally have automated bounds/text/input checks. Earlier visual defects (portrait square, clipped title, rectangular price repairs, original coin remnants, stretched empty panel) were corrected before final review.

No reference-fidelity percentage is claimed. Physical phone verification remains pending.
