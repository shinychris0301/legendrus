"""Check native caption faces and preserve every other packed path/GUID/type."""
from collections import Counter
from pathlib import Path
import json

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'
before = json.loads((ROOT / 'ProjectDocs/AI/CoreOutcomeReadability/R2AndroidRefresh/QA/PackedAssets.json').read_text())['assets']
record = json.loads((QA / 'PackedAssets.json').read_text())
assert record['result'] == 'Succeeded'
after = record['assets']
path = 'Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab'
key = lambda row: (row['path'], row['guid'], row['type'])
assert Counter(key(row) for row in before if row['path'] != path) == Counter(key(row) for row in after if row['path'] != path)
old = [row for row in before if row['path'] == path]
new = [row for row in after if row['path'] == path]
assert {row['guid'] for row in old} == {row['guid'] for row in new}
delta = Counter(row['type'] for row in new)
delta.subtract(Counter(row['type'] for row in old))
expected = {'GameObject': 4, 'CanvasRenderer': 4, 'RectTransform': 4, 'MonoBehaviour': 4}
assert dict(delta) == expected, dict(delta)
assert len(after) - len(before) == 16
result = {
    'beforeRows': len(before), 'afterRows': len(after), 'addedRows': 16,
    'removedPaths': [], 'addedPaths': [],
    'pageDeltas': {'Gameplay': {'beforeObjects': len(old), 'afterObjects': len(new),
        'addedObjectTypes': expected,
        'packedObjectByteDelta': sum(row['bytes'] for row in new) - sum(row['bytes'] for row in old)}},
    'allOtherPathGuidTypeMembershipUnchanged': True,
    'successfulUnityBuildReportVerified': True,
    'packedObjectBytesAreNotCompressedApkOrDeviceMemory': True
}
(QA / 'PackedComparison.json').write_text(json.dumps(result, indent=2) + '\n')
print(json.dumps(result, indent=2))
