"""Reconcile fresh isolated Unity checks, narrative-only source scope and unchanged current APK."""
from pathlib import Path
import ast,difflib,hashlib,json,re,subprocess,xml.etree.ElementTree as ET,zipfile
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';read=lambda n:json.loads((QA/n).read_text())
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
before=read('BeforeHashes.json')
after={str(p.relative_to(ROOT)):sha(p) for f in ('Assets','Packages','ProjectSettings','Backend') for p in (ROOT/f).rglob('*') if p.is_file() and '__pycache__' not in p.parts and p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key') and not p.name.startswith('.env')}
expected=sorted(['Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs','Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs','Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs','Assets/LegendsOfRus/Tests/PlayMode/DialogueAlignmentPlayModeTests.cs'])
assert set(before)==set(after) and len(before)==7839
changed=sorted(p for p in before if before[p]!=after[p]);assert changed==expected
patch=''
for n in changed:
 old=BASE/'Sources/Before'/n;assert sha(old)==before[n]
 patch+=''.join(difflib.unified_diff(old.read_text().splitlines(True),(ROOT/n).read_text().splitlines(True),fromfile='a/'+n,tofile='b/'+n))
(BASE/'Sources/TaskChanges.patch').write_text(patch)
subprocess.run(['git','apply','--reverse','--check','-'],input=patch,text=True,cwd=ROOT,capture_output=True,check=True)
old=(BASE/'Sources/Before'/expected[0]).read_text();new=(ROOT/expected[0]).read_text()
# All other GameState methods, including previous transaction fixes, remain exact.
for start,end in [('        public bool CompleteStoryScene(', '        public ChapterCompletionPhase GetChapterCompletionPhase'),('        public bool AcknowledgeChapterCompletion(', '        public ChapterCompletionSummary GetChapterCompletionSummary')]:
 old=old[:old.index(start)]+old[old.index(end):];new=new[:new.index(start)]+new[new.index(end):]
assert new==old
# Router changes are confined to the two established acknowledgement handlers.
n='Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs'
a=(BASE/'Sources/Before'/n).read_text();b=(ROOT/n).read_text()
start='        void ContinueAfterDialogue()';end='        void ContinueAfterChapterComplete('
a=a[:a.index(start)]+a[a.index(end):];b=b[:b.index(start)]+b[b.index(end):]
start=end
# Determine the next declared method from the original handler boundary.
pos=a.index(start);match=re.search(r'\n        (?:public |private |internal |protected |static )?(?:void|bool|IEnumerator|GameObject|ScreenId) \w+\(',a[pos+len(start):])
assert match
endpos=pos+len(start)+match.start()+1;next_header=a[endpos:a.index('\n',endpos)]
a=a[:pos]+a[a.index(next_header,pos+len(start)):];b=b[:b.index(start)]+b[b.index(next_header,b.index(start)+len(start)):]
assert a==b
# All story setup, content selection and routing policy remain exact.
n='Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs'
a=(BASE/'Sources/Before'/n).read_text();b=(ROOT/n).read_text().replace('        ChapterComplete,\n        SaveRejected\n','        ChapterComplete\n',1)
start='        public static StoryAdvanceResult AdvanceDialogue()';end='        public static bool ShouldShowChapterComplete()'
a=a[:a.index(start)]+a[a.index(end):];b=b[:b.index(start)]+b[b.index(end):]
assert a==b
# Existing test code outside the added narrative blocks is byte-preserved.
n='Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs'
a=(BASE/'Sources/Before'/n).read_text();b=(ROOT/n).read_text()
marker='        public void NarrativeBlockedWriteKeepsAcknowledgementPendingAndDialogueVisible'
start=b.rfind('        [TestCase(',0,b.index(marker));end=b.index('        void SnapshotAndClearSaveJournal()',start)
assert b[:start]+b[end:]==a
n='Assets/LegendsOfRus/Tests/PlayMode/DialogueAlignmentPlayModeTests.cs'
a=(BASE/'Sources/Before'/n).read_text();b=(ROOT/n).read_text()
start=b.index(' [UnityTest] public IEnumerator RejectedNarrativeWritesKeepExactNativePageInBothLanguages()');end=b.index(' [UnityTest] public IEnumerator RebindingAndDisabledStateDoNotAdvanceOrDuplicateActions()',start)
assert b[:start]+b[end:]==a
repro=read('Reproduce/summary.json');assert repro['exitCode']==2 and repro['result']['failed']=='5'
focus=read('Focus/summary.json');assert focus['result']['passed']=='8' and focus['result']['failed']=='0'
counts={};names=set();modes={'PlayMode':set(),'EditMode':set()}
for group in ('Core','UI','Runtime','Retained','Journey','NarrativeFinalUI','RecapQA','Legacy','Edit'):
 s=read(group+'/summary.json');cases=ET.parse(QA/group/'results.xml').findall('.//test-case');groupnames={c.get('fullname') for c in cases}
 assert s['exitCode']==0 and s['result']['failed']==s['result']['skipped']=='0'
 assert len(cases)==len(groupnames) and all(c.get('result')=='Passed' for c in cases)
 assert not names.intersection(groupnames),group
 names.update(groupnames);modes[s['mode']].update(groupnames);counts[group]=len(cases)
 assert s['isolatedConfig'].startswith('/tmp/lor-continuation-r1/NarrativeR1Retained/' if group=='Retained' else '/tmp/lor-narrative-recovery-r1/')
assert [counts[g] for g in ('Core','UI','Runtime','Retained','Journey')]==[143,45,16,51,3]
assert counts['Edit']>61
for method,num in [('NarrativeBlockedWriteKeepsAcknowledgementPendingAndDialogueVisible',5),('NarrativeAcknowledgementCommitsOnceWithoutOtherSaveMutation',2)]:
 assert sum('.'+method+'(' in n for n in names)==num,method
for method in ('NarrativeInvalidAndIneligibleAcknowledgementsDoNotInitializeOrWrite','NarrativeExternallyCommittedActiveSceneReturnsWithoutDuplicateGrantOrEvent','RejectedNarrativeWritesKeepExactNativePageInBothLanguages'):
 assert any(n.endswith('.'+method) for n in names),method
assert any(n.endswith('.LiveHud_RefreshesFromServiceChangesAndReentry') for n in names)
boundaries={c.get('fullname') for c in ET.parse(QA/'Boundaries/results.xml').findall('.//test-case')};assert len(boundaries)==10 and boundaries<=names
assert read('Boundaries/summary.json')['exitCode']==0
assert not list((ROOT/'Assets').glob('InitTestScene*.unity'))
artifact=read('ArtifactBefore.json');apk=ROOT/artifact['path'];assert sha(apk)==artifact['sha256'] and apk.stat().st_size==artifact['bytes']
assert Path(str(apk)+'.sha256').read_text()==artifact['sidecar']
with zipfile.ZipFile(apk) as z:
 metadata=z.read('assets/bin/Data/Managed/Metadata/global-metadata.dat')
 assert all((n+'\0').encode() not in metadata for n in ('SaveRejected',))
human=read('HumanRecordingReview/Review.json');assert human['status']=='needs_human_recording' and human['eligibleOutcomeCount']==0 and human['humanAcceptanceGranted'] is False
for r in human['files']:
 if 'sha256' in r:assert sha(Path(r['path']))==r['sha256']
for p in (BASE/'Sources').glob('*.py'):ast.parse(p.read_text(),filename=str(p))
subprocess.run(['git','diff','--check'],cwd=ROOT,capture_output=True,check=True)
result={'result':'Passed','freshUnityGroupCounts':counts,'unityPlayDistinctPassed':len(modes['PlayMode']),'unityEditDistinctPassed':len(modes['EditMode']),'unityTotalDistinctPassed':len(names),'failed':0,'skipped':0,'preFixFailures':5,'initialPostFixFocusedFixtureFailures':0,'initialNarrativeGroupingSkippedProfileDependentCase':1,'focused10CasesCountedWithinFinalGroups':True,'nativeRejectionScenarios':8,'changedProductionInputs':changed,'projectInputsUnchanged':7834,'addedProductionInputs':[],'deletedProductionInputs':[],'allOtherGameStateMethodsAndPreviousFixesBytePreserved':True,'routerOutsideAcknowledgementHandlersBytePreserved':True,'allExistingMetasArtPrefabsFontsSettingsPackagesBackendAndGuardsPreserved':True,'taskPatchReverseCheckPassed':True,'currentApkBytes':artifact['bytes'],'currentApkSha256':artifact['sha256'],'apkAndSidecarUnchanged':True,'narrativeFixNotYetPackaged':True,'human201To210EligibleOutcomes':0,'humanUserConfirmedNotRecorded':True,'manualExportsUnchanged':True,'normalPersonalJournalRead':False,'physicalGameplayTested':False,'newAndroidBuild':False,'phoneOrHostingAction':False,'gitCommitOrPushPerformed':False}
(QA/'TestReconciliation.json').write_text(json.dumps({'result':'Passed','groups':counts,'playPassed':len(modes['PlayMode']),'editPassed':len(modes['EditMode']),'totalDistinctPassed':len(names),'failed':0,'skipped':0,'focusedDuplicatesCountedAgain':False},indent=2)+'\n')
(QA/'SourceValidation.json').write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result,indent=2))
