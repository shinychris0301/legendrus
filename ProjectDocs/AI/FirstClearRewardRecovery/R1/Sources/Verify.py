"""Reconcile the reproduced first-clear defects, current Unity checks and source-only scope."""
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
            'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs',
            'Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs',
            'Assets/LegendsOfRus/Tests/PlayMode/VictoryAlignmentPlayModeTests.cs']
assert len(before) == len(after) == 7839 and set(before) == set(after)
assert changed == expected
scope = {'baselineFiles': 7839, 'finalFiles': 7839, 'unchanged': 7835,
         'changed': changed, 'added': [], 'deleted': [], 'allExistingMetadataUnchanged': True}
(QA / 'Scope.json').write_text(json.dumps(scope, indent=2) + '\n')
for path in expected:
    assert sha(BASE / 'Sources/Before' / path) == before[path]
original = (BASE / 'Sources/Before' / expected[0]).read_text()
current = (ROOT / expected[0]).read_text()
start = '        public LevelCompletionResult CompleteLevel('
assert original.split(start)[0] == current.split(start)[0]
a, z = '        public bool TryDoubleLastLevelGoldReward()', '        public int CalculateTotalStars()'
assert original.split(a)[1].split(z)[0] == current.split(a)[1].split(z)[0]
assert original.split('        public void WinLevel(')[1] == current.split('        public void WinLevel(')[1]
patch = (BASE / 'Sources/TaskChanges.patch').read_text()
subprocess.run(['git', 'apply', '--reverse', '--check', '-'], input=patch, text=True,
               cwd=ROOT, check=True, capture_output=True)

reproduced = read('Reproduce/summary.json')
assert reproduced['exitCode'] == 2 and reproduced['result']['failed'] == '8'
assert len(reproduced['nonPassing']) == 8
assert all('Strings differ' in row['message'] for row in reproduced['nonPassing'])
names = set()
counts = {}
for group in ('Edit', 'Core', 'Retained', 'Error', 'Journeys', 'Recap', 'RecapQA'):
    summary = read(group + '/summary.json')
    cases = ET.parse(QA / group / 'results.xml').getroot().findall('.//test-case')
    assert summary['exitCode'] == 0 and summary['result']['failed'] == summary['result']['skipped'] == '0'
    assert all(c.get('result') == 'Passed' for c in cases)
    count = len({c.get('fullname') for c in cases})
    assert len(cases) == count
    counts[group] = count
    names.update(c.get('fullname') for c in cases)
assert counts == {'Edit': 47, 'Core': 56, 'Retained': 51, 'Error': 8, 'Journeys': 2, 'Recap': 5, 'RecapQA': 1}
assert len(names) == sum(counts.values())
assert read('PlayReconciliation.json')['playDistinctPassed'] == 123
assert read('PlayReconciliation.json')['result'] == 'Passed'
assert sum('.FirstClearArithmeticRejectionPreservesRewardsProgressAndJournal(' in name for name in names) == 7
for method in ('FirstClearBlockedSaveCannotPublishRewardsOrProgress',
               'FirstClearFullGrantFitsExactLimitsAndReplayCannotCreditAgain',
               'FirstClearExistingInvalidArgumentsAndNonpositiveRewardsRemainGuarded',
               'FirstClearOverflowNativeRetryCommitsExactlyOnceInBothLanguages',
               'FirstClearRetryRouteChangeRejectsStaleCompletedBoard',
               'FirstClearBlockedSaveDisablesRetryAndBackLeavesWithoutReward',
               'FirstClearOverflowCloseLeavesProgressEligibleAndUnchanged',
               'NativeJourneyWinsAndReturnsHome', 'NativeJourneyFailsRetriesAndExitsHome',
               'OverflowDoubleGoldCanRetryAtExactLimitAndPersistsOnce',
               'OverflowClaimPreservesAllBalancesEligibilityAndJournal',
               'RouteChangeCancelsPendingRetry'):
    assert any(name.endswith('.' + method) for name in names), method
for group in ('Reproduce', 'Focus', 'FinalPlay', 'Edit', 'Core', 'Error', 'Journeys', 'Recap', 'RecapQA'):
    assert read(group + '/summary.json')['isolatedConfig'].startswith('/tmp/lor-first-clear-reward-recovery-r1/')
assert not list((ROOT / 'Assets').glob('InitTestScene*.unity'))

artifact = read('ArtifactBefore.json')
apk = ROOT / artifact['path']
assert apk.stat().st_size == artifact['bytes'] == 766014334
assert sha(apk) == artifact['sha256'] == '9f72045fe3a2e68114be24d687dfb2a51c07c48d39ec58aa4afbafe9a5744654'
assert apk.with_suffix('.apk.sha256').read_text().strip() == artifact['sha256'] + '  ' + apk.name
captures = QA / 'Core/Captures/LOR_VICTORY_CAPTURE_DIR'
for language in ('ru', 'en'):
    name = 'Victory_FirstClearOverflowRecovered_' + language
    assert (captures / (name + '.png')).is_file()
    record = json.loads((captures / (name + '.json')).read_text())
    assert record['capture'].startswith('Actual Unity Play Mode') and record['language'] == language

checked = 0
for path in [BASE / 'TestResults.md', *[ROOT / 'ProjectDocs/AI' / n for n in
              ('CurrentTask.md', 'ConversationHandoff.md', 'SessionLog.md', 'DevelopmentSchedule.md')]]:
    current = path.read_text()
    backup = BASE / 'Sources/Before' / path.relative_to(ROOT)
    if backup.exists():
        old = backup.read_text()
        assert current.endswith(old) if path.name in ('CurrentTask.md', 'ConversationHandoff.md') else current.startswith(old.rstrip())
        current = '\n'.join(line[1:] for line in difflib.unified_diff(old.splitlines(), current.splitlines())
                            if line.startswith('+') and not line.startswith('+++'))
    for destination in re.findall(r'\[[^\]]+\]\(([^)]+)\)', current):
        if re.match(r'^(https?://|#)', destination):
            continue
        target = (path.parent / destination.split('#')[0]).resolve()
        assert target.exists() or target == QA / 'FinalValidation.json', destination
        checked += 1
for path in (BASE / 'Sources').glob('*.py'):
    ast.parse(path.read_text())
subprocess.run(['git', 'diff', '--check'], cwd=ROOT, check=True, capture_output=True)
assert read('Retained/summary.json')['isolatedConfig'].startswith('/tmp/lor-continuation-r1/FirstClearR1Retained/')
assert read('OptionsQA.json') == {'LOR_GAMEPLAY_QA': 'fresh'}
result = {'result': 'Passed', 'freshGroupDistinctCounts': counts, 'unityEditDistinctPassed': counts['Edit'],
    'unityPlayDistinctPassed': sum(v for k,v in counts.items() if k != 'Edit'), 'unityTotalDistinctPassed': len(names),
    'failed': 0, 'skipped': 0, 'preFixRuntimePartialMutationCases': 8,
    'arithmeticBoundaryCases': 7, 'nativeRecoveryLanguages': ['ru', 'en'], 'priorCombinedFixtureFailuresRetained': True,
    'sourceOnlyFixNotPackagedInCurrentApk': True, 'staleRetryAndExplicitExitVerified': True, 'currentApkBytes': artifact['bytes'],
    'currentApkSha256': artifact['sha256'], 'sidecarUnchanged': True,
    'projectInputsUnchanged': 7835, 'changedProductionInputs': expected,
    'addedProductionInputs': [], 'deletedProductionInputs': [],
    'allExistingMetadataArtPrefabsSettingsPackagesGuardsPreserved': True,
    'taskPatchReverseCheckPassed': True, 'newLocalLinksChecked': checked,
    'gitDiffWhitespacePassed': True, 'phoneAndHostingDeferred': True, 'effectsLast': True,
    'newAndroidBuild': False, 'normalPersonalJournalRead': False, 'nativeRussianRecoveryPortraitInspected': True,
    'human201To210AcceptanceStillPending': True, 'gitCommitOrPushPerformed': False}
(QA / 'FinalValidation.json').write_text(json.dumps(result, indent=2) + '\n')
print(json.dumps(result, indent=2))
