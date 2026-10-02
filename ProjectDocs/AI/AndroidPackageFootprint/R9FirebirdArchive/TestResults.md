# Legacy Home Firebird archive R9 — 2026-10-01

## Scope and implementation

Continued the scheduled single-resource archive after R8 established no current consumer. Moved `home_firebird_generated.png` and its `.meta` from `Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/` to `Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/`. Both files retain their exact task-start bytes. GUID remains `072000aa439cc61468feba1c5d67b108`; PNG SHA256 `1dcbbfed9e0dc640592280cb416d33a78c8aa87cc8e6b460ea574559a90d5b7d`, metadata SHA256 `d3985ab66ff85e86a0de8e93257a04b67c2ac1025d675f6c5e918c1507ce00a1`.

`LockedDesignImporter` adds only this exact archive path to the existing Home import contract. It retains 1024×1536 source dimensions, RGBA32, 11 mip levels, trilinear filtering, Android override at 4096, and unreadable/zero-border settings. Added folder metadata and the permanent five-case `RetiredHomeFirebirdEditModeTests` regression. Its optional JSON evidence has no effect on ordinary test execution.

The existing Home builder and active prefab already leave the old Firebird Image empty/inactive and use `HomeReferenceAligned/source_hero.png`. No runtime owner or prefab replacement is needed. All approved visible art, especially Splash ColorFix06/PrefabFix07, is preserved.

## Verification

Unity 6000.5.9f1 (b57deb96f08d), current installed project configuration. Test runs use isolated `/tmp/lor-firebird-archive-r9/` storage and the real bootstrap/router. Edit Mode uses batch/nographics; Play Mode uses the previously authorized process-only Vulkan override. No project graphics setting is changed.

[Edit Mode evidence](QA/EditAccepted/summary.json): **149 passed, zero failed/skipped**, including five archive cases, Home delivery, Splash build wiring and Android policy/content delivery checks. Old Resources key returns null. The archive has zero consumers among 99 recursively checked serialized roots. Fresh and legacy Home each request 24 other resources; current migration requests zero through the legacy delegate. All three keep valid bindings and the current hero.

[Play Mode evidence](QA/PlayAccepted/summary.json): **26 passed, zero failed/skipped**. Home alignment (13), Home loading preparation (7), Loading alignment (4), and Splash responsive (2). This verifies actual resource loads, native route transitions, no duplicate router infrastructure, measured loading completion, cancellation/retry behavior, Home state/controls/localization, and portrait/safe-area layouts. Total: **175 distinct passing cases**.

Retained 38 actual Unity PNG captures: Home 22 including diagnostic views, Loading 9, Splash 7. Visually inspected the standalone [Home](QA/PlayAccepted/HOME/Home_941x1672_RU.png), [Splash](QA/PlayAccepted/SPLASH/Splash_941x1672.png), and [actual Loading flow](QA/PlayAccepted/LOADING/ActualLoading_941x1672.png). [Flow record](QA/PlayAccepted/LOADING/ActualLoadingFlow.json) records resource-driven progress from 0 to 1 and one transition through Loading to Home. Other Loading layout captures use the existing explicit layout fixture. These are Editor runtime captures, not device screenshots or a before/after pixel comparison.

## Guarded Android artifact

The existing `AndroidDevelopmentBuildCommand.BuildDevelopmentApk` succeeded with its guards unchanged, called through a temporary report-export wrapper. The R7 APK and inherited checksum are retained under `Builds/Verification/v1_17_step01/BeforeFirebirdArchiveR9/`. Only launcher package-task incremental state/output folders were isolated into generated `Library/LegendsOfRusPackagingBackups/BeforeFirebirdArchiveR9/`; player assets, IL2CPP, Gradle configuration and signing state were preserved.

Current artifact: `Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk`.

| Measure | R7 | R9 |
| --- | ---: | ---: |
| APK bytes | 835,948,002 | 830,122,766 |
| Packed object rows | 29,370 | 29,367 |
| ZIP entries | 403 | 403 |

**Measured APK saving: 5,825,236 bytes (0.696842%).** SHA256 `6d0e866931cf46cd5f2386b9ba0c6a67887bf92a0fa05c4d6ba92e156adb934c`. [Artifact](QA/Artifact.json) · [full ZIP comparison](QA/ArtifactComparison.json).

[Packed comparison](QA/PackedComparison.json) excludes the archived Firebird GUID and both R7 progress GUIDs. Exactly the candidate's three rows are removed (8,389,220 reported object bytes); every other path/GUID/type multiplicity is unchanged. Packed bytes are not compressed APK savings or measured memory savings.

400 ZIP payloads are byte-identical, including IL2CPP binary/metadata, all DEX files and manifest. Only `data.unity3d`, `boot.config`, and `unity_app_guid` differ. Asset data shrinks by 5,825,236 bytes; ZIP padding/identifier compression account for the remaining archive bookkeeping. `boot.config` changes only build and player-connection identifiers; all runtime switches remain identical. The initial comparison assumed only build-guid would change, then was corrected after inspecting the normal player-connection-guid change ([verification](QA/BootConfigVerification.json)). No unexplained ZIP gaps or added/removed ZIP paths.

[APK inspection](QA/ApkVerification.json): signature and previous certificate match; identity `com.example.legendsofrus.v117debug`, versionCode 1 / versionName 0.0.0, development/debuggable, ARM64, API 26–36, portrait. Permissions unchanged; CRC, ZIP 16 KiB alignment and load-segment alignment of all six ELF libraries pass. [Nine credential DEX classes/JNI definitions](QA/CredentialDex.json) remain intact; all four [Java source/metadata pins](QA/PinnedPluginInventory.json) match. No credentials or keystores were inspected.

## Final scope and limitations

[Task-start scope](QA/Scope.json): **7,776 baseline project files unchanged, one existing file changed, five added paths and two removed paths**. The additions/removals include the two byte-identical moved files, one archive folder metadata file, and test source/metadata. Only `Assets/LegendsOfRus/Scripts/Editor/LockedDesignImporter.cs` changed in place; [task-only patch](QA/TaskChanges.patch). All runtime scripts, active prefabs, scene/settings/package inputs and other art retain task-start bytes; prior user edits are preserved.

Temporary build source/metadata removed. Only newly generated Addressables `link.xml` and metadata needed [cleanup](QA/GeneratedFileCleanup.json). Test scenes absent at task start were removed by the runner. Raw logs and generated build backups remain outside the authored patch; no cache files are added to the task evidence. QA scripts are adaptations of local R7/R8 harnesses. Python syntax and targeted diff whitespace checks passed. All 175 Unity cases passed on the first archive run; the subsequent importer comment grammar correction is nonfunctional and compiled in the successful build.

No physical Android run, release build, device memory/performance measurement, screenshot pixel-diff acceptance, font identity verification, commit or deployment. Existing font provenance remains unresolved. This is Editor regression and signed development packaging verification.

## Next action

Read-only audit of the remaining legacy Home constructor resources, starting with HomeAligned/home_logo.png and home_hero_firebird.png. Both are still requested by fresh/legacy construction before current composition is applied; trace serialized, shared-page, loading and migration consumers before proposing any removal. Preserve the current hero, approved Splash logo and all visible art. No coin downsizing or blanket compression. Phone work and hosting remain deferred; final effects last. Human Chapter1 pacing, font provenance and Android lifecycle/performance/readability remain open.

