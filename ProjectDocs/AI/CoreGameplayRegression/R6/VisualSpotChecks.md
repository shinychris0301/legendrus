# R6 — actual Unity capture spot checks

Inspected these standalone 360×640 Unity Editor captures from the broad run. They are offscreen native Canvas captures with simulated viewports, not generated designs or phone screenshots. Source artwork and production prefabs were not changed in this task.

| Page | Capture and observation |
| --- | --- |
| Gameplay | [Russian booster names](QA/Broad/Captures/LOR_GAMEPLAY_CAPTURE_DIR/CaptionNames_ru_360.png): all four full names appear on the installed native caption faces. Existing board, wood cards, icons and quantities retained. Small captions/badges are not a universal phone-size acceptance claim. |
| Victory | [Russian first-clear](QA/Broad/Captures/LOR_VICTORY_CAPTURE_DIR/LiveCaptions_FirstClear_ru_360.png): maximum Int32 score fits; live score/reward/booster headings and count labels remain visible over the blank panel, with no duplicate baked headings. Offer availability is established by native tests, not this still image. |
| Chapter Complete | [Round 500](QA/Broad/Captures/LOR_CHAPTER_COMPLETE_CAPTURE_DIR/Chapter1_500_ru_360.png): 500-round completion and unavailable Chapter 2 retained. The 3/1500 star display reflects explicit earlier-progress setup with zero historical stars, not a human campaign result. |
| Pause | [Russian panel](QA/Broad/Captures/LOR_PAUSE_CAPTURE_DIR/Pause_360_RU.png): goals, inventory and Continue/Restart/Settings/Exit remain visible within the existing panel. |
| PreLevel | [Claimed reward](QA/Broad/Captures/LOR_PRELEVEL_CAPTURE_DIR/PreLevel_RewardClaimed_360_RU.png): replay/claimed state shows zero pending currency reward, retained booster controls and Play. This is a test fixture. |

These five checks are not comprehensive review of every generated capture. Physical input, device readability/performance, large stock badges and exact source-font identity remain separate open items. No image regeneration, new font distribution or design acceptance is claimed.
