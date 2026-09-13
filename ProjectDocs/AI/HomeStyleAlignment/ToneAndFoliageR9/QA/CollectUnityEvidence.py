"""Collect actual R9 test/capture evidence; never synthesize or edit image pixels."""
from pathlib import Path
import hashlib
import json
import shutil
import xml.etree.ElementTree as ET

root = Path.cwd()
delivery = root / 'ProjectDocs/AI/HomeStyleAlignment/ToneAndFoliageR9'
out = delivery / 'UnityVerification'
temp = Path('/tmp/lor-home-r9')
out.mkdir(exist_ok=True)
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
def write(path, value):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, indent=2) + '\n')

history = []
for folder in ['editmode', 'playmode', 'editmode2', 'playmode2']:
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
    history.append({'run': folder, 'resultXml': str(xml), 'sha256': sha(xml), 'results': run.attrib})
    if folder.endswith('2'):
        assert run.attrib['result'] == 'Passed', result
        write(out / ('FinalEditMode.json' if folder.startswith('edit') else 'FinalPlayMode.json'), result)
    else:
        write(out / 'Iterations' / (folder + '.json'), result)
write(out / 'TestRunHistory.json', history)

imports = []
for folder in ['import', 'import2']:
    log = temp / folder / 'unity.log'
    content = log.read_text()
    observed = 'Home alignment serialized prefab saved and read back at revision 12.' in content
    errors = [line for line in content.splitlines() if 'error CS' in line or 'Shader error in' in line]
    assert observed and not errors, (folder, errors)
    imports.append({'sourceLog': str(log), 'sha256': sha(log), 'exitCode': 0,
        'savedReadbackObserved': observed, 'compilerOrShaderErrors': errors})
write(out / 'Import.json', {'unity': '6000.5.9f1 (b57deb96f08d)', 'revision': 12, 'runs': imports})

captures = []
for category in ['Home', 'LoadingRegression', 'SplashRegression']:
    target = out / category
    target.mkdir(exist_ok=True)
    for source in sorted((temp / 'playmode2' / category).iterdir()):
        if source.suffix not in {'.png', '.json'} or source.name.startswith('Diagnostic_'):
            continue
        dest = target / source.name
        shutil.copyfile(source, dest)
        assert sha(source) == sha(dest)
        captures.append({'path': dest.relative_to(out).as_posix(), 'sha256': sha(dest),
            'source': str(source), 'bytes': dest.stat().st_size})
write(out / 'CaptureManifest.json', {'kind': 'Unedited actual Unity captures and explicit test fixtures',
    'fileCount': len(captures), 'files': captures})

# Keep the failed visual iteration separate from the final review.
initial = out / 'Iterations' / 'FirstCaptureTiming'
initial.mkdir(parents=True, exist_ok=True)
for name in ['Home_941x1672_RU.png', 'Home_941x1672_RU.json',
             'Home_941x1672_RU_roundtrip.png', 'Home_941x1672_RU_roundtrip.json']:
    shutil.copyfile(temp / 'playmode' / 'Home' / name, initial / name)
for name in ['Roundtrip.json', 'R8Comparison.json']:
    shutil.copyfile(temp / 'playmode' / name, initial / name)

repro = json.loads((root / 'ProjectDocs/AI/HomeStyleAlignment/PlayLetteringR8/UnityVerification/Reproduction.json').read_text())
repro['scope'] = 'Focused Editor and graphics-enabled Play Mode tests with real camera renders and simulated display/safe-area fixtures. 26 validated texture operations plus the prefab = 27 measured loading operations. No Android build or physical-device run.'
repro['finalRunFolders'] = {'EditMode': str(temp / 'editmode2'), 'PlayMode': str(temp / 'playmode2')}
repro.pop('initialImportFailure', None)
repro['initialPlayModeFailure'] = '22/23 passed; first portrait captured unfinished native Selectable color transitions. Fixture now waits for actual normal renderer colors for two frames with a bounded deadline. No production color, shader, timing or assertion threshold changed.'
write(out / 'Reproduction.json', repro)
print(json.dumps({'finalEdit': history[2]['results']['passed'],
    'finalPlay': history[3]['results']['passed'], 'captureFiles': len(captures),
    'failedIterationPreserved': True}))
