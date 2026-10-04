"""Reconcile actual Unity checks, the narrow tutorial patch and the unchanged APK."""
from pathlib import Path
import ast,difflib,hashlib,json,subprocess,xml.etree.ElementTree as ET,zipfile
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
read=lambda n:json.loads((QA/n).read_text())
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
before=read('BeforeHashes.json')
after={str(p.relative_to(ROOT)):sha(p) for f in ('Assets','Packages','ProjectSettings','Backend') for p in (ROOT/f).rglob('*') if p.is_file() and '__pycache__' not in p.parts and p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key') and not p.name.startswith('.env')}
expected=sorted(['Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/LevelTutorialPresenter.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs','Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs','Assets/LegendsOfRus/Tests/PlayMode/TutorialAlignmentPlayModeTests.cs'])
assert set(before)==set(after) and len(before)==7839
changed=sorted(p for p in before if before[p]!=after[p]);assert changed==expected
patch=''
for n in changed:
 old=BASE/'Sources/Before'/n;assert sha(old)==before[n]
 patch+=''.join(difflib.unified_diff(old.read_text().splitlines(True),(ROOT/n).read_text().splitlines(True),fromfile='a/'+n,tofile='b/'+n))
(BASE/'Sources/TaskChanges.patch').write_text(patch)
subprocess.run(['git','apply','--reverse','--check','-'],input=patch,text=True,cwd=ROOT,capture_output=True,check=True)
def originals(n):return (BASE/'Sources/Before'/n).read_text(),(ROOT/n).read_text()
def strip(s,start,end):return s[:s.index(start)]+s[s.index(end,s.index(start)):]
a,b=originals(expected[0]);assert strip(a,'        public bool CompleteTutorial(','        public bool HasCompletedStoryScene(')==strip(b,'        public bool CompleteTutorial(','        public bool HasCompletedStoryScene(')
a,b=originals('Assets/LegendsOfRus/Scripts/Runtime/UI/LevelTutorialPresenter.cs')
b=b.replace('        public event Action<LevelTutorialPresenter> AcknowledgementRejected;\n\n','',1)
b=b.replace('''            GameObject presentedOverlay = overlay;
            action.onClick.AddListener(() =>
            {
                if (overlay == presentedOverlay && CurrentStep == step) Advance();
            });''','            action.onClick.AddListener(Advance);',1)
assert strip(a,'        public void Advance()','        void ResumeBoard()')==strip(b,'        public void Advance()','        void ResumeBoard()')
a,b=originals('Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs')
b=b.replace('        LevelTutorialPresenter tutorialErrorOwner;\n','',1).replace('            tutorialErrorOwner = null;\n','',1)
b=b.replace('                    if(tutorialErrorOwner != null)CloseModal();\n                    else if(IsGameplayInventoryError(activeBoard))CloseGameplayInventoryError(activeBoard);','                    if(IsGameplayInventoryError(activeBoard))CloseGameplayInventoryError(activeBoard);',1)
b=strip(b,'        void ShowGameplayTutorialError(','        bool IsGameplayInventoryError(')
b=b.replace('''            var tutorial = tutorialGo.GetComponent<LevelTutorialPresenter>();
            tutorial.AcknowledgementRejected += failed => ShowGameplayTutorialError(board, failed);
            tutorial.Initialize(activeBoard, activeBoard.CurrentLevel);''','''            tutorialGo.GetComponent<LevelTutorialPresenter>().Initialize(
                activeBoard,
                activeBoard.CurrentLevel
            );''',1)
assert a==b
n='Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs';a,b=originals(n);start=b.index('        [TestCase(false)] [TestCase(true)]\n        public void TutorialRejectedWrite');end=b.index('        void SnapshotAndClearSaveJournal()',start);assert b[:start]+b[end:]==a
n='Assets/LegendsOfRus/Tests/PlayMode/TutorialAlignmentPlayModeTests.cs';a,b=originals(n);start=b.index(' [UnityTest] public IEnumerator TutorialRejectedNative');end=b.index(' [UnityTest] public IEnumerator SectionsAndLocalizedCaptures',start);assert b[:start]+b[end:]==a
assert read('Reproduce/summary.json')['result']['failed']=='5'
assert read('Focus/summary.json')['exitCode']==0 and read('Focus/summary.json')['result']['passed']=='8'
initial=read('Boundaries/summary.json');assert initial['result']['passed']=='13' and initial['result']['failed']=='1'
counts={};names=set();modes={'PlayMode':set(),'EditMode':set()}
for group in ('Core','UI','Runtime','Retained','Journey','TutorialFunnel','Edit'):
 s=read(group+'/summary.json');cases=ET.parse(QA/group/'results.xml').findall('.//test-case');groupnames={c.get('fullname') for c in cases}
 assert s['exitCode']==0 and s['result']['failed']==s['result']['skipped']=='0'
 assert len(cases)==len(groupnames) and all(c.get('result')=='Passed' for c in cases)
 assert not names.intersection(groupnames),group
 names.update(groupnames);modes[s['mode']].update(groupnames);counts[group]=len(cases)
 assert s['isolatedConfig'].startswith('/tmp/lor-continuation-r1/TutorialR1Retained/' if group=='Retained' else '/tmp/lor-tutorial-recovery-r1/')
assert counts['Core']==154 and counts['Runtime']==16 and counts['Retained']==51 and counts['Journey']==3
for group,num in [('Focus',8),('BoundariesFinal',14)]:
 s=read(group+'/summary.json');focused={c.get('fullname') for c in ET.parse(QA/group/'results.xml').findall('.//test-case')};assert len(focused)==num and focused<=names and s['exitCode']==0
assert not list((ROOT/'Assets').glob('InitTestScene*.unity'))
artifact=read('ArtifactBefore.json');apk=ROOT/artifact['path'];assert sha(apk)==artifact['sha256'] and apk.stat().st_size==artifact['bytes']
assert Path(str(apk)+'.sha256').read_text()==artifact['sidecar']
with zipfile.ZipFile(apk) as z:
 metadata=z.read('assets/bin/Data/Managed/Metadata/global-metadata.dat');assert b'ShowGameplayTutorialError\0' not in metadata
for p in (BASE/'Sources').glob('*.py'):ast.parse(p.read_text(),filename=str(p))
subprocess.run(['git','diff','--check'],cwd=ROOT,capture_output=True,check=True)
result={'result':'Passed','freshUnityGroupCounts':counts,'unityPlayDistinctPassed':len(modes['PlayMode']),'unityEditDistinctPassed':len(modes['EditMode']),'unityTotalDistinctPassed':len(names),'failed':0,'skipped':0,'preFixFailures':5,'initialNativeIntegrationFailure':1,'initialFailureReason':'Android Back selected broad inventory-lock recovery for a tutorial-owned error; fixed with scoped modal owner and destroyed callback guard','focused14CasesCountedWithinFinalGroups':True,'nativeRejectionLanguages':['ru','en'],'changedProductionInputs':changed,'projectInputsUnchanged':7834,'addedProductionInputs':[],'deletedProductionInputs':[],'otherGameStateMethodsAndAllPreviousTransactionFixesBytePreserved':True,'otherPresenterCodeQueueSkinWordingAndFontsBytePreserved':True,'routerOutsideScopedTutorialOwnerIntegrationBytePreserved':True,'priorFixturesOutsideNewTutorialBlocksBytePreserved':True,'allExistingMetasArtPrefabsSettingsPackagesBackendAndBuildGuardsPreserved':True,'taskPatchReverseCheckPassed':True,'currentApkBytes':artifact['bytes'],'currentApkSha256':artifact['sha256'],'apkAndSidecarUnchanged':True,'tutorialFixNotYetPackaged':True,'human201To210AcceptanceStillPending':True,'humanRecordingsRescannedThisTask':False,'normalPersonalJournalRead':False,'physicalGameplayTested':False,'newAndroidBuild':False,'phoneOrHostingAction':False,'gitCommitOrPushPerformed':False}
(QA/'TestReconciliation.json').write_text(json.dumps({'result':'Passed','groups':counts,'playPassed':len(modes['PlayMode']),'editPassed':len(modes['EditMode']),'totalDistinctPassed':len(names),'failed':0,'skipped':0,'focusedDuplicatesCountedAgain':False},indent=2)+'\n')
(QA/'SourceValidation.json').write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result,indent=2))
