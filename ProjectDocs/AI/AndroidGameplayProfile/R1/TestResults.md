# Disposable gameplay profile R1 — test results

Implemented the approved development-only gameplay profile in the existing startup and persistence services. **Final: 52 targeted normal-mode regression tests and 4 native gameplay checks passed, plus 8 fresh-process profile checks; 0 failures. One backend-dependent test skipped.** These are 64 successful executions across 10 runs, with 57 distinct passing test names: the eight profile phases deliberately share one test method. Intermediate runs are excluded.

## Implemented

- Separate primary/backup/staging game journal, language and compliance keys. No migration/copy of normal progress into QA, no `PlayerPrefs.DeleteAll`, no credential access to create a gameplay fixture.
- Cold-launch `fresh`, `resume`, and `empty-energy` commands. Profile selection happens before a store reads data and stays locked for that session. Reset touches only owned QA slots; no in-game profile switching.
- Existing GameState, RuntimeBootstrap, router, gameplay rules and canonical pages retained. A development-only label attaches to the existing services root; no second Canvas or EventSystem.
- Accounts, credential probe, optional analytics and monetization disabled within QA. Normal-mode behavior remains covered by the selected regressions.
- Android intent activation is restricted in source to development builds, the approved debug package and Android's debuggable flag. Editor uses an explicit environment variable. Android-specific execution is still pending.

[Usage and scenario instructions](Usage.md) · [Exact run inventory](QA/RunInventory.json) · [Scope and changed files](QA/Scope.json) · [Task-only existing-source patch](Sources/TaskChanges.patch).

## Executed evidence

| Run | Result | What it establishes |
|---|---|---|
| FinalNormalWrite | 1 passed | Valid synthetic normal player/compliance/language state created in isolated `/tmp` product storage; key fingerprints recorded. |
| FinalProfileWrite | 1 passed | Fresh QA state, energy debit, mace inventory debit, first-clear reward and replay protection, preferences and persistence; normal fingerprints unchanged. |
| FinalProfileResume | 1 passed | A separate process restores QA balances/inventory/preferences; replay does not duplicate rewards. |
| FinalCorrupt | 1 passed | Corrupt QA primary and backup lock saving and do not load normal progress. Fault injection restored afterward. |
| FinalProfileReset | 1 passed | Fresh fixture resets QA history, inventory and language while preserving normal keys. |
| FinalEmptyEnergy | 1 passed | Zero-energy fixture refuses spending; normal keys unchanged. |
| FinalNormalRead | 1 passed | Launch without QA restores original normal progress/preferences; every recorded normal-key fingerprint matches. |
| NativeJourneys | 4 passed | Real native win/fail/free-retry/pause/settings/return routes, same-process lifecycle callbacks and capped energy catch-up while QA is active. |
| NormalAfterJourneys | 1 passed | Normal fingerprints still match after the complete QA journeys. |
| NormalRegression | 52 passed, 1 skipped | Save migration/recovery, language UI/persistence, privacy, commercial wiring and backend session unit/Play Mode coverage with QA disabled. |

The native Russian win used **6 legal swaps**, energy **5 → 4**, gold **12,540 → 12,660**. The English failure used **28 legal swaps**, energy **2 → 1**, gold unchanged; Retry restored the initial move budget without another energy charge. Native route tests use existing deterministic boards and actual bound controls, not forced outcome flags. [Win record](QA/NativeJourneyCaptures/Win_RU_Result.json), [failure record](QA/NativeJourneyCaptures/Fail_EN_Result.json).

Actual Unity gameplay and Retry captures were inspected; the established page/artwork remained intact. These offscreen Canvas captures do not include the new IMGUI QA label, so they do not establish its on-device appearance. No new visual design acceptance is claimed.

The skipped case is `BackendSessionPlayModeTests.RealHttpBootstrapAndRefreshKeepIdentityAcrossClientRecreation`, which requires an explicitly configured isolated backend. Hosting remains deferred. This task did not run a full-project suite or Edit Mode suite. The earlier R5 broad result is retained as historical evidence, not counted again here.

## Correction during verification

The first profile sequence passed QA write/resume/reset/zero-energy, but the final normal-reader check failed because its test had written an invalid consent sentinel. Normal startup correctly replaced that invalid fixture. The test now creates valid consent state through the existing service before recording fingerprints. All seven corrected phases passed on a fresh isolated store, followed by the native journeys and another normal-reader pass. No production behavior was weakened to satisfy the fixture.

## Scope and limitations

Unity **6000.5.9f1**, graphics-enabled Play Mode, previously authorized process-only Vulkan. Product storage isolated under `/tmp/lor-gameplay-profile-r1/`; only the existing Editor licensing directory is linked. The developer's normal save and Android device were not used.

**7 existing runtime source files changed, 6 Assets files added (3 C# files and their metadata), 7,778 baseline files unchanged, 0 deleted.** Existing GUIDs, prefabs/art/fonts, Packages, ProjectSettings, backend, Android guards and Java plugins unchanged. New GUID uniqueness, task-source whitespace and runner Python syntax checks passed. Exact paths and hashes are recorded in Scope and Sources/AfterHashes.json.

No APK build, install, phone connection/launch, Android intent/JNI execution, release-player compilation, physical lifecycle/performance/readability test, backend activation, commit or deployment. Invalid-intent and absent-owner blocking are implemented but have not been exercised through Android. This is isolated gameplay state within one app, not a separate Android OS account or install; existing debug export destinations remain unchanged.

**Next:** prepare the guarded development APK containing this profile and inspect its build identity/entry guards. Keep actual device installation and testing deferred until the user resumes it. Human Chapter 1 difficulty/pacing, device acceptance and live-service work remain open; final gameplay animation/effects remain last.
