from pathlib import Path
import hashlib,json,shutil,subprocess
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';QA.mkdir(exist_ok=False)
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
prior=json.loads((ROOT/'ProjectDocs/AI/NarrativeRecovery/R2AndroidRefresh/QA/BeforeHashes.json').read_text());(QA/'BeforeHashes.json').write_text(json.dumps({n:sha(ROOT/n) for n in prior},indent=2)+'\n')
paths=['Assets/LegendsOfRus/Scripts/Runtime/'+n for n in ('Core/GameState.cs','UI/LevelTutorialPresenter.cs','UI/ScreenRouter.cs')]+['Assets/LegendsOfRus/Tests/PlayMode/'+n for n in ('LevelSystemRuntimePlayModeTests.cs','TutorialAlignmentPlayModeTests.cs')]+['ProjectDocs/AI/'+n+'.md' for n in ('CurrentTask','SessionLog','ConversationHandoff','DevelopmentSchedule')]
for n in paths:
 target=BASE/'Sources/Before'/n;target.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(ROOT/n,target)
(QA/'WorkingTreeBefore.txt').write_text(subprocess.check_output(['git','status','--short'],text=True,cwd=ROOT))
a=json.loads((ROOT/'ProjectDocs/AI/NarrativeRecovery/R2AndroidRefresh/QA/Artifact.json').read_text());apk=ROOT/a['path'];assert sha(apk)==a['sha256'];a['sidecar']=Path(str(apk)+'.sha256').read_text();(QA/'ArtifactBefore.json').write_text(json.dumps(a,indent=2)+'\n')
s=(ROOT/'ProjectDocs/AI/NarrativeRecovery/R1/Sources/Run.py').read_text().replace('lor-narrative-recovery-r1','lor-tutorial-recovery-r1').replace('NarrativeR1','TutorialR1');(BASE/'Sources/Run.py').write_text(s)
print('Existing source/user changes backed up;7839inputs and currentAPK pinned.')
