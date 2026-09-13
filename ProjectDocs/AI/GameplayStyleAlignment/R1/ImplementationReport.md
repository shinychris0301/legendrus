# Visual Page 07 — Gameplay R1

> Follow-up design audit: alignment is partial. [Confirmed missing styling and evidence](QA/DesignAudit/DesignImplementationAudit.md). This historical implementation report does not establish visual acceptance.

Gameplay is implemented in the existing Unity project using the supplied gem and booster artwork, live board state and a native responsive HUD. Pre-Level R1 is accepted by the user’s instruction to proceed; Gameplay R1 is ready for review and has not yet been accepted.

[Open the standalone portrait review](Gameplay_Alignment_R1_Review.html). Its default image is an **actual Unity Play Mode capture**, with reference, phone, English and later-level views selectable individually. This HTML is a capture viewer, not a replacement game implementation.

## Implementation

Project root: `/home/k2/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL`. Recorded and executed Editor: **6000.5.9f1 (b57deb96f08d)**; capture renderer: **OpenGLCore**. The earlier Splash/Loading handoff is historical. The workspace had progressed through accepted Home R11, FrontEnd R1, World Map R1, Chapter Map R1 and Pre-Level R1; its 1,585 recorded production hashes matched before this work.

Canonical page: `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab`, native revision **1**. Its existing serialized owner, importer, runtime dispatcher and router were updated together. `ScreenRouter.BuildGameplayOverlay` mounts the same `Match3Board` under the responsive native page; original win/fail, progression, analytics and tutorial callbacks remain. There is one existing Canvas/EventSystem and five native page buttons.

The page includes Ivan’s accepted whole Home portrait/ring with a circular stencil, selected level, live moves, typed goals, actual goal/star progress, live score, four booster buttons with raised emerald inventory badges and pause. Board input remains on the complete cell rectangles; only their decorative inset and visual size changed. Art now represents real ice, chain and crate durability, travelling feathers, and row/column/burst/colour specials. Specials include a small untinted gem marker for their underlying match identity; fixed red/blue special artwork does not imply a different match colour. Existing gesture ownership, animations, pooled VFX, rules, level records, attempts and booster transactions were retained. Star display follows the existing `LevelData.EvaluateStars` first-star convention.

## Artwork and reference comparison

[Reference comparison](ReferenceComparison.md) · [Original supplied files](UserAssetProvenance.json) · [Atlas rectangles](AtlasRegions.json) · [Transparency cleanup](TransparencyCleanup.json).

All **five** supplied sheets were located and visually matched. Three are imported unchanged as runtime atlases, using 14 clean regions. Four additional crowded regions use separate PNGs after local removal of neighbouring alpha components: potion, feather, vertical rocket and colour special. Original RGB and retained alpha remain unchanged; no resampling or repainting. This is extraction from user-supplied isolated asset sheets, not screenshot-region cropping. The two alternate/state sheets remain preserved as source material; extra tile identities and a new glow-state animation system were not added.

Six normal tiles use the five distinct coloured gems plus the supplied blue-and-gold shield. Existing six-type game rules are preserved. The supplied booster medallions and all four special types are integrated. The reference’s example values, board arrangement and giant mid-action flash are not baked into an idle page.

Three new resources were generated with the **built-in image_gen tool**: continuous dusk environment, empty gold/navy panel, separate chain overlay without a baked red gem. Runtime location: `Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned/`. [Exact prompt set](GenerationPrompts.json) and [generated provenance](GeneratedAssetProvenance.json) are retained. The generated panel had a painted checkerboard; its exterior was cleaned under the user’s existing **“Allow local transparency cleanup”** authorization, retaining the opaque navy face and RGB. Environment and chain use the original generated PNGs. Foreground assets were inspected over black, white and magenta; initial atlas contamination and corrected proofs remain in QA.

New filigree, scenery and chain artwork are recreated, not pixel-identical. Exact source-font identity remains unverified; existing local typography and gold text treatment are reused. Earlier tutorial and Pause modal designs remain in place. Reference/source screenshots are not runtime UI dependencies.

## Checks performed

| Check | Result |
| --- | --- |
| Unity compilation, import, canonical prefab save/readback | Passed. First owner launch found a local variable name collision; corrected. Four subsequent owner launches saved/read back revision 1. |
| Final UI Edit Mode | **56/56 passed**, covering Gameplay and all six earlier pages. |
| Final combined Play Mode | **104/105 passed**. All 68 Gameplay, input, level, goal, motion and VFX checks passed. The failure was the unchanged Splash strict language-roundtrip capture check. |
| New Gameplay fixture | **4/4 passed**: all ten authored level states, inventory and +5-move transaction/double invocation, native pause/resume/input blocking, responsive captures. |
| Splash strict assertion recheck | **1/1 passed**, same source and tolerance. Broad-run failure was seven lettering pixels, maximum difference 1/255. Intermittent behavior is retained, not reported as a clean broad run. |
| Additional pure gameplay-core Edit Mode | **64/66 passed**. Two failures in unchanged core/test sources remain: `SpecialMetadata_FallsWithItsTile` and `AdjacentMatch_DamagesCrateOnlyOnceForTheWave`. The latter exhausted 128 reshuffles. Root causes not fully diagnosed; see [recorded failures](QA/ExistingCoreTestFailures.json). |
| Source/GUID preservation | Passed against the accepted local baseline: **1,577 unchanged, eight modified, 39 added**, final hash count 1,624. All **822** prior metadata files/GUIDs and all five original Gameplay prefab object IDs preserved. |
| Actual render and input review | Ten Gameplay portraits/fixtures; RU/EN, 941×1672, 576×1280 and 1080×2400 simulated safe areas; later ice/crate/chain/feather/combined states. All 72 cell centres and five controls raycast to the correct targets; button/board separation, cell/star/badge meshes, environment coverage, safe-area buttons and repeat layout checked. |
| Earlier-page capture comparison | Chapter Map 6/6, World Map 8/8 and Splash baseline comparisons 7/7 exact. Loading 6/8 exact: six pixels by 1/255 in the tall capture and 19,323 pixels by at most 2/255 within the error/retry button bounds (318,1593)–(621,1658). The latter is consistent with native button fade timing (inference); no Loading source changes were made. Home 0/7 exact, at most 26 pixels by 1/255. Pre-Level 1/9 exact, at most 50 pixels by 2/255. Earlier production sources/assets are byte-identical. Gameplay RU/EN/RU capture is exact. |
| Standalone browser review | Passed in local Chrome: 11 selectable views, four local links and 360px width without horizontal overflow. Recorded in `QA/StandaloneReviewBrowserChecks.json`; viewer check only, distinct from Unity rendering. |

Initial baseline and first-native capture runs each passed 1/1. The first wider runtime run passed 66/68; two pre-existing test expectations were corrected: score goals now subtract authored starting score, and the intentional watchdog timeout log is explicitly expected. Runtime rule behavior was unchanged. The final combined run confirms both corrected tests pass. A subsequent full focused run after the special match-colour marker also passed 56/56 UI Edit Mode and 104/105 Play Mode, with the same unchanged Splash assertion as its only failure. The independent Splash recheck used identical Splash code; its passing result does not erase either broad-run failure. Default Python lacked Pillow for one attempted asset check; the existing image Python environment ran the completed checks, without installing project packages. XML, run history, owner diagnostics and **130 unedited final capture/metadata files** are retained in [UnityVerification](UnityVerification/TestRunHistory.json), with baseline/recheck evidence and earlier iterations separately preserved. [Reproduction details](UnityVerification/Reproduction.json) and [verification script](QA/RunFinalVerification.py) are included.

## Changed files and preservation

Eight existing paths changed: Gameplay prefab, `LockedDesignImporter`, `Step20ProductionUiFix01SerializedPrefabBuilder`, `ProductionUiFix01Runtime`, `ScreenRouter`, `Match3Board`, and the two runtime test files named above. Added: five native Gameplay UI/art classes, a cell-mechanics presenter, an art importer, two test fixtures, ten PNG resources and their metadata. [Exact file manifest](SourceChangeManifest.json) · [source audit](FinalSourceAudit.json) · [applied diff](AppliedSourceChanges.diff).

Accepted Home, Splash, Loading, World Map, Chapter Map and Pre-Level, FrontEnd art, packages, project settings, Android guards and signing code remain unchanged. No cache/build folders are part of the change set. No secrets or signing material were read or packaged.

**Not performed:** APK/AAB or Android Player build, physical phone installation/render/touch, full-project test suite, performance/memory profiling, release/signing checks, physical display-change testing or exhaustive level solving. Existing APKs predate this page. The two core-test failures and intermittent Splash capture assertion remain visible limitations of the verification.

Next: review Gameplay R1 in Unity/on a phone before visual acceptance; refine action VFX or the existing tutorial/Pause modal as a separate focused task.
