# World Map — Twenty-region design review R1

[Open interactive portrait proposal](WorldMap_Regions_Design_R1_Review.html). This is a browser design prototype, not a Unity implementation. All production Assets remain unchanged.

## Recommendation

Use **three complete chapter regions plus a partial fourth** at the reference portrait size. The larger landmarks and longer Cyrillic names remain easier to read at phone size. A four-region option is included for direct comparison. Swipe upward inside the map to move its content upward and reveal later chapters; chapter numbering increases from top to bottom. Keep header, title, Back and bottom navigation fixed. On reopening, center the next playable chapter and preserve manual scroll position when returning from a chapter.

Each chapter becomes a wide scenic region with its own landmark, chapter number, live name and state. This replaces the original fifteen small numbered nodes squeezed onto one view. It is an adaptation of the reference's Russian-fairy-tale world and materials, not a pixel-identical copy of the original composition. Regions are presented as a vertical illustrated journey, not a geographically accurate map of Russia.

| State | Proposed appearance | Intended behavior |
|---|---|---|
| Completed | Restored color, gold number, green/gold check, “Пройдено” | Open for replay |
| Active / available | Full-color scene, restrained gold highlight, “Доступно” | Open its chapter map |
| Future locked | Subdued color, visible landmark, separate gold lock, “Закрыто” | Explain preceding chapter requirement; no entry |
| Content not released | Lock plus future availability wording | Never route into absent level data |

Uncompleted does not automatically mean locked: the current playable chapter must remain open. The prototype progress selector is explicitly a visual fixture; it never changes the actual saved game. Header values are example data for the proposal.

## Confirmed chapter source

User confirmed `/home/k2/Downloads/Match3_Full_Story_and_Scenario.docx`. Titles and themes come from that document; no new chapter names were invented. Source hash/transcription retained in Sources/ChapterSource.json and Sources/Chapters.json.

| Chapter | Original title | English description from source |
|---|---|---|
| 1 | Родная Деревня | The Native Village |
| 2 | Густой Лес | The Dense Forest |
| 3 | Избушка на Курьих Ножках | Baba Yaga’s Hut |
| 4 | Царство Жар-Птицы | Kingdom of the Firebird |
| 5 | Молочные Берега | The Milky Shores |
| 6 | Подводное Царство | The Underwater Kingdom |
| 7 | Уральские Горы | The Ural Mountains |
| 8 | Сибирская Тайга | The Siberian Taiga |
| 9 | Северное Сияние | The Northern Lights |
| 10 | Москва Златоглавая | Golden-Domed Moscow |
| 11 | Северная Венеция | Northern Venice |
| 12 | Таврида | Taurida / Crimea |
| 13 | Кавказские Горы | The Caucasus Mountains |
| 14 | Священный Байкал | Sacred Lake Baikal |
| 15 | Дальний Восток | The Far East |
| 16 | Корни Мирового Древа | Roots of the World Tree |
| 17 | Ствол Мирового Древа | Trunk of the World Tree |
| 18 | Небесные Чертоги | The Heavenly Halls |
| 19 | Царство Чернобога | The Realm of Chernobog |
| 20 | Возрождение Руси | The Rebirth of Rus |

## Artwork and Scenario usage

- Five Scenario-generated vertical terrain strips provide twenty distinct regional scenes. They use the original World Map as visual guidance and the confirmed story document as location guidance. Each strip contains four regions; individual bands are saved as separate review assets. Generated pixels are not recolored; locked-state dimming is a separate preview treatment.
- Original Russian title/ornament and Map, Tasks, Guild medallions are extracted with Scenario Pixa, retaining original source RGB. Opaque medallion faces are restored where automatic alpha removed them.
- Home's approved source header art and existing embedded project fonts, Back and lock sprites are reused unchanged.
- Chapter labels, numbers, progress states and hit areas are separate browser elements. The full original reference is never used as the interactive map background.

**8 accepted Scenario jobs / 74 CU:** five terrain generations at13CU, three alpha extractions at3CU. No duplicate generation submissions. [API/billing audit](QA/ScenarioAudit.json) · [requests and exact prompts](Sources/Scenario/) · [asset provenance](Sources/AssetManifest.json).

The initial title crop clipped the crest. The expanded extraction then returned a truncated PNG on two downloads of the same asset. Only its validated complete upper rows are used for the crest; the complete initial extraction supplies the title body. Missing decoded rows are never used. Original source RGB is retained throughout. Recovery details in Sources/TitleRecovery.json. One earlier title download timeout was recovered with GET; no replacement generation was submitted.

## Review limitations and refinements to decide

The scenery is new concept artwork, not exact original terrain. Some regions include small narrative figures or legendary beings despite the scenery-only prompt; these are part of the paintings, not interactive characters. Chapter20 is particularly celebratory and busy. Before production, review whether those scenes should be simplified, and whether the map should retain these wide scenic regions or use freer territorial shapes. Terrain transitions between the five generated groups are not a seamless geographic atlas. Empty-title English localization, exact gold marker ornament and final region-boundary blending belong to the subsequent implementation pass. Russian source title/navigation lettering is shown in this review.

## Existing game and later implementation

The current native World Map has15 nodes and no regional scroll layout. Its ChapterAvailable logic explicitly permits Chapter1 only; the only authored catalog is Chapter1 with10 levels. The content pipeline reserves IDs chapter01–chapter20 but does not provide those later levels. The story's long-term round counts are design ambitions, not implemented content.

After design review, implement20 metadata-driven region entries in the owning World Map builder/prefab and a native vertical ScrollRect. Bind completion and active state to actual saved progression plus catalog availability; locked regions must not open missing chapters. Preserve the current router and existing chapter/story gates. Generate only remaining art repairs approved from this preview. Virtualize/recycle region views and load nearby art for Android memory control, reuse UI textures, and verify masks, safe areas, RU/EN names, all20 entries, Back/navigation and missing-content locks in Unity. Do not hard-code the preview's Chapter3-active state into production.

## Verification

Browser checks passed: all20 entries, completed/active/locked counts, unlocked selection preview, locked explanation, all-completed fixture, scroll-to-end, both density options, image/font loading, label width, 390px viewport without horizontal page overflow, no JavaScript page errors. Eleven standalone portrait captures retained, explicitly offline browser renders. Source UI transparency inspected on black, white and magenta. All five generated strips inspected. [Browser results](QA/BrowserChecks.json) · [production preservation](QA/ProductionPreservation.json).

No production code/assets/prefabs changed; no Unity compilation/runtime test, Android build or phone test was performed for this design-only pass.
