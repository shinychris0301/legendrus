"""Back up the actual startup owner, fixture and continuity before reproduction."""
from pathlib import Path
import hashlib,json,shutil,subprocess
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';QA.mkdir(exist_ok=False)
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
keys=json.loads((ROOT/'ProjectDocs/AI/PersistenceRecovery/R2AndroidRefresh/QA/BeforeHashes.json').read_text());assert len(keys)==7839
(QA/'BeforeHashes.json').write_text(json.dumps({n:sha(ROOT/n) for n in keys},indent=2)+'\n')
files=['Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs','Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs']+['ProjectDocs/AI/'+n+'.md' for n in ('CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule')]
for n in files:
 p=BASE/'Sources/Before'/n;p.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(ROOT/n,p)
s=(ROOT/'ProjectDocs/AI/PersistenceRecovery/R1/Sources/Run.py').read_text().replace('lor-persistence-recovery-r1','lor-startup-recovery-r1').replace('PersistenceR1','StartupR1')
(BASE/'Sources/Run.py').write_text(s)
(QA/'WorkingTreeBefore.txt').write_text(subprocess.check_output(['git','status','--short'],cwd=ROOT,text=True))
a=json.loads((ROOT/'ProjectDocs/AI/PersistenceRecovery/R2AndroidRefresh/QA/Artifact.json').read_text());assert sha(ROOT/a['path'])==a['sha256']
a['sidecar']=Path(str(ROOT/a['path'])+'.sha256').read_text();assert a['sidecar']==a['sha256']+'  '+Path(a['path']).name+'\n'
(QA/'ArtifactBefore.json').write_text(json.dumps(a,indent=2)+'\n')
print('7839 current inputs, six files and APK backed up; isolated startup runner prepared.')
