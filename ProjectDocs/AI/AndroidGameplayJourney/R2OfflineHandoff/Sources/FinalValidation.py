"""Reconcile current offline handoff, unchanged project/artifacts and preserved history."""
from pathlib import Path
import ast
import datetime
import difflib
import hashlib
import json
import re
import subprocess

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
scope = {'baselineFiles': len(before), 'finalFiles': len(after), 'unchanged': len(before),
         'changed': [], 'added': [], 'deleted': [], 'excludedCachesAndSecretExtensions': True}
(QA / 'Scope.json').write_text(json.dumps(scope, indent=2) + '\n')

build = ROOT / 'ProjectDocs/AI/CoreOutcomeReadability/R6StockAndroidRefresh/QA'
artifact = read(build / 'Artifact.json')
apk = ROOT / artifact['path']
assert artifact['sha256'] == '42b29fca2c6941ed2ac96e5c7dd00f9e6c79aab70dfd5b0892a64d7faa815ce5'
assert apk.stat().st_size == artifact['bytes'] == 765764694 and sha(apk) == artifact['sha256']
assert apk.with_suffix('.apk.sha256').read_text().strip() == artifact['sha256'] + '  ' + apk.name
prior = read(build / 'PreviousArtifact.json')
old = ROOT / prior['path']
assert sha(old) == prior['sha256'] == '924ecabcdbdb58f30512e6e2b134db4246be22c8f9ecc6b6086551e4ad937eff'
assert old.stat().st_size == prior['bytes'] == 765772146
assert old.with_suffix('.apk.sha256.inherited').read_text().split()[0] == prior['sha256']

offline = read(QA / 'OfflinePreflight.json')
assert offline['status'] == 'offline_prepared_device_deferred'
assert offline['matchesVerifiedR6Apk'] and offline['checksumSidecarMatches']
assert offline['localApkSha256'] == artifact['sha256'] and offline['localApkBytes'] == artifact['bytes']
assert offline['deviceExecutionDeferred'] and not offline['deviceEnumerationAttempted']
assert not any(offline[key] for key in ('physicalGameplayTested', 'appLaunched', 'appDataReadOrChanged',
                                      'qaActivationVerified', 'signatureRecheckedThisRun'))
checks = read(QA / 'PreflightBoundaryChecks.json')
assert checks['result'] == 'Passed' and checks['passed'] == 36 and checks['failed'] == 0
assert len(checks['cases']) == len({row['name'] for row in checks['cases']}) == 36
assert all(row['result'] == 'Passed' for row in checks['cases']) and checks['realAdbInvocations'] == 0

manifest = (QA / 'LocalManifest.txt').read_text()
for marker in ("name='com.example.legendsofrus.v117debug' versionCode='1' versionName='0.0.0'",
               "sdkVersion:'26'", "targetSdkVersion:'36'", 'application-debuggable',
               "launchable-activity: name='com.unity3d.player.UnityPlayerGameActivity'",
               "native-code: 'arm64-v8a'", "uses-feature: name='android.hardware.screen.portrait'"):
    assert marker in manifest, marker
history = read(ROOT / 'ProjectDocs/AI/AndroidGameplayJourney/R1/QA/DevicePreflight.json')
assert history['localApkSha256'] == '6a379c4671143b595d1d0e5ce731cf12d7c40104e8f3dbb3ffbfc26f48a761f0'
assert history['attachedCount'] == history['authorizedCount'] == 0
assert history['status'] == 'connect_one_authorized_device' and not history['physicalGameplayTested']

inventory = read(ROOT / 'ProjectDocs/AI/ChapterOneHumanAcceptance/R1/QA/EvidenceInventory.json')
prior_recordings = {row['path']: row['sha256'] for row in inventory['rawFiles'] if Path(row['path']).is_absolute()}
folder = Path('/home/k2/.config/unity3d/DefaultCompany/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL/CampaignPlaytests')
current_recordings = {str(p): sha(p) for p in folder.rglob('*.json')}
assert prior_recordings == current_recordings and len(current_recordings) == 5

changes = []
patch = []
for name in read(QA / 'TaskStart.json')['existingFilesInScope']:
    old_file, current = BASE / 'Sources/Before' / name, ROOT / name
    assert sha(old_file) != sha(current), name
    changes.append({'path': name, 'beforeSha256': sha(old_file), 'afterSha256': sha(current)})
    patch.extend(difflib.unified_diff(old_file.read_text().splitlines(keepends=True),
        current.read_text().splitlines(keepends=True), fromfile='a/' + name, tofile='b/' + name))
assert len(changes) == 7
(BASE / 'Sources/TaskChanges.patch').write_text(''.join(patch))
(QA / 'ChangedFiles.json').write_text(json.dumps({'existingFilesChanged': changes,
    'newTaskFilesUnder': str(BASE.relative_to(ROOT)), 'projectInputsChanged': 0}, indent=2) + '\n')

checklist = ROOT / 'ProjectDocs/AI/AndroidGameplayJourney/R1/DeviceChecklist.md'
usage = ROOT / 'ProjectDocs/AI/AndroidGameplayProfile/R1/Usage.md'
text = checklist.read_text()
rows = re.findall(r'^\| ([A-Z][A-Z0-9-]*-\d+) \|', text, re.M)
assert len(rows) == len(set(rows)) == 15 and all(text.count('| STOCK-' + suffix + ' |') == 1 for suffix in ('01', '02', '03'))
assert 'Pending fixture and phone evidence.' in text and 'cannot seed arbitrary booster quantities' in text
assert 'packaged Android profile accepts only' in text and '--device' in text
assert 'The previously installed APK does not contain this implementation.' not in usage.read_text()
assert 'This task does not implement Android gameplay save isolation.' not in text
for path in (BASE / 'Sources').glob('*.py'):
    ast.parse(path.read_text(), filename=str(path))
assert not list(BASE.rglob('__pycache__'))

output = QA / 'FinalValidation.json'
links = []
for report in (BASE / 'TestResults.md', checklist, usage):
    for target in re.findall(r'\]\(([^)]+)\)', report.read_text()):
        if target.startswith(('http:', 'https:', '#')):
            continue
        destination = (report.parent / target.split('#')[0]).resolve()
        assert destination.exists() or destination == output, (report, target)
        links.append(destination)
subprocess.run(['git', 'diff', '--check', '--', *[row['path'] for row in changes], str(BASE.relative_to(ROOT))],
               cwd=ROOT, check=True, capture_output=True, text=True)
result = {'checkedAtUtc': datetime.datetime.now(datetime.timezone.utc).isoformat(), 'result': 'Passed',
          'offlineBoundaryChecksPassed': 36, 'all7825ProjectInputsUnchanged': True,
          'apkAndSidecarUnchanged': True, 'archivedR4ApkAndSidecarVerified': True,
          'actualLocalManifestVerified': True, 'all15PhysicalRowsPending': True,
          'threeLargeStockRowsNeedDisposableFixture': True, 'historicalR1DeviceEvidenceRetained': True,
          'normalManualRecordingsUnchanged': 5, 'existingScopedFilesChanged': 7,
          'currentReportChecklistUsageLinksResolved': len(links), 'taskPythonCacheAbsent': True,
          'newUnityCompilationOrRuntimeTests': False, 'realAdbCalls': 0, 'phoneExecution': False,
          'hostingDeploymentOrCommit': False}
output.write_text(json.dumps(result, indent=2) + '\n')
assert all(path.exists() for path in links)
print(json.dumps(result, indent=2))
