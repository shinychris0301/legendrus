# Special Offer R2 — Fresh Scenario chest extraction

The Special Offer hero and bonus icon now use a newly isolated chest from the original Shop reference. The old PreLevel sprite and previous Shop matte were not reused. Other pages are unchanged.

## Extraction

One new Scenario API job (`model_pixa-background-removal`, `job_8trr4FcfDkdMCccxSiZh5DmZ`) succeeded at 3 CU. Scenario also retained the attached 2 500 price plaque. A local traced alpha boundary removes that plaque while keeping original source RGB exactly. Final sprite is 366 × 255; no upscaling or regenerated hidden details. Transparent edges were inspected on black, white and magenta. A small underside occlusion inherited from the source remains; this is not claimed to restore unseen pixels or provide unlimited resolution.

Dedicated runtime resource: `SpecialOfferAligned/chest_r2.png`. Native runtime sets its aspect-correct hero dimensions and shares it with the bonus icon. Owning prefab builder now explicitly verifies both serialized sprite references. Structural revision remains 1 because the binding schema did not change; runtime, prefab and cutover marker agree. No price, timer, reward recipe or purchase behavior changed.

## Checks performed

- Unity 6000.5.9f1 compilation/import, owner rebuild and serialized readback: passed.
- EditMode: 4/4 passed (SpecialOffer and Shop).
- PlayMode: 6/6 passed (SpecialOffer and Shop); no failures or skips. Coverage includes localized live text/balances, safe areas, aspect ratios, raycast targets, navigation and unavailable-purchase behavior.
- Five actual SpecialOffer PNG/JSON pairs and seven Shop regression pairs saved. SpecialOffer 941×1672 RU/EN and 576×1280 safe-area RU/EN visually inspected. 1080×2400 and Shop captures checked automatically, not manually reviewed this pass.
- Source RGB equality assertion and runtime provenance hash: passed. Python syntax and local review links: passed.
- Production snapshot: 4649 baseline files; 4646 unchanged, 3 changed (SpecialOffer runtime, builder, prefab), 2 new (sprite/meta), no deletions. Every existing meta preserved. Shop/PreLevel assets, services, Android guards, packages and settings unchanged.
- Broad `git diff --check` returned existing Unity YAML trailing-whitespace findings in the previously dirty worktree; these unrelated files were not rewritten. Changed C# files checked separately for trailing whitespace.

## Not performed

APK/AAB build, physical-phone testing, full-project test suite, manual browser execution, payment/provider/SMS testing, extended manual play/profiling, calibrated color or exact-font verification. No claim of exact whole-page fidelity: no approved standalone SpecialOffer reference exists locally.

Review: [actual Unity captures](SpecialOffer_ChestExtraction_R2_Review.html). [Changed files](ChangedFiles.md). [Provenance](Sources/RuntimeProvenance.json).
