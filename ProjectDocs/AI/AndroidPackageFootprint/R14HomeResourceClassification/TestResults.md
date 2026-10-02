# R14 — Remaining Home resource classification, 2026-10-01

Completed the approved next scheduled task: consolidated read-only classification of all21entries in ProductionHomeRuntime.TextureNames. Verified current project instructions/handoff, dirty working tree and Unity6000.5.9f1(b57deb96f08d). Production files and the R13 APK remain unchanged. This audit identifies implementation dependencies; it does not archive assets or claim new Android savings.

## Findings and decisions

**No asset can be archived unchanged today:** all21missing-key probes cause fresh and legacy Home construction to throw. Current migration skips the legacy loader, but this does not establish fresh/migration safety. Current Home resource preparation separately uses HomeReferenceAligned resources and social shortcuts, excluding these21legacy keys.

Five resources have current enabled consumer states; keep them. Four others retain shared builders/control paths and remain outside the next cleanup scope. The other twelve can be considered for coordinated owner cleanup, including nine distinct dormant resource assignments on Home and one on WorldMap; two have no serialized consumer. This classification is not permission to remove all twelve as a batch.

| Legacy sprite | Decision | Recursive consumer roots | Prefab sprite Images | Eligible Image states |
|---|---|---:|---:|---:|
| nav_map_generated | Home owner cleanup required | 1 | 1 | 0 |
| nav_shop_generated | Home owner cleanup required | 1 | 1 | 0 |
| nav_events_generated | Home owner cleanup required | 1 | 1 | 0 |
| nav_characters_generated | Home owner cleanup required | 1 | 1 | 0 |
| mail_medallion_generated | Home owner cleanup required | 1 | 1 | 0 |
| reward_medallion_generated | Home owner cleanup required | 1 | 1 | 0 |
| home_logo | Retain: current consumer | 1 | 1 | 1 |
| avatar_portrait | Retain: current consumer | 20 | 25 | 1 |
| hud_frame | Home owner cleanup required | 0 | 0 | 0 |
| avatar_ring | Retain: shared builder | 15 | 42 | 0 |
| resource_capsule | Retain: shared builder | 1 | 3 | 0 |
| quest_panel | Home owner cleanup required | 0 | 0 | 0 |
| coin_icon | Retain: current consumer | 5 | 9 | 3 |
| gems_icon | Retain: current consumer | 5 | 6 | 3 |
| energy_icon | Retain: current consumer | 5 | 5 | 1 |
| settings_medallion | Retain: shared builder | 1 | 1 | 0 |
| resource_plus | Retain: shared builder | 6 | 18 | 0 |
| quest_reward_frame | Home owner cleanup required | 1 | 1 | 0 |
| quest_reward_gem | Home owner cleanup required | 1 | 1 | 0 |
| footer_base_generated | Home + WorldMap cleanup required | 1 | 1 | 0 |
| home_map_foliage | Home owner cleanup required | 1 | 1 | 0 |

Consumer roots are distinct recursive prefab/scene/asset/material/controller/atlas/etc. roots, not usage counts. Image counts use the sprite slot only; overrideSprite getter mirrors sprite in the absence of an override, so its observations are not counted twice. Eligible Image state means enabled, active within its local serialized hierarchy and positive Image/ancestor CanvasGroup alpha. It is **structural evidence**, not a screenshot, clipping/occlusion check or Play Mode measurement. Loaded prefab assets report activeInHierarchy=false even when their serialized hierarchy is active; activeWithinRoot is recorded separately. Every resource's references, states, dependencies and source sites are retained in [Classification](QA/Classification.json) and [Unity inventory](QA/EditorAccepted/PrefabImages.json).

**home_logo** — Login uses an enabled ApprovedLogo; Home source_logo is a separate replacement.

**avatar_portrait** — Gameplay keeps the enabled Ivan portrait; IndividualRanking also has a dynamic fallback. Do not remove based on Home replacement.

**coin_icon** — Live PreLevel rewards, SpecialOffer preview and DailyReward day1; Victory refresh also requests it dynamically.

**gems_icon** — Live PreLevel gem reward/goal, SpecialOffer preview and DailyReward day2; Victory refresh also requests it dynamically.

**energy_icon** — Live DailyReward day3; additional PreLevel/Home/shared builder and hidden Fail symbol loads remain.

**avatar_ring** — Retained on15dependency roots; shared/profile constructors and NavigationStyle fallback still load it. No eligible rendered-image state observed in audited final pages; this does not establish removal safety.

**resource_capsule** — Three dormant Victory Images; used by Victory/Shop/SpecialOffer/DailyReward constructor phases, including monthly rail before current composition. Retain until those phases are coordinated.

**resource_plus** — 18retained Images across6dependency roots; shared builders and live-target/control migration must be reviewed before clearing. No eligible final Image state observed in this audit.

**settings_medallion** — One dormant Home Image; Home/Shop/DailyReward constructors still require/load it before current shared header. Retain for a separate coordinated shared-control task.

**footer_base_generated** — One disabled, inactive historical WorldMap/WORLD_MAP_ALIGNED_CONTENT/FooterOrnament Image. The public view.FooterOrnament field points to current WorldMapReferenceAligned/footer_backing. Home replaces its footer before return; fresh WorldMap clones Home source_footer_carving. Clearing only the historical Image on a disposable installed map preserves bindings/components/targets under its current owner.

**hud_frame** — No serialized consumers or Image assignments in completed pages. Home initially creates/assigns its Hud/HomeHud ReferenceArtwork, then ProductionPlayerHeader replaces the frame with source_header synchronously. Home strict loader and historical HasGeneratedControlBindings remain.

**quest_panel** — No serialized consumers or Image assignments in completed pages. Home initially constructs/aligns QuestFrame, then source composition assigns source_quest_ru/source_quest_blank. Home strict loader and historical HasGeneratedControlBindings remain.

The six old navigation/mail/reward sprites each remain on one disabled Home Image after source composition assigns the separate current artwork/targets. quest_reward_frame and quest_reward_gem each remain on one disabled Home Image; home_map_foliage remains on one disabled Home Image. Current migration may reactivate their GameObjects while leaving Graphics disabled; do not infer drawing from GameObject activity alone. Sprite-only clearing on a disposable Home instance preserves all components and Selectable targets under BuildInto and current source migration. Legacy migration still reloads them through the strict21-entry constructor list, so owner alignment is required before any archive.

## Source and shared-path trace

[Static inventory](QA/Assets.json) records each actual PNG/header/dimensions, GUID, PNG/meta hashes, importer metadata and actual R13 packed rows. [References](QA/References.json) scans5,624text files under Assets/Packages/ProjectSettings using exact GUID/name boundaries. [Loader sites](QA/LoaderSites.json) records dynamic loading/current key preparation and historical diagnostic sites.

Reviewed Home LoadSprites/BuildInto/UpgradeReferenceForeground and source Apply/Refresh; shared ProductionPlayerHeader and map/ChapterMap/PreLevel clone construction; NavigationStyle's conditional old branches; Home wrappers in Gameplay, Victory, Shop, SpecialOffer and shared paths in DailyReward/IndividualRanking/Login/Fail. Currency arrays and Victory refresh calls remain valid dynamic consumers even when old header Graphics are disabled. Gameplay's Resources.LoadAll targets its separate atlas/sheet root; no HomeAligned bulk load was found. Literal-reference absence is not proof of dynamic non-use.

Historical HasGeneratedControlBindings and HasReferenceBackdropBindings are definitions with no current external source call sites. Current HasRequiredBindings/HasReferenceForegroundBindings use the active source contract. Review those historical diagnostics during owner cleanup without inventing an active consumer.

Inspected actual existing footer_base_generated and quest_panel PNGs; no design, transparency, color, scaling or import modification. No generated replacement art or design-review page.

## Unity execution and verification

| Run | Outcome | Meaning |
|---|---|---|
| EditorAudit | Exit1; no NUnit result | Sandbox prevented Package Manager IPC/server startup; no tests ran |
| EditorVerified | 35passed,0failed/skipped | Initial consolidated inventory and builder checks |
| EditorFinal | 36passed,1failed,0skipped | Expanded37-case audit; WorldMap probe incorrectly selected live public footer field |
| EditorAccepted | **37passed,0failed/skipped** | Complete rerun after selecting the historical FooterOrnament object explicitly |

The expanded fixture also records activeWithinRoot to interpret serialized prefab state correctly. Its WorldMap selection failure was a temporary audit mistake: expected old HomeAligned footer but selected current WorldMapReferenceAligned footer_backing. Corrected only the fixture to locate WORLD_MAP_ALIGNED_CONTENT/FooterOrnament and require that it differs from view.FooterOrnament. This was not a production failure or production fix. Initial/expanded fixture sources are retained under Sources; the final source is [here](Sources/HomeResourceClassificationEditModeTests.cs).

Final37distinctcases cover:

- 21resource inventories and missing-key denials in both fresh/legacy Home (42throw probes), with recursive consumers among99dependency roots each.
- 3 Home construction modes: fresh/legacy request21legacy keys, current migration0; all bindings valid and image states recorded after synchronous composition.
- 10 shared-page cases, each testing fresh and installed WorldMap, ChapterMap, PreLevel, Gameplay, Fail, Shop, SpecialOffer, DailyReward, Login and Victory bindings and image states.
- 1 Image-reference inventory across all72prefab roots.
- 2 disposable sprite-clearing checks: Home current migration and WorldMap historical footer preserve components/live targets and keep cleared sprites null.

[Accepted exact command/NUnit summary](QA/EditorAccepted/summary.json) and [XML](QA/EditorAccepted/results.xml). Runs are sequential batch/nographics Edit Mode with isolated temporary player configuration; no scene/prefab/asset saves. Initial sandbox IPC failure was resolved using reviewed escalation for the same authorized audit. No package upgrade, settings/graphics switch, guard bypass or network research. Temporary test source/meta removed after every run; no permanent test changes. Raw Unity logs/configuration under /tmp/lor-home-resource-classification-r14, outside the authored evidence. No player credentials or license contents read/packaged.

## Next concrete implementation scope

Select **hud_frame + quest_panel** first. Both have zero serialized consumers, and all final page/constructor observations use current replacements. Each still requires removal from Home's TextureNames and temporary assignments in BuildInto/AlignReferenceComposition; keep hierarchy and current ProductionPlayerHeader/source quest bindings. Add denial/remigration/identity regressions, verify Home plus map/ChapterMap/PreLevel cloned-header paths and native Splash/Loading, then archive the two PNG/meta pairs through the existing exact-path importer owner. Preserve GUIDs, byte hashes, source dimensions, RGBA32/mipmaps and Android4096format4 settings. Both actual Unity textures have12mip levels.

Their R13 packed-object bytes are16,771,660combined (hud_frame8,386,648 +quest_panel8,385,012). **This is not compressed APK savings or memory reduction.** All21legacy entries total176,090,708packed-object bytes, including resources that must remain. Actual build membership and APK size must be measured only after the coordinated next implementation. Do not combine the old footer: its WorldMap dormant serialized reference needs its own active-prefab/runtime/source-owner alignment later. Nine other dormant Home resources are documented for subsequent work; shared four and current five remain retained.

## Scope and limitations

[Scope](QA/Scope.json): all7,786task-start Assets/Packages/ProjectSettings inputs byte-identical; zero changed/added/deleted. Read-only baseline excludes secret/key/signing/player-storage files. All inherited local work, packages/settings, GUIDs/imports and approved Splash preserved. Added audit sources, JSON/XML evidence and TestResults plus updates to CurrentTask, SessionLog, DevelopmentSchedule and ConversationHandoff only. No permanent asset/test/code change, new caches in the authored patch, commit or deployment.

R13 APK remains815,985,566bytes, SHA256 `f0be6efcfe4ad0b5e60cd38cdc0baccaec84340233ac5d6dba033c0f60b76a6a`; actual hash verified against its saved artifact, no new build. [Artifact baseline](QA/ArtifactBaseline.json).

No Play Mode run, pixel comparison, new screenshot, device installation/execution, lifecycle/performance acceptance or size reduction. Prior R13 runtime/build/capture results remain historical. Phone execution/hosting remain deferred, effects last; human Chapter1 pacing and exact font provenance remain open.
