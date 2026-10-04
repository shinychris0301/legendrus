"""Snapshot reviewed selection source and adapt the existing guarded APK audit."""
from pathlib import Path
import json,hashlib,subprocess,shutil
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';QA.mkdir(exist_ok=False)
OLD=ROOT/'ProjectDocs/AI/TutorialRecovery/R2AndroidRefresh'
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
files=['VerifyApk.py','Compare.py','CompareNativeEngine.py','InspectCredentialDex.py','ComparePacked.py','ReviewPayloads.py','PreparePacking.py','RecordScope.py','Build.py','CaptureNative.py','TutorialRefreshBuildAudit.cs','RestoreFingerprint.py','Run.py']
for name in files:
 s=(OLD/'Sources'/name).read_text().replace('ProjectDocs/AI/TutorialRecovery/R2AndroidRefresh','ProjectDocs/AI/SelectionRecovery/R2AndroidRefresh').replace('lor-tutorial-android-refresh-r2','lor-selection-android-refresh-r2').replace('BeforeTutorialRefreshR2','BeforeSelectionRefreshR2').replace('TutorialRefreshBuildAudit','SelectionRefreshBuildAudit')
 s=s.replace('ProjectDocs/AI/NarrativeRecovery/R2AndroidRefresh/QA','ProjectDocs/AI/TutorialRecovery/R2AndroidRefresh/QA')
 if name=='PreparePacking.py':
  start=s.index('paths = ');end=s.index('record = []',start)
  s=s[:start]+"paths = "+repr(['Library/PlayerDataCache/Android','Library/Bee/Android/Prj/IL2CPP/Gradle/launcher/build/outputs/apk/debug','Library/Bee/Android/Prj/IL2CPP/Gradle/unityLibrary/src/main/assets','Library/LegendsOfRusPackagingBackups/BeforeFirebirdArchiveR9','Library/LegendsOfRusPackagingBackups/BeforeHomeHeaderQuestCleanupR15','Library/LegendsOfRusPackagingBackups/BeforeHomeEnvironmentPlayCleanupR13'])+'\n'+s[end:]
  s=s.replace('three active and four historical','three active and three historical').replace('Seven generated','Six generated')
 if name=='ReviewPayloads.py':
  s=s.replace('IL2CPP metadata for tutorial acknowledgement/error ownership; previous transaction owners retained','IL2CPP metadata for acknowledged selection/normalization APIs; previous recovery owners retained').replace('Compiled atomic tutorial acknowledgement, retained rejected step, callback ownership and scoped Back/error handling; previous transaction owners retained; method bytes checked separately','Compiled atomic selection/normalization and acknowledged native map/start routing; all prior recovery owners retained; method bytes checked separately')
 (BASE/'Sources'/name.replace('TutorialRefreshBuildAudit','SelectionRefreshBuildAudit')).write_text(s)
prior=json.loads((OLD/'QA/BeforeHashes.json').read_text());(QA/'BeforeHashes.json').write_text(json.dumps({p:sha(ROOT/p) for p in prior},indent=2)+'\n')
paths=['ProjectDocs/AI/'+n+'.md' for n in ('CurrentTask','SessionLog','DevelopmentSchedule','ConversationHandoff')]+['ProjectDocs/AI/AndroidGameplayJourney/R1/DeviceChecklist.md','ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/Preflight.py','ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/CheckPreflight.py','ProjectDocs/AI/AndroidGameplayProfile/R1/Usage.md']
for n in paths:
 target=BASE/'Sources/Before'/n;target.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(ROOT/n,target)
pins=json.loads((OLD/'QA/PinnedPluginInventory.json').read_text());assert len(pins)==4 and all(sha(ROOT/p)==h for p,h in pins.items());(QA/'PinnedPluginInventory.json').write_text(json.dumps(pins,indent=2)+'\n')
status=subprocess.check_output(['git','status','--short'],cwd=ROOT,text=True);(QA/'WorkingTreeTaskStart.txt').write_text(status)
fingerprint='.utmp/Debug/4u1s6d3l/arm64-v8a/configure_fingerprint.bin';assert fingerprint not in status
tmp=Path('/tmp/lor-selection-android-refresh-r2');tmp.mkdir(exist_ok=False)
backup=tmp/'BeforeTrackedFingerprint.bin';shutil.copy2(ROOT/fingerprint,backup)
(QA/'BeforeTrackedFingerprint.json').write_text(json.dumps({'path':fingerprint,'backup':str(backup),'sha256':sha(backup)},indent=2)+'\n')
a=json.loads((OLD/'QA/Artifact.json').read_text());assert sha(ROOT/a['path'])==a['sha256'];(QA/'ArtifactBefore.json').write_text(json.dumps(a,indent=2)+'\n')
print('7839 inputs/eight handoff files snapshotted; prior APK and four pins confirmed.')
