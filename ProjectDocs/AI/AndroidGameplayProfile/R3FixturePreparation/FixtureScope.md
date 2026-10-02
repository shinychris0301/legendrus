# Bounded disposable fixture preparation — 2026-10-02

This milestone prepares and rehearses six fixed synthetic Editor fixtures for the existing development gameplay profile. The current APK and runtime still accept only `fresh`, `resume` and `empty-energy`. The names below are proposed selectors, not installed Android commands. Exported player DTOs are audit evidence, not a supported import mechanism.

| Proposed preset | Exact intended state | Device scenario dependency |
| --- | --- | --- |
| `stock-999` | All four booster quantities 999; round1 selected/unlocked; no completed levels or acknowledged story. | STOCK-01 circular badge boundary. |
| `stock-1000` | All four quantities 1 000; otherwise the same fresh progress. | STOCK-01/02/03 exact badge transition, one Sea Pearl consumption and +5 moves. |
| `stock-9999` | All four quantities 9 999; otherwise the same fresh progress. | STOCK-01/03 grouped count readability and control routing. |
| `stock-max` | All four quantities 2 147 483 647; otherwise the same fresh progress. | STOCK-01 extreme display boundary. Use for observation/consumption only, reset before reward-producing actions: current first-clear reward code adds booster quantities without an overflow guard. This scope does not change economy behavior or claim safe maximum-stock reward accumulation. |
| `story-early` | Rounds1–8 each completed once through the existing service; their first-clear rewards claimed, three stars each; round9 selected/unlocked. Intro and the first two milestone scenes acknowledged through normal StoryService advancement. Three recap entries; later scenes remain unread; eight History rows permit page2. | RECAP-02 paging, latest entry, Earlier/Later, native Back, language and resume checks. |
| `story-finale` | All500 installed rounds completed once through the service, first-clear flags set, 1 500 stars, selected/unlocked round500. Intro and all57 milestone scenes advanced; actual round500 finale and Chapter1 completion acknowledged. All58 scenes eligible; Chapter2 remains unavailable. | RECAP-03 actual acknowledged finale without a pending normal ChapterComplete presentation. |

Every prepared preset uses Russian/default consent and preferences, energy4/5 with a current timestamp, no retained failure or continuation transaction, and the existing offline QA restrictions. Stock presets retain gold12 540/gems350. Story fixtures accumulate the installed first-clear rewards through the existing service; they are synthetic service-level completions, not played boards, human pacing evidence or phone acceptance. Their maximum test scores are deliberately synthetic and must be labeled as such in reports.

## Installed ownership and storage boundary

The active game uses PlayerSaveData v6 and SaveService's existing three-slot journal. The separate CampaignSaveSession v9 development system and its scoped story ledger must not be used to seed this profile. ChapterLevelCatalog and StoryCatalog resolve installed CampaignActive resources; no fallback catalogue or guessed legacy finale ID is acceptable. In particular, the installed finale is `campaign.chapter01.village_departure`, after round500.

Keep `DevelopmentGameplayProfile` as the sole cold-launch selector and use its `LegendsOfRus_GameplayQA_v1_` namespace, ownership marker and existing exact-slot resets. All stores must select QA before their first read; failed initialization must remain sticky and must never select normal data afterward. `resume` must require the exact owner. No runtime switching, preference enumeration/DeleteAll, normal-save import/copy, credential access, arbitrary quantity/progress input or file-path/JSON import is in scope.

The existing Android development/package/debuggable gates, intent consumption and release exclusion must remain intact. Existing `fresh`, `resume`, `empty-energy` and no-command semantics remain unchanged. All new selectors should be exact allowlisted strings, validated before mutations. A present foreign owner must block a new preset before reset; an absent owner may be created in the dedicated QA namespace. That additional new-selector owner rule is an implementation requirement, not a claim about the current `fresh` command or this prototype.

Commit a bounded candidate through the existing save journal before writing its owner marker. Do not create GameState, Canvas, EventSystem or routing alongside startup. The future factory must construct its payload before initialization completes; the rehearsal uses already running services to establish reference outcomes and is not production startup code. A commit failure must leave initialization blocked without normal fallback.

## Rehearsal and implementation boundary

[Archived prototype](Sources/FixturePrototype.cs) runs only as a temporary Unity test under explicitly isolated XDG_CONFIG_HOME/LOR_LOADING_TEST_CONFIG_ROOT. It writes reference DTOs under [QA/Fixtures](QA/Fixtures/), verifies exact journal round trips and fresh-process restoration, compares five synthetic normal preference fingerprints, and tests native History page2 entry/return and finale completion without recap writes or duplicate replay grants. It never reads the developer's personal save.

[Process plan](Sources/PhasePlan.json) brackets six writer/reader pairs with normal-profile writer/reader processes. Existing release/development entry logic is separately compiled against fake JNI/preferences, including rejection of all six proposed names. Actual APK activation, Android JNI, background/foreground, physical input/readability and process-kill durability remain pending. Test execution results belong in [TestResults](TestResults.md); this scope alone is not a pass claim.

The implementation milestone should port the verified reference outcomes into a bounded pure fixture factory owned by DevelopmentGameplayProfile, add the exact six selectors and new-selector owner/commit failure cases, retain current normal/profile/recap regressions, then refresh the guarded APK separately. Do not use the current APK with these proposed extras or mark pending phone rows passed.
