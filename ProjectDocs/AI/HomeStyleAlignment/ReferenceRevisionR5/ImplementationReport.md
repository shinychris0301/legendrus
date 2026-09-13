# Home R5 — reference revisions

> **2026-09-13: R5 visual match rejected by the user.** The user now requires the original Home reference, including its background and character. The functional verification below is historical; it does not establish reference fidelity. See the [current R6 implementation](../ReferenceReplicaR6/ImplementationReport.md) and [R6 Unity review](../ReferenceReplicaR6/Home_ReferenceReplica_R6_Review.html).

Home revision **8** implements the four authorized revisions: broader Firebird and right-side tail, deeper aged HUD/navigation frames, a continuous carved footer, and less empty sky above Ivan on tall screens. Exact approved Splash Ivan, crest and Start remain shared; the winter background is unchanged. User visual acceptance remains pending.

[Actual Unity portrait](UnityVerification/Home/Home_941x1672_RU.png) · [Standalone portrait review](Home_ReferenceRevision_R5_Review.html) · [Direct visual findings](QA/FinalVisualReview.md)

Project root: `/home/k2/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL`. Recorded/executed Unity **6000.5.9f1 (b57deb96f08d)**. Active prefab: `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab`. Existing owner: `Step20ProductionUiFix01SerializedPrefabBuilder.RebuildHomeAlignment`; composition remains `ProductionHomeRuntime`, `ProductionHomeView` and `ProductionHomeLayout`. No parallel startup or routing system.

The historical handoff describes earlier Loading work; the current workspace already contained rejected Home R4. All1,485 expected R4 source hashes matched before edits. Existing local changes were the baseline and were preserved. R3/R4 functional passes did not establish reference fidelity.

| Revision | Implemented result |
| --- | --- |
| Firebird | Independent true-alpha orange-gold Firebird with a broad raised wing, longer outward/downward tail and sapphire eyes. Complete sprite at `(495,442,550,825)`, local angle−15°, claws on Ivan’s shoulder and beak clear of his face. |
| HUD/navigation | New dark wood/lacquer header, aged capsule, four independently painted navigation cards: rolled map/compass, angled blue-awning market, red scroll and steel/gold helmet. Wider blank plaques keep captions live and readable. |
| Footer | New full-width opaque carved base behind retained mailbox/gift and a new shallower dark wood quest panel. Decoration extends to physical side/bottom edges beneath simulated safe insets; controls retain safe bounds. |
| Tall layout | Ivan and Firebird now move down by10% of extra height instead of32%, reducing the gap below the crest while preserving their coupling and native shared Sprite geometry. |

Nine new PNG outputs are installed: eight replacements plus `footer_base_generated.png`. Twelve previous foreground sprites and the winter plate are retained. All nine exactly match original built-in imagegen bytes and runtime resources; all21 R5 source sprites equal runtime bytes. No screenshot crops, screenshot UI, pixel editing or replacement font files. Transparent resources passed black/white/magenta inspection; header/footer intentionally preserve opaque wood surfaces. Narrow navigation candidates and an RGB checkerboard edit were rejected before installation. Prompts/provenance: `FirebirdPrompts.json`, `NavPrompts.json`, `R5PanelPrompts.json`, `SelectedAssetProvenance.json`.

The native12 controls, live resource/level/daily-availability bindings and established routes are retained. Loading prepares25 textures plus1 Home prefab,26 measured operations through the existing service. No artificial progress timer. Existing quest/XP providers are absent, so unavailable markers remain instead of fabricated reference values.

**Checks performed**

- Existing Home-only Unity owner import/compilation/save/readback passed at revision8, exit0. Owner/importer already consume the runtime contract and needed no new source edits.
- **32/32 Edit Mode**, zero failures/skips: `HomeAlignmentDeliveryEditModeTests` and `SplashTitleBuildWiringEditModeTests`. Checks include exact shared Splash resources/material/style, sprite dependencies/import settings, footer ownership, measured whole-source placement and native controls. [Exact cases](UnityVerification/FinalEditMode.json).
- **21/21 Play Mode**, zero failures/skips: `HomeAlignmentPlayModeTests`, `SplashResponsivePlayModeTests`, `LoadingAlignmentPlayModeTests`, `HomeLoadingPreparationPlayModeTests`. Checks cover first/repeated Canvas layout, safe-area bounds, glyph/plus clearance, full maximum balances, all12 pointer routes, service refresh/reentry and real Splash→Loading→Home progress/error/retry. Isolated `/tmp/lor-home-r3/test-config` saves. [Exact cases](UnityVerification/FinalPlayMode.json).
- Seven actual Unity Home portraits retained:941×1672 RU/EN/RU roundtrip,576×1280 simulated safe area/maximum balances,1080×2340 and1080×2400 simulated safe area. Root and independent reviewer directly compared five distinct reference/tall/EN/max-balance portraits to the original and rejected R4. These are Unity render-texture captures, not phone screenshots.
- Reference-size Splash and Loading captures are **pixel-identical to R4**. Home RU→EN→RU roundtrip is also pixel-identical. Read-only decoded PNG comparison JSONs retained under `UnityVerification/`.
- Source audit:1,485 baseline files,1,471 unchanged,14 intended modifications and2 new footer files. All770 existing metadata byte hashes/GUIDs preserved. All332 old Home prefab IDs/class IDs retained;4 added. Protected Splash/Loading/background/packages/ProjectSettings/Android files unchanged. All33 owned text backups match the before hashes. Five changed C# files pass scoped whitespace checks. [Exact16-file inventory](SourceChangeManifest.json), [audit](FinalSourceAudit.json), [GUID evidence](GuidPreservation.json).
- Local Chrome checked all five standalone viewer selections for successful image load and no horizontal overflow. This validates the review viewer, separately from Unity runtime execution.

Changed production scope is eight Home PNGs, Home.prefab, runtime/view/layout, the two Home test files, and new footer PNG/meta. Root documentation/evidence changes are under this R5 folder plus CurrentTask/SessionLog. No package, guard, backend/color-space, Player/signing or approved Splash/Loading edits; no staging/reset/commit. Generated caches and raw Unity logs are excluded from the source change manifest.

**Not performed:** APK/AAB build; physical-phone installation, rendering/input or performance checks; full-project regression/memory/performance suite; release/custom-signing validation; physical display-resolution-change testing; exact font identification; numerical color or pixel identity against the Home reference. The existing APK predates this Home revision.

Remaining visual limits: the mandatory Splash character/crest/Start and preserved daytime winter setting differ from the original dusk reference. Independently painted ornament details and colors are approximations. Navigation caption baselines vary slightly with their painted plaques; extreme maximum balances fit in small two-line text. Quest/XP state remains unavailable. No exact-reference claim or user approval is inferred. Next action: review the actual R5 Unity portrait; any later Android build uses the existing guarded path.
