# Individual Ranking — Selected Typography R4

The user selected three comparison rows, counting candidates below the “Original” row: player names #6, title #3 and ranks #2. These choices are implemented only on Individual Ranking.

| Role | Selected family | Packaged form |
| --- | --- | --- |
| Player names and header Ivan | Roboto Serif SemiBold, weight 600, optical size 20, width 100, grade 0 | Static instance, family renamed LoR Ranking Names |
| Main title | FreeSerif Bold, version 0412.4271 | Unmodified downloadable font |
| Rank numerals | Bitter SemiBold, weight 600 | Static instance, family renamed LoR Ranking Numerals |

The role fonts are serialized in the native prefab and assigned by the owning layout builder. Refreshing content, locale or period retains the selection. Synthetic bold is disabled on these roles. Rank numerals are horizontally centered in ordinary circular badges. The longest English name can shrink to 18 design units to fit its existing single-line column. No names were shortened.

The shared typography asset and all other text roles remain unchanged. This revision does not identify the original image’s exact typefaces or replace medal artwork. Existing remaining differences in medal relief, footer decoration and artwork are outside this typography change.

## Resources

Three fonts with embedded font data are in `Assets/LegendsOfRus/Resources/ProductionUI/IndividualRankingFonts/`. Licences are in `Assets/LegendsOfRus/ThirdParty/RankingFonts/`. Roboto Serif and Bitter came from official Google Fonts downloads already recorded during identification. Their static derivatives are renamed to avoid reserved-name conflicts. FreeSerif Bold was obtained from a fresh official Ubuntu archive download of GNU FreeFont; no installed system font file was copied. The older 2012 GNU binary was examined but not used because it differs from the selected version. FontSources.json records URLs, axes, versions and SHA-256 hashes.

## Verification

Final verification results and scope audit are recorded alongside this report. The first run passed 2 Edit Mode tests and 3 of 6 Play Mode tests; the other three failed on the longest English name’s height. The minimum fitted name size was corrected before the final rebuild and repeated targeted tests.

No Scenario call or image generation was needed. No APK, Android phone, all-pages regression or live leaderboard/backend test was performed. Test screenshots use an isolated save and sample standings, not real ranking data.

Final Unity 6000.5.9f1 results: prefab import/rebuild PASS; Edit Mode 2/2 PASS; Play Mode 6/6 PASS. Ten actual captures cover Russian/English, 941×1672, 576×1280 and 1080×2400 safe areas, tabs, rules and maximum balances. Tests assert serialized embedded fonts, builder idempotence, font persistence through locale/tab changes, text height, row bounds, safe areas, raycasts, routing and sample-data behavior. Cyrillic/Latin/digit glyph coverage and static font tables passed. FreeSerif output SHA-256 equals the selected comparison binary. Visually inspected final English portrait and narrow Russian safe-area captures, plus the initial Russian portrait.

Scope audit: seven existing production/test files changed, with font resources/licences added. All pre-existing `.meta` files, other page prefabs, artwork, packages and ProjectSettings are byte-identical to the start-of-task snapshot. See ScopeAudit.json for the exact list.

Review HTML check: all12 screenshot/reference options loaded in Chrome through Playwright;390px viewport had no horizontal overflow. The default bundled Playwright browser was absent, so the check used the installed Google Chrome executable. Scoped `git diff --check` passed.
