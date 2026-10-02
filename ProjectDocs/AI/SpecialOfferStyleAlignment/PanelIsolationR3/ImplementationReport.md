# Special Offer R3 — Panel isolation

The black polygon surrounding the bundle panel came from the R1 matte, which retained background outside the gold frame. R2 used Scenario for the new chest but left this panel unchanged. This pass corrects the panel with a fresh Scenario API background-removal request.

## Change

- Scenario `model_pixa-background-removal`, job `job_VRqWCxaqyY1BHmL8y4SsKsnN`, successful, 3 CU; one submission, no repeated generation.
- Kept all original panel RGB pixels and dimensions; replaced alpha with the Scenario output. 66,150 alpha pixels changed. The opaque central face remains fully opaque, and the gold outline is preserved. Existing card.png GUID/meta retained.
- Interpreted the ambiguous “upper-right option” as the lightning/energy indicator after requesting clarification and allowing time for a response. No reply was received before implementation. Capsule, icon and value are inactive only on SpecialOffer; serialized bindings and energy service remain intact. Gold corner decoration is preserved. If a different target was intended, this narrow visibility change can be reversed.
- Runtime and owning builder updated together; saved prefab readback verifies the hidden indicator and R2 chest. Structural revision1 remains consistent. Other pages and the fresh R2 chest unchanged.

## Performed checks

Unity6000.5.9f1 compilation/import, prefab rebuild and readback passed. EditMode 4/4 and PlayMode 6/6 passed (SpecialOffer + Shop regression), zero failed or skipped. Existing tests cover bindings, repeated builds, safe-area layout, text fit, aspect ratio, input routing, live balances and unavailable purchase behavior.

Saved five actual SpecialOffer PNG/JSON capture pairs and seven Shop regression pairs. Visually inspected SpecialOffer941×1672RU and576×1280safeEN. Other captures were tested automatically, not manually inspected in this pass. Final panel alpha inspected on black, white and magenta. Exact sourceRGB preservation and opaque-face assertion passed. Python syntax, review links and changed C# whitespace passed.

Production hash comparison: 4651 baseline files; 4647 unchanged; 4 changed (card texture, runtime, builder, prefab); none added/deleted; all existing metas intact. No changes to Shop, Splash, Loading, Home, ChapterComplete, services, Android guards, packages, project settings or signing.

## Not performed

APK/AAB build, physical phone, full-project test suite, manual browser execution, payment/SMS/provider integration, prolonged play/profiling or calibrated-color/exact-font testing. The extraction preserves the existing panel design; no claim of exact whole-page reference fidelity.

[Actual Unity review](SpecialOffer_PanelIsolation_R3_Review.html) · [Provenance](Sources/RuntimeProvenance.json) · [Changed files](ChangedFiles.md).
