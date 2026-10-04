"""Back up current handoff and project inputs, adapt the prior guarded APK inspection."""
from pathlib import Path
import json,hashlib,subprocess,shutil
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';QA.mkdir(exist_ok=False)
OLD=ROOT/'ProjectDocs/AI/RoundLifecycleRecovery/R2AndroidRefresh'
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
subprocess.run(['git','apply','--reverse','--check',str(ROOT/'ProjectDocs/AI/SettingsRecovery/R1/Sources/TaskChanges.patch')],check=True,cwd=ROOT,capture_output=True)
files=['VerifyApk.py','Compare.py','CompareNativeEngine.py','InspectCredentialDex.py','ComparePacked.py','ReviewPayloads.py','PreparePacking.py','RecordScope.py','Build.py','CaptureNative.py','RoundLifecycleRefreshBuildAudit.cs','RestoreFingerprint.py','Run.py','VerifyBuild.py','VerifyFinal.py']
for name in files:
 s=(OLD/'Sources'/name).read_text().replace('ProjectDocs/AI/RoundLifecycleRecovery/R2AndroidRefresh','ProjectDocs/AI/SettingsRecovery/R2AndroidRefresh').replace('lor-round-lifecycle-android-refresh-r2','lor-settings-android-refresh-r2').replace('BeforeRoundLifecycleRefreshR2','BeforeSettingsRefreshR2').replace('RoundLifecycleRefreshBuildAudit','SettingsRefreshBuildAudit')
 s=s.replace('ProjectDocs/AI/SelectionRecovery/R2AndroidRefresh/QA','ProjectDocs/AI/RoundLifecycleRecovery/R2AndroidRefresh/QA')
 if name=='PreparePacking.py':
  start=s.index('paths = ');end=s.index('record = []',start)
  s=s[:start]+'paths = '+repr(['Library/PlayerDataCache/Android','Library/Bee/Android/Prj/IL2CPP/Gradle/launcher/build/outputs/apk/debug','Library/Bee/Android/Prj/IL2CPP/Gradle/unityLibrary/src/main/assets'])+'\n'+s[end:]
  s=s.replace('three active and two historical','three active').replace('Five generated','Three generated')
 if name in ('VerifyBuild.py','VerifyFinal.py'):
  s=s.replace('RoundLifecycleRecovery/R1','SettingsRecovery/R1').replace('452','396').replace("'sourceStageUnityPlayDistinctPassed':340","'sourceStageUnityPlayDistinctPassed':311").replace("'sourceStageUnityEditDistinctPassed':112","'sourceStageUnityEditDistinctPassed':85").replace('==61','==68').replace('==54','==67').replace("'nativeMethodCount':61","'nativeMethodCount':68").replace("'beforeNativeMethodCount':54","'beforeNativeMethodCount':67").replace("'nativeOwnersVerified':61","'nativeOwnersVerified':68").replace('>122','>155').replace('==5','==3').replace('766023250','766031502').replace('0c7c454dc83154e622fff646ac865a2c03fee5b5852ab5f46be24703bf48fb7a','71e042f7eaef9edba33c16916c843aa28737b5432666f166aec1a59a0b3b3984').replace('SelectionR2Apk','RoundLifecycleR2Apk').replace('oldSelectionR2Apk','oldRoundLifecycleR2Apk').replace('fiveGeneratedCache','threeGeneratedCache')
 if name=='VerifyFinal.py':
  s=s.replace("'canonicalOfflinePreflightPinsRoundLifecycleAndAllPreviousFixes'","'canonicalOfflinePreflightPinsSettingsAndAllPreviousFixes'")
 if name=='ReviewPayloads.py':
  s=s.replace('IL2CPP metadata for acknowledged combined round start and failure recovery; previous recovery owners retained','IL2CPP metadata for acknowledged preference saving and owned Settings callbacks; previous recovery owners retained').replace('Compiled atomic energy/attempt start, checked failure counters, native once-only forwarding; all prior recovery owners retained; method bytes checked separately','Compiled atomic preference commitment, rejected-save error panel and callback ownership/reentry; all prior recovery owners retained; method bytes checked separately')
 (BASE/'Sources'/name.replace('RoundLifecycleRefreshBuildAudit','SettingsRefreshBuildAudit')).write_text(s)
shutil.copy2(OLD/'Sources/RoundNativePolicy.json',BASE/'Sources/RoundNativePolicy.json')
prior=json.loads((OLD/'QA/BeforeHashes.json').read_text());assert len(prior)==7839
(QA/'BeforeHashes.json').write_text(json.dumps({p:sha(ROOT/p) for p in prior},indent=2)+'\n')
paths=['ProjectDocs/AI/'+n+'.md' for n in ('CurrentTask','SessionLog','DevelopmentSchedule','ConversationHandoff')]+['ProjectDocs/AI/AndroidGameplayJourney/R1/DeviceChecklist.md','ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/Preflight.py','ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/CheckPreflight.py','ProjectDocs/AI/AndroidGameplayProfile/R1/Usage.md']
for n in paths:
 target=BASE/'Sources/Before'/n;target.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(ROOT/n,target)
pins=json.loads((OLD/'QA/PinnedPluginInventory.json').read_text());assert len(pins)==4 and all(sha(ROOT/p)==h for p,h in pins.items());(QA/'PinnedPluginInventory.json').write_text(json.dumps(pins,indent=2)+'\n')
status=subprocess.check_output(['git','status','--short'],cwd=ROOT,text=True);(QA/'WorkingTreeTaskStart.txt').write_text(status)
fingerprint='.utmp/Debug/4u1s6d3l/arm64-v8a/configure_fingerprint.bin';assert fingerprint not in status
tmp=Path('/tmp/lor-settings-android-refresh-r2');tmp.mkdir(exist_ok=False)
backup=tmp/'BeforeTrackedFingerprint.bin';shutil.copy2(ROOT/fingerprint,backup)
(QA/'BeforeTrackedFingerprint.json').write_text(json.dumps({'path':fingerprint,'backup':str(backup),'sha256':sha(backup)},indent=2)+'\n')
a=json.loads((OLD/'QA/Artifact.json').read_text());assert sha(ROOT/a['path'])==a['sha256'] and (ROOT/a['path']).stat().st_size==a['bytes']
assert Path(str(ROOT/a['path'])+'.sha256').read_text()==a['sha256']+'  '+Path(a['path']).name+'\n'
(QA/'ArtifactBefore.json').write_text(json.dumps(a,indent=2)+'\n')
print('7839 inputs/eight handoff files snapshotted; previous round lifecycle APK and four pins confirmed.')
