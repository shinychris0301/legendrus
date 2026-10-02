# Team Ranking R8 — Footer Ornaments

Implemented the original large red side banners, candles, coins and central gold carving in the active Unity Team Ranking prefab. User explicitly selected “Move Back above footer; larger banners.” Back now sits above the decorations; seven table rows were compacted to make room at the reference aspect ratio. The corrected R7 Guild Details button and approved shared header/reward artwork remain intact.

## Assets and implementation

Three Scenario background-removal jobs completed successfully: 3 CU each, 9 CU total, no generation retries. Original source RGB is preserved after uniform 3× upsampling. Returned alpha received bounded local cleanup to remove scenery and neighboring rails; red cloth remains opaque. This is source isolation, not newly generated ornament artwork. Extraction scripts, inputs, responses, billing and hashes are retained under Sources.

Added banner_left_r8, banner_right_r8 and footer_carving_r8 sprites. Disabled the former generic footer carving. Decoration Images do not intercept input and draw below the Details/Back controls. Back and footer follow the safe bottom. Reference layout, responsive layout, view, prefab builder, active prefab and cutover agree on internal revision 9.

Rows use a 74-pixel pitch and 73-pixel height; reward enclosures, counts and crests were fitted to the shorter rows. Podiums and their approved reward hierarchy were preserved. Source fonts and alpha contours remain approximate; this is not a claim of exact full-page reproduction.

## Verification

Unity 6000.5.9f1 Verify01 import/rebuild/readback passed. All 13 EditMode and 15 PlayMode tests passed (28 total; zero failures/skips), including TeamHub and IndividualRanking regressions. Checks cover localized glyph bounds, reward clearance, Back/table/banner separation, safe-area anchoring, decorative raycast behavior, Details modal input, navigation and saved-state preservation.

Nine native Team Ranking captures were produced. Final 941×1672 Russian and 576×1280 safe-area English captures were visually inspected, along with extracted assets over black, white and magenta. Eleven browser review views decoded successfully; comparison toggle passed with no horizontal overflow or script errors.

Existing PNGs/metas/fonts and other page prefabs were preserved against the task-start snapshot. Eight existing Assets files changed, three PNGs and three metas added, no deletions. Repository-wide git diff --check reports serialized empty-name trailing whitespace in the accumulated prefab changes; no unrelated prefab cleanup was applied. Android builds, physical-device tests, performance tests and the full test suite were not run.

## Changed Assets files

- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/TeamRankingCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/TeamRanking.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionTeamRankingPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTeamRankingLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTeamRankingReferenceLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTeamRankingView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/TeamRankingAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/TeamRankingAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamRankingAligned/banner_left_r8.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamRankingAligned/banner_left_r8.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamRankingAligned/banner_right_r8.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamRankingAligned/banner_right_r8.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamRankingAligned/footer_carving_r8.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamRankingAligned/footer_carving_r8.png.meta`

## Next action

Review the native R8 footer and tighter ranking rows. If accepted, continue to Visual Page 23 — Team Chat. Remaining live-font, scenery and fine alpha differences are documented in QA/ReferenceFidelityReview.md.
