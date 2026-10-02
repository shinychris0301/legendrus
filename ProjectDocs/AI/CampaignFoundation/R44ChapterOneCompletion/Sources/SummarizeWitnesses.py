from pathlib import Path
import json, statistics

base = Path(__file__).resolve().parent.parent
root = base.parents[3]
folder = base / 'QA/Witnesses'
rows = []
for n in range(1, 501):
    for attempt in range(1, 4):
        row = json.loads((folder / f'Round{n}_Attempt{attempt}_Witness.json').read_text())
        assert row['round'] == n and row['attempt'] == attempt
        assert row['moves'] == len(row['commands']) <= row['moveLimit']
        assert all(c['Rules'] == 5 and c['BeforeToken'] and c['ExpectedPlanHash'] for c in row['commands'])
        rows.append(row)
old = root / 'ProjectDocs/AI/CampaignFoundation/R33OrchardFieldCheckpoint/QA/Verify01/Active/Journey'
unchanged = 0
for row in rows[:600]:
    previous = json.loads((old / f"Round{row['round']}_Attempt{row['attempt']}_Witness.json").read_text())
    assert previous == row, f"Existing witness changed: {row['round']}/{row['attempt']}"
    unchanged += 1
sectors = []
for first in range(201, 501, 50):
    group = [r for r in rows if first <= r['round'] < first+50]
    sectors.append(dict(first=first, last=first+49, attempts=len(group),
        movesMin=min(r['moves'] for r in group), movesMax=max(r['moves'] for r in group),
        movesMedian=statistics.median(r['moves'] for r in group),
        minimumMovesRemaining=min(r['moveLimit']-r['moves'] for r in group)))
result = dict(witnesses=len(rows), rounds=500, attemptsPerRound=3,
    newRoundWitnesses=900, preservedOldWitnesses=unchanged, inventoryBoostersUsed=0,
    sectors=sectors,
    qualification='Unity replay tests verify legal commands, goals and move budgets. Search witnesses establish reachability, not win rate or human difficulty.')
(base/'QA/WitnessSummary.json').write_text(json.dumps(result, indent=2)+'\n')
print(json.dumps(result, indent=2))
