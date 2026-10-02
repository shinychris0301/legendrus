# Events R3 — extraction-first reimplementation

Reimplemented the active native Events page using the user's modified Scenario workflow. The owner, runtime, prefab and cutover now agree at revision3. Source-derived UI artwork and original subjects replace the R1 redesigns. Existing routes, live text/balances, disabled participation, Upcoming default and empty Active/Completed states remain intact. Gameplay and other pages are unchanged.

The user explicitly approved showing the original rewards as **labelled examples**. Both Russian and English headings identify them as examples; no quantities, promised grants or invented event schedules were added. Only the first event opens the existing EventDetail preview. EventDetail itself remains outside this visual task.

## What was extracted, repaired or generated

- Source assessment and crop bounds recorded before isolation in Sources/AssetAssessment.json.
- Original title plaque, angular tab, green action, narrow status strip and card frame. Baked lettering/shadows removed locally within recorded masks; original RGB outside those regions preserved. Hidden title/card faces use source texture reconstruction, and the green caption surface is fitted from original green pixels. These repaired surfaces are not claimed to be exact hidden-pixel recovery.
- Original Firebird, Baba Yaga with cauldron, merchant and knight/shield isolated through Scenario. Their visible RGB was not regenerated/recolored. Small neighbouring corner fragments were removed from alpha. Source clipping and edge resolution remain limitations.
- Original reward examples isolated. The grouped merchant request retained only the gem; two separate requests recovered its chest and medal. Knight's baked heading was removed from alpha so it appears only once as live text.
- One generated2×2 clean scenery sheet supplies four separate512×384 environment plates behind the original subjects. Hidden scenery cannot be extracted from a flattened source. No character regeneration. A city crop from that generated sheet also supports the heading, with edge fades. It is not an original screenshot crop.
- Existing clock from R2, live gold lettering/material, shared header/Back and main environment reused. The separate hollow frame overlay draws above scene sprites to keep its curls visible; scene edges blend into the native opaque card. A final inner-rail cleanup removed residual source scenery from narrow strips.

## Scenario accounting

**16 successful jobs,57CU:**15 Pixa background-removal requests at3CU each plus one GPT Image2 scenery generation at12CU. No duplicate paid submissions or automatic retries. Two additional merchant-icon requests addressed identified omissions. The status-strip alpha result was empty and unusable; its cost remains included, and local source-silhouette restoration is explicitly recorded. Frame alpha required opaque-face restoration for the backing; the original hollow alpha remains useful for the ornament overlay.

See QA/ScenarioCosts.json, per-job Request/Job/Status/Billing/Output records under Sources/Scenario, AssetProvenance.json and QA/OriginalPixelAudit.json. New runtime art:21 PNGs plus21 new meta files. All previous PNG/meta files are unchanged.

## Verification performed

- Read current instructions, historical handoff/current task/workflow; inspected original reference, R2 implementation, source-extraction precedent and working tree. Confirmed root and Unity6000.5.9f1; no running user Editor before batch verification. Snapshot at /tmp/lor-events-r3.
- All three verification runs passed import/C# compilation, Events-only owner rebuild and serialized readback. Each passed **4 EditMode +13 PlayMode =17 tests**, zero failed/skipped. Run1 established native behavior/layout; manual review prompted frame-overlay/scene-edge changes in run2. Final run3 includes narrow inner-rail scenery cleanup. Logs/XML retained for all runs.
- EditMode: two Events and two Shop checks. PlayMode: five Events, three Shop and five OutOfEnergy checks. Coverage includes native resource/dependency validation, repeat-build reuse, Home/preview/header/Back routes, category behavior, disabled callbacks, rebind/once-only navigation, no save/demo-claim mutation, live/max balances, RU/EN text bounds, portrait safe areas, button raycasts, card gaps, background coverage, title/status/reward separation and Back placement. Updated checks verify original subject aspect, separate clean background coverage, labelled examples and ornament draw order above scenery.
- Final capture pairs: Events8, Shop7, OutOfEnergy8. Resolutions941×1672,576×1280 and1080×2400 with simulated safe areas. Manually inspected final standardRU, tallEN and Active-empty capture; earlier run1/run2 standardRU/tallEN informed visual corrections. Other captures were checked automatically.
- Inspected source-derived UI/subject/reward alpha over black, white and magenta; final overlay and heading fades likewise inspected. Inspected generated clean scenery sheet for composition and unwanted subjects/UI. Local alpha/repair preparation reused the existing isolated image environment, not Unity packages.
- Independently verified all15 primary source-derived sprites retain original RGB outside recorded repair regions. Download hashes and final production hashes verified. Transparency changes are separate from RGB preservation. Upsampling does not add source detail.
- Scoped git whitespace checks, Python AST, review links, asset scope and existing prefab object-ID preservation passed. Baseline4863 Assets files: 4856 unchanged,7 modified,42 added,zero deleted. All409 prior Events prefab object IDs retained; see QA/PrefabIdentity.json. Importer changes are restricted to new Events sprite-border names.

## Not performed / limits

No APK/AAB, physical-phone/hardware-Back test, full project test suite, browser execution, profiling, calibrated color analysis or exact-font identification. No live-event backend, participation, authoritative reward recipes, grants or EventDetail redesign. No changes to Gameplay, other page prefabs/art, Android guards, packages, settings, signing or existing deferred feature tasks.

This is a closer source-art implementation, not an exact-reference claim or assumed user acceptance. ReferenceComparison.md records remaining surface, typography, scenery, glow and layout differences.

## Exact changed and added production/test files

- `Assets/LegendsOfRus/Tests/PlayMode/EventsAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionEventsPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Editor/LockedDesignImporter.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionEventsRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionEventsView.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Events.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/EventsCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/action_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/yaga_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/rewards_yaga_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/rewards_merchant_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/frame_ornaments_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/scene_merchant_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/action_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/firebird_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/frame_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/status_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/scene_merchant_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/scene_knight_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/tab_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/reward_merchant_medal_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/scene_firebird_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/rewards_firebird_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/scene_knight_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/reward_merchant_medal_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/reward_merchant_chest_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/merchant_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/merchant_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/heading_scene_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/knight_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/tab_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/frame_ornaments_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/rewards_merchant_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/rewards_knight_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/knight_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/title_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/status_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/firebird_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/scene_firebird_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/yaga_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/heading_scene_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/rewards_yaga_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/rewards_knight_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/reward_merchant_chest_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/title_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/rewards_firebird_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/scene_yaga_r3.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/frame_r3.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/scene_yaga_r3.png.meta`
