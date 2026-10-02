# Feature and page coverage

Status meanings: **local** = working local implementation; **partial** = working subset with missing behavior; **preview** = native UI using samples; **foundation** = contracts/validation/development composition without a live service; **missing** = requested behavior not delivered; **unverified** = no adequate acceptance evidence. These labels do not certify release readiness.

## Systems

All paths below are relative to the project root. Current source inventory: [QA/CurrentInventory.json](QA/CurrentInventory.json).

| Feature | Status | Implementation/evidence | Missing / acceptance work |
|---|---|---|---|
| Startup, loading, routing | Local | Runtime core/router; native Loading/SplashTitle; navigation and lifecycle audits | Current device cold/warm start and recovery; preserve approved Splash |
| Match engine and round loop | Local | `Scripts/GameplayCore`, `Runtime/Gameplay/Match3Board`; active 200-round witness evidence | 23 R43 board/input/motion fixture failures need explicit fixture dimensions and intended rules reviewed |
| Generated match items and combinations | Local, visual polish deferred | New rules integrated in normal gameplay; GameplayDesignIntegration/R1 | Final creation, activation, combo and gem-shatter visual acceptance; five obsolete VFX expectations remain open |
| Obstacles | Local for existing types | Layered ice/chains/crates; approved artwork and damage-state presentations | Chapter-specific bosses, nested objects, moving/current obstacles; no restoration of unwanted obstacle numerals |
| Goals | Local | Active data uses collect_tile, score, clear_obstacle, bring_down, survival | Timed/boss/nested-object/scrolling-board mechanics are not delivered by those existing goal types |
| Four inventory boosters | Local | `GameState.BoosterType` and gameplay consumption: Mace, Firebird, LivingLight, SeaPearl | Four other BoosterSelect tools remain previews; pre-level equipment differs from in-round consumption |
| Fail retry / quit | Local | Native Fail and router; new-round retry | Same-attempt paid/ad continuation missing; loss result is not a resumable board |
| Paid +5 / ad continuation | Missing behavior, UI present | `DeferredFeatures/FAIL-CONT-01.md`; disabled Fail offer controls | Snapshot, interruption recovery, configured price/limits, spend+grant atomicity, verified provider callbacks |
| Chapter progression | Partial | ActiveCampaignContent, ValidatedCampaignCatalog, CampaignFoundation; 20×500 definition | Only Chapter 1:1–200 installed; chapters 2–20 and finale not playable |
| Expanded campaign save/lifecycle | Foundation plus local integration | CampaignSaveSession, CampaignRoundLifecycle, CampaignGameplayCoordinator, migration tests | Explicit development composition is not proof every normal path uses a new multi-chapter save; reconcile account/cloud authority before rollout |
| Story milestones and reactions | Local | StoryService / CampaignStoryFlow; 28 scenes, 27 milestones, 400 reactions | Later narrative, choices/relationship ledger, story history and optional catch-up recap |
| Local save and settings | Local | SaveCore journal, SaveService, GameState, LanguagePreference; separate-process R43 tests | Cross-device identity, cloud conflict rules and recovery; server must not trust local currency balances |
| Energy and daily rewards | Local subset | GameState refill / DailyRewardSchedule with saved claims | Monthly bonuses 7/14/21/28 undefined; ad refill unavailable; server time/claim authority for live economy |
| First-clear rewards and replay | Local | Completion result and claimed markers, campaign reward tests | Preserve no-double-grant semantics through account migration and authoritative wallet |
| Commerce adapters / consent / analytics | Foundation, disabled | CommercialCore + CommercialRuntime; manifests false; fresh focused tests | Live consent collection, approved destinations/providers, actual analytics transport and verification |
| Purchase / ad fulfillment | Foundation, not durable | CommercialFulfillmentCoordinator / CommercialEntitlementRecipeCatalog | All IAP recipes absent; ad-energy recipe absent; reward double candidate-only; receipt ledger, atomic grant and recovery missing |
| Live accounts / cloud | Missing | Native Login guest entry, compliance policy helpers | Real identity/linking/recovery/deletion, local-to-cloud merge, published legal destinations; AUTH-ACCOUNT-01 |
| Rankings | Preview | ProductionIndividualRankingView / ProductionTeamRankingView | Trusted score submission, season boundaries, ties, pagination, personal rank and payout authority |
| Friends | Preview with local search/sort | ProductionFriendsListView / Runtime | Mutual relationships, invitations, block/report, real energy gifts and rate limits |
| Teams and chat | Preview with local composer | ProductionTeamHubView / ProductionTeamChatView; DemoFeatureState | Server membership/roles, durable messages, moderation/report/block, gifts, team tasks and chest claims |
| Events and pass | Preview | ProductionEventsView only first card opens preview; EventDetail pass disabled | Server schedule/eligibility, playable event content, progress and claim ledger, reconnect/expiry |
| Mail | Preview | ProductionMailView explicitly states no send/delete/claim service | Authoritative inbox, attachment claims, expiry, Claim All and durable replay protection |
| Profile / relics / achievements | Partial | Native Profile, history/refinement artifacts | Account-backed identity and authoritative relic/achievement ownership/unlocks; preview art is not acquisition |
| Hero collection | Preview | ProductionCharactersView, eight browsable cards, future placeholders | Ownership, equipped hero, upgrades/stats, story unlocks and bonuses; HERO-COLLECTION-01 |
| SMS verification | Missing | Explicit deferred CHAPTER-SMS-01 request | Purpose/trigger/optional behavior, backend/provider, code expiry/retry; separate from testing on a phone |
| Remote content / LiveOps | Foundation | Addressables package, ContentPipeline and LiveOps evaluator families | Hosted delivery, signed/versioned compatible content, download/recovery/rollback and live operations validation |
| Audio / haptics | Partial | PrototypeBoardAudio uses MechanicsAudioR11 cues; PlatformHapticProvider; saved toggles | Full chapter score/ambience/voice coverage and device mix/haptic acceptance not established |
| Notifications / support / legal | Partial presentation | Settings and saved preference; Login explicitly reports unpublished legal documents | Real notification delivery/scheduling, support and published policy destinations; toggle persistence alone is insufficient |
| Android / release | Historical dev build only | AndroidDeviceAudit/R1 | Current-content build, real hardware, network/failure/provider performance, release packaging and distribution |
| Automated QA / manual balance | Partial acceptance | R43 1,917 Edit Mode passes; runtime 857/40 latest-case aggregate | Clean combined runtime suite; human 31–200 balance; fresh broad visual and Android acceptance |

## All 36 native pages

Native prefab presence confirms construction, not completion of its business feature. Current inventory is under `Assets/LegendsOfRus/Resources/ProductionUI/Pages`. `ScreenId` has an additional legacy `Chapter15` route; it is not a 37th newly completed production page.

| Page(s) | Working scope | Open behavior / verification |
|---|---|---|
| SplashTitle, Loading | Accepted visual startup and transition | Preserve art; device startup timing/memory |
| Home, WorldMap, ChapterMap | Navigation, shared resource values, installed-round selection | Future chapter nodes are not content; current map/phone verification |
| PreLevel, Gameplay, Pause | Goals, current rounds, four tools, pause/resume/retry/navigation | Equipment previews, final effects, retained failed attempt |
| Victory, Fail, ChapterComplete | Result/reaction presentation, rewards/first-clear handling, retry/continue routes | Ad double, paid/ad fail continuation; active Chapter 1 has no installed finale |
| Shop, SpecialOffer, OutOfEnergy | Native offers and local energy presentation | Real purchase/ad inventory, prices and verified grants |
| DailyReward, RewardClaim | Seven-day local claim and reward presentation | Monthly payouts and live claim authority |
| BoosterSelect, Inventory | Four real booster stocks and native details | Four previews, pre-level equipment rules, future item ownership |
| Events, EventDetail | Sample event cards/detail and disclosures | Active event play, pass, server progress/rewards |
| IndividualRanking, TeamRanking | Tabs, sample standings/rewards and native navigation | Real scores, seasons, ranks, payouts |
| TeamHub, TeamChat | Sample team/tasks, local chat composer | Multi-user membership/chat/moderation/help/chest |
| FriendsList, Mail | Sample browsing, friends search/sort, letter detail | Invitations/gifts/inbox mutations/attachment claims |
| Profile, Characters | Local progress/history, collection browsing | Live profile, relic/hero ownership and abilities |
| Settings, LanguageSelect, Login | Local preferences, RU/EN selection, guest entry | Notifications, real account/consent/legal integration; global localization acceptance |
| TutorialOverlay, Dialogue | Reference-based tutorial with gameplay gems, current story display | Instruction audit against current mechanics, choices and recap/history |
| OfflineState, MaintenanceState, EmptyErrorState | Native states and navigation/retry presentation | Real service outage/maintenance/retry behavior must be bound and tested |

Detailed acceptance and dependency order are in [LiveServicesFirst.md](LiveServicesFirst.md) and the [revised schedule](../../DevelopmentSchedule.md). Unchanged prior design approvals remain valid within their original scope.
