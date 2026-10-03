"""Check task-only source scope and reconcile actual Unity evidence."""
from pathlib import Path
import ast,difflib,hashlib,json,re,subprocess,xml.etree.ElementTree as ET
ROOT=Path(__file__).resolve().parents[5]; BASE=Path(__file__).resolve().parent.parent; QA=BASE/'QA'
read=lambda n:json.loads((QA/n).read_text())
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
before=read('BeforeHashes.json')
after={str(p.relative_to(ROOT)):sha(p) for folder in ('Assets','Packages','ProjectSettings','Backend') for p in (ROOT/folder).rglob('*') if p.is_file() and '__pycache__' not in p.parts and p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key') and not p.name.startswith('.env')}
expected=['Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs','Assets/LegendsOfRus/Tests/PlayMode/InventoryAlignmentPlayModeTests.cs','Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs']
assert len(before)==len(after)==7839 and set(before)==set(after)
changed=sorted(n for n in before if before[n]!=after[n]);assert changed==expected
patch=''
for n in expected:
 old=BASE/'Sources/Before'/n;assert sha(old)==before[n]
 patch+=''.join(difflib.unified_diff(old.read_text().splitlines(True),(ROOT/n).read_text().splitlines(True),fromfile='a/'+n,tofile='b/'+n))
(BASE/'Sources/TaskChanges.patch').write_text(patch)
subprocess.run(['git','apply','--reverse','--check','-'],input=patch,text=True,cwd=ROOT,check=True,capture_output=True)
old=(BASE/'Sources/Before'/expected[0]).read_text();new=(ROOT/expected[0]).read_text()
# Ordinary credit methods are the only changed production source; previous patches are retained byte for byte.
new=new.replace('        // Compatibility entry points retain their void contract. Callers that need\n        // confirmation of a full, committed credit must use the TryAdd APIs.\n', '', 1)
for start,end in [('        public void AddGold(', '        public bool SpendGold('),('        public void AddGems(', '        public bool SpendGems('),('        public void AddBooster(', '        public int BoosterCount(')]:
 old=old[:old.index(start)]+old[old.index(end):]
 new=new[:new.index(start)]+new[new.index(end):]
assert old==new
repro=read('Reproduce/summary.json');assert repro['exitCode']==2 and repro['result']['failed']=='12'
assert all('Strings differ' in r['message'] for r in repro['nonPassing'])
counts={};names=set();play=set();edit=set()
for group in ('Core','UI','Runtime','Edit'):
 summary=read(group+'/summary.json');cases=ET.parse(QA/group/'results.xml').findall('.//test-case')
 assert summary['exitCode']==0 and summary['result']['failed']==summary['result']['skipped']=='0'
 assert all(c.get('result')=='Passed' for c in cases)
 groupnames={c.get('fullname') for c in cases};assert len(groupnames)==len(cases)
 counts[group]=len(cases);names.update(groupnames)
 (edit if summary['mode']=='EditMode' else play).update(groupnames)
 assert summary['isolatedConfig'].startswith('/tmp/lor-ordinary-credit-recovery-r1/')
for method in ('OrdinaryCreditOverflowPreservesLiveSaveAndJournal','OrdinaryCreditBlockedWritePreservesLiveSaveAndFutureJournal','OrdinaryCreditFullLimitReloadAndOverflowRecovery','OrdinaryCreditLargestGrantPreservesOtherResources'):
 assert sum('.'+method+'(' in n for n in names)==6
for method in ('OrdinaryCreditNonpositiveAndUnknownBoosterKeepLegacyBehavior','LiveHud_RefreshesFromServiceChangesAndReentry','RealBoosterQuantitiesRefreshWithoutConsumption','LiveInventoryAndMovesUseTheExistingBoosterTransaction','TimeoutRecovery_RestoresBoardAndRefundsBooster','Recovery_ClearsBoosterVfxSuppressesCompletionAndRefundsOnce','OverflowDoubleGoldCanRetryAtExactLimitAndPersistsOnce','OverflowClaimPreservesAllBalancesEligibilityAndJournal','FirstClearOverflowNativeRetryCommitsExactlyOnceInBothLanguages'):
 assert any(n.endswith('.'+method) for n in names),method
assert not list((ROOT/'Assets').glob('InitTestScene*.unity'))
artifact=read('ArtifactBefore.json');apk=ROOT/artifact['path'];assert sha(apk)==artifact['sha256'] and apk.stat().st_size==artifact['bytes']
assert Path(str(apk)+'.sha256').read_text()==artifact['sidecar']
checked=0
for p in [BASE/'TestResults.md',*[ROOT/'ProjectDocs/AI'/f'{n}.md' for n in ('CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule')]]:
 content=p.read_text();backup=BASE/'Sources/Before'/p.relative_to(ROOT)
 if backup.exists():
  previous=backup.read_text();assert content.endswith(previous) if p.name in ('CurrentTask.md','ConversationHandoff.md') else content.startswith(previous.rstrip())
  content='\n'.join(line[1:] for line in difflib.unified_diff(previous.splitlines(),content.splitlines()) if line.startswith('+') and not line.startswith('+++'))
 for destination in re.findall(r'\[[^\]]+\]\(([^)]+)\)',content):
  if re.match(r'^(https?://|#)',destination):continue
  target=(p.parent/destination.split('#')[0]).resolve();assert target.exists() or target==QA/'FinalValidation.json';checked+=1
for p in (BASE/'Sources').glob('*.py'):ast.parse(p.read_text())
subprocess.run(['git','diff','--check'],cwd=ROOT,check=True,capture_output=True)
result={'result':'Passed','freshUnityGroupCounts':counts,'unityEditDistinctPassed':len(edit),'unityPlayDistinctPassed':len(play),'unityTotalDistinctPassed':len(names),'failed':0,'skipped':0,'preFixRuntimeMutationCases':12,'changedProductionInputs':expected,'projectInputsUnchanged':7836,'addedProductionInputs':[],'deletedProductionInputs':[],'previousGameStateFixesBytePreserved':True,'allExistingMetasAssetsPrefabsPackagesSettingsGuardsPreserved':True,'taskPatchReverseCheckPassed':True,'newLocalLinksChecked':checked,'currentApkBytes':artifact['bytes'],'currentApkSha256':artifact['sha256'],'apkAndSidecarUnchanged':True,'ordinaryCreditFixNotPackaged':True,'newAndroidBuild':False,'phoneOrHostingAction':False,'gitCommitOrPushPerformed':False,'personalJournalRead':False,'human201To210AcceptanceStillPending':True,'legacyNoncampaignRefundRejectionHandlingNotChanged':True}
(QA/'FinalValidation.json').write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result,indent=2))
