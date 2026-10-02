# Disposable fixed-fixture preparation R3 — 2026-10-02

Prepared six bounded synthetic reference fixtures. **All 14 separate Unity process checks passed**, zero runtime failures/skips; **34 standalone compiled entry-guard checks passed**. The existing profile and APK still support only fresh/resume/empty-energy. Proposed selector names are not installed Android commands, and the exported DTOs are not a supported import route.

| Proposed preset | Completed rounds | Eligible recap scenes | Stars | Reference |
| --- | ---: | ---: | ---: | --- |
| `stock-999` | 0 | 0 | 0 | [DTO](QA/Fixtures/stock-999.player.json) |
| `stock-1000` | 0 | 0 | 0 | [DTO](QA/Fixtures/stock-1000.player.json) |
| `stock-9999` | 0 | 0 | 0 | [DTO](QA/Fixtures/stock-9999.player.json) |
| `stock-max` | 0 | 0 | 0 | [DTO](QA/Fixtures/stock-max.player.json) |
| `story-early` | 8 | 3 | 24 | [DTO](QA/Fixtures/story-early.player.json) |
| `story-finale` | 500 | 58 | 1500 | [DTO](QA/Fixtures/story-finale.player.json) |

[Bounded scope](FixtureScope.md) · [Machine-readable plan](FixturePlan.json) · [Actual reconciliation](QA/Reconciliation.json) · [14-process plan](Sources/PhasePlan.json) · [Compiled guard results](QA/EntryGuards/summary.json).

Unity 6000.5.9f1 ran sequentially in one new disposable product store under /tmp, with normal writer/reader processes bracketing six fresh writer/resume reader pairs. All launchers exited 0; their total duration was 12.13 minutes. These are 14 phase executions of two test methods, not 14 distinct method names or a full-project sweep. Synthetic normal save primary/backup/staging, language and compliance fingerprints remain identical across every QA phase and the final normal restore.

The active v6 journal is kept separate from the v9 campaign development session. Each fixture commits through existing GameState/SaveService behavior. All four booster quantities round-trip exactly at 999, 1 000, 9 999 and Int32.MaxValue. Early story has eight completed rounds, three normally advanced scenes and History page2. Finale has all500 completed rounds, all58 scenes, 1 500 stars and an acknowledged actual round500 ending; Chapter2 remains unavailable. These are synthetic service-level completions with maximum test scores, not legal-swap replays or human outcomes.

Both story resume processes opened the actual native Profile History reader on page2. Early Back and completed finale return preserve the same page, complete player payload and all three QA journal slots. A replay completion rejects duplicate first-clear currency and booster grants. Stock maximum is limited to display/consumption: the existing reward code adds booster quantities without overflow protection; this milestone does not change that economy behavior.

An initial temporary test compilation failed because this NUnit version lacks Is.AnyOf. The corrected prototype uses ordinary membership assertions; original source, initial run/log and initial phase plan are retained. No production correction or assertion removal was made. [Initial compiler run](QA/NormalBefore/summary.json) · [Corrected normal writer](QA/NormalBeforeVerified/summary.json) · [Archived prototype](Sources/FixturePrototype.cs).

The 34 guard checks compile the actual unchanged profile source for Android development and release against fake JNI/preferences. They cover existing activation/package/debuggable/owner/failure/session-lock controls and rejection of all six proposed commands before writes. Release paths make no JNI call or mutation and exclude the badge. This is not actual Android JNI, a new Unity release build or device activation. The planned additional foreign-owner rule for new presets remains an implementation requirement.

The temporary prototype and its metadata were removed after Unity closed. **All 7,833 task-start Assets/Packages/ProjectSettings/Backend inputs are byte-identical**, including existing tests, production runtime, art/fonts/prefabs/GUIDs/metas, approved Splash, settings/packages/backend/guards and inherited edits. Six documentation files change, plus new task scope/tools/evidence. [Scope](QA/Scope.json) · [Exact temporary cleanup](QA/TemporaryFixtureCleanup.json) · [Task-only patch](Sources/TaskChanges.patch) · [Final reconciliation](QA/FinalValidation.json).

Existing R3 APK/sidecar remains 766,017,314 bytes / SHA256 2302b069bddf4b0261513fa5696bfa60dc9940f1c9bd61bc34a54c063079335e. Five normal manual recordings are unchanged. No normal personal journal/credential access, new Edit Mode suite, Android build/device/ADB action, hosting/deployment or commit. No new design/capture or visual acceptance is claimed; R7's native captures are historical.

**Next:** Implement the six bounded preset selectors and a pure fixture factory in the existing DevelopmentGameplayProfile, including foreign-owner rejection and failed-commit isolation. Preserve the current fresh/resume/empty-energy behavior and development/release guards; verify normal saves, cold resume, native stock and recap before a separate guarded APK refresh. Human rounds 201–210 remain the parallel acceptance action. Phone execution and hosting remain deferred; final gameplay effects stay last.
