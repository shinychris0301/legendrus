# Gameplay booster cards R9

[Review actual Unity portraits, R8 and the reference](Gameplay_BoosterCards_R9_Review.html).

The four original booster objects now render directly on wood/gold cards. Removed their baked circular rims and backings through alpha-only isolation, preserving original central artwork colors and trim. Names, inventory and actions remain unchanged. A separate blank card provides a deeper two-line name well; stock badges remain above the labels and the armed indicator remains visible. Gameplay revision 9; Pre-Level presentation and R8 gems are unchanged. Visual acceptance is pending.

## Implementation and assets

Added five PNG/meta pairs in `Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned/`: `booster_mace_r9`, `booster_firebird_r9`, `booster_livinglight_r9`, `booster_seapearl_r9`, `card_wood_r9`.

Four boosters come from the existing dedicated atlas / isolated SeaPearl, not reference-screen crops. [Provenance](GeneratedAssetProvenance.json), source regions and reproducible alpha masks retain exact RGB; [asset checks](QA/AssetChecks.json) verify this. Own mace/shield/bottle metal decoration remains. Native art resolution is preserved; no new booster drawing, upscaling or recoloring. Fine flame and chain edges remain limited by the source.

Only the blank card uses built-in imagegen. [Initial prompt](GenerationPrompts.json), [label-well correction prompt](CardLabelWellCorrection.json), raw sources, earlier variant and [card provenance](CardProvenance.json) are saved. Authorized local alpha-only cleanup removes painted checkerboards; interior wood stays opaque. Black/white/magenta proofs were inspected for all five assets.

The new `ProductionGameplayArt.Booster(index)` mapping is exclusive to Gameplay; prior `Load`, atlas regions, normal gems, obstacles and Pre-Level resources are intact. Cards are 180×252 at their existing positions, increasing gaps from 8 to 12 reference pixels. Protected corner slices keep the lower name well deep. Existing button/count/marker object IDs are retained; stock badges are reparented to the card to separate them from object scaling. Live labels use explicit RU/EN line breaks with unchanged wording. Full-card input forwards to the same retained buttons and transactions.

Nine existing files changed: `Gameplay.prefab`; `ProductionGameplayArt.cs`; `ProductionGameplayRuntime.cs`; `ProductionGameplayHudStyle.cs`; `ProductionGameplayView.cs`; `GameplayAlignmentArtImporter.cs`; owning `Step20ProductionUiFix01SerializedPrefabBuilder.cs`; `GameplayAlignmentEditModeTests.cs`; `GameplayAlignmentPlayModeTests.cs`. [Exact manifest](SourceChangeManifest.json), [incremental diff](AppliedSourceChanges.diff), [source audit](FinalSourceAudit.json).

## Verification

Initial checks caught a 2-reference-pixel shortfall in the longest Russian label; its text area widened from 150 to 154 without reducing the 18 px display font. The next run passed tests, but manual and independent inspection found gold leaves behind the ends of “МОРСКОГО ЦАРЯ”. The final card removes those inward ornaments to clear the label field. These findings demonstrate why logical bounds checks are supplemented by visual inspection.

Final verification used Unity 6000.5.9f1 (b57deb96f08d). The canonical Gameplay rebuild completed successfully. EditMode: **8/8 passed**; PlayMode: **25/25 passed**, no skipped tests. EditMode filters cover Gameplay/Pre-Level alignment; PlayMode adds level-goal and level-system runtime fixtures. This was not a full-project test run.

Saved 24 final PNG/JSON capture pairs: 15 Gameplay and 9 Pre-Level. Root directly inspected four final Gameplay portraits: RU and EN at 941×1672, RU at 576×1280 with simulated safe area, and the RU Living Light armed/status state. An independent reviewer inspected those four too and confirmed the SeaPearl text/ornament collision is resolved. Other portraits are available for review and were checked by the capture tests and viewer loader; they were not all manually inspected. [Final independent visual review](QA/IndependentFinalVisualReview.md).

The standalone Chrome viewer check passed all 26 views and four local links, with no horizontal overflow at 360 px. Its 360 px browser screenshot was inspected; it displays saved Unity images, not a new runtime render. [Browser checks](QA/StandaloneReviewBrowserChecks.json).

The final source audit passed: 1,694 of 1,703 pre-task production files unchanged, nine existing files modified, ten PNG/meta files added; final manifest 1,713 files. All 881 existing metadata files and GUIDs remain unchanged. Exact prefab object-ID sets are retained: Gameplay 395, Pre-Level 476. The four booster derivatives retain exact source RGB at every retained pixel. All 66 archived Unity evidence hashes were checked.

[Preservation/pixel checks](QA/PreservationAndPixelChecks.json) revalidated all 1,713 final production hashes and all 63 original R8 evidence hashes. All nine fresh Pre-Level portraits are hash- and pixel-identical to R8. Scene/board strips are pixel-identical across all 15 Gameplay comparisons. Exact equality above the footer is **not** achieved: the HUD/status text band differs by 0–70 pixels per portrait, with at most 2/255 channel difference. A dynamic-font atlas sampling change is a possible explanation, not a verified cause. The audit records this exact-pixel check as false while source preservation passes.

Whitespace: all eight modified C# files pass incremental checks and the C# git check. The full Unity YAML check reports 79 empty-value trailing-space warnings, versus 81 in R8; it is not a clean full git-diff check. Commands and all run outcomes, including the first failed fit check, are in [VerificationRuns.json](UnityVerification/VerificationRuns.json). Tests extend existing fixtures with actual card-edge raycasts/forwarding, RU/EN rendered label bounds and no truncation, badge clearance and armed-marker rendering. Existing board/goals/routing/booster semantics checks remain.

The source audit protects all previous art and GUIDs, exact Gameplay/Pre-Level object IDs, board geometry/data, character/earlier pages, existing booster input/transactions, packages/settings and Android guards. It also checks that runtime edits are limited to the sprite mapping, footer style loop, name line breaks and revision. Pre-Level is not rebuilt or edited by this pass.

## Limits and next work

No APK/AAB, physical-phone test, long-session comfort measurement, profiling, full-project/pure-core suites, full human playthrough, balance study, VFX video or exhaustive special/obstacle combinations. The exact font remains unverified and the existing development fallback is unchanged. Final Unity logs contain the existing development-font warning, a non-blocking license-token refresh error, and Editor memory guard notices (up to 487 MiB allocated / 965 MiB reserved). These Editor observations are not an Android performance measurement. No OS font file was added or distributed; passing tests do not mean warning-free startup.

Cards are an adaptation to the reference using the original booster types. Header/portrait/plaque/star materials and spacing remain the next focused pass. Scenery composition, special/obstacle states and activation effects remain open; details in the [comparison](ReferenceComparison.md).
