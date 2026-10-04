"""Preserve current inputs and adapt the completed guarded Settings build audits."""
from pathlib import Path
import ast, hashlib, json, shutil, subprocess
ROOT=Path(__file__).resolve().parents[5]; BASE=Path(__file__).resolve().parent.parent
QA=BASE/'QA'; QA.mkdir(exist_ok=False)
OLD=ROOT/'ProjectDocs/AI/SettingsRecovery/R2AndroidRefresh'
def sha(p):
    with p.open('rb') as f: return hashlib.file_digest(f,'sha256').hexdigest()
subprocess.run(['git','apply','--reverse','--check',str(ROOT/'ProjectDocs/AI/PersistenceRecovery/R1/Sources/TaskChanges.patch')],cwd=ROOT,check=True,capture_output=True)
names=['Build.py','CaptureNative.py','Compare.py','CompareNativeEngine.py','InspectCredentialDex.py','ComparePacked.py','ReviewPayloads.py','RecordScope.py','RestoreFingerprint.py','Run.py','VerifyApk.py','VerifyBuild.py','VerifyFinal.py','SettingsRefreshBuildAudit.cs']
for name in names:
    s=(OLD/'Sources'/name).read_text()
    s=s.replace('ProjectDocs/AI/SettingsRecovery/R2AndroidRefresh','ProjectDocs/AI/PersistenceRecovery/R2AndroidRefresh').replace('lor-settings-android-refresh-r2','lor-persistence-android-refresh-r2').replace('BeforeSettingsRefreshR2','BeforePersistenceRefreshR2').replace('SettingsRefreshBuildAudit','PersistenceRefreshBuildAudit')
    s=s.replace('ProjectDocs/AI/RoundLifecycleRecovery/R2AndroidRefresh/QA','ProjectDocs/AI/SettingsRecovery/R2AndroidRefresh/QA')
    if name in ('VerifyBuild.py','VerifyFinal.py'):
        s=s.replace('SettingsRecovery/R1','PersistenceRecovery/R1').replace('==396','==426').replace(':396',':426').replace(':311',':341')
        s=s.replace('==68','==72').replace('==67','==71').replace(':68',':72').replace(':67',':71')
        s=s.replace('>155','>184').replace('==3\nfor row in read', '==6\nfor row in read')
        s=s.replace('766031502','766032162').replace('71e042f7eaef9edba33c16916c843aa28737b5432666f166aec1a59a0b3b3984','0310f6a446f268fb36a70c3a7af604cc89b54c5cbc51f19a864316aeaf421adc')
        s=s.replace('RoundLifecycleR2Apk','SettingsR2Apk').replace('threeGeneratedCache','sixGeneratedCache').replace('canonicalOfflinePreflightPinsSettings','canonicalOfflinePreflightPinsPersistence')
    if name=='ReviewPayloads.py':
        s=s.replace('IL2CPP metadata for acknowledged preference saving and owned Settings callbacks; previous recovery owners retained','IL2CPP metadata for acknowledging ordinary persistence and its reentry guard; previous recovery owners retained').replace('Compiled atomic preference commitment, rejected-save error panel and callback ownership/reentry; all prior recovery owners retained; method bytes checked separately','Compiled acknowledging ordinary persistence, session-lock rejection and callback reentry cleanup; all prior recovery owners retained; method bytes checked separately')
    if name.endswith('.py'): ast.parse(s)
    (BASE/'Sources'/name.replace('SettingsRefreshBuildAudit','PersistenceRefreshBuildAudit')).write_text(s)
for name in ('RoundNativePolicy.json','SettingsNativePolicy.json'): shutil.copy2(OLD/'Sources'/name,BASE/'Sources'/name)
prior=json.loads((OLD/'QA/BeforeHashes.json').read_text()); assert len(prior)==7839
(QA/'BeforeHashes.json').write_text(json.dumps({p:sha(ROOT/p) for p in prior},indent=2)+'\n')
paths=['ProjectDocs/AI/'+n+'.md' for n in ('CurrentTask','SessionLog','DevelopmentSchedule','ConversationHandoff')]+['ProjectDocs/AI/AndroidGameplayJourney/R1/DeviceChecklist.md','ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/Preflight.py','ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/CheckPreflight.py','ProjectDocs/AI/AndroidGameplayProfile/R1/Usage.md']
for n in paths:
    p=BASE/'Sources/Before'/n; p.parent.mkdir(parents=True,exist_ok=True); shutil.copy2(ROOT/n,p)
pins=json.loads((OLD/'QA/PinnedPluginInventory.json').read_text()); assert len(pins)==4 and all(sha(ROOT/p)==h for p,h in pins.items())
(QA/'PinnedPluginInventory.json').write_text(json.dumps(pins,indent=2)+'\n')
status=subprocess.check_output(['git','status','--short'],cwd=ROOT,text=True); (QA/'WorkingTreeTaskStart.txt').write_text(status)
fingerprint='.utmp/Debug/4u1s6d3l/arm64-v8a/configure_fingerprint.bin'; assert fingerprint not in status
tmp=Path('/tmp/lor-persistence-android-refresh-r2'); tmp.mkdir(exist_ok=False)
backup=tmp/'BeforeTrackedFingerprint.bin'; shutil.copy2(ROOT/fingerprint,backup)
(QA/'BeforeTrackedFingerprint.json').write_text(json.dumps({'path':fingerprint,'backup':str(backup),'sha256':sha(backup)},indent=2)+'\n')
a=json.loads((OLD/'QA/Artifact.json').read_text()); assert sha(ROOT/a['path'])==a['sha256'] and (ROOT/a['path']).stat().st_size==a['bytes']
assert Path(str(ROOT/a['path'])+'.sha256').read_text()==a['sha256']+'  '+Path(a['path']).name+'\n'
(QA/'ArtifactBefore.json').write_text(json.dumps(a,indent=2)+'\n')
print('7839 current inputs and eight handoff files backed up; prior APK and four plugin pins verified.')
