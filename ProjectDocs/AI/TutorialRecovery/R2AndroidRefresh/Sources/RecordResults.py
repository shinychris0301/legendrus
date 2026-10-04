"""Record inspected tutorial APK and update eight current handoff/tool files with history retained."""
from pathlib import Path
import difflib,json,shutil
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
read=lambda n:json.loads((QA/n).read_text())
a=read('Artifact.json');v=read('BuildValidation.json');b=read('PreflightBoundaryChecks.json');assert v['result']==b['result']=='Passed'
h=a['sha256'];size=f"{a['bytes']:,}";checks=b['passed'];seconds=read('Build/summary.json')['durationSeconds'];native=read('AfterNative.json');assert native['atomicTutorialAcknowledgementRecoveryPackaged']
report=f'''# Tutorial recovery Android refresh R2 — complete

[Build reconciliation](QA/BuildValidation.json) · [Compiled owners and callbacks](QA/AfterNative.json) · [Source results](../R1/TestResults.md) · [Final validation](QA/FinalValidation.json).

Current development APK: `{a['path']}`, **{size} bytes**, SHA256 `{h}`. It now packages atomic CompleteTutorial, retained step/queue/overlay/pause after rejected saving, guards against stale Continue or destroyed/disabled presenters, and exact tutorial error ownership for Close/Back. Successful or already acknowledged steps continue using the existing sequence and telemetry policies. All earlier narrative, energy, reward, spending and durable booster-refund fixes remain included. No new skin, progression rules, checkpoint or art.

Previous NarrativeR2 APK:766,019,098bytes/SHA256`932535353501df1e04e65814ffd2383edc44b33bc60973aa692ae5e84b3ae928`, preserved with its inherited checksum under `Builds/Verification/v1_17_step01/BeforeTutorialRefreshR2/`.

Unity6000.5.9f1: **160 fresh distinct Edit Mode policy/content/preset checks passed**, zero failures/skips. Existing guarded development build succeeded, launcher exit0, BuildReport Succeeded, {seconds:.2f}s. Prior tutorial R1 **405 distinct passes (294Play/111Edit)** remain same-source runtime/service evidence; no new Play Mode run in this build task. **{checks} synthetic/mocked offline boundary checks passed**. Actual default preflight verifies this APK pin and compiled evidence without ADB, install, launch or player-data access.

Before rebuild41generated/native owners matched the old APK; after rebuild42owners and two tutorial callback byte regions match the actual packaged stripped IL2CPP library/Gradle input. CompleteTutorial calls atomic commit without direct Persist. Advance returns after AcknowledgementRejected before clearing CurrentStep and checks presentation/active lifecycle. BuildOverlay captures the step and overlay; its compiled Continue callback checks both before Advance. BuildGameplayOverlay subscribes the retained error callback. ShowGameplayTutorialError scopes board, paused phase, current presenter/content and modal/transition state, and binds Close with no retry. NavigateBack prioritizes tutorial ownership; retirement clears it. Previous narrative, energy, checked grants, spending/receipt/refund owners remain verified. These compiled checks are separate from phone acceptance.

Signature/original certificate, package `com.example.legendsofrus.v117debug`, version0.0.0/code1,ARM64,min26/target36,portrait/debug and permissions retained. ZIP CRC/exact accounting/16KiB alignment and six ELF load alignments pass. Nine credential DEX/JNI classes/four Java-meta pins retained. All403ZIP paths remain:399payloads identical/four reviewed changes (IL2CPP code/metadata and two build identifiers). APK grows7,452bytes; engine library/bindings and the entire player data pack are byte-identical. All29,387path/GUID/type rows and seven page object sizes remain unchanged. Profile13/Dialogue2/Gameplay24/Victory12/Daily5/receipt2, four stock layouts,58story scenes/57milestones and six pure fixed disposable selectors retained.

All7,839production inputs remain unchanged after temporary helper removal/generated Addressables cleanup; tracked compiler fingerprint backed up/restored outside the repository. Seven generated Library caches (three active packing caches plus four historical generated packing folders) preserved reversibly under `/tmp/lor-tutorial-android-refresh-r2/BeforeFreshPacking/`, satisfying the unchanged8GiB disk guard. See [cache mapping](QA/FreshPackingCachePreservation.json). Two incremental packaging states preserved in Library/LegendsOfRusPackagingBackups/BeforeTutorialRefreshR2. Sources/settings/packages/backend/build guards, approved Splash/art/GUIDs/metas/prefabs/builders/fonts and pending edits preserved.

Preparation corrections: an adapted runner initially retained the prior temporary path; its collision guard stopped before Unity invocation. The empty task-local output folder was removed and the task path corrected; earlier logs were untouched. The first After native audit hit an overbroad method-list replacement after saving snippets but before publishing its JSON proof. The audit was corrected and rerun against identical snippets; final42owners/two callbacks verified. Both were tooling failures; the guarded Unity test/build runs succeeded.

Actual changed existing files: CurrentTask, ConversationHandoff, SessionLog, DevelopmentSchedule, AndroidGameplayJourney/R1/DeviceChecklist, AndroidGameplayProfile/R1/Usage and canonical Preflight/CheckPreflight. All eight were backed up, and earlier acceptance/dates retained. [Handoff/tool patch](Sources/HandoffChanges.patch). New review sources, generated C++ snippets and QA records reside only in this task folder; APK/sidecar and caches remain ignored local artifacts.

No normal personal journal/credentials, Git commit/push, phone/hosting or store action. Human201–210 remains unrecorded per prior confirmation; prior review0eligible, no new scan/tuning. All18physical rows pending. Fresh GameState re-offer/reload remains same-process Unity evidence, without a persisted tutorial/board checkpoint. Paid/video settlement and production RuStore readiness remain unverified; stock-max display/consumption only. General Persist/settings/selection/start/failure callers remain outside this patch; effects last.

Next scheduled dependency: collect human rounds201–210 recordings, outcome ratings and block/story observations, then review before tuning and subsequent blocks/finale. Physical QA remains deferred until phone work resumes.
'''
(BASE/'TestResults.md').write_text(report)
entry=f'''# 2026-10-03 — Tutorial recovery guarded Android refresh R2 complete

[Build results](TutorialRecovery/R2AndroidRefresh/TestResults.md) · [Reconciliation](TutorialRecovery/R2AndroidRefresh/QA/BuildValidation.json) · [Final validation](TutorialRecovery/R2AndroidRefresh/QA/FinalValidation.json). APK **{size} bytes**, SHA256 `{h}`, now includes atomic tutorial acknowledgement, retained rejected step/queue/overlay/pause, stale/lifecycle callback guards and scoped error/Back handling alongside all prior transaction/narrative fixes. Prior NarrativeR2 APK/sidecar preserved under BeforeTutorialRefreshR2.

Unity6000.5.9f1:160fresh distinct Edit checks pass, zero failures/skips; existing guarded build succeeds/exit0 ({seconds:.2f}s). Prior same-source405passes (294Play/111Edit) remain source-stage evidence; no new Play Mode/device run.42generated/native owners and two callback regions match packaged code (41owners before). Signature/original certificate/identity/permissions/CRC/16KiB/sixELFs/nineDEX/fourJava-meta pins retained.403ZIP paths/399payloads unchanged/four reviewed changes; engine and complete player data pack identical, all29387membership rows/seven page sizes unchanged. {checks}mocked offline checks pass; actual default preflight pins new artifact with zero ADB calls.7839production inputs unchanged; helper/generated cleanup complete, tracked fingerprint restored, seven generated caches preserved reversibly with unchanged disk/build guards. Eight current handoff/tool files updated; earlier acceptance/history preserved. Two preparation/audit tooling corrections are documented separately from successful Unity runs.

**Next:** human201–210recordings/ratings/observations and review before tuning, then later blocks/finale/gaps per schedule. User previously confirmed unrecorded; prior0eligible, no rescan here. Phone/hosting remains deferred, all18physical rows pending. Paid/video settlement and RuStore production readiness unverified; stock-max restricted, effects last. General Persist/settings/selection/start/failure callers remain outside this patch. No personal journal/credentials/Git/store or physical acceptance action.

'''
for name in ('CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule'):
 p=ROOT/'ProjectDocs/AI'/f'{name}.md';old=p.read_text();assert old==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
 p.write_text(entry+old if name in ('CurrentTask','ConversationHandoff') else (old.rstrip()+'\n\n'+entry).rstrip()+'\n')
for relative,boundary in [('ProjectDocs/AI/AndroidGameplayProfile/R1/Usage.md','## 2026-10-03 — daily-reward build pin'),('ProjectDocs/AI/AndroidGameplayJourney/R1/DeviceChecklist.md','## Device execution matrix')]:
 p=ROOT/relative;s=p.read_text();assert s==(BASE/'Sources/Before'/relative).read_text();end=s.index(boundary);current,history=s[:end],s[end:]
 current=current.replace('narrative-recovery R2','tutorial-recovery R2').replace('narrative recovery Android refresh R2','tutorial recovery Android refresh R2').replace('NarrativeRecovery/R2AndroidRefresh','TutorialRecovery/R2AndroidRefresh').replace('linked narrative R2','linked tutorial R2').replace('and35native-method','and42native-method').replace('766,019,098',size).replace('932535353501df1e04e65814ffd2383edc44b33bc60973aa692ae5e84b3ae928',h)
 note=f'''\n\n## 2026-10-03 — tutorial recovery build pin

[Current TutorialR2 results](../../TutorialRecovery/R2AndroidRefresh/TestResults.md) verify {size}bytes/SHA256`{h}`, original development certificate,42native owners/two tutorial callbacks,160fresh Unity Edit checks and{checks}mocked offline checks. Prior same-source405runtime/service checks remain separate. Previous NarrativeR2APK/sidecar retained; earlier dated pins remain historical. Tutorial saving rejection preserves its paused step and exact overlay; Close/Back dismisses the error. After repair/relaunch, unread authored steps are re-offered without a line/board checkpoint. All18physical rows still require phone touch, lifecycle/recovery and disposable-profile isolation evidence. No phone/hosting or commercial acceptance claim.\n'''
 p.write_text((current+history).rstrip()+note)
for name in ('Build.unity.log','Build.launcher.log'):shutil.copy2(Path('/tmp/lor-tutorial-android-refresh-r2/build')/name,QA/'Build'/name)
for name in ('unity.log','launcher.log'):shutil.copy2(Path('/tmp/lor-tutorial-android-refresh-r2/Policy')/name,QA/'Policy'/name)
patch=[]
for original in sorted((BASE/'Sources/Before').rglob('*')):
 if original.is_file():
  rel=original.relative_to(BASE/'Sources/Before');patch.extend(difflib.unified_diff(original.read_text().splitlines(True),(ROOT/rel).read_text().splitlines(True),fromfile='a/'+str(rel),tofile='b/'+str(rel)))
(BASE/'Sources/HandoffChanges.patch').write_text(''.join(patch))
print('Results and eight handoff/tool files recorded; earlier history preserved.')
