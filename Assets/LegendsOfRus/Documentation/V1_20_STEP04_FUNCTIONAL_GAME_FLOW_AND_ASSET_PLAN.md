# v1.20 Step 4 — Functionality-First Game Flow and Page Asset Plan

## Priority

This checkpoint prioritizes complete navigation and game-function boundaries over final visual fidelity. Existing demo/captured pages may remain temporarily. Production services are never faked: social, leaderboard, account, LiveOps publishing and payment-provider actions remain local-demo or fail-closed until real providers are connected.

## Core flow

`Splash/Title → Loading → Home → World Map → Level Select → Pre-Level → Gameplay → Victory/Fail → Story/Chapter Complete → Map`

Hardware/Android Back and visible Back controls use one deterministic parent-routing policy. In particular, `ChapterMap → WorldMap → Home`.

## Page function + future asset requirements

### Splash / Title (`SplashTitle`)
- **Function:** Boot entry; continue into Loading.
- **Transitions:** Continue→Loading
- **State:** none
- **Backend:** none
- **Logic status:** demo-local
- **Asset resources:** full-screen title environment, game logo, title ornament, start/continue button, version/legal microcopy, ambient particles.

### Loading (`Loading`)
- **Function:** Load local services/content and transition to Home.
- **Transitions:** Auto→Home
- **State:** none
- **Backend:** none
- **Logic status:** implemented
- **Asset resources:** loading environment, logo, progress frame/fill, tip parchment, loading animation.

### Home (`Home`)
- **Function:** Main navigation hub; show resources and progression entry.
- **Transitions:** Profile, Settings, WorldMap, Shop, Events, Characters, Mail, DailyReward, Play
- **State:** save-v6 resources/progress
- **Backend:** none
- **Logic status:** implemented
- **Asset resources:** home environment, hero character layers, Firebird layer, top HUD, resource icons, navigation icons, play CTA, quest/progress panel, notification badges.

### World Map (`WorldMap`)
- **Function:** Select chapter/region and return to Home.
- **Transitions:** Chapter→ChapterMap; Back→Home
- **State:** selected chapter
- **Backend:** none
- **Logic status:** implemented
- **Asset resources:** world map environment plate, region art layers, route/path, chapter nodes, lock states, star counters, top HUD, back/home control, bottom navigation.

### Level Select / Chapter Map (`ChapterMap`)
- **Function:** Select unlocked level in current chapter.
- **Transitions:** Level→PreLevel; Back→WorldMap
- **State:** selected level, unlock state, stars
- **Backend:** none
- **Logic status:** implemented
- **Asset resources:** chapter environment, chapter ribbon/title, level path, level nodes, star states, lock states, back button, play button, reward chest marker.

### Pre-Level (`PreLevel`)
- **Function:** Review goals/moves/rewards and optional boosters before spending energy.
- **Transitions:** Boosters modal; Start→Gameplay; Back→LevelSelect
- **State:** selected level, boosters, energy
- **Backend:** none
- **Logic status:** implemented
- **Asset resources:** chapter background, goals frame, goal icons, moves medallion, booster slots, reward panel, back button, start button.

### Gameplay (`Gameplay`)
- **Function:** Run Match-3 board, goals, moves, boosters, pause, win/fail.
- **Transitions:** Pause; Victory/Fail; Tutorial/Dialogue
- **State:** board state, score, goals, moves, boosters, level attempt
- **Backend:** none
- **Logic status:** implemented
- **Asset resources:** gameplay environment, top HUD, portrait, goal icons, moves frame, board frame/cells, gems/special gems, obstacles, booster bar, score bar, VFX, combo text, pause button.

### Pause Menu (`Pause`)
- **Function:** Pause safe gameplay state and resume/restart/settings/exit.
- **Transitions:** Resume; Restart; Settings; Exit→LevelSelect
- **State:** live board pause state
- **Backend:** none
- **Logic status:** implemented
- **Asset resources:** modal dimmer, pause frame, goals snapshot, booster row, resume/restart/settings/exit buttons.

### Victory (`Victory`)
- **Function:** Present clear result, stars/rewards, optional verified reward-double.
- **Transitions:** Continue→story/chapter flow; Double reward provider
- **State:** last result/reward
- **Backend:** rewarded-ad provider optional
- **Logic status:** implemented
- **Asset resources:** victory environment, hero/Firebird celebration, victory ribbon, star reveal, reward chest/items, continue button, reward-double panel, confetti/glow VFX.

### Fail (`Fail`)
- **Function:** Present failed goals and retry/exit options.
- **Transitions:** Retry→PreLevel/Gameplay; Exit→LevelSelect
- **State:** failure/attempt counters
- **Backend:** none
- **Logic status:** implemented
- **Asset resources:** failure environment, hero reaction, fail ribbon, failed goal rows, retry button, exit button, continue offer area, failure VFX.

### Chapter Complete (`ChapterComplete`)
- **Function:** Present chapter summary/reward and acknowledge completion.
- **Transitions:** Continue/Map→WorldMap
- **State:** chapter completion acknowledgement
- **Backend:** none
- **Logic status:** implemented
- **Asset resources:** chapter finale environment, hero/Firebird, chapter complete title, star summary, reward items, next chapter card, continue/map buttons.

### Dialogue / Story (`Dialogue`)
- **Function:** Show localized story line and advance deterministic story state.
- **Transitions:** Continue→next dialogue/prelevel/chapter map/chapter complete
- **State:** completed story scenes/storyStage
- **Backend:** none
- **Logic status:** implemented
- **Asset resources:** story environment/character scene, dialogue panel, speaker plate/avatar, continue button, skip/history optional icons.

### Character Collection (`Characters`)
- **Function:** Browse/unlock/select characters; current demo selects Ivan.
- **Transitions:** Select/Use; Back→Home
- **State:** selected/owned character future state
- **Backend:** account/content backend optional
- **Logic status:** partial
- **Asset resources:** collection background, character cards, portrait/full-body layers, rarity frames, progress shards, use/select button, filters/tabs, lock states.

### Daily Reward (`DailyReward`)
- **Function:** Claim once-per-day local reward and show claim modal.
- **Transitions:** Claim→RewardClaim; Back→Home
- **State:** lastDailyRewardUtc/resources
- **Backend:** none
- **Logic status:** implemented
- **Asset resources:** daily calendar frame, 7-day reward cells, claimed/current/locked states, reward icons, claim button, timer.

### Events Hub (`Events`)
- **Function:** Browse active events and open details.
- **Transitions:** Event card→EventDetail; Back→Home
- **State:** event availability future
- **Backend:** LiveOps backend required for production
- **Logic status:** demo-local
- **Asset resources:** events environment, event cards/banners, countdown badges, progress bars, reward previews, event currency, tabs.

### Event Detail (`EventDetail`)
- **Function:** Review event objective/progress/rewards; demo claim records session only.
- **Transitions:** Claim demo; Back→Events
- **State:** demo event claim session state
- **Backend:** LiveOps/reward authority required
- **Logic status:** demo-local
- **Asset resources:** event hero art, event title frame, objective rows, progress bar, milestone rewards, event shop/pass CTA, claim button, timer.

### Individual Ranking (`IndividualRanking`)
- **Function:** Display player leaderboard and player rank.
- **Transitions:** Back→Home
- **State:** ranking snapshot future
- **Backend:** leaderboard backend required
- **Logic status:** demo-local
- **Asset resources:** ranking background, season header, rank tabs, ranking rows, avatar frames, rank badges, score icons, player sticky row.

### Team Hub (`TeamHub`)
- **Function:** Social hub for team ranking/chat/friends.
- **Transitions:** Ranking, Chat, Friends; Back→Home
- **State:** team membership future
- **Backend:** social backend required
- **Logic status:** demo-local
- **Asset resources:** team banner/emblem, team header, member summary, team chest/progress, ranking/chat/friends buttons, member avatars.

### Team Ranking (`TeamRanking`)
- **Function:** Display ranked teams.
- **Transitions:** Back→TeamHub
- **State:** ranking snapshot future
- **Backend:** leaderboard backend required
- **Logic status:** demo-local
- **Asset resources:** team ranking header, team rows, emblems, rank badges, score/trophy icons, player team highlight.

### Team Chat (`TeamChat`)
- **Function:** Send/view team messages; demo send is session-only.
- **Transitions:** Send demo; Back→TeamHub
- **State:** demo message count
- **Backend:** chat/moderation backend required
- **Logic status:** demo-local
- **Asset resources:** chat background, message bubbles, avatars, timestamp/status icons, composer field, send button, moderation/report icons.

### Friends List (`FriendsList`)
- **Function:** Browse friends and request energy; demo request is session-only.
- **Transitions:** Request energy; Back→TeamHub/Home
- **State:** demo request state
- **Backend:** friends backend required
- **Logic status:** demo-local
- **Asset resources:** friends rows, avatars, online status, gift/energy icons, invite/add button, search field, request button.

### Mail / Inbox (`Mail`)
- **Function:** List messages/rewards; current reward claim is intentionally unavailable.
- **Transitions:** ClaimAll safe stub; Back→Home
- **State:** mail future
- **Backend:** mail/backend required
- **Logic status:** partial
- **Asset resources:** mail background, message rows, sender icons, unread badges, attachments/reward icons, claim buttons, empty state.

### Profile (`Profile`)
- **Function:** Show player identity/progress and demo access to ranking/team/inventory/login.
- **Transitions:** Ranking, Team, Inventory, Login; Back→Home
- **State:** save-v6 progress; account future
- **Backend:** account backend optional
- **Logic status:** partial
- **Asset resources:** profile background, avatar frame, name plate, level/progress, stats panels, achievements, edit-name button, account link badge.

### Inventory (`Inventory`)
- **Function:** Show owned boosters/items from save/game state.
- **Transitions:** Back→Home
- **State:** booster counts/resources
- **Backend:** none
- **Logic status:** demo-local
- **Asset resources:** inventory background, tabs, item slots, item icons, quantity badges, rarity frames, empty slot/state, item detail popup.

### Shop (`Shop`)
- **Function:** Display offers and route purchases through verified monetization facade.
- **Transitions:** Packs; SpecialOffer; Back→Home
- **State:** catalog/provider receipts
- **Backend:** IAP backend/provider required
- **Logic status:** implemented-fail-closed
- **Asset resources:** shop background, category tabs, pack cards, currency icons, price buttons, best-value badges, restore/info controls, special offer banner.

### Special Offer Popup (`SpecialOffer`)
- **Function:** Present targeted offer; purchase remains fail-closed without verified provider.
- **Transitions:** Buy; Back→Shop
- **State:** provider receipt only
- **Backend:** IAP backend/provider required
- **Logic status:** demo-local
- **Asset resources:** offer hero art, modal frame, bundle item icons, original/sale price, discount badge, countdown, buy/close buttons.

### Out of Energy (`OutOfEnergy`)
- **Function:** Block level start when energy insufficient; buy/ad/friend options fail safely.
- **Transitions:** Buy; WatchAd; AskFriends demo; Close
- **State:** energy/time
- **Backend:** IAP/rewarded/social providers optional
- **Logic status:** implemented
- **Asset resources:** energy modal, heart/energy icon, refill timer, purchase row, friend request row, rewarded-ad row, close button.

### Booster Select Popup (`BoosterSelect`)
- **Function:** Choose pre-level boosters and start.
- **Transitions:** Play/close
- **State:** booster inventory/selection
- **Backend:** none
- **Logic status:** implemented
- **Asset resources:** booster modal, booster grid, booster icons, count badges, selected states, info tooltips, play/close buttons.

### Reward Claim Popup (`RewardClaim`)
- **Function:** Acknowledge a verified/local reward result.
- **Transitions:** Claim/close
- **State:** reward result
- **Backend:** provider-specific when applicable
- **Logic status:** implemented
- **Asset resources:** reward modal, chest/reward burst, reward item icons, quantity labels, claim button, shine/particle VFX.

### Settings (`Settings`)
- **Function:** Toggle music/SFX/vibration/notifications; access language/help/system demos.
- **Transitions:** Toggles; Language; Tutorial; Back
- **State:** save-v6 settings
- **Backend:** OS/provider optional for notifications
- **Logic status:** implemented
- **Asset resources:** settings panel, toggle controls, audio/haptic icons, notification icon, language row, support/privacy rows, back/close button.

### Language Select (`LanguageSelect`)
- **Function:** Choose locale; demo stores session language only.
- **Transitions:** RU/EN demo; Back→Settings
- **State:** session language; future persisted locale
- **Backend:** localization content required
- **Logic status:** demo-local
- **Asset resources:** language background, language rows, flags/locale icons, selected checkmark, download indicator optional, back button.

### Login (`Login`)
- **Function:** Guest entry now; production account providers later.
- **Transitions:** Guest→Home; Back→Profile
- **State:** demo guest state
- **Backend:** identity/cloud backend required
- **Logic status:** demo-local
- **Asset resources:** login environment, logo, guest button, provider buttons, privacy/legal links, account recovery link, loading/error states.

### Tutorial Overlay (`TutorialOverlay`)
- **Function:** Explain controls; gameplay tutorial remains data-driven.
- **Transitions:** Back/help close; gameplay triggers existing presenter
- **State:** completedTutorialSteps
- **Backend:** none
- **Logic status:** partial
- **Asset resources:** spotlight mask, hand/pointer, callout panel, arrow, step dots, skip/next controls, highlight frames.

### Offline State (`OfflineState`)
- **Function:** Explain offline limitations and allow safe retry/local play.
- **Transitions:** Retry/Home
- **State:** network status future
- **Backend:** network provider
- **Logic status:** demo-local
- **Asset resources:** offline illustration, status icon, retry button, continue-offline button, support text.

### Maintenance State (`MaintenanceState`)
- **Function:** Block online-only operations during maintenance.
- **Transitions:** Back/Home
- **State:** maintenance flag future
- **Backend:** remote config/backend
- **Logic status:** demo-local
- **Asset resources:** maintenance illustration, status panel, estimated-time text, retry/home controls.

### Empty / Error Variants (`EmptyErrorState`)
- **Function:** Reusable empty/error/retry state.
- **Transitions:** Retry/Home
- **State:** error context
- **Backend:** varies
- **Logic status:** demo-local
- **Asset resources:** empty-state illustrations, error icons, message panel, retry/back buttons, variant badges.

## Asset-production rule

When final design work resumes, each page must be rebuilt from the resources listed above as independent environment/character/UI/icon/VFX assets. Full-screen screenshots and large screenshot crops remain demo/reference-only and are not acceptable as final production assets.
