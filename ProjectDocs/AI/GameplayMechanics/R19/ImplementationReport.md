# R19 — Artwork-based effects in the mechanics sandbox

The user approved continuing after the R18 artwork sequence review. This pass integrates those assets with the existing PrototypeR4 presentation events. The campaign remains on Legacy rules; this is not a campaign rollout or a completed overhaul of every booster family.

## Implemented

- Imported the two R18 transparent atlases unchanged, with their measured, nonuniform region boundaries. Existing gem and idle special artwork remains unchanged.
- Four-match and L/T-match formation use crystal/bomb assembly poses. Gathering holds a visible core until the committed creation replaces it; the idle item fades into that final pose.
- Row/column activations launch crystalline projectile artwork in both directions, including combined attacks and carried payloads. Bomb activations use ignition, flame and smoke poses plus separate shell fragments. The existing contact timeline remains authoritative.
- Ordinary gem clears use six color-specific fracture regions with varied trajectories, spin and scale. Converted/hidden ordinary gems do not emit fragments.
- Wood, ice and chains emit corresponding material artwork. First hits are restrained and retain the damaged obstacle; final breaks are larger. Existing damaged-state sprites and obstacle rules are preserved. Runic seals retain their prior presentation.
- Reduced the previous dominant procedural rings/stars behind the new artwork. The mace gains the new line/blast treatment; Firebird flight, rainbow links and other inventory-booster families retain their previous presentation.
- Fixed capture timestamp name collisions between material and spectacle suites. Review animations use the timestamps for their own captured sequence.

## Integration and limits

`Match3Board.InitializeMechanicsSandbox` attaches two clipped, non-interactive `SpecialSequenceView` mesh layers only for R4. Both subscribe to existing motion/contact events and clear on recovery/disable. Each layer is bounded to 128 cues; oldest cues are dropped at capacity. There are no per-fragment GameObjects or gameplay RNG changes. Six existing gem batches remain bounded to 16 breaks each, with six textured quads per break. The atlas helper uses the existing UI material; no new shader, graphics backend, project setting or package is required.

The existing import postprocessor applies the project's sprite/alpha policy. Texture2D access samples measured atlas UVs; mipmaps remain disabled and the source dimensions are retained in the verified desktop process. Android quality/performance has not been verified. The atlases remain in Resources and will add build size even though presentation attachment is sandbox-only.

This sandbox is runtime-created; there is no owning serialized production prefab to rebuild. Existing prefabs, core resolution, move budgets and campaign selection are preserved. Session content version advances to PresentationR19.

Four poses are crossfaded per main sequence; these are not dense flipbooks. Small ornament differences between formation art and exact idle sprites remain a visual-review item. The first/final obstacle captures use separate controlled starting states. Some synthetic fixtures load a 50-move board over the four-move playtest HUD; the resulting 49/4 caption is fixture-only, not changed gameplay balance.

## Verification

See [test results](QA/Verification.json), [scope](QA/Scope.json) and [animated Unity review](Gameplay_Mechanics_R19_Review.html) for final evidence.

Verification uses Unity 6000.5.9f1 with the previously authorized **process-only Vulkan** option and child descriptor limit4096. Project backend settings are not changed. Early focused runs found a missing CanvasRenderer requirement and two fixture mistakes (direction and expected move budget); these were corrected. The renderer requires CanvasRenderer explicitly. Focused Verify03 passed4/4 before the final core-hold adjustment. Final Verify04 passed **79/79 selected PlayMode tests**, zero failures/skips, after the core-hold adjustment. This includes outcome hashes, all15 special pairs, five round replays, contact timing, recording/storage, audio and effect regressions plus the new artwork lifecycle tests. Intermediate runs are not added to that count. EditMode was not rerun for this presentation-only pass.

The final captures are sampled actual Unity output, not generated composites or a phone frame-rate benchmark. Playback preserves sample intervals, with review holds before/after; no interpolated frames. Sound is not recorded. Physical Android profiling, human playtest acceptance and broader family expansion remain next steps before any deliberate campaign migration.

Task-start SHA audit:8 existing source/test paths changed,11 helper/test/asset/meta paths added,0 deleted,7413 unchanged. All GameplayCore files, prefabs, packages and project settings match the task-start inventory. Temporary test scenes were removed and the original linker file state restored. Superseded task-owned early PNGs were removed with a SHA manifest; final captures and all run logs/results remain.

Review verification:25 lossless animated sequences and430 actual Unity portraits;860 still-load checks,50 animation-load checks and16 R16 comparison loads passed across430px/1440px viewports. No JavaScript errors or horizontal overflow. Full-size links, still/play controls and local report links checked. Mobile HTML and actual formation, combo, bomb and wood-fragment captures visually inspected.
