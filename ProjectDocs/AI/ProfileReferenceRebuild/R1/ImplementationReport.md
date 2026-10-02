# Profile — Reference Rebuild R1

Implemented Visual Page 26 after the user's continuation from Mail R5. The active Profile prefab now uses the shared Home player header, source-derived identity/stat/tab surfaces, the original hanging crest and a wooden footer. A dark native content backing improves contrast against reused Home scenery. Profile remains an interactive Unity page with separate assets and live localized text.

The earlier approval for real saved statistics and labelled hero/relic previews remains in force. Saved level results, chapter completion and booster quantities are unchanged. Preview collections grant nothing. Existing guarded navigation, modal dismissal, milestone information and paged history remain available.

## Changed production files

- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileRuntime.cs`: composition, complete shared header instance, source panel references, native dark backing and wooden footer.
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileView.cs`: revision 2, shared header binding and refresh, new selection surfaces and approved local title font assignments.
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileLayout.cs`: safe-top composition, safe-bottom Back/footer and extending detail/content backing on tall screens.
- `Assets/LegendsOfRus/Scripts/Editor/ProductionProfilePrefabBuilder.cs`: revision 2 cutover; existing builder rebuilds and reads back the real prefab.
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Profile.prefab` and `CutoverState/ProfileCutoverState.txt`: rebuilt native revision 2.
- Nine new PNG/meta pairs under `ProductionUI/ProfileAligned/`; names/hashes in `Sources/Provenance.json`.
- Two existing Profile test files: require exactly one fully bound shared header; allow one rendered pixel of camera rounding at safe edges and report actual rectangles.

Eight existing Assets files changed, eighteen added, none deleted. `QA/ScopeAndVerification.json` lists exact paths against the pre-task snapshot. Existing images, fonts, metadata/GUIDs and other page prefabs are unchanged by SHA. No save, service, startup, package or Android setting changes.

## Assets and Scenario

Dedicated reference: `/home/k2/Downloads/match3final/profile.png`. Existing title, portraits, relic cards, stat symbols, footer icons and shared boosters are reused. Original title lettering remains for Russian; English and dynamic text remain live.

One successful Scenario `model_pixa-background-removal` sheet request: **3 CU**, no paid retry and no RGB generation. Request/job/status/billing evidence is in `Sources/Scenario/profile_surfaces/`.

Visible source frames and the identity banner are retained; hidden faces are rebuilt with clean navy/wood source donors. Scenario alpha is bounded to avoid exterior remnants and restore deliberately opaque interiors. The prepared sheet had a stats/tab overlap: final RGB comes from independent prepared panels, not the overlapped sheet. The collection donor mask was expanded to remove caption remnants. The submitted footer crop missed its right rail: the final wider original crop uses a bounded local silhouette, not the incomplete Scenario alpha. Original submitted inputs remain intact. `PreparedRepairs.json`, masks and `Provenance.json` disclose these repairs; this is not a claim of exact recovery of hidden pixels.

All nine textures were inspected on black, white and saturated pink in `QA/AlphaBackgrounds.png`. Prepared-composite visible RGB, final asset hashes and protected original assets passed the integrity script.

## Verification

Final run: **Verify05**, Unity **6000.5.9f1**.

- Native prefab rebuild and readback passed; view, prefab and cutover revision 2 agree.
- **3 Edit Mode + 6 Play Mode tests passed**, zero failed/skipped.
- Routing, repeat-action guards, modal input blocking, saved-stat sanitization, preview no-grant behavior, tab/relic selection, history, milestones and maximum balances covered.
- Fifteen actual Unity captures: RU/EN, 941×1672, 576×1280 safe area, 1080×2400 safe area, items, relics, history, achievements and modal states.
- Seventeen review choices loaded in headless Chrome at 430-pixel viewport width, no JavaScript errors or horizontal overflow.
- Scoped C# whitespace checks passed; asset/provenance/protected-file checks passed.

Earlier evidence retained: Verify01 found a duplicate local variable; Verify02 flagged strict edge containment; Verify03 passed before the final dark backing; Verify04 rejected a sprite-less Image, corrected by using the project's existing native `ProductionHomePanel` graphic for the backing. Verify05 is the final result.

No Android build, physical-device, full-suite, backend, online-profile or performance testing was performed. Safe areas are simulated in Unity captures.

## Review and next step

Open `Profile_ReferenceRebuild_R1_Review.html`. Typography, relic framing/caption fit, footer continuity and reconstructed materials are still approximate; see `QA/ReferenceFidelityReview.md`. Recommended R2: relic collection frame, heading ornaments and captions, keeping approved assets and real saved statistics.
