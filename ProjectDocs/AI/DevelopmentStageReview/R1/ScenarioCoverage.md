# Scenario coverage and omissions

Reviewed 26 September 2026 against `Match3_Full_Story_and_Scenario.docx` and `Match3_Legends_of_Rus_Scenario_Document.docx`. Extracted text is retained under `Sources/`. The project copy of the full story has the same SHA-256 as the user-designated Downloads file. The world-map `chapters.json` names and narrative paragraphs follow the full story's 20-region order. Reference UI text is not the campaign canon.

## Coverage by chapter

All 20 chapters have world-map presentation definitions. Only Chapter 1 has an active runtime level/story catalog. “Missing” here means missing from the playable campaign, not that no related illustration, prototype, test or reusable engine capability exists.

| # | Canonical chapter | Playable story coverage / principal missing work |
|---|---|---|
| 1 | Родная Деревня | 10 levels; 8 scenes. Wounded Firebird, raven, elder, fading embers, river shadow and finale are present. Missing 490 rounds, expanded pacing, restoration progression and actual transition to Chapter 2. |
| 2 | Густой Лес | Missing playable chapter, Leshy illusions/moving trees, corrupted clearings, night survival sequence and sacred-oak reward. Existing survival goal alone does not implement this encounter. |
| 3 | Избушка на Курьих Ножках | Missing Baba Yaga's three trials, tests of character, mortar/pestle reward and continuing mentor role. |
| 4 | Царство Жар-Птицы | Missing sister rescues, dark vines, corrupted queen encounter and visible restoration of the Firebird realm. |
| 5 | Молочные Берега | Missing Vasilisa introduction, quieter pacing, friendship/romance choices and magic-doll unlock. |
| 6 | Подводное Царство | Missing Sea Tsar story, floating/current lanes, pearl collection, rescue/boss sequence. Current Sea Pearl grants five moves; document whether that replaces the story's time-freeze concept. |
| 7 | Уральские Горы | Missing Ilya mentoring, strength progression, stone/platform mechanics and golem encounter. Existing chains are reusable, not a complete chapter. |
| 8 | Сибирская Тайга | Missing Bear spirit/cub rescue, blizzard pressure, loneliness and recovery beats. |
| 9 | Северное Сияние | Missing Morozko/Snegurochka story, dynamic freezing rules and lieutenant encounter. Existing ice blockers are a foundation. |
| 10 | Москва Златоглавая | Missing siege, bogatyr reunion, multi-character play and mid-campaign climax. |
| 11 | Северная Венеция | Missing spirit-ball/canal story and companion relationship consequences. |
| 12 | Таврида | Missing prophecy, ancient memory and Chernobog vision. |
| 13 | Кавказские Горы | Missing honor/hospitality choices, new ally and vertically scrolling gameplay boards. A scrolling chapter map is not this mechanic. |
| 14 | Священный Байкал | Missing surface/underwater combination, cleansing, Master of Baikal and revelation of human responsibility for the wound. |
| 15 | Дальний Восток | Missing eastern alliances and multi-headed Zmey encounter. A legacy Chapter15 route is not authored Chapter 15 campaign content. |
| 16 | Корни Мирового Древа | Missing descent, fallen protectors and the consequential sacrifice choice. |
| 17 | Ствол Мирового Древа | Missing returning-allies state, collaborative board rules and ascent sequences. |
| 18 | Небесные Чертоги | Missing council, blessing, ultimate Living Light progression and final army reveal. |
| 19 | Царство Чернобога | Missing separated/weakened companions, corrupted revisits and Koschei's multi-stage nested-container encounter. |
| 20 | Возрождение Руси | Missing finale, reunion, Chernobog's rebalancing, restored World Tree, village epilogue and postgame state. |

## Omissions in the scenario specification itself

The documents provide an evocative arc, but are not yet an implementable campaign specification. Before mass authoring, resolve these points explicitly:

1. **Scope and pacing.** Replace the old 10,000+ rounds/chapter assumption with the approved 500. Define main path vs optional/seasonal content, ten 50-round sectors if accepted, story milestone density, difficulty/recovery cadence, finale and unlock criteria. A chapter map section is not automatically a story act.
2. **Character chronology.** The current Chapter 1 opens with Dobrynya as mentor; the full scenario emphasizes the Firebird discovery and the later gathering of the three bogatyrs. Decide whether early Dobrynya is intentional foreshadowing, a prior adaptation, or should change. The shift from active bogatyrs at Moscow to luminous spirits at the roots also needs an explicit explanation. Preserve existing accepted scenes until this decision is made.
3. **Antagonist continuity.** Specify who the black raven serves and how clues lead through Leshy, Koschei, Zmey and Chernobog. The scenario names Nightingale the Robber but needs a clear chapter/encounter assignment. Explain prophecy references to the roots versus the final realm, and preserve the ending's restoration of balance rather than silently changing it into simple destruction.
4. **Player identity.** The scenario offers name and male/female appearance/voice choices; runtime saves and presentation currently use Ivan. Define the initial character selection, alternate portraits, pronouns, localization and compatibility with existing saves—or explicitly defer customization from MVP.
5. **Choices and relationships.** Define choice IDs, conditions, consequences and rejoin points for Vasilisa and moral/sacrifice decisions. The present story schema supports linear lines/triggers, not a choice graph or relationship variables. Avoid giving a cosmetic choice a promised gameplay consequence that does not exist.
6. **Companions and ability unlocks.** Specify acquisition, availability, upgrades and loss/recovery rules for Firebird, Vasilisa, Yaga, bogatyrs and regional allies. Inventory starts with all four working boosters; their counts do not prove story-earned unlocks. Separate consumable tools from on-board match-created specials. Resolve sacred branch, mortar, doll, pearl and bogatyr power into a single documented ability catalog.
7. **Mechanics and bosses.** Assign exact board rules, goal types, turn limits, phase transitions, telegraphs, win/fail conditions and replay behavior to each new mechanic. Rescue, cleanse, memory and boss level labels are not sufficient specifications. Define whether survival is move-based or timed for the night sequence; define currents, platforms, nested objects, multi-head attacks and scrolling playfields.
8. **Restoration state.** Define persistent corruption/restoration stages, when scenery and audio change, and how the world map reflects restored areas. Current chapter art and VFX do not establish a reactive world-state system. Reuse approved assets and create variants only for an agreed state requirement.
9. **Narrative presentation.** Specify intro, mid-level interruption, post-result reaction, cutscene, chapter finale and epilogue routes. Current native Victory/Fail deliberately skip the legacy reaction overlay. Decide a suitable visible location for the existing 20 reactions without covering approved result art.
10. **Localization and audio.** Complete the 23 missing English reaction/completion keys, unify region titles (the Chapter Complete teaser uses “Mystery of the Forest” while the map uses “The Dense Forest”), and create a glossary for Living Light/Flame and booster identities. Define chapter music, voice-over scope, subtitles and audio fallbacks; existing game SFX do not cover a 20-chapter soundtrack/VO production.
11. **Rewards, failure and recovery.** Specify first-clear/replay grants, chapter bonuses, optional objectives, energy/retry policy, booster restoration after interrupted actions, and once-only story rewards. Current chapter-complete display summarizes already earned rewards; it does not issue an extra chapter bonus. Free retry exists and needs an explicit product decision.
12. **Postgame and release boundary.** Specify what is playable after Chapter 20 and what is deferred: endless realm modes, seasonal revisits and 50–200-level Side Legends. Set measurable acceptance for each chapter instead of promising all future modes as shipped.

## Suggested chapter production contract

For each chapter record: stable ID; RU/EN canonical names; narrative entry/exit state; 500 stable round IDs grouped into approved sectors; installed/published availability; scene/choice/milestone IDs; characters and abilities unlocked; board mechanics and teaching levels; difficulty bands and rewards; finale/boss rules; restoration art/audio stages; localization keys; content version and dependencies; save-migration rules; authoring/balance/QA acceptance.

Keep this authoritative data separate from preview art text. Generate map labels and completion teasers from the same source to prevent drift. First reconcile Chapter 1's accepted ten-level slice with the expanded contract; do not rewrite all twenty chapters or generate thousands of boards before the contract is validated.
