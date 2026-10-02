# Event Detail — Responsive Audit R8

The installed Event Detail page already anchors its shared header and carved footer to the safe edges. Its Back medallion, main action and Home gift remain visible at the checked phone sizes. No production layout or artwork change was needed.

## Coverage added

Extended `EventDetailAlignmentPlayModeTests.cs` with tall English capture coverage, exact footer/Back/action clearances, full aspect-preserved footer sprites, Back arrow containment, gift/action separation and Back raycasts at several points inside the medallion. Footer and leaders panels may meet at the reference size without overlapping.

A new test enters from Events in both languages, captures large and narrow tall sizes, sends native pointer clicks to disabled Play/Activate actions, changes language on the same view and returns through the native Back handler. It verifies one return route, restored Events bindings, unchanged saved gameplay state and unchanged reward-claim state. Existing font bindings, live balances, text fit, reference rails, shared reward assets and safe-area checks also run.

Capture files now save before geometry assertions so failed checks retain visual evidence.

## Results and scope

Final Verify02 passed all 8 checks (2 EditMode + 6 PlayMode) on the installed prefab with isolated saves in Unity 6000.5.9f1. Retained 13 native captures; all 11 browser views and the native-pixel toggle passed. Four comparable R7 captures are pixel-identical; two English captures differ only in 29 and 8 text pixels, maximum channel delta 1. Scope: 7,195 Assets files unchanged, one test file modified, no added/deleted assets. Results and exact scope are in QA/ScopeAndVerification.json. Production remains revision 8: no runtime, builder, prefab, cutover, font or image changes; only the Event Detail PlayMode fixture changed under Assets.

Verify01's two failures were caused by a new test requiring a positive gap where the existing footer and leaders frame intentionally meet at reference size. The assertion was corrected to allow the shared boundary within half a screen pixel; production geometry was not altered. Tall-language and native navigation checks passed in that initial run.

Review captures are actual Unity PlayMode renders with simulated safe areas, not Android device recordings. The viewer includes native-pixel inspection. Tests do not establish exact source-image fidelity.

## Remaining limitations

The finite content leaves a larger scenery gap above the bottom-anchored footer on tall screens. Existing reconstructed carving, Firebird alpha, shared rewards and live-text differences remain. Russian title/action source lettering is already installed; English remains localizable. Preview availability, unknown milestone values and unavailable participation/purchase behavior are retained.

No new generation/Scenario requests, production rebuild, Android build, device test, backend test, performance test or full-project suite. Next recommended page: Individual Ranking responsive/footer audit.
