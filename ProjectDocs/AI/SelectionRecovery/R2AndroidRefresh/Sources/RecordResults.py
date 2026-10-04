"""Record inspected selection APK and preserve existing handoff/acceptance history."""
from pathlib import Path
import difflib,json,shutil
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
read=lambda n:json.loads((QA/n).read_text())
a=read('Artifact.json');v=read('BuildValidation.json');b=read('PreflightBoundaryChecks.json');assert v['result']==b['result']=='Passed'
h=a['sha256'];size=f"{a['bytes']:,}";checks=b['passed'];seconds=read('Build/summary.json')['durationSeconds'];native=read('AfterNative.json');assert native['atomicSelectionNormalizationRecoveryPackaged']
report=f'''# Level selection recovery Android refresh R2 — complete

[Build reconciliation](QA/BuildValidation.json) · [Compiled owners/callbacks](QA/AfterNative.json) · [Source results](../R1/TestResults.md) · [Final validation](QA/FinalValidation.json) · [Handoff/tool patch](Sources/HandoffChanges.patch).

Current development APK: `{a['path']}`, **{size} bytes**, SHA256 `{h}`. It now packages atomic selection and selection normalization, persistence-lock rejection and acknowledged map/Play/start routing. Failed node/Play saving preserves the selected level and exact map, using the existing localized error modal; Close/Back dismisses it and rejected controls remain available. Normalization is acknowledged before story/PreLevel routing or energy spending. Prior tutorial/narrative/energy/reward/spending/durable booster-refund fixes remain packaged. No visual, localization, progression, combined attempt/energy transaction or unfinished-board checkpoint changes.

Previous TutorialR2 APK:766,026,550bytes/SHA256`b699646db5f7ac9effc403c768eb19200b5eb3b58f14858b331f5edb80aa5fb4`, preserved with its inherited checksum under `Builds/Verification/v1_17_step01/BeforeSelectionRefreshR2/`.

Unity6000.5.9f1: **160 fresh distinct Edit Mode build-policy/content/preset checks passed**, zero failures/skips. Existing guarded development build succeeded, launcher exit0, BuildReport Succeeded, {seconds:.2f}s. Prior same-source R1 **350 distinct passes (251Play/99Edit)** remain runtime/service evidence; no new Play Mode run in this build task. **{checks} synthetic/mocked offline preflight boundaries passed**; actual default preflight verifies this artifact and code proof with zero ADB/device/player-data calls.

Before rebuild47generated/native owners matched the old APK; after rebuild49owners and five callback regions match the packaged stripped IL2CPP library/Gradle input. Selection and TryEnsureSelectedLevelIsUnlocked call atomic commit without Persist and return before cloning when persistence is locked. The candidate normalizer retains existing clamping/deduplication rules, and the void wrapper delegates to the acknowledging API. StartLevel has a rejected-normalization return/error before energy spending; OpenSelectedLevel rejects before story routing. The compiled map node callback checks selection success, invokes its error action and returns before opening/committing navigation; Play assigns navigation commitment from its Func<bool> result. The router supplies that acknowledged callback and a compiled existing-error action. Two tutorial callback guards and all earlier recovery owners remain verified. These are compiled inclusion checks, separate from physical execution.

Signature/original certificate/package/version0.0.0/code1/ARM64/min26/target36/portrait/debug and permissions retained. ZIP CRC/exact accounting/no unindexed gaps/16KiB alignment and six ELF load alignments pass. Nine credential DEX/JNI classes/four Java-meta pins retained. All403ZIP paths remain:399payloads identical/four reviewed changes (IL2CPP code/metadata and two build identifiers). APK is3,300bytes smaller; native code grows1,120uncompressed bytes while compression changes. The engine library/bindings and complete player data pack are byte-identical, without asset reduction. All29,387packed path/GUID/type rows and seven page object sizes unchanged. Profile13/Dialogue2/Gameplay24/Victory12/Daily5/receipt2, four stock layouts,58story scenes/57milestones and six pure fixed disposable selectors retained.

All7,839production inputs unchanged after temporary editor-helper removal/generated Addressables cleanup. Tracked compiler fingerprint backed up/restored outside the repository. Six generated Library caches (three active packing caches plus BeforeFirebirdArchiveR9, BeforeHomeHeaderQuestCleanupR15 and BeforeHomeEnvironmentPlayCleanupR13 historical packing folders) preserved reversibly under `/tmp/lor-selection-android-refresh-r2/BeforeFreshPacking/`; [cache mapping](QA/FreshPackingCachePreservation.json). The unchanged8GiB disk guard passed. Two incremental packaging states preserved under Library/LegendsOfRusPackagingBackups/BeforeSelectionRefreshR2. Approved Splash/art/GUIDs/metas/prefabs/builders/fonts/revisions/settings/packages/backend/build guards and pending user edits preserved.

Actual changed existing files: CurrentTask, ConversationHandoff, SessionLog, DevelopmentSchedule, AndroidGameplayJourney/R1/DeviceChecklist, AndroidGameplayProfile/R1/Usage and canonical Preflight/CheckPreflight. All eight backed up; earlier acceptance decisions and dated pins retained. New audit sources, C++ snippets, test/inspection/compiled proof records and logs reside in this task folder. APK/sidecar and generated caches remain ignored local artifacts. No task failures, normal personal journal/credentials, Git commit/push, phone/hosting or store action.

Human201–210 remains unrecorded by the latest user confirmation; no rescan/acceptance/tuning. All18physical rows pending. Fresh GameState recovery remains same-process Unity evidence, without physical/process restart acceptance. Real paid/video settlement and production RuStore readiness unverified; stock-max display/consumption only. Registration/failure/settings and other Persist/lifecycle callers remain outside this delivery; effects last.

Next core work: audit remaining round-registration/failure save recovery and scope fixes from reproduced defects. Human201–210recordings/ratings/block/story observations remain the scheduled Chapter1 acceptance dependency before tuning. Phone/hosting remains deferred.
'''
(BASE/'TestResults.md').write_text(report)
entry=f'''# 2026-10-03 — Level selection recovery guarded Android refresh R2 complete

[Build results](SelectionRecovery/R2AndroidRefresh/TestResults.md) · [Reconciliation](SelectionRecovery/R2AndroidRefresh/QA/BuildValidation.json) · [Final validation](SelectionRecovery/R2AndroidRefresh/QA/FinalValidation.json). APK **{size} bytes**, SHA256 `{h}`, now includes atomic selection/normalization, persistence-lock rejection and acknowledged map/Play/start routing alongside all earlier recovery fixes. Prior TutorialR2APK/sidecar preserved under BeforeSelectionRefreshR2.

Unity6000.5.9f1:160fresh distinct Edit checks pass, zero failures/skips; existing guarded build succeeds/exit0 ({seconds:.2f}s). Prior same-source350passes (251Play/99Edit) remain R1evidence; no new Play Mode/device run.49generated/native owners and five callback regions match packaged code (47owners before). Original certificate/identity/permissions/CRC/16KiB/sixELFs/nineDEX/fourJava-meta pins retained.403ZIP paths/399payloads unchanged/four reviewed changes; engine and complete player data pack identical, all29387membership rows/seven page sizes unchanged. {checks}mocked offline checks pass; actual default preflight pins the new artifact with zero ADB calls.7839production inputs unchanged; helper/generated cleanup complete, tracked fingerprint restored, six generated caches preserved reversibly with unchanged build/disk guards. Eight current handoff/tool files updated; earlier acceptance/history retained.

**Next core work:** audit remaining round-registration/failure save recovery using reproduced defects to scope changes. Human201–210recordings/ratings/observations and review before tuning remain parallel; user reconfirmed unrecorded at R1, no rescan here. Phone/hosting deferred, all18physical rows pending. Paid/video settlement and RuStore production readiness unverified; stock-max restricted, effects last. Registration/failure/settings and other Persist/lifecycle callers remain outside this delivery. No personal journal/credentials/Git/store or physical acceptance action.

'''
for name in ('CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule'):
 p=ROOT/'ProjectDocs/AI'/f'{name}.md';old=p.read_text();assert old==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
 p.write_text(entry+old if name in ('CurrentTask','ConversationHandoff') else (old.rstrip()+'\n\n'+entry).rstrip()+'\n')
for relative,boundary in [('ProjectDocs/AI/AndroidGameplayProfile/R1/Usage.md','## 2026-10-03 — daily-reward build pin'),('ProjectDocs/AI/AndroidGameplayJourney/R1/DeviceChecklist.md','## Device execution matrix')]:
 p=ROOT/relative;s=p.read_text();assert s==(BASE/'Sources/Before'/relative).read_text();end=s.index(boundary);current,history=s[:end],s[end:]
 current=current.replace('tutorial-recovery R2','selection-recovery R2').replace('tutorial recovery Android refresh R2','selection recovery Android refresh R2').replace('TutorialRecovery/R2AndroidRefresh','SelectionRecovery/R2AndroidRefresh').replace('linked tutorial R2','linked selection R2').replace('and42native-method','and49native-method').replace('766,026,550',size).replace('b699646db5f7ac9effc403c768eb19200b5eb3b58f14858b331f5edb80aa5fb4',h)
 note=f'''\n\n## 2026-10-03 — selection recovery build pin

[Current SelectionR2 results](../../SelectionRecovery/R2AndroidRefresh/TestResults.md) verify {size}bytes/SHA256`{h}`, original development certificate,49native owners/five callback regions,160fresh Unity Edit checks and{checks}mocked offline boundaries. Prior same-source350runtime/service checks remain separate. Previous TutorialR2APK/sidecar retained; earlier dated pins remain historical. Rejected selection/normalization keeps the exact map and selected level; Close/Back dismisses the existing error, with retry disabled until recovery. Start checks normalization before spending. All18physical rows still require native touches, disposable-profile isolation and lifecycle/recovery evidence. No phone/hosting or commercial acceptance claim; no arbitrary board checkpoint.\n'''
 p.write_text((current+history).rstrip()+note)
for name in ('Build.unity.log','Build.launcher.log'):shutil.copy2(Path('/tmp/lor-selection-android-refresh-r2/build')/name,QA/'Build'/name)
for name in ('unity.log','launcher.log'):shutil.copy2(Path('/tmp/lor-selection-android-refresh-r2/Policy')/name,QA/'Policy'/name)
patch=[]
for original in sorted((BASE/'Sources/Before').rglob('*')):
 if original.is_file():
  rel=original.relative_to(BASE/'Sources/Before');patch.extend(difflib.unified_diff(original.read_text().splitlines(True),(ROOT/rel).read_text().splitlines(True),fromfile='a/'+str(rel),tofile='b/'+str(rel)))
(BASE/'Sources/HandoffChanges.patch').write_text(''.join(patch))
print('Verified APK results and eight handoff/tool files recorded; earlier history retained.')
