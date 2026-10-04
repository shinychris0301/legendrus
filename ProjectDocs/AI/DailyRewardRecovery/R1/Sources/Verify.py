"""Tie the source-only daily-claim fix to fresh Unity results and the unchanged APK."""
from pathlib import Path
import ast
import difflib
import hashlib
import json
import re
import subprocess
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'
read = lambda name: json.loads((QA / name).read_text())

def sha(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()

before = read('BeforeHashes.json')
after = {str(p.relative_to(ROOT)): sha(p)
         for folder in ('Assets', 'Packages', 'ProjectSettings', 'Backend')
         for p in (ROOT / folder).rglob('*')
         if p.is_file() and '__pycache__' not in p.parts and p.suffix.lower() not in
         ('.keystore', '.jks', '.p12', '.pem', '.key') and not p.name.startswith('.env')}
changed = sorted(path for path in before if before[path] != after.get(path))
expected = ['Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs',
            'Assets/LegendsOfRus/Tests/PlayMode/DailyRewardAlignmentPlayModeTests.cs']
assert len(before) == len(after) == 7839 and set(before) == set(after)
assert changed == expected
scope = {'baselineFiles': 7839, 'finalFiles': 7839, 'unchanged': 7837,
    'changed': changed, 'added': [], 'deleted': [], 'allExistingMetadataUnchanged': True}
(QA / 'Scope.json').write_text(json.dumps(scope, indent=2) + '\n')
for path in expected:
    assert sha(BASE / 'Sources/Before' / path) == before[path]
source = (ROOT / expected[0]).read_text()
assert 'catch (OverflowException)' in source
assert 'return false;' in source[source.index('catch (OverflowException)', source.index('public bool ClaimDailyReward(out')):]
patch = (BASE / 'Sources/TaskChanges.patch').read_text()
subprocess.run(['git', 'apply', '--reverse', '--check', '-'], input=patch, text=True, cwd=ROOT, check=True, capture_output=True)

names = set()
for group, count in (('Edit', 7), ('Verify', 14)):
    summary = read(group + '/summary.json')
    cases = ET.parse(QA / group / 'results.xml').getroot().findall('.//test-case')
    assert summary['exitCode'] == 0 and len(cases) == count
    assert summary['result']['failed'] == summary['result']['skipped'] == '0'
    assert all(c.get('result') == 'Passed' for c in cases)
    names.update(c.get('fullname') for c in cases)
assert len(names) == 21
for method in ('OverflowClaimPreservesAllBalancesEligibilityAndJournal',
               'OverflowButtonShowsFailureAndRecoveredClaimOpensExactlyOneReceipt'):
    assert any(n.endswith('.' + method) for n in names)
assert read('Reproduce/summary.json')['exitCode'] == 1
repro = read('ReproduceVerified/summary.json')
assert repro['exitCode'] == 2 and repro['result']['failed'] == '1'
assert 'OverflowException' in repro['nonPassing'][0]['message']
assert 'GameState.ClaimDailyReward' in repro['nonPassing'][0]['message']
for group in ('Reproduce', 'ReproduceVerified', 'Verify', 'Edit'):
    config = read(group + '/summary.json')['isolatedConfig']
    assert config.startswith('/tmp/lor-daily-reward-recovery-r1/')
assert not list((ROOT / 'Assets').glob('InitTestScene*.unity'))

artifact = json.loads((ROOT / 'ProjectDocs/AI/AndroidGameplayProfile/R5AndroidRefresh/QA/Artifact.json').read_text())
apk = ROOT / artifact['path']
assert artifact['bytes'] == apk.stat().st_size == 766013502
assert artifact['sha256'] == sha(apk) == '91c38f1a6ae63482d8d9e713dcb8f0f891b9b0a67e8743b30de390ab906dc8a9'
assert apk.with_suffix('.apk.sha256').read_text().strip() == artifact['sha256'] + '  ' + apk.name
captures = QA / 'Verify/Captures/LOR_DAILY_REWARD_CAPTURE_DIR'
for language in ('ru', 'en'):
    assert (captures / ('DailyReward_OverflowRecovered_' + language + '.png')).is_file()
    record = json.loads((captures / ('DailyReward_OverflowRecovered_' + language + '.json')).read_text())
    assert record['capture'].startswith('Actual Unity Play Mode') and record['language'] == language

docs = ['CurrentTask.md', 'ConversationHandoff.md', 'SessionLog.md', 'DevelopmentSchedule.md']
link_count = 0
for path in [BASE / 'TestResults.md', *[ROOT / 'ProjectDocs/AI' / n for n in docs]]:
    current = path.read_text()
    backup = BASE / 'Sources/Before' / path.relative_to(ROOT)
    if backup.exists():
        previous = backup.read_text()
        assert current.endswith(previous) if path.name in ('CurrentTask.md', 'ConversationHandoff.md') else current.startswith(previous.rstrip())
        current = '\n'.join(line[1:] for line in difflib.unified_diff(previous.splitlines(), current.splitlines())
                            if line.startswith('+') and not line.startswith('+++'))
    for destination in re.findall(r'\[[^\]]+\]\(([^)]+)\)', current):
        if re.match(r'^(https?://|#)', destination):
            continue
        target = (path.parent / destination.split('#')[0]).resolve()
        assert target.exists() or target == QA / 'FinalValidation.json', destination
        link_count += 1
for path in (BASE / 'Sources').glob('*.py'):
    ast.parse(path.read_text())
subprocess.run(['git', 'diff', '--check'], cwd=ROOT, check=True, capture_output=True)
result = {'result': 'Passed', 'unityEditDistinctPassed': 7, 'unityPlayDistinctPassed': 14,
    'unityTotalDistinctPassed': 21, 'failed': 0, 'skipped': 0,
    'initialFixtureCompileErrorRetained': True, 'preFixOverflowRuntimeFailureReproduced': True,
    'syntheticArithmeticBoundariesInServiceTest': 7, 'nativeRetryAndReceiptLanguages': ['ru', 'en'],
    'projectInputsUnchanged': 7837, 'changedProductionInputs': expected, 'addedProductionInputs': [],
    'deletedProductionInputs': [], 'allExistingMetadataArtPrefabsSettingsPackagesGuardsPreserved': True,
    'sourceOnlyFixNotPackagedInCurrentApk': True, 'currentApkBytes': artifact['bytes'],
    'currentApkSha256': artifact['sha256'], 'sidecarUnchanged': True,
    'taskPatchReverseCheckPassed': True, 'newLocalLinksChecked': link_count,
    'gitDiffWhitespacePassed': True, 'phoneAndHostingDeferred': True, 'effectsLast': True,
    'newAndroidBuild': False, 'normalPersonalJournalRead': False,
    'human201To210AcceptanceStillPending': True, 'gitCommitOrPushPerformed': False}
(QA / 'FinalValidation.json').write_text(json.dumps(result, indent=2) + '\n')
print(json.dumps(result, indent=2))
