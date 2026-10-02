# Tutorial / Help — Reference Rebuild R1

2026-09-25. User authorized the next page after Login / Account: Visual Page 31 Tutorial / Help.

The native Help guide now uses current Gameplay source rails/junctions, wood and recessed cells in its two matching diagrams. The highlighted red-match column is inset from the metal to keep junctions visible. Booster explanations use the approved shared framed medallions. Close, topic tabs, Previous and Next/Done use the established native press feedback.

The existing approved ornate navy panel, Close/Next artwork, fonts, wording and four sections are preserved. Instructions remain live and localized. Diagrams are read-only; no gameplay board, inventory, save, routing service or in-round tutorial rules were changed. The original legal adjacent-swap example still produces exactly one vertical red match. The moves plaque uses an illustrative 20, not an active-round value or timer.

## Reference and assets

The basis is the user-approved generated Help design from `TutorialStyleAlignment/DesignR2/Tutorial_Help_Design_R2.png`, already implemented by NativeDesignR3. It is copied to `Sources/Reference.png` and labelled as an approved concept in the review, not as a separate original game screenshot. The older supplied `tuturial.png` was a Special Offer panel style reference.

**0 Scenario jobs / 0 CU.** Existing accepted source extractions and shared artwork are reused. No new sprites, alpha edits, font files, materials or packages were needed. The source-art grid component is reused without modifying Gameplay itself.

## Production scope

Seven existing Assets files modified; none added/deleted:

- `ProductionTutorialRuntime.cs`: diagrams and native press feedback.
- `ProductionTutorialView.cs`: shared framed booster art and revision 3.
- `ProductionTutorialPrefabBuilder.cs`: revision 3 cutover/readback logging; existing owned-content rebuild retained.
- `Pages/TutorialOverlay.prefab` and `TutorialOverlayCutoverState.txt`: actual rebuilt native revision 3.
- `TutorialAlignmentEditModeTests.cs` and `TutorialAlignmentPlayModeTests.cs`: feedback/shared-art regression coverage.

Full paths are in [ScopeAndVerification.json](QA/ScopeAndVerification.json). Prefab GUID, source art and all other page prefabs preserved. Runtime builder remains idempotent.

## Validation

Unity 6000.5.9f1, isolated test saves using XDG_CONFIG_HOME and LOR_LOADING_TEST_CONFIG_ROOT.

- Baseline01: 5 EditMode + 9 PlayMode = 14 targeted Tutorial/Login checks passed.
- Verify01: prefab rebuild/readback succeeded; 5 EditMode + 10 PlayMode = **15 passed**, no failures/skips.
- Covers complete bindings, no screenshot dependencies, legal matching example, section switching, RU/EN layouts, control raycasts, Next/Previous/Done/Close/system Back, origin preservation, listener rebinding and unchanged saved progress. Login regression suite retained.
- Added test verifies current shared rail/booster bindings, 3×3 joint count, non-interactive diagrams and held-press/cancel behavior without changing section, route or save.
- Ten actual Tutorial captures: all four RU 941×1672 sections, all four EN 576×1280 safe-area sections, RU 1080×2400 booster section, and held Next press. Main RU and tall EN booster views inspected directly.
- Browser loaded all 12 review views at 430px, with no page errors or horizontal overflow.
- SHA-256 verifies 5,615 protected existing image/meta/font/other-prefab files unchanged; scoped source/test/cutover whitespace passed.

No Android build, physical-device/keyboard, full-suite, performance, fresh-process persistence or backend tests. No changes to actual gameplay rules or tutorials. See [fidelity review](QA/ReferenceFidelityReview.md).
