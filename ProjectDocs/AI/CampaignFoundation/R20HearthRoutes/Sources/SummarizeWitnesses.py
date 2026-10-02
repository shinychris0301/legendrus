from pathlib import Path
import json
base=Path(__file__).resolve().parent.parent
witnesses=[json.loads(p.read_text()) for p in (base/'QA/Verify01/Active/Journey').glob('*Witness.json')]
assert len(witnesses)==270
rows=[]
for n in range(1,91):
 ws=sorted([w for w in witnesses if w['round']==n],key=lambda w:w['attempt']);assert [w['attempt'] for w in ws]==[1,2,3]
 assert all(w['moves']<=w['moveLimit'] and len(w['commands'])==w['moves'] for w in ws)
 rows.append(dict(round=n,moveLimit=ws[0]['moveLimit'],moves=[w['moves'] for w in ws],expanded=[w['expanded'] for w in ws]))
old=base.parent/'R19Courtyards/QA/Verify01/Active/Journey';compared=0
for w in witnesses:
 if w['round']>80:continue
 previous=json.loads((old/f"Round{w['round']}_Attempt{w['attempt']}_Witness.json").read_text())
 assert w==previous,(w['round'],w['attempt']);compared+=1
assert compared==240
report=dict(passed=True,completionWitnesses=270,attemptSeedsPerRound=3,inventoryUsed=False,identicalPriorWitnesses=compared,meaning='Legal replay solutions, not human difficulty or win rate',rows=rows)
(base/'QA/DifficultyScreening.json').write_text(json.dumps(report,indent=2)+'\n');print(json.dumps(rows[80:],indent=2))
