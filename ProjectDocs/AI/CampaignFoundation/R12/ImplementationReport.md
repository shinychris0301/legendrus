# Campaign R12 — completion flow and conservative tuning candidate

The recorded campaign preview now ends with an explicit completion panel after round 20's victory and final story. Players can save the recording, exit, or deliberately replay round 20. A separate R12 menu loads the four adjusted move allowances; the R9 candidate and its recording identity are preserved.

## Implementation

- `CampaignDevelopmentUI` reuses the approved EmptyErrorState panel and button materials with live English/Russian completion text. No prefab asset or production route was rebuilt. The runtime composition owns its temporary layout. Its error emblem is hidden. The recorder's explicit end-round setting gates this state; ordinary previews retain their previous behavior.
- The coordinator still acknowledges the result before offering pending story and only then calls the map route. The completion view does not advance selection, spend energy, grant rewards or complete the chapter. Replay opens the selected round's Pre-Level page; pressing Play starts a normal new attempt. Exit saves first; local save failure remains visible and retryable.
- The editor monitor identifies the content revision, offers completion Exit/Replay and retains Save/Copy/Show recording controls. There is no automatic upload. The game panel offers Save; revealing the file is handled by the existing editor monitor.
- `CampaignPlaytestRecorder.Attach` adds an optional terminal round, disabled by default. Both explicit recorded menus enable the round-20 ending. Existing sandbox logging, serialization and ordinary recorder callers retain their contracts.
- `EditorContent/CampaignCandidates/Chapter01Sector01R12/levels.json` is a separate full level catalog, changing only move limits 13:28→25, 14:29→26, 17:29→26 and 19:29→25. The R12 loader deliberately shares R9 story/localization files. Production startup does not discover this editor-only candidate. Content contract revision remains 1; R12 identifies an experiment, not a published content release.
- The R12 analyzer derives the exact four-file fingerprint and reuses R10's source/deduplication/booster separation logic. The supplied R9 human recording is excluded as different content. Zero R12 human sessions have been evaluated.

## Why these allowances

The first human session showed a generous normal-round margin but a loss on15 and materially longer clears on15/20. Three-move reductions on13/14/17 and four on19 are a controlled first experiment. They keep substantial headroom above the observed successful attempts. No blanket reduction, new obstacle, goal inflation or effect change was made. Untuned recovery/teaching/peak rounds preserve the surrounding rhythm.

## Verification

See [machine-readable verification](QA/Verification.json), [content comparison](QA/ContentDiff.json), [scope](QA/Scope.json), and Unity logs/results in QA/Verify01 and QA/Verify02.

The targeted test suite validates only four move-limit changes; replays thirty existing inventory-free solutions against the tuned goals and budgets with exact board tokens; checks unavailable21; runs an actual animated20 win followed by story/completion/replay; verifies no currency/energy changes on repeated completion; confirms failed20 and default recorder callers do not show completion; and exercises local-save failure/retry/exit without campaign mutations. Progress through19 is explicitly a transaction-created test fixture, not a human journey.

The first capture showed clipped English body text and a wrapped Russian replay label. Completion-only layout and wording were corrected and the targeted suite rerun. Final English941×1672 and Russian576×1280 images in the review are actual Unity render captures on the test harness's black background. They do not show a full Home-to-completion human walkthrough.

No Android, full test suite, manual editor-menu walkthrough or new human balance acceptance is claimed. Authorized process-only Vulkan/descriptor4096 verification leaves project graphics settings unchanged. Gameplay animations/art remain at their accepted baseline; all further effect refinement stays last.

## Follow-up

Use [playtest instructions](PlaytestInstructions.md) for the R12 recorded menu. Rate12/13/14/17/19 before Continue; share the new JSON. Review those results before proceeding to21–30/granary. Preserve the original R9 evidence for comparison; do not combine both revisions into one outcome rate.
