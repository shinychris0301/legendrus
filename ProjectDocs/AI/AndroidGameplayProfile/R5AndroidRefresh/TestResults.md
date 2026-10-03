# Fixed-preset Android refresh R5 — 2026-10-03

Completed the authorized guarded **development APK** refresh. The current build packages the six accepted R4 presets in the existing disposable profile; production/store release and physical gameplay acceptance remain open.

[APK](../../../../Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk): **766,013,502 bytes**, SHA256 `91c38f1a6ae63482d8d9e713dcb8f0f891b9b0a67e8743b30de390ab906dc8a9`. [Checksum](../../../../Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk.sha256) matches. Original R3 APK/sidecar are preserved in [BeforeFixedPresetsRefreshR5](../../../../Builds/Verification/v1_17_step01/BeforeFixedPresetsRefreshR5); rejected initial candidate is retained separately there.

| Fresh check | Outcome | Evidence |
|---|---|---|
| Unity Edit Mode: Android policy/content and fixed-preset factory | 153 distinct passed; zero failed/skipped; launcher exit0 | [XML](QA/Policy/results.xml), [summary](QA/Policy/summary.json) |
| Actual-source compiled Development/Release entry checks with fake JNI/content/preferences | 120 passed; zero failed; synthetic, not Android/release APK execution | [Summary](QA/EntryGuards/summary.json) |
| Final guarded Android build, Unity6000.5.9f1 | Launcher exit0,66.90s wall time; native report Succeeded | [Launcher](QA/BuildVerified/summary.json), [packed report](QA/PackedAssets.json) |
| Public certificate/signature, manifest, ZIP CRC/16KiB, six ELF alignment checks | Passed; original certificate retained | [APK inspection](QA/ApkVerification.json) |
| Packed membership | 29,386→29,387: exactly the existing fixed-factory MonoScript added; all existing path/GUID/type membership preserved | [Comparison](QA/PackedComparison.json) |
| ZIP identity/accounting/permissions | All403paths retained,398identical payloads/five reviewed; zero unindexed bytes; permissions unchanged | [Comparison](QA/ArtifactComparison.json), [review](QA/PayloadReview.json) |
| Native engine and credential plugin | libunity.so binary/dynamic bindings unchanged; nine DEX/JNI classes and four Java/meta pins verified | [Engine](QA/NativeEngineComparison.json), [DEX](QA/CredentialDex.json) |
| Source/config scope | All7,839 inputs unchanged, zero added/deleted; helper/new generated linker files removed; tracked compiler fingerprint restored | [Scope](QA/Scope.json), [cache cleanup](QA/TrackedBuildCacheCleanup.json) |
| Canonical offline preflight boundaries | 45 passed; synthetic files/mocked ADB only | [Boundary checks](QA/PreflightBoundaryChecks.json) |
| Actual default offline artifact check | Passed; no device enumeration, install, launch or app-data access | [Offline result](QA/OfflinePreflight.json) |

[Build reconciliation](QA/BuildValidation.json) and [final handoff validation](QA/FinalValidation.json) tie these records to the actual APK and original archive. Counts describe different kinds of checks; they are not a combined Unity test total. Prior R4 Play Mode/cold-process tests and R7 broad regression remain historical; neither suite was rerun here.

The final APK remains `com.example.legendsofrus.v117debug`, versionName0.0.0/versionCode1, ARM64 IL2CPP, minAPI26/targetAPI36, portrait/debuggable. Profile13/Dialogue2/Gameplay24/four stock layouts/Victory12 and all five inspected page object counts/packed bytes remain unchanged. No art, approved Splash, prefabs, builders, GUIDs, font assignments, settings, packages, backend or Android approval guards changed.

Exact selectors packaged: `stock-999`, `stock-1000`, `stock-9999`, `stock-max`, `story-early`, `story-finale`. Six pure Editor candidates were checked without loading player state: stocks999/1000/9999/Int32.MaxValue; early8completed rounds/3acknowledged scenes/selected9; finale500/58/selected500; energy4 throughout. The factory type and exact literal strings are present in IL2CPP metadata. Its installed SDK defines metadata version107 literals as uint32 offsets with an end sentinel; [format verification](QA/MetadataFormatVerification.json) records the distinction from older record layouts. Marker presence is packaging evidence, not intent activation/save isolation on Android.

The first incremental build produced an APK, but the audit helper rejected an empty packed inventory. Its isolated build context also selected a different default debug certificate. [Initial launcher failure](QA/Build/summary.json), [rejected artifact](QA/InitialArtifact.json) and [public certificate rejection](QA/InitialSignatureRejection.json) remain recorded. The candidate was not accepted. The final build used the existing Editor signing context and a fresh player asset pack after reversible relocation of task-owned caches under `/tmp`; no existing keys/passwords were read or copied and signing settings/guards were retained. [Cache locations](QA/FreshPackingCachePreservation.json) are temporary recovery material; the original APK archive above is the durable artifact backup. Raw Unity logs remain at the paths recorded by launcher summaries under `/tmp`.

No new Play Mode run/captures/visual acceptance, phone testing, normal personal journal access, hosted-service activation, production signing/release, RuStore upload, Git commit or push was performed. Unity's build shutdown can stop its own ADB server; the QA tools made zero real ADB calls and did not contact a phone. All18physical gameplay rows remain pending. `stock-max` permits display/consumption only because ordinary reward additions lack overflow protection; this task does not alter economy rules.

Next: collect human rounds201–210 outcomes and feedback using the existing matrix, then201–250/491–500 and close demonstrated core/content gaps. Phone/hosting work remains deferred; final effects stay last. Distinct story-location art remains a separately scoped implementation/design decision.
