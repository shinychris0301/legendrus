"""Snapshot reviewed narrative source and adapt the existing guarded APK audit."""
from pathlib import Path
import json,hashlib,subprocess,shutil
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';QA.mkdir(exist_ok=False)
OLD=ROOT/'ProjectDocs/AI/EnergyRecovery/R2AndroidRefresh'
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
files=['VerifyApk.py','Compare.py','CompareNativeEngine.py','InspectCredentialDex.py','ComparePacked.py','ReviewPayloads.py','PreparePacking.py','RecordScope.py','Build.py','CaptureNative.py','EnergyRefreshBuildAudit.cs','RestoreFingerprint.py','Run.py']
for name in files:
 s=(OLD/'Sources'/name).read_text().replace('ProjectDocs/AI/EnergyRecovery/R2AndroidRefresh','ProjectDocs/AI/NarrativeRecovery/R2AndroidRefresh').replace('lor-energy-android-refresh-r2','lor-narrative-android-refresh-r2').replace('BeforeEnergyRefreshR2','BeforeNarrativeRefreshR2').replace('EnergyRefreshBuildAudit','NarrativeRefreshBuildAudit')
 s=s.replace('ProjectDocs/AI/SpendingRecovery/R2AndroidRefresh/QA','ProjectDocs/AI/EnergyRecovery/R2AndroidRefresh/QA')
 if name=='PreparePacking.py':
  s=s.replace("'Library/LegendsOfRusPackagingBackups/BeforeFreshPackagingR2/outputs/apk/debug',\n         ",'')
  s=s.replace('three active packing caches plus one historical generated APK-output cache','three active packing caches').replace('Four generated','Three generated')
 if name=='CaptureNative.py':
  s=s.replace("'GameState_AdvanceEnergyClock']", "'GameState_AdvanceEnergyClock','GameState_CompleteStoryScene','GameState_AcknowledgeChapterCompletion','StoryService_AdvanceDialogue','ScreenRouter_ContinueAfterDialogue','ScreenRouter_ContinueAfterChapterComplete']")
  s=s.replace('(?:GameState|Match3Board|ScreenRouter)', '(?:GameState|Match3Board|ScreenRouter|StoryService)')
  s=s.replace("set(targets)-{'GameState_TryRefreshEnergyFromClock','GameState_TryAddEnergy','GameState_TryRefillEnergy','GameState_AdvanceEnergyClock'}",'set(targets)')
  s=s.replace("'callsPersist':'GameState_Persist_' in body", "'callsStoryCommit':'GameState_CompleteStoryScene_' in body,'callsErrorDialog':'ScreenRouter_ShowErrorDialog_' in body,'callsChapterAcknowledgement':'StoryService_AcknowledgeChapterComplete_' in body,'callsPersist':'GameState_Persist_' in body")
  s=s.replace("assert {'GameState_SpendEnergy','GameState_AddEnergy','GameState_RefillEnergy','GameState_RefreshEnergyFromClock'}<=set(changed)", "assert {'GameState_CompleteStoryScene','GameState_AcknowledgeChapterCompletion','StoryService_AdvanceDialogue','ScreenRouter_ContinueAfterDialogue','ScreenRouter_ContinueAfterChapterComplete'}<=set(changed)")
  needle=" record['changedNativeMethodsFromBaseline']=changed"
  s=s.replace(needle,""" for owner in ('GameState_CompleteStoryScene','GameState_AcknowledgeChapterCompletion'):
  assert record['methods'][owner]['callsAtomicBalanceCommit'] and not record['methods'][owner]['callsPersist']
 assert record['methods']['StoryService_AdvanceDialogue']['callsStoryCommit']
 for owner in ('ScreenRouter_ContinueAfterDialogue','ScreenRouter_ContinueAfterChapterComplete'):
  assert record['methods'][owner]['callsErrorDialog']
 assert record['methods']['ScreenRouter_ContinueAfterChapterComplete']['callsChapterAcknowledgement']
 assert b'SaveRejected\\0' in metadata
 record['atomicNarrativeAcknowledgementRecoveryPackaged']=True
"""+needle)
 if name=='ReviewPayloads.py':
  s=s.replace('IL2CPP metadata for capped atomic energy grants/refills/clock APIs; previous reward/spending/refund owners retained','IL2CPP metadata for rejected narrative acknowledgement status; previous transaction owners retained')
  s=s.replace('Compiled capped energy candidates and shared widened clock math; all previous transaction owners retained; method bytes checked separately','Compiled atomic story/chapter acknowledgement and retained final dialogue/native error routes; previous transaction owners retained; method bytes checked separately')
 (BASE/'Sources'/name.replace('EnergyRefreshBuildAudit','NarrativeRefreshBuildAudit')).write_text(s)
prior=json.loads((OLD/'QA/BeforeHashes.json').read_text());(QA/'BeforeHashes.json').write_text(json.dumps({p:sha(ROOT/p) for p in prior},indent=2)+'\n')
paths=['ProjectDocs/AI/'+n+'.md' for n in ('CurrentTask','SessionLog','DevelopmentSchedule','ConversationHandoff')]+['ProjectDocs/AI/AndroidGameplayJourney/R1/DeviceChecklist.md','ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/Preflight.py','ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/CheckPreflight.py','ProjectDocs/AI/AndroidGameplayProfile/R1/Usage.md']
for n in paths:
 target=BASE/'Sources/Before'/n;target.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(ROOT/n,target)
pins=json.loads((OLD/'QA/PinnedPluginInventory.json').read_text());assert len(pins)==4 and all(sha(ROOT/p)==h for p,h in pins.items());(QA/'PinnedPluginInventory.json').write_text(json.dumps(pins,indent=2)+'\n')
status=subprocess.check_output(['git','status','--short'],cwd=ROOT,text=True);(QA/'WorkingTreeTaskStart.txt').write_text(status)
fingerprint='.utmp/Debug/4u1s6d3l/arm64-v8a/configure_fingerprint.bin';assert fingerprint not in status
tmp=Path('/tmp/lor-narrative-android-refresh-r2');tmp.mkdir(exist_ok=False)
backup=tmp/'BeforeTrackedFingerprint.bin';shutil.copy2(ROOT/fingerprint,backup)
(QA/'BeforeTrackedFingerprint.json').write_text(json.dumps({'path':fingerprint,'backup':str(backup),'sha256':sha(backup)},indent=2)+'\n')
a=json.loads((OLD/'QA/Artifact.json').read_text());assert sha(ROOT/a['path'])==a['sha256'];(QA/'ArtifactBefore.json').write_text(json.dumps(a,indent=2)+'\n')
print('7839 inputs/eight handoff files snapshotted; prior APK and four pins confirmed.')
