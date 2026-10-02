# Profile R7 — Identity and Progress

The reference separates the small identity/level plaque from the longer progress rail. R7 restores that two-part geometry within the existing Profile identity panel, using the already approved gold/navy frame and sapphire fill resources.

The left plaque displays the authored chapter label. The right rail displays completed levels / authored chapter levels, using the existing approved numeral font. Fill and count retain the existing saved Chapter 1 completion calculation; no XP, extra levels or progress is fabricated. Native captions remain localizable.

The chapter plaque is 136 × 40 at (532,321); the adjacent rail is 216 × 40 at (666,321). The 192 × 19 fill is inset from all four rail edges. Separate text areas keep the labels clear of each other and the frame. Shared header, identity crest, collection artwork, relic selection, footer, routes and preview semantics remain.

View, builder, active prefab and cutover use revision8, including the new required chapter-label binding. The completion count retains its numeral font through refresh/localization.

## Changed Assets files

- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/ProfileCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Profile.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionProfilePrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/ProfileAlignmentPlayModeTests.cs`

Six existing Assets files changed; none added or deleted. Review evidence, scripts and handoff documentation accompany the change. No image or font files changed. No Scenario request, extraction or generation was required:0 calls,0CU.

## Validation

Final run `/tmp/lor-profile-progress-r7/Verify01`, Unity6000.5.9f1. Import/rebuild succeeded;3 EditMode and8 PlayMode tests passed, zero failures/skips. The added test checks empty, half-complete and complete states in Russian and English against isolated saved records, preserving the save during presentation. Layout assertions check separate text bounds and four-sided fill inset. Existing selection, routing, text fit, save and phone-safe-area tests also ran.

35 actual Unity Play Mode captures are included. Visually inspected half-complete Russian and complete tall-English captures; all37 browser review views decoded without JavaScript errors or horizontal overflow at430px. Original and previous captures are separately labelled.

SHA checks preserve5,575 existing protected artwork/font/meta/other-page files. Four matched captures differ only inside the identity-progress area; shared header/title pixels are unchanged. Scoped source whitespace checks passed.

No Android build, physical-device, full-suite, backend or performance verification. Safe areas are simulated. This retains the existing authored Chapter1 catalog scope and does not implement the proposed500-round chapter plan.

[Scope and Unity checks](QA/ScopeAndVerification.json) · [Integrity](QA/IntegrityChecks.json) · [Browser checks](QA/BrowserChecks.json) · [Fidelity](QA/ReferenceFidelityReview.md)
