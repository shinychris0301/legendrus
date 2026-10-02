# Gameplay upper HUD clarity R5 — implemented, awaiting visual review

The first pass of the agreed readability/effects work improves the upper HUD. The hero name and level sit lower, with larger bold cream lettering; goal and move labels use a simpler solid finish. Score stars are 56×56 reference pixels with stronger inactive gold rims. Objective icons are centered over a common, larger count baseline. Score and survival goals now use isolated painted gold-star and gold/sapphire-hourglass sprites instead of geometric symbols.

[Review actual Unity portraits, R4 and the reference](Gameplay_UpperHudClarity_R5_Review.html). The full-resolution image link avoids the review viewer's display downscaling when inspecting lettering.

The existing font family and glyph sampling remain. Only upper text instances lose the decorative gold mesh effect, gaining a restrained dark shadow and solid cream fill. Exact source-font identity and the cause of device blur remain unverified. This is improved capture readability, not proof of a phone rendering fix.

## Implementation and preservation

Canonical Gameplay prefab, owning serialized builder and runtime view agree on revision 5, marker `gameplay-upper-hud-r5-readable-text-stars-painted-goals`. The eight existing changed files relative to the local R4 baseline are:

- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayHudStyle.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayArt.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayStarPolish.cs`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/GameplayAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/GameplayAlignmentPlayModeTests.cs`

Added `goal_score_r5.png`, `goal_survival_r5.png` and their two `.meta` files under `Resources/ProductionUI/GameplayAligned`. Existing goal-symbol components remain serialized but disabled. Counts, completion checks, score thresholds and live rules retain their original semantics.

Built-in imagegen created two independent resources; no screenshot region was used. [Exact prompts](GenerationPrompts.json), raw sources in `Sources/`, and [provenance](GeneratedAssetProvenance.json) record the workflow. Previously authorized local transparency cleanup adjusted alpha and trimmed empty margins, preserving retained RGB bytes. All six black/white/magenta proofs were directly inspected. Gold and painted glass surfaces remain intentionally opaque.

The [source audit](FinalSourceAudit.json) confirms 1,636/1,644 baseline files unchanged, all 852 existing metadata files and all 304 prefab object IDs retained, and new GUIDs unique. Four added files bring the recorded production manifest to 1,648. Earlier local work is retained. Prior pages, supplied sheets, previous artwork, gameplay rules/VFX, Android guards, packages and settings are unchanged against that baseline. See [exact incremental diff](AppliedSourceChanges.diff) and [file hashes](SourceChangeManifest.json).

## Checks performed

- Unity 6000.5.9f1 import and owning Gameplay rebuild exited 0.
- Gameplay EditMode fixture: **4/4 passed**. Checks canonical controls/reference isolation, polish reapplication, imports/prior revisions and supplied-sheet bytes/regions.
- Gameplay PlayMode fixture: **5/5 passed**. Checks authored goal/obstacle/feather bindings (including new score/survival sprites), portrait captures, localized status placement, live booster stock/moves and pause/input behavior.
- Fifteen fresh Unity PNG/JSON capture pairs: RU/EN, 941×1672, simulated 576×1280 and 1080×2400 safe areas, language roundtrip, status and authored objective variants. Directly inspected eight: RU and EN reference portraits, both RU phone shapes, Level 6 RU, Level 9 RU, Level 10 RU and tall EN status. Other captures were produced by passing tests, not all manually inspected.
- R4→R5 reference RU pixel comparison: **zero changed pixels below the allowed upper-HUD region (y=285)**. Board, scenery and footer are pixel-identical in this comparison. RU language roundtrip differs in 57 pixels by at most 1/255 per channel; not exact pixel identity.
- Source preservation, unique new GUIDs, retained prefab IDs and changed-C# whitespace checks passed. Evidence hashes and command/result records are archived under `UnityVerification/`; raw Unity logs remain outside the patch with hashes recorded.
- Standalone browser viewer results are recorded in `QA/StandaloneReviewBrowserChecks.json`, including every selectable portrait, local links and 360-pixel layout. This checks the review page, not Unity rendering.

## Not performed and remaining work

No APK build, physical-phone test, performance/memory profiling, full-project or pure-core suite, fresh prior-page captures, or continuous effects video. No assertion that every rendering detail matches the reference exactly.

Next pass: integrate match identity into generated specials, unify row/column booster artwork, replace flat activation washes with shaped local effects, and complete readable obstacle damage states. Those designs and implementations are still unfinished. The current header pass is implemented and verified, not yet visually accepted by the user.
