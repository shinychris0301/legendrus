# Profile — Footer and Detail R3

The user approved the next refinement after R2. The active Unity page now uses a continuous original wooden footer with two dividers, aligned independent button faces and bounded sapphire selection lighting. The selected-relic panel restores the original larger sword artwork, a compact preview badge and separate description/action areas. Its source frame corners remain proportional on tall phones.

The real-stat and preview contract is unchanged. Existing routing, footer selection, history, milestones, modal input guards and relic selection remain in use. No upgrades, ownership or fake rewards were added.

## Changed files

- `ProductionProfileRuntime.cs`: continuous footer strip, three aligned native action faces and detail composition.
- `ProductionProfileView.cs`: per-action normal/active sprite selection; native revision 4.
- `ProductionProfileLayout.cs`: footer remains safe-bottom anchored; detail contents center in added tall-screen space.
- `ProductionProfilePrefabBuilder.cs`: detail sprite border metadata and cutover revision 4.
- Active `Pages/Profile.prefab` and `CutoverState/ProfileCutoverState.txt` rebuilt/read back at revision 4.
- Ten PNG/meta pairs added under `ProductionUI/ProfileAligned/`: footer strip, six aligned normal/active action faces, expanded sword detail art, blank preview badge and an unchanged R1 detail surface with independent nine-slice metadata.

Six existing Assets files modified, twenty added, none deleted. Full paths are in `QA/ScopeAndVerification.json`. SHA verification confirms 5,555 protected existing artwork/font/metadata/other-page-prefab files are unchanged. No services, saves, startup, packages or Android settings changed.

## Source and Scenario

Original `/home/k2/Downloads/match3final/profile.png`, retained in the R1 review directory. One successful `model_pixa-background-removal` sheet job, **3 CU**, no retries and no RGB generation. Request/status/billing are retained in `Sources/Scenario/footer_detail/`.

Original footer rim/dividers and badge border use source RGB, with clean wood/navy donor masks clearing old content. Scenario alpha is bounded outside panels and restores intentionally opaque interiors. The sword crop was locally expanded to `[56,1270,260,1509]` after the initial crop omitted outer curls; its visible RGB equals the enlarged original source under a bounded alpha contour. No regenerated sword pixels are used.

Footer action faces are exact aligned crops of the prepared continuous surface. Active tint fades to unchanged edge pixels. The R1 detail frame pixels are duplicated unchanged; only the new sprite's importer borders differ. Alpha backgrounds were inspected on black, white and saturated pink. Provenance and repair masks are retained under `Sources/`.

## Verification

Final **Verify01**, Unity **6000.5.9f1**:

- Native prefab rebuild/readback passed; runtime/view/prefab/cutover revision 4 agree.
- **3 Edit Mode + 6 Play Mode tests passed**, zero failed/skipped. No test limits changed.
- Fifteen actual Unity captures cover RU/EN, 941×1672 and tall 576×1280 / 1080×2400 simulated safe areas, collections, saved quantities/history/milestones, modal behavior and maximum balances.
- Existing checks verify button raycasts, safe-area bounds, live text fit, guarded routes, repeat actions, no preview rewards and save preservation.
- Seventeen browser review choices passed at 430 pixels with no page errors or horizontal overflow.
- Standard RU/EN pixels above y1266 are unchanged from R2. Compared Items and History views are unchanged above the footer at y1518.
- Asset hashes, unchanged prior resources, source sword RGB, aligned footer faces and selection edges passed integrity checks. Scoped C# whitespace check passed.

No Android build, physical-device, backend, online-profile, full-suite or performance testing was performed. Phone sizes and safe areas are simulated in Unity captures.

Open `Profile_FooterAndDetail_R3_Review.html` for the implemented page, earlier version and original reference. Remaining font/material/contour differences are documented in `QA/ReferenceFidelityReview.md`.
