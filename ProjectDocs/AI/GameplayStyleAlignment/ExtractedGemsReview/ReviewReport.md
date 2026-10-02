# Extracted gem color review

The large gray-background sheet posted by the user is closer to the intended reference than R7 in its framed faces, central emblems, glass interiors and embossed gold coin. Its stronger appearance involves bright specular edges and local contrast as well as saturation. A blanket desaturation or darkening pass would lose useful jewel depth and color distinction.

## Two different sources were checked

1. **Large gray-background conversation attachment:** seven prominent jewel illustrations with strong reflections and a broad rainbow glow. Reviewed visually; no verified local file identity or numeric color comparison is claimed for this sheet. Some proportions and details differ from the original board pieces.
2. **Local native extraction package:** found at `/home/k2/Downloads/LegendsOfRus_Extracted_Elements/LegendsOfRus_SourceMatched_Elements`. Its README explicitly distinguishes native source extracts from a subsequently generated combined sheet. The seven canonical PNGs are only 77–85 pixels wide, with estimated alpha, and come from the original 941×1672 board reference.

Independently compared **every retained RGB pixel** in those seven native canonical PNGs against its declared rectangle in `Reference/original_gameplay.png`: all match, with **zero changed RGB pixels and zero channel difference**. The reference hash also matches the package manifest. Consequently, those native files have not gained saturation through an RGB recoloring step. Background, display size and estimated edge alpha can still affect perceived appearance. This pixel result must not be attributed to the larger conversation sheet.

The local original was directly viewed and matches the latest night-village reference shown earlier in the conversation. An unchanged [review-only copy](Gameplay_Reference_Source.png) and [provenance](ReferenceProvenance.json) are now saved here. Earlier “no verified local copy” statements are superseded by this discovery. [Exact native RGB comparisons](SourceColorCheck.json).

## Suggested treatment for the large-sheet artwork

| Piece | Assessment | Suggested correction |
|---|---|---|
| Red | Deep ruby core and integrated fleur-de-lis are appropriate. Bright edge streaks are more assertive. | Preserve the dark red core; soften the strongest reflections without flattening the glass. |
| Blue | Broad cyan/white reflections and bright outlines create the strongest visual intensity. | Reduce highlight intensity/coverage first; keep blue/cyan facets distinct and readable. |
| Green | Warm leaf-green is closer to the reference than R7's cooler green. | Preserve that hue direction. Check bright yellow-green facets against the actual board before reducing saturation. |
| Gold | Embossed coin form is more faithful than R7's yellow cut-glass stone. | Keep the coin design and antique-gold shadows; restrain hot reflections rather than turning it gray. |
| Purple | Good violet identity, but sharp white streaks and vivid edges increase contrast. | Soften those reflections while keeping a dark violet core. |
| Orange | Sun emblem and amber/orange material suit the reference. | Preserve separation from the gold coin; reduce only excessive bright orange highlights if the board-size comparison warrants it. |
| Rainbow special | The separate special identity is useful; the broad rays are visually dominant. | Keep stronger emphasis for a special, but use a tighter resting halo and reserve the broad flare for activation. No new mechanic is implied. |

The large sheet also has somewhat taller/narrower pieces than the source board, particularly green, purple and blue. Judge them at actual cell size with preserved aspect ratio. Evaluate six normal pieces repeated across the dark 8×7 board before choosing a global color adjustment. No measured desaturation percentage has been established.

The native extracts can now serve as exact color/shape references. They are screenshot-derived samples with estimated masks, not recovered high-resolution authoring layers. RGB equality does not certify clean alpha or suitability for direct production replacement. No extracted sprites were imported, no reference crop became runtime UI, and R7 remains installed.

## Checks and scope

Performed: read current task/version, inspected local working-tree state, verified all **1,691 R7 production hashes** and **63 existing evidence hashes**, viewed the posted sheet, R7 asset proof and actual Unity portrait, local original reference and canonical-gem preview. Read the extraction README/manifest/integrity report and independently checked all seven canonical RGB mappings and alpha ranges. An independent visual reviewer compared the posted sheet, reference and R7. Rechecked production/evidence hashes after documentation updates.

Changed only documentation: this report, source-color results, [preservation results](PreservationCheck.json), reference provenance and an unchanged reference copy, plus prepended CurrentTask/SessionLog entries. No production edits, image generation/color editing, new Unity compilation/tests/captures, browser viewer, APK/AAB, physical-phone testing, calibrated color measurement or full transparency acceptance was performed. Previous Unity results are historical, not rerun here.

Next recommendation: use the verified native reference colors to assess the larger artwork at board size; adjust highlights selectively before deciding whether its midtones need modest desaturation. Keep the six color identities and the gold coin design. Footer/header work remains pending while this gem feedback is reviewed.
