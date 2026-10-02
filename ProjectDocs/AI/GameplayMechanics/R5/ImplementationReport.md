# Gameplay Mechanics R5 — attempt and feedback recording

The five-round sandbox now records attempts and offers optional difficulty, goal-clarity and effect-clarity ratings. Recording does not alter round content, mechanics, effects, campaign routing, inventory or saves. It provides evidence for the next balancing pass rather than inventing human feedback.

## Use

Open the Gameplay Mechanics Sandbox from its existing development menu after reaching Home in Play Mode. Play, rate if useful, then Retry or Next. Press **Export session JSON before Exit or stopping Play Mode**. Copy location copies the latest export path, or the export directory before the first export. Open the file in the [local review viewer](Gameplay_Mechanics_R5_Review.html).

Records stay in memory until explicitly exported. Every export creates a new JSON snapshot under `Application.persistentDataPath/MechanicsPlaytests`, with a unique filename and temporary-file rename. It does not overwrite earlier exports. Export again after additional attempts or feedback. There is no auto-save/upload or telemetry service. Closing the sandbox discards unexported memory; exported ongoing attempts remain ongoing snapshots. Export failures retain the in-memory session and show a retry status.

## Recorded information

- Session UUID, schema/content/application versions, UTC timestamps, and source (`manual_sandbox` by default or explicitly `automated_test` in the recording tests). No account/player identifiers, network calls or free-text personal notes.
- Per-attempt sequence, round, rule version, opening and last observed board hashes, move budget/remaining moves, score, remaining goals and foreground elapsed time.
- `won`, `lost`, `interrupted` and `ongoing` remain distinct. Retry, Next, fixture mode, Exit and external disable/destruction close an active attempt at most once. A completed outcome is not overwritten by a later navigation event.
- Interruption during resolution keeps the observed state and sets `snapshotSettled=false`; it is not presented as a completed command or loss. These summaries are not full replay commands; R4's witness files remain separate.
- Optional difficulty (`unrated/easy/fair/hard`), goals (`unrated/clear/unclear`) and effects (`unrated/readable/busy`). A win does not automatically imply easy, nor a loss hard. Ratings can be changed on the current terminal record before moving on; a new attempt starts unrated.
- Foreground duration includes animation/decision time and excludes background and post-terminal time. It is not CPU/GPU/frame-pacing measurement. At most200 closed attempts plus one active attempt are retained; dropped older records are counted.

## Implementation and review

New MechanicsPlaytestLog owns bounded session state and explicit local export. MechanicsPlaytestFeedback builds the developer feedback controls using the existing font and navy/gold palette. MechanicsSandbox observes the board and closes attempt boundaries without changing Match3Board or puzzle rules. New recording tests cover lifecycle, UI feedback, terminal outcomes, interruption, bounded retention and export behavior.

The feedback panel sits in the free area between the board and inventory footer. Actual Unity captures show a win with deliberately entered automated ratings and a loss left unrated. These are test examples, not human findings. The standalone HTML shows one portrait and accepts an exported JSON file locally in the browser. It labels source, lists outcomes/ratings, rejects malformed files and keeps interrupted/ongoing attempts separate from wins/losses. It does not calculate a population win rate or upload files.

**9/9 selected PlayMode tests passed**, zero failed/skipped: recording5, all-round win/loss/retry2, earlier mechanics2. Unity compilation succeeded. Browser checks passed four capture layouts and six automated-session imports at430/1440, invalid JSON/rating rejection, manual/automated source labels and local links, with no JavaScript errors. An initial review-page overflow from the long export-directory string was corrected with wrapping and rechecked.

Scope: one existing sandbox source changed; three new C# files plus metadata, zero deletions;7,300 existing Assets/Packages/ProjectSettings files unchanged. Exact pre-run Addressables link.xml/meta restored after the final Unity process. No generated test scenes remain in the change set.

[Accepted test results](QA/Verification.json) · [Scope audit](QA/Scope.json) · [Automated sample](QA/Samples/Recording_Completed.json).

Verify01's five tests passed, but inspection revealed JsonUtility serializes a null nested active-attempt object as an empty-looking object. Replaced it with an explicit zero-or-one `activeAttempts` array, added the empty-array assertion and reran as Verify02. No empty placeholder is counted as an attempt.

## Limits and next step

No real human session has been collected during this work. Automated timing and ratings are excluded from any claimed difficulty evidence. No physical Android, IL2CPP build, full-project suite or new gameplay-core test sweep ran; puzzle rules and round definitions are unchanged. This is a development tool, not production analytics.

Next: play several manual attempts per round without solution spoilers, export the sessions, then review unclear goals and busy effects together with moves remaining. Use those observations for a bounded difficulty/effect-readability pass before adding more rounds or activating the new rules in campaign saves.
