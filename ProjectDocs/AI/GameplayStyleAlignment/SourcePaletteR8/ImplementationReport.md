# Gameplay source palette R8

[Open the final Unity review](Gameplay_SourcePalette_R8_Review.html).

Implemented six independent normal gems guided by the verified source colors, silhouettes and emblems. Gold is now an embossed coin; framed glass faces and the green botanical motif replace R7's simplified facets. The existing 8×7 board, staged two/three missions, Ivan and booster functions remain. Gameplay revision8; Pre-Level revision2 uses the same collection icons. Visual approval is pending.

## Files and artwork

Added six PNG/meta pairs under `Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned/`: `gem_ruby_r8`, `gem_sapphire_r8`, `gem_emerald_r8`, `gem_gold_r8`, `gem_amethyst_r8`, `gem_orange_r8`.

Changed exactly six existing files: `ProductionGameplayArt.cs` (registration/mapping), `ProductionGameplayView.cs` (revision), owning `Step20ProductionUiFix01SerializedPrefabBuilder.cs` (R8 marker and verified reference hash), `GameplayAlignmentEditModeTests.cs` (revision assertion), and canonical `Gameplay.prefab`/`PreLevel.prefab` through their owners. [Exact manifest](SourceChangeManifest.json) and [incremental diff](AppliedSourceChanges.diff).

Built-in imagegen was used. [Original prompts](GenerationPrompts.json), [selected blue reconstruction](SapphireFinal.json), intermediate correction prompts and unchanged raw sources are retained. [Provenance](GeneratedAssetProvenance.json) records each final source/runtime hash and alpha preparation. The seven native source extracts were previously verified against source RGB; the new generated sprites do not claim identical pixels. No screenshot-region runtime assets, local recoloring or font replacement.

The final four RGBA sprites received previously authorized alpha-only cleanup: near-opaque alpha restored, faint exterior specks removed, empty margins trimmed. Emerald and amethyst required connectivity-based removal of painted checkerboards, preserving every retained RGB value. Reproducible preparation scripts and black/white/magenta proofs are under `QA/`. Blue needed another reconstruction after its first correction remained too narrow; final ratio is about0.73. Earlier outputs are archived as variants.

## Checks performed

- Inspected project instructions, recorded Unity6000.5.9f1, current files and local changes. All1,691 R7 production hashes matched before editing; backups saved outside the patch.
- Two Unity runs rebuilt/read back both canonical prefabs successfully. Final **EditMode8/8 and PlayMode25/25 passed**, no failures/skips. Existing fixtures cover bindings, imports/idempotence, 56 cell input roots and corners, gem aspect/padding, 120 seeded board starts, staged goals/snapshots, booster/card actions, pause, localization and routing/rewards. No new implementation-mirroring tests were added.
- Saved24 final PNG/JSON pairs:15 Gameplay and9 Pre-Level. Root directly reviewed six final portraits: Gameplay RU941, RU576 safe area, RU1080 safe area, round6 RU, round10 EN and Pre-Level round8 EN. Independent review covered Gameplay RU941/RU576 and Pre-Level RU941. These are actual Unity captures with simulated displays.
- Inspected final six-gem proofs on black, white and magenta, plus source/R7/R8 board-size comparison. RGB-preservation assertions passed for alpha preparation. Numerical diagnostics are sRGB proxies, not calibrated color or comfort measurements.
- Final source audit passed: **1,685 baseline files unchanged, six modified, twelve added**; all875 existing metas and all395 Gameplay/476 Pre-Level object IDs retained. Earlier pages, original art, board geometry/data, matching/VFX logic, packages/settings and Android guards unchanged. Final manifest1,703files.
- Actual RU941 R7→R8 pixel comparison: zero changes outside the board and collection-icon area. [Results](QA/UnchangedAreasComparison.json).
- Standalone Chrome viewer:26 views, four local links and360px layout passed. C# whitespace checks passed; full-working-tree `git diff --check` reports81 trailing-space warnings on Unity-serialized empty YAML values in Gameplay/Pre-Level. The broad diff includes earlier local changes. All63 final evidence hashes and63 prior R7 evidence hashes verified. Unity logs retain a non-blocking license-token refresh message in some stages and existing development-font warnings; successful checks are not a claim of warning-free logs.

[Unity commands/results](UnityVerification/VerificationRuns.json), XML and evidence hashes are archived. CurrentTask and SessionLog are updated, retaining earlier decisions.

## Not performed

No APK/AAB, physical-phone or long-session comfort test, profiling, full-project/pure-core test suites, new captures of earlier pages, VFX video, exhaustive special/obstacle combinations, calibrated color measurement or human level-completion/balance study. Exact font/device consistency remains unresolved. Header/footer/rimless boosters and special/obstacle effects are still open, as detailed in the [comparison](ReferenceComparison.md).
