"""Summarize actual broad/restart evidence without double-counting repeated cases."""
from pathlib import Path
import json
import struct
import xml.etree.ElementTree as ET

BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'
root = ET.parse(QA / 'Broad/results.xml').getroot()
assemblies = []
for suite in root.findall('.//test-suite[@type="Assembly"]'):
    assemblies.append({key: suite.get(key) for key in ('name', 'fullname', 'result', 'total', 'passed', 'failed', 'skipped', 'duration')})
inventory = json.loads((BASE / 'Sources/AssemblyInventory.json').read_text())
assert len(assemblies) == len(inventory) == 8
assert {row['name'].removesuffix('.dll') for row in assemblies} == {row['assembly'] for row in inventory}
(QA / 'BroadAssemblyResults.json').write_text(json.dumps(assemblies, indent=2) + '\n')
witnesses = [json.loads(path.read_text()) for path in (QA / 'BroadJourneys').glob('Round*_Attempt*_Witness.json')]
assert len(witnesses) == 1500
assert {(row['round'], row['attempt']) for row in witnesses} == {(level, attempt) for level in range(1, 501) for attempt in (1, 2, 3)}
assert all(0 < row['moves'] <= row['moveLimit'] and len(row['commands']) == row['moves'] for row in witnesses)
(QA / 'CampaignWitnessSummary.json').write_text(json.dumps({
    'witnessCount': len(witnesses), 'rounds': 500, 'attemptSeedsPerRound': 3,
    'completeRoundSeedPairsVerified': True, 'withinMoveLimit': True,
    'minMoves': min(row['moves'] for row in witnesses),
    'maxMoves': max(row['moves'] for row in witnesses),
    'automatedReplaysAreNotHumanPacingAcceptance': True
}, indent=2) + '\n')
captures = []
for path in sorted(QA.rglob('*.png')):
    with path.open('rb') as stream:
        header = stream.read(24)
    assert header[:8] == b'\x89PNG\r\n\x1a\n'
    width, height = struct.unpack('>II', header[16:24])
    assert width > 0 and height > 0
    captures.append({'path': str(path.relative_to(BASE)), 'width': width, 'height': height})
(QA / 'CaptureInventory.json').write_text(json.dumps(captures, indent=2) + '\n')
print(json.dumps({'assemblies': len(assemblies), 'witnesses': len(witnesses), 'actualUnityPngFiles': len(captures)}, indent=2))
