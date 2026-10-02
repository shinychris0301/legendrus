# Home temporary hero dependency cleanup and archive R11 — 2026-10-01

## Implemented behavior

Fresh Home construction and legacy migration now complete with `ProductionUI/HomeAligned/home_hero_firebird` absent. Removed the old hero from `ProductionHomeRuntime.TextureNames` and the two intermediate assignments. The existing `ApprovedIvan` Image is constructed/positioned before the same synchronous current composition assigns `HomeReferenceAligned/source_hero`; no replacement hierarchy or startup system is introduced.

The historical `HasReferenceForegroundBindings` diagnostic now delegates to the current `HasRequiredBindings` contract. Removed its unused private portrait helper; current binding validation is unchanged and still rejects incorrect hero artwork. The R10 audit already established that the obsolete diagnostic had no C# caller.

Moved only `home_hero_firebird.png` and its `.meta` to `Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/`. Both retain exact bytes, GUID `7b33517046ee539d4a86ecfd63012ea5`, and import settings via one exact-path addition in `LockedDesignImporter`. PNG SHA256 `f87834da25125c7e07dedd06fe284cd5a5f3bfb538a29a1eabbbda44006fa125`; metadata SHA256 `272f4ab00cd390189e3406b06f69bac1509e8b63914d145fb9403bd2baceb62d`. Existing RGBA32, 1254×1254, 11 mip levels, trilinear and Android 4096 override retained.

Active Home already binds the current hero; every prefab retains task-start bytes, object IDs and revision. Its owner now constructs that same approved result without the superseded asset. Login's shared `home_logo` and all current visible art remain installed. Added nine permanent `RetiredHomeHeroEditModeTests` cases.

## Unity checks

Unity **6000.5.9f1 (b57deb96f08d)**. Isolated `/tmp/lor-home-hero-archive-r11/` storage; existing bootstrap/router. Edit Mode batch/nographics; Play Mode uses the previously authorized process-only Vulkan override.

| Run | Passed | Failed/skipped |
| --- | ---: | --- |
| [Edit Mode](QA/EditAccepted/summary.json) | 152 | 0/0 |
| [Play Mode](QA/PlayAccepted/summary.json) | 32 | 0/0 |
| Distinct cases | **184** | **0/0** |

New nine-case regression covers archive imports, recursive dependency exclusion, Home fresh/legacy/current migration, unchanged installed components under repeated builds, current diagnostic rejection of incorrect hero art, and fresh/installed Login logo preservation. Old Resources key loads null; archive has zero consumers among 99 serialized roots. Fresh and legacy paths now request 23 other legacy sprites instead of 24; current migration requests zero through the legacy delegate. All paths end at valid current `source_hero` art.

Also ran prior Firebird archive checks, Home delivery, Login Edit Mode and Android policy/content delivery fixtures. Play Mode: Home alignment13, Home loading preparation7, Loading alignment4, Splash responsive2, Login alignment6. Covers actual resource preparation, Splash → Loading → Home, monotonic measured progress, completion/retry/cancellation, Home state/navigation/localization/layout, and Login actions/legal panels/guest return.

No unexpected Unity failures or skipped cases. R10's earlier expected denial exceptions established the baseline defect; those historical checks are not counted as rerun tests here.

## Actual captures and comparison

Retained **64 actual Unity PNGs**: Home22, Splash7, Loading9, Login26. Inspected standalone [Home](QA/PlayAccepted/HOME/Home_941x1672_RU.png) and [Login](QA/PlayAccepted/LOGIN/Login_941x1672_RU.png), plus both Loading error captures. These are Editor captures, not physical-device acceptance.

[Comparison against R9](QA/CaptureComparison.json): **37 of 38 paired captures proven pixel-identical** (all22Home, all7Splash and8Loading, including actual loading and normal layout views). Equality is established by matching PNG settings and inflated filtered scanlines, which implies matching decoded pixels. Login's26new captures have no paired R9 baseline.

The sole unmatched [Loading error capture](QA/ErrorCaptureComparison.json) was decoded for measurement:12,156changed pixels, maximum channel difference3/255, inclusive bounds x323–613/y1483–1569, entirely within the Retry button. All geometry JSON is identical; the rest of the image pixels match. The existing fixture captures immediately after error activation and does not settle that Button's default color transition. Frame timing is a plausible explanation (inference), not proven by these captures. No complete error-image pixel-identity claim. Loading source, prefab and sprites retain baseline bytes, and its retry behavior tests pass.

The optional comparison initially required unavailable Pillow; it was replaced with a dependency-free PNG check and a standard-library decoder. No production/package changes were made for that tooling correction.

## Guarded Android build and measured saving

The existing `AndroidDevelopmentBuildCommand.BuildDevelopmentApk` succeeded with normal guards unchanged. Temporary `HomeHeroArchiveBuildAudit` exported the actual report and was removed afterward. Prior R9 APK and checksum retained under `Builds/Verification/v1_17_step01/BeforeHomeHeroArchiveR11/`. Only generated launcher package-task outputs/incremental state were isolated; project configuration and signing state retained.

Current APK: `Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk`.

| Measure | R9 | R11 |
| --- | ---: | ---: |
| APK bytes | 830,122,766 | 824,676,966 |
| Packed object rows | 29,367 | 29,364 |
| ZIP entries | 403 | 403 |

**Measured reduction: 5,445,800 bytes (0.656023%).** SHA256 `64d521032ac0414a45aafa59225aa8ab3d96a1cafa2c4cf12493c687450915e5`. [Artifact](QA/Artifact.json) · [ZIP comparison](QA/ArtifactComparison.json).

[Packed membership](QA/PackedComparison.json): only the hero's three rows removed, all other path/GUID/type multiplicity unchanged. All four retired GUIDs excluded, including prior Firebird/progress archives. Removed8,384,680reported object bytes are not compressed APK or measured memory savings.

398ZIP payloads are byte-identical. Five changed: `data.unity3d`, `libil2cpp.so`, `global-metadata.dat`, `boot.config`, `unity_app_guid`, consistent with asset removal and the two runtime owner changes. All DEX files and manifest remain identical. Asset data shrinks5,442,938bytes; code/metadata/compression and archive bookkeeping account for the remaining net reduction. [Boot configuration](QA/BootConfigVerification.json) changes only build and player-connection identifiers; runtime switches unchanged. No added/removed ZIP paths or unindexed stale records.

[APK verification](QA/ApkVerification.json): same certificate/signature verified; `com.example.legendsofrus.v117debug`, versionCode1/versionName0.0.0, development/debuggable, ARM64, API26–36, portrait. Permissions, CRC, ZIP16KiB alignment and all six ELF library load alignments pass. [Nine credential DEX classes/JNI definitions](QA/CredentialDex.json) and all four [Java source/metadata pins](QA/PinnedPluginInventory.json) retained. No credential data/signing files inspected.

## Exact task scope

[Task-start comparison](QA/Scope.json): **7,777 baseline project files unchanged; three existing files changed; four paths added/two removed.** The additions/removals comprise two byte-identical moved files and new regression source/metadata; the archive folder already existed. Existing changes: Home runtime, Home view diagnostic, exact importer rule. [Reviewable task patch](QA/TaskChanges.patch). Every active prefab, scene, package/settings input and other art retains task-start bytes. Prior local edits preserved.

Temporary build helper/meta removed. Only newly generated Addressables linker source/metadata required [cleanup](QA/GeneratedFileCleanup.json); new test scenes removed by the existing runner. Raw logs/generated backups are outside the authored patch, and Python caches removed from evidence. Python syntax, JSON/link consistency and targeted whitespace checks passed.

No physical-device execution, release build, device memory/performance measurement, font identity verification, commit or deployment. Native captures verify this Editor run; human readability/pacing acceptance remains open.

## Next action

Read-only audit of the remaining temporary Home constructor artwork, starting with HomeAligned/home_environment_generated.png and home_play_button.png. Trace serialized/shared-page, fresh/legacy, loading and validation consumers from the new packed report before proposing cleanup. Preserve current clean_environment/source_play artwork, the Login logo, currency imports and approved Splash. Phone execution/hosting remain deferred; final effects last. Human Chapter1 pacing, font provenance and Android lifecycle/performance/readability remain open.
