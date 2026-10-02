"""Recover completed XML when the launcher was terminated during Editor shutdown."""
from pathlib import Path
import json
import xml.etree.ElementTree as ET

BASE = Path(__file__).resolve().parent.parent
out = BASE / 'QA/EditVerified'
local = Path('/tmp/lor-returning-player-recap-r2/EditVerified')
marker = 'Test run completed. Exiting with code 0 (Ok). Run completed.'
assert marker in (local / 'unity.log').read_text()
result = ET.parse(out / 'results.xml').getroot().attrib
assert result['result'] == 'Passed' and result['passed'] == '27' and result['failed'] == result['skipped'] == '0'
assert not (out / 'summary.json').exists()
record = {'launcherExitCode': 143, 'unityTestRunnerAnnouncedExitCode': 0,
          'recoveredAfterLauncherTermination': True, 'exitMarker': marker,
          'isolatedConfig': str(local / 'config'), 'rawLogs': str(local),
          'mode': 'EditMode', 'processOnlyVulkan': False,
          'result': result, 'nonPassing': [], 'invocationDurationSeconds': None}
(out / 'summary.json').write_text(json.dumps(record, indent=2) + '\n')
baseline = json.loads((BASE / 'QA/BeforeHashes.json').read_text())
root = Path(__file__).resolve().parents[5]
assert all(str(p.relative_to(root)) in baseline for p in (root / 'Assets').glob('InitTestScene*'))
(out / 'TemporaryFixtureCleanup.json').write_text('{"removed": [], "recoveredAudit": true}\n')
print(json.dumps(record, indent=2))
