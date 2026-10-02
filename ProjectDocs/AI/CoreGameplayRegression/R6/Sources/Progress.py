"""Observe live evidence; tolerate a witness file still being written."""
from pathlib import Path
import datetime
import json
import time

base = Path(__file__).resolve().parent.parent
files = list((base / 'QA/BroadJourneys').glob('Round*_Attempt*_Witness.json'))
records = []
pending = 0
for path in files:
    try:
        records.append(json.loads(path.read_text()))
    except json.JSONDecodeError:
        pending += 1
pairs = {(row['round'], row['attempt']) for row in records}
log = base / 'QA/Broad/unity.log'
result = {
    'observedAtUtc': datetime.datetime.now(datetime.timezone.utc).isoformat(),
    'witnessesWritten': len(records),
    'completeThreeSeedRounds': sum(all((level, attempt) in pairs for attempt in (1, 2, 3)) for level in range(1, 501)),
    'lastWitnessRound': max((row['round'] for row in records), default=0),
    'captureCount': len(list((base / 'QA/Broad').rglob('*.png'))),
    'resultAvailable': (base / 'QA/Broad/summary.json').exists(),
    'logAgeSeconds': round(time.time() - log.stat().st_mtime, 1) if log.exists() else None,
    'witnessFilesCurrentlyIncomplete': pending,
    'liveProgressIsNotFinalAcceptance': True
}
(base / 'QA/LiveProgress.json').write_text(json.dumps(result, indent=2) + '\n')
print(json.dumps(result))
