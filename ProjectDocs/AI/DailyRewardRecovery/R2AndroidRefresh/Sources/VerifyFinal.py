"""Validate the completed R2 offline handoff without device or Unity execution."""
from pathlib import Path
import ast
import difflib
import hashlib
import json
import re
import subprocess

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'
read = lambda name: json.loads((QA / name).read_text())
build = read('BuildValidation.json')
assert build['result'] == 'Passed'
assert build['freshUnityEditDistinctPassed'] == 160
assert build['dailyRewardOverflowRejectionPackaged'] is True
boundaries = read('PreflightBoundaryChecks.json')
assert boundaries['result'] == 'Passed' and boundaries['passed'] == 52 and boundaries['failed'] == 0
assert len(boundaries['cases']) == len({r['name'] for r in boundaries['cases']}) == 52
assert all(r['result'] == 'Passed' for r in boundaries['cases'])
assert boundaries['realAdbInvocations'] == 0 and boundaries['realDeviceExecution'] is False
assert boundaries['syntheticFilesOnly'] is True
offline = read('OfflinePreflight.json')
assert offline['status'] == 'offline_prepared_device_deferred'
assert offline['localApkSha256'] == build['artifactSha256'] and offline['localApkBytes'] == build['artifactBytes']
assert all(offline[k] is True for k in ('fixedPresetsPackaged', 'matchesVerifiedPresetApk',
    'dailyRewardOverflowCatchPackaged', 'checksumSidecarMatches', 'deviceExecutionDeferred'))
assert all(offline[k] is False for k in ('deviceEnumerationAttempted', 'physicalGameplayTested',
    'appLaunched', 'appDataReadOrChanged', 'qaActivationVerified'))

# Verify current inputs against this task's baseline, which already included the R1 fix.
for relative, expected in read('BeforeHashes.json').items():
    with (ROOT / relative).open('rb') as stream:
        assert hashlib.file_digest(stream, 'sha256').hexdigest() == expected, relative
assert len(read('BeforeHashes.json')) == 7839

owned = []
for original in sorted((BASE / 'Sources/Before').rglob('*')):
    if not original.is_file():
        continue
    relative = original.relative_to(BASE / 'Sources/Before')
    current = ROOT / relative
    old, new = original.read_text(), current.read_text()
    assert old != new, relative
    if relative.name in ('CurrentTask.md', 'ConversationHandoff.md'):
        assert new.endswith(old), relative
    if relative.name in ('SessionLog.md', 'DevelopmentSchedule.md'):
        assert new.startswith(old.rstrip()), relative
    if current.suffix == '.py':
        ast.parse(new, filename=str(relative))
    added = [line[1:] for line in difflib.unified_diff(old.splitlines(), new.splitlines())
             if line.startswith('+') and not line.startswith('+++')]
    owned.append({'path': str(relative), 'changedLinesAdded': len(added)})
assert len(owned) == 8
subprocess.run(['git', 'diff', '--check'], cwd=ROOT, check=True, capture_output=True)
status = subprocess.check_output(['git', 'status', '--porcelain'], cwd=ROOT, text=True)
paths = [line[3:] for line in status.splitlines()]
start = (QA / 'WorkingTreeTaskStart.txt').read_text()
start_paths = {line[3:] for line in start.splitlines() if len(line) > 3}
protected = ('Assets/', 'Packages/', 'ProjectSettings/', 'Backend/', 'Library/', 'Temp/', 'obj/', 'Logs/', '.utmp/')
assert all(p in start_paths for p in paths if p.startswith(protected))
for pending in ('RuStoreReleaseReview/', 'StoryProgressionReview/',
                'AndroidGameplayProfile/R5AndroidRefresh/', 'ChapterOneHumanAcceptance/R2RecordingReview/'):
    assert any(pending in p for p in paths), pending

# Check only new/changed Markdown links; copied historical notes retain their original base.
checked = 0
for path in [BASE / 'TestResults.md', *[ROOT / r['path'] for r in owned if r['path'].endswith('.md')]]:
    content = path.read_text()
    snapshot = BASE / 'Sources/Before' / path.relative_to(ROOT)
    if snapshot.is_file():
        content = '\n'.join(line[1:] for line in difflib.unified_diff(snapshot.read_text().splitlines(), content.splitlines())
                            if line.startswith('+') and not line.startswith('+++'))
    for target in re.findall(r'\[[^\]]+\]\(([^)]+)\)', content):
        if re.match(r'^(?:https?://|#)', target):
            continue
        destination = (path.parent / target.split('#', 1)[0]).resolve()
        # The final report is written only after all checks pass.
        assert destination == (QA / 'FinalValidation.json').resolve() or destination.exists(), (path, target)
        checked += 1
for path in (BASE / 'Sources').glob('*.py'):
    ast.parse(path.read_text(), filename=str(path))

result = {'result': 'Passed', 'artifactSha256': build['artifactSha256'],
    'artifactBytes': build['artifactBytes'], 'buildReconciliationPassed': True,
    'freshUnityEditDistinctPassed': 160, 'offlineBoundaryChecksPassed': 52,
    'realQaAdbInvocations': 0, 'currentCanonicalPreflightPinsCompiledDailyRewardFix': True,
    'existingOwnedFiles': owned, 'preexistingSourceEditsAndReviewDirectoriesPreserved': True,
    'gitDiffWhitespacePassed': True, 'productionInputsUnchanged': 7839,
    'newOrChangedLocalMarkdownLinksChecked': checked, 'taskPythonSyntaxPassed': True,
    'phoneExecutionDeferred': True, 'hostingDeferred': True, 'physicalRowsPending': 18,
    'priorServiceUnityDistinctPassesHistorical': 21, 'newPlayModeRun': False,
    'effectsLast': True, 'gitCommitOrPushPerformed': False}
(QA / 'FinalValidation.json').write_text(json.dumps(result, indent=2) + '\n')
print(json.dumps(result, indent=2))
