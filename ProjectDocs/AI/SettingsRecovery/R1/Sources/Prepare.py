"""Back up reviewed settings owners, fixtures and continuity before reproduction."""
from pathlib import Path
import hashlib,json,shutil,subprocess
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';QA.mkdir(exist_ok=False)
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
old=ROOT/'ProjectDocs/AI/RoundLifecycleRecovery/R1'
keys=json.loads((old/'QA/BeforeHashes.json').read_text());assert len(keys)==7839
(QA/'BeforeHashes.json').write_text(json.dumps({n:sha(ROOT/n) for n in keys},indent=2)+'\n')
files=['Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionSettingsView.cs','Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs','Assets/LegendsOfRus/Tests/PlayMode/SettingsAlignmentPlayModeTests.cs']+['ProjectDocs/AI/'+n+'.md' for n in ('CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule')]
for n in files:
 target=BASE/'Sources/Before'/n;target.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(ROOT/n,target)
s=(old/'Sources/Run.py').read_text().replace('lor-round-lifecycle-recovery-r1','lor-settings-recovery-r1').replace('RoundLifecycleR1','SettingsR1');(BASE/'Sources/Run.py').write_text(s)
(QA/'WorkingTreeBefore.txt').write_text(subprocess.check_output(['git','status','--short'],cwd=ROOT,text=True))
a=json.loads((ROOT/'ProjectDocs/AI/RoundLifecycleRecovery/R2AndroidRefresh/QA/Artifact.json').read_text());assert sha(ROOT/a['path'])==a['sha256']
a['sidecar']=Path(str(ROOT/a['path'])+'.sha256').read_text();assert a['sidecar']==a['sha256']+'  '+Path(a['path']).name+'\n'
(QA/'ArtifactBefore.json').write_text(json.dumps(a,indent=2)+'\n')
print('7839 inputs, eight files and current APK snapshotted; isolated test runner prepared.')
