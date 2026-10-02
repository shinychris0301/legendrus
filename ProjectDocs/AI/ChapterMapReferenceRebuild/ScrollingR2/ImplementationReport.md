# Chapter Map scrolling and World Map scenery — R2

The active Chapter Map now uses a portrait environment derived from the approved World Map Chapter 1 painting. This follows the user's latest direction and supersedes retaining the earlier church-and-windmill reference scenery. Shared Home/World Map header, original chapter banner, Play/Back controls, saved progress and live level numbers remain native.

## Scenery

One Scenario `model_openai-gpt-image-2` request, one accepted output, **13 CU**, no retries. The input was `WorldMapReferenceRebuild/DesignR1/Assets/region_01.png`. The portrait expands the same carved timber village, birches, gardens, autumn foliage and amber lamps around a winding route. The original painting and earlier Chapter Map environment are preserved. Output imported without bitmap edits as `environment_worldmap01.png`; source/hash/prompt/job/billing/output records are in Sources/Scenario/chapter01 and Sources/SceneryProvenance.json.

This is a new composition based on approved art, not exact recovered source pixels. It has no baked UI. Three reused native scenery meshes overlap and fade at their ends; the map viewport softly clips them. One painting repeats along the route. The node path follows sampled road positions. Sources/ChapterSceneryPlan.json maps all twenty chapters to their approved World Map paintings; only Chapter 1 has received its portrait expansion. Future chapters must use their own themes rather than repeat this village.

## Map behavior

Revision 3 contains ten recycled level buttons for **500 planned positions**, with about seven visible at a time. Later rounds are above; dragging downward reveals them. Scrolling uses unscaled inertia, clamps at both ends, tracks the owning pointer, and cancels click eligibility while dragging. Current-level shortcut returns to the selected saved round. Header/title/footer stay fixed. Three-digit round numbers use a smaller font inside their existing medallions.

Only the existing ten authored Chapter 1 rounds are playable. Unpublished slots stay locked, show an upcoming-level message, and cannot alter saved selection or enter gameplay. Recycled button callbacks read their current level IDs. There are no 500 instantiated buttons or invented completion/star values.

## First gameplay effects pass

`CoreLoopEffectGraphic` replaces flat pooled Image geometry with temporary faceted match particles, bomb/Firebird impact rings and embers, and thin horizontal/vertical booster trails. The existing pool, cue durations, clipping, grid visibility and raycast exclusions remain in use. This adds no persistent gem fragment beside a booster.

Match events now carry the resolution plan's actual cleared-cell instructions. A bounded sample of up to three cleared cells emits short facet bursts, including cascades. The event is presentation-only; deterministic clearing, scoring, boosters, inventory costs and the production 8×7 board are unchanged. No new image-generation request was needed for these native effects. Royal Kingdom informs effect roles/readability; these are original restrained effects in the existing art style, not extracted Royal Kingdom assets or a claim of exact animation timing.

Effects review images are **controlled Unity 8×9 QA stress fixtures with hit-area gem visuals hidden to expose the effect**, not full gameplay screen captures or an 8×9 production-board change. Separate tests execute the actual five inventory-booster variants and verify spending, recovery/refunds and cleanup.

## Verification

**Final Verify07: 12 EditMode + 37 PlayMode = 49 passed, 0 failed or skipped.** Final results are archived in QA/EditMode and QA/PlayMode. Unity 6000.5.9f1 rebuilt and read back the canonical prefab. Targeted coverage includes serialized bindings/imports, Home/World Map regressions, RU/EN and simulated safe areas, scrolling bounds, pointer ownership, current-level jump, node reuse, unpublished clicks, earned stars, real booster executions, native geometry, cleanup and pool reuse. Native map and effects captures are in QA/ChapterMap and QA/VFX.

Earlier diagnostic failures are retained separately: old VFX fixtures initialized from the current 8×7 catalog but loaded 8×9 QA snapshots; the fixture now explicitly uses its snapshot dimensions. New test compilation used an unsupported CanvasRenderer.GetMesh overload, corrected to the local Unity API. A diagnostic assertion initially omitted the existing secondary combination cue; it was corrected without removing the primary-cue/lifetime checks. Visual inspection prompted current-caption alignment, three-digit fitting and stronger local impact geometry.

No Android build, physical-phone test, full project suite, device performance profiling, new audio or hardware-input verification was performed. The 500-round catalog and story progression have **not** been migrated: the legacy story catalog still defines its finale at round 10. Relocating this gate, authoring/playtesting the first 50 rounds, then expanding content are required before releasing a 500-round chapter. This delivery is the scrolling presentation foundation and initial effects work, not 500 finished rounds.

## Files and next work

QA/ScopeAudit.json records changes against this turn's Assets hash snapshot. Existing GUIDs and prefab records are retained; there are no art deletions. Only ChapterMap's prefab changes. Other page art/prefabs and the existing World Map paintings are preserved. Scope: ten existing Assets files changed, eight added, zero deleted. All 751 original prefab records retained; 780 total. Home and World Map native RU captures are pixel-identical before/after. Thirteen browser review choices passed at 390×844 with no errors, missing images or horizontal overflow.

Next: review the Chapter 1 portrait/map composition, then migrate published-content and story-completion gates and author the first 50 rounds. Continue each later chapter's background from its corresponding World Map painting as its content is implemented. VFX still needs play-session and device-performance review before final visual acceptance.
