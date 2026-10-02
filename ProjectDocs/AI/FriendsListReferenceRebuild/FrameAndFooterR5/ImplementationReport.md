# Friends List Frame and Footer R5 — 2026-09-25

Continued with the Friends List frame correction and checked footer icons on both Friends List and the currently open World Map. The source frame's former slicing bands cut through its lower curls and upper finials. Changed its border from(96,80,100,86) to(160,300,180,200) source pixels so complete corners stay fixed as the central panel extends. Existing frame texture/RGB/alpha are unchanged. The inner sapphire rail intentionally ends above the bottom corner, as shown in the original reference; it is separate from the continuous outer frame.

Runtime, owning builder, active FriendsList.prefab and cutover revision6 agree. Requests layout, search/sort, unavailable social actions, saved state, Home/Team caller-aware return behavior and footer routes are preserved.

## Footer check

Friends List uses its previously approved original Russian icon/caption sprites and corresponding English blank faces with live labels. Inspected full source artwork and original page, then native reference/tallRU and tallEN captures. All five icons retain complete sprite rectangles, original aspect ratios, non-overlapping bounds and visible-center raycast targets. Existing route tests verify World Map, Shop, Events, Friends and Characters actions commit once. No icon replacement or recoloring was warranted.

The full-width footer region is pixel-identical to the pre-task audit capture at941×1672 (rows1498 onward) and1080×2400 (rows2118 onward). Zero differing pixels in both comparisons. This confirms the frame repair did not alter approved icons or footer decoration.

World Map was also checked using its approved full-sprite navigation artwork and rerun layout/raycast/routes tests; its production files were not modified. Its latest native tall capture is included in the review. No clipping/overlap was found in the tested footer layouts. This does not establish pixel-perfect extraction across every asset or physical device.

## Verification

Verify01:2Edit Mode+13Play Mode=15passed,0failed/skipped. Play coverage:6Friends List and7World Map tests. Added per-capture footer aspect/full-sprite/non-overlap assertions and a tallEnglish Friends capture. Existing checks cover visible control raycasts, safe-area bounds, footer placement and once-only routes.

14actual Friends List Unity captures retained with World Map regression captures/logs/XML/commands. Nine portrait-review choices decoded in Chrome without script errors or horizontal overflow. Inspected final tallRU/EN Friends captures and source images; safe areas are simulated in Unity6000.5.9f1, not physical-phone evidence.

7160existing Assets unchanged;6files modified,no Assets added/deleted. All PNGs/fonts/other page prefabs unchanged. Only spriteBorder changed in the frame meta; GUID and all other metadata verified unchanged by reconstructing the before hash. No image generation/Scenario calls. Scoped whitespace check passed; generated YAML empty-field whitespace normalized after tests without changing serialized values.

## Changed files

- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/FriendsListCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/FriendsListAligned/outer_frame_r2.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/FriendsList.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionFriendsListPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionFriendsListView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/FriendsListAlignmentPlayModeTests.cs`

## Limits and next step

No Android build, physical-device/keyboard, backend, performance or full-game test run. Existing live-font, repaired surface and finite six-row preview spacing remain. The icon checks cover the displayed RU/EN layouts; they do not introduce live friends services or send messages.

Next: review the corrected frame/footer, then extend the safe-area audit to remaining pages such as Mail, Events and ranking screens.
