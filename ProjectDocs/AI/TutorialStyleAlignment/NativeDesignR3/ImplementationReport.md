# Tutorial / Help — native Design R3

The approved DesignR2 composition is now implemented in the active TutorialOverlay prefab. The first section has two 3×3 native illustration grids using actual gameplay gems, a neighbor-swap arrow, a highlighted red match, concise goals/moves guidance, and internal page navigation. Four RU/EN sections cover basics, match-created specials, inventory boosters and obstacles. The final section's Done, Close and system Back return to the existing origin. These diagrams are read-only teaching illustrations, not playable boards.

## Art and fidelity

One Scenario source-alpha isolation job cost **3CU**; no new RGB generation or repeated submission. The approved concept's frame and sapphire ornament retain their original RGB outside the saved repair mask. Occupied interior replaced with reflected clean navy donor from the concept; upper-right shoulder mirrored beneath the separate Close. Scenario input remains immutable; opaque interior restored and disconnected exterior fragments removed. Source/input/output/runtime hashes retained. Initial busy face and misplaced moves caption were corrected before final verification.

R1 isolated Close/emerald control, existing Settings card/tab/Back resources, approved embedded fonts, clean scenery and actual gameplay illustrations are reused. Text is live and localized, with native controls. No full-screen reference, screenshot hotspots, or generated instructional text enters runtime. Compared with the concept, fonts, internal card/tab decoration and gem appearance follow existing assets, so this is not a pixel-identical rendering. Exact source font identity remains unknown.

The example is deliberately corrected: one adjacent swap produces exactly one red vertical match. A moves plaque replaces the hourglass, avoiding a timed-round implication. "20" is an illustrative counter inside Help, not a claim about the active level. All gameplay rules and inventory semantics remain unchanged.

## Changed production files

- ProductionTutorialRuntime.cs: new native composition and deterministic grid illustrations; safe owned-content rebuilding outside Play Mode.
- ProductionTutorialView.cs: revision2 bindings, live diagram labels, compact basic guidance, selected tabs and responsive content switching.
- ProductionTutorialPrefabBuilder.cs: rebuild owning hierarchy and save revision2 cutover/prefab together.
- Pages/TutorialOverlay.prefab and TutorialOverlayCutoverState.txt: rebuilt, verified revision2.
- TutorialAligned/panel_r3.png and its new meta: independent blank ornate panel.
- TutorialAlignmentEditModeTests.cs and TutorialAlignmentPlayModeTests.cs: example validity and diagram/navigation state checks.

Production scope is **7 changed files +2 added files**, with no deletions or changes to prior metas/GUIDs. Existing ScreenRouter, responsive layout component, Gameplay, Splash, other pages, packages, ProjectSettings and Android guards match the5,650-file start snapshot.

## Verification performed

- Unity6000.5.9f1 import, C# compile, owning prefab rebuild, serialized binding/readback and cutover checks passed.
- Final Verify03: **5 Edit Mode +8 Play Mode =13 tests passed**, zero failures/skips. Includes Tutorial and Login regression fixtures, idempotent runtime build, one legal illustrated swap, section/language switching, Next/Previous/Done/Close/system Back, origin preservation, event-listener rebinding and unchanged save/inventory.
- Nine actual Help captures: four RU941×1672, four EN576×1280 with simulated safe area, one RU1080×2400 safe-area boosters view. Seven Login regression captures retained separately.
- Visual inspection: basic matching, moves plaque, combinations, boosters and obstacle guidance; panel edges examined against black, white and magenta. Final Next alignment inspected after correction.
- Original RGB outside the repair mask, immutable Scenario input, output/runtime hashes, Python script syntax and production scope checked. Source whitespace check passed. Browser390px loaded all10views with zero errors, missing links or horizontal overflow; results recorded in QA/BrowserReview.json.

## Not performed

APK build, physical phone/touch, full-project test suite, performance/accessibility profiling, backend/online tests, or redesign of the in-round interactive tutorial. Prior completed visual pages and deferred service features are preserved.

## Review

Open Tutorial_NativeDesign_R3_Review.html for one standalone portrait at a time and the approved concept comparison. Next step: user review and later physical-device validation.
