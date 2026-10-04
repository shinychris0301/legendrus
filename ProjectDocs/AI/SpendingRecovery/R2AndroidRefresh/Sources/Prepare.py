"""Snapshot source-complete inputs and handoff tools before the authorized APK refresh."""
from pathlib import Path
import json,hashlib,shutil,subprocess
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
prior=json.loads((ROOT/'ProjectDocs/AI/SpendingRecovery/R1/QA/BeforeHashes.json').read_text())
(QA/'BeforeHashes.json').write_text(json.dumps({p:sha(ROOT/p) for p in prior},indent=2)+'\n')
paths=['ProjectDocs/AI/'+n+'.md' for n in ('CurrentTask','SessionLog','DevelopmentSchedule','ConversationHandoff')]+['ProjectDocs/AI/AndroidGameplayJourney/R1/DeviceChecklist.md','ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/Preflight.py','ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/CheckPreflight.py','ProjectDocs/AI/AndroidGameplayProfile/R1/Usage.md']
for n in paths:
 target=BASE/'Sources/Before'/n;target.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(ROOT/n,target)
pins=json.loads((ROOT/'ProjectDocs/AI/DailyRewardRecovery/R2AndroidRefresh/QA/PinnedPluginInventory.json').read_text());assert len(pins)==4 and all(sha(ROOT/p)==h for p,h in pins.items())
(QA/'PinnedPluginInventory.json').write_text(json.dumps(pins,indent=2)+'\n')
(QA/'WorkingTreeTaskStart.txt').write_text(subprocess.check_output(['git','status','--short'],text=True,cwd=ROOT))
print('Build inputs and eight handoff/tool files backed up; four Java/meta pins verified.')
