"""Check the precise installed UI additions and preserve all other packed membership."""
from collections import Counter
from pathlib import Path
import json

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'
before = json.loads((ROOT / 'ProjectDocs/AI/AndroidPackageFootprint/R17HomeDormantArtworkCleanup/QA/PackedAssets.json').read_text())['assets']
record = json.loads((QA / 'PackedAssets.json').read_text())
assert record['result'] == 'Succeeded'
after = record['assets']
script = 'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionControlTouchTarget.cs'
pages = {page: 'Assets/LegendsOfRus/Resources/ProductionUI/Pages/' + page + '.prefab'
         for page in ['Gameplay', 'Victory', 'ChapterComplete']}
key = lambda row: (row['path'], row['guid'], row['type'])
new_script = [row for row in after if row['path'] == script]
assert len(new_script) == 1 and new_script[0]['type'] == 'MonoScript'
meta = (ROOT / (script + '.meta')).read_text().splitlines()
guid = next(line.split(': ', 1)[1] for line in meta if line.startswith('guid: '))
assert new_script[0]['guid'] == guid
assert not any(row['path'] == script for row in before)
allowed = set(pages.values()) | {script}
assert Counter(key(row) for row in before if row['path'] not in allowed) == Counter(
    key(row) for row in after if row['path'] not in allowed)
page_deltas = {}
for page, path in pages.items():
    old = [row for row in before if row['path'] == path]
    new = [row for row in after if row['path'] == path]
    assert {row['guid'] for row in old} == {row['guid'] for row in new}
    delta = Counter(row['type'] for row in new)
    delta.subtract(Counter(row['type'] for row in old))
    expected = {'GameObject': 1, 'CanvasRenderer': 1, 'RectTransform': 1, 'MonoBehaviour': 5} if page == 'Gameplay' else {
        'GameObject': 2, 'CanvasRenderer': 2, 'RectTransform': 2, 'MonoBehaviour': 2}
    assert dict(delta) == expected, (page, dict(delta))
    page_deltas[page] = {'beforeObjects': len(old), 'afterObjects': len(new),
        'addedObjectTypes': expected, 'packedObjectByteDelta': sum(row['bytes'] for row in new) - sum(row['bytes'] for row in old)}
assert len(after) - len(before) == 25
result = {'beforeRows': len(before), 'afterRows': len(after), 'addedRows': 25,
    'removedPaths': [], 'addedPaths': [script], 'newScript': new_script[0],
    'pageDeltas': page_deltas, 'allOtherPathGuidTypeMembershipUnchanged': True,
    'successfulUnityBuildReportVerified': True,
    'packedObjectBytesAreNotCompressedApkOrDeviceMemory': True}
(QA / 'PackedComparison.json').write_text(json.dumps(result, indent=2) + '\n')
print(json.dumps(result, indent=2))
