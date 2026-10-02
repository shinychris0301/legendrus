"""Explain the regenerated libunity payload by inspecting bindings and dynamic symbols."""
import hashlib
import json
import re
import subprocess
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'
comparison = json.loads((QA / 'ArtifactComparison.json').read_text())
LOCAL = Path('/tmp/lor-core-outcome-android-refresh-r2/native-comparison')
inventories = {}
for name, key in [('Before', 'old'), ('After', 'new')]:
    folder = LOCAL / name
    folder.mkdir(parents=True, exist_ok=True)
    with zipfile.ZipFile(ROOT / comparison[key]['path']) as archive:
        data = archive.read('lib/arm64-v8a/libunity.so')
    library = folder / 'libunity.so'
    library.write_bytes(data)
    symbols = subprocess.check_output(['readelf', '--wide', '--dyn-syms', str(library)], text=True)
    (folder / 'DynamicSymbols.txt').write_text(symbols)
    definitions = []
    for line in symbols.splitlines():
        fields = line.split()
        if len(fields) >= 8 and re.fullmatch(r'\d+:', fields[0]):
            definitions.append({'name': fields[7], 'type': fields[3], 'binding': fields[4],
                'visibility': fields[5], 'undefined': fields[6] == 'UND'})
    inventories[name] = {'bytes': len(data), 'sha256': hashlib.sha256(data).hexdigest(),
        'bindings': sorted(set(x.decode() for x in re.findall(
            rb'UnityEngine\.[A-Za-z0-9_+./]+::[A-Za-z0-9_]+', data))),
        'dynamicDefinitions': definitions}
before = inventories['Before']
after = inventories['After']
added = sorted(set(after['bindings']) - set(before['bindings']))
removed = sorted(set(before['bindings']) - set(after['bindings']))
assert added == ['UnityEngine.CanvasRenderer::set_cullTransparentMesh_Injected'] and not removed
assert before['dynamicDefinitions'] == after['dynamicDefinitions']
source = ROOT / 'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionControlTouchTarget.cs'
assert 'canvasRenderer.cullTransparentMesh = false;' in source.read_text()
log = Path('/tmp/lor-core-outcome-android-refresh-r2/build/Build.unity.log').read_text(errors='replace')
link_lines = [line for line in log.splitlines() if 'Link libunity Library/' in line or
              ('Stripping Library/' in line and 'libunity.so' in line)]
assert len(link_lines) == 2
assert after['bytes'] - before['bytes'] == 1288
result = {'before': before, 'after': after, 'addedBindings': added,
    'removedBindings': removed, 'dynamicSymbolDefinitionsUnchanged': True,
    'newSourceCaller': str(source.relative_to(ROOT)),
    'sourceCallerSha256': hashlib.file_digest(source.open('rb'), 'sha256').hexdigest(),
    'regularUnityLinkAndStripSteps': link_lines,
    'byteGrowth': 1288,
    'interpretation': 'The new native binding matches the touch Graphic CanvasRenderer setter; Unity regenerated its stripped engine through the regular build pipeline.',
    'fullBinaryIdentityOrDeviceAcceptanceClaimed': False}
(QA / 'NativeEngineComparison.json').write_text(json.dumps(result, indent=2) + '\n')
print(json.dumps({key: value for key, value in result.items() if key not in ('before', 'after')}, indent=2))
