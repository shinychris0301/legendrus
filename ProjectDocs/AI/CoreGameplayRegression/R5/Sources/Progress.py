from pathlib import Path
import json,time
base=Path(__file__).resolve().parent.parent
files=list((base/'QA/BroadJourneys').glob('Round*_Attempt*_Witness.json'))
records=[json.loads(p.read_text()) for p in files]
log=base/'QA/Broad/unity.log'
print(json.dumps({'witnessesWritten':len(records),'completeThreeSeedRounds':sum(all(any(r['round']==n and r['attempt']==a for r in records) for a in [1,2,3]) for n in range(1,501)), 'lastWitnessRound':max((r['round'] for r in records),default=0), 'captureCount':len(list((base/'QA/Broad/Captures').rglob('*.png'))),'resultAvailable':(base/'QA/Broad/summary.json').exists(),'logAgeSeconds':round(time.time()-log.stat().st_mtime,1)}))
