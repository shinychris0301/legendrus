# Team Chat R3 — Message Icons and Parchment

Continued the documented R2 recommendation. Restored the reference blue energy medallion, crossed swords and strength illustration, reused approved gold lightning, and restored visible grain to the previously smoothed parchment interior. R2 frames, row heights, crests, information control, shared header and source-lettered footer remain unchanged.

## Source and asset workflow

The preserved original Team Chat image is copied under Sources/OriginalReference.png. Existing energy and sword resources were inspected before extraction: the available green energy action plaque, gold bolt and shield-backed swords do not match the reference's blue medallion and plain crossed swords. NavigationAligned/energy was suitable for the small gold bolt and is reused unchanged. No matching existing strength illustration was found.

Three successful Scenario background-removal jobs cost **9 CU total**, with no generation retries. They supply alpha only. Runtime icons retain original source RGB after uniform upsampling; disconnected parchment fragments were removed from alpha, and the medallion received a bounded silhouette support. Original inputs, request hashes, job/status, billing and unmodified outputs remain under Sources/Scenario. Tiny source icons contain limited detail; enlargement does not recover new detail or establish exact transparency.

The new parchment variant retains the old alpha and all pixels outside the initial text-removal mask. High-frequency grain from a clean original patch, source bounds696,500–830,527, is blended only into that mask with a soft interior transition. Existing repaired base color and original ornamental edges remain. This is reconstructed grain, not exact recovery of paper hidden under source letters. No new RGB image generation or full-screen composite was used.

## Native behavior and localization

Decorative Images do not receive input. Short RU/EN messages reserve space for their icons and are measured again after responsive canvas scaling. The gift row keeps a native +5 amount beside the reused gold bolt; sample chat/member labels and feature explanation still make its illustrative status clear. No energy is granted, messages remain local-only, and no external messaging occurs.

Sample row heights, scroll viewport, source R2 frames, live shared account header, local composer/history, unavailable-feature explanations and routing are preserved. Both sample and user-entered message bubbles use the refined parchment. Owner/view/layout/builder/active prefab/cutover agree on internal revision4.

## Verification

Evidence is retained under QA and UnityCaptures. Verify01 imported/rebuilt and passed editor checks, but detected a Russian inline message slightly exceeding its reserved width after display scaling. Fitting was changed to remeasure after responsive layout and decrement font size until the reserved lane is clear; Verify02 verifies the resulting implementation.

Checks include localized text bounds and icon clearance, noninteractive icon raycasts, sample amount, native modal controls, local message retention/literal content, safe areas, navigation/save preservation and Team Hub/Team Ranking regressions. Black/white/magenta transparency and the clean donor patch were inspected. Asset script assertions verify original icon RGB, unchanged parchment alpha and unchanged pixels outside the repair mask.

No Android build, physical-device/keyboard/IME test, performance test, online backend test or full-project suite was run. Live typography, reconstructed panel/paper surfaces, fine alpha and reused footer/scenery remain visual approximations.

## Final delivery evidence

Verify02 Unity6000.5.9f1 import/rebuild/readback passed. **13 EditMode +15 PlayMode =28 passed**, zero failures/skips, including Team Hub and Team Ranking regressions. Ten actual Team Chat captures retained; final941×1672 RU and576×1280 safe-area EN visually inspected. All12 browser views decoded, comparison toggle passed, no script errors/horizontal overflow. Scoped source whitespace check passed.

Seven existing Assets files changed; four PNGs and four metas added; no deletions. Existing PNGs/metas/fonts, R2 frame resources and other page prefabs match task-start hashes. Exact changed files:

- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/TeamChatCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/TeamChat.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionTeamChatPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTeamChatLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTeamChatRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTeamChatView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/TeamChatAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/energy_medallion_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/energy_medallion_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/message_frame_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/message_frame_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/message_strength_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/message_strength_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/message_swords_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/message_swords_r3.png.meta`
