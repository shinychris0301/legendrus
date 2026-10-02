"""Reconcile the narrow recap implementation, native evidence and preserved inputs."""
from pathlib import Path
import ast
import datetime
import difflib
import hashlib
import json
import re
import struct
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


def write(path, value):
    path.write_text(json.dumps(value, indent=2) + '\n')


before = read(QA / 'BeforeHashes.json')
after = {str(p.relative_to(ROOT)): sha(p) for directory in ('Assets', 'Packages', 'ProjectSettings', 'Backend')
         for p in (ROOT / directory).rglob('*') if p.is_file() and '__pycache__' not in p.parts
         and p.suffix.lower() not in ('.keystore', '.jks', '.p12', '.pem', '.key') and not p.name.startswith('.env')}
changed = sorted(p for p in before if p in after and before[p] != after[p])
added = sorted(set(after) - set(before))
deleted = sorted(set(before) - set(after))
expected_changed = sorted(str(p.relative_to(BASE / 'Sources/Before')) for p in (BASE / 'Sources/Before/Assets').rglob('*') if p.is_file())
new_sources = ['Assets/LegendsOfRus/Scripts/Runtime/Services/StoryRecap.cs',
               'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionStoryRecapView.cs',
               'Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/StoryRecapEditModeTests.cs',
               'Assets/LegendsOfRus/Tests/PlayMode/StoryRecapPlayModeTests.cs']
assert changed == expected_changed and len(changed) == 6
assert added == sorted(new_sources + [p + '.meta' for p in new_sources]) and not deleted
assert len(before) == 7825 and len(after) == 7833
write(QA / 'Scope.json', {'baselineFiles': len(before), 'finalFiles': len(after),
                         'unchanged': len(before) - len(changed), 'changed': changed, 'added': added, 'deleted': deleted})
write(QA / 'AfterHashes.json', after)
prefab = 'Assets/LegendsOfRus/Resources/ProductionUI/Pages/Profile.prefab'
assert (ROOT / prefab).read_text() == (BASE / 'Sources/Before' / prefab).read_text().replace('  Revision: 12\n', '  Revision: 13\n')
assert read(ROOT / 'Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/ProfileCutoverState.txt')['revision'] == 13
assert 'CurrentRevision=13;' in (ROOT / 'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileView.cs').read_text()
assert '\\"revision\\": 13' in (ROOT / 'Assets/LegendsOfRus/Scripts/Editor/ProductionProfilePrefabBuilder.cs').read_text()
guids = {}
for path in (ROOT / 'Assets').rglob('*.meta'):
    found = re.search(r'^guid: ([0-9a-f]{32})$', path.read_text(), re.M)
    if found:
        guids.setdefault(found.group(1), []).append(str(path.relative_to(ROOT)))
for source in new_sources:
    guid = re.search(r'^guid: ([0-9a-f]{32})$', (ROOT / (source + '.meta')).read_text(), re.M).group(1)
    assert guids[guid] == [source + '.meta']

cases = {}
invocations = []
for name, expected in [('EditVerified', 27), ('NativeFinal', 21), ('QARecap', 1), ('NativeEntryVerified', 1)]:
    summary = read(QA / name / 'summary.json')
    selected = ET.parse(QA / name / 'results.xml').findall('.//test-case')
    assert summary['result']['result'] == 'Passed' and not summary['nonPassing'] and len(selected) == expected
    if name == 'EditVerified':
        assert summary['launcherExitCode'] == 143 and summary['unityTestRunnerAnnouncedExitCode'] == 0
        assert summary['recoveredAfterLauncherTermination']
    else:
        assert summary['exitCode'] == 0
    assert str(summary['isolatedConfig']).startswith('/tmp/lor-returning-player-recap-r2/')
    assert read(QA / name / 'TemporaryFixtureCleanup.json')['removed'] == []
    assert all(case.get('result') == 'Passed' for case in selected)
    for case in selected:
        cases[case.get('fullname')] = {'name': case.get('fullname'), 'result': 'Passed', 'run': name, 'mode': summary['mode']}
    invocations.append({'run': name, 'selected': expected, 'summary': str((QA / name / 'summary.json').relative_to(BASE))})
assert len(cases) == 49 and sum(c['mode'] == 'EditMode' for c in cases.values()) == 27
assert any(c.endswith('EveryMilestoneRendersAndReturnsToItsEstablishedRoute') for c in cases)
assert cases['LegendsOfRus.Tests.PlayMode.StoryRecapPlayModeTests.NativeReaderBrowsesAndReturnsWithoutChangingStoryOrJournal']['run'] == 'NativeEntryVerified'
write(QA / 'CaseInventory.json', {'distinctPassed': 49, 'failed': 0, 'skipped': 0,
                                 'invocations': invocations, 'cases': list(cases.values())})
images = sorted(p for run in ['NativeFinal', 'QARecap', 'NativeEntryVerified'] for p in (QA / run / 'Captures').rglob('*.png'))
assert len(images) == 77
image_rows = []
for path in images:
    with path.open('rb') as stream:
        header = stream.read(24)
    assert header[:8] == b'\x89PNG\r\n\x1a\n'
    width, height = struct.unpack('>II', header[16:24])
    image_rows.append({'path': str(path.relative_to(BASE)), 'width': width, 'height': height,
                       'bytes': path.stat().st_size, 'sha256': sha(path), 'actualUnityCapture': True,
                       'simulatedEditorViewport': True})
write(QA / 'CaptureInventory.json', image_rows)

artifact = read(ROOT / 'ProjectDocs/AI/CoreOutcomeReadability/R6StockAndroidRefresh/QA/Artifact.json')
apk = ROOT / artifact['path']
assert sha(apk) == artifact['sha256'] == '42b29fca2c6941ed2ac96e5c7dd00f9e6c79aab70dfd5b0892a64d7faa815ce5'
assert apk.stat().st_size == artifact['bytes'] == 765764694
assert apk.with_suffix('.apk.sha256').read_text().strip() == artifact['sha256'] + '  ' + apk.name
inventory = read(ROOT / 'ProjectDocs/AI/ChapterOneHumanAcceptance/R1/QA/EvidenceInventory.json')
recordings = {row['path']: row['sha256'] for row in inventory['rawFiles'] if Path(row['path']).is_absolute()}
folder = Path('/home/k2/.config/unity3d/DefaultCompany/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL/CampaignPlaytests')
assert {str(p): sha(p) for p in folder.rglob('*.json')} == recordings and len(recordings) == 5
content = read(ROOT / 'ProjectDocs/AI/ReturningPlayerReview/R1/QA/InstalledStoryInventory.json')
for path, digest in content['fiveResourceSha256'].items():
    assert before[path] == after[path] == digest

runtime_patch, docs_patch, rows = [], [], []
docs = sorted(str(p.relative_to(BASE / 'Sources/Before')) for p in (BASE / 'Sources/Before/ProjectDocs').rglob('*.md'))
assert len(docs) == 4
for path in changed + docs:
    original, current = BASE / 'Sources/Before' / path, ROOT / path
    assert sha(original) != sha(current)
    rows.append({'path': path, 'beforeSha256': sha(original), 'afterSha256': sha(current)})
    patch = runtime_patch if path in changed else docs_patch
    patch.extend(difflib.unified_diff(original.read_text().splitlines(keepends=True), current.read_text().splitlines(keepends=True),
                                   fromfile='a/' + path, tofile='b/' + path))
for path in added:
    runtime_patch.extend(difflib.unified_diff([], (ROOT / path).read_text().splitlines(keepends=True), fromfile='/dev/null', tofile='b/' + path))
(BASE / 'Sources/RuntimeChanges.patch').write_text(''.join(runtime_patch))
(BASE / 'Sources/ContinuityChanges.patch').write_text(''.join(docs_patch))
write(QA / 'ChangedFiles.json', {'existingFilesChanged': rows, 'addedProjectInputs': added,
                                'newTaskFilesUnder': str(BASE.relative_to(ROOT))})
for source in (BASE / 'Sources').glob('*.py'):
    ast.parse(source.read_text(), filename=str(source))
assert not list(BASE.rglob('__pycache__'))
output = QA / 'FinalValidation.json'
links = []
for report in [BASE / 'TestResults.md', BASE / 'ImplementationNotes.md', BASE / 'VisualSpotChecks.md']:
    for target in re.findall(r'\]\(([^)]+)\)', report.read_text()):
        if target.startswith(('http:', 'https:', '#')):
            continue
        destination = (report.parent / target.split('#')[0]).resolve()
        assert destination.exists() or destination in (output, QA / 'WhitespaceChecks.json'), (report, target)
        links.append(destination)
html = (BASE / 'StoryRecap_R2_Review.html').read_text()
assert len(re.findall(r'<img\b', html)) == 1
for target in re.findall(r'(?:src|href)="([^"]+)"', html):
    assert (BASE / target).resolve().exists()
# HEAD includes substantial inherited edits; check only this task's added lines against its snapshot.
inherited_check = subprocess.run(['git', 'diff', '--check', '--', *changed, *docs], cwd=ROOT, capture_output=True, text=True)
warnings = []
for location in re.findall(r'^(.+?):(\d+): trailing whitespace\.$', inherited_check.stdout, re.M):
    path, line = location[0], int(location[1])
    existing = BASE / 'Sources/Before' / path
    assert existing.exists() and (ROOT / path).read_text().splitlines()[line - 1] in existing.read_text().splitlines()
    warnings.append({'path': path, 'line': line, 'presentInTaskStartSnapshot': True})
assert inherited_check.returncode in (0, 2) and (inherited_check.returncode == 0 or warnings)
standard_meta_lines = []
for patch in [runtime_patch, docs_patch]:
    path = ''
    for line in patch:
        if line.startswith('+++ '):
            path = line[4:].strip()
        elif line.startswith('+') and line[1:].rstrip('\n\r') != line[1:].rstrip():
            assert path.endswith('.cs.meta') and line[1:].strip() in ('userData:', 'assetBundleName:', 'assetBundleVariant:')
            standard_meta_lines.append({'path': path, 'emptySerializedField': line[1:].strip()})
assert len(standard_meta_lines) == 12
for patch_name in ['RuntimeChanges.patch', 'ContinuityChanges.patch']:
    subprocess.run(['git', 'apply', '--check', '--reverse', str(BASE / 'Sources' / patch_name)], cwd=ROOT, check=True, capture_output=True, text=True)
write(QA / 'WhitespaceChecks.json', {'wholeHeadDiffExitCode': inherited_check.returncode,
                                    'inheritedWhitespaceWarnings': warnings,
                                    'taskAddedCodeAndDocumentLinesHaveNoTrailingWhitespace': True,
                                    'newUnityMetaStandardEmptyFields': standard_meta_lines,
                                    'bothTaskPatchesReverseApplyCheckPassed': True})
result = {'checkedAtUtc': datetime.datetime.now(datetime.timezone.utc).isoformat(), 'result': 'Passed',
          'clientTaskDate': '2026-10-02', 'currentDistinctUnityChecksPassed': 49, 'editModePassed': 27, 'playModePassed': 22,
          'editLauncherTerminationWithCompletedXmlRecorded': True, 'projectInputsUnchanged': 7819,
          'projectInputsChanged': 6, 'projectInputsAdded': 8, 'projectInputsDeleted': 0,
          'profileRevision': 13, 'profilePrefabOnlyRevisionChanged': True, 'newMetaGuidsUnique': True,
          'storyResourcesAndNormalStoryServicesPreserved': True, 'existingArtworkFontsAndMetaFilesPreserved': True,
          'currentApkAndSidecarUnchanged': True, 'normalManualRecordingsUnchanged': 5,
          'actualFinalUnityCaptureCount': len(images), 'standaloneReviewImageCount': 1,
          'existingContinuityDocumentsChanged': 4, 'reportLinksResolved': len(links),
          'taskPythonCacheAbsent': True, 'taskWhitespaceAndReversePatchChecksPassed': True,
          'inheritedWhitespaceWarningsPreserved': len(warnings), 'apkBuildPhoneExecutionHostingDeploymentOrCommit': False}
write(output, result)
assert all(path.exists() for path in links)
print(json.dumps(result, indent=2))
