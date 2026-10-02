# Visual Page 09 — Failure / Retry R1

Implemented in the existing Unity 6000.5.9f1 project following the user's authorization to start Visual Page 09. The active `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Fail.prefab` now contains the native revision-1 page, owned by the existing serialized prefab builder. Historical handoff remains Splash/Loading context; latest CurrentTask and verified local Victory R8 baseline governed this continuation.

## Design and assets

Separate darker castle/village environment, isolated waist-up Ivan, dedicated opaque navy panel with gold frame, live Russian/English title and labels, failed-goal progress, emerald retry and chapter-map return, four current booster illustrations with stock badges and explanations. HUD is cloned from accepted Pre-Level resources; existing local font assignments and wording are reused. The retry sprite is Victory R7's emerald control with its natural aspect ratio, not stretched. Panel corner/center ornaments retain uniform scaling through a page-only mesh.

New runtime resources in `Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/`:

- `environment_r1.png`: 948 × 1659 clean environment plate, built-in imagegen from a textual specification informed by inspecting the reference.
- `ivan_r1.png`: 1115 × 1311 isolated sprite, built-in imagegen using the existing Loading Ivan as identity guidance. New pose is not pixel-identical to the reference portrait.
- `panel_r1.png`: 810 × 1391 independently generated blank tall panel; protected decorative geometry fits the page.

[Environment/Ivan prompts and source paths](Sources/GenerationPrompts.json), [panel prompt and source path](Sources/PanelGeneration.json), raw selected Ivan/panel outputs and [reproducible preparation](Sources/PrepareIsolatedAssets.py) are retained. The environment is copied unchanged into runtime. No full-page screenshot or contaminated screenshot region is a runtime asset.

Ivan arrived with a painted checkerboard. Used the existing user authorization “Allow local transparency cleanup” to remove its connected neutral exterior and inspected enclosed hair gaps, retaining RGB artwork. Panel arrived with real alpha but alpha=1 residue far outside its outline; removed that residue and trimmed padding, retaining every alpha sample above 1/255. Opaque interiors preserved. All six final black/white/magenta composites inspected. Ivan retains a relatively hard, bright hair rim; edge softness is a remaining limitation.

## Real state and interactions

`LevelFailureResult` is a transient copied progress snapshot, not a save-format change. The existing registered Gameplay failure callback copies goal counts before routing removes the board. Failure displays current-attempt score and only unfinished objectives with correct gameplay icons; completed goals are filtered and one-to-three slots center dynamically. Legacy callers without progress show it as unavailable; no failed result disables retry. Reset clears the snapshot. Rendering grants no rewards or boosts.

Eleven native buttons: retry, chapter-map return, profile, settings, three resource plus controls, four booster explanations. Existing router/service paths are reused. Retry creates a fresh board with the same existing retry policy, including no additional energy debit; this change does not rebalance that policy. Booster taps explain their existing in-game behavior without spending stock or arming a tool. Story reaction remains live in the new page; the legacy overlay is suppressed only for native Fail. Existing Chapter Map, Pre-Level, Gameplay, Victory and other pages retain their art and prefabs.

The reference's 40-gem extra-move purchase and rewarded-ad continuation have no existing Failure implementation. They are not presented as usable offers or fake transactions. This version preserves supported retry and exit behavior. Implementing paid/rewarded continuation would require retaining/reviving the failed board and a separate verified transaction flow.

## Verification performed

Read AGENTS, handoff, current task, source provenance, reference, current files and local changes; verified Unity version and closed Editor. Verified/backed up all 1,778 R8 production entries before editing. No subagents used.

- Run 1: compilation caught a builder local-variable name conflict; fixed. Tests did not run.
- Runs 2 and 3: Failure-only compile/rebuild/readback passed; 10/10 EditMode and 17/18 PlayMode. All four new Failure PlayMode tests passed. The same existing Pre-Level booster-guide test failed because its whole-save assertion included the legitimate five-second full-energy timestamp refresh.
- Corrected that test only: compare a copied save with the clock field normalized, preserving every balance/inventory/other-save assertion. No energy-service behavior changed. Visual review also corrected goal/bottom-label insets, replaced the stretched outer panel and removed exterior alpha noise.
- Final run 4: compile/rebuild/readback passed; **10/10 EditMode + 18/18 PlayMode**, zero failures/skips. No production changes afterward.

Exact test history in [QA/TestHistory.json](QA/TestHistory.json); commands/logs/XML in `UnityVerification/History/`. New checks cover whole-sprite imports/native bindings/no duplicates; copied/clamped/unknown snapshots; actual progress/stock/no rendering transactions; completed-goal filtering; no-result behavior; RU/EN at 941×1672, 576×1280 and 1080×2400 with safe areas; native hit targets, text bounds, centered icons, button proportions and carving clearance; retry double-click protection and existing energy policy; chapter/settings routes; booster explanations; registered Gameplay failure callback copying before board retirement. Callback test invokes the registered delegate, not a manual gem-swap loss. Existing Victory and Pre-Level regressions ran alongside these.

Retained nine final Failure, twelve Victory and twenty Pre-Level PNG/JSON pairs, all four run histories and three inspected earlier Failure pairs. Visually inspected final RU941 and EN576 three-goal portraits; not every capture. These are actual Editor rendering with isolated test saves and simulated displays, not phone screenshots.

Headless Chrome 390×1100 default portrait inspected; all ten portrait choices and three local links resolve. Dropdown interaction was not exercised. Automatic approval review permitted Unity and Chrome launches; no rejected launch.

Preservation audit: 1,771 prior files unchanged, seven existing files changed; 21 new script/art/meta paths. All 919 prior metas and four prior Fail object IDs retained (387 IDs now). All 89 R8 evidence hashes unchanged. Exact paths in [SourceAudit](QA/SourceAudit.json). Prior screens, unrelated local edits, packages/settings/signing/Android guards retained. Changed C# trailing-whitespace check passed. Final production manifest contains 1,799 files.

## Not performed / remaining limits

No APK/AAB, physical Android test, full project suite, live rewarded-ad/purchase transaction, manual full gem-swap loss/retry, manual animation playback, prolonged-play/profiling, calibrated color/blur measurement, exact-font identification or inspection of every capture. No ad/purchase continuation implemented. The reference's exact portrait, lettering, ornaments and booster names differ; current gameplay booster identities/functions are preserved. Existing story fallback can remain Russian in English; small booster labels need phone review. User visual acceptance is pending.
