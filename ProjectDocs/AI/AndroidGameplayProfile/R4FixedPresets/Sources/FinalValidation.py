"""Validate task-only scope against the inherited working tree, not Git HEAD."""
from pathlib import Path
import ast
import datetime
import difflib
import hashlib
import json
import re
import subprocess
import xml.etree.ElementTree as ET
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
def read(path): return json.loads(path.read_text())
def sha(path):
    with path.open('rb') as stream:return hashlib.file_digest(stream,'sha256').hexdigest()
before=read(BASE/'Sources/BeforeHashes.json')
after={str(path.relative_to(ROOT)):sha(path) for directory in ['Assets','Packages','ProjectSettings','Backend']
       for path in (ROOT/directory).rglob('*') if path.is_file() and '__pycache__' not in path.parts
       and path.suffix.lower() not in ['.keystore','.jks','.p12','.pem','.key'] and not path.name.startswith('.env')}
expectedChanged=['Assets/LegendsOfRus/Scripts/Runtime/Services/DevelopmentGameplayProfile.cs']
new=read(BASE/'Sources/NewSources.json')
expectedAdded=sorted(p for row in new for p in [row['path'],row['path']+'.meta'])
changed=sorted(p for p in before.keys() & after.keys() if before[p]!=after[p])
added=sorted(after.keys()-before.keys());deleted=sorted(before.keys()-after.keys())
assert len(before)==7833 and len(after)==7839 and changed==expectedChanged and added==expectedAdded and not deleted
allguids={}
for path in (ROOT/'Assets').rglob('*.meta'):
    match=re.search(r'^guid: ([0-9a-f]{32})$',path.read_text(),re.M)
    if match:allguids.setdefault(match.group(1),[]).append(str(path.relative_to(ROOT)))
for row in new:assert allguids[row['guid']]==[row['path']+'.meta']
scope={'baselineFiles':7833,'currentFiles':7839,'unchangedOriginalInputs':7832,'changed':changed,'added':added,'deleted':[],
       'allExistingMetasPrefabsArtFontsSettingsPackagesBackendAndOtherSourcePreserved':True,
       'newMetadataGuidsUnique':True,'preservedInheritedWorkingTree':True}
(QA/'Scope.json').write_text(json.dumps(scope,indent=2)+'\n')
for path in BASE.rglob('*.json'):read(path)
for path in BASE.rglob('*.xml'):ET.parse(path)
for path in (BASE/'Sources').glob('*.py'):ast.parse(path.read_text(),filename=str(path))
assert not list(BASE.rglob('__pycache__'))
r=read(QA/'Reconciliation.json')
assert r['result']=='Passed' and r['unitySeparateProcessesPassed']==21 and r['unitySuccessfulCaseExecutions']==77
assert r['compiledEntryGuardChecksPassed']==120 and len(r['implementedPresets'])==6
for row in r['implementedPresets']:assert row['sha256']==sha(BASE/row['dtoPath']) and row['implementedEditorCommand']
plan=read(BASE/'FixturePlan.json')
for path,expected in plan['installedSourceHashes'].items():assert sha(ROOT/path)==expected
assert len(plan['currentEditorCommands'])==9 and plan['currentApkCommands']==['fresh','resume','empty-energy']
assert plan['allDeviceRowsRemainPending'] and not plan['currentApkContainsProposedSelectors']
assert not plan['arbitraryQuantityJsonOrSaveImportSupported']
assert not [path for path in (ROOT/'Assets').glob('InitTestScene*') if str(path.relative_to(ROOT)) not in before]
artifact=read(QA/'ExistingApk.json');apk=ROOT/artifact['path']
assert apk.stat().st_size==artifact['bytes']==766017314
assert sha(apk)==artifact['sha256']=='2302b069bddf4b0261513fa5696bfa60dc9940f1c9bd61bc34a54c063079335e'
assert apk.with_suffix('.apk.sha256').read_text().strip()==artifact['sha256']+'  '+apk.name
manual=read(ROOT/'ProjectDocs/AI/ChapterOneHumanAcceptance/R1/QA/EvidenceInventory.json')
expected={row['path']:row['sha256'] for row in manual['rawFiles'] if Path(row['path']).is_absolute()}
folder=Path('/home/k2/.config/unity3d/DefaultCompany/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL/CampaignPlaytests')
assert expected=={str(path):sha(path) for path in folder.rglob('*.json')} and len(expected)==5
changes=[];patch=[]
oldfiles=sorted(path for path in (BASE/'Sources/Before').rglob('*') if path.is_file())
assert len(oldfiles)==7
for old in oldfiles:
    path=str(old.relative_to(BASE/'Sources/Before'));current=ROOT/path
    assert sha(old)!=sha(current),path
    changes.append({'path':path,'beforeSha256':sha(old),'afterSha256':sha(current)})
    patch.extend(difflib.unified_diff(old.read_text().splitlines(keepends=True),current.read_text().splitlines(keepends=True),fromfile='a/'+path,tofile='b/'+path))
for path in added:
    patch.extend(difflib.unified_diff([], (ROOT/path).read_text().splitlines(keepends=True),fromfile='/dev/null',tofile='b/'+path))
assert all(line[1:].rstrip('\r\n')==line[1:].rstrip() for line in patch if line.startswith('+') and not line.startswith('+++'))
patchpath=BASE/'Sources/TaskChanges.patch';patchpath.write_text(''.join(patch))
subprocess.run(['git','apply','--check','--reverse',str(patchpath)],cwd=ROOT,check=True,capture_output=True,text=True)
(QA/'ChangedFiles.json').write_text(json.dumps({'existingFilesChanged':changes,'newProjectInputs':added,'newTaskFilesUnder':str(BASE.relative_to(ROOT))},indent=2)+'\n')
for row in read(QA/'Captures.json'):assert sha(BASE/row['path'])==row['sha256']
visual=read(QA/'VisualInspection.json')
assert len(visual['inspected'])>=2 and visual['actualUnityCaptures'] and not visual['physicalPhoneAcceptance']
for row in visual['inspected']:assert sha(BASE/row['path'])==row['sha256']
output=QA/'FinalValidation.json';links=[]
for document in [BASE/'TestResults.md',BASE/'ImplementationNotes.md']:
    for target in re.findall(r'\]\(([^)]+)\)',document.read_text()):
        if target.startswith(('http:','https:','#')):continue
        path=(document.parent/target.split('#')[0]).resolve()
        assert path.exists() or path==output,(document,target)
        links.append(path)
result={'checkedAtUtc':datetime.datetime.now(datetime.timezone.utc).isoformat(),'clientTaskDate':'2026-10-02','result':'Passed',
        'originalProjectInputsPreserved':7832,'singleExistingSourceChanged':changed,'newSourceAndMetadataFiles':6,
        'newGuidUniquenessAndExistingMetasPreserved':True,'approvedSplashAndAllExistingPrefabsArtFontsSettingsPreserved':True,
        'sixProductSelectorsVerifiedAcrossColdUnityProcesses':True,'unityEditModePassed':50,'unityNativePlayModePassed':8,
        'unityColdProcessPhaseChecksPassed':19,'unityDistinctTestCasesPassed':r['unityDistinctTestCasesPassed'],
        'compiledEntryGuardChecksPassed':120,'fakeJniContentAndPreferences':True,'actualAndroidJniVerified':False,
        'normalSyntheticPreferenceFingerprintsPreserved':5,'normalManualRecorderFilesUnchanged':5,
        'existingR3ApkAndChecksumUnchanged':True,'currentApkIncludesNewSelectors':False,'originalCommandsPreserved':True,
        'initialReflectionFixtureFailureRetainedAndCorrected':True,'existingDocumentsChanged':6,'existingSourcesChanged':1,
        'reportLinksResolved':len(links),'nativeUnityPngCount':r['nativeUnityPngCount'],'actualPortraitsInspected':len(visual['inspected']),
        'newAndroidBuildDeviceAdbHostingDeploymentOrCommit':False,'humanPacingOrPhysicalAcceptance':False}
output.write_text(json.dumps(result,indent=2)+'\n');assert all(path.exists() for path in links)
print(json.dumps(result,indent=2))
