# Development stage review — 26 September 2026

## Assessment

**The project is a visually advanced, locally playable vertical slice. It is not yet a complete 20-chapter campaign or a connected commercial game.** The next phase should focus on campaign architecture, authored content and device acceptance, while preserving the approved visual system.

Verified from the working tree: Unity **6000.5.9f1**, **36 native page prefabs**, **20 world-map region definitions**, **one active level catalog containing 10 levels**, and **one story catalog containing 8 scenes, 7 milestones and 20 outcome reactions**. A 500-position chapter map is implemented; positions beyond the authored levels are not playable content. Against the approved 20 × 500 scope, 9,990 rounds remain unauthored in the active catalogs. This is a content count, **not a project-completion percentage**.

This review inspected production source, configuration, the two scenario documents, and existing test/build evidence. It did not rerun Unity, test a phone, or independently reaccept every page's visual fidelity. All existing artwork, prefabs, settings and approvals are preserved.

## Feature status

“Implemented” below means working within the current local slice, not release acceptance. “Preview” means a native, navigable screen exists but its apparent service is not live. “Foundation” means contracts/tools exist without production activation.

| Area | Current state | Remaining work |
|---|---|---|
| Visual system and navigation | Implemented locally: 36 native prefabs, shared header where applicable, isolated approved art, RU/EN presentation, page-specific responsive/touch passes | Physical-device text, safe-area, touch, keyboard and performance acceptance; source-image lettering retains resolution limits |
| Core journey | Implemented: Home → maps → intro → Pre-Level → Gameplay → Victory/Fail → return, plus Pause/Settings | Full campaign transitions, all-level balance coverage, device interruptions |
| Match-3 engine | Implemented: matching, cascades, row/column/burst/color specials and combinations; deterministic seeded board infrastructure | Large-content balance and solvability testing; chapter-specific mechanics and boss progression |
| Goals and obstacles | Implemented: collect gems, score, clear obstacles, bring down feathers, survival; ice, chains and crates | Moving currents, rescue-specific logic, multi-phase bosses, scrolling gameplay boards and other scenario mechanics |
| Boosters | Four functional tools: Mace (row/column), Firebird (3×3), Living Light (color), Sea Pearl (+5 moves) | Luck Scroll, Lightning Orb, Veles Bomb and Swap remain explicitly labelled previews; decide unlock cadence and reconcile story/art terminology |
| Effects and audio | Implemented local cues: gem shards, bursts, line sweeps, special/booster activation, cascades, audio and haptics | Phone performance/feel acceptance; chapter-specific music and environment restoration; no verified claim of Royal Kingdom quality parity |
| Campaign | Partial: 20 illustrated regions; Chapter 1 has 10 playable levels; 500 map slots are virtualized in 50-round sections | Versioned multi-chapter loading, 500-round contract, chapter unlock/transition, content production and migration |
| Story/dialogues | Partial: Chapter 1 linear scenes, milestones, reactions and completion state | Chapters 2–20 scripts, choices, relationship/companion state, restoration stages, boss/epilogue integration |
| Player progression | Local level records, scores, stars, attempts, first-clear rewards and profile history implemented | Campaign-wide progression, hero identity, ownership/upgrade/ability systems, relic earning/equipping |
| Inventory | Saved counts for four working boosters, browsing/details and gameplay use | Production acquisition/economy and gameplay behavior for preview collectibles/tools |
| Energy and rewards | Local recharge, spending, first-clear reward protection and daily reward claims implemented | Trusted server time/fulfillment if commercial or social; settle free-retry policy; chapter bonus is not currently an extra grant |
| Save/lifecycle | Versioned local journal, recovery/migration checks, separate-process persistence verified | Cold-start unfinished-board recovery is absent; cloud backup/account recovery absent; real OS suspension/force-kill pending |
| Settings/language | Local music/SFX/haptics/notification preference and RU/EN selection saved | Notification preference is not proof of push delivery; complete story localization and connected account/support/legal routes |
| Login/account | Local guest flow; online authentication explicitly unavailable | Provider integration, cloud progress, account linking/recovery/deletion |
| Rankings, Teams, Friends, Mail | Native previews; local friend search/sort and local-session chat work | Authoritative standings, membership, network messaging, moderation, invitations, energy gifts, mail delivery and attachment claims |
| Events/Event Detail | Native preview, sample progress/rewards and availability messaging | Event scheduling, real goals/scoring, eligibility, anti-duplication and rewards |
| Characters | Ivan current; hero cards, rarity filters and details browseable | Ownership, changing active hero, upgrades, stats and team bonuses are explicitly unavailable |
| Shop/Special Offer/rewarded actions | Screens and disabled service boundaries exist | Approved providers, products, receipt verification, durable grants, consent, failure/retry handling; no live purchase/ad revenue |
| Offline/Maintenance/Error | Local UI/routing coverage | Real service health/maintenance integration; gameplay failure recovery and physical network-loss validation |
| Content/LiveOps/release | Extensive local contracts, validators and guarded workflow foundations | Activation is disabled; staged packages are not runtime chapter delivery; providers, operational evidence and release approval remain |
| Android | Development ARM64 IL2CPP APK built; 99 policy tests and static package checks passed | Device installation and acceptance, memory/FPS/startup/thermal profiling, install/update compatibility, release packaging |

## Highest-priority findings

### 1. Three different content-size assumptions must be reconciled before expansion

- Approved product target: **500 rounds per chapter, 20 chapters**.
- `LevelDataValidator` and `Chapter01ProgressionValidator` enforce **exactly 10 levels** for Chapter 1. `chapter01.json`, story finale and staged CH01 definition also end at 10.
- `ChapterPipelinePolicy.MaximumLevelCount` is **100**, and the migration dry-run policy deliberately pins the legacy Chapter 1 package to 10.
- The source scenario describes **10,000+ rounds per chapter**. Treat that as superseded by the user's later 500-round instruction, and record the decision in the canonical specification.

Do not merely append 490 JSON levels or change one constant. Introduce a versioned campaign contract and compatible save/content migration. Keep the ten-level demo fixture and its historical acceptance distinct from the new campaign. Do not relocate an already acknowledged finale without an explicit migration rule.

### 2. Multi-chapter runtime loading is not implemented

`ChapterLevelCatalog.Find/ResolveSelected/Next` load Chapter 1. `StoryCatalog` also loads Chapter 1. World Map deliberately refuses chapters other than 1. Profile completion uses the Chapter 1 catalog; saved progress normalization is tied to that catalog. The existing content manifest includes legacy `DesignScreens` addresses and a `Chapter15` route; neither proves Chapter 15 gameplay exists.

Required: stable chapter/level IDs, per-chapter availability and completion, chapter transitions, resumable selected location, content-version checks, migration tests, and a loading strategy compatible with approved assets. Enable future chapters only when their packages are valid and installed.

### 3. Story coverage and localization are incomplete

All current scene/milestone/reaction references and Russian localization keys resolved in the static audit. English covers current dialogue speakers and lines, but **23 catalog keys are absent: 20 outcome reactions and 3 completion strings**. `StoryService` loads Russian for reactions and completion.

There is a second presentation gap: `ScreenRouter.BuildOutcomeReactionOverlay` exits for native Victory/Fail pages. Therefore the authored reaction data does not automatically appear there. Native Chapter Complete supplies its own RU/EN text. This is a service/data coverage omission, **not evidence that every current English result screen shows Russian**. Decide where outcome reactions should appear, then localize and test that actual route.

### 4. Polished social and commercial pages are not connected services

Preview labels are accurate. Friends/Team/Ranking/Mail/Event data is sample or session-local. Monetization defaults to a disabled port; commercial, delivery and other production activation manifests remain off. Existing modules and hundreds of historical policy artifacts are foundations, not evidence of a functioning backend or an active release.

### 5. Android acceptance and content footprint remain open

The built APK is **1,380,231,718 bytes (about 1.29 GiB)**, for the current 10-level slice with broad UI/art coverage. Signature, ZIP integrity, alignment and build checks passed. No device was attached during the last device audit; no physical acceptance is recorded. Measure installed size, startup/loading time, peak memory, frame pacing, heat and long-session behavior before scaling content. Investigate duplicate/legacy resources, texture imports and loading residency; preserve approved artwork and do not enable remote delivery merely to hide size issues.

## Verification confidence

Existing result XML files were re-read during this audit:

| Evidence | Recorded result | Limits |
|---|---|---|
| Core Journey R1 | 6/6 PlayMode | Actual legal level-one win/fail paths; not all ten levels or a phone |
| Persistence/Lifecycle R1 | 21/21 across writer, reader and regression runs | Editor processes and simulated lifecycle; no OS force-kill/unfinished-round restoration |
| Text Clarity R1 | 24/24 across writer, reader and regression runs | Shared-scale fix and representative captures; not every asset/device |
| Android Device R1 Verify02 | 99/99 policy EditMode; APK build/static verification passed | Not physical play, release acceptance or a current whole-project suite |

These runs overlap and must not be summed as unique full-suite coverage. Current review ran **nine static inventory/reference checks**, all passing, and recorded the 23 missing English keys as findings. It did not change production code or claim a new Unity compilation.

## Recommended next phase

1. **Campaign foundation and scenario contract (next implementation milestone).** Consolidate the 20×500 decision, names, stable IDs, chapters/sectors, authored vs planned availability, finale placement and save migration. Generalize loading/validation/profile/progression without invalidating accepted demo saves. Acceptance: existing ten levels still work; a validated Chapter 2 test package can transition and restore correctly; unavailable content stays unavailable; 500-round schema tests pass. New production content migration/activation remains a separate reviewed action.
2. **A complete first 50-round sector before mass production.** Use the existing 50-round map section as an initial delivery unit. Author varied boards, goals, difficulty peaks/recovery, story beats and rewards; connect tutorial explanations to the exact working booster rules. Review playtest/balance results before scaling to 500. This sector breakdown is a recommendation, not an already approved story beat schedule.
3. **Chapter 1 completion and Chapter 2 continuity.** Build the remaining Chapter 1 sectors with paced narrative, then Chapter 2's Leshy/forest mechanic and genuine unlock flow. Test all ten sectors, replay, save migration, absent packages, terminal state and repeated reward attempts. Chapter 2 content can be prototyped earlier to prove architecture without presenting it as campaign-ready.
4. **Android acceptance alongside content work.** Use the existing APK/checklist once a phone is connected; profile and optimize proven bottlenecks. Decide whether interrupted rounds should resume after cold restart and implement that policy before external testing.
5. **Connected product systems, in dependency order.** Identity/cloud and durable reward ownership first; then friends/teams/chat/mail/rankings, then scheduled events and monetization. Keep previews explicit until each service is proven. Define MVP inclusions rather than activating every foundation simultaneously.
6. **Closed test and release readiness.** Fresh full regression, campaign balance, accessibility/text/device coverage, operational/privacy/support readiness, release identity/version/signing and store delivery. No release or monetization activation is authorized by this review.

**Immediate recommendation: Campaign Foundation R1 — 20-chapter/500-round contract, multi-chapter progression design, and save-migration specification.** Keep the approved UI; move development effort to the systems and authored content that make it a full game.

See [Scenario coverage](ScenarioCoverage.md), [review dashboard](Development_Stage_Review_R1.html) and [machine-readable inventory](QA/Inventory.json).

## Evidence index

- [Active levels](../../../../Assets/LegendsOfRus/Resources/Config/chapter01_levels.json), [active story](../../../../Assets/LegendsOfRus/Resources/Config/chapter01_story.json), [regions](../../../../Assets/LegendsOfRus/Resources/ProductionUI/WorldMapReferenceAligned/chapters.json).
- [Level catalog/validator](../../../../Assets/LegendsOfRus/Scripts/Runtime/Gameplay/LevelData.cs), [Chapter 1 progression validator](../../../../Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Chapter01ProgressionValidator.cs), [pipeline policy](../../../../Assets/LegendsOfRus/Scripts/ContentPipelineCore/ChapterPipelinePolicy.cs), [migration policy](../../../../Assets/LegendsOfRus/Scripts/ContentPipelineCore/ChapterMigrationPolicy.cs).
- [GameState/save fields](../../../../Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs), [story service](../../../../Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs), [router](../../../../Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs).
- Runtime UI `Production*View.cs` source for each named page explicitly describes preview/availability behavior; [local demo state](../../../../Assets/LegendsOfRus/Scripts/Runtime/UI/DemoFeatureState.cs).
- [Commercial activation](../../../../Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json), [content pipeline activation](../../../../Assets/LegendsOfRus/Resources/Config/content_pipeline_activation.json), [delivery activation](../../../../Assets/LegendsOfRus/Resources/Config/content_delivery_activation.json), [monetization facade](../../../../Assets/LegendsOfRus/Scripts/Runtime/Services/MonetizationService.cs).
- [Core journey](../../CoreJourneyAudit/R1/ImplementationReport.md), [persistence](../../PersistenceLifecycleAudit/R1/ImplementationReport.md), [text](../../TextClarityAudit/R1/ImplementationReport.md), [Android](../../AndroidDeviceAudit/R1/ReadinessReport.md).
