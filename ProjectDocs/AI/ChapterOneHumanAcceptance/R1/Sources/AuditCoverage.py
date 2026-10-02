"""Audit existing recorder JSON only; never modify game content or player storage."""
from pathlib import Path
import argparse
import collections
import csv
import hashlib
import json
import re
import subprocess

BASE = Path(__file__).resolve().parent.parent
ROOT = BASE.parents[3]
DEFAULT = Path('/home/k2/.config/unity3d/DefaultCompany/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL/CampaignPlaytests')
SOURCES = ('manual_campaign', 'automated_campaign', 'manual_sandbox', 'automated_test')


def digest(data):
    return hashlib.sha256(data).hexdigest()


def write_json(path, value):
    path.write_text(json.dumps(value, ensure_ascii=False, indent=2) + '\n')


def attempt_issues(attempt, session, native):
    problems = []
    number = attempt.get('round')
    expected = {row['round']: row['fingerprint'] for row in native['rounds']}
    if type(number) is not int or number not in expected:
        return ['unavailable_round']
    if attempt.get('levelFingerprint') != expected[number]:
        problems.append('round_fingerprint_mismatch')
    if attempt.get('ruleVersion') != native['ruleVersion']:
        problems.append('rules_mismatch')
    if attempt.get('chapter') != 1:
        problems.append('chapter_mismatch')
    if attempt.get('outcome') not in ('won', 'lost'):
        problems.append('no_completed_outcome')
    if not attempt.get('finishedUtc') or not attempt.get('startedUtc'):
        problems.append('missing_attempt_dates')
    if not attempt.get('snapshotSettled'):
        problems.append('unsettled_snapshot')
    if attempt.get('committedAttempt', 0) < 1 or attempt.get('campaignRun', 0) < 1:
        problems.append('missing_committed_run')
    if attempt.get('inventoryBoostersUsed', -1) < 0:
        problems.append('unknown_booster_use')
    start, end = session.get('campaignStartRound'), session.get('campaignEndRound')
    if type(start) is not int or type(end) is not int or not start <= number <= end:
        problems.append('outside_recorded_block')
    return problems


def session_issues(session, native):
    issues = []
    if session.get('source') != 'manual_campaign':
        issues.append('not_manual_campaign')
    if session.get('schemaVersion') != 1:
        issues.append('unsupported_schema')
    if not re.fullmatch('[0-9a-f]{32}', str(session.get('sessionId', ''))):
        issues.append('invalid_session_id')
    if session.get('contentVersion') != native['contentFingerprint']:
        issues.append('installed_content_mismatch')
    start, end = session.get('campaignStartRound'), session.get('campaignEndRound')
    if type(start) is not int or start not in native['availableBlocks'] or end != start + 9:
        issues.append('missing_or_invalid_active_block')
    if type(start) is not int or session.get('fixtureCompletedThrough') != start - 1:
        issues.append('missing_or_invalid_fixture_context')
    if session.get('collectionContext') != 'CampaignActive; disposable earlier-progress fixture; 30 energy; zero starting boosters':
        issues.append('missing_or_other_collection_context')
    if session.get('droppedAttempts', 0) != 0:
        issues.append('truncated_attempt_history')
    attempts = session.get('attempts', []) + session.get('activeAttempts', [])
    numbers = [a.get('number') for a in attempts]
    if any(type(n) is not int or n < 1 for n in numbers) or len(numbers) != len(set(numbers)):
        issues.append('invalid_or_duplicate_attempt_numbers')
    return issues


def priority(start):
    if 201 <= start <= 241:
        return 1
    if start == 491:
        return 2
    if start in (291, 341, 391, 441, 481):
        return 3
    if start < 201:
        return 4
    return 5


def collect_candidates():
    # Exact recorder source tags find raw sessions and historical inventories. Inventories
    # are retained separately and cannot become observed attempts.
    found = subprocess.check_output(['rg', '-l', '--glob', '*.json',
        '"source"\\s*:\\s*"(manual_campaign|manual_sandbox|automated_campaign|automated_test)"',
        'ProjectDocs/AI'], cwd=ROOT, text=True).splitlines()
    paths = {ROOT / value for value in found if not (ROOT / value).is_relative_to(BASE)}
    if DEFAULT.exists():
        paths.update(p for p in DEFAULT.rglob('*') if p.is_file() and
                     (p.name.endswith('.json') or p.name.endswith('.json.bak')))
    return sorted(paths)


def run():
    parser = argparse.ArgumentParser()
    parser.add_argument('--native', type=Path, default=BASE / 'QA/NativeAudit/InstalledContent.json')
    native_path = parser.parse_args().native.resolve()
    assert native_path.is_relative_to(BASE / 'QA'), 'Use native evidence retained under this task QA directory'
    native = json.loads(native_path.read_text())
    content_root = ROOT / 'Assets/LegendsOfRus/Resources/CampaignActive'
    names = ['definition', 'levels', 'story', 'story_ru', 'story_en']
    # No newline normalization: Resources TextAsset.text is verified against this exact join.
    texts = [(content_root / (name + '.json')).read_bytes().decode('utf-8-sig') for name in names]
    fingerprint = 'CampaignActive-' + digest('\n'.join(texts).encode('utf-8'))
    assert fingerprint == native['contentFingerprint'], 'Re-export native fingerprints after content changes'
    assert native['editorVersion'] == '6000.5.9f1'
    expected = {r['round']: r['fingerprint'] for r in native['rounds']}
    assert set(expected) == set(range(1, 501))
    for r in native['rounds']:
        assert digest(r['canonicalJson'].encode('utf-8')) == r['fingerprint']
    files, inventories, invalid, groups = [], [], [], collections.defaultdict(list)
    for path in collect_candidates():
        if path.is_symlink():
            invalid.append({'path': str(path), 'reason': 'symlink_not_followed'})
            continue
        data = path.read_bytes()
        descriptor = {'path': str(path.relative_to(ROOT)) if path.is_relative_to(ROOT) else str(path),
                      'sha256': digest(data), 'bytes': len(data), 'mtimeNs': path.stat().st_mtime_ns}
        try:
            session = json.loads(data)
        except (ValueError, UnicodeError) as error:
            invalid.append({**descriptor, 'reason': type(error).__name__})
            continue
        if not isinstance(session, dict) or 'sessionId' not in session or 'attempts' not in session:
            inventories.append(descriptor)
            continue
        if not isinstance(session['attempts'], list) or not isinstance(session.get('activeAttempts', []), list):
            invalid.append({**descriptor, 'reason': 'invalid_attempt_lists'})
            continue
        descriptor.update(source=session.get('source'), sessionId=session['sessionId'],
                          contentVersion=session.get('contentVersion'))
        files.append(descriptor)
        groups[session['sessionId']].append((descriptor, session, data))
    canonical, attempts, conflicts = [], [], []
    archived = BASE / 'Sources/ManualSessions'
    archived.mkdir(exist_ok=True)
    for sid, versions in sorted(groups.items()):
        identities = {(v[1].get('source'), v[1].get('contentVersion')) for v in versions}
        if len(identities) != 1:
            conflicts.append({'sessionId': sid, 'paths': [v[0]['path'] for v in versions]})
            continue
        def rank(version):
            d, s, _ = version
            all_attempts = s['attempts'] + s.get('activeAttempts', [])
            return (max((a.get('number', 0) for a in all_attempts), default=0),
                    max((a.get('number', 0) for a in s['attempts']), default=0),
                    len(s['attempts']), d['mtimeNs'])
        descriptor, session, data = max(versions, key=rank)
        issues = session_issues(session, native)
        record = {**descriptor, 'issues': issues, 'createdUtc': session.get('createdUtc'),
                  'applicationVersion': session.get('applicationVersion'),
                  'campaignStartRound': session.get('campaignStartRound'),
                  'campaignEndRound': session.get('campaignEndRound'),
                  'droppedAttempts': session.get('droppedAttempts', 0),
                  'completedAttempts': len(session['attempts']),
                  'activeAttempts': len(session.get('activeAttempts', [])),
                  'rounds': sorted({a.get('round') for a in session['attempts'] + session.get('activeAttempts', [])}),
                  'copies': [v[0] for v in versions]}
        canonical.append(record)
        if session.get('source') == 'manual_campaign':
            assert re.fullmatch('[0-9a-f]{32}', sid)
            (archived / ('playtest-' + sid + '.json')).write_bytes(data)
        for a in session['attempts'] + session.get('activeAttempts', []):
            problems = attempt_issues(a, session, native)
            ratings = (a.get('difficulty') in ('easy', 'fair', 'hard') and
                       a.get('goalClarity') in ('clear', 'unclear') and
                       a.get('specialClarity') in ('clear', 'unclear'))
            attempts.append({**a, 'sessionId': sid, 'source': session.get('source'),
                             'sessionIssues': issues, 'attemptIssues': problems,
                             'nativeRoundMatches': a.get('levelFingerprint') == expected.get(a.get('round')),
                             'ratingsComplete': ratings, 'eligibleCurrentOutcome': not issues and not problems,
                             'eligibleCurrentRatedOutcome': not issues and not problems and ratings})
    levels = json.loads(texts[1])['levels']
    story = json.loads(texts[2])
    milestones = collections.defaultdict(list)
    for milestone in story['milestones']:
        milestones[milestone['afterLevelId']].append(milestone['sceneId'])
    rows = []
    for level in levels:
        number = level['levelId']
        if number < 31:
            continue
        start = ((number - 1) // 10) * 10 + 1
        evidence = [a for a in attempts if a.get('round') == number]
        outcomes = [a for a in evidence if a['eligibleCurrentOutcome']]
        rated = [a for a in outcomes if a['ratingsComplete']]
        rows.append({'round': number, 'blockStart': start, 'blockEnd': start + 9,
            'priority': priority(start), 'sector': (number - 1) // 50 + 1,
            'title': level['displayName'], 'authoredDifficulty': level['difficulty'],
            'authoredDifficultyRating': level['difficultyRating'], 'moveLimit': level['moveLimit'],
            'goals': '; '.join(f"{g['type']}:{g.get('tileType', g.get('obstacleType', g.get('itemType', '')))}={g['target']}" for g in level['goals']),
            'obstacleTypes': '; '.join(sorted({o['type'] for o in level['obstacles']})),
            'storyScenesAfterWin': '; '.join(milestones[number]),
            'roundFingerprint': expected[number], 'currentManualOutcomes': len(outcomes),
            'currentRatedOutcomes': len(rated), 'noInventoryBoosterOutcomes': sum(a.get('inventoryBoostersUsed') == 0 for a in outcomes),
            'manualWins': sum(a['outcome'] == 'won' for a in outcomes),
            'manualLosses': sum(a['outcome'] == 'lost' for a in outcomes),
            'automatedAttempts': sum(a['source'] != 'manual_campaign' for a in evidence),
            'collectionStatus': 'recorded_review_required' if rated else 'ratings_needed' if outcomes else 'needs_human_recording'})
    blocks = []
    for start in range(31, 501, 10):
        block = [r for r in rows if r['blockStart'] == start]
        blocks.append({'start': start, 'end': start + 9, 'priority': priority(start),
                       'roundsNeedingRecording': [r['round'] for r in block if not r['currentManualOutcomes']],
                       'roundsNeedingRatings': [r['round'] for r in block if not r['currentRatedOutcomes']],
                       'hardRounds': [r['round'] for r in block if r['authoredDifficulty'] == 'hard'],
                       'recoveryRounds': [r['round'] for r in block if r['authoredDifficulty'] == 'recovery'],
                       'storyCheckpoint': [r['round'] for r in block if r['storyScenesAfterWin']]})
    blocks.sort(key=lambda b: (b['priority'], b['start']))
    manual = [a for a in attempts if a['source'] == 'manual_campaign']
    summary = {'contentFingerprint': fingerprint, 'ruleVersion': native['ruleVersion'],
               'candidateFiles': len(files) + len(inventories) + len(invalid),
               'rawSessionFiles': len(files), 'historicalInventoryFiles': len(inventories),
               'uniqueSessions': len(canonical), 'duplicateSessionFiles': sum(len(s['copies']) - 1 for s in canonical),
               'fileSourceCounts': dict(collections.Counter(f['source'] for f in files)),
               'uniqueSourceCounts': dict(collections.Counter(s['source'] for s in canonical)),
               'manualAttempts': len(manual), 'manualOutcomes': dict(collections.Counter(a['outcome'] for a in manual)),
               'historicalManualRounds': sorted({a['round'] for a in manual}),
               'historicalNativeRoundMatchingAttempts': sum(a['nativeRoundMatches'] for a in manual),
               'historicalFullyRatedAttempts': sum(a['ratingsComplete'] for a in manual),
               'historicalMatchingFullyRatedAttempts': sum(a['nativeRoundMatches'] and a['ratingsComplete'] for a in manual),
               'currentManualSessions': sum(not s['issues'] for s in canonical),
               'currentManualRounds31To500': sum(bool(r['currentManualOutcomes']) for r in rows),
               'currentRatedRounds31To500': sum(bool(r['currentRatedOutcomes']) for r in rows),
               'roundsNeedingRecording': sum(not r['currentManualOutcomes'] for r in rows),
               'blocksNeedingRecording': sum(bool(b['roundsNeedingRecording']) for b in blocks),
               'invalidFiles': invalid, 'conflictingSessions': conflicts,
               'scope': ['Exact recorder source-tagged JSON under ProjectDocs/AI (this audit excluded)', str(DEFAULT)],
               'limitations': ['Other external export folders and devices not searched',
                   'manual_campaign is recorder provenance, not independent human identity verification',
                   'Historical content/ratings retained, never counted as current acceptance',
                   'Recorded outcomes still require human review; no automatic acceptance or balancing',
                   'Authored difficulty labels are not human measurements; automated attempts are excluded']}
    write_json(BASE / 'QA/EvidenceInventory.json', {'summary': summary, 'sessions': canonical,
                                                  'historicalInventories': inventories, 'rawFiles': files})
    write_json(BASE / 'QA/AttemptAudit.json', attempts)
    write_json(BASE / 'CoverageMatrix.json', {'summary': summary, 'blocks': blocks, 'rounds': rows})
    for filename, data in [('CoverageMatrix.csv', rows), ('CollectionQueue.csv', blocks)]:
        with (BASE / filename).open('w', newline='') as stream:
            writer = csv.DictWriter(stream, fieldnames=list(data[0]))
            writer.writeheader()
            writer.writerows(data)
    write_json(BASE / 'QA/ResourceHashes.json', {name: digest((content_root / (name + '.json')).read_bytes()) for name in names})
    print(json.dumps(summary, ensure_ascii=False, indent=2))


if __name__ == '__main__':
    run()
