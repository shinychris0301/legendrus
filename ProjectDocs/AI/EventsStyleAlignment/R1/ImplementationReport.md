# Events R1 — implementation and verification

Native Visual Page18 implemented in Unity6000.5.9f1 at the existing project root. The new Events prefab, owning builder, runtime/view/layout and cutover revision1 agree. ScreenRouter binds the native controls and suppresses its legacy resource overlay for this page. Existing Home→Events and EventDetail→Events routing retained; no services or Gameplay code changed.

## Behavior and reference adaptation

Four reference concepts appear under Upcoming: Firebird Festival, Baba Yaga's Trials, Merchants' Treasures and Ice Tournament. Current runtime has no authoritative event schedule or reward recipes. Active and Completed therefore show empty states; cards say dates and rewards will be announced. The first card retains a clearly labelled preview route to the existing local EventDetail demo. Other controls stay disabled; no fabricated participation, premium label, countdown or currency grant. Navigating this page does not alter demo claim state. EventDetail itself remains the earlier implementation and is outside this visual pass.

Native live RU/EN labels and actual saved balances. Accepted navigation header/portrait copied from the actual Shop prefab, circular Back reused from DailyReward. Existing clean dusk environment reused with a page-local tint. Frame corners use nine-slicing; illustrations retain their aspect ratios. No full-screen reference or screenshot-region runtime dependencies. ReferenceComparison.md documents fine-detail/layout/font differences; no exact-match claim.

## Scenario artwork

Six generation jobs (12CU each): title plaque, empty card frame and four individual paintings. Two alpha jobs (3CU each) isolate title and frame. **Eight successful jobs, 78CU; zero retries or corrective regeneration.** Source crops are API guidance only. Generated RGB preserved; alpha trim applied to title/frame and a gentle8px outer/42px right-edge fade integrates each painting into its own opaque card. All six assets inspected over black, white and magenta. Prompts, source bounds, hashes, jobs, status and billing retained under Sources and AssetProvenance.json. Earlier art was not modified.

## Checks performed

- Read AGENTS, CurrentTask, historical handoff, reference provenance and current files; verified Unity version/root, inspected initial dirty working tree and saved a before-work hash snapshot. Handoff's Splash baseline is historical; Page18 follows the current approved sequence. Other approvals retained.
- Final run3: Unity import/C# compilation, Events-only owner rebuild and serialized prefab readback passed.
- **4/4 EditMode tests:** two Events tests (native bindings/dependencies/header reuse/Back/import settings and repeat-build identity/unavailable participation) and two existing Shop tests.
- **13/13 PlayMode tests:** five Events, three Shop, five OutOfEnergy. All passed, zero failed/skipped.
- Events tests exercise Home entry and once-only preview routing/return, all header routes/Back, localization/live gold update, actual maximum balances, category switching/empty states, disabled callbacks/repeated Bind, no save or demo-claim mutation, safe-area bounds, raycasts, background coverage/aspect, card gaps and fitted text bounds. Existing Shop and OutOfEnergy regressions cover adjacent routes. Energy timestamp is normalized only in test save comparisons.
- Actual PNG+JSON capture pairs: Events8, Shop7, OutOfEnergy8. Simulated941×1672,576×1280 and1080×2400 displays/safe areas. Manual review: final standardRU, tallEN, Active/Completed empty states; run2maximum balances inspected. Other captures automated. See UnityVerification.
- Run1:3/4Edit and12/13Play passed. Generic importer overrode new Android texture settings; added an Events-only rule. Maximum-balance test incorrectly measured pre-fit text; corrected measurement. Manual review caught too-narrow tab faces; replaced with sliced frame faces.
- Run2:4/4Edit and12/13Play passed. Remaining high-resolution fitted-font assertion compared pixel size to logical size; corrected using pixelsPerUnit. Added visible blue selected-tab face. Run3 is final. All earlier logs/XML retained.
- Python AST, scoped C# whitespace, source git diff check, local review links and final asset-hash scope checks passed. Review HTML has not been executed in a browser.
- Baseline **4776 files: 4774 unchanged, 2 modified, 29 added, zero deleted**. All existing PNG/.meta/prefab hashes unchanged. The two existing source edits are ScreenRouter and a narrowly scoped Events folder import rule in LockedDesignImporter. All existing Gameplay files and approved page prefabs/art preserved. No Android guards, packages, signing, backend or project-settings edits.

## Not performed / remaining scope

No APK/AAB build, physical phone/hardware Back, full project test suite, performance profiling, browser execution, calibrated color or exact-font verification. No live event backend, timed calendar, participation/progression, premium entitlement or event reward fulfillment implemented/tested. EventDetail visual redesign and authoritative events belong to later work. Existing deferred Failure continuation, SMS and other feature tasks retained. User visual acceptance pending.

## Exact changed/new production and test files

- `Assets/LegendsOfRus/Scripts/Editor/LockedDesignImporter.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/EventsAlignmentEditModeTests.cs.meta`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/EventsAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/EventsAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/EventsAlignmentPlayModeTests.cs.meta`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionEventsPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionEventsPrefabBuilder.cs.meta`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionEventsRuntime.cs.meta`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionEventsView.cs.meta`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionEventsRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionEventsLayout.cs.meta`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionEventsLayout.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionEventsView.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/frame.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/title.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/merchant.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/yaga.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/frame.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/knight.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/firebird.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/merchant.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/firebird.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/title.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/knight.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/yaga.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Events.prefab.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Events.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/EventsCutoverState.txt.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/EventsCutoverState.txt`
