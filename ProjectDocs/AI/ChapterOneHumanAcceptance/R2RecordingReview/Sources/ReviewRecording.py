"""Read explicit Campaign Active exports into a rounds 201–210 review; never grant acceptance."""
from pathlib import Path
from datetime import datetime
import argparse
import csv
import hashlib
import importlib.util
import json
import re

BASE = Path(__file__).resolve().parent.parent
ROOT = BASE.parents[3]
PRIOR = BASE.parent / 'R1'
NATIVE = PRIOR / 'QA/NativeAudit/InstalledContent.json'
DEFAULT = Path('/home/k2/.config/unity3d/DefaultCompany/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL/CampaignPlaytests/Sessions/manual_campaign')
spec = importlib.util.spec_from_file_location('coverage_rules', PRIOR / 'Sources/AuditCoverage.py')
rules = importlib.util.module_from_spec(spec)
spec.loader.exec_module(rules)

def sha(data):
    return hashlib.sha256(data).hexdigest()

def load_native():
    native = json.loads(NATIVE.read_text())
    content = ROOT / 'Assets/LegendsOfRus/Resources/CampaignActive'
    texts = [(content / (name + '.json')).read_bytes().decode('utf-8-sig')
             for name in ('definition', 'levels', 'story', 'story_ru', 'story_en')]
    if 'CampaignActive-' + sha('\n'.join(texts).encode()) != native['contentFingerprint']:
        raise ValueError('Installed content changed; regenerate the native audit before reviewing recordings.')
    assert native['editorVersion'] == '6000.5.9f1'
    assert [r['round'] for r in native['rounds']] == list(range(1, 501))
    assert all(sha(r['canonicalJson'].encode()) == r['fingerprint'] for r in native['rounds'])
    return native

def session_errors(session, native):
    if not isinstance(session, dict):
        return ['invalid_session_object']
    if any(not isinstance(session.get(key, []), list) for key in ('attempts', 'activeAttempts')):
        return ['invalid_attempt_lists']
    if 'attempts' not in session:
        return ['missing_attempts']
    attempts = session['attempts'] + session.get('activeAttempts', [])
    if any(not isinstance(a, dict) for a in attempts):
        return ['invalid_attempt_object']
    if any(type(a.get('number')) is not int for a in attempts):
        return ['invalid_attempt_number']
    errors = rules.session_issues(session, native)
    if type(session.get('schemaVersion')) is not int or type(session.get('droppedAttempts', 0)) is not int:
        errors.append('invalid_session_numeric_types')
    if session.get('campaignStartRound') != 201 or session.get('campaignEndRound') != 210:
        errors.append('not_target_block')
    return errors

def attempt_errors(attempt, session, native):
    integer_fields = ('round', 'chapter', 'ruleVersion', 'committedAttempt', 'campaignRun', 'inventoryBoostersUsed')
    if any(type(attempt.get(key)) is not int for key in integer_fields):
        return ['invalid_attempt_numeric_types']
    errors = rules.attempt_issues(attempt, session, native)
    if attempt.get('snapshotSettled') is not True:
        errors.append('snapshot_not_boolean_true')
    try:
        dates = [datetime.fromisoformat(attempt[k].replace('Z', '+00:00'))
                 for k in ('startedUtc', 'finishedUtc')]
        if any(d.tzinfo is None for d in dates) or dates[1] < dates[0]:
            errors.append('invalid_attempt_date_order_or_timezone')
    except (ValueError, TypeError, KeyError, AttributeError):
        errors.append('invalid_attempt_dates')
    return sorted(set(errors))

def review(records, native):
    files, groups = [], {}
    for label, session, digest in records:
        errors = session_errors(session, native)
        row = {'path': label, 'sha256': digest, 'issues': errors}
        files.append(row)
        if isinstance(session, dict):
            row['sessionId'] = session.get('sessionId')
            row['source'] = session.get('source')
        if isinstance(session, dict) and re.fullmatch('[0-9a-f]{32}', str(session.get('sessionId', ''))):
            groups.setdefault(session['sessionId'], []).append((row, session))
    attempts, conflicts = [], []
    copies = 0
    for sid, versions in sorted(groups.items()):
        # Divergent snapshots need an explicit canonical export; do not choose by filesystem time.
        if len({json.dumps(s, sort_keys=True) for _, s in versions}) != 1:
            conflicts.append(sid)
            for row, _ in versions:
                row['issues'].append('conflicting_session_snapshots')
            continue
        copies += len(versions) - 1
        if versions[0][0]['issues']:
            continue
        session = versions[0][1]
        for active, entries in ((False, session['attempts']), (True, session.get('activeAttempts', []))):
            for attempt in entries:
                errors = attempt_errors(attempt, session, native)
                if active:
                    errors.append('active_snapshot_not_completed_export')
                rated = (attempt.get('difficulty') in ('easy', 'fair', 'hard')
                         and attempt.get('goalClarity') in ('clear', 'unclear')
                         and attempt.get('specialClarity') in ('clear', 'unclear'))
                attempts.append({'sessionId': sid, 'number': attempt['number'], 'round': attempt.get('round'),
                    'outcome': attempt.get('outcome'), 'inventoryBoostersUsed': attempt.get('inventoryBoostersUsed'),
                    'score': attempt.get('score'), 'moveBudget': attempt.get('moveBudget'),
                    'movesRemaining': attempt.get('movesRemaining'), 'activeSeconds': attempt.get('activeSeconds'),
                    'goalsRemaining': attempt.get('goalsRemaining'),
                    'difficulty': attempt.get('difficulty'), 'goalClarity': attempt.get('goalClarity'),
                    'specialClarity': attempt.get('specialClarity'), 'issues': sorted(set(errors)),
                    'eligibleCurrentOutcome': not errors, 'ratingsComplete': rated})
    rounds = []
    for source in native['rounds'][200:210]:
        number = source['round']
        definition = json.loads(source['canonicalJson'])
        eligible = [a for a in attempts if a['round'] == number and a['eligibleCurrentOutcome']]
        wins = sum(a['outcome'] == 'won' for a in eligible)
        losses = sum(a['outcome'] == 'lost' for a in eligible)
        unrated = sum(not a['ratingsComplete'] for a in eligible)
        rounds.append({'round': number, 'title': definition['displayName'],
            'authoredDifficulty': definition['difficulty'], 'moveLimit': definition['moveLimit'],
            'roundFingerprint': source['fingerprint'], 'currentOutcomes': len(eligible),
            'wins': wins, 'losses': losses, 'unratedOutcomes': unrated,
            'noInventoryBoosterOutcomes': sum(a['inventoryBoostersUsed'] == 0 for a in eligible),
            'boosterAssistedOutcomes': sum(a['inventoryBoostersUsed'] > 0 for a in eligible),
            'status': 'needs_human_recording' if not eligible else 'needs_successful_completion' if not wins
                      else 'ratings_needed' if unrated else 'recorded_review_required'})
    status = ('recording_snapshot_conflict' if conflicts else
              'needs_human_recording' if any(not r['currentOutcomes'] for r in rounds) else
              'needs_successful_completion' if any(not r['wins'] for r in rounds) else
              'ratings_needed' if any(r['unratedOutcomes'] for r in rounds) else 'ready_for_human_review')
    return {'status': status, 'blockStart': 201, 'blockEnd': 210,
        'contentFingerprint': native['contentFingerprint'], 'sourceTagDoesNotVerifyHumanIdentity': True,
        'humanAcceptanceGranted': False, 'balanceChangesRecommendedAutomatically': False,
        'requiresBlockObservationAndHumanReview': True,
        'duplicateCopiesIgnored': copies, 'conflictingSessionIds': conflicts,
        'eligibleOutcomeCount': sum(r['currentOutcomes'] for r in rounds),
        'excludedAttemptCount': sum(not a['eligibleCurrentOutcome'] for a in attempts),
        'files': files, 'rounds': rounds, 'attempts': attempts}

def read_recording(path):
    if path.is_symlink() or path.suffix != '.json' or not path.is_file():
        raise ValueError('Use a regular JSON recorder export, not a symlink or backup.')
    if path.stat().st_size > 4 * 1024 * 1024:
        raise ValueError('Recorder export exceeds the review size limit.')
    data = path.read_bytes()
    def reject_constant(value):
        raise ValueError('Non-finite JSON number: ' + value)
    return str(path), json.loads(data, parse_constant=reject_constant), sha(data)

def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--session', action='append', type=Path, help='Explicit recorder JSON; repeat for separate sessions.')
    parser.add_argument('--output', type=Path, required=True, help='New output directory; existing paths are refused.')
    args = parser.parse_args(argv)
    if args.output.exists():
        parser.error('Output already exists; use a fresh directory to preserve earlier evidence.')
    try:
        native = load_native()
        paths = args.session if args.session is not None else sorted(DEFAULT.glob('*.json'))
        records, invalid = [], []
        for path in paths:
            try:
                records.append(read_recording(path))
            except (OSError, ValueError, UnicodeError) as error:
                invalid.append({'path': str(path), 'reason': str(error)})
        report = review(records, native)
        report['invalidFiles'] = invalid
        if invalid:
            report['status'] = 'input_requires_review'
        # Output must never overwrite input or alter the native evidence/active product resources.
        target = args.output.resolve()
        if any(target.is_relative_to(ROOT / name) for name in ('Assets', 'Packages', 'ProjectSettings', 'Backend')) or target.is_relative_to(DEFAULT.parents[2]):
            raise ValueError('Write the review outside product content and recorder storage.')
        args.output.mkdir(parents=True)
        (args.output / 'Review.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
        with (args.output / 'Rounds.csv').open('w', newline='') as stream:
            writer = csv.DictWriter(stream, fieldnames=list(report['rounds'][0]))
            writer.writeheader(); writer.writerows(report['rounds'])
        print(json.dumps({key: report[key] for key in ('status', 'eligibleOutcomeCount',
                         'excludedAttemptCount', 'humanAcceptanceGranted')}, indent=2))
        return 0 if report['status'] == 'ready_for_human_review' else 2
    except (OSError, ValueError) as error:
        parser.error(str(error))

if __name__ == '__main__':
    raise SystemExit(main())
