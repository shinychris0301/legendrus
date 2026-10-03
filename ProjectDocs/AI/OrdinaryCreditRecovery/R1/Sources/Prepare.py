from pathlib import Path
import hashlib,json,subprocess,shutil
ROOT=Path(__file__).resolve().parents[5]; BASE=Path(__file__).resolve().parent.parent
qa=BASE/'QA'; qa.mkdir(exist_ok=False)
prior=json.loads((ROOT/'ProjectDocs/AI/FirstClearRewardRecovery/R1/QA/BeforeHashes.json').read_text())
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
(qa/'BeforeHashes.json').write_text(json.dumps({n:sha(ROOT/n) for n in prior},indent=2)+'\n')
paths=['Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs','Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs','Assets/LegendsOfRus/Tests/PlayMode/InventoryAlignmentPlayModeTests.cs']
paths+=['ProjectDocs/AI/'+n+'.md' for n in ['CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule']]
for n in paths:
 p=BASE/'Sources/Before'/n;p.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(ROOT/n,p)
(qa/'WorkingTreeBefore.txt').write_text(subprocess.check_output(['git','status','--short'],cwd=ROOT,text=True))
apk=ROOT/'Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk'
(qa/'ArtifactBefore.json').write_text(json.dumps({'path':str(apk.relative_to(ROOT)),'bytes':apk.stat().st_size,'sha256':sha(apk),'sidecar':Path(str(apk)+'.sha256').read_text()},indent=2)+'\n')
script=(ROOT/'ProjectDocs/AI/FirstClearRewardRecovery/R1/Sources/Run.py').read_text().replace('lor-first-clear-reward-recovery-r1','lor-ordinary-credit-recovery-r1').replace('FirstClearR1','OrdinaryCreditR1')
(BASE/'Sources/Run.py').write_text(script)
print('Backed up current source/docs and',len(prior),'input hashes; APK pinned.')
