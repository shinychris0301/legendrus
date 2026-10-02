# Team Chat R2 — Chest and Message Frames

Implemented the user's approved R2 reference-fidelity recommendation in the active TeamChat prefab. The chest now uses a source-derived navy enclosure and original guild crest, chest placement and information medallion. A continuous dark inset and source side rails replace the separate textured chat rectangle. The system message uses the original crest and dedicated source-derived rail. Eight sample rows now follow the reference's vertical landmarks and larger portrait sizes.

## Art and Scenario

Source is the preserved dedicated original from the initial Team Chat extraction, copied under Sources/OriginalReference.png. Three successful Scenario `model_pixa-background-removal` jobs isolated guild crest, system crest and information icon: **9 CU total**, no generation retries. Original source RGB was restored under returned alpha with bounded support masks to exclude neighboring rails. Request inputs, hashes, job/status, billing and unmodified outputs are retained under Sources/Scenario; QA/ScenarioAudit.json verifies the submitted inputs remain unchanged.

Three additional independent panel sprites retain source rails and replace obscured contents with a clean original navy donor. The large surround retains narrow source side bands and bottom rail; its top continuation and opaque navy interior are reconstructed. Chest rails underneath the original crest/chest and the system frame's text area are similarly repaired. Estimated outer masks preserve intentionally opaque faces. These are isolated empty panel surfaces, not screenshot content with interactive hotspots. Reconstruction and reflected donor texture are not exact recovery of hidden source pixels.

All six new sprites were inspected over black, white and magenta. The three isolated icons assert original RGB preservation after uniform upsampling. Existing portraits, chest, parchment, fonts, shared header, progress fill/rail and source-lettered footer controls are reused unchanged. No regeneration of approved characters or rewards was needed.

## Native implementation

- Runtime/view/layout/builder/active prefab/cutover agree on internal revision 3.
- Chest enclosure: source-relative 58,278,823×133; guild crest:88,250,136×158; chest:638,269,191×147. New native information button uses the existing chest explanation and modal/input guards.
- Continuous surround begins at24,276 and extends to1404 at reference size; it expands toward the safe bottom on taller phones. Its sliced edge preserves rail thickness. Chat viewport begins at410 and is985 reference pixels tall.
- Sample row heights are141,132,125,120,90,120,125,132, aligning later messages and filling the lower gap. Existing live text/localized content and parchment remain. Dedicated system heading, timestamp and body stay inside its source rail; crest has its original circular ornament.
- Sample notice moves under the chest-progress lane. Real shared account values, local-only composer/history, sample status, native Back/header routes and unavailable-feature explanations remain intact. No external messages sent or online guild features added.

## Verification

Unity6000.5.9f1 Verify01 import/rebuild/serialized readback passed. **13 EditMode +15 PlayMode =28 passed**, zero failures/skips, including Team Hub/Team Ranking regressions. Existing phone/layout/local-message/navigation/save tests ran; the new information button was added to the modal/background-input-blocking checks. Ten Team Chat captures were produced, including RU/EN, safe areas, maximum balances, long local messages and info overlays. Final native Russian941×1672 and narrow English576×1280 captures were visually inspected.

Browser review checks and exact changed-file/hash preservation results are in QA. No Android build, physical-device/keyboard test, performance profiling, backend test or full-project test suite was performed. Exact source font, parchment grain, missing energy medallion/message illustrations, outer scenery and page-specific footer carving remain future visual work.

## Changed Assets files

- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/TeamChatCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/TeamChat.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionTeamChatPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTeamChatLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTeamChatRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTeamChatView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/TeamChatAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/chat_surround_r2.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/chat_surround_r2.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/chest_info_r2.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/chest_info_r2.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/chest_panel_r2.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/chest_panel_r2.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/guild_crest_r2.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/guild_crest_r2.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/system_crest_r2.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/system_crest_r2.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/system_panel_r2.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/system_panel_r2.png.meta`
