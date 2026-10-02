# Disposable gameplay profile — development only

This facility runs the existing game with a separate save journal, language preference and consent record. It does not clone normal progress, change campaign rules, unlock commercial services or create a second startup/router. The yellow `QA · DISPOSABLE SAVE` indicator identifies the profile on screen. The existing approved art is unchanged.

## Commands

Select one command at **cold process launch**, before any game state loads:

| Command | Result |
|---|---|
| `fresh` | Replace only the five owned QA preference slots with a fresh round-1 profile: default balances, energy and boosters; Russian language; fresh consent state. First-attempt board and rules are the ordinary campaign's. |
| `resume` | Continue the owned QA journal. Requires a previously created profile. It does not restore an arbitrary unfinished round; supported retained Failure behaves as in normal gameplay. |
| `empty-energy` | Reset QA as above with zero energy and a new clock timestamp. Used to exercise the normal Out of Energy gate. |
| No command | Normal player profile. The QA save remains available for a later explicit `resume`. |

There is no runtime switch or in-game destructive reset control. To repeat a fixture, end the QA process and explicitly launch `fresh` again. To verify persistence, launch `resume`, **not** `fresh`. Invalid commands and missing resume ownership block initialization; they must not silently load normal data. A corrupt QA journal uses the existing recovery/write-lock policy, never the normal save. `fresh` deliberately replaces the QA journal, including its recovery slots.

## Editor

`LOR_GAMEPLAY_QA=fresh`, `resume` or `empty-energy` is read when entering Play Mode. The value is locked for that session. Ordinary Editor runs without the variable use normal data. Automated verification additionally requires isolated `XDG_CONFIG_HOME` and `LOR_LOADING_TEST_CONFIG_ROOT`; see `Sources/Verify.py` and the phase options. Those tests use synthetic normal data under `/tmp`, not the developer's personal player save.

`Sources/RunFinalPhases.py` uses fresh output directories and must not overwrite an earlier run. To repeat it, copy the run names/options to new names and a new isolated config directory. The phases share that one disposable config directory to verify real restart behavior.

## Android — prepared, not executed

The Android entry reads the intent string extra `lor_gameplay_qa` on cold launch. It is compiled for Android development builds only and additionally requires the current approved development application ID `com.example.legendsofrus.v117debug` and Android's debuggable flag. Non-development players have no activation path. No manifest, package ID, Java plugin or build-approval flags were changed.

The current [verified stock-refresh R6 APK](../../CoreOutcomeReadability/R6StockAndroidRefresh/TestResults.md) includes this profile and Gameplay revision 24: 765,764,694 bytes, SHA256 `42b29fca2c6941ed2ac96e5c7dd00f9e6c79aab70dfd5b0892a64d7faa815ce5`. Its launch activity is `com.unity3d.player.UnityPlayerGameActivity`, verified from the local APK manifest. Packaging inclusion is established; Android cold-launch activation and isolation remain untested. The earlier credential-test installed artifact is historical, and no current installed build identity is assumed.

Run the [current preflight](../../AndroidGameplayJourney/R2OfflineHandoff/Sources/Preflight.py) without arguments for an offline artifact check. After phone execution resumes, use `--device` for read-only installed-identity queries. Neither mode launches the game. Verify the yellow QA indicator after a separately performed cold launch before any controlled action. Do not clear app data, uninstall, read credential records or reuse the credential probe as a gameplay fixture.

After phone testing resumes, pass the extra with `fresh` for a new fixture and `resume` on subsequent cold launches. A launcher-icon start without the extra is normal mode. Do not assume Android background/foreground creates a new process; do not deliver a new fixture command to a running process. Returning from the background keeps the selected mode for that process.

## Repeatable scenarios

1. **Win/replay:** `fresh`; play round 1 through the normal Home/map/story/PreLevel route, record energy/boosters, win, record first-clear rewards, then `resume` and replay. No second first-clear reward.
2. **Fail/retry:** `fresh`; exhaust moves without completing goals. Retry must create a new board with the initial move budget and retain the existing no-extra-energy retry policy. Pause → Exit returns through the normal map route.
3. **Pause/settings:** `fresh`; start a round, pause, open Settings, change language, return and resume. The same board/moves and inventory remain. No duplicate energy charge.
4. **Energy gate:** `empty-energy`; reach PreLevel and attempt Play. The standard energy dialog must block starting. This is a zero-energy fixture, not a time-regeneration simulation.
5. **Booster accounting:** `fresh`; use each available booster, also cancel targeting and try unavailable inventory. Accepted use charges once; cancellation does not charge. Final effect quality remains deferred.
6. **Durability:** `resume` after completing actions. Verify durable balances, settings, progress and supported retained Failure; then launch without an extra to return to normal data. Only `fresh`/`empty-energy` resets QA data.

The existing automated native win/fail journeys execute legal swaps on the ordinary deterministic round-1 boards; they are not a forced win/fail gameplay override. Phone touch accuracy, Android lifecycle, human pacing, frame time and readability still require physical testing. QA debug labeling itself adds rendering overhead that must be noted in performance measurements.

## Large-stock limits

The packaged command set has no stock-quantity, campaign-unlock or arbitrary save-import extra. `fresh` seeds ordinary default stock; it does not supply 1 000 / 9 999 / Int32.MaxValue. The [current device matrix](../../AndroidGameplayJourney/R1/DeviceChecklist.md) records these revision-24 scenarios as pending a separately reviewed disposable fixture. R5's synthetic stock Editor tests are historical runtime evidence, not on-device setup or acceptance. Keep exact-count readability and 1 000→999 input accounting separate from general profile activation.

## Storage boundary

Only `LegendsOfRus_GameplayQA_v1_` keys are reset: the primary/backup/staging save journal, language and compliance keys, plus the profile's own ownership marker. No preference enumeration or `DeleteAll` is used. Normal progress/preferences and Android credentials are not imported or reset. Account configuration and the credential probe are blocked in QA; optional analytics and monetization calls are blocked too. Current content delivery remains locally configured. This is gameplay-state isolation, not a separate Android OS user, install or security boundary; ordinary debug capture/export files retain their existing destinations.
