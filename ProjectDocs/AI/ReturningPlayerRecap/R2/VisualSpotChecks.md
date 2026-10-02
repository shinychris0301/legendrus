# Native portrait inspection — story recap R2

Six exact current captures were opened and examined. They render the real instantiated production prefabs/components through the existing router Canvas at simulated Editor viewports and safe areas. They are neither generated compositions nor phone captures.

| Inspected capture | Observation |
|---|---|
| [Latest RU, 576×1280](QA/NativeEntryVerified/Captures/LOR_RECAP_CAPTURE_DIR/Recap_Latest_RU.png) | Level2, scene3/3. Live narrator/story text on existing parchment, disabled Later, readable gold top navigation with distinct gaps. Used as the sole review-page image. |
| [Profile story entry RU, 576×1280](QA/NativeEntryVerified/Captures/LOR_RECAP_CAPTURE_DIR/Profile_StoryEntry_RU.png) | History page2/2, existing rows/page controls retained. Story action fits beside the narrowed section heading below original tabs. Actual native pointer entry passed in this capture case. |
| [Earlier EN, 1080×2400](QA/NativeFinal/Captures/LOR_RECAP_CAPTURE_DIR/Recap_Earlier_EN.png) | Existing Ivan/forest composition and localized text retained. Live heading/navigation remain within the simulated safe area. Tall safe-area margins are visible. |
| [Intro Back EN, 941×1672](QA/NativeFinal/Captures/LOR_RECAP_CAPTURE_DIR/Recap_Back_EN.png) | Existing two-character intro, final English line and top controls. Both browse controls disabled for intro-only ledger. Real Back pointer closes reader. |
| [Empty History RU, 576×1280](QA/NativeFinal/Captures/LOR_RECAP_CAPTURE_DIR/Profile_Empty_RU.png) | Disabled two-line no-read-scenes label and existing truthful no-attempts message. Header/tab/footer composition preserved; small disabled label remains a physical-readability limitation. |
| [Earlier RU, 576×1280, legacy filename](QA/NativeFinal/Captures/LOR_RECAP_CAPTURE_DIR/Recap_Latest_RU.png) | Despite its filename, this capture shows level1/scene2 after Earlier. Existing Ivan/forest/parchment and live Russian lines inspected; superseded as latest-scene review evidence. |

The [standalone portrait page](StoryRecap_R2_Review.html) displays one actual native image. No sprite/font asset was changed or generated; existing family materials and fonts are reused. No phone touch-size/readability or exact font identity acceptance is implied. Runtime assertions cover safe bounds and actual pointer targets; these inspections do not establish every localized glyph at every device size or user design acceptance.
