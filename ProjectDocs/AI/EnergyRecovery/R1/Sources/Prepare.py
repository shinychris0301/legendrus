from pathlib import Path
import hashlib,json,subprocess,shutil
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';QA.mkdir(exist_ok=False)
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
prior=json.loads((ROOT/'ProjectDocs/AI/SpendingRecovery/R2AndroidRefresh/QA/BeforeHashes.json').read_text())
(QA/'BeforeHashes.json').write_text(json.dumps({n:sha(ROOT/n) for n in prior},indent=2)+'\n')
paths=['Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs','Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs']+['ProjectDocs/AI/'+n+'.md' for n in ('CurrentTask','SessionLog','ConversationHandoff','DevelopmentSchedule')]
for n in paths:
 p=BASE/'Sources/Before'/n;p.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(ROOT/n,p)
(QA/'WorkingTreeBefore.txt').write_text(subprocess.check_output(['git','status','--short'],text=True,cwd=ROOT))
a=json.loads((ROOT/'ProjectDocs/AI/SpendingRecovery/R2AndroidRefresh/QA/Artifact.json').read_text());apk=ROOT/a['path'];assert sha(apk)==a['sha256'] and apk.stat().st_size==a['bytes'];a['sidecar']=Path(str(apk)+'.sha256').read_text();(QA/'ArtifactBefore.json').write_text(json.dumps(a,indent=2)+'\n')
s=(ROOT/'ProjectDocs/AI/SpendingRecovery/R1/Sources/Run.py').read_text().replace('lor-spending-recovery-r1','lor-energy-recovery-r1').replace('SpendingR1','EnergyR1');(BASE/'Sources/Run.py').write_text(s)
print('Current source/user edits backed up;7839input hashes and latestAPK pinned.')
