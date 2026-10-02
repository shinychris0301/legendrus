from pathlib import Path
import ast
import datetime
import difflib
import hashlib
import json
import re
import subprocess
import xml.etree.ElementTree as ET

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
QA=BASE/'QA'
def read(path): return json.loads(path.read_text())
def sha(path):
    with path.open('rb') as stream:return hashlib.file_digest(stream,'sha256').hexdigest()
before=read(BASE/'Sources/BeforeHashes.json')
after={str(path.relative_to(ROOT)):sha(path) for directory in ['Assets','Packages','ProjectSettings','Backend']
       for path in (ROOT/directory).rglob('*') if path.is_file() and '__pycache__' not in path.parts
       and path.suffix.lower() not in ['.keystore','.jks','.p12','.pem','.key'] and not path.name.startswith('.env')}
assert len(before)==len(after)==7833 and before==after
scope={'baselineFiles':7833,'unchanged':7833,'changed':[],'added':[],'deleted':[],
       'allProductionAndExistingTestInputsPreserved':True}
(QA/'Scope.json').write_text(json.dumps(scope,indent=2)+'\n')
for path in BASE.rglob('*.json'):read(path)
for path in BASE.rglob('*.xml'):ET.parse(path)
for path in (BASE/'Sources').glob('*.py'):ast.parse(path.read_text(),filename=str(path))
assert not list(BASE.rglob('__pycache__'))
reconciled=read(QA/'Reconciliation.json')
assert reconciled['result']=='Passed' and reconciled['unitySeparateProcessesPassed']==14
assert reconciled['compiledEntryGuardChecksPassed']==34 and len(reconciled['rehearsedPresets'])==6
for row in reconciled['rehearsedPresets']:
    assert row['sha256']==sha(BASE/row['dtoPath']) and not row['implementedPlayerCommand']
plan=read(BASE/'FixturePlan.json')
for path,expected in plan['installedSourceHashes'].items():assert sha(ROOT/path)==expected
assert plan['currentPlayerCommands']==['fresh','resume','empty-energy'] and plan['allDeviceRowsRemainPending']
assert not plan['currentApkContainsProposedSelectors'] and not plan['arbitraryQuantityJsonOrSaveImportSupported']
cleanup=read(QA/'TemporaryFixtureCleanup.json')
assert len(cleanup['removedExactOwnedFiles'])==2 and cleanup['noUnityProcessDuringCleanup']
assert all(not (ROOT/path).exists() for path in cleanup['removedExactOwnedFiles'])
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
assert len(oldfiles)==6
for old in oldfiles:
    path=str(old.relative_to(BASE/'Sources/Before'));current=ROOT/path
    assert sha(old)!=sha(current),path
    changes.append({'path':path,'beforeSha256':sha(old),'afterSha256':sha(current)})
    patch.extend(difflib.unified_diff(old.read_text().splitlines(keepends=True),current.read_text().splitlines(keepends=True),fromfile='a/'+path,tofile='b/'+path))
assert all(line[1:].rstrip('\r\n')==line[1:].rstrip() for line in patch if line.startswith('+') and not line.startswith('+++'))
patchpath=BASE/'Sources/TaskChanges.patch';patchpath.write_text(''.join(patch))
subprocess.run(['git','apply','--check','--reverse',str(patchpath)],cwd=ROOT,check=True,capture_output=True,text=True)
(QA/'ChangedFiles.json').write_text(json.dumps({'existingFilesChanged':changes,'newTaskFilesUnder':str(BASE.relative_to(ROOT))},indent=2)+'\n')
output=QA/'FinalValidation.json';links=[]
for document in [BASE/'TestResults.md',BASE/'FixtureScope.md']:
    for target in re.findall(r'\]\(([^)]+)\)',document.read_text()):
        if target.startswith(('http:','https:','#')):continue
        path=(document.parent/target.split('#')[0]).resolve()
        assert path.exists() or path==output,(document,target)
        links.append(path)
result={'checkedAtUtc':datetime.datetime.now(datetime.timezone.utc).isoformat(),'clientTaskDate':'2026-10-02','result':'Passed',
        'all7833ProjectInputsByteIdentical':True,'temporaryPrototypeAndMetadataRemoved':True,
        'sixReferenceFixturesVerifiedAcrossColdUnityProcesses':True,'fourteenUnityProcessChecksPassed':True,
        'twoNativeHistoryRecapReturnsPreservePageAndJournal':True,'firstClearReplayGrantsRejected':True,
        'compiledEntryGuardChecksPassed':34,'fakeJniAndPreferences':True,'actualAndroidJniVerified':False,
        'normalSyntheticPreferenceFingerprintsPreserved':5,'normalManualRecorderFilesUnchanged':5,
        'existingR3ApkAndChecksumUnchanged':True,'currentPlayerCommandSetUnchanged':True,
        'initialTemporaryCompilerFailureRetainedAndCorrected':True,'existingDocumentsChanged':6,
        'reportLinksResolved':len(links),'noNewEditModeTestSuite':True,
        'newAndroidBuildDeviceAdbHostingDeploymentOrCommit':False,'humanPacingOrPhysicalAcceptance':False}
output.write_text(json.dumps(result,indent=2)+'\n');assert all(path.exists() for path in links)
print(json.dumps(result,indent=2))
