"""Retain selected, non-secret test evidence and original capture bytes."""
from pathlib import Path
import hashlib
import json
import shutil
import sys
import xml.etree.ElementTree as ET

root = Path.cwd()
delivery = root / 'ProjectDocs/AI/HomeStyleAlignment/ReferenceReplicaR6'
temporary = Path('/tmp/lor-home-r6')
iteration = sys.argv[1]
out = delivery / 'UnityVerification'
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
for kind, label in [('editmode', 'FinalEditMode'), ('playmode', 'FinalPlayMode')]:
    p = temporary / (kind + iteration) / 'results.xml'
    tree = ET.parse(p).getroot()
    assert tree.get('failed') == '0' and tree.get('skipped') == '0', (kind, tree.attrib)
    result = {'sourceXml': str(p), 'sha256': sha(p), 'unity': '6000.5.9f1', 'revision': 9,
        'summary': tree.attrib, 'cases': [{'name': c.get('fullname'), 'result': c.get('result'),
        'failure': c.findtext('failure/message')} for c in tree.iter('test-case')]}
    (out / (label + '.json')).write_text(json.dumps(result, indent=2) + '\n')

captures = []
for folder in ['Home', 'SplashRegression', 'LoadingRegression']:
    source = temporary / ('playmode' + iteration) / folder
    destination = out / folder
    destination.mkdir(exist_ok=True)
    for p in sorted(source.iterdir()):
        if p.suffix not in {'.png', '.json'} or p.name.startswith('Diagnostic_'):
            continue
        target = destination / p.name
        assert not target.exists(), 'Use a fresh output directory; do not overwrite retained captures.'
        shutil.copyfile(p, target)
        assert target.read_bytes() == p.read_bytes()
        captures.append({'path': target.relative_to(root).as_posix(), 'sha256': sha(target),
            'source': str(p), 'unalteredUnityOutput': True})
(out / 'CaptureManifest.json').write_text(json.dumps(captures, indent=2) + '\n')

imports = []
for folder in ['import'] + ['import' + str(i) for i in range(2, int(iteration) + 1)]:
    p = temporary / folder / 'unity.log'
    text = p.read_text()
    readback = 'Home alignment serialized prefab saved and read back at revision 9.'
    imports.append({'owner': 'LegendsOfRus.Editor.Step20ProductionUiFix01SerializedPrefabBuilder.RebuildHomeAlignment',
        'logPath': str(p), 'sha256': sha(p), 'readbackObserved': readback in text,
        'readback': readback, 'successfulBatchExitObserved': 'Batchmode quit successfully invoked' in text})
(out / 'ImportRuns.json').write_text(json.dumps({'unity': '6000.5.9f1 (b57deb96f08d)', 'runs': imports}, indent=2) + '\n')
print(json.dumps({'retainedCaptureFiles': len(captures), 'importRuns': len(imports)}))
