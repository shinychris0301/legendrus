"""Retain test evidence without distributing raw Editor/licensing logs."""
from pathlib import Path
import hashlib, json, re, shutil, xml.etree.ElementTree as ET

root = Path.cwd()
delivery = root / 'ProjectDocs/AI/GameplayStyleAlignment/HudCompletionR3'
source = Path('/tmp/lor-gameplay-hud-r3')
target = delivery / 'UnityVerification'
target.mkdir(exist_ok=True)
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
runs = []
for name in ['import1', 'focused1/EditMode', 'focused1/PlayMode', 'final/Import',
             'final/EditMode', 'final/PlayMode', 'final2/EditMode', 'final2/PlayMode', 'final3/PlayMode']:
    src = source / name
    dst = target / 'Iterations' / name
    dst.mkdir(parents=True, exist_ok=True)
    command = json.loads((src / 'command.json').read_text())
    shutil.copy2(src / 'command.json', dst / 'command.json')
    log = src / 'unity.log'
    text = log.read_text(errors='replace')
    row = {'run': name, 'exitCode': command['exitCode'], 'rawLogPath': str(log), 'rawLogSha256': sha(log),
           'compilerErrorCount': len(re.findall(r'\berror CS\d+', text)),
           'ownerReadback': [line for line in text.splitlines() if line.startswith('Gameplay alignment serialized prefab saved and read back')]}
    if (src / 'results.xml').exists():
        shutil.copy2(src / 'results.xml', dst / 'results.xml')
        xml = ET.parse(src / 'results.xml').getroot()
        row['tests'] = {k: xml.get(k) for k in ['result', 'total', 'passed', 'failed', 'skipped', 'inconclusive']}
        row['failures'] = [{'test': t.get('name'), 'message': t.findtext('failure/message')}
                           for t in xml.iter('test-case') if t.get('result') == 'Failed']
    runs.append(row)

shutil.copy2(source / 'final2/EditMode/results.xml', target / 'EditMode.xml')
shutil.copy2(source / 'final3/PlayMode/results.xml', target / 'PlayMode.xml')
shutil.copytree(source / 'final3/Gameplay', target / 'Gameplay', dirs_exist_ok=True)
# Retain one unedited initial composition to explain the centering correction.
shutil.copy2(source / 'focused1/Gameplay/Gameplay_941x1672_RU.png', target / 'Iterations/InitialMovesAlignment.png')
shutil.copy2(source / 'focused1/Gameplay/Gameplay_941x1672_RU.json', target / 'Iterations/InitialMovesAlignment.json')
for suite in ['EditMode', 'PlayMode']:
    assert ET.parse(target / (suite + '.xml')).getroot().get('result') == 'Passed'
assert len(list((target / 'Gameplay').glob('*.png'))) == 15
assert len(list((target / 'Gameplay').glob('*.json'))) == 15
(target / 'VerificationRuns.json').write_text(json.dumps({
    'unityVersion': '6000.5.9f1', 'runs': runs,
    'finalEditMode': 'final2/EditMode', 'finalPlayMode': 'final3/PlayMode', 'finalOwnerImport': 'final/Import',
    'finalCaptureCount': 15, 'rawEditorLogsNotDistributed': True,
    'iterationNotes': [
        'Initial 4/4 Edit Mode and 5/5 Play Mode passed, but direct visual inspection found an off-center moves image. Corrected its aspect/placement and added a centering assertion.',
        'The next Play Mode run passed 4/5. The new Pause expectation was incorrect: the existing Pause behavior preserves booster targeting. Restored the idle Pause test and tested the real silent-cancel method separately; no gameplay rule changes.',
        'Corrected tests then passed 4/4 Edit Mode and 5/5 Play Mode, including legacy Home toast fallback.',
        'Final Play Mode 5/5 rerun replaced an injected long-message preview with the real Living Light button, so captures show matching armed state and instruction.'
    ]
}, indent=2) + '\n')
(target / 'EvidenceHashes.json').write_text(json.dumps([
    {'path': str(p.relative_to(target)), 'sha256': sha(p)} for p in sorted(target.rglob('*'))
    if p.is_file() and p.name != 'EvidenceHashes.json'
], indent=2) + '\n')
print(json.dumps({'archivedRuns': len(runs), 'latestCaptures': 15, 'finalEditPassed': 4, 'finalPlayPassed': 5}))
