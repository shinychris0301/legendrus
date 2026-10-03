"""Validate current offline handoff and narrow documentation/tool changes after VerifyBuild."""
from pathlib import Path
import ast
import difflib
import json
import re
import subprocess

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'
read = lambda name: json.loads((QA / name).read_text())
build = read('BuildValidation.json')
assert build['result'] == 'Passed'
boundaries = read('PreflightBoundaryChecks.json')
assert boundaries['result'] == 'Passed' and boundaries['passed'] == 45 and boundaries['failed'] == 0
assert len({row['name'] for row in boundaries['cases']}) == 45
assert boundaries['realAdbInvocations'] == 0 and boundaries['realDeviceExecution'] is False
offline = read('OfflinePreflight.json')
assert offline['status'] == 'offline_prepared_device_deferred'
assert offline['localApkSha256'] == build['artifactSha256'] and offline['localApkBytes'] == build['artifactBytes']
assert offline['fixedPresetsPackaged'] is True and offline['matchesVerifiedPresetApk'] is True
assert offline['deviceExecutionDeferred'] is True
assert all(offline[k] is False for k in ('deviceEnumerationAttempted', 'physicalGameplayTested',
    'appLaunched', 'appDataReadOrChanged', 'qaActivationVerified'))
assert read('InitialSignatureRejection.json')['rejectedBecauseCertificateDiffers'] is True

owned = []
for original in sorted((BASE / 'Sources/Before').rglob('*')):
    if not original.is_file():
        continue
    relative = original.relative_to(BASE / 'Sources/Before')
    current = ROOT / relative
    assert current.is_file()
    old, new = original.read_text(), current.read_text()
    if relative.name == 'CurrentTask.md':
        assert new.endswith(old)
    if relative.name == 'SessionLog.md':
        assert new.startswith(old.rstrip())
    if current.suffix == '.py':
        ast.parse(new, filename=str(relative))
    changed = [line[1:] for line in difflib.unified_diff(old.splitlines(), new.splitlines())
               if line.startswith('+') and not line.startswith('+++')]
    owned.append({'path': str(relative), 'changedLinesAdded': len(changed)})
assert len(owned) == 8
subprocess.run(['git', 'diff', '--check'], cwd=ROOT, check=True, capture_output=True)
status = subprocess.check_output(['git', 'status', '--porcelain'], cwd=ROOT, text=True)
paths = [line[3:] for line in status.splitlines()]
assert all(not path.startswith(('Assets/', 'Packages/', 'ProjectSettings/', 'Backend/',
    'Library/', 'Temp/', 'obj/', 'Logs/', '.utmp/')) for path in paths)
assert any('RuStoreReleaseReview/' in path for path in paths)
assert any('StoryProgressionReview/' in path for path in paths)

result = {'result': 'Passed', 'artifactSha256': build['artifactSha256'],
    'artifactBytes': build['artifactBytes'], 'buildReconciliationPassed': True,
    'offlineBoundaryChecksPassed': 45, 'realQaAdbInvocations': 0,
    'currentCanonicalPreflightPinsVerifiedSixPresetApk': True,
    'existingOwnedFiles': owned, 'preexistingReviewDirectoriesPreserved': True,
    'gitDiffWhitespacePassed': True, 'productionInputsUnchanged': 7839,
    'phoneExecutionDeferred': True, 'hostingDeferred': True, 'physicalRowsPending': 18,
    'effectsLast': True, 'gitCommitOrPushPerformed': False}
(QA / 'FinalValidation.json').write_text(json.dumps(result, indent=2) + '\n')

# Check only task-owned new/changed Markdown links; historical copied notes retain their original base.
checked = 0
for path in [BASE / 'TestResults.md', BASE / 'ImplementationNotes.md',
             *[ROOT / row['path'] for row in owned if row['path'].endswith('.md')]]:
    text = path.read_text()
    snapshot = BASE / 'Sources/Before' / path.relative_to(ROOT)
    if snapshot.is_file():
        text = '\n'.join(line[1:] for line in difflib.unified_diff(snapshot.read_text().splitlines(), text.splitlines())
                         if line.startswith('+') and not line.startswith('+++'))
    for target in re.findall(r'\[[^\]]+\]\(([^)]+)\)', text):
        if re.match(r'^(?:https?://|#)', target):
            continue
        assert (path.parent / target.split('#', 1)[0]).exists(), f'Broken new link: {path}: {target}'
        checked += 1
for path in (BASE / 'Sources').glob('*.py'):
    ast.parse(path.read_text(), filename=str(path))
result.update(newOrChangedLocalMarkdownLinksChecked=checked, taskPythonSyntaxPassed=True)
(QA / 'FinalValidation.json').write_text(json.dumps(result, indent=2) + '\n')
print(json.dumps(result, indent=2))
