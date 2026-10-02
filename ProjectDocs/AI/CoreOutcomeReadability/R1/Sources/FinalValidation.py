"""Validate saved evidence and task links; does not launch Unity or build Android."""
import ast
import hashlib
import json
import re
import subprocess
import xml.etree.ElementTree as ET
from collections import Counter
from pathlib import Path
from urllib.parse import unquote

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'

def read_json(path):
    return json.loads(path.read_text())

def sha(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()

json_paths = [path for path in BASE.rglob('*.json') if path != QA / 'FinalValidation.json']
for path in json_paths:
    read_json(path)
xml_paths = list(BASE.rglob('*.xml'))
for path in xml_paths:
    ET.parse(path)
python_paths = list((BASE / 'Sources').glob('*.py'))
for path in python_paths:
    ast.parse(path.read_text(), filename=str(path))

tests = {}
for run, expected in [('EditVerified', 17), ('PlayFinal', 43)]:
    summary = read_json(QA / run / 'summary.json')
    tree = ET.parse(QA / run / 'results.xml')
    cases = tree.findall('.//test-case')
    assert len(cases) == expected
    assert all(case.attrib['result'] == 'Passed' for case in cases)
    assert len({case.attrib['fullname'] for case in cases}) == expected
    assert summary['exitCode'] == 0 and not summary['nonPassing']
    assert int(summary['result']['passed']) == expected
    assert summary['result']['failed'] == summary['result']['skipped'] == '0'
    tests[run] = expected

scope = read_json(QA / 'Scope.json')
assert (scope['baselineFiles'], scope['finalFiles'], scope['unchanged']) == (7792, 7798, 7783)
assert (len(scope['changed']), len(scope['added']), len(scope['deleted'])) == (9, 6, 0)
patch = QA / 'TaskChanges.patch'
subprocess.run(['git', 'apply', '--reverse', '--check', str(patch)], cwd=ROOT, check=True)
unity_empty_names = 0
current_file = ''
for line in patch.read_text().splitlines():
    if line.startswith('+++ '):
        current_file = line[4:]
    elif line.startswith('+') and line.rstrip() != line:
        # Unity emits an empty serialized string with a space after the colon.
        assert current_file.endswith('.prefab') and line == '+  m_Name: '
        unity_empty_names += 1

observations = read_json(QA / 'Measurements.json')
assert len(observations) == 56
assert len({(x['page'], x['state'], x['language'], x['width'], x['height'])
            for x in observations}) == 56
touch = read_json(QA / 'TouchMeasurements.json')
assert touch and all(x['touchWidth'] >= 43.9 and x['touchHeight'] >= 43.9 for x in touch)
review = (BASE / 'CoreOutcome_R1_Review.html').read_text()
assert review.count('<img ') == 1
review_paths = []
for run in ['Audit', 'PlayFinal']:
    for observation in observations:
        name = '_'.join(str(observation[key]) for key in ['page', 'state', 'language'])
        name += f"_{observation['width']}x{observation['height']}.png"
        path = QA / run / name
        assert path.is_file(), path
        assert path.read_bytes()[:8] == b'\x89PNG\r\n\x1a\n'
        review_paths.append(str(path.relative_to(BASE)))
assert len(list((QA / 'PlayFinal').rglob('*.png'))) == 129

comparison = read_json(QA / 'CaptureComparison.json')
assert comparison['pairedCount'] == 36 and comparison['pixelIdentityProvenCount'] == 16
assert sum(x['pixelIdentityProven'] for x in comparison['rows']) == 16
pair_counts = Counter()
for row in comparison['rows']:
    pair_counts[row['path'].split('/')[0]] += 1
    rel = Path(row['path']).with_suffix('.json')
    assert read_json(QA / 'Baseline' / rel) == read_json(QA / 'PlayFinal' / rel)
differences = read_json(QA / 'MapPixelComparison.json')['rows']
assert len(differences) == 20 and not any(x['decodedPixelIdentical'] for x in differences)
game = [x for x in differences if x['path'].startswith('GAMEPLAY/')]
victory = [x for x in differences if x['path'].startswith('VICTORY/')]
assert len(game) == 12 and len(victory) == 8
assert (min(x['changedPixels'] for x in game), max(x['changedPixels'] for x in game),
        max(x['maxChannelDifference'] for x in game)) == (21, 190, 186)
assert (min(x['changedPixels'] for x in victory), max(x['changedPixels'] for x in victory),
        max(x['maxChannelDifference'] for x in victory)) == (1, 72, 1)
assert all(x['pixelIdentityProven'] for x in comparison['rows']
           if x['path'].startswith('CHAPTER_COMPLETE/'))

links = []
report = BASE / 'TestResults.md'
documents = {report: report.read_text()}
ai = ROOT / 'ProjectDocs/AI'
documents[ai / 'CurrentTask.md'] = (ai / 'CurrentTask.md').read_text().split(
    '# 2026-10-01 — Home dormant artwork cleanup R17 complete')[0]
documents[ai / 'SessionLog.md'] = (ai / 'SessionLog.md').read_text().rsplit(
    '# 2026-10-01 — Core outcome readability and input R1 complete', 1)[1]
documents[ai / 'ConversationHandoff.md'] = (ai / 'ConversationHandoff.md').read_text().split(
    'Prepared 2026-09-12.')[0]
schedule = ai / 'DevelopmentSchedule.md'
schedule_lines = schedule.read_text().splitlines()
assert sum('CoreOutcomeReadability/R1/TestResults.md' in line for line in schedule_lines) == 1
documents[schedule] = '\n'.join(line for line in schedule_lines if 'CoreOutcomeReadability/' in line)
next_action = next(line for line in schedule_lines if line.startswith('**Immediate next action:**'))
assert 'Refresh the guarded development APK' in next_action
for path, content in documents.items():
    for target in re.findall(r'\[[^\]]*\]\(([^)]+)\)', content):
        if target.startswith(('http:', 'https:', '#')):
            continue
        resolved = path.parent / unquote(target.split('#')[0].strip('<>'))
        assert resolved.exists(), (path, target)
        links.append(target)
assert '21–190' in documents[report] and 'delta186/255' in documents[report]

apk = read_json(QA / 'ExistingApk.json')
apk_path = ROOT / apk['path']
assert apk_path.stat().st_size == apk['bytes'] == 765750206
assert sha(apk_path) == apk['sha256'] == 'c83940412715c9ebed87eb3e47b12bdd971cb8aae8a48735f015dd9ac9279fbe'
assert apk_path.with_suffix('.apk.sha256').read_text().split()[0] == apk['sha256']
assert not apk['includesThisTaskChanges'] and not apk['newAndroidBuildPerformed']
for path in ['Assets/LegendsOfRus/Tests/PlayMode/CoreOutcomeReadabilityAuditPlayModeTests.cs',
             'Assets/LegendsOfRus/Scripts/Editor/CoreOutcomeReadabilityPrefabAudit.cs']:
    assert not (ROOT / path).exists() and not (ROOT / (path + '.meta')).exists()

result = {
    'result': 'Passed',
    'validatedJsonFiles': len(json_paths),
    'validatedXmlFiles': len(xml_paths),
    'parsedPythonSources': len(python_paths),
    'finalUnityChecksReadFromXml': tests,
    'finalDistinctUnityChecks': sum(tests.values()),
    'temporaryDiagnosticChecksIncluded': 3,
    'temporaryDiagnosticAndInstallSourcesRemovedFromAssets': True,
    'reviewImagePathsValidated': len(review_paths),
    'standaloneReviewImageElements': 1,
    'finalUnityPngFiles': 129,
    'pairedCaptureCounts': dict(pair_counts),
    'pixelIdenticalPairs': 16,
    'allPairedLayoutJsonIdentical': True,
    'taskPatchReverseCheckPassed': True,
    'addedPatchLinesWhitespaceCheckPassed': True,
    'unitySerializedEmptyNameWhitespaceLines': unity_empty_names,
    'taskPatchSha256': sha(patch),
    'newDocumentationLinksValidated': len(links),
    'existingR17ApkAndSidecarVerified': True,
    'existingApkContainsUiChanges': False,
    'newUnityRunPerformedByThisScript': False,
    'androidBuildOrDeviceExecutionPerformedByThisScript': False,
    'scope': {'unchanged': 7783, 'changed': 9, 'added': 6, 'deleted': 0},
}
(QA / 'FinalValidation.json').write_text(json.dumps(result, indent=2) + '\n')
print(json.dumps(result, indent=2))
