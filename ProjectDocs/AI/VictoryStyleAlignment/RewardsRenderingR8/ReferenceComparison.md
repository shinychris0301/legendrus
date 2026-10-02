# Victory R8 compared with the original reference

Reviewed the original `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Victory.png`, actual R7 RU portrait and actual R8 final RU 941 three-reward / EN 576 five-reward portraits.

| Reported issue | Confirmed cause | R8 result |
| --- | --- | --- |
| Reward board expands in all directions | Nonuniform scaling of the outer panel and tall card sprites distorts carvings and jewels. | Protected decorative geometry, shorter outer panel and smaller, naturally proportioned card ornaments. |
| Stars poorly rendered | Smooth face interpolation loses the reference's strong planar gold facets; rays are weak. | Distinct gold planes, stronger warm lip/rays; empty/earned states remain truthful. Fine painted sparkle detail still differs. |
| Reward icons lean left | Aspect fitting in a wide rectangle follows its top-left pivot. | Source-proportioned rectangles centered per card; verified in rendered screen space for dynamic reward counts. |
| Blurry text | Small text carries decorative bevel layers and long shadows; glyphs are not pixel snapped. | Small labels use plain local font rendering, short shadows and snapping. Still needs physical-phone assessment, particularly narrow five-card labels. |

R7 Firebird and unstretched Continue are preserved. Background, ribbon, reward illustration subjects, precise font identity and reference values are not an exact match. Existing independent sprites and live UI remain the implementation; the reference is only a comparison image in the review viewer.

Next focused check: inspect these four changes at phone size, including one-star replay and a five-reward completion. User visual acceptance remains pending.
