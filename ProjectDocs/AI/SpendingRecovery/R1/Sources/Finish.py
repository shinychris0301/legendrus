"""Reconcile isolated Unity results and retain a narrow reviewable source patch."""
from pathlib import Path
import ast,difflib,hashlib,json,subprocess,xml.etree.ElementTree as ET
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
def save(n,v):(QA/n).write_text(json.dumps(v,indent=2)+'\n')
before=json.loads((QA/'BeforeHashes.json').read_text())
after={str(p.relative_to(ROOT)):sha(p) for f in ('Assets','Packages','ProjectSettings','Backend') for p in (ROOT/f).rglob('*') if p.is_file() and '__pycache__' not in p.parts and p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key') and not p.name.startswith('.env')}
expected=['Assets/LegendsOfRus/Scripts/Runtime/'+p for p in ['Core/GameState.cs','Gameplay/Match3Board.cs','Gameplay/RuntimeCommandTransaction.cs','Services/PlayerSavePayloadCodec.cs','UI/ScreenRouter.cs']]+['Assets/LegendsOfRus/Tests/PlayMode/'+p+'.cs' for p in ['CoreLoopVfxBoosterFeedbackPlayModeTests','GameplayAlignmentPlayModeTests','LevelSystemRuntimePlayModeTests','Match3RuntimePlayModeTests','PreLevelAlignmentPlayModeTests']]
assert set(before)==set(after) and len(before)==7839
changed=sorted(p for p in before if before[p]!=after[p]);assert changed==sorted(expected)
patch=''
for n in changed:
 old=BASE/'Sources/Before'/n;assert sha(old)==before[n]
 patch+=''.join(difflib.unified_diff(old.read_text().splitlines(True),(ROOT/n).read_text().splitlines(True),fromfile='a/'+n,tofile='b/'+n))
(BASE/'Sources/TaskChanges.patch').write_text(patch)
subprocess.run(['git','apply','--reverse','--check','-'],input=patch,text=True,capture_output=True,check=True,cwd=ROOT)
counts={};allnames=set();modes={'PlayMode':set(),'EditMode':set()}
for group,count in {'Core':112,'Runtime':16,'UI':40,'Retained':51,'Journeys':2,'Edit':61,'Policy':160}.items():
 s=json.loads((QA/group/'summary.json').read_text());cases=ET.parse(QA/group/'results.xml').findall('.//test-case');names={c.get('fullname') for c in cases}
 assert s['exitCode']==0 and s['result']['failed']==s['result']['skipped']=='0'
 assert len(cases)==len(names)==count and all(c.get('result')=='Passed' for c in cases)
 assert not allnames.intersection(names),group
 allnames.update(names);modes[s['mode']].update(names);counts[group]=count
 assert s['isolatedConfig'].startswith('/tmp/lor-continuation-r1/SpendingR1Retained/' if group=='Retained' else '/tmp/lor-spending-recovery-r1/')
repro=json.loads((QA/'Reproduce/summary.json').read_text());assert repro['result']['failed']=='9'
assert len(allnames)==442 and len(modes['PlayMode'])==len(modes['EditMode'])==221
assert not list((ROOT/'Assets').glob('InitTestScene*.unity'))
a=json.loads((QA/'ArtifactBefore.json').read_text());apk=ROOT/a['path'];assert sha(apk)==a['sha256'] and apk.stat().st_size==a['bytes'] and Path(str(apk)+'.sha256').read_text()==a['sidecar']
for p in (BASE/'Sources').glob('*.py'):ast.parse(p.read_text())
subprocess.run(['git','diff','--check'],check=True,capture_output=True,cwd=ROOT)
save('TestReconciliation.json',{'result':'Passed','groups':counts,'distinctPlayModePassed':221,'distinctEditModePassed':221,'distinctTotalPassed':442,'failed':0,'skipped':0,'preFixFailures':9,'focusedRepeatedRunsCountedAgain':False})
result={'result':'Passed','unityTotalDistinctPassed':442,'unityPlayDistinctPassed':221,'unityEditDistinctPassed':221,'changedProductionInputs':changed,'projectInputsUnchanged':7829,'addedProductionInputs':[],'deletedProductionInputs':[],'allExistingMetasArtPrefabsFontsPackagesSettingsBackendAndGuardsPreserved':True,'taskPatchReverseCheckPassed':True,'sourceStageApkUnchanged':True,'sourceStageApkSha256':a['sha256'],'sourceStageApkBytes':a['bytes'],'normalPersonalJournalRead':False,'physicalGameplayTested':False,'human201To210AcceptancePending':True,'gitCommitOrPushPerformed':False}
save('FinalValidation.json',result);print(json.dumps(result,indent=2))
