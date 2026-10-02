"""Reproduce the R14 review of the supplied R13 session; no Unity or player-save writes."""
import hashlib
import json
import sys
from collections import Counter
from pathlib import Path

HERE = Path(__file__).resolve().parent.parent
ROOT = HERE.parents[3]
import importlib.util
spec=importlib.util.spec_from_file_location('r12_analysis', HERE.parent/'R13/Sources/AnalyzeSessions.py')
r12=importlib.util.module_from_spec(spec);spec.loader.exec_module(r12)
analyze=r12.analyze
candidate_id=r12.candidate_id

source = Path(sys.argv[1])
raw = source.read_bytes()
session = json.loads(raw)
expected = candidate_id()
assert session['source'] == 'manual_campaign'
assert session['contentVersion'] == expected
assert session['schemaVersion'] == 1
assert session['droppedAttempts'] == 0 and not session['activeAttempts']
levels = {x['levelId']: x for x in json.loads((ROOT / 'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector01R13/levels.json').read_text())['levels']}
attempts = session['attempts']
rows = []
for a in attempts:
    level = levels[a['round']]
    witness_path = HERE.parent / f"R13/QA/Verify02/Granary/Journey/Round{a['round']}_Attempt{a['committedAttempt']}_Witness.json"
    witness = json.loads(witness_path.read_text())
    assert a['openingHash'] == witness['initialHash']
    assert a['moveBudget'] == level['moveLimit']
    assert 0 <= a['movesRemaining'] <= a['moveBudget']
    assert a['chapter'] == 1 and a['ruleVersion'] == 5
    assert a['snapshotSettled'] and a['endReason'] == 'board_result'
    assert len(a['goalsRemaining']) == len(level['goals'])
    assert all(0 <= v <= g['target'] for v, g in zip(a['goalsRemaining'], level['goals']))
    assert a['outcome'] in ('won', 'lost')
    assert a['phase'] == ('Completed' if a['outcome'] == 'won' else 'Failed')
    assert all(v == 0 for v in a['goalsRemaining']) if a['outcome'] == 'won' else a['movesRemaining'] == 0
    prior = [x for x in attempts if x['number'] < a['number'] and x['round'] == a['round']]
    assert a['committedAttempt'] == len(prior) + 1
    assert all(x['levelFingerprint'] == a['levelFingerprint'] for x in prior)
    rows.append({**a, 'movesUsed': a['moveBudget'] - a['movesRemaining'], 'role': level['difficulty'],
                 'attemptKind': 'first' if not prior else ('replay_after_win' if prior[-1]['outcome'] == 'won' else 'retry_after_loss'),
                 'openingMatchesR9Witness': True,
                 'remainingGoals': [{'goal': g, 'remaining': v} for g, v in zip(level['goals'], a['goalsRemaining']) if v]})
assert [a['number'] for a in attempts] == list(range(1, len(attempts) + 1))
assert [a['campaignRun'] for a in attempts] == list(range(11, len(attempts) + 11))
folder = HERE / 'Sources/Session'
folder.mkdir(parents=True, exist_ok=True)
snapshot = folder / source.name
snapshot.write_bytes(raw)
qa = HERE / 'QA'
qa.mkdir(exist_ok=True)
aggregate = analyze(folder, expected)
assert aggregate['manualSessions'] == 1 and not aggregate['invalidFiles']
unassisted = [a for a in attempts if a['inventoryBoostersUsed'] == 0]
summary = {'sessionId': session['sessionId'], 'contentVersion': expected,
           'completedRounds': sorted({a['round'] for a in attempts if a['outcome'] == 'won'}),
           'firstAttemptWins': sum(a['committedAttempt'] == 1 and a['outcome'] == 'won' for a in attempts),
           'attempts': len(attempts), 'unassistedOutcomes': dict(Counter(a['outcome'] for a in unassisted)),
           'boostedAttempts': len(attempts) - len(unassisted),
           'allAttemptRatings': {k: dict(Counter(a[k] for a in attempts)) for k in ('difficulty', 'goalClarity', 'specialClarity', 'effectClarity')},
           'activeSecondsApproximate': sum(a['activeSeconds'] for a in attempts),
           'openingHashesMatched': len(rows),
           'limits': 'One supplied session; not a population win rate. No commands, exact booster type, story acknowledgements, reward commits, frame timings, or visual evidence. Per-level fingerprints checked for repeat consistency only, not reserialized with Unity.'}
provenance = {'sourcePath': str(source), 'sourceBytes': len(raw), 'sha256': hashlib.sha256(raw).hexdigest(),
              'snapshot': str(snapshot.relative_to(HERE)), 'applicationVersion': session['applicationVersion'],
              'sourceMtimeNs': source.stat().st_mtime_ns, 'candidateMatches': True}
for name, value in [('Summary', summary), ('Attempts', rows), ('ManualEvidence', aggregate), ('Provenance', provenance)]:
    (qa / (name + '.json')).write_text(json.dumps(value, ensure_ascii=False, indent=2) + '\n')
print(json.dumps(summary, ensure_ascii=False, indent=2))
