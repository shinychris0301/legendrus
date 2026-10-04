from pathlib import Path
import hashlib,json,subprocess,shutil
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';QA.mkdir(exist_ok=False)
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
prior=json.loads((ROOT/'ProjectDocs/AI/OrdinaryCreditRecovery/R1/QA/BeforeHashes.json').read_text())
(QA/'BeforeHashes.json').write_text(json.dumps({n:sha(ROOT/n) for n in prior},indent=2)+'\n')
paths=['Assets/LegendsOfRus/Scripts/Runtime/'+p for p in ['Core/GameState.cs','Gameplay/Match3Board.cs','Gameplay/RuntimeCommandTransaction.cs','UI/ScreenRouter.cs','Services/PlayerSavePayloadCodec.cs']]
paths+=['Assets/LegendsOfRus/Tests/PlayMode/'+p+'.cs' for p in ['LevelSystemRuntimePlayModeTests','Match3RuntimePlayModeTests','PreLevelAlignmentPlayModeTests','GameplayAlignmentPlayModeTests','CoreLoopVfxBoosterFeedbackPlayModeTests']]
paths+=['ProjectDocs/AI/'+n+'.md' for n in ['CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule']]
for n in paths:
 p=BASE/'Sources/Before'/n;p.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(ROOT/n,p)
(QA/'WorkingTreeBefore.txt').write_text(subprocess.check_output(['git','status','--short'],text=True))
apk=ROOT/'Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk'
(QA/'ArtifactBefore.json').write_text(json.dumps({'path':str(apk.relative_to(ROOT)),'bytes':apk.stat().st_size,'sha256':sha(apk),'sidecar':Path(str(apk)+'.sha256').read_text()},indent=2)+'\n')
s=(ROOT/'ProjectDocs/AI/OrdinaryCreditRecovery/R1/Sources/Run.py').read_text().replace('lor-ordinary-credit-recovery-r1','lor-spending-recovery-r1').replace('OrdinaryCreditR1','SpendingR1');(BASE/'Sources/Run.py').write_text(s)
print('Source and handoff backups captured; APK pinned; 7839 input hashes recorded.')
