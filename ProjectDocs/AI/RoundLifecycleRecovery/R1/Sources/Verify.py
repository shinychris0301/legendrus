"""Reconcile actual Unity runs, task-local scope and unchanged development APK."""
from pathlib import Path
import ast,difflib,hashlib,json,subprocess,xml.etree.ElementTree as ET,zipfile
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
read=lambda n:json.loads((QA/n).read_text())
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
before=read('BeforeHashes.json');after={str(p.relative_to(ROOT)):sha(p) for folder in ('Assets','Packages','ProjectSettings','Backend') for p in (ROOT/folder).rglob('*') if p.is_file() and '__pycache__' not in p.parts and p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key') and not p.name.startswith('.env')}
expected=sorted(['Assets/LegendsOfRus/Scripts/Runtime/'+n for n in ('Core/GameState.cs','Core/GameState.RetainedRound.cs','Gameplay/Match3Board.cs','UI/ScreenRouter.cs')]+['Assets/LegendsOfRus/Tests/PlayMode/'+n for n in ('LevelSystemRuntimePlayModeTests.cs','PreLevelAlignmentPlayModeTests.cs','Match3RuntimePlayModeTests.cs','RetainedRoundPlayModeTests.cs')])
changed=sorted(n for n in before if after.get(n)!=before[n]);assert set(before)==set(after) and len(before)==7839 and changed==expected,(changed,expected)
patch=''
for n in changed:
 original=BASE/'Sources/Before'/n;assert sha(original)==before[n]
 patch+=''.join(difflib.unified_diff(original.read_text().splitlines(True),(ROOT/n).read_text().splitlines(True),fromfile='a/'+n,tofile='b/'+n))
(BASE/'Sources/TaskChanges.patch').write_text(patch)
subprocess.run(['git','apply','--reverse','--check','-'],input=patch,text=True,cwd=ROOT,check=True,capture_output=True)
def original(n):return (BASE/'Sources/Before'/n).read_text(),(ROOT/n).read_text()
def region(s,start,end):return s[s.index(start):s.index(end,s.index(start))]
def strip(s,start,end):return s.replace(region(s,start,end),'',1)
a,b=original('Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs');assert strip(a,'        public int RegisterLevelAttempt(','        public LevelCompletionResult CompleteLevel(')==strip(b,'        public int RegisterLevelAttempt(','        public LevelCompletionResult CompleteLevel(')
a,b=original('Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.RetainedRound.cs');assert a.replace('            progress.failures++;','            try { progress.failures=checked(progress.failures+1); }\n            catch (OverflowException) { return false; }\n            ',1)==b
# Ordinary attempt forwarding is separate from existing campaign/retained inventory ownership.
a,b=original('Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs')
b=b.replace(region(b,'        public void Initialize(\n','        // Explicit development campaign entry:'),region(a,'        public void Initialize(\n','        // Explicit development campaign entry:'),1)
b=b.replace(', bool preRegisteredOrdinaryAttempt = false)',')',1).replace('campaignManaged = committedAttempt.HasValue && !preRegisteredOrdinaryAttempt;','campaignManaged = committedAttempt.HasValue;',1).replace('            if (attempt < 1) throw new InvalidOperationException("Round attempt could not be committed.");\n','',1);assert a==b
# Only normal start guarding, synchronous one-time forwarding and its fields change in the router.
a,b=original('Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs')
b=b.replace('        bool levelStartInProgress;\n        LevelData pendingGameplayLevel;\n        int pendingGameplayAttempt;\n','',1)
b=b.replace(region(b,'        void StartLevel()','        bool OpenSelectedLevel()'),region(a,'        void StartLevel()','        bool OpenSelectedLevel()'),1)
b=b.replace('            LevelData startedLevel = pendingGameplayLevel ?? ChapterLevelCatalog.ResolveSelected(GameState.Instance.Save);\n            int? startedAttempt = pendingGameplayAttempt > 0 ? (int?)pendingGameplayAttempt : null;\n            pendingGameplayLevel = null;\n            pendingGameplayAttempt = 0;\n','',1).replace('                levelData: startedLevel,\n','',1).replace('                },\n                registeredAttempt: startedAttempt\n','                }\n',1);assert a==b
for n,marker in [('LevelSystemRuntimePlayModeTests.cs','        [TestCase(false)] [TestCase(true)]\n        public void RoundRegistrationRejected'),('PreLevelAlignmentPlayModeTests.cs','        [UnityTest]\n        public IEnumerator RoundStartOverflow'),('RetainedRoundPlayModeTests.cs','        [Test]\n        public void RoundFailureFinalizeOverflow'),('Match3RuntimePlayModeTests.cs','        [UnityTest]\n        public IEnumerator RoundStartCommitted')]:
 a,b=original('Assets/LegendsOfRus/Tests/PlayMode/'+n);assert b[:b.index(marker)].rstrip()+'\n'+b[b.rfind('    }'):]==a[:a.rfind('    }')].rstrip()+'\n'+a[a.rfind('    }'):],n
assert read('Reproduce/summary.json')['result']['failed']=='8'
assert read('Boundaries/summary.json')['exitCode']==1 and 'result' not in read('Boundaries/summary.json')
assert read('BoundariesCorrected/summary.json')['result']['failed']=='2'
counts={};names=set();modes={'PlayMode':set(),'EditMode':set()}
for group in read('FinalGroups.json'):
 s=read(group+'/summary.json');cases=ET.parse(QA/group/'results.xml').findall('.//test-case');groupnames={c.get('fullname') for c in cases}
 assert s['exitCode']==0 and s['result']['failed']==s['result']['skipped']=='0'
 assert len(cases)==len(groupnames) and all(c.get('result')=='Passed' for c in cases)
 assert not names.intersection(groupnames);names.update(groupnames);modes[s['mode']].update(groupnames);counts[group]=len(cases)
 assert s['isolatedConfig'].startswith('/tmp/lor-continuation-r1/RoundLifecycleR1Retained/' if group=='Retained' else '/tmp/lor-round-lifecycle-recovery-r1/')
for group,num in [('Focus',8),('BoundariesFinal',11)]:
 s=read(group+'/summary.json');focused={c.get('fullname') for c in ET.parse(QA/group/'results.xml').findall('.//test-case')};assert len(focused)==num and focused<=names and s['exitCode']==0
artifact=read('ArtifactBefore.json');apk=ROOT/artifact['path'];assert sha(apk)==artifact['sha256'] and apk.stat().st_size==artifact['bytes'] and Path(str(apk)+'.sha256').read_text()==artifact['sidecar']
with zipfile.ZipFile(apk) as z:assert b'TryStartLevel\0' not in z.read('assets/bin/Data/Managed/Metadata/global-metadata.dat')
for p in (BASE/'Sources').glob('*.py'):ast.parse(p.read_text(),filename=str(p))
subprocess.run(['git','diff','--check'],cwd=ROOT,check=True,capture_output=True)
assert not list((ROOT/'Assets').glob('InitTestScene*.unity'))
result={'result':'Passed','freshUnityGroupCounts':counts,'unityPlayDistinctPassed':len(modes['PlayMode']),'unityEditDistinctPassed':len(modes['EditMode']),'unityTotalDistinctPassed':len(names),'failed':0,'skipped':0,'preFixFailures':8,'focusedChecksIncludedOnce':True,'nativeRejectionAndReentryLanguages':['ru','en'],'changedProductionInputs':changed,'projectInputsUnchanged':len(before)-len(changed),'addedProductionInputs':[],'deletedProductionInputs':[],'otherGameStateAndPreviousRecoveryMethodsBytePreserved':True,'retainedMemoryRetryPolicyPreserved':True,'routerOutsideNormalStartForwardingBytePreserved':True,'boardOutsideOrdinaryAttemptForwardingAndAcknowledgementBytePreserved':True,'existingSeedAndRuntimeBoosterOwnershipPreserved':True,'existingFixtureChecksBytePreserved':True,'allExistingMetasArtPrefabsBuildersSettingsPackagesBackendAndBuildGuardsPreserved':True,'taskPatchReverseCheckPassed':True,'currentApkBytes':artifact['bytes'],'currentApkSha256':artifact['sha256'],'apkAndSidecarUnchanged':True,'roundLifecycleFixNotYetPackaged':True,'human201To210AcceptanceStillPending':True,'userPreviouslyConfirmedStillUnrecorded':True,'freshStateRecoveryIsSameProcessUnityEvidence':True,'normalPersonalJournalRead':False,'physicalGameplayTested':False,'newAndroidBuild':False,'phoneOrHostingAction':False,'gitCommitOrPushPerformed':False}
(QA/'TestReconciliation.json').write_text(json.dumps({'result':'Passed','groups':counts,'playPassed':len(modes['PlayMode']),'editPassed':len(modes['EditMode']),'totalDistinctPassed':len(names),'failed':0,'skipped':0,'focusedDuplicatesCountedAgain':False},indent=2)+'\n')
(QA/'SourceValidation.json').write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result,indent=2))
