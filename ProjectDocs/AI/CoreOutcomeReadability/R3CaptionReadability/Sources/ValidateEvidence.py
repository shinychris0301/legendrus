"""Validate final R3 evidence without launching Unity or building Android."""
from pathlib import Path
import ast
import difflib
import hashlib
import json
import re
import struct
import subprocess
import sys
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'
subprocess.run([sys.executable, str(BASE / 'Sources/RecordScope.py')], check=True, stdout=subprocess.DEVNULL)
scope = json.loads((QA / 'Scope.json').read_text())
json_count = 0
for path in BASE.rglob('*.json'):
    json.loads(path.read_text())
    json_count += 1
for path in (BASE / 'Sources').glob('*.py'):
    ast.parse(path.read_text(), filename=str(path))
xml_count = 0
for path in QA.rglob('*.xml'):
    ET.parse(path)
    xml_count += 1
passed = []
new_count = 0
for run, count in [('EditAccepted', 19), ('PlayAccepted', 25)]:
    summary = json.loads((QA / run / 'summary.json').read_text())
    tree = ET.parse(QA / run / 'results.xml').getroot()
    cases = tree.findall('.//test-case')
    assert summary['exitCode'] == 0 and not summary['nonPassing']
    assert int(tree.attrib['passed']) == count and len(cases) == count
    assert all(int(tree.attrib[key]) == 0 for key in ('failed', 'skipped', 'inconclusive'))
    for case in cases:
        assert case.attrib['result'] == 'Passed'
        name = case.attrib['fullname']
        passed.append(name)
        if ('CoreCaptionReadabilityEditModeTests' in name or
                'LargerBoosterNamesPreserveTheBoardAndForwardCaptionInput' in name or
                'LiveBilingualRewardCaptionsRetainAmountsAndUnavailableSemantics' in name):
            new_count += 1
assert len(set(passed)) == 44 and new_count == 6
rows = json.loads((QA / 'CaptionMeasurements.json').read_text())
assert len(rows) == 48
assert sum(row['page'] == 'Gameplay' for row in rows) == 8
assert sum(row['page'] == 'Victory' for row in rows) == 40
assert len({(r['page'], r['state'], r['language'], r['width']) for r in rows}) == 48
html = (BASE / 'CoreCaptions_R3_Review.html').read_text()
assert len(re.findall(r'<img\b', html)) == 1
review_rows = json.loads(re.search(r'const captures=(\[.*?\]);const states=', html).group(1))
assert {r['capture'] for r in rows} == {r['capture'] for r in review_rows}
for row in rows:
    with (BASE / row['capture']).open('rb') as stream:
        header = stream.read(24)
    assert header[:8] == b'\x89PNG\r\n\x1a\n'
    assert struct.unpack('>II', header[16:24]) == (row['width'], row['height'])
    for caption in row['captions']:
        assert caption['visibleCharacters'] > 0
        assert caption['effectiveFontPixels'] > 0
        if row['page'] == 'Gameplay':
            assert caption['meshScale'] >= .98
            if row['width'] == 360:
                assert caption['effectiveFontPixels'] >= 8.1
assert len(list((QA / 'PlayAccepted').rglob('*.png'))) == 105
apk_evidence = json.loads((QA / 'ExistingApk.json').read_text())
apk = ROOT / apk_evidence['path']
with apk.open('rb') as stream:
    apk_hash = hashlib.file_digest(stream, 'sha256').hexdigest()
assert apk.stat().st_size == apk_evidence['bytes'] == 765773174
assert apk_hash == apk_evidence['sha256'] == 'c065e5ab23f9404cb960660e4fd460ae57f15c4310a181022a65da094890fdb5'
assert apk.with_suffix('.apk.sha256').read_text().split()[0] == apk_hash
assert not apk_evidence['includesR3CaptionChanges']

# Check authored additions; Unity preserves empty serialized string fields with a space.
empty_yaml_spaces = 0
for relative in scope['changed'] + scope['added']:
    new = (ROOT / relative).read_text().splitlines()
    old_path = BASE / 'Sources/Before' / relative
    old = old_path.read_text().splitlines() if old_path.exists() else []
    matcher = difflib.SequenceMatcher(None, old, new)
    for operation, _, _, start, end in matcher.get_opcodes():
        if operation not in ('replace', 'insert'):
            continue
        for line in new[start:end]:
            if line != line.rstrip():
                assert relative.endswith('.prefab') and re.fullmatch(r' +[A-Za-z_][A-Za-z0-9_]*: ', line), (relative, line)
                empty_yaml_spaces += 1

documents = [(BASE / 'TestResults.md', (BASE / 'TestResults.md').read_text())]
docs_root = ROOT / 'ProjectDocs/AI'
current = (docs_root / 'CurrentTask.md').read_text()
documents.append((docs_root / 'CurrentTask.md', current.split('# 2026-10-01 — Core outcome Android refresh R2 complete')[0]))
session = (docs_root / 'SessionLog.md').read_text()
documents.append((docs_root / 'SessionLog.md', session[session.rindex('# 2026-10-01 — Core caption readability R3 complete'):]))
handoff = (docs_root / 'ConversationHandoff.md').read_text()
documents.append((docs_root / 'ConversationHandoff.md', handoff.split('Prepared 2026-09-12.')[0]))
schedule = (docs_root / 'DevelopmentSchedule.md').read_text()
documents.append((docs_root / 'DevelopmentSchedule.md', next(line for line in schedule.splitlines() if line.startswith('**2026-10-01 core caption readability completed:'))))
links = []
output = QA / 'FinalValidation.json'
for path, text in documents:
    assert all(line == line.rstrip() for line in text.splitlines())
    for target in re.findall(r'\[[^\]]+\]\(([^)]+)\)', text):
        if '://' not in target:
            resolved = (path.parent / target.split('#')[0]).resolve()
            assert resolved.exists() or resolved == output, (path, target)
            links.append(resolved)
assert not list((BASE / 'Sources').rglob('__pycache__'))
result = {
    'status': 'Passed', 'unityEditPassed': 19, 'unityPlayPassed': 25,
    'distinctUnityCases': 44, 'newPermanentCases': 6,
    'failed': 0, 'skipped': 0, 'measurementConfigurations': 48,
    'reviewPortraitElements': 1, 'reviewCapturesVerified': 48, 'finalUnityPngFiles': 105,
    'scopeRecomputed': scope, 'taskPatchReverseCheckPassed': True,
    'taskJsonParsedBeforeThisOutput': json_count, 'taskXmlParsed': xml_count,
    'sourcePythonAstParsed': True, 'newDocumentationLinksChecked': len(links),
    'authoredAdditionWhitespaceChecked': True,
    'allowedUnityEmptySerializedStringSpaces': empty_yaml_spaces,
    'existingR2ApkBytes': apk.stat().st_size, 'existingR2ApkSha256': apk_hash,
    'existingApkSidecarMatches': True, 'apkIncludesR3': False,
    'newUnityRunForEvidenceValidation': False, 'newAndroidBuild': False, 'physicalDeviceTest': False
}
output.write_text(json.dumps(result, indent=2) + '\n')
assert json.loads(output.read_text()) == result and all(path.exists() for path in links)
print(json.dumps({key: result[key] for key in ('status', 'distinctUnityCases', 'measurementConfigurations', 'reviewCapturesVerified', 'existingApkSidecarMatches')}, indent=2))
