# Team Chat — Reference Rebuild R1

Visual Page 23 now uses the shared Home player header, a proportional sapphire sample chest-progress bar, original Russian Send/footer lettering, and controls anchored to the phone safe edges. The existing portrait, parchment and local-composer implementation remains active in the real TeamChat prefab.

## Reference and assets

Compared the retained original `TeamChatStyleAlignment/R1/Sources/OriginalReference.png` with the earlier native Team Chat capture. The original Downloads path recorded by that pass is no longer present; the preserved project source is used and hashed in Sources/AssetProvenance.json.

Four new sprites retain original Russian lettering and source RGB under the unchanged accepted Scenario alpha masks: Send, Gifts, Help Requests and Stickers. English uses the existing blank faces with native translated labels. Black/white/magenta inspection confirmed the retained opaque faces and isolated contours. Small inherited contour approximations remain, especially the Help plaque's lower edge. No new Scenario requests or credits were needed; the initial Team Chat pass supplies the existing isolation evidence.

The shared header comes from ProductionPlayerHeader. Loading's sapphire fill, Team Hub's progress rail, Event Detail's carved footer and existing Ranking environment are reused. No existing PNGs, fonts or metas are deliberately replaced; original chat portraits and chest are preserved. The reused footer ornament and environment are consistent materials, not exact copies of this page's reference surround.

## Runtime changes

- Replaced construction from the old Shop hierarchy with direct native controls bound to the shared player header, including saved coins, gems and energy.
- Separated sample chest level and count, with a horizontal filled Image representing 7,450 / 15,000. This remains an explicitly labelled example, not live guild progress.
- Retained Russian source button lettering and live English labels; restored source footer-control bounds and added the approved carved backing.
- Reduced message-text size and inset Help captions. Limited the preview notice to the clear space beside the chest.
- Anchored header to the safe top and composer/footer/Back to the safe bottom. Taller displays increase the scrollable history viewport, rather than leaving navigation centered away from the edges. The explanation overlay expands its input shield to the available safe area.
- Owning runtime, responsive layout, view, builder, active prefab and cutover agree on revision 2. Existing routes, modal guards, local-session history and saved account state remain unchanged.

No online messaging was added and no messages were sent externally. Static presence dots and the Help badge remain labelled sample artwork. Sending adds only to local session history; unavailable chest/help/gift/sticker actions explain their current preview status.

## Verification and limits

Final run and exact totals are recorded in QA/ScopeAndVerification.json; import logs and EditMode/PlayMode XML are retained under QA. Targeted checks cover shared bindings, sample fill, RU/EN text, source-label switching, safe edges, preview/chest clearance, button hit areas, local send deduplication/literal content/retention, long-message scrolling, modal input blocking, routes, save preservation and Team Hub/Team Ranking regression behavior.

Verify01 stopped on exact floating-point containment at the shared profile safe edge. The check now allows half a screen pixel at each boundary while preserving actual edge assertions. Verify02 passed all 28 checks. Its captures identified a preview notice crowding the chest; the notice was inset and an explicit separation assertion added for Verify03. Its final visual inspection also found interference with the title jewel; Verify04 uses a shorter Sample chat label in the clear left space. The local-only session status and feature explanation remain.

No Android build, physical-device keyboard/IME, performance profiling, full-project suite or real-time backend test was performed. Font identity, repaired parchment, reused frames/scenery and fine alpha remain differences from the source. See QA/ReferenceFidelityReview.md and the standalone native capture review.

## Final delivery evidence

Verify04 Unity 6000.5.9f1 import/rebuild/readback passed. **13 EditMode + 15 PlayMode = 28 passed**, zero failed/skipped. Ten native Team Chat captures retained; final Russian and narrow English visually inspected, along with the earlier long-message capture and asset alpha sheet. All 12 browser review views and comparison toggle passed without overflow or script errors. Scoped C# whitespace checks passed. Nine existing Assets files modified, four PNGs and four metas added, no deletions. Existing PNG/meta/font files and other page prefabs match task-start hashes.

Changed Assets files:

- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/TeamChatCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/TeamChat.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionTeamChatPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionPlayerHeader.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTeamChatLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTeamChatRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTeamChatView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/TeamChatAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/TeamChatAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/footer_gift_ru_rebuild.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/footer_gift_ru_rebuild.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/footer_help_ru_rebuild.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/footer_help_ru_rebuild.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/footer_sticker_ru_rebuild.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/footer_sticker_ru_rebuild.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/send_ru_rebuild.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned/send_ru_rebuild.png.meta`
