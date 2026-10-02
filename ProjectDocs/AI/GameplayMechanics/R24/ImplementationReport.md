# R24 — Remove circular overlays and correct effect continuity

The user reported an unwanted circle when match-created items animate, and asked why the animations still look simple. The cause was overlapping presentation systems, plus several continuity and geometry defects. This pass fixes those defects in PrototypeR4. It does not change campaign rules or claim a complete animation redesign.

## Findings and corrections

1. `CoreLoopVfxPresenter` still emitted `SpecialCreationHalo`, cascade and booster pulses while dedicated R4 renderers played. These pooled defaults draw expanding rings. R4 now keeps only the non-circular swap trail from this presenter. Cue/trace diagnostics, gameplay events and audio remain intact; earlier rule versions retain their presentation.
2. `PrototypeImpactView` independently queued generic creation charges. These are no longer queued in R4. R4 rich effects no longer draw circular Glow/Ring geometry: immediate contacts use compact, short-lived faceted highlights; targeting uses corner brackets.
3. `CreationAccentsGraphic` doubled alpha at the transition into completion (0.5 to 1.0). Completion now inherits the actual gather opacity and fades continuously. Opposed ribbons/orbiting crescents were replaced with open accents. Approved item bodies remain unchanged.
4. `PrototypeImpactView.Vertex` clamped every vertex against board edges, flattening effects into streaks. R4 now preserves raw geometry under a RectMask2D, using the existing clipping-capable material. Earlier rule versions keep their existing path.
5. The bomb repeated one blast shape six times around its center. The six lobes are removed; the evolving central blast, shell fragments and authoritative contact schedule remain.

Some families still rely on a few cross-faded sprite poses and scale/fade transforms. That limits their visual richness even after the glitches are removed. Next art work should replace those remaining pose-limited sequences with distinct formation, launch, contact and dissipation layers, beginning with color and Firebird activation. It should be judged in motion before a campaign rollout.

## Scope

Nine modified runtime/test sources, no added/deleted files under Assets/Packages/ProjectSettings; see [Scope.json](QA/Scope.json). Runtime: CoreLoopVfxPresenter, PrototypeImpactView, PrototypeImpactViewR15, CreationAccentsGraphic, SpecialCreationGraphic, SpecialSequenceView, MechanicsPlaytestLog (PresentationR24). Tests: MechanicsCreationFinishTests, MechanicsSpectacleTests. This is the existing runtime-created sandbox composition; no prefab/builder regeneration is needed. No new art, package, project graphics setting or core matching/damage/timing change. Production campaign remains Legacy.

## Verification and review

- Unity 6000.5.9f1 PlayMode: initial broad run **87/87 passed**; after the final pooled-presenter fix, focused run **29/29 passed**, zero failures/skips. The focused run includes generic presenter regressions, sequence recordings, creation lifecycle, opacity continuity and the new no-pooled-ring assertion. These are separate runs, not one final broad run. [Results](QA/Verification.json).
- Mesh regression verifies an actual board mask and preserved off-board geometry; existing checks cover clipping material behavior, bounded meshes, recovery and disabling effects.
- Previously authorized process-only Vulkan and descriptor limit 4096; isolated test configuration. No new EditMode run or physical Android profiling.
- Actual Unity review: nine sequences, 296 portraits, eighteen normal/half-speed animations, previous R23 comparisons. Recorded intervals, no interpolation/audio. Inspected creation, color formation, row launch, bomb and mobile HTML captures. [Review](Gameplay_CircleCleanup_R24_Review.html).
- Browser validation at 430 and 1440 pixels: 592 still loads, 36 animation loads, 18 comparison loads, zero JavaScript errors or overflow. Source diff whitespace check passed.
- Removed 686 unreferenced task-owned numbered PNGs with hashes recorded in [PrunedFrames.json](QA/PrunedFrames.json). Final review frames, before/after evidence, logs and results retained. Restored linker state and removed only newly created temporary test scenes.

Synthetic test boards can show 49/4 moves; this is an existing fixture HUD mismatch. This candidate still needs user visual acceptance and physical-device performance measurement. No Git commit was made.
