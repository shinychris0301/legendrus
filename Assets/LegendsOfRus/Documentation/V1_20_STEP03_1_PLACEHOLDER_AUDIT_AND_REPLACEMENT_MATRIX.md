# v1.20 Step 3.1 — Placeholder Audit and Replacement Matrix

## Audit result

- Runtime `Resources/DesignScreens` PNGs: **22**
- Screenshot-backed current ScreenIds: **22**
- Unity prefab files under `Assets`: **0**
- Missing product surfaces in the target page inventory: **14**
- Reference-only approved screenshots: **11**
- Estimated raw RGBA upper bound for runtime placeholder screens: **132.04 MiB**

Every current `ScreenId` is rendered by `ScreenRouter.BuildDesignLayer` from `DesignScreenRegistry` using a `Resources.Load<Sprite>` screenshot. Invisible hotspot rectangles and selected runtime overlays are layered on top. No production screen prefab exists yet.

## Replacement matrix

| Priority | Product surface | Current implementation | Screenshot | Target prefab | Wave |
|---|---|---|---|---|---|
| P0 | Splash / Title | `missing` | — | `Assets/LegendsOfRus/Prefabs/UI/SplashScreen.prefab` | 3.3 |
| P0 | Loading | `screenshot_only` | Loading.png | `Assets/LegendsOfRus/Prefabs/UI/LoadingScreen.prefab` | 3.3 |
| P0 | Home | `hybrid_screenshot_runtime_overlay` | Home.png | `Assets/LegendsOfRus/Prefabs/UI/HomeScreen.prefab` | 3.3 |
| P0 | World Map | `screenshot_plus_hotspots` | WorldMap.png | `Assets/LegendsOfRus/Prefabs/UI/WorldMapScreen.prefab` | 3.3 |
| P0 | Level Select | `hybrid_screenshot_runtime_overlay` | ChapterMap.png | `Assets/LegendsOfRus/Prefabs/UI/LevelSelectScreen.prefab` | 3.3 |
| P0 | Pre-Level | `hybrid_screenshot_runtime_overlay` | PreLevel.png | `Assets/LegendsOfRus/Prefabs/UI/PreLevelScreen.prefab` | 3.3 |
| P0 | Gameplay | `hybrid_screenshot_runtime_board` | Gameplay.png | `Assets/LegendsOfRus/Prefabs/UI/GameplayScreen.prefab` | 3.3 |
| P0 | Pause Menu | `modal_screenshot_plus_hotspots` | Pause.png | `Assets/LegendsOfRus/Prefabs/UI/PauseModal.prefab` | 3.3 |
| P0 | Victory | `hybrid_screenshot_runtime_overlay` | Victory.png | `Assets/LegendsOfRus/Prefabs/UI/VictoryScreen.prefab` | 3.3 |
| P0 | Fail | `hybrid_screenshot_runtime_overlay` | Fail.png | `Assets/LegendsOfRus/Prefabs/UI/FailScreen.prefab` | 3.3 |
| P0 | Chapter Complete | `hybrid_screenshot_runtime_overlay` | ChapterComplete.png | `Assets/LegendsOfRus/Prefabs/UI/ChapterCompleteScreen.prefab` | 3.3 |
| P1 | Dialogue / Story | `hybrid_screenshot_runtime_overlay` | Dialogue.png | `Assets/LegendsOfRus/Prefabs/UI/DialogueScreen.prefab` | 3.4 |
| P1 | Character Collection | `hybrid_screenshot_runtime_overlay` | Characters.png | `Assets/LegendsOfRus/Prefabs/UI/CharacterCollectionScreen.prefab` | 3.4 |
| P1 | Daily Reward | `hybrid_screenshot_runtime_overlay` | DailyReward.png | `Assets/LegendsOfRus/Prefabs/UI/DailyRewardScreen.prefab` | 3.4 |
| P1 | Events Hub | `hybrid_screenshot_runtime_overlay` | Events.png | `Assets/LegendsOfRus/Prefabs/UI/EventsHubScreen.prefab` | 3.4 |
| P1 | Event Detail | `missing` | — | `Assets/LegendsOfRus/Prefabs/UI/EventDetailScreen.prefab` | 3.4 |
| P1 | Individual Ranking | `missing` | — | `Assets/LegendsOfRus/Prefabs/UI/IndividualRankingScreen.prefab` | 3.4 |
| P2 | Team Hub | `missing` | — | `Assets/LegendsOfRus/Prefabs/UI/TeamHubScreen.prefab` | 3.5 |
| P2 | Team Ranking | `missing` | — | `Assets/LegendsOfRus/Prefabs/UI/TeamRankingScreen.prefab` | 3.5 |
| P2 | Team Chat | `missing` | — | `Assets/LegendsOfRus/Prefabs/UI/TeamChatScreen.prefab` | 3.5 |
| P2 | Friends List | `missing` | — | `Assets/LegendsOfRus/Prefabs/UI/FriendsScreen.prefab` | 3.5 |
| P2 | Mail / Inbox | `hybrid_screenshot_runtime_overlay` | Mail.png | `Assets/LegendsOfRus/Prefabs/UI/MailScreen.prefab` | 3.5 |
| P1 | Profile | `hybrid_screenshot_runtime_overlay` | Profile.png | `Assets/LegendsOfRus/Prefabs/UI/ProfileScreen.prefab` | 3.4 |
| P1 | Inventory | `missing` | — | `Assets/LegendsOfRus/Prefabs/UI/InventoryScreen.prefab` | 3.4 |
| P1 | Shop | `hybrid_screenshot_runtime_overlay` | Shop.png | `Assets/LegendsOfRus/Prefabs/UI/ShopScreen.prefab` | 3.4 |
| P1 | Special Offer Popup | `missing` | — | `Assets/LegendsOfRus/Prefabs/UI/SpecialOfferModal.prefab` | 3.4 |
| P0 | Out of Energy | `modal_screenshot_plus_hotspots` | OutOfEnergy.png | `Assets/LegendsOfRus/Prefabs/UI/OutOfEnergyModal.prefab` | 3.3 |
| P0 | Booster Select Popup | `modal_screenshot_plus_hotspots` | BoosterSelect.png | `Assets/LegendsOfRus/Prefabs/UI/BoosterSelectModal.prefab` | 3.3 |
| P1 | Reward Claim Popup | `modal_screenshot_plus_hotspots` | RewardClaim.png | `Assets/LegendsOfRus/Prefabs/UI/RewardClaimModal.prefab` | 3.4 |
| P1 | Settings | `hybrid_screenshot_runtime_overlay` | Settings.png | `Assets/LegendsOfRus/Prefabs/UI/SettingsScreen.prefab` | 3.4 |
| P2 | Language Select | `missing` | — | `Assets/LegendsOfRus/Prefabs/UI/LanguageSelectScreen.prefab` | 3.5 |
| P2 | Login | `missing` | — | `Assets/LegendsOfRus/Prefabs/UI/LoginScreen.prefab` | 3.5 |
| P0 | Tutorial Overlay | `runtime_generated_unstyled` | — | `Assets/LegendsOfRus/Prefabs/UI/TutorialOverlay.prefab` | 3.3 |
| P2 | Offline State | `missing` | — | `Assets/LegendsOfRus/Prefabs/UI/OfflineStateModal.prefab` | 3.5 |
| P2 | Maintenance State | `missing` | — | `Assets/LegendsOfRus/Prefabs/UI/MaintenanceScreen.prefab` | 3.5 |
| P2 | Empty / Error Variants | `missing` | — | `Assets/LegendsOfRus/Prefabs/UI/SystemStateVariants.prefab` | 3.5 |
| P2 | Legacy Chapter 15 special-case page | `legacy_screenshot_special_case` | Chapter15.png | `RETIRE_AS_DATA_DRIVEN_CHAPTER_DETAIL` | 3.5 |

## Reconstruction order

**Wave 3.2 — Canonical UI kit**: build reusable frames, 9-sliced panels, buttons, icon buttons, top/currency bars, navigation, modal blockers, reward/booster slots, badges, toggles, and system-state components. No screen cutover yet.

**Wave 3.3 — Core loop**: Splash/Title, Loading, Home, World Map, Level Select, Pre-Level, Gameplay, Tutorial Overlay, Pause, Victory, Fail, Chapter Complete, Out of Energy, Booster Select.

**Wave 3.4 — Meta and LiveOps**: Dialogue, Character Collection, Daily Reward, Events Hub/Detail, Individual Ranking, Profile, Inventory, Shop, Special Offer, Reward Claim, Settings.

**Wave 3.5 — Social and system**: Team Hub/Ranking/Chat, Friends, Mail, Language, Login, offline/maintenance/empty/error states, and retirement of the legacy Chapter15 special-case route.

## Hard constraints

- Approved screenshots are reference/QA artifacts only; they are not to be deleted in Step 3.1.
- Locked artwork may not be overwritten automatically.
- Screenshot-backed runtime routes remain active until their target prefab passes visual and interaction verification.
- Runtime cutover must be page-by-page and reversible.
- Final text must be live TMP/uGUI text, not baked screenshot text.
- Responsive validation must cover 16:9, 18:9, 19.5:9 and 20:9 portrait targets.
- Gameplay board logic is preserved; Step 3 replaces presentation around/under the board rather than rewriting board rules.

## Step 3.1 exit condition

Step 3.1 is complete when the inventory is reproducible, all screenshot-backed routes and missing target surfaces are catalogued, every target prefab has a reconstruction wave and shared-kit dependency list, and no runtime cutover has occurred.

## Next checkpoint

**v1.20 Step 3.2 — Canonical UI Kit Reconstruction**.
