# Events R4 — Tab Surfaces and Footer Ornamentation

Implemented the next refinement documented after Events R3. The selected category uses a repaired original sapphire surface instead of a flat inset. The footer uses the source carved navy strip, antique-gold curls, lower rail and sapphire centre jewel. The live schedule note sits between the small source flourishes; Back has a separate area. Both stay at the safe bottom on tall phones.

Home's exact environment sprite/tint, shared player header with energy, title, four R3 event scenes, Firebird glow and R2 reward layout are retained. Upcoming remains the default. Active/Completed show honest empty states, rewards remain labelled examples, and only the Firebird preview is available.

## Assets and Scenario

Original: `Assets/LegendsOfRus/Resources/DesignScreens/Events.png` (941 × 1672). Selected tab source box `(40,306,323,380)`; footer source box `(0,1614,941,1672)`.

One Scenario `model_openai-gpt-image-2` repair job, **12 CU**, no retries. The generated output fills bounded masks under the tab lettering/alert and footer sentence. It does not replace exposed source artwork. Normal-tab source alpha is reused for the selected-tab silhouette. Footer navy/carved lacquer remains intentionally opaque, with its outer bottom corners cleared. Black, white and magenta inspection images are in QA.

44,909 visible source pixels outside the repair masks retain identical RGB; zero mismatches. This count is an asset audit, not a whole-page similarity score. Hidden surfaces, the badge-covered corner and fine alpha remain reconstructed. Captions remain live/localizable; exact reference font identity is still unverified.

## Installed changes

Internal revision **7** agrees across view, active prefab, builder and cutover. Existing objects/bindings are preserved, including the disabled legacy flat faces. Repeated builds do not add objects or compound text scale.

- Runtime: `ProductionEventsRuntime.cs`, `ProductionEventsView.cs`, `ProductionEventsLayout.cs`.
- Owner: `ProductionEventsPrefabBuilder.cs`.
- Active serialized assets: `Pages/Events.prefab`, `CutoverState/EventsCutoverState.txt`.
- Two sprites and their new meta files: `EventsAligned/tab_selected_r7.png`, `EventsAligned/footer_r7.png`.
- Extended existing `EventsAlignmentPlayModeTests.cs` for selection state, source-metal tint, footer bounds, draw order, safe bottom and Back separation.

Scope audit: **7 existing Assets files changed, 4 added, 0 deleted**. No other page prefab or existing artwork changed. Detailed hashes/scope are in `Sources/BeforeAssetsHashes.json` and `QA/ScopeAndVerification.json`.

## Verification

Unity **6000.5.9f1**, isolated test save, `Verify01`:

- Events prefab rebuild and serialized readback passed.
- Edit Mode: **4 passed**, 0 failed/skipped.
- Play Mode: **13 passed**, 0 failed/skipped.
- Existing Events, Shop and OutOfEnergy checks cover routing, balance preservation, unavailable actions, category changes, localization, text fit, raycasts and simulated safe areas.
- Eight actual Unity captures included. Russian 941×1672 and English 576×1280 inspected visually.
- Browser review: ten selectable views, original comparison toggle, no script errors or horizontal overflow.

No Android build, physical-device test or full-suite run. Review image dropdown explicitly distinguishes native captures, the original reference and historical R3.

## Next

Review R4 visually. Event Detail (Visual Page 19) is next in the screen sequence. Remaining Events differences are recorded in `QA/ReferenceFidelityReview.md`; this pass does not claim pixel-exact whole-page reproduction.
