# Gameplay Mechanics R13 — causal presentation

Implemented the next approved animation stage for PrototypeR4 sandbox play. The approved R10 artwork and five-round move budgets are retained. Regular campaign play is still Legacy.

## What now appears during play

- Matched gems in connected runs gather into their creation cell, followed by the existing special reveal. Separate matches do not fly into an unrelated creation.
- Combination ingredients exchange a short energy link before release. Rainbow combinations visibly replace eligible gems with the planned special artwork, then hold before activation.
- Directional sweeps and area effects clear cells progressively. A chained special waits for incoming contact before beginning its wind-up.
- Firebirds visibly carry the actual row, column or bomb sprite. The matching effect and sound begin at the planned landing cell after flight.
- Obstacle artwork changes at the scheduled hit, so surviving layers appear immediately. Model state still commits at the original deterministic boundary.
- Special items no longer shed ordinary-gem fragments based on their hidden color in the new presentation path.

## Ownership and safeguards

`BoardPresentationTimeline` derives presentation times from immutable wave instructions and the pre-wave snapshot. Forced roots, matched-cell roots, rainbow conversions, line/area footprints and explicit flight targets determine earliest contact. It does not modify board state, RNG, snapshots, scoring or replay serialization. Existing core rule sources are unchanged. The helper reports unlinked activation fallbacks for diagnostics; tested combinations must have zero.

`Match3BoardPresentationR13` owns the coroutine and emits contact, gather, conversion and payload events through the existing motion stream. Existing effects/audio services receive these events; ordinary shatter is emitted at each contact batch. Generic overlapping legacy activation effects are suppressed for R4, and pools remain bounded. The session content version is `MechanicsRoundsR4-RulesR4-PresentationR13`.

No prefab rebuild is needed: these are runtime-generated sandbox board components. No artwork, fonts, localization, packages, graphics project settings, campaign rules or level content were replaced. The previously authorized Vulkan override is used for verification processes only.

## Verification and review

Final Unity6000.5.9f1 results: 86 EditMode and 62 PlayMode checks passed, zero failures/skips. Initial Verify01 EditMode85/85 and Verify02 focused animation4/4 passed. Verify03 added the all-pair timeline check:86/86. Verify04 broad run61/62 failed because the older fragment test injected wave-start instead of the new contact event; the fixture was updated and converted-item hidden-color fragments were separately corrected before Verify05 passed62/62. See [Verification.json](QA/Verification.json) for the final run totals and [Scope.json](QA/Scope.json) for exact paths. Tests include all 15 special pairs, directional contact ordering, rainbow reveal-before-release, payload landing delays, creation gather, final state hashes, one-move input completion, retry cleanup, historic replay compatibility, five rounds, recording and existing audio/effect regressions.

The review includes six sampled actual Unity sequences: creation, line-to-bomb chain, rainbow conversion, and three Firebird payloads. Playback shows up to 20 opening samples followed by the final settled state; it preserves intervals between those opening samples and adds explicit review holds before/after. It contains no generated/interpolated frames and no recorded audio. Still frames are selectable for closer inspection. Automated capture overhead affects cadence; this is not a device frame-rate benchmark.

## Remaining

Player acceptance of impact and readability, physical-phone performance/audio/haptics, broader material-specific debris, and production campaign migration remain outstanding. This pass improves sequencing using the accepted artwork; it is not a claim of exact Royal Kingdom animation parity or final visual approval.

Task-start scope: 9 existing source/test files modified, 8 new helper/test/meta paths, 0 deleted, 7,391 unchanged. Temporary test scenes/linker state were cleaned/restored after Unity exited. Redundant task-generated older PNG copies were pruned; test logs/results and final R13 captures remain. See QA/PrunedRedundantCaptures.json.

Browser verification:238 still-frame/viewport and12 animated-image checks passed at430/1440, no script errors or horizontal overflow. Selected Unity frames and mobile review visually inspected. See QA/BrowserVerification.json.
