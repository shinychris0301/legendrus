# Dormant progress cleanup R7 — 2026-10-01

Removed the two retired progress textures from runtime dependencies without changing the sampled visible UI. The guarded development APK is now **835,948,002 bytes**, down **2,271,232 bytes (0.271%)** from R2's 838,219,234 bytes. This is measured net APK reduction, including R6/R7 code changes, not the earlier raw mip-payload estimate or device-memory savings.

## Implementation

- Cleared exactly 11 `m_Sprite` assignments in six active prefabs: Home3, Gameplay4, WorldMap/ChapterMap/PreLevel/Fail1 each. Every serialized object ID, all other fields and all revisions remain unchanged. [Exact identity check](QA/PrefabIdentity.json).
- Home's fresh/legacy constructors no longer request either progress resource. Its current composition and existing-view path clear stale assignments; the legacy foreground validator now retains the containers without requiring retired art.
- Shared header presentation clears its legacy profile frame. WorldMap's owning builder also clears the separately retained inactive old HUD. Gameplay creates the old Images without sprites and clears stale assignments during in-place styling. Its Image components and `fillAmount` state remain; score/goal update logic is unchanged.
- Moved both PNGs **with their metadata** from `Resources/ProductionUI/HomeAligned/` to `Art/ReferenceOnly/RetiredProgress/`. PNG and metadata bytes/GUIDs are identical to task start. Added an exact-two-path importer rule to preserve 2172×724 dimensions, RGBA32, mipmaps and existing settings during future imports. No global importer rule changed.
- Added 15 permanent Edit Mode cases for six serialized/fresh paths, six stale-reference repairs, old Home migration and two archive import contracts. Updated Gameplay's existing Editor contract to require null sprites only on its four retired Images. Extended runtime layout checks to compare hidden goal/star fill values against actual board goals/score and the visible meter.

[Scope](QA/Scope.json) and [task-start patch](QA/TaskChanges.patch): 16 existing files changed (six prefabs, eight owning/runtime/importer sources, two test sources); 7,756 baseline files unchanged. Seven added/four removed paths represent four byte-identical moved files, one archive-folder metadata file and one new test source/meta pair. No artwork was deleted or recompressed. Prior user edits were compared against task-start snapshots and preserved.

## Tests and visual evidence

Unity **6000.5.9f1**; isolated test storage and existing bootstrap/router. Process-only Vulkan for Play Mode, batch/nographics for Edit Mode. No project graphics/package/backend/build-guard change.

| Stage | Passed | Failed | Skipped |
|---|---:|---:|---:|
| Construction/migration before archival | 19 | 0 | 0 |
| Pixel comparison + Gameplay before archival | 11 | 0 | 0 |
| Initial archived-state Edit Mode | 162 | 1 | 0 |
| Final archived-state Edit Mode | **163** | **0** | **0** |
| Final archived-state Play Mode | **55** | **0** | **0** |

Final distinct acceptance is **218 passing cases**, excluding earlier overlapping runs. The one intermediate failure was an existing test's blanket requirement that every Gameplay Image have a sprite. Replaced it with explicit assertions that the four retired Images are empty and inactive, retaining the requirement for every other Image. [Final Editor results](QA/EditAccepted/summary.json), [runtime results](QA/PlayFinal/summary.json); full NUnit XML is retained with each run.

The 163 Editor cases include 130 Android policy/content-delivery tests. Runtime coverage includes all six affected pages, Home loading, native routing, actual goal/score values, score-meter agreement, resolved board moves, all 500 authored board presentations, energy/booster behavior, RU/EN and portrait/safe-area checks.

The temporary rendering fixture compared each native page **in the same frame** with cleared sprites versus the original sprites temporarily reassigned to their retained hidden Images. Six pages × two languages × three display/safe-area configurations = **36 comparisons; zero changed pixels**, both before and after archival. Final run inspected 66 retired-Image observations. This isolates the sprite-assignment effect without comparing randomized boards across launches. The temporary fixture was removed; its [source](Sources/ProgressArtworkRenderPlayModeTests.cs) and [final measurements](QA/PlayFinal/RuntimeMeasurements.json) remain as evidence.

Final runtime fixtures retained 138 actual Unity PNG captures. Visually inspected Home and Gameplay reference pages from the pre-archive run and the [final settled Gameplay capture after 12 test moves](QA/PlayFinal/GAMEPLAY/Gameplay_After12Moves_Settled_438x780_RU.png). These are Editor renders and test states, not phone screenshots or new human art acceptance. PNG/meta preservation makes new alpha extraction/compression acceptance unnecessary; no visible art was modified.

## Android artifact

Used the existing `AndroidDevelopmentBuildCommand.BuildDevelopmentApk` with all normal guards. A temporary Editor wrapper only invoked that command and exported the resulting packed-asset inventory. Prior APK and checksum retained under `Builds/Verification/v1_17_step01/BeforeProgressCleanupR7/`; only three generated launcher packaging directories were isolated in restorable Library backups. Build succeeded in 100.36 seconds.

- Current artifact: `Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk`
- Size: **835,948,002 bytes**; SHA256 **`2b7caabb7612d1c762a55bf7190d757f33a9d1a10f8e03e120979d9fd76f0aac`**.
- [Packed inventory](QA/PackedAssets.json): 29,370 object rows; both retired GUIDs and old/archive paths absent. Source scan of 5,500 asset text files finds target GUIDs only in archived metadata and explicit regression assertions, not runtime serialized references.
- [APK comparison](QA/ArtifactComparison.json): all 403 ZIP paths retained; 398 payloads byte-identical. Five expected payloads changed: Unity asset data, IL2CPP binary/metadata and two build-ID files. Asset-data payload alone shrank by 2,272,000 bytes. No unindexed ZIP records remain.
- [Verification](QA/ApkVerification.json): signature/certificate, identity/version/permissions, portrait/ARM64/API settings, ZIP CRC, 16 KiB ZIP alignment and all six ELF load alignments passed. Nine credential DEX classes/JNI members and approved Java pins remain intact. [Reviewed delta](QA/ReviewedDelta.json).

Temporary rendering/build helper files were removed. Only newly generated linker files were deleted; any pre-existing generated build metadata was preserved/restored. [Cleanup](QA/GeneratedFileCleanup.json). Raw logs remain under `/tmp/lor-progress-cleanup-r7/`; caches are not authored changes.

No install/device run, release build, backend activation, commit or deployment. Physical memory/loading/frame-time acceptance and human round31–500 pacing remain pending. Exact font provenance remains unverified; approved Splash/Title and original coin imports are unchanged.

## Next action

Use the new packed inventory for a read-only usage audit of the remaining large legacy resources, starting with `HomeAligned/home_firebird_generated.png` (about 8.39 MB in the packed-object report). Establish all serialized and dynamic builder uses before another narrow cleanup. Packed-object sizes are not compressed APK savings. Do not resume visible-coin downsizing or broadly compress approved art. Phone execution and hosting remain deferred; final gameplay effects last.
