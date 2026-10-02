# Chapter Complete reference rebuild R1

Implemented 2026-09-21 after the user approved continuing from Pause. The real ChapterComplete prefab, owning builder and runtime now agree on native revision 8.

## Reference and visual changes

Original: `/home/k2/Downloads/match3final/chapter finish.png`, SHA-256 `d537fc07cc5453512dedc994d850f833eb03a012a53aa8702162d9e99f67c3f8`. The reference has a carved chapter title rather than a player/resource header; its composition is retained.

Restored visible original scenery, preserving 248,350 source pixels in the 941-square environment plate. Previously reconstructed scenery is used beneath expanded actor/UI masks. Extracted the original Russian “СЛЕДУЮЩАЯ ГЛАВА” lettering, with separate live “СКОРО” status. Existing source Russian title, ribbon, characters, stars, controls and teaser remain intact. Earlier frame-fit corrections remain in place.

One Scenario `model_openai-gpt-image-2` request repaired the English banner, Victory ribbon, Continue/World Map button faces and teaser parchment in one atlas: **12 credits, one accepted job, no paid retries**. Five bounded composites preserve source alpha and outside-mask RGB. Original Russian lettering is reused; English labels and actual quantities remain live. Request/output/billing, masks and hashes are archived under Sources. Alpha was inspected against black, white and magenta.

## Behavior and intentional differences

Six existing reward categories show actual accumulated first-clear rewards rather than the reference's four illustrative samples. The completed test fixture shows 1,850 gold, 40 gems and two of each booster; opening the page grants no additional chapter bonus. Existing Continue/World Map routes and one-time acknowledgement behavior are preserved. Chapter 2 remains an upcoming preview. The current authored chapter has ten levels; this pass does not implement the planned 500 levels per chapter. SMS verification remains deferred to the feature-development phase after UI work.

## Validation

Unity 6000.5.9f1 compiled, rebuilt and read back the active prefab successfully. Fresh Baseline01 and final Verify01 each passed four Edit Mode and six Play Mode tests covering Chapter Complete and Pause. Final RU/EN, tall safe-area and incomplete-state captures are included in the review. Native English, Russian tall-screen and incomplete views were visually inspected. All eight browser review choices loaded without JavaScript errors or horizontal overflow.

All 184 prior prefab records are retained; the new source heading brings the total to 188. Six existing Assets files changed, fourteen were added (seven PNGs and their metas), and none were deleted. Other page prefabs and previous art/metas match the task-start snapshot. Full test suite, Android build and physical-device tests were not run.

## Remaining limits

Hidden scenery and blank material faces are reconstructed, not recovered original pixels. Live English typography is an approximation; exact source font identity remains unverified. Existing narrow phone typography and complete-page scaling need physical-device assessment. This is a reference-aligned native implementation, not a claim of pixel-perfect identity.

## Changed production files

- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/ChapterCompleteAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/ChapterCompleteAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionChapterCompleteView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionChapterCompleteRuntime.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/ChapterComplete.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/banner_blank_r8.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/banner_blank_r8.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/environment_source_r8.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/environment_source_r8.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/foreground_blank_r8.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/foreground_blank_r8.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/map_blank_r8.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/map_blank_r8.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/next_blank_r8.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/next_blank_r8.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/next_heading_ru_r8.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/next_heading_ru_r8.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/teaser_blank_r8.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/teaser_blank_r8.png.meta`

Review: [ChapterComplete_ReferenceRebuild_R1_Review.html](ChapterComplete_ReferenceRebuild_R1_Review.html). Next: user visual review, then the next screen in sequence (Shop) or a requested refinement.
