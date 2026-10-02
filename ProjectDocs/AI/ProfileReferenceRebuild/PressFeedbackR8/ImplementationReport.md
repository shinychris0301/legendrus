# Profile R8 — Button Press Feedback

Profile now reuses `ProductionMapButtonFeedback` for its native controls, including dynamically rebuilt collection cards, tabs, footer, modal actions and the local shared-header instance. The shared header prefab and the feedback component itself are unchanged.

Artwork and child labels receive the existing map press tint, then recover over the component's existing 0.12-second unscaled-time release. Button geometry, hit areas and click callbacks are unchanged. No new audio/haptic callbacks are installed. This is an interaction refinement; it does not revise the source artwork.

The local builder switches off the default ColorTint transition and immediately clears any tween started by Button.OnEnable while its parent was blocked. This prevents stale disabled tint from leaving dynamically constructed cards dark. View, builder, active prefab and cutover use revision9.

## Verification history

Verify01 passed the new press behavior test but failed the modal capture's old default-tint expectation. The capture fixture was updated to wait for the active custom feedback's resting state. Verify02 exposed a remaining stale tint; visual inspection also showed idle cards dim during a press capture. The installation helper now stops the prior default tween. Final Verify03 passed after both corrections. Earlier failed logs/results are retained.

The added PlayMode test checks tab/relic/footer press tint on artwork and captions, unchanged geometry, unrelated pointer rejection, pointer-exit restoration, disabled controls, component-disable restoration, no click/navigation/save mutation from pointer-down, and a tall-English Back capture. Existing layout tests require feedback on all visible controls after content rebuilds. Existing navigation, modal input guards, progress, collection and save tests remain.

No asset extraction or generation is needed:0 Scenario calls/0CU. Existing artwork/fonts are reused unchanged. No Android build, physical-device, full-suite, backend or performance validation is claimed.

## Final validation and scope

Unity6000.5.9f1 final run `/tmp/lor-profile-press-r8/Verify03`: import/rebuild succeeded,3 EditMode+9 PlayMode=12passed,zero failed/skipped.39 actual Unity captures include four labelled pressed states. Final Russian pressed-relic and tall-English pressed-Back captures were visually inspected; resting graphics remain normal outside the pressed control.

All41 browser review views decoded at430px without JavaScript errors or horizontal overflow. The review initially shows the pressed tab; select a resting-state view to compare. Screenshots demonstrate captured states, not a live browser version of the game.

SHA preserves5,575 existing protected artwork/font/meta/other-page prefab files. Standard RU/EN, ItemsRU and HistoryRU captures are pixel-identical to R7. Scoped source whitespace checks passed. Safe areas are simulated, and test saves are isolated.

Six existing Assets files changed; none added/deleted:

- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/ProfileCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Profile.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionProfilePrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/ProfileAlignmentPlayModeTests.cs`

[Unity and scope](QA/ScopeAndVerification.json) · [Integrity](QA/IntegrityChecks.json) · [Browser](QA/BrowserChecks.json) · [Fidelity](QA/ReferenceFidelityReview.md)
