# Failure reference rebuild R1 — shared header and source composition

Delivered for visual review on 2026-09-21. Native Failure revision **13** uses `ProductionPlayerHeader`, the same component and Home source assets used by Home, World Map, Chapter Map and Pre-Level. The latest user explicitly requested the shared header while the next-screen Failure rebuild was underway.

[Standalone review](Fail_ReferenceRebuild_R1_Review.html) · [scope and test results](QA/ScopeAndVerification.json) · [asset provenance](Sources/AssetProvenance.json).

## Implementation

The Failure adapter reuses its existing profile, settings, balance and plus-button bindings. It disables the copied header graphics and backing, creates one shared artwork layer, and refreshes the shared typography and real saved values after page localization. Profile, Settings, both Shop routes and Out of Energy retain their existing navigation. The header is positioned by the existing safe-area layout; its hanging portrait crest remains visible. Home's presentation and other header adapters were not changed.

Two Scenario Pixa alpha extractions recover the original Ivan silhouette and main panel. Original RGB is retained for Ivan and visible outer carving. The title silhouette is subtracted from both masks so it renders as an independent layer. The main panel's occupied interior is cleared using the existing reconstructed navy material. Original visible scenery surrounds those assets; previous Scenario scenery fills areas hidden in the source. The old secondary scenery crop is disabled to avoid superimposing another background composition.

The original Russian unfinished-goals heading is restored with its plaque. It appears only for a real failed round with pending goals. English, unknown progress and completed-goal states retain live labels; counts, goals, stock and balances stay live in both languages. Existing original title, retry and booster-name lettering is reused.

Retry and Chapter Map remain functional. Paid +5 moves and ad continuation remain disabled and explicitly unavailable, as scheduled in [FAIL-CONT-01](../../DeferredFeatures/FAIL-CONT-01.md); this visual pass does not implement or simulate them.

## Scenario and visual evidence

- Two accepted `model_pixa-background-removal` jobs, **3 CU each / 6 CU total**. No paid retry or additional generation.
- Requests, outputs, billing and masks are retained in `Sources/Scenario` and `Sources`.
- Ivan and heading RGB match their source crops. Panel face changes are recorded by a repair mask; hidden scenery is reconstruction, not recovered original detail.
- Transparent assets inspected on black, white and saturated magenta; the clean environment was inspected independently.
- Actual Unity Russian two/three-goal, English, missing-result and tall safe-area captures inspected. Original carving and character placement are substantially closer; exact overall identity is not claimed.

## Verification

Unity **6000.5.9f1** rebuilt only the active Failure prefab and verified save/readback at revision 13. Baseline01 and Verify01 passed their 28 checks. Final **Verify02 passed 10 Edit Mode + 19 Play Mode = 29 checks**, no failures or skips, covering Failure, Victory and Pre-Level. The added test exercises every shared-header route, changed resource balances and preservation of the failed-round snapshot. Existing checks cover hit targets, safe-area bounds, localization, retry transaction behavior, goal filtering and unavailable offers.

The standalone review passed all **12 image choices**, with no JavaScript errors or horizontal overflow. Captures are real Unity renders with isolated test saves. No user save was used.

No Android build, physical-device test, full-project regression suite, paid continuation or live ad-provider test was performed.

## Changed production and test files

Seven existing Assets files changed:

- `Scripts/Runtime/UI/ProductionPlayerHeader.cs`: Failure adapter.
- `Scripts/Runtime/UI/ProductionFailRuntime.cs`: shared header and source composition.
- `Scripts/Runtime/UI/ProductionFailView.cs`: revision and shared refresh lifecycle.
- `Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`: importer ownership comment; existing art-list iteration imports the added files.
- `Resources/ProductionUI/Pages/Fail.prefab`: rebuilt active revision 13.
- `Tests/ProductionCoreLoopUiEditMode/FailAlignmentEditModeTests.cs`: shared binding/art and idempotence checks.
- `Tests/PlayMode/FailAlignmentPlayModeTests.cs`: source-heading state and shared-header navigation/value coverage.

Four versioned PNGs and their Unity metadata were added under `Resources/ProductionUI/FailAligned`: `environment_source_r13`, `ivan_source_r13`, `panel_source_r13`, and `heading_source_ru_r13`. All 511 prior prefab records remain; the prefab now contains 516 records. No Assets were deleted. Existing art/metadata and all other page prefabs, including Gameplay and Victory, retain their before-task hashes.

## Remaining differences / next action

The shared header intentionally follows Home's approved component, with real values and its existing typography, rather than the reference's sample data. Hidden scenery, blank material repairs, live font identity, inner goal/booster frame details and lower action spacing remain approximations. The Chapter Map action and unavailable labels are intentional functional additions. User visual acceptance remains pending; next action is review of this Failure rebuild.
