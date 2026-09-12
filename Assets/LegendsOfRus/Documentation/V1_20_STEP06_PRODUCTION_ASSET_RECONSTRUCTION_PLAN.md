# v1.20 Step 6 — Functional Acceptance Gate and Production Asset Reconstruction Readiness

## Decision

Step 6 freezes the Step 5 functionality baseline and prepares the production-art reconstruction lane. Static/source acceptance passes. Unity and Android executable acceptance are still required on the exact Step 6 bytes, so asset authoring preparation may proceed, but runtime production-art cutover remains blocked until those checks pass.

## Non-negotiable functional lock

Production art work must not change ScreenId routing, navigation parent policy, gameplay rules, save-v6 serialization, economy authority, backend activation, tutorial state ownership, pause continuity, or demo fail-closed commercial behavior. World Map Back remains `WorldMap -> Home`; Chapter Map Back remains `ChapterMap -> WorldMap`; Pre-Level Back remains `PreLevel -> ChapterMap`; Friends List Back remains `FriendsList -> TeamHub`.

## Runtime asset rule

Approved full-screen screenshots are **reference-only**. They may remain under `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/` for comparison, but they must not be used as composite runtime page images or fallback UI. Production pages are assembled from separate environment plates, character layers, 9-slice frames/panels/buttons, icons, live text, board/gem/obstacle sprites, and VFX inside Unity prefabs.

## Inventory at Step 6

- 36/36 pages have frozen functional + asset contracts.
- 11 approved reference-screen PNGs are present; 10 map directly to the 36 product surfaces and `Chapter15.png` is a chapter/style reference.
- 10 production page prefabs currently exist.
- 34 canonical reusable UI-kit PNGs exist.
- 38 reconstructed core-loop component PNGs exist across 6 pages.
- 11 approved gameplay sprites exist.
- Final visual implementation is therefore incomplete and must be reconstructed in controlled waves.

## Reconstruction sequence

### Wave 0 — shared foundation
Normalize the common UI kit first: panels, frames, buttons, top HUD, resource pills, back/close/pause controls, tabs/toggles, badges, reward tiles, modal blocker, icon conventions, Cyrillic typography, and common motion/VFX hooks. This wave reduces repeated page-specific work.

### Wave 1 — core loop and story spine (12 pages)
`SplashTitle, Loading, Home, WorldMap, LevelSelect, PreLevel, Gameplay, Pause, Victory, Fail, ChapterComplete, Dialogue`. These pages carry the highest gameplay and navigation risk and therefore establish the locked visual grammar and prefab patterns used by later waves.

### Wave 2 — meta, economy, and collection (9 pages)
`Profile, CharacterCollection, Inventory, Shop, SpecialOffer, DailyReward, RewardClaim, OutOfEnergy, BoosterSelect`. Reuse Wave 0/1 frames, buttons, currencies, reward components, and modal patterns.

### Wave 3 — events, rankings, and social (8 pages)
`EventsHub, EventDetail, IndividualRanking, TeamHub, TeamRanking, TeamChat, FriendsList, Mail`. Keep server-backed actions as current safe demo/fail-closed boundaries until real providers are separately approved.

### Wave 4 — settings, account, tutorial, and system states (7 pages)
`Settings, LanguageSelect, Login, TutorialOverlay, OfflineState, MaintenanceState, EmptyErrorStates`. Complete reusable system-state, language/account, and tutorial presentation after common components are stable.

## Per-page acceptance

Every reconstructed page must satisfy all of these conditions before replacing its demo surface: separate runtime assets; no screenshot composite fallback; live localized text; existing callback/control roles preserved; safe-area/aspect behavior preserved; source-to-destination transitions unchanged; no production backend activation; four target-aspect captures reviewed; and a navigation smoke test from every entry/exit route.

## Art technical profile

Use clean painterly environment plates for backgrounds, separate transparent character/creature layers, 9-slice-capable ornate UI frames and buttons, transparent icons/items/boosters, isolated board/gem/obstacle sprites, and separate VFX layers. Keep the locked Russian fairy-tale visual language: deep navy/black lacquer panels, antique-gold filigree, sapphire jewel accents, warm parchment, carved dark wood, painterly historical environments, embossed gold Cyrillic serif headings, and emerald primary CTAs.

## Gate execution in Unity

1. Open the exact Step 6 project in Unity 6000.5.9f1 and allow a clean compile.
2. Run `Legends of Rus > Tests > Run v1.20 Step 5 Functional QA Self-Tests`.
3. Run `Legends of Rus > Tests > Run v1.20 Step 6 Functional Acceptance + Asset Readiness Gate`.
4. Run the complete EditMode suite.
5. Run the complete PlayMode suite.
6. Execute `V1_20_STEP05_FULL_DEMO_FLOW_CHECKLIST.md` end-to-end.
7. On Android, verify the visible map Back control and the system Back gesture/button.
8. Only after every item passes, begin Wave 1 runtime production-art replacement.
