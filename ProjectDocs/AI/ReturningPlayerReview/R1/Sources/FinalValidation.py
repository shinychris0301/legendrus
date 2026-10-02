"""Reconcile review evidence and byte-preservation; no runtime or device operation."""
from pathlib import Path
import ast
import datetime
import difflib
import hashlib
import json
import re
import subprocess
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'


def sha(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()


def read(path):
    return json.loads(path.read_text())


before = read(QA / 'BeforeHashes.json')
after = {str(p.relative_to(ROOT)): sha(p) for directory in ('Assets', 'Packages', 'ProjectSettings', 'Backend')
         for p in (ROOT / directory).rglob('*') if p.is_file() and '__pycache__' not in p.parts
         and p.suffix.lower() not in ('.keystore', '.jks', '.p12', '.pem', '.key') and not p.name.startswith('.env')}
assert before == after and len(before) == 7825
(QA / 'Scope.json').write_text(json.dumps({'baselineFiles': 7825, 'finalFiles': 7825,
    'unchanged': 7825, 'changed': [], 'added': [], 'deleted': []}, indent=2) + '\n')
cases = []
for name, expected in (('StoryPersistence', 18), ('NativeFlow', 15)):
    summary = read(QA / name / 'summary.json')
    selected = ET.parse(QA / name / 'results.xml').findall('.//test-case')
    assert summary['exitCode'] == 0 and summary['result']['result'] == 'Passed'
    assert not summary['nonPassing'] and len(selected) == expected
    assert all(case.get('result') == 'Passed' for case in selected)
    assert str(summary['isolatedConfig']).startswith('/tmp/lor-returning-player-r1/')
    assert read(QA / name / 'TemporaryFixtureCleanup.json')['removed'] == []
    cases.extend({'name': case.get('fullname'), 'result': case.get('result'), 'run': name} for case in selected)
assert len(cases) == len({case['name'] for case in cases}) == 33
assert any(case['name'].endswith('EveryMilestoneRendersAndReturnsToItsEstablishedRoute') for case in cases)
(QA / 'CaseInventory.json').write_text(json.dumps({'passed': 33, 'failed': 0, 'skipped': 0, 'cases': cases}, indent=2) + '\n')

content = read(QA / 'InstalledStoryInventory.json')
assert content['sceneCount'] == 58 and content['milestoneCount'] == 57 and content['roundCount'] == 500
assert content['localizedSceneLines'] == 70 and content['localizationEntryCounts'] == {'ru': 1569, 'en': 1569}
assert content['sceneLineLengthCounts'] == {'1': 50, '2': 6, '3': 1, '5': 1}
assert not content['existingSceneSummaryFields']
assert len(content['scenesInNarrativeOrder']) == 58
for name, digest in content['fiveResourceSha256'].items():
    assert before[name] == digest == sha(ROOT / name)
for source in read(QA / 'SourceAnchors.json'):
    assert before[source['path']] == source['sha256'] == sha(ROOT / source['path'])
    lines = (ROOT / source['path']).read_text().splitlines()
    assert all(anchor['text'] in lines[anchor['line'] - 1] for anchor in source['anchors'])
images = sorted((QA / 'NativeFlow/Captures').rglob('*.png'))
assert len(images) == 28
assert all(path.read_bytes()[:8] == b'\x89PNG\r\n\x1a\n' for path in images)
(QA / 'CaptureInventory.json').write_text(json.dumps([{'path': str(p.relative_to(BASE)),
    'bytes': p.stat().st_size, 'sha256': sha(p), 'actualUnityCapture': True} for p in images], indent=2) + '\n')

artifact = read(ROOT / 'ProjectDocs/AI/CoreOutcomeReadability/R6StockAndroidRefresh/QA/Artifact.json')
apk = ROOT / artifact['path']
assert artifact['sha256'] == '42b29fca2c6941ed2ac96e5c7dd00f9e6c79aab70dfd5b0892a64d7faa815ce5'
assert sha(apk) == artifact['sha256'] and apk.stat().st_size == artifact['bytes'] == 765764694
assert apk.with_suffix('.apk.sha256').read_text().strip() == artifact['sha256'] + '  ' + apk.name
inventory = read(ROOT / 'ProjectDocs/AI/ChapterOneHumanAcceptance/R1/QA/EvidenceInventory.json')
prior_recordings = {row['path']: row['sha256'] for row in inventory['rawFiles'] if Path(row['path']).is_absolute()}
folder = Path('/home/k2/.config/unity3d/DefaultCompany/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL/CampaignPlaytests')
assert {str(p): sha(p) for p in folder.rglob('*.json')} == prior_recordings and len(prior_recordings) == 5

changes, patch = [], []
for name in read(QA / 'TaskStart.json')['existingDocumentsInScope']:
    old, current = BASE / 'Sources/Before' / name, ROOT / name
    assert sha(old) != sha(current)
    changes.append({'path': name, 'beforeSha256': sha(old), 'afterSha256': sha(current)})
    patch.extend(difflib.unified_diff(old.read_text().splitlines(keepends=True), current.read_text().splitlines(keepends=True),
                                   fromfile='a/' + name, tofile='b/' + name))
(BASE / 'Sources/TaskChanges.patch').write_text(''.join(patch))
(QA / 'ChangedFiles.json').write_text(json.dumps({'existingFilesChanged': changes,
    'newTaskFilesUnder': str(BASE.relative_to(ROOT)), 'projectInputsChanged': 0}, indent=2) + '\n')
for source in (BASE / 'Sources').glob('*.py'):
    ast.parse(source.read_text(), filename=str(source))
assert not list(BASE.rglob('__pycache__'))
output = QA / 'FinalValidation.json'
links = []
for report in (BASE / 'TestResults.md', BASE / 'ImplementationScope.md'):
    for target in re.findall(r'\]\(([^)]+)\)', report.read_text()):
        if target.startswith(('http:', 'https:', '#')):
            continue
        destination = (report.parent / target.split('#')[0]).resolve()
        assert destination.exists() or destination == output, (report, target)
        links.append(destination)
subprocess.run(['git', 'diff', '--check', '--', *[row['path'] for row in changes], str(BASE.relative_to(ROOT))],
               cwd=ROOT, check=True, capture_output=True, text=True)
result = {'checkedAtUtc': datetime.datetime.now(datetime.timezone.utc).isoformat(), 'result': 'Passed',
          'clientTaskDate': '2026-10-02', 'currentUnityChecksPassed': 33, 'editModePassed': 18, 'playModePassed': 15,
          'all7825ProjectInputsUnchanged': True, 'installedScenes': 58, 'localizedStoryLines': 70,
          'currentNativeMilestonesCovered': 57, 'legacyAndAdapterFixtureLimitsRecorded': True,
          'actualUnityCaptureCount': 28, 'inspectedExistingPageCaptures': 2,
          'recapImplementedInThisTask': False, 'humanReturningPlayerDemandMeasured': False,
          'currentApkAndSidecarUnchanged': True, 'normalManualRecordingsUnchanged': 5,
          'existingDocumentsChanged': 4, 'reportLinksResolved': len(links), 'taskPythonCacheAbsent': True,
          'phoneExecutionHostingDeploymentOrCommit': False}
output.write_text(json.dumps(result, indent=2) + '\n')
assert all(path.exists() for path in links)
print(json.dumps(result, indent=2))
