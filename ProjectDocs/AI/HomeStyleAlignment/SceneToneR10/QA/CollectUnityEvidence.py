"""Retain real test results and byte-identical captures from the R10 Unity run."""
from pathlib import Path
import hashlib
import json
import shutil
import xml.etree.ElementTree as ET

root = Path.cwd()
delivery = root / 'ProjectDocs/AI/HomeStyleAlignment/SceneToneR10'
out = delivery / 'UnityVerification'
temp = Path('/tmp/lor-home-r10')
out.mkdir(exist_ok=True)
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
def write(path, value):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, indent=2) + '\n')

history = []
for folder, title in [('editmode', 'FinalEditMode.json'), ('playmode', 'FinalPlayMode.json')]:
    xml = temp / folder / 'results.xml'
    run = ET.parse(xml).getroot()
    tests = []
    for t in run.iter('test-case'):
        row = dict(t.attrib)
        failure = t.find('failure/message')
        if failure is not None:
            row['failure'] = failure.text
        tests.append(row)
    result = {'sourceXml': str(xml), 'sha256': sha(xml), 'run': run.attrib, 'tests': tests}
    assert run.attrib['result'] == 'Passed', result
    write(out / title, result)
    history.append({'run': folder, 'resultXml': str(xml), 'sha256': sha(xml), 'results': run.attrib})
write(out / 'TestRunHistory.json', history)

imports = []
for folder, code in [('import', 1), ('import2', 0)]:
    log = temp / folder / 'unity.log'
    content = log.read_text()
    observed = 'Home alignment serialized prefab saved and read back at revision 13.' in content
    errors = [line for line in content.splitlines() if 'error CS' in line or 'Shader error in' in line]
    if code == 0:
        assert observed and not errors, errors
    imports.append({'sourceLog': str(log), 'sha256': sha(log), 'exitCode': code,
        'savedReadbackObserved': observed, 'compilerOrShaderErrors': errors,
        'result': 'Project already open; aborted before owner execution. Tool stdout confirmed the lock. No process killed or lock deleted.' if code else 'Home-only compile/save/readback passed after the observed Editor process exited.'})
write(out / 'Import.json', {'unity': '6000.5.9f1 (b57deb96f08d)', 'revision': 13, 'runs': imports})

captures = []
for category in ['Home', 'LoadingRegression', 'SplashRegression']:
    target = out / category
    target.mkdir(exist_ok=True)
    for source in sorted((temp / 'playmode' / category).iterdir()):
        if source.suffix not in {'.png', '.json'} or source.name.startswith('Diagnostic_'):
            continue
        dest = target / source.name
        shutil.copyfile(source, dest)
        assert sha(source) == sha(dest)
        captures.append({'path': dest.relative_to(out).as_posix(), 'sha256': sha(dest),
            'source': str(source), 'bytes': dest.stat().st_size})
write(out / 'CaptureManifest.json', {'kind': 'Unedited actual Unity captures and explicit test fixtures',
    'fileCount': len(captures), 'files': captures})

repro = json.loads((root / 'ProjectDocs/AI/HomeStyleAlignment/ToneAndFoliageR9/UnityVerification/Reproduction.json').read_text())
repro['scope'] = 'Focused Editor and graphics-enabled Play Mode tests with actual camera renders and simulated display/safe-area fixtures. Existing 27 measured resource operations unchanged. No Android build or physical-device run.'
repro['finalRunFolders'] = {'EditMode': str(temp / 'editmode'), 'PlayMode': str(temp / 'playmode')}
repro.pop('initialPlayModeFailure', None)
repro['initialImportFailure'] = imports[0]['result']
write(out / 'Reproduction.json', repro)
print(json.dumps({'editModePassed': history[0]['results']['passed'],
    'playModePassed': history[1]['results']['passed'], 'captureFiles': len(captures)}))
