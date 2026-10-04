# Disposable gameplay profile — development only

This facility runs the existing game with a separate save journal, language preference and consent record. It does not clone normal progress, change campaign rules, unlock commercial services or create a second startup/router. The yellow `QA · DISPOSABLE SAVE` indicator identifies the profile on screen. The existing approved art is unchanged.

## Commands

Select one command at **cold process launch**, before any game state loads. The installed Editor/source and verified settings-recovery R2 development APK support the six fixed presets below, along with `fresh`, `resume` and `empty-energy`. Android activation and save isolation still require physical evidence.

| Command | Result |
|---|---|
| `fresh` | Replace only the five owned QA preference slots with a fresh round-1 profile: default balances, energy and boosters; Russian language; fresh consent state. First-attempt board and rules are the ordinary campaign's. |
| `resume` | Continue the owned QA journal. Requires a previously created profile. It does not restore an arbitrary unfinished round; supported retained Failure behaves as in normal gameplay. |
| `empty-energy` | Reset QA as above with zero energy and a new clock timestamp. Used to exercise the normal Out of Energy gate. |
| `stock-999`, `stock-1000`, `stock-9999`, `stock-max` | Replace owned QA state with round 1/default balances, energy 4 and exactly 999, 1 000, 9 999 or 2 147 483 647 of each of the four boosters. Packaged in the verified settings-recovery R2 development APK; physical activation pending. |
| `story-early` | Eight synthetically completed rounds, 24 stars, intro and two acknowledged milestone scenes; selected/unlocked round 9, energy 4. Packaged in the verified settings-recovery R2 development APK; physical activation pending. |
| `story-finale` | All 500 rounds synthetically completed, 1 500 stars, all 58 scenes and Chapter 1 acknowledged; selected/unlocked round 500, energy 4. Chapter 2 remains unavailable. Packaged in the verified settings-recovery R2 development APK; physical activation pending. |
| No command | Normal player profile. The QA save remains available for a later explicit `resume`. |

There is no runtime switch or in-game destructive reset control. To repeat a fixture, end the QA process and explicitly launch its reset command again. To verify persistence, launch `resume`, **not** `fresh`. Invalid commands and missing resume ownership block initialization; they must not silently load normal data. A corrupt QA journal uses the existing recovery/write-lock policy, never the normal save. `fresh` deliberately replaces the QA journal, including its recovery slots.

## Editor

`LOR_GAMEPLAY_QA` accepts `fresh`, `resume`, `empty-energy` and the six exact preset names above when entering Play Mode. The value is locked for that session. Ordinary Editor runs without the variable use normal data. Automated verification additionally requires isolated `XDG_CONFIG_HOME` and `LOR_LOADING_TEST_CONFIG_ROOT`; see `Sources/Verify.py` and the phase options. Those tests use synthetic normal data under `/tmp`, not the developer's personal player save.

`Sources/RunFinalPhases.py` uses fresh output directories and must not overwrite an earlier run. To repeat it, copy the run names/options to new names and a new isolated config directory. The phases share that one disposable config directory to verify real restart behavior.

## Android — prepared, not executed

The Android entry reads the intent string extra `lor_gameplay_qa` on cold launch. It is compiled for Android development builds only and additionally requires the current approved development application ID `com.example.legendsofrus.v117debug` and Android's debuggable flag. Non-development players have no activation path. No manifest, package ID, Java plugin or build-approval flags were changed.

The current [verified settings-recovery R2 APK](../../SettingsRecovery/R2AndroidRefresh/TestResults.md) includes this profile, Profile revision13/story recap and Gameplay revision24: 766,032,162 bytes, SHA256 `0310f6a446f268fb36a70c3a7af604cc89b54c5cbc51f19a864316aeaf421adc`. Its launch activity is `com.unity3d.player.UnityPlayerGameActivity`, verified from the local APK manifest. Packaging inclusion is established; Android cold-launch activation and isolation remain untested. The earlier credential-test installed artifact is historical, and no current installed build identity is assumed.

Run the [current preflight](../../AndroidGameplayJourney/R2OfflineHandoff/Sources/Preflight.py) without arguments for an offline artifact check. After phone execution resumes, use `--device` for read-only installed-identity queries. Neither mode launches the game. Verify the yellow QA indicator after a separately performed cold launch before any controlled action. Do not clear app data, uninstall, read credential records or reuse the credential probe as a gameplay fixture.

After phone testing resumes, pass the extra with `fresh` for a new fixture and `resume` on subsequent cold launches. A launcher-icon start without the extra is normal mode. Do not assume Android background/foreground creates a new process; do not deliver a new fixture command to a running process. Returning from the background keeps the selected mode for that process.

## Repeatable scenarios

1. **Win/replay:** `fresh`; play round 1 through the normal Home/map/story/PreLevel route, record energy/boosters, win, record first-clear rewards, then `resume` and replay. No second first-clear reward.
2. **Fail/retry:** `fresh`; exhaust moves without completing goals. Retry must create a new board with the initial move budget and retain the existing no-extra-energy retry policy. Pause → Exit returns through the normal map route.
3. **Pause/settings:** `fresh`; start a round, pause, open Settings, change language, return and resume. The same board/moves and inventory remain. No duplicate energy charge.
4. **Energy gate:** `empty-energy`; reach PreLevel and attempt Play. The standard energy dialog must block starting. This is a zero-energy fixture, not a time-regeneration simulation.
5. **Booster accounting:** `fresh`; use each available booster, also cancel targeting and try unavailable inventory. Accepted use charges once; cancellation does not charge. Final effect quality remains deferred.
6. **Durability:** `resume` after completing actions. Verify durable balances, settings, progress and supported retained Failure; then launch without an extra to return to normal data. The reset commands (`fresh`, `empty-energy` and the six fixed presets) replace QA data; `resume` preserves it.

The existing automated native win/fail journeys execute legal swaps on the ordinary deterministic round-1 boards; they are not a forced win/fail gameplay override. Phone touch accuracy, Android lifecycle, human pacing, frame time and readability still require physical testing. QA debug labeling itself adds rendering overhead that must be noted in performance measurements.

## Large-stock limits

The verified settings-recovery R2 APK packages the four exact stock names, with no arbitrary quantity, JSON, path or save-import command. Use `stock-1000` for the native 1 000→999 Sea Pearl boundary, then `resume` to check durability. Stock fixtures have no completed rounds or acknowledged scenes. All presets reset language/consent as fresh QA state; ordinary language controls remain available.

`stock-max` is restricted to display/consumption testing. Checked reward/credit rejection and durable command refunds are now packaged; avoid reward-producing actions with this maximum fixture, which has no grant capacity. This implementation does not change the normal economy or introduce a stock-cap policy. Exact-count Editor rendering/input tests and cold persistence do not establish physical legibility or Android activation.

## Storage boundary

Only `LegendsOfRus_GameplayQA_v1_` keys are reset: the primary/backup/staging save journal, language and compliance keys, plus the profile's own ownership marker. No preference enumeration or `DeleteAll` is used. Normal progress/preferences and Android credentials are not imported or reset. Account configuration and the credential probe are blocked in QA; optional analytics and monetization calls are blocked too. Current content delivery remains locally configured. This is gameplay-state isolation, not a separate Android OS user, install or security boundary; ordinary debug capture/export files retain their existing destinations.


Optional recap is reached through Profile → History → Story. Eligible scenes must be acknowledged and milestone eligibility requires its completed round. Installed `story-early` and `story-finale` create bounded synthetic ledgers matching the independently rehearsed R3 service outcomes, including ordinary first-clear balances/stock and claimed rewards; replay cannot claim those grants again. These are synthetic QA completions, not human play or earned phone evidence. Both selectors remain packaged in the current settings-recovery R2 APK; cold-launch activation remains unverified on a phone.

## Fixed fixtures — installed source and verified development APK

[Implementation and checks R4](../R4FixedPresets/TestResults.md) installs the six bounded selectors in the existing profile. Candidates are prepared from validated active Chapter 1 content before owned-slot deletion. New presets reject a present foreign or empty ownership marker before mutation; no marker or the exact existing owner is allowed. The candidate commits through the existing QA SaveService journal before publishing ownership. Content/commit failures remain blocked for that process and never read normal data. Existing `fresh`, `resume`, `empty-energy` and no-command behavior is preserved, including the older fresh reset semantics.

The factory is compiled only for Editor or Android development builds. Android intent/package/debuggable guards and cold-process selection remain unchanged. Current reference DTO exports have no supported import route. [Current packaging checks](../../SettingsRecovery/R2AndroidRefresh/TestResults.md) verify the factory and exact selectors; [R5 checks](../R5AndroidRefresh/TestResults.md) remain historical. Resumed device execution is still required for Android activation/isolation acceptance; all18physical rows remain pending. The original R3 artifact is archived and does not contain these presets.

## 2026-10-03 — daily-reward build pin

[Daily reward Android refresh R2](../../DailyRewardRecovery/R2AndroidRefresh/TestResults.md) packages the verified overflow-rejection service while preserving the six QA selectors and existing save ownership guards. Current offline preflight now requires matching compiled native-method evidence;52synthetic/mocked boundaries pass. Native code inclusion is not physical claim/activation/isolation acceptance. Original R5APK/sidecar is archived; all18physical rows remain pending. The ordinary level/booster credit overflow paths remain unchanged, so stock-max stays display/consumption only.

## 2026-10-03 — spending/refund build pin

[Current R2 results](../../SpendingRecovery/R2AndroidRefresh/TestResults.md) establish the 766,022,530byte artifact/SHA256`5f6cb7d0658cfabf6289527b36743dd19d90027150b418a453ccf9c797a18136`, matching original certificate and22compiled transaction owners. All accumulated reward/credit/spending fixes and durable booster refunds are packaged. Canonical offline preflight now checks these owner methods and passes64synthetic/mocked boundaries. Earlier dated pins above are historical. No phone activation, process restart, isolation, physical readability or store acceptance claimed; all18physical rows pending. Stock-max remains display/consumption only.

## 2026-10-03 — energy recovery build pin

[Current EnergyR2results](../../EnergyRecovery/R2AndroidRefresh/TestResults.md) establish the 766,024,682byte APK/SHA256`8db3d3e4fb4f8ba24a69ccff1b981df9fdb7b99bd207be19e252f16eb338b569` with matching original certificate and30compiled transaction/energy owners. Energy grants/refills/clock now commit before live publication; overflow respects the existing cap and full/no-due clock is a healthy no-op. Three energy Try APIs support callers needing acknowledgement; existing void callers and unavailable commercial offers remain. Canonical offline preflight checks energy and previous transaction proof;77synthetic/mocked checks pass. All earlier dated pins above are historical. Original SpendingR2APK/sidecar preserved, all18physical rows pending; no phone/hosting action, stock-max remains display/consumption only.

## 2026-10-03 — narrative recovery build pin

[Current NarrativeR2results](../../NarrativeRecovery/R2AndroidRefresh/TestResults.md) verify 766,019,098bytes/SHA256`932535353501df1e04e65814ffd2383edc44b33bc60973aa692ae5e84b3ae928` and the original development certificate. All35native owners include atomic story/chapter acknowledgement and rejected-save final-line/error handling, alongside earlier transaction fixes.160fresh Unity Edit and88mocked offline checks pass; source-stage396passes remain separate. Previous EnergyR2APK/sidecar retained. Earlier dated pins above are historical. All18physical rows remain pending, including story acknowledgement/relaunch recovery; no phone/hosting action. Failed unread scenes reopen from the first line, with no persisted line checkpoint.

## 2026-10-03 — tutorial recovery build pin

[Current TutorialR2 results](../../TutorialRecovery/R2AndroidRefresh/TestResults.md) verify 766,026,550bytes/SHA256`b699646db5f7ac9effc403c768eb19200b5eb3b58f14858b331f5edb80aa5fb4`, original development certificate,42native owners/two tutorial callbacks,160fresh Unity Edit checks and104mocked offline checks. Prior same-source405runtime/service checks remain separate. Previous NarrativeR2APK/sidecar retained; earlier dated pins remain historical. Tutorial saving rejection preserves its paused step and exact overlay; Close/Back dismisses the error. After repair/relaunch, unread authored steps are re-offered without a line/board checkpoint. All18physical rows still require phone touch, lifecycle/recovery and disposable-profile isolation evidence. No phone/hosting or commercial acceptance claim.

## 2026-10-03 — selection recovery build pin

[Current SelectionR2 results](../../SelectionRecovery/R2AndroidRefresh/TestResults.md) verify 766,023,250bytes/SHA256`0c7c454dc83154e622fff646ac865a2c03fee5b5852ab5f46be24703bf48fb7a`, original development certificate,49native owners/five callback regions,160fresh Unity Edit checks and122mocked offline boundaries. Prior same-source350runtime/service checks remain separate. Previous TutorialR2APK/sidecar retained; earlier dated pins remain historical. Rejected selection/normalization keeps the exact map and selected level; Close/Back dismisses the existing error, with retry disabled until recovery. Start checks normalization before spending. All18physical rows still require native touches, disposable-profile isolation and lifecycle/recovery evidence. No phone/hosting or commercial acceptance claim; no arbitrary board checkpoint.

## 2026-10-03 — round lifecycle recovery build pin

[Current round lifecycle R2 results](../../RoundLifecycleRecovery/R2AndroidRefresh/TestResults.md) verify 766,031,502 bytes/SHA256 `71e042f7eaef9edba33c16916c843aa28737b5432666f166aec1a59a0b3b3984`, original development certificate, 61 native owners/five earlier callback regions, 160 fresh Unity Edit checks and 155 mocked offline checks. Prior same-source 452 runtime/service checks remain separate. The previous selection APK/sidecar is retained; earlier dated pins remain historical. Normal start commits energy/clock/attempt together and initializes the ordinary board once, retaining its booster owner. Rejected writes and checked counter overflow preserve progress/failure/checkpoint; retained in-session retry rules remain intact. All 18 physical rows still require native touches, disposable-profile isolation and lifecycle/recovery evidence. No phone/hosting or commercial acceptance claim; arbitrary mid-round resume is not implemented.

## 2026-10-03 — Settings recovery build pin

[Current Settings R2 results](../../SettingsRecovery/R2AndroidRefresh/TestResults.md) verify 766,032,162 bytes/SHA256 `0310f6a446f268fb36a70c3a7af604cc89b54c5cbc51f19a864316aeaf421adc`, original development certificate, 68 native owners/six callbacks, 160 fresh Unity Edit checks and 184 mocked offline checks. Prior same-source 396 runtime/service checks remain separate. Previous round lifecycle APK/sidecar is retained; earlier dated pins remain historical. Four preferences commit before adoption; rejected saves preserve the switch/feedback state and existing standalone/paused Settings root. Existing localized rejection panel and owned callback/reentry guard are packaged. All 18 physical rows still require native touches, disposable-profile isolation and lifecycle/recovery evidence. No physical audibility/haptics/notifications or commercial acceptance claim.
