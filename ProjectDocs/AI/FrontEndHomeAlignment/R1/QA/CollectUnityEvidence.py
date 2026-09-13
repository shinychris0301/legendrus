"""Retain actual Unity results/captures without modifying their pixels."""
from pathlib import Path
import hashlib
import json
import shutil
import xml.etree.ElementTree as ET

root = Path.cwd()
delivery = root / 'ProjectDocs/AI/FrontEndHomeAlignment/R1'
out = delivery / 'UnityVerification'
temp = Path('/tmp/lor-front-home-r1')
out.mkdir(exist_ok=True)
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
def write(name, value): (out / name).write_text(json.dumps(value, indent=2)+'\n')
history = []
for folder, name in [('editmode', 'FinalEditMode'), ('playmode2', 'FinalPlayMode')]:
    xml = temp / folder / 'results.xml'
    run = ET.parse(xml).getroot()
    tests = []
    for t in run.iter('test-case'):
        row = dict(t.attrib)
        if t.find('failure/message') is not None: row['failure'] = t.findtext('failure/message')
        tests.append(row)
    assert run.attrib['result'] == 'Passed', tests
    shutil.copyfile(xml, out / (name + '.xml'))
    write(name + '.json', {'sourceXml': str(xml), 'sha256': sha(xml), 'run': run.attrib, 'tests': tests})
    history.append({'folder': folder, 'results': run.attrib, 'sourceXml': str(xml), 'sha256': sha(xml)})
history.insert(1, {'folder': 'playmode', 'results': ET.parse(temp / 'playmode/results.xml').getroot().attrib, 'note': 'Initial capture fixture issue; see InitialCaptureIteration/README.md. Production unchanged for the fixture correction.'})
write('TestRunHistory.json', history)
log = temp / 'import/unity.log'
content = log.read_text()
lines = ['Splash Home-style alignment saved and read back at revision 7',
         'Loading alignment serialized prefab saved and read back at revision 4.']
assert all(line in content for line in lines)
assert 'error CS' not in content and 'Shader error in' not in content
write('Import.json', {'unity': '6000.5.9f1 (b57deb96f08d)', 'sourceLog': str(log), 'sha256': sha(log),
    'exitCode': 0, 'saveReadbackLinesObserved': lines, 'compilerOrShaderErrors': [],
    'owner': 'LegendsOfRus.Editor.Step20SplashTitleR3PrefabRebuilder.BuildHomeAlignedFrontEnd'})
captures = []
for category in ('HomeRegression', 'Splash', 'Loading'):
    target = out / category
    target.mkdir(exist_ok=True)
    for source in sorted((temp / 'playmode2' / category).iterdir()):
        if source.suffix not in ('.png', '.json') or source.name.startswith('Diagnostic_'): continue
        dest = target / source.name
        shutil.copyfile(source, dest)
        assert sha(source) == sha(dest)
        captures.append({'path': dest.relative_to(out).as_posix(), 'sha256': sha(dest), 'source': str(source), 'bytes': dest.stat().st_size})
write('CaptureManifest.json', {'kind': 'Unedited actual Unity captures and explicit layout/error fixtures', 'fileCount': len(captures), 'files': captures})
repro = json.loads((root / 'ProjectDocs/AI/HomeStyleAlignment/SceneToneR10/UnityVerification/Reproduction.json').read_text())
repro['owner'] = 'LegendsOfRus.Editor.Step20SplashTitleR3PrefabRebuilder.BuildHomeAlignedFrontEnd'
repro['finalRunFolders'] = {'EditMode': str(temp/'editmode'), 'PlayMode': str(temp/'playmode2')}
repro.pop('initialImportFailure', None)
repro['captureFolders'] = {p: str(temp/'playmode2'/p) for p in ('HomeRegression', 'Splash', 'Loading')}
write('Reproduction.json', repro)
print(json.dumps({'EditMode': history[0]['results']['passed'], 'PlayMode': history[-1]['results']['passed'], 'captureFiles': len(captures)}))
