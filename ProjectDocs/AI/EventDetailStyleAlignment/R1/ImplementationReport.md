# Visual Page 19 — Event Detail R1

Implemented the native «Фестиваль Жар-Птицы» preview using the current Events artwork, as explicitly selected by the user. The old preview/hotspot page is replaced by an owned, serialized native prefab. Gameplay remains excluded.

## Result

- Shared live player/resource header, native Back, Profile, Settings and resource routes.
- Large isolated source Firebird over a separate clean city plate, original frame ornamentation and title plaque.
- Live Russian/English event title, pending-date status, objective explanation and clearly labelled reward examples.
- Disabled “Скоро”/“Coming soon” participation button. There is no event schedule, objective contract or authoritative reward recipe yet; preview interactions neither grant currency nor toggle the old session demo-claim flag.
- Uniform safe-area composition and full-bleed aspect-preserved environment. Title/footer backings were added after the first narrow-display capture exposed low contrast.

No dedicated original Event Detail reference was found. The user approved adapting Events artwork; no exact match to a missing reference is claimed. [Reference context and limitations](ReferenceComparison.md).

## Scenario and assets

21 direct serialized sprite references reuse existing runtime PNGs unchanged. Firebird, rewards, frames, clock and action art retain their recorded Scenario provenance. This pass made **zero new Scenario API requests, spent zero new credits and performed zero image edits**. This is the reuse step of the extraction-first workflow, not a fresh generation pass. [Asset paths/hashes/provenance](AssetProvenance.json).

No new alpha cleanup was needed; previous black/white/magenta checks are historical upstream evidence. Source-isolation limitations, reward-source softness, generated scenery and approximate serif font remain. New scene placement is native layout, not a screenshot as runtime UI.

## Actual changes

Added:

- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionEventDetailRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionEventDetailView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionEventDetailLayout.cs`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionEventDetailPrefabBuilder.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/EventDetail.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/EventDetailCutoverState.txt`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/EventDetailAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/EventDetailAlignmentPlayModeTests.cs`
- Their eight Unity-generated `.meta` files.

Modified existing production file: `ScreenRouter.cs`, limited to Event Detail native binding and excluding its duplicate legacy resource header.

Added this review/report/provenance/QA/Unity evidence directory; updated `CurrentTask.md`, `SessionLog.md` and the workflow reuse record. Existing user changes were preserved. Baseline4905 Assets files:4904 unchanged,1 changed,16 added,0 deleted. All earlier art, metadata and page prefabs remain byte-identical to the turn snapshot. [Scope evidence](QA/ScopeAudit.json).

## Checks performed

Unity **6000.5.9f1**, two recorded runs:

- Native prefab rebuild, serialization/readback and script compilation passed in both runs.
- **4 EditMode tests passed**: two Event Detail tests plus two Events regression tests.
- **10 PlayMode tests passed**: five Event Detail tests plus five Events regression tests.
- Zero failed/skipped in either run. Run2 is the final code/art state; run1 predates text backings.
- Verified native Events→Event Detail→Back route, duplicate-click/rebinding guards, Profile/Settings/Shop/energy navigation, live balances, RU/EN labels, maximum balances, no preview rewards/save mutation, and unavailable participation.
- Verified native prefab dependencies exclude DesignScreens/ReferenceOnly, no nested Canvas/hotspots, builder idempotence, foreground ornament order, text fit, section separation, actual button-centre raycasts, safe-area containment and full environment coverage.
- Saved **6 Event Detail capture/metadata pairs** at941×1672 RU/EN,576×1280 simulated safe-area RU/EN,1080×2400 simulated safe-area RU, plus maximum balances; **8 Events regression pairs**. Final standard RU and tall EN manually inspected; other final captures checked automatically.
- Assets scope/hash audit, reused-asset provenance, changed-file whitespace and review-link checks passed.

Commands, exit codes, logs and test XML: [final rebuild](UnityVerification/run2/Import/Command.json), [EditMode](UnityVerification/run2/EditMode/results.xml), [PlayMode](UnityVerification/run2/PlayMode/results.xml). Captures are actual Unity Play Mode output from isolated test storage; sample balances are save data, not baked art.

## Not performed

No APK/build-on-device test, physical-phone verification, full project test suite, browser execution of the review, performance/memory profiling, calibrated color comparison, exact font identification, new Scenario service-access test, or real event participation/reward fulfillment. No Android guard/package/signing/backend changes.

## Next action

Review the native Event Detail portrait. Before event functionality is scheduled, define event dates, objective/progress rules, authoritative reward recipes and claim validation; none is implied by this visual preview.
