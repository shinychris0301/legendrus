# Event Detail R5 — shared assets and panel fit

The page previously retained its own gem, potion and Firebird reward extractions despite the reuse request. It now directly references the approved shared gem/booster loaders. The footer gift remains the exact Home sprite. Shared source files were not changed.

Six chest silhouettes were repaired from the original pixels, retaining dark surfaces and disconnected ornament details that the old foreground extraction discarded. Eight transparent pixels surround each sprite. Milestone chests now retain their individual source positions and sizes; the premium chest fits inside its original enclosure without clipping.

The old objective/rewards panels overlapped by seven design pixels, and rewards/lower panels by four. Their opaque faces now meet at y=837 and y=1180. One source border layer owns all adjoining rails and junctions. Blank lacquer reuses earlier accepted Scenario output at native texture scale, excluding old frame residue. This is a native Unity hierarchy with separate text/images/controls, not a reference screenshot.

## Changed files

- ProductionEventDetailRuntime.cs: shared bindings, padded/source-proportion chests, adjoining faces and source border layer.
- ProductionEventDetailView.cs and ProductionEventDetailPrefabBuilder.cs: internal revision 6.
- EventDetail.prefab and EventDetailCutoverState.txt: rebuilt and serialized at revision 6.
- EventDetailAlignmentPlayModeTests.cs and EventDetailAlignmentEditModeTests.cs: shared resource identity, panel seams/layering and corrected chest binding assertions.
- Eleven new EventDetailAligned sprites and their Unity metadata: six chests, four opaque faces, one border layer. Existing sprites/GUIDs and other page prefabs retained.
- Reproduction, provenance, pixel checks, native captures and review files in this directory. Exact file scope is in QA/ScopeAndVerification.json.

## Validation

Final verification evidence: **Verify04**, Unity **6000.5.9f1**. Prefab rebuild/readback plus targeted EventDetail and Events Edit/Play Mode checks. **4 Edit Mode + 10 Play Mode = 14 passed; 0 failed/skipped.** Nine review views and the comparison toggle passed browser checks. See QA/ScopeAndVerification.json for the complete scope. Six native EventDetail capture variants and eight Events regression captures use isolated saves; no user save is modified. Native Russian and narrow English views inspected, plus chest alpha over black, white and magenta.

Verify01 stopped on an obsolete Edit Mode expectation for the old chest sprite. Verify02 passed; capture inspection exposed stretched lacquer and undersized premium chest. Verify03 passed; remaining extruded old edge pixels in the face materials were removed before Verify04. Earlier evidence is retained.

No new Scenario calls or credits: reused approved shared resources, existing repaired lacquer and original source RGB. QA/ChestPixelChecks.json measures visible RGB preservation and transparent padding; Sources/ReusedAssets.json records unchanged shared art hashes. Browser comparison/selector checks are in QA/BrowserChecks.json. No Android build, physical-device test, performance profile or full test-suite run.

## Fidelity limits

See QA/ReferenceFidelityReview.md. Shared gem/booster consistency deliberately takes priority over this page's differing source icon silhouettes. Fine alpha, reconstructed surfaces, live typography, Home scenery and honest unavailable data states prevent an exact full-page match.
