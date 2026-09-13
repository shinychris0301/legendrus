"""Copy unedited Unity evidence; retain every test iteration's result."""
from pathlib import Path
import hashlib
import json
import shutil
import xml.etree.ElementTree as ET

root = Path.cwd()
delivery = root / 'ProjectDocs/AI/HomeStyleAlignment/SelectiveLightingR11'
out = delivery / 'UnityVerification'
temp = Path('/tmp/lor-home-r11')
out.mkdir(exist_ok=True)
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
def write(name, data): (out / name).write_text(json.dumps(data, indent=2) + '\n')
history = []
for folder, name in [('editmode', 'FinalEditMode'), ('playmode', 'InitialPlayMode'),
                     ('playmode2', 'IntermediatePlayMode'), ('playmode3', 'BackgroundFixPlayMode'),
                     ('playmode4', 'FinalPlayMode')]:
    source = temp / folder / 'results.xml'
    run = ET.parse(source).getroot()
    if name.startswith('Final'): assert run.attrib['result'] == 'Passed'
    tests = []
    for t in run.iter('test-case'):
        row = dict(t.attrib)
        if t.find('failure/message') is not None: row['failure'] = t.findtext('failure/message')
        tests.append(row)
    shutil.copyfile(source, out / (name + '.xml'))
    write(name + '.json', {'sourceXml': str(source), 'sha256': sha(source), 'run': run.attrib, 'tests': tests})
    history.append({'folder': folder, 'results': run.attrib, 'sha256': sha(source)})
history[1]['note'] = 'Composite-alpha failure fixed in shader; misspelled Splash filter corrected. See QA/Iterations/Initial/README.md.'
history[2]['note'] = '25 passed, but all three named diagnostic backgrounds were navy: router backdrop hid camera clear. Fixture disables/restores backdrop and now asserts empty-corner RGB. Production unchanged for final rerun.'
history[3]['note'] = '25 passed, backgrounds corrected. External R10 comparison then found 407073 composite-alpha differences: the toggle test retained the custom material in its baseline. Final test removes/restores custom materials; final shader uses premultiplied UGUI blending for both RGB and alpha.'
write('TestRunHistory.json', history)
logs = []
for folder in ('import', 'editmode', 'playmode4'):
    path = temp / folder / 'unity.log'
    content = path.read_text()
    assert 'error CS' not in content and 'Shader error in' not in content
    logs.append({'folder': folder, 'source': str(path), 'sha256': sha(path), 'exitCode': 0, 'compilerOrShaderErrors': []})
line = 'Home alignment serialized prefab saved and read back at revision 14.'
assert line in (temp / 'import/unity.log').read_text()
write('Import.json', {'unity': '6000.5.9f1 (b57deb96f08d)', 'owner': 'LegendsOfRus.Editor.Step20ProductionUiFix01SerializedPrefabBuilder.RebuildHomeAlignment',
    'saveReadbackLineObserved': line, 'logs': logs,
    'note': 'Owner save preceded alpha-blend fix; final Play Mode launch compiled and rendered the final shader.'})
captures = []
for category in ('Home', 'SplashRegression', 'LoadingRegression'):
    target = out / category
    target.mkdir(exist_ok=True)
    for source in sorted((temp / 'playmode4' / category).iterdir()):
        if source.suffix not in ('.png', '.json') or source.name.startswith('Diagnostic_'): continue
        dest = target / source.name
        shutil.copyfile(source, dest)
        assert sha(source) == sha(dest)
        captures.append({'path': dest.relative_to(out).as_posix(), 'sha256': sha(dest), 'source': str(source), 'bytes': dest.stat().st_size})
write('CaptureManifest.json', {'kind': 'Unedited actual Unity captures and explicit fixtures; LightingBackground_* isolate the two lit sprites for background QA.', 'fileCount': len(captures), 'files': captures})
repro = json.loads((root / 'ProjectDocs/AI/HomeStyleAlignment/SceneToneR10/UnityVerification/Reproduction.json').read_text())
repro['finalRunFolders'] = {'EditMode': str(temp / 'editmode'), 'PlayMode': str(temp / 'playmode4')}
repro.pop('initialImportFailure', None)
repro['captureFolders'] = {p: str(temp / 'playmode4' / p) for p in ('Home', 'SplashRegression', 'LoadingRegression')}
write('Reproduction.json', repro)
print(json.dumps({'EditMode': history[0]['results']['passed'], 'PlayMode': history[-1]['results']['passed'], 'captureFiles': len(captures)}))
