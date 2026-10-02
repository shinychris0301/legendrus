from pathlib import Path
import ast
import csv
import hashlib
import json
import re
import shutil

BASE = Path(__file__).resolve().parent.parent
ROOT = BASE.parents[3]


def sha(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()


before = json.loads((BASE / 'Sources/BeforeHashes.json').read_text())
after = {str(p.relative_to(ROOT)): sha(p) for directory in ['Assets', 'Packages', 'ProjectSettings', 'Backend']
         for p in (ROOT / directory).rglob('*') if p.is_file() and '__pycache__' not in p.parts and
         p.suffix.lower() not in ('.keystore', '.jks', '.p12', '.pem', '.key') and not p.name.startswith('.env')}
scope = {'baselineFiles': len(before), 'unchanged': sum(v == after.get(k) for k, v in before.items()),
         'changed': sorted(k for k in before.keys() & after.keys() if before[k] != after[k]),
         'added': sorted(after.keys() - before.keys()), 'deleted': sorted(before.keys() - after.keys())}
assert before == after and len(before) == 7821, scope
(BASE / 'QA/Scope.json').write_text(json.dumps(scope, indent=2) + '\n')
inventory = json.loads((BASE / 'QA/EvidenceInventory.json').read_text())
default = Path('/home/k2/.config/unity3d/DefaultCompany/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL/CampaignPlaytests')
original = {f['path']: f['sha256'] for f in inventory['rawFiles'] if Path(f['path']).is_absolute()}
current = {str(p): sha(p) for p in default.rglob('*') if p.is_file() and
           (p.name.endswith('.json') or p.name.endswith('.json.bak'))}
assert current == original, 'Normal player recordings changed during audit'
for session in inventory['sessions']:
    if session['source'] == 'manual_campaign':
        saved = BASE / 'Sources/ManualSessions' / ('playtest-' + session['sessionId'] + '.json')
        assert sha(saved) == session['sha256']
matrix = json.loads((BASE / 'CoverageMatrix.json').read_text())
assert [r['round'] for r in matrix['rounds']] == list(range(31, 501))
assert sorted(b['start'] for b in matrix['blocks']) == list(range(31, 501, 10))
assert len(matrix['rounds']) == 470 and len(matrix['blocks']) == 47
assert [b['start'] for b in matrix['blocks'][:6]] == [201, 211, 221, 231, 241, 491]
for name, count in [('CoverageMatrix.csv', 470), ('CollectionQueue.csv', 47), ('BlockObservations.csv', 47)]:
    with (BASE / name).open(newline='') as stream:
        assert len(list(csv.DictReader(stream))) == count
with (BASE / 'BlockObservations.csv').open(newline='') as stream:
    for row in csv.DictReader(stream):
        assert all(not value for key, value in row.items() if key not in ('priority', 'block_start', 'block_end'))
html = (BASE / 'CoverageReview.html').read_text()
assert json.loads(re.search(r'<script id="data" type="application/json">(.*?)</script>', html, re.S).group(1)) == matrix
for target in re.findall(r'href="([^"]+)"', html):
    assert (BASE / target).is_file(), target
browser = json.loads((BASE / 'QA/Browser/Checks.json').read_text())
assert browser['passed'] == 7 and browser['failed'] == 0 and not browser['errors']
assert 'Ran 8 tests' in (BASE / 'QA/ClassificationChecks.txt').read_text()
assert (BASE / 'QA/ClassificationChecks.txt').read_text().rstrip().endswith('OK')
for path in (BASE / 'Sources').glob('*.py'):
    ast.parse(path.read_text())
apk_before = json.loads((BASE / 'Sources/ApkBefore.json').read_text())
apk = ROOT / apk_before['path']
assert apk.stat().st_size == apk_before['bytes'] and sha(apk) == apk_before['sha256']
assert Path(str(apk) + '.sha256').read_text().strip() == apk_before['sidecar']
assert apk_before['sha256'] in apk_before['sidecar']
cache = BASE / 'Sources/__pycache__'
if cache.exists():
    shutil.rmtree(cache)
assert not list(BASE.rglob('*.pyc'))
report = {'result': 'Passed', 'unchangedProjectInputs': len(before), 'unchangedNormalRecorderFiles': len(current),
          'manualSnapshotsVerified': 5, 'matrixRows': 470, 'queueBlocks': 47,
          'classificationScenariosPassed': 8, 'browserChecksPassed': 7,
          'apkBytes': apk.stat().st_size, 'apkSha256': apk_before['sha256'], 'checksumSidecarUnchanged': True,
          'temporaryHelperRemoved': not (ROOT / 'Assets/LegendsOfRus/Scripts/Editor/ChapterOneAcceptanceAudit.cs').exists(),
          'newHumanOutcomes': 0, 'scope': 'Evidence/report validation; no new gameplay acceptance'}
assert report['temporaryHelperRemoved']
(BASE / 'QA/FinalValidation.json').write_text(json.dumps(report, indent=2) + '\n')
print(json.dumps(report, indent=2))
