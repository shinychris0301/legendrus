# Home dormant artwork cleanup R17 — 2026-10-01

The user approved the scheduled cleanup. Removed nine dormant Home resource loads and assignments, cleared exactly nine disabled Image sprite fields in the active Home prefab, and archived each PNG/meta pair outside Resources. Current Home controls, source artwork, localizable labels, loading services, routes and revisions are preserved. This completes the R14 dormant Home removal candidates; nine live/shared resources remain.

ProductionHomeRuntime now requests nine retained sprites in fresh/legacy construction and zero in current migration. Retired nav/medallion/quest/foliage placeholders start empty; gift geometry is selected by the existing button identity rather than a retired sprite name. Current runtime and source owners clear reinjected dormant sprites after live targets are assigned. Control targets and enabled field Images are protected. The historical backdrop and lighting diagnostics validate current artwork and inactive historical overlays/effects. Lighting components/materials and serialized objects are retained.

## Resources and preservation

| Archived HomeAligned resource | Dimensions | Preserved GUID |
| --- | --- | --- |
| nav_map_generated | 1254 × 1254 | `c799ce05d21b30886a6fa33a9c3e4c33` |
| nav_shop_generated | 1254 × 1254 | `4bbb7d70ab8e8c29385b536e7e8dbf2f` |
| nav_events_generated | 1254 × 1254 | `b22f4fdf95dddd63f8c32fab7a753da0` |
| nav_characters_generated | 1254 × 1254 | `69135cdae1222fa598c05c4ab1539397` |
| mail_medallion_generated | 1254 × 1254 | `23aee3e96f6777c45a4cae8a64c33c4c` |
| reward_medallion_generated | 1254 × 1254 | `717db484c9b631d909a47c6df0c87daf` |
| quest_reward_frame | 1095 × 1437 | `d76c3dc6c6b59ff2ca8b7698b698ec03` |
| quest_reward_gem | 1230 × 1278 | `2cb9251bbe55ba200b4383179b701814` |
| home_map_foliage | 1024 × 1536 | `94bad924675b43dcadb2595547a51117` |

All nine sprites retain RGBA32, 11 mip levels, trilinear filtering, Read/Write disabled and the existing Android RGBA32/4096 override. LockedDesignImporter explicitly owns these nine exact archive paths; no blanket archive-folder rule. [PNG/meta byte proof](QA/ArchiveByteVerification.json) and [Unity import/dependency evidence](QA/EditFinal/nav_map_generated.json) are retained. Each archive has zero consumers across 99 recursive serialized dependency roots. Current source/GUID tracing found one historical Home Image per resource before the edit, no other GUID consumers: [inventory](QA/CandidatesBefore.json).

The nine retained HomeAligned resources are home_logo, avatar_portrait, avatar_ring, resource_capsule, resource_plus, settings_medallion, coin_icon, gems_icon and energy_icon. Login logo, Gameplay portrait and PreLevel currency dependency checks pass. Current clean_environment/source_hero/source_logo/source_header, navigation, mail/gift/footer and quest artwork remain live.

Home keeps all 480 serialized object headers. Its only byte changes are the nine sprite fields; other references, IDs, geometry and revisions are identical. All other 71 prefabs are byte-identical: [proof](QA/PrefabPreservation.json).

## Unity verification

Unity 6000.5.9f1 (b57deb96f08d). Isolated test storage/configuration, existing bootstrap/router/Canvas and process-only Vulkan used. Unity launches used reviewed permissions for local Package Manager IPC.

- Baseline: four native capture tests passed before implementation; all 7,790 task-start project inputs verified unchanged before editing. These four methods repeat in final coverage and are not counted twice.
- Final Edit Mode: **211 passed, zero failed/skipped**, including 130 Android policy/content checks and 20 new permanent cleanup cases. Archives/GUIDs/imports/dependencies, fresh/legacy/current loads 9/9/0, repeated stale-sprite clearing with object/component/live-target preservation, live-target protection, historical diagnostics, fresh/installed cloned-header pages and retained graphs verified. Updated two prior fixtures' expected load count 18→9, removed their obsolete retained quest_reward_frame expectation, and moved the missing-unrelated-resource probe to retained resource_capsule before running. No Unity compile/test failure or production correction was required.
- Final Play Mode: **44 passed, zero failed/skipped**. Home 13, WorldMap 7, ChapterMap 5, PreLevel 6, Home preparation 7, Loading 4, Splash 2. Native controls/navigation, inventory, RU/EN, portraits, measured progress/retry/cancellation covered. **255 distinct final passes**; no new broad R5/full-game or device acceptance claim.

[Edit results](QA/EditFinal/summary.json) · [Play results](QA/PlayFinal/summary.json) · [Coverage](QA/TestCoverage.json). A local preservation checker initially filtered only 36 /Pages/ prefabs while expecting all 72; corrected that audit filter to include all prefab files. Production and Unity tests were unaffected.

Retained **100 final actual Unity PNG captures**, compared 35 task-start pairs. All 14 Home, 5 ChapterMap, 7 PreLevel and 7 WorldMap pairs are pixel-identical (33/35). Two tall WorldMap pairs differ at 9/17 pixels, maximum channel delta1/255, identical layout JSON; the same recorded pattern as R15/R16. These two pairs are not claimed identical. [Pixel comparison](QA/CaptureComparison.json) · [Decoded differences](QA/MapPixelComparison.json).

Standalone actual Unity pages inspected: [Home](QA/PlayFinal/HOME/Home_941x1672_RU.png), [WorldMap](QA/PlayFinal/WORLDMAP/WorldMap_941x1672_RU.png), [PreLevel](QA/PlayFinal/PRELEVEL/PreLevel_941x1672_RU.png). No offline/generated preview or physical-phone acceptance is claimed.

## Guarded Android package

Existing AndroidDevelopmentBuildCommand ran with its normal approval/fingerprint/options/plugin guards. Unity process exited 0; actual build report Succeeded; 105.16s invocation. Prior R16 APK and inherited sidecar preserved under BeforeHomeDormantArtworkCleanupR17; its SHA verified before archival. Only three generated launcher packaging directories were isolated, leaving signing/settings/player asset build paths intact. Temporary audit helper/meta removed normally; no wrapper recovery was needed.

| Measurement | R16 | R17 |
| --- | ---: | ---: |
| APK bytes | 805,137,130 | 765,750,206 |
| Packed rows | 29,349 | 29,322 |
| ZIP paths | 403 | 403 |

**Measured saving: 39,386,924 bytes (4.892%).** Exactly 27 packed rows for the nine candidates removed; all other path/GUID/type membership unchanged. Removed packed-object bytes 75,465,648 are a separate metric, not compressed APK or device-memory savings. All 18 retired artwork GUIDs absent. [Packed comparison](QA/PackedComparison.json) · [APK comparison](QA/ArtifactComparison.json).

APK: `Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk`. SHA256 `c83940412715c9ebed87eb3e47b12bdd971cb8aae8a48735f015dd9ac9279fbe`. Current SHA sidecar matches.

Signing/certificate, manifest identity com.example.legendsofrus.v117debug version 1/0.0.0, ARM64 API 26→36, portrait/debug, unchanged permissions, ZIP CRC, zero unindexed bytes, 16 KiB ZIP and six ELF alignment checks passed. All 403 ZIP names retained; 398 payloads byte-identical. Five expected asset/code/build-ID payload changes checked; boot settings differ only in build/player-connection identifiers. Nine credential DEX classes/JNI methods/operation fields and four Java source/meta pins pass. [APK checks](QA/ApkVerification.json) · [Payload review](QA/PayloadReview.json) · [DEX evidence](QA/CredentialDex.json).

## Scope and next action

Of 7,790 task-start Assets/Packages/ProjectSettings inputs, **7,765 unchanged / 7 changed / 20 added / 18 removed**. Changes: active Home prefab, three Home runtime/view/composition owners, importer and two existing regressions. Additions/removals include 18 byte-identical moved paths and a new permanent test source/meta. [Exact scope](QA/Scope.json) · [Task-only patch](QA/TaskChanges.patch). Only two newly generated linker files cleaned; prior user edits, packages/settings, fonts, approved Splash and Android guards preserved. Caches/logs/backups are outside the authored patch. No commit, deployment or phone execution. Raw local logs: `/tmp/lor-home-dormant-artwork-cleanup-r17/`.

Return to the core UI acceptance schedule with a bounded native RU/EN readability and input audit of Gameplay, Victory and ChapterComplete at small and tall portrait sizes, including long values and chapter-final states. Reuse the current router/services and preserve rewards, saves, artwork, fonts and existing acceptance. Measure actual glyph clipping, touch targets and recovery/navigation before proposing any fixes. Retain all nine live/shared Home resources. Physical-phone execution and hosting remain deferred; human Chapter 1 pacing, font provenance and Android lifecycle/performance remain open; final gameplay effects stay last.
