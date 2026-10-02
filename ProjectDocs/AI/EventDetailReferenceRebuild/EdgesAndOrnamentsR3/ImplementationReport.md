# Event Detail R3 — Firebird Edges and Panel Ornamentation

Repaired the Firebird silhouette and restored the countdown plaque, feather-balance border and premium reward enclosure in the active Unity Event Detail page. This implements the first refinement recommended in the R2 fidelity recheck.

[Native capture review](EventDetail_EdgesAndOrnaments_R3_Review.html) · [Reference findings](QA/ReferenceFidelityReview.md) · [Changed files and test results](QA/ScopeAndVerification.json)

## Visual changes

- Firebird uses a fresh Scenario contour guide mapped back onto original RGB. Closed the broken feather ribbons and restored their blue eye markings, removed adjacent title-rail fragments, and restored recognizable original starburst highlights. Independent light layers remain aligned to the bird.
- Countdown is the original shaped plaque at its source position, overlapping the bird where the reference occludes it. The exposed straight cutoff above R2's smaller rectangular bar is covered in the final native captures. No claim is made to have recovered feathers hidden behind the original plaque.
- Feather balance has its separate gold-bordered navy inset. Live unknown balance remains inside it.
- Premium examples have the original gold enclosure and opaque navy face. The preview note was resized to clear the enclosure; native item art draws in front of the frame.

The shared Home background/header, live energy, original Russian title/button lettering, safe-edge navigation, event preview restrictions and existing routes remain. Play and Activate stay disabled. No event dates, thresholds, quantities, rankings or grants were invented.

## Source and Scenario use

Source: the dedicated Firebird Festival original, copied in [Sources/OriginalReference.png](Sources/OriginalReference.png). One `model_openai-gpt-image-2` Scenario edit returned the bird extraction guide and empty panel faces: **12 CU**, no paid retries. The prompt and input are preserved in `Sources/Scenario/Request.json` and `Sources/Guidance.png`; billing/status/download evidence is beside them.

Scenario output colours do not replace the bird: source-guided local segmentation and bounded manual corrections use original RGB. The final bird has 102,010 visible pixels with zero RGB mismatches against its source crop; the source-light layer also preserves RGB. This measures colour provenance, not perfect alpha fidelity. Repaired panel interiors and the soft halo are reconstructed. A short countdown rail donor removes old claw remnants that Scenario retained.

Inspected bird and panel transparency on black, white and magenta, then inspected final native Russian and narrow English views. Fine strand alpha and tiny highlights remain approximate. Intentionally opaque panel faces were retained.

## Implementation scope

Seven existing Assets files changed: runtime, view, builder, active EventDetail prefab, cutover state, Edit Mode fixture and Play Mode fixture. Six new PNGs plus six metas were added under `Resources/ProductionUI/EventDetailAligned/`: `firebird_r3`, `firebird_sparks_r3`, `firebird_halo_r3`, `countdown_r3`, `balance_r3`, `premium_inset_r3`.

Internal revision **4** agrees across prefab owner and cutover. No existing PNG/meta or other page prefab was changed; no deletions. Source generation/extraction scripts, review builder and audit evidence are stored in this review directory. Scope compares against the task's initial Assets hash snapshot, preserving earlier working-tree changes.

## Verification

Unity **6000.5.9f1**, Verify01: prefab rebuild/import succeeded; **4 Edit Mode + 10 Play Mode tests passed**, zero failures/skips. EventDetail and Events regression fixtures cover shared header/energy, repeat-build stability, localization, native navigation, disabled preview behavior, safe areas and text fit. Added assertions verify that feather content and premium items stay inside restored borders and that the frame cannot intercept input or cover the icons.

Six EventDetail captures and eight Events regression captures were produced with isolated test saves. Review browser check passed nine views and original-comparison toggle at 430×900, without script errors or horizontal overflow. The review displays actual Unity captures rather than offline composites.

No Android build, physical device, performance assessment or full-project suite was run. Remaining fidelity work: carved footer/side circles, leaderboard row and portrait proportions, individual chest scale and section typography. Recommended next: **R4 — Footer and Leaderboard Proportions**, after visual review.
