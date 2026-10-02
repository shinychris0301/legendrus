# Victory R3 — scene, Firebird and resource header

Implemented the approved R3 refinement in the active Victory prefab and its owning runtime/builder. Replaced the reused rural plate with a clean illuminated evening-city environment and floral foreground, replaced the heavy Firebird with an airy gold sprite and flowing luminous tail, and rebuilt the top bar as three wood/gold resource compartments. All resource values and plus buttons remain live; the inset energy status reads the existing GameState.EnergyRemaining() clock, or shows localized FULL / ПОЛНАЯ.

[Portrait review](Victory_SceneAndHeader_R3_Review.html) · [Remaining reference differences](ReferenceComparison.md) · [Exact changed-file and preservation audit](QA/SourceAudit.json)

## Artwork and ownership

New files are `environment_r3.png` (941×1672), `firebird_r3.png` (1254×1254), and `resource_header_r3.png` (1978×282), plus their metas, in `Assets/LegendsOfRus/Resources/ProductionUI/VictoryAligned/`. Generated with builtin image_gen, using the original Victory image only for visual guidance. Prompts and source copies are in [Sources](Sources/GenerationPrompts.json); [provenance](Sources/AssetProvenance.json) records final paths and hashes.

The environment and Firebird are byte-identical copies of the generated assets. Both header generation attempts produced painted checkerboards. Used the user's earlier authorization for local transparency cleanup: removed only border-connected neutral background, preserved opaque wood/art RGB, and trimmed transparent exterior margins. [Cleanup record](QA/HeaderTransparencyCleanup.json). Firebird and header were inspected on black, white and saturated magenta backgrounds. No screenshot-region crops, reference texture runtime dependency, baked resource numbers or invisible hotspots.

Canonical page remains `Resources/ProductionUI/Pages/Victory.prefab`, revision 3. Existing HudFrame is reused; old capsule/icon objects remain present but inactive. All 313 previous serialized object IDs remain (319 now). Builder and runtime apply the same in-place upgrade. A narrowly scoped import rule for only the three new R3 asset paths preserves native dimensions, alpha and Android RGBA32; older art imports are unchanged.

Seven existing files changed: Victory prefab, ProductionVictoryRuntime.cs, ProductionVictoryView.cs, owning builder, LockedDesignImporter.cs, and two existing Victory test fixtures. Six new art/meta files added. Of 1,754 baseline production-manifest entries, 1,747 are unchanged. All 907 existing metas are unchanged. Final manifest has 1,760 entries. Unrelated local edits were preserved; no git reset/commit was made. ProductionVictoryLayout, R2 reward/title/star/footer styling, GameState, navigation services, other pages, packages/settings and Android guards are unchanged.

## Verification performed

- Read AGENTS, handoff, CurrentTask, provenance, recorded Unity version and existing working-tree changes. The Loading-era handoff is historical; installed R2 and current task records establish this baseline.
- Directly compared the original 941×1672 reference, retained R2 capture and new actual Unity captures. Independent source/preservation review found no blocking regression.
- Unity 6000.5.9f1 compile + owner rebuild/readback passed twice, using isolated test storage and existing display/licensing services. Initial sandbox launch failed before compilation on display/licensing access; automatic approval review allowed the same run with service access.
- First successful run: 6/6 EditMode and 13/13 PlayMode tests passed.
- After the energy compartment spacing correction: 6/6 EditMode, 12/13 PlayMode passed. The new countdown test raced the phase between UTC and Unity's frame timestamp after a fixed 1.2-second wait.
- Replaced only that test's fixed delay with a bounded 2.5-second observation of the next displayed tick. Countdown decrement, service-clock agreement, save timestamp/energy/gold preservation, and RU/EN full status assertions remain. Targeted retry compiled and passed 1/1 in 1.48 seconds; no production changes after the spacing correction. All 19 distinct selected tests now have passing evidence for the final production code. No claim that the final combined run was wholly green.
- Functional coverage includes ten authored level rewards, current-round score, replay eligibility, earned stars, reset/no-result, gold-only once-only domain bonus, unavailable video, Continue/Shop/double-click, registered Gameplay win callback, Pre-Level regression, native input and RU/EN responsive layout.
- Retained 12 final Victory portraits: 11 from final combined run and one countdown portrait from the labelled retry; 20 Pre-Level portraits from the final combined run. Retry records remain separately under CountdownRetry. Reference-size RU, tall-phone countdown RU and five-reward EN were directly inspected; all portraits were produced with automated text/input/safe-area assertions.
- Single-portrait HTML viewer structure/paths checked; 14 portrait choices, three report links. Separate temporary headless Chrome rendered the default portrait at 390×1100; screenshot inspected. Sandboxed Chrome initially failed a socket operation, then the approved isolated-profile run passed. Dropdown interaction was not exercised in the browser.
- Production/meta/object-ID preservation and native source-dimension/import contracts checked. Scoped C# whitespace check passed. Whole-working-tree git diff --check reports existing/Unity-serialized trailing whitespace in prefab YAML; unrelated files were not reformatted.

Exact Unity commands, XML, logs and previous-run records are retained in UnityVerification; [test history](QA/TestHistory.json). [Final production hashes](FinalProductionHashes.json).

## Not performed / remaining limits

No APK/AAB build, physical Android test, full project test suite, successful live rewarded-ad transaction, full gem-swap win session, prolonged-play/profiling, calibrated color metric or exact-font identification. Still captures do not establish animation quality. Existing story text may fall back to Russian in EN. Reference resemblance has improved, but independent artwork is not pixel-identical and new user visual acceptance is pending.

Next focused art pass: curved banner lettering and softer luminous stars, then reward-card scale and footer ornament fidelity. The energy timer is a secondary small label on tall-phone captures; device readability remains untested.
