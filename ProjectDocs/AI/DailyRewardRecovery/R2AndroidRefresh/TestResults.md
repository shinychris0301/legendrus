# Daily reward Android refresh R2 — 2026-10-03

The guarded development APK now packages the verified daily-reward overflow rejection from [R1](../R1/TestResults.md). Its original signing certificate and all six fixed QA presets are retained. This task refreshes packaging; human and Android device acceptance remain pending.

## Artifact and preservation

Current [development APK](../../../../Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk): **766,014,334 bytes**, SHA256 `9f72045fe3a2e68114be24d687dfb2a51c07c48d39ec58aa4afbafe9a5744654`. The [checksum sidecar](../../../../Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk.sha256) matches. Package `com.example.legendsofrus.v117debug`, version `0.0.0` / code 1, debuggable ARM64, minimum API 26 and target API 36 remain unchanged. The verification directory's `v1_17_step01` name does not override the inspected manifest version.

The [original R5 APK](../../../../Builds/Verification/v1_17_step01/BeforeDailyRewardRefreshR2/LegendsOfRus-v1.17-step01-development.apk) and [inherited sidecar](../../../../Builds/Verification/v1_17_step01/BeforeDailyRewardRefreshR2/LegendsOfRus-v1.17-step01-development.apk.sha256.inherited) are preserved: 766,013,502 bytes, SHA256 `91c38f1a6ae63482d8d9e713dcb8f0f891b9b0a67e8743b30de390ab906dc8a9`. Historical R5 reports retain their original meaning; the current canonical offline preflight and usage/checklist now pin R2.

## Executed checks

| Check | Actual result | Evidence |
| --- | --- | --- |
| Fresh Unity Edit Mode policy, content, preset, daily-reward and receipt checks | 160 distinct passes, zero failures/skips; launcher exit 0 | [XML](QA/Policy/results.xml), [launcher](QA/Policy/summary.json) |
| Existing guarded development build command | Unity exit 0; native BuildReport Succeeded; 96.076 seconds launcher wall time | [Launcher](QA/Build/summary.json), [native inventory](QA/PackedAssets.json) |
| Signature, certificate, manifest, CRC and alignment | Original certificate retained; identity/permissions unchanged; ZIP 16 KiB alignment and six ELF libraries pass | [APK inspection](QA/ApkVerification.json), [signature](QA/ApkSignature.txt), [ZIP alignment](QA/ZipAlignment.txt) |
| Exact ZIP comparison | Same 403 paths; 400 identical payloads; three reviewed changes | [Comparison](QA/ArtifactComparison.json), [payload review](QA/PayloadReview.json) |
| Packed asset comparison | All 29,387 path/GUID/type rows unchanged; seven native page object counts and packed byte totals unchanged | [Packed comparison](QA/PackedComparison.json) |
| Credential/native bindings | Nine compiled credential DEX definitions and four Java/meta pins verified; native engine binary identical | [DEX](QA/CredentialDex.json), [pins](QA/PinnedPluginInventory.json), [engine](QA/NativeEngineComparison.json) |
| Native daily claim inclusion | Generated OverflowException catch and rejected-candidate return present; exact compiled method bytes match packaged native library | [Native proof](QA/NativeClaimVerification.json) |
| Offline preflight boundaries | 52 distinct synthetic/mocked passes, zero real ADB calls | [Boundary results](QA/PreflightBoundaryChecks.json) |
| Actual default offline preflight | Current APK pin and compiled inclusion verified; device execution deferred | [Offline result](QA/OfflinePreflight.json) |
| Source preservation and reconciliation | All 7,839 task-start production inputs unchanged; no additions/deletions | [Scope](QA/Scope.json), [build reconciliation](QA/BuildValidation.json), [final handoff validation](QA/FinalValidation.json) |

Unity was the installed 6000.5.9f1. The prior service task's **7 Edit Mode + 14 Play Mode = 21 distinct passes** are historical evidence, not rerun counts for this build task. There was no new Play Mode, fake-JNI guard suite or physical-device execution in R2.

## Compiled change and retained UI

The old generated claim method had no overflow catch. The rebuilt method catches `OverflowException` and rejects the prepared candidate before `SaveService.TrySave`; its native body changed from 388 to 496 bytes. The exact method bytes in the stripped APK library match the unstripped build output, and the entire packaged library matches the Gradle native input. The packaged `libil2cpp.so` SHA256 is `6055d1997f31313acb5014142ee0e5ce028b7241013f949bb47ea169593ceaf2`. This proves inclusion of the compiled fix, not its execution on Android.

Only `lib/arm64-v8a/libil2cpp.so`, the two build identifiers in `assets/bin/Data/boot.config`, and `assets/bin/Data/unity_app_guid` changed. Metadata and the complete `data.unity3d` player pack are byte-identical. The APK grew 832 bytes; no size reduction is claimed.

Profile revision 13, Dialogue 2, Gameplay 24 with four stock layouts, Victory 12, DailyReward 5 and reward receipt 2 retain their existing references and owners. Packed comparisons cover Profile, Dialogue, Gameplay, Victory, ChapterComplete, DailyReward and RewardClaim. Approved Splash/Title materials, all existing GUIDs/metas, other pages, source fonts and settings remain untouched.

Selectors `stock-999`, `stock-1000`, `stock-9999`, `stock-max`, `story-early` and `story-finale` remain present. Their six pure Editor candidate checks and compiled metadata markers do not establish Android activation. Ordinary level/booster reward credit overflow remains a separate issue: `stock-max` is still restricted to display and consumption checks.

## Change scope and next action

The service/test edits from R1 were already present at this task's baseline and remain intact. R2 updates eight existing handoff, schedule, checklist, usage and offline-tool files plus its own evidence/tools. The temporary Editor audit helper and metadata were removed; four generated Addressables artifacts were restored. One previously clean tracked compiler fingerprint was restored after the build. No additional production source, prefab, art, package, backend or guard change was made.

Three generated packing caches were reversibly moved to `/tmp/lor-daily-reward-android-refresh-r2/BeforeFreshPacking/` while Unity was idle to satisfy the existing disk guard and obtain a fresh packed inventory. [Cache preservation](QA/FreshPackingCachePreservation.json), [incremental packaging isolation](QA/PackagingIsolation.json), [generated-file cleanup](QA/GeneratedFileCleanup.json) and [tracked cache cleanup](QA/TrackedBuildCacheCleanup.json) record the actions. These temporary cache copies differ from the durable original APK archive. Existing signing context and IL2CPP cache were retained; no private signing material or passwords were inspected.

No normal personal journal read, phone enumeration/install/launch, app-data access, hosted activation, store submission, production build or Git commit/push occurred. No performance, visual acceptance, profitability or human difficulty conclusion is inferred from these checks. All 18 physical checklist rows remain pending; hosting remains deferred and effects stay last.

Next: collect genuine rounds 201–210 attempts, ratings and block observations using the existing Active recorder and [recording intake](../../ChapterOneHumanAcceptance/R2RecordingReview/README.md). Retain losses/retries and review actual feedback before balance changes. Continue subsequent story blocks after review; any separate core credit-overflow change requires a reproduced defect and preservation of existing reward/save semantics.
