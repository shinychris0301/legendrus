"""Reconcile current core/recap/restart evidence, preserved production inputs and APK identity."""
from pathlib import Path
import ast
import datetime
import difflib
import hashlib
import json
import re
import subprocess
import xml.etree.ElementTree as ET
from CheckScope import ROOT, BASE, inspect, sha

QA=BASE/'QA'


def read(path):
    return json.loads(path.read_text())


scope=inspect()
for path in BASE.rglob('*.json'):
    read(path)
for path in BASE.rglob('*.xml'):
    ET.parse(path)
for path in (BASE/'Sources').glob('*.py'):
    ast.parse(path.read_text(),filename=str(path))
inventory=read(QA/'RunInventory.json')
latest=read(QA/'LatestCases.json')
assert len({case['name'] for case in latest})==len(latest)
assert inventory['latestCounts'].get('Failed',0)==0
expected_backend={
    'LegendsOfRus.Tests.PlayMode.BackendSessionPlayModeTests.RealHttpBootstrapAndRefreshKeepIdentityAcrossClientRecreation',
    'LegendsOfRus.Tests.PlayMode.BackendSessionRestartPlayModeTests.ReadAndRecoverCommittedRotationAfterRestart',
    'LegendsOfRus.Tests.PlayMode.BackendSessionRestartPlayModeTests.WriteCommittedRotationWithLostResponse'}
assert {case['name'] for case in inventory['latestNonPassing']}==expected_backend
assert all(case['result']=='Skipped' for case in inventory['latestNonPassing'])
phases=read(BASE/'Sources/PhasePlan.json')
assert len(phases)==17
for phase in phases:
    summary=read(QA/phase['name']/'summary.json')
    assert summary['exitCode']==0 and not summary['nonPassing']
    assert summary['result']['result']=='Passed' and int(summary['result']['passed'])==1
for name,passed in [('QARecap',1),('EditRecap',27)]:
    summary=read(QA/name/'summary.json')
    assert summary['exitCode']==0 and not summary['nonPassing']
    assert summary['result']['result']=='Passed' and int(summary['result']['passed'])==passed
by_name={case['name']:case for case in latest}
qa_name='LegendsOfRus.Tests.PlayMode.StoryRecapPlayModeTests.DisposableRecapPreservesSyntheticNormalJournal'
assert by_name[qa_name]['result']=='Passed' and by_name[qa_name]['run']=='QARecap'
assert read(QA/'QARecap/summary.json')['phaseOverrides']['LOR_GAMEPLAY_QA']=='fresh'
broad=read(QA/'Broad/summary.json')
assert broad['exitCode'] is None and broad['launcherExitStatusUnavailableAfterInterruption']
assert broad['reportedUnityExitCode']==0
assert broad['unityCompletionMarker'] in (QA/'Broad/unity.log').read_text()
assert int(broad['result']['failed'])==0
broad_cases=ET.parse(QA/'Broad/results.xml').findall('.//test-case')
assert any(case.get('fullname')==qa_name and case.get('result')=='Skipped' for case in broad_cases)
assemblies=read(QA/'BroadAssemblyResults.json')
assert len(assemblies)==8 and sum(int(row['total']) for row in assemblies)==len(broad_cases)
assert len(latest)==len(broad_cases)+27
witness=read(QA/'CampaignWitnessSummary.json')
assert witness['witnessCount']==1500 and witness['completeRoundSeedPairsVerified'] and witness['withinMoveLimit']
artifact=read(QA/'ExistingApk.json');apk=ROOT/artifact['path']
assert artifact['bytes']==apk.stat().st_size==766017314
assert artifact['sha256']==sha(apk)=='2302b069bddf4b0261513fa5696bfa60dc9940f1c9bd61bc34a54c063079335e'
assert apk.with_suffix('.apk.sha256').read_text().strip()==artifact['sha256']+'  '+apk.name
manual=read(ROOT/'ProjectDocs/AI/ChapterOneHumanAcceptance/R1/QA/EvidenceInventory.json')
recordings={row['path']:row['sha256'] for row in manual['rawFiles'] if Path(row['path']).is_absolute()}
folder=Path('/home/k2/.config/unity3d/DefaultCompany/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL/CampaignPlaytests')
assert recordings=={str(path):sha(path) for path in folder.rglob('*.json')} and len(recordings)==5
before=read(BASE/'Sources/BeforeHashes.json')
assert not [path for path in (ROOT/'Assets').glob('InitTestScene*') if str(path.relative_to(ROOT)) not in before]
captures=read(QA/'CaptureInventory.json')
assert len(captures)==len(list(QA.rglob('*.png')))
assert all((BASE/row['path']).is_file() for row in captures)
changes=[];patch=[]
files=sorted(path for path in (BASE/'Sources/Before').rglob('*') if path.is_file())
assert len(files)==5
for old in files:
    path=str(old.relative_to(BASE/'Sources/Before'));current=ROOT/path
    assert sha(old)!=sha(current),path
    changes.append({'path':path,'beforeSha256':sha(old),'afterSha256':sha(current)})
    patch.extend(difflib.unified_diff(old.read_text().splitlines(keepends=True),current.read_text().splitlines(keepends=True),
                                   fromfile='a/'+path,tofile='b/'+path))
assert all(line[1:].rstrip('\r\n')==line[1:].rstrip() for line in patch if line.startswith('+') and not line.startswith('+++'))
patch_path=BASE/'Sources/TaskChanges.patch';patch_path.write_text(''.join(patch))
subprocess.run(['git','apply','--check','--reverse',str(patch_path)],cwd=ROOT,check=True,capture_output=True,text=True)
(QA/'ChangedFiles.json').write_text(json.dumps({'existingFilesChanged':changes,'newTaskFilesUnder':str(BASE.relative_to(ROOT))},indent=2)+'\n')
output=QA/'FinalValidation.json';links=[]
for document in [BASE/'TestResults.md',BASE/'VisualSpotChecks.md']:
    for target in re.findall(r'\]\(([^)]+)\)',document.read_text()):
        if target.startswith(('http:','https:','#')):
            continue
        path=(document.parent/target.split('#')[0]).resolve()
        assert path.exists() or path==output,(document,target)
        links.append(path)
edit_cases=[case for case in latest if case['run']=='EditRecap']
assert len(edit_cases)==27 and all(case['result']=='Passed' for case in edit_cases)
result={'checkedAtUtc':datetime.datetime.now(datetime.timezone.utc).isoformat(),'result':'Passed','clientTaskDate':'2026-10-02',
        'eightPlayModeAssembliesVerified':True,'broadPassed':int(broad['result']['passed']),
        'broadFailed':int(broad['result']['failed']),'broadSkipped':int(broad['result']['skipped']),
        'broadLauncherExitStatusUnavailable':True,'broadCompletedXmlAndUnityExit0Verified':True,
        'seventeenSeparateRestartProfilePhasesPassed':True,'separateFreshQaRecapPassed':True,
        'targetedEditModePassed':27,'latestDistinctPassed':inventory['latestCounts']['Passed'],
        'latestDistinctFailed':0,'deferredBackendCases':sorted(expected_backend),
        'all1500ReplayWitnessesWithinMoveLimit':True,'unchangedProjectInputs':7832,'changedFixtureFiles':1,
        'productionArtFontsMetasGuidsPrefabsSettingsPackagesBackendGuardsPreserved':True,
        'currentR3ApkAndChecksumUnchanged':True,'normalManualRecordingsUnchanged':5,
        'actualUnityPngFiles':len(captures),'existingFilesChanged':5,'reportLinksResolved':len(links),
        'newAndroidBuildDeviceRunHostingOrCommit':False,'humanDifficultyOrPacingAccepted':False}
output.write_text(json.dumps(result,indent=2)+'\n')
assert all(path.exists() for path in links)
print(json.dumps(result,indent=2))
