# Victory — Rewards, Layout and Depth R2

Implemented the approved first reference-fidelity refinement in the existing native Victory page: one enclosing wood/gold rewards panel, compact level/score/reward status, higher and wider lower controls, visible bottom ornament, larger beveled live gold lettering and deeper earned/unearned stars. The story reaction now uses the footer's wood surface; a video-status message temporarily takes its place, so they cannot overlap.

[Standalone current Unity portraits, R1 and reference](Victory_RewardsAndDepth_R2_Review.html) · [Reference comparison](ReferenceComparison.md) · [Source preservation](QA/SourceAudit.json).

## Implementation

- Canonical `Resources/ProductionUI/Pages/Victory.prefab` and owning runtime/builder agree on revision2. Upgrade reuses the existing hierarchy, including reparented score/reaction text, and retains every previous serialized object ID.
- New independent `rewards_panel_r2.png` provides a genuinely opaque wooden surface and transparent outer contour. Cards, text, quantities and actions remain separate native elements. All1942×809RGBA pixels match the generated source; PNG was re-encoded without pixel edits. No screenshot crops.
- The layout reserves space for the entire footer at reference height and scales the body within the remaining safe area. Existing Firebird/banner sprites remain individual assets. Continue and video controls are wider and begin60reference units higher than R1. The panel groups the level/score and actual reward status above its cards.
- Victory title and Continue use the existing Home bevel material/mesh effect with a stronger gold gradient and larger bold serif text. Font assignment still comes from the project's typography resolver; exact reference font identity is not claimed. Shared material and shared text-effect source are unchanged.
- `ProductionVictoryStarDepth` supplies a page-only layered gold lip, beveled border, shaded facets and dark unearned recess. Existing one-time reveal remains; earned halo alpha is stronger. Gameplay/World Map star rendering is unchanged.
- Rewards, replays, transient score snapshot, provider guard, navigation, wallet and inventory rules remain unchanged. The optional action still states coins-only doubling. Story localization continues to use the existing service and may fall back to Russian.

## Actual changed files

Seven existing files changed relative to the verified R1 baseline:

- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Victory.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionVictoryRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionVictoryView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionVictoryLayout.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/VictoryAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/VictoryAlignmentEditModeTests.cs`

Added `Scripts/Runtime/UI/ProductionVictoryStarDepth.cs` and `Resources/ProductionUI/VictoryAligned/rewards_panel_r2.png`, plus their metas:four new files. See [source diff](AppliedSourceChanges.diff) and [full manifest](FinalProductionHashes.json).

Verified/backed up all1,750R1 production files before editing. Final audit:1,743unchanged, seven changed; all905existing metas and304prior Victory object IDs retained (313now). Final production manifest covers1,754files. GameState, ScreenRouter, other page prefabs/assets, shared font/material/mesh-effect sources, packages, settings, signing and Android guards are unchanged.

## Artwork and visual review

Used the builtin imagegen tool for one standalone blank panel. Saved the source, exact prompt, runtime path and provenance in [Sources/Provenance.json](Sources/Provenance.json) and [PanelPrompt.txt](Sources/PanelPrompt.txt). Viewed the generated sprite and composited it over black, white and magenta (`QA/PanelAlphaBackgrounds.jpg`). Solid wood remains opaque; no local background-removal step was required.

Inspected actual R2 reference-sizeRU three-reward, narrow-phoneEN five-reward and unavailable-video portraits in both runs. The first run exposed text too close to the generated panel's carved edges, despite passing rectangle containment checks. The final layout provides more wood-surface clearance, moves reaction/status to the footer and includes stricter text inset checks. The reference and R1 images were inspected in the preceding review that this approved task implements. No new visual acceptance is assumed.

## Exact checks

Unity6000.5.9f1, isolated save/config storage:

- Two compilation/owner-rebuild/save-readback runs passed.
- First run:6/6EditMode and12/12PlayMode passed. Visual inspection then prompted the panel-content correction.
- Final run:6/6EditMode passed; all6Victory PlayMode tests passed. Existing Pre-Level regression passed5/6; its remaining test failed strict JSON save equality solely because `lastEnergyUtcTicks` advanced during a slow capture run.
- Targeted unchanged retry of that Pre-Level test passed1/1 in2.24seconds. No production or test edit between failure and retry; assertion was not weakened. Full failed result and retry result are retained in [QA/TestHistory.json](QA/TestHistory.json) and `UnityVerification/RegressionRetry/`.
- Victory coverage retains all ten authored reward configurations, one/two/three stars, lower-scoring replay, no-result/reset, gold-only/once-only domain operation, unavailable-provider path, Continue/double-click/Shop and real registered Gameplay win-callback wiring. New assertions check enclosing panel/card clearance, result text inset from trim, footer bottom anchoring, gold-text binding and rendered star-bevel geometry.
- RU/EN captures exercise941×1672,576×1280 and1080×2400 with simulated safe areas; text fitting and native button raycasts checked. The real board callback is invoked by the fixture; this is not a full gem-swap win test.
- Retained11final Victory PNG/JSONpairs,17Pre-Level pairs from the final combined run and10pairs from the targeted retry. The retry evidence is kept separately without replacing failed-run evidence. Source/preservation and whitespace checks passed. Chrome checked all13portrait choices, three report links and360px horizontal overflow; all passed and the standalone screenshot was inspected. Results are recorded in `QA/BrowserChecks.json`.

## Not performed / remaining differences

No APK/AAB, physical Android testing, full project suite, live successful rewarded-ad transaction, profiling/long-session test, calibrated color measurement, exact font identification, or manual review of every capture. Existing automated text checks cover more portraits than the ones directly inspected.

The reference's evening-city scenery, slimmer luminous Firebird silhouette, three-part wood/gold header, curved title lettering, specific footer crest and confetti still differ. This focused pass improves rewards/layout/typographic depth; it does not claim a pixel-identical page. No changes to sample reward values or unimplemented egg rewards were made. The next visual task is the scenery/Firebird/header alignment, after reviewing this refinement.
