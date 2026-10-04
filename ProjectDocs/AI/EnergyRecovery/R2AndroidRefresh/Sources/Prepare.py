"""Adapt reviewed guarded delivery tools and snapshot the verified energy source state."""
from pathlib import Path
import json,hashlib,subprocess,shutil
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';QA.mkdir(exist_ok=False)
OLD=ROOT/'ProjectDocs/AI/SpendingRecovery/R2AndroidRefresh/Sources'
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
for name in ('VerifyApk.py','Compare.py','CompareNativeEngine.py','InspectCredentialDex.py','ComparePacked.py','ReviewPayloads.py','PreparePacking.py','RecordScope.py','Build.py','CaptureNative.py','SpendingRefreshBuildAudit.cs'):
 s=(OLD/name).read_text().replace('ProjectDocs/AI/SpendingRecovery/R2AndroidRefresh','ProjectDocs/AI/EnergyRecovery/R2AndroidRefresh').replace('lor-spending-android-refresh-r2','lor-energy-android-refresh-r2').replace('BeforeSpendingRefreshR2','BeforeEnergyRefreshR2').replace('SpendingRefreshBuildAudit','EnergyRefreshBuildAudit')
 if name=='Build.py':
  s=s.replace("ROOT/'ProjectDocs/AI/SpendingRecovery/R1/QA'/test_run/'summary.json'", "QA/test_run/'summary.json'")
  s=s.replace('ProjectDocs/AI/DailyRewardRecovery/R2AndroidRefresh/QA/Artifact.json','ProjectDocs/AI/SpendingRecovery/R2AndroidRefresh/QA/Artifact.json')
 if name=='ComparePacked.py':s=s.replace('ProjectDocs/AI/DailyRewardRecovery/R2AndroidRefresh/QA/PackedAssets.json','ProjectDocs/AI/SpendingRecovery/R2AndroidRefresh/QA/PackedAssets.json')
 if name=='ReviewPayloads.py':
  s=s.replace('IL2CPP metadata for atomic spending/credit APIs, validated optional booster-command receipt, and refund recovery','IL2CPP metadata for capped atomic energy grants/refills/clock APIs; previous reward/spending/refund owners retained')
  s=s.replace('Compiled accumulated reward, credit, spending and durable booster-command recovery; native method bytes checked separately','Compiled capped energy candidates and shared widened clock math; all previous transaction owners retained; method bytes checked separately')
 if name=='PreparePacking.py':
  s=s.replace("paths = ['Library/PlayerDataCache/Android',", "paths = ['Library/LegendsOfRusPackagingBackups/BeforeFreshPackagingR2/outputs/apk/debug',\n         'Library/PlayerDataCache/Android',")
  s=s.replace('only three generated packing caches','three active packing caches plus one historical generated APK-output cache').replace('Three generated packing caches preserved','Four generated packing caches preserved')
 (BASE/'Sources'/name.replace('SpendingRefreshBuildAudit','EnergyRefreshBuildAudit')).write_text(s)
s=(ROOT/'ProjectDocs/AI/EnergyRecovery/R1/Sources/Run.py').read_text().replace('lor-energy-recovery-r1','lor-energy-android-refresh-r2');(BASE/'Sources/Run.py').write_text(s)
prior=json.loads((ROOT/'ProjectDocs/AI/SpendingRecovery/R2AndroidRefresh/QA/BeforeHashes.json').read_text());(QA/'BeforeHashes.json').write_text(json.dumps({p:sha(ROOT/p) for p in prior},indent=2)+'\n')
paths=['ProjectDocs/AI/'+n+'.md' for n in ('CurrentTask','SessionLog','DevelopmentSchedule','ConversationHandoff')]+['ProjectDocs/AI/AndroidGameplayJourney/R1/DeviceChecklist.md','ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/Preflight.py','ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/CheckPreflight.py','ProjectDocs/AI/AndroidGameplayProfile/R1/Usage.md']
for n in paths:
 target=BASE/'Sources/Before'/n;target.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(ROOT/n,target)
pins=json.loads((ROOT/'ProjectDocs/AI/SpendingRecovery/R2AndroidRefresh/QA/PinnedPluginInventory.json').read_text());assert len(pins)==4 and all(sha(ROOT/p)==h for p,h in pins.items());(QA/'PinnedPluginInventory.json').write_text(json.dumps(pins,indent=2)+'\n')
status=subprocess.check_output(['git','status','--short'],cwd=ROOT,text=True);(QA/'WorkingTreeTaskStart.txt').write_text(status)
fingerprint='.utmp/Debug/4u1s6d3l/arm64-v8a/configure_fingerprint.bin';assert fingerprint not in status
tmp=Path('/tmp/lor-energy-android-refresh-r2');tmp.mkdir(exist_ok=False)
backup=tmp/'BeforeTrackedFingerprint.bin';shutil.copy2(ROOT/fingerprint,backup)
(QA/'BeforeTrackedFingerprint.json').write_text(json.dumps({'path':fingerprint,'backup':str(backup),'sha256':sha(backup)},indent=2)+'\n')
a=json.loads((ROOT/'ProjectDocs/AI/SpendingRecovery/R2AndroidRefresh/QA/Artifact.json').read_text());assert sha(ROOT/a['path'])==a['sha256'];(QA/'ArtifactBefore.json').write_text(json.dumps(a,indent=2)+'\n')
print('7839inputs/eight handoff files snapshotted; previousAPK/pins confirmed; existing guards retained.')
