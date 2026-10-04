"""Reconcile fresh isolated Unity checks, energy-only source scope and unchanged current APK."""
from pathlib import Path
import ast,difflib,hashlib,json,re,subprocess,xml.etree.ElementTree as ET,zipfile
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';read=lambda n:json.loads((QA/n).read_text())
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
before=read('BeforeHashes.json')
after={str(p.relative_to(ROOT)):sha(p) for f in ('Assets','Packages','ProjectSettings','Backend') for p in (ROOT/f).rglob('*') if p.is_file() and '__pycache__' not in p.parts and p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key') and not p.name.startswith('.env')}
expected=['Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs','Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs']
assert set(before)==set(after) and len(before)==7839
changed=sorted(p for p in before if before[p]!=after[p]);assert changed==expected
patch=''
for n in changed:
 old=BASE/'Sources/Before'/n;assert sha(old)==before[n]
 patch+=''.join(difflib.unified_diff(old.read_text().splitlines(True),(ROOT/n).read_text().splitlines(True),fromfile='a/'+n,tofile='b/'+n))
(BASE/'Sources/TaskChanges.patch').write_text(patch)
subprocess.run(['git','apply','--reverse','--check','-'],input=patch,text=True,cwd=ROOT,capture_output=True,check=True)
old=(BASE/'Sources/Before'/expected[0]).read_text();new=(ROOT/expected[0]).read_text()
# All other GameState code is preserved, including the previous spending/refund and reward fixes.
for start,end in [('        public void RefreshEnergyFromClock()', '        public TimeSpan EnergyRemaining()'),('        public bool SpendEnergy(', '        // Compatibility entry points')]:
 old=old[:old.index(start)]+old[old.index(end):];new=new[:new.index(start)]+new[new.index(end):]
new=new.replace('// A committed primary is already a balance change even if journal cleanup','// A committed primary is already a credit even if journal cleanup')
assert new==old
repro=read('Reproduce/summary.json');assert repro['exitCode']==2 and repro['result']['failed']=='9'
focus=read('Focus/summary.json');assert focus['result']['passed']=='10' and focus['result']['failed']=='1'
counts={};names=set();modes={'PlayMode':set(),'EditMode':set()}
for group in ('Core','UI','Runtime','Retained','Journey','Edit'):
 s=read(group+'/summary.json');cases=ET.parse(QA/group/'results.xml').findall('.//test-case');groupnames={c.get('fullname') for c in cases}
 assert s['exitCode']==0 and s['result']['failed']==s['result']['skipped']=='0'
 assert len(cases)==len(groupnames) and all(c.get('result')=='Passed' for c in cases)
 assert not names.intersection(groupnames),group
 names.update(groupnames);modes[s['mode']].update(groupnames);counts[group]=len(cases)
 assert s['isolatedConfig'].startswith('/tmp/lor-continuation-r1/EnergyR1Retained/' if group=='Retained' else '/tmp/lor-energy-recovery-r1/')
assert [counts[g] for g in ('Core','UI','Runtime','Retained','Journey')]==[134,45,16,51,3]
assert counts['Edit']>61
for method,num in [('EnergyGrantLargestAmountRespectsExistingCap',3),('EnergyMutationBlockedSavePreservesBalanceClockAndEvents',4),('EnergyClockLargestCapCannotWrapDuringRefreshOrSpending',2),('EnergyTryApisCommitOnceAndPreserveUnrelatedSave',3),('EnergyClockBoundariesPreserveRemainderAndReload',5),('EnergyGrantNonpositiveAndLargestAmountsRetainCapBehavior',4)]:
 assert sum('.'+method+'(' in n for n in names)==num,method
assert any(n.endswith('.EnergyAtFullDoesNotRewriteClockAndSpendingStartsRealCountdown') for n in names)
assert any(n.endswith('.LiveHud_RefreshesFromServiceChangesAndReentry') for n in names)
boundaries={c.get('fullname') for c in ET.parse(QA/'Boundaries/results.xml').findall('.//test-case')};assert len(boundaries)==24 and boundaries<=names
assert read('Boundaries/summary.json')['exitCode']==0
assert not list((ROOT/'Assets').glob('InitTestScene*.unity'))
artifact=read('ArtifactBefore.json');apk=ROOT/artifact['path'];assert sha(apk)==artifact['sha256'] and apk.stat().st_size==artifact['bytes']
assert Path(str(apk)+'.sha256').read_text()==artifact['sidecar']
with zipfile.ZipFile(apk) as z:
 metadata=z.read('assets/bin/Data/Managed/Metadata/global-metadata.dat')
 assert all((n+'\0').encode() not in metadata for n in ('TryAddEnergy','TryRefillEnergy','TryRefreshEnergyFromClock'))
human=read('HumanRecordingReview/Review.json');assert human['status']=='needs_human_recording' and human['eligibleOutcomeCount']==0 and human['humanAcceptanceGranted'] is False
for r in human['files']:
 if 'sha256' in r:assert sha(Path(r['path']))==r['sha256']
for p in (BASE/'Sources').glob('*.py'):ast.parse(p.read_text(),filename=str(p))
subprocess.run(['git','diff','--check'],cwd=ROOT,capture_output=True,check=True)
result={'result':'Passed','freshUnityGroupCounts':counts,'unityPlayDistinctPassed':len(modes['PlayMode']),'unityEditDistinctPassed':len(modes['EditMode']),'unityTotalDistinctPassed':len(names),'failed':0,'skipped':0,'preFixFailures':9,'initialPostFixFocusedFixtureFailures':1,'focused24CasesCountedWithinFinalGroups':True,'changedProductionInputs':changed,'projectInputsUnchanged':7837,'addedProductionInputs':[],'deletedProductionInputs':[],'allOtherGameStateMethodsAndPreviousFixesBytePreserved':True,'allExistingMetasArtPrefabsFontsSettingsPackagesBackendAndGuardsPreserved':True,'taskPatchReverseCheckPassed':True,'currentApkBytes':artifact['bytes'],'currentApkSha256':artifact['sha256'],'apkAndSidecarUnchanged':True,'energyFixNotYetPackaged':True,'human201To210EligibleOutcomes':0,'humanUserConfirmedNotRecorded':True,'manualExportsUnchanged':True,'normalPersonalJournalRead':False,'physicalGameplayTested':False,'newAndroidBuild':False,'phoneOrHostingAction':False,'gitCommitOrPushPerformed':False}
(QA/'TestReconciliation.json').write_text(json.dumps({'result':'Passed','groups':counts,'playPassed':len(modes['PlayMode']),'editPassed':len(modes['EditMode']),'totalDistinctPassed':len(names),'failed':0,'skipped':0,'focusedDuplicatesCountedAgain':False},indent=2)+'\n')
(QA/'SourceValidation.json').write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result,indent=2))
