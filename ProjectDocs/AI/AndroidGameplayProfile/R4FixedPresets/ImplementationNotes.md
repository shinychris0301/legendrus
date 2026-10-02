# Fixed disposable gameplay presets R4 — 2026-10-02

The existing `DevelopmentGameplayProfile` now accepts six bounded cold-session selectors through `LOR_GAMEPLAY_QA` in Editor and the existing guarded Android development intent entry in source. The current APK predates this change; packaging and phone activation are separate next steps. No arbitrary quantity, progression, JSON, path or runtime-switch API was added.

| Selector | Initial synthetic state |
|---|---|
| `stock-999` | Each of four boosters 999; round 1, no completed rounds/scenes |
| `stock-1000` | Each of four boosters 1 000; round 1, no completed rounds/scenes |
| `stock-9999` | Each of four boosters 9 999; round 1, no completed rounds/scenes |
| `stock-max` | Each of four boosters 2 147 483 647; display/consumption only |
| `story-early` | Rounds 1–8 completed once, claimed first-clear rewards, 24 stars, intro/two milestones acknowledged; selected/unlocked 9 |
| `story-finale` | Rounds 1–500 completed once, claimed first-clear rewards, 1 500 stars, 58 scenes/Chapter 1 acknowledged; selected/unlocked 500 |

Every preset starts with energy 4/current clock, fresh Russian language and fresh consent; ordinary language controls are retained. Stock presets retain default gold 12 540/gems 350. Story presets accumulate ordinary catalogue first-clear rewards in checked arithmetic. The complete v6 DTOs match the independently service-derived [R3 references](../R3FixturePreparation/FixturePlan.json), apart from the fresh clock. Synthetic completions are not human pacing or earned phone evidence. Separate campaign v9 development saves are not touched.

`DevelopmentGameplayFixtures` is an internal pure candidate factory: direct validated active content, no GameState/StoryService instance, preference reads/writes, SaveService, file imports or normal-save access. It requires the installed 500 ordered Chapter 1 rounds, 58 unique scenes, 57 ordered milestones, matching triggers/finale, valid timestamp and overflow-safe fixture reward accumulation. It returns independent mutable DTOs and never mutates content. The factory type is excluded unless `UNITY_EDITOR` or `UNITY_ANDROID && DEVELOPMENT_BUILD` is defined.

The profile rejects a present foreign or empty ownership marker for every new preset before loading content or deleting an owned slot. An absent marker or the exact existing owner is accepted. Candidate preparation precedes mutation. Reset remains limited to the five existing QA save/language/consent slots and owned marker. The candidate must commit through the existing QA journal before ownership is published. Initialization failures remain sticky; recursive and later stores cannot fall through to normal saves. `fresh`, `resume`, `empty-energy` and no-command semantics are retained, including legacy fresh behavior with a foreign marker. Existing Android package/debuggable checks, intent consumption and release exclusion remain in place.

Existing ordinary booster reward additions have no maximum overflow protection. Consequently `stock-max` is for display and consumption only; avoid reward-producing actions. No normal economy or save-recovery policy was changed. Renderer/readability code, builders, active prefabs, artwork, fonts and approved Splash are byte-preserved; no visual redesign or revision change is needed for these selectors.

New permanent Edit Mode coverage compares six complete candidates with R3 references, rejects invalid commands/clocks/catalogues and verifies independent lists. New staged Play Mode coverage launches the actual product selector without test seeding, then restores via a separate `resume` process. Both story readers use real native controls from History page2, preserve full save/journal and reject duplicate replay grants. Existing native stock and recap suites additionally verify RU/EN live quantities, pointer forwarding, consumption and reader routing. The compiled entry harness uses the actual profile/factory sources with fake JNI/content/preferences; it is not an Android runtime or release build.

See [test results](TestResults.md), [scope](QA/Scope.json), [reconciliation](QA/Reconciliation.json), [task-only patch](Sources/TaskChanges.patch) and [final validation](QA/FinalValidation.json). A separate guarded development APK refresh is next; current phone/hosting deferrals and human 201–210 collection remain in force, with final gameplay effects last.
