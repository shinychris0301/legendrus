# Shared Booster Continuous Rim R3

The user rejected the color mismatch and visible joins in R2 and authorized recreating the circular metal band. A single complete Scenario-generated ring now replaces every patched rim in the shared booster family.

## Implementation

- Recreated the full circumference, concentric gold rails, engraved bronze channel and sapphire ornaments in one generation. No mirrored donor arcs remain.
- Used one identical transparent ring for all eight medallions: mace, firebird, shield, potion, scroll, lightning, bomb and swap. The latter four remain unavailable previews.
- Recombined original booster artwork and opaque navy faces beneath the new metal. Excluded old metal and baked badges; extended only the hidden backing locally, excluding subject colors from the donor pool. The eight tool-only PNGs are byte-identical to the previous version. 104,384 centrally retained source pixels have zero RGB mismatches; this is not a whole-image fidelity score.
- Updated eight existing `_framed.png` and eight `_rim.png` resources in `Assets/LegendsOfRus/Resources/ProductionUI/SharedBoosters/`. Dimensions remain 192×192. Existing GUIDs, importer settings, shared loaders, prefab bindings, live stock badges and functionality remain unchanged.
- Existing framed consumers (Booster Selection, Pre-Level, Pause and Failure) receive the replacement through the existing shared resources. Pages using unframed subjects retain those subjects.

## Scenario evidence

[Generation request and full prompt](Sources/Scenario/rim/Request.json): `model_openai-gpt-image-2`, one output, medium quality, 1024×1024, original mace-medallion crop as the reference. Requested a full continuous narrow antique-gold band, empty center and exterior, no tool or baked badges.

One generation: **12 CU**. One Pixa alpha isolation: **3 CU**. **15 CU total**, no replacement generations. Job/status/output hashes and actual billing records are retained under `Sources/Scenario/`. Generated RGB is preserved for the ring; Pixa supplies its alpha. `Sources/Compose.py` records the deterministic composition process.

## Verification

- Unity **6000.5.9f1** import succeeded.
- **13 Edit Mode + 21 Play Mode = 34 passed**, zero failed/skipped. Existing shared-art, Booster Selection, Pre-Level, Pause and Failure checks ran with isolated test save directories.
- Actual Unity captures collected in RU/EN and tall safe-area configurations. Booster Selection and Pause captures visually inspected; full-circle and lower-arc details inspected against source/R2.
- Final asset alpha inspected over black, white and saturated magenta. Ring center is transparent and closed; all eight isolated-ring files have the same content hash.
- Browser review: seven native/reference views and eight icon comparisons loaded, no page errors or horizontal overflow at 430px width.
- Scope audit: **16 existing PNGs changed, zero added/deleted Assets files**. No code, prefab, meta or tool-only asset changed in this pass. Exact list and NUnit summaries: [ScopeAndVerification](QA/ScopeAndVerification.json).

No Android build, physical-device test or full test suite was run. The recreated rim deliberately replaces the original metal and its hidden sections; its fine engraving and ornament shapes are an interpretation, not an exact source extraction. Original source resolution still limits enlarged subject sharpness. User visual acceptance remains pending.

[Actual Unity page review](SharedBooster_ContinuousRim_R3_Review.html) · [Original / R2 / new rim comparison](AssetReview.html)
