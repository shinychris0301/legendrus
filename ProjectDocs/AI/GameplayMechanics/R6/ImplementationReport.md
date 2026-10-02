# Gameplay Mechanics R6 — Reliable local sessions

The five-round development sandbox now checkpoints attempts and optional feedback locally. The user approved improving collection before difficulty tuning because no manual session exports were found. R5 automated examples are not evidence of human difficulty or readability preferences.

## Behavior

- Checkpoint after opening a round, settled board transitions, terminal outcomes, feedback changes, Retry/Next/fixture changes, backgrounding, and normal Exit/disable/destruction/application quit. A 15-second realtime checkpoint also retains elapsed foreground time during play.
- `Saved locally` confirms the last successful checkpoint. `Save now` retries storage errors. Export remains an explicit, uniquely named snapshot; later checkpoints never rewrite that export. Copy location copies the latest explicit export, otherwise the session checkpoint.
- Normal interrupted exits retain the observed board state, moves, goals and reason. Win/loss stays terminal even if disable/destruction follows. An abrupt shutdown can leave an older **ongoing** checkpoint; that is not counted as a win, loss or completed attempt. This saves observations, not resumable board cells.
- New sandbox openings start a new UUID and fresh attempts; earlier files remain readable after the sandbox is gone. The review's local JSON viewer accepts both checkpoint and export files using the existing schema1.

## Storage and isolation

`Application.persistentDataPath/MechanicsPlaytests/Sessions/manual_sandbox/playtest-<session>.json` stores manual sessions. Automated sessions use `Sessions/automated_test`; batch runs are automatically tagged and older sandbox tests now also pass their automated flag explicitly for interactive Editor test runs.

A checkpoint is written to a sibling temporary file, flushed, then moved or replaced atomically. Replacement retains the previous bytes in `.json.bak`. There is no delete-first fallback. Failed writes leave the prior file and session memory intact and display a retry status. Unchanged snapshots do not rewrite the file. This is tested local filesystem behavior, not a physical power-loss or Android durability guarantee.

Each session retains at most 200 closed attempts plus one active attempt and reports its discarded count. Each session uses one primary and one backup; older sessions are not automatically deleted. A damaged primary can be inspected by copying the `.bak` file to a separate `.json` file. Do not count a primary, backup and explicit export of the same session as separate sessions. Nothing is uploaded. No account identity, campaign state, PlayerPrefs or live save provider is accessed.

## Scope

Runtime: MechanicsSandbox, MechanicsPlaytestLog, MechanicsPlaytestFeedback and new MechanicsPlaytestStorage. Tests: new storage/lifecycle coverage plus explicit automated flags in the older round, mechanics, Spark and seal test classes. Approved prefabs/art, production routing/startup, mechanics rules, budgets, layouts, inventory and campaign saves are unchanged.

## Verification

Unity 6000.5.9f1 storage checks passed **6/6** in two runs: Verify04 verifies actual win, feedback, Exit and a new sandbox session; Verify05 verifies replacement/previous copy, unchanged-write suppression, forced commit failure and retry, unique session/source separation, focus/pause timing, mid-animation disable/destruction, application-quit callback and a visible save-error/retry UI. Tests use temporary folders and automated data. The lifecycle tests exercise the callbacks, not a physical device shutdown.

Two actual 576×1280 Unity captures (saved win and deliberately blocked save) were inspected for status and control fit. Verify01–03 hit native SIGSEGV during the second offscreen capture and produced no test-result XML. Waiting for impacts to expire did not fix the combined run. Splitting the capture tests allowed both to pass. Verify06's combined regression encountered the same capture symptom; Verify07 runs the existing behavioral tests without optional offscreen capture. This Editor capture instability remains unresolved; no graphics backend/settings changes were made.

Verify07 passed **12/12 existing behavioral PlayMode tests**: recording5, round win/loss/retry2, earlier mechanics/boosters2, seal2 and Spark1. Total accepted results: **18/18**, zero failures/skips. The optional screenshot mesh assertions were not executed in Verify07; real-board behavioral assertions were. Browser checks passed four capture layouts and four sample imports at430/1440 widths, outcome totals, source labels, malformed JSON/rating rejection and local links, with no JavaScript errors. No new EditMode sweep was run because gameplay core is unchanged.

Scope audit: seven existing C# files changed (three runtime, four test flags), four new paths (storage/test files with metadata), zero deletions, 7,300 unchanged Assets/Packages/ProjectSettings files. Exact pre-run Addressables link.xml/meta restored. Removed eight runner-generated scene/meta paths left by the crashed runs, all absent from the pre-task inventory. Prior local edits preserved. See QA/Scope.json, QA/RemovedTemporaryScenes.json, QA/Verification.json and QA/BrowserReview.json.

## Next

Collect manual attempts on all five rounds without reading solution witnesses. Inspect persisted JSONs for remaining moves, interruptions, goal clarity and effect clarity. Tune only issues supported by those observations before expanding the round set. No manual results, human win-rate estimate, difficulty acceptance, physical Android test or production rule migration is claimed by this pass.
