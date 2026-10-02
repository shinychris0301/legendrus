# Individual Ranking — Original Medals R5

User requested direct extraction of the original1st,2nd and3rd place icons. The previous icons had reconstructed blank faces and threshold-damaged laurel edges. This revision restores original numeral, face shading, engraved rim, crown, jewel and leaf pixels.

## Extraction

Source: `/home/k2/Downloads/match3final/individual ranking.png`,941×1672. Source hash and exact bounds are recorded in Sources/Source.json. Each medal was submitted separately at8× interpolation to Scenario `model_pixa-background-removal`; only its alpha was used. Three successful jobs cost3CU each,9CU total. No generation or automatic resubmission occurred.

The original RGB was retained without recoloring, painting, inpainting or sharpening. Local cleanup removed disconnected fragments from the next row's crown and a one-pixel interpolation fringe at the canvas edge. The opaque medal faces remain intact; surrounding scenery and panel rails are excluded. The assets were inspected over black, white and saturated magenta. Native-size extractions preserve original RGB exactly; runtime3× resources equal a Lanczos resize of original RGB with separate alpha. Upscaling adds no source detail. Alpha boundaries are segmented estimates, so perfect recovery of the original pre-composite edge alpha is not claimed.

## Native implementation

Replaced `medal_gold.png`, `medal_silver.png` and `medal_bronze.png` in `Assets/LegendsOfRus/Resources/ProductionUI/IndividualRankingAligned/`, preserving their meta files and GUIDs. The owning layout aligns their94px width and77/77/74px source height at original y615/686/758 positions. The live1/2/3 Text components retain semantic values but rendering is disabled to prevent a second numeral over the original engraving. These are fixed podium icons; the later ranks retain live Bitter600 numerals. Title and player names retain the user's R4 selected fonts.

View, prefab builder, native prefab and cutover state use revision5. No navigation, ranking examples, period behavior, other artwork or gameplay changes.

## Verification

Alpha/RGB/provenance assertions pass for all three medals. Transparency QA was visually inspected at3×. Fresh Unity verification and scope audit are recorded alongside this report; review screenshots are actual Unity Play Mode, not offline implementation mockups.

Not performed: APK build, physical-phone check, all-page regression or backend/live leaderboard validation. No claim that this completes every remaining reference difference.

Final Unity6000.5.9f1 verification: prefab rebuild PASS;2/2 Edit Mode and6/6 Play Mode PASS;10 actual screenshots. Checks include no duplicate numeral rendering, retained selected fonts, idempotent rebuild, live localization/tab refresh, responsive text/row bounds, safe areas, routing and raycasts. Visually inspected Russian portrait capture and black/white/magenta extraction QA.

ScopeAudit.json lists9 changed existing production/test files and no new production files. All existing meta/GUID files, selected font files, other page prefabs, packages and ProjectSettings match the pre-R5 snapshot.

Review-page browser check passed: all12 capture options loaded; no horizontal overflow at390px. Scoped git diff whitespace check passed.
