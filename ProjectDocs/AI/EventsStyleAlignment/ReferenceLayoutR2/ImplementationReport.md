# Events R2 — reference layout and hierarchy

Implemented the approved R1 audit recommendation in the active native Events prefab and its owning runtime/builder. Revision and cutover now agree at2. Existing object IDs, file GUIDs and all previous sprite pixels are preserved. Gameplay and other pages are unchanged.

## Result

First two cards now have reference-like335/320px heights; lower rows move down to1069/1347. Individual generated paintings fill native masks without stretching or regenerating them. The last card ends at1601, approximately13px above the source's lower edge to preserve Back and a compact footer. Heading uses larger live lettering and the existing Home gold bevel material. Event names have separate one-/two-line layouts and gold depth. Each card has an original clock, navy/gold status strip, live reward heading and framed pending-reward area beside its native action.

No confirmed event schedule or reward recipe exists. Dates/rewards remain unannounced; no fabricated countdown, premium entitlement, prize amount, grant or participation action. Existing first-event preview, disabled other actions, categories, live balances, RU/EN localization and all routes are preserved. Reward examples from the reference are not advertised as available prizes. See ReferenceComparison.md for remaining visual differences. Exact font identity is still unknown.

## Scenario API

One successful `model_pixa-background-removal` job, **3CU**, no regeneration/retries. The42×42 reference clock was enlarged8× for the API; its original RGB interpolation is retained with the returned alpha, then transparent bounds trimmed. Final258×269sprite is displayed at38×38 with aspect preserved. Upsampling adds no original detail. The black/white/magenta QA composite was inspected: no surrounding scenery, opaque clock face preserved. Sources, one-shot submission marker, status, billing and hashes are retained. All six R1 sprites are unchanged.

## Checks performed

- Read project instructions, current task/historical handoff/reference provenance; verified root and Unity6000.5.9f1; inspected the dirty tree and saved an Assets hash snapshot before editing.
- Final run2: Unity import/C# compilation, Events-only prefab rebuild and serialized readback passed. **4 EditMode +13 PlayMode =17 passed; zero failed/skipped.** Two Events/two Shop EditMode, five Events/three Shop/five OutOfEnergy PlayMode.
- Events behavior: live balances and maximum values, RU/EN, Home entry, once-only preview/return, header/Back routes, category empty states, disabled callbacks/rebinding, no save/demo-claim mutation. Adjacent Shop/OutOfEnergy regressions passed.
- Layout: portrait941×1672,576×1280 and1080×2400 with simulated safe areas; text bounds, button raycasts, background cover/aspect, card gaps, clock/date separation, title/status separation, description/reward separation, pending-frame/action separation, Back below final card, and aspect-preserved paintings covering their masks.
- Fresh capture pairs: Events8, Shop7, OutOfEnergy8. Manually reviewed final standardRU, tallEN, maximum balances and Active empty state against the original. Other captures were verified automatically.
- Run1:4/4Edit and9/13Play passed. Short-title rectangles overlapped status rows; corrected. Restored original date-pending wording after an unnecessary shortening tripped its contract assertion. Shop had one unrelated saved-state timestamp comparison failure (`lastEnergyUtcTicks` advanced); no Shop code/test edits. Its unchanged test passed in final run2. Earlier failures/logs/XML retained; known timestamp-sensitive Shop assertion remains a test-harness limitation.
- Existing315 Events prefab object IDs retained,94 new objects/components; zero removed. Baseline4861 Assets files: 4855 unchanged,6 modified,2 added,zero deleted. All previous PNG/.meta files and every other page prefab remain unchanged. Scope/identity hashes retained under QA.
- Python AST, scoped C# trailing whitespace/git diff checks and all local review links passed. Standalone HTML displays one portrait at a time and labels reference versus actual capture.

## Not performed

No APK/AAB, physical phone/hardware Back, full project test suite, browser execution, profiling, calibrated color comparison, exact-font identification or real live-event services. No new character/scenery generation, reward recipes or EventDetail redesign. Existing deferred feature work remains unchanged. Visual acceptance of R2 is pending user review.

## Exact production/test changes

- `Assets/LegendsOfRus/Tests/PlayMode/EventsAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionEventsPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionEventsRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionEventsView.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Events.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/EventsCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/clock_r2.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/clock_r2.png.meta`
