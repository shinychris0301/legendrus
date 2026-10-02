"""Verify final broad/restart coverage, current inputs, artifacts and report links."""
from pathlib import Path
import ast
import datetime
import hashlib
import json
import re
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'

def read(path):
    return json.loads(path.read_text())

def sha(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()

evidence = [path for path in BASE.rglob('*.json') if path != QA / 'FinalValidation.json']
for path in evidence:
    read(path)
for path in BASE.rglob('*.xml'):
    ET.parse(path)
for path in (BASE / 'Sources').glob('*.py'):
    ast.parse(path.read_text(), filename=str(path))
run_inventory = read(QA / 'RunInventory.json')
latest = read(QA / 'LatestCases.json')
assert len({case['name'] for case in latest}) == len(latest)
assert run_inventory['latestCounts'].get('Failed', 0) == 0
expected_backend = {
    'LegendsOfRus.Tests.PlayMode.BackendSessionPlayModeTests.RealHttpBootstrapAndRefreshKeepIdentityAcrossClientRecreation',
    'LegendsOfRus.Tests.PlayMode.BackendSessionRestartPlayModeTests.ReadAndRecoverCommittedRotationAfterRestart',
    'LegendsOfRus.Tests.PlayMode.BackendSessionRestartPlayModeTests.WriteCommittedRotationWithLostResponse'
}
assert {case['name'] for case in run_inventory['latestNonPassing']} == expected_backend
assert all(case['result'] == 'Skipped' for case in run_inventory['latestNonPassing'])
broad = read(QA / 'Broad/summary.json')
assert broad['exitCode'] == 2 and int(broad['result']['failed']) == 2
assert {case['name'] for case in broad['nonPassing'] if case['result']=='Failed'} == {
    'LegendsOfRus.Tests.CampaignPlayMode.MechanicsSpectacleTests.EveryInventoryBoosterHasAVisibleReleaseAndKeepsMoveCost',
    'LegendsOfRus.Tests.PlayMode.AndroidInputCompatibilityPlayModeTests.QueuedTouchMouseAndEscapeUseNativeUiWithoutLegacyInput'}
assert int(broad['result']['skipped']) == 13
broad_cases = ET.parse(QA / 'Broad/results.xml').findall('.//test-case')
assert len(broad_cases) == len(latest)
phase_plan = read(BASE / 'Sources/PhasePlan.json')
assert len(phase_plan) == 17
for phase in phase_plan:
    summary = read(QA / phase['name'] / 'summary.json')
    assert summary['exitCode'] == 0 and not summary['nonPassing']
    assert summary['result']['result'] == 'Passed' and int(summary['result']['passed']) == 1
assert run_inventory['latestCounts']['Passed'] == int(broad['result']['passed']) + 12
assert run_inventory['latestCounts']['Skipped'] == 3
assemblies = read(QA / 'BroadAssemblyResults.json')
assert len(assemblies) == 8 and sum(int(row['failed']) for row in assemblies) == 2
for name,count in [('SpectacleVerified',14),('InputVerified',2)]:
    record=read(QA/name/'summary.json')
    assert record['exitCode']==0 and record['result']['passed']==str(count) and not record['nonPassing']
assert sum(int(row['passed']) for row in assemblies) == int(broad['result']['passed'])
assert any('LargerBoosterNamesPreserveTheBoardAndForwardCaptionInput' in case['name'] and case['result'] == 'Passed' and case['run'] == 'Broad' for case in latest)
assert any('LiveBilingualRewardCaptionsRetainAmountsAndUnavailableSemantics' in case['name'] and case['result'] == 'Passed' and case['run'] == 'Broad' for case in latest)
witness = read(QA / 'CampaignWitnessSummary.json')
assert witness['witnessCount'] == 1500 and witness['completeRoundSeedPairsVerified'] and witness['withinMoveLimit']
scope = read(QA / 'Scope.json')
assert scope['baselineFiles']==7821 and scope['unchanged']==7819
expected_changes={'Assets/LegendsOfRus/Tests/CampaignPlayMode/MechanicsSpectacleTests.cs',
                  'Assets/LegendsOfRus/Tests/PlayMode/AndroidInputCompatibilityPlayModeTests.cs'}
assert set(scope['changed'])==expected_changes and not scope['added'] and not scope['deleted']
before = read(BASE / 'Sources/BeforeHashes.json')
after = {str(path.relative_to(ROOT)): sha(path)
         for folder in ('Assets', 'Packages', 'ProjectSettings', 'Backend')
         for path in (ROOT / folder).rglob('*') if path.is_file()
         and '__pycache__' not in path.parts
         and path.suffix.lower() not in ('.keystore', '.jks', '.p12', '.pem', '.key')
         and not path.name.startswith('.env')}
assert set(before)==set(after)
assert {path for path in before if before[path]!=after[path]}==expected_changes
artifact = read(QA / 'ExistingApk.json')
apk = ROOT / artifact['path']
assert apk.stat().st_size == artifact['bytes'] == 765772146
assert sha(apk) == artifact['sha256'] == '924ecabcdbdb58f30512e6e2b134db4246be22c8f9ecc6b6086551e4ad937eff'
assert apk.with_suffix('.apk.sha256').read_text().split()[0] == artifact['sha256']
captures = read(QA / 'CaptureInventory.json')
assert len(captures) == len(list(QA.rglob('*.png')))
assert all((BASE / row['path']).is_file() for row in captures)
assert not list((BASE / 'Sources').rglob('__pycache__'))
assert not [path for path in (ROOT / 'Assets').glob('InitTestScene*') if str(path.relative_to(ROOT)) not in before]

ai = ROOT / 'ProjectDocs/AI'
report = BASE / 'TestResults.md'
documents = {report: report.read_text(), BASE / 'VisualSpotChecks.md': (BASE / 'VisualSpotChecks.md').read_text()}
documents[ai / 'CurrentTask.md'] = (ai / 'CurrentTask.md').read_text().split('# 2026-10-01 — Core caption Android refresh R4 complete')[0]
documents[ai / 'SessionLog.md'] = (ai / 'SessionLog.md').read_text().rsplit('# 2026-10-01 — Core gameplay regression R6 complete', 1)[1]
documents[ai / 'ConversationHandoff.md'] = (ai / 'ConversationHandoff.md').read_text().split('Prepared 2026-09-12.')[0]
schedule = ai / 'DevelopmentSchedule.md'
documents[schedule] = '\n'.join(line for line in schedule.read_text().splitlines() if 'CoreGameplayRegression/R6/' in line)
links = []
output = QA / 'FinalValidation.json'
for path, content in documents.items():
    assert all(line == line.rstrip() for line in content.splitlines())
    for target in re.findall(r'\]\(([^)]+)\)', content):
        if not target.startswith(('http:', 'https:', '#')):
            resolved = (path.parent / target.split('#')[0]).resolve()
            assert resolved.exists() or resolved == output, (path, target)
            links.append(resolved)
result = {
    'checkedAtUtc': datetime.datetime.now(datetime.timezone.utc).isoformat(), 'result': 'Passed',
    'jsonEvidenceFilesParsed': len(evidence), 'eightPlayModeAssembliesVerified': True,
    'initialBroadPassed': int(broad['result']['passed']), 'initialBroadFailed': 2, 'initialBroadConditionalSkips': 13,
    'seventeenSeparateRestartAndProfilePhasesPassed': True, 'distinctCurrentPassed': run_inventory['latestCounts']['Passed'],
    'distinctFailed': 0, 'deferredBackendCases': sorted(expected_backend),
    'all1500ReplayWitnessesWithinMoveLimit': True,
    'approvedCaptionCasesPassedInBroadSequence': True,
    '7819InputsByteIdenticalAndOnlyTwoFixturesChanged': True, 'singleCleanPostCorrectionBroadRunClaimed': False, 'existingR4ApkAndChecksumUnchanged': True,
    'sourceArtFontGuidPrefabSettingsPackagesBackendAndGuardsPreserved': True,
    'actualUnityPngFiles': len(captures), 'newDocumentationLinksResolved': len(links),
    'newAndroidBuildDeviceRunHostingOrCommit': False, 'humanDifficultyOrPacingAccepted': False
}
output.write_text(json.dumps(result, indent=2) + '\n')
assert read(output) == result and all(path.exists() for path in links)
print(json.dumps(result, indent=2))
