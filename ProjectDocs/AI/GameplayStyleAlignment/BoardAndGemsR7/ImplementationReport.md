# Gameplay board and gems R7 — implemented and verified

[Open the standalone Unity review](Gameplay_BoardAndGems_R7_Review.html).

The user authorized the recommended first refinement: the board and six normal gems. R7 replaces large enamel-like emblems with cut-glass pieces, increases padding, adds a dark wood surface and fits an antique-gold carved frame around the existing **8×7** grid. Gameplay and Pre-Level collection goals use the same new sprites. Header and rimless-booster-card refinement remain subsequent passes; R7 has not yet been visually approved.

## Implementation

- Added `gem_ruby_r7`, `gem_sapphire_r7`, `gem_emerald_r7`, `gem_gold_r7`, `gem_amethyst_r7`, `gem_orange_r7`, `board_frame_r7` and `board_wood_r7` under `Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned/`, with new unique metas. Existing artwork and GUIDs remain intact.
- Mount changed from 808×707 to **808×748** at the existing reference origin. Responsive fit uses the matching height. Cell input still fills each nominal cell; the common visual container now occupies 80% of its width and 86% of its height, with sprite aspect preserved. Specials share this smaller container but retain their source artwork and actions.
- Cell overlays reveal the independent opaque wood texture. Thin internal joints use subdued gold. The previous frame object/component remains serialized, disabled, with a separate non-interactive `CarvedFrame` Image child.
- Initial stretched-frame captures exposed overlap with top/bottom gems. Corrected the image to use protected corner slices: source border 128 pixels, display multiplier 4, center drawing disabled. Final captures show unobstructed gem tips/corner cells. [Correction evidence](QA/FrameFitCorrection.json) records the rejected first fit.
- Active Gameplay prefab, owner marker, resource mapping and runtime view agree on R7. Required bindings now include the carved frame sprite. The Pre-Level prefab was rebuilt to serialize the matching collection icons; its page revision and layout remain 2.

No mission targets, level dimensions, move limits, rewards, matching rules, stock or booster functions changed. The only changes inside `Match3Board.cs` are its two visual-anchor assignments; the source audit checks that exact restriction. Two missions in rounds 1–5 and three in rounds 6–10 remain.

## Artwork provenance

Built-in imagegen produced eight independent components from saved descriptions. [Prompts](GenerationPrompts.json), unchanged source copies in `Sources/` and [provenance](GeneratedAssetProvenance.json) record the inputs and outputs. No reference-screen region was cropped into runtime art.

All outputs had real alpha where required. Previously authorized local transparency cleanup restored alpha ≥240 to opaque, cleared alpha <4 and trimmed only empty exterior canvas. Retained RGB is unchanged. There was no local recoloring, rescaling or checkerboard reconstruction. Wood remains fully opaque; the frame's central window is transparent. Inspected all eight source images and three black/white/magenta proof sheets; an independent reviewer also inspected these proofs. Individual proofs and alpha counts are saved under `QA/`.

## Verification performed

- **Unity 6000.5.9f1 (b57deb96f08d):** two verification runs, each rebuilding and reading back Gameplay and Pre-Level successfully. First-run tests passed but direct visual inspection found the frame overlap; run 2 validates the correction.
- Final **EditMode 8/8 passed**, no failures/skips. Gameplay and Pre-Level fixtures cover canonical bindings, idempotent rebuilds, retained controls, source-sheet integrity/imports, frame transparency and prior-page revisions.
- Final **PlayMode 25/25 passed**, no failures/skips. GameplayAlignment, PreLevelAlignment, LevelGoalRuntime and LevelSystemRuntime fixtures cover all authored levels, 120 seeded playable starts, 2/3-goal completion/snapshot behavior, booster/card input, pause, localization and routing/rewards. Board capture checks validate all 56 input roots at center/corners, safe-area containment, gem aspect and padding. This does not prove level win rates or every special combination.
- Produced **24 final PNG/JSON capture pairs**: 15 Gameplay and 9 Pre-Level. Directly reviewed eight final portraits: Gameplay RU/EN 941×1672, RU 576×1280 and 1080×2400 safe areas, round 6 RU, round 10 EN; Pre-Level round 8 EN and round 10 rewards RU. An independent reviewer checked final reference/narrow RU frame clearance. These are actual Unity captures with simulated displays, not phone screenshots.
- R6→R7 reference-size RU image comparison found **zero changed pixels outside the board/frame and mission-panel regions**. Portrait, footer and scenery above the board are pixel-identical in that comparison. [Results](QA/UnchangedAreasComparison.json).
- Source audit: **12 existing files modified, 16 new files** (eight PNG/meta pairs); **1,663** baseline files unchanged. All **867** existing metas preserved, new GUIDs unique, all **391** original Gameplay object IDs retained and Pre-Level IDs retained. Final manifest: **1,691** files. Earlier pages, original character/booster art, font configuration, core rules/VFX, level catalog, packages/settings and Android guards match the recorded baseline. [Audit](FinalSourceAudit.json), [exact file manifest](SourceChangeManifest.json), [incremental diff](AppliedSourceChanges.diff).
- C# whitespace checks passed. Whole-working-tree `git diff --check` reports 81 trailing-space warnings in Unity-serialized empty YAML values in Gameplay/Pre-Level prefabs; retained the actual Unity save/readback output. This broad diff includes earlier local changes. [Check details](QA/WhitespaceChecks.json).
- Standalone review browser checks: 25 portrait choices, four local links and 360px layout passed. [Results](QA/StandaloneReviewBrowserChecks.json). This verifies the viewer, not gameplay execution.

Commands, run results and XML are archived in `UnityVerification/`. Raw logs remain outside the patch, with paths/hashes recorded. The final evidence manifest covers 63 files. CurrentTask and SessionLog have been updated without removing earlier decisions.

## Exact existing production/test files changed

- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/PreLevel.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/GameplayAlignmentArtImporter.cs`
- `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayArt.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayCell.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayHudStyle.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/GameplayAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/GameplayAlignmentEditModeTests.cs`

## Not performed and remaining work

No APK/AAB build, physical-phone test, long-session comfort study, profiling, full-project/pure-core suites, new earlier-page captures, VFX video, exhaustive special/damage combinations, win-rate simulation or complete human playthrough. No pixel-exact comparison against the latest upload because its local source copy is unverified. Artwork is adapted, not an exact reconstruction.

Booster rim/backing isolation, card label space, header plaque/score/star detail, mission alignment, scenery composition and special/obstacle effects remain open as described in [the comparison](ReferenceComparison.md). The existing unassigned font asset/development fallback remains unchanged; device/release font consistency is not established by these Editor captures.
