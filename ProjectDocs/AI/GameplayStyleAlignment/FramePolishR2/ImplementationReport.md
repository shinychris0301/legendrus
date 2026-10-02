# Gameplay frame polish R2

Gameplay's plain board border, blue Chapter Map moves node, simple goal box, flat score area and grey inactive stars did not reproduce the reference's material and ornament. Native revision **2** refines those five areas while preserving the supplied gems, boosters, environment, portrait and live gameplay behavior.

[Actual Unity portrait review](Gameplay_FramePolish_R2_Review.html) · [Reference comparison](ReferenceComparison.md) · [Exact change manifest](SourceChangeManifest.json).

The user authorized the recommended first refinement with “okay, continue you recommend” and confirmed closing Unity. R2 is delivered for visual review; acceptance is not inferred. The later VFX/tutorial/Pause/toast work identified by the R1 audit remains separate.

## Changes

- A dedicated black-lacquer moves frame replaces the Chapter Map node. Its antique-gold leaf scrolls and sapphire accents surround the existing live moves label/value.
- One continuous, independent lacquer texture underlies the board; native translucent cell faces retain selection tint. Thin gold joints now have small lobed ornaments. The outer rim has shaped corners, directional metal shading and sapphire details.
- The goal box gains lacquer material, ornamental corners and engraved curls. Goal identity, targets and progress calculations are unchanged.
- An opaque dark bronze score inset with gold trim and inner shadow sits beneath the live score; heading/value positions are adjusted within the existing footer.
- An effect attached only to Gameplay's three star objects renders dark centres and gold outlines when unearned. Existing earned facets and score thresholds remain. The shared World Map renderer is unchanged.

The real `ProductionUI/Pages/Gameplay.prefab` and `ProductionGameplayRuntime` composition were updated together through `Step20ProductionUiFix01SerializedPrefabBuilder.RebuildGameplayAlignment`. The owner records the R2 marker. Upgrade/reapplication modifies the existing hierarchy in place. All **250 previous prefab object IDs** remain; 15 component/object IDs were added, for 265 total. Five native controls, board input roots and serialized references remain intact.

Eight existing files changed: Gameplay prefab, editor owner, ProductionGameplayArt/Cell/Runtime/View, and the two Gameplay alignment test fixtures. Six files were added: ProductionGameplayStarPolish.cs/.meta and two PNG/.meta pairs. [Full source diff](AppliedSourceChanges.diff).

## Artwork and provenance

Built-in imagegen produced two new assets using the entire Gameplay reference as style guidance: `Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned/moves_frame.png` and `cell_lacquer.png`. [Exact prompts](GenerationPrompts.json), original outputs in [Sources](Sources), and [source/runtime hashes](GeneratedAssetProvenance.json) are retained.

The moves frame output was RGB with a painted checkerboard. Under the user's existing local-transparency-cleanup authorization, only alpha was authored to remove neutral exterior pixels and gaps; original RGB, dimensions and opaque lacquer were preserved. The [black/white/magenta proof](QA/moves_frame_alpha.png) was directly inspected. The cell texture is intentionally opaque and byte-identical to its generated source. No screenshot region, complete reference image, repainted gem, new logo or OS font is used at runtime. New ornament is a reconstruction, not an exact source-asset match.

## Checks performed

- Verified project root, Unity **6000.5.9f1 (b57deb96f08d)**, instructions/handoff/current task and local changes. All 1,624 baseline hashes matched before editing. Prior user edits were retained.
- Unity Gameplay-only owner invocation compiled/imported/saved/read back revision 2, exit **0**. [Owner evidence](UnityVerification/OwnerImport.json).
- Focused **4/4 Edit Mode** and **4/4 Play Mode** tests passed, zero failures/skips. [Exact runs and cases](UnityVerification/TestRuns.json), [Edit XML](UnityVerification/EditMode.xml), [Play XML](UnityVerification/PlayMode.xml). New checks cover repeated application without duplicate objects or lost bindings, opaque score inset placement, Gameplay-only star effects and rendered dark star centres. Existing checks cover all ten levels, asset mappings, real cell/button raycasts, responsive layout, live inventory/+5 moves, double invocation and pause/resume.
- Ten unedited Gameplay PNGs and ten metadata files retained. Directly inspected RU/EN 941×1672, 576×1280 and 1080×2400 simulated safe areas, plus Level 6. All later authored levels are exercised by the existing fixture; no exhaustive playthrough is implied. [Capture manifest](UnityVerification/CaptureManifest.json).
- Source/GUID audit: **1,616/1,624 original paths unchanged, eight modified, six added**; final hash count **1,630**. All **842 original metadata files/GUIDs** preserved; new GUIDs unique. Earlier pages, shared World Map graphics, Match3Board/rules, prior artwork, packages, ProjectSettings and Android guards/signing sources are unchanged. [Audit](FinalSourceAudit.json).
- Pixel comparison with R1 confines substantive changes to the five intended areas. Outside their bounds, three pixels differ by at most 1/255. R2 RU/EN/RU roundtrip differs at eight pixels, also at most 1/255; these images are not described as identical. [Measurements](QA/VisualScopeComparison.json).
- Local Chrome checks of the standalone review are recorded in [browser results](QA/StandaloneReviewBrowserChecks.json). This HTML displays actual Unity captures; it is not a browser implementation of Gameplay.

## Not performed / remaining work

No APK/AAB or Android Player build, phone installation/render/touch, fresh earlier-page runtime captures, full-project/core suite, exhaustive level solving, release/signing checks or performance/memory profiling. Historical R1 core failures and intermittent capture differences were not rerun or cleared by these focused checks. Unity Editor captures and simulated safe areas do not establish device acceptance.

Selection/activation artwork, tutorial/Pause/toast styling, dedicated score/survival goal symbols, ice/durability polish and broader scenery/typography fidelity remain as recorded in the [R1 audit](../R1/QA/DesignAudit/DesignImplementationAudit.md). Exact font identity and reference pixel identity remain unverified. The first recommended refinement is complete; next is review of this R2 portrait, then event-driven gem/booster lighting if requested.
