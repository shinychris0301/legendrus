"""Synthetic boundary checks: human coverage must not be fabricated by tooling."""
from pathlib import Path
from contextlib import redirect_stdout
from unittest.mock import patch
import copy
import io
import json
import tempfile
import unittest
import ReviewRecording as tool

class RecordingReviewTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.native = tool.load_native()

    def setUp(self):
        self.session = {'schemaVersion': 1, 'source': 'manual_campaign', 'sessionId': 'a' * 32,
            'contentVersion': self.native['contentFingerprint'], 'campaignStartRound': 201,
            'campaignEndRound': 210, 'fixtureCompletedThrough': 200, 'droppedAttempts': 0,
            'collectionContext': 'CampaignActive; disposable earlier-progress fixture; 30 energy; zero starting boosters',
            'attempts': [], 'activeAttempts': []}
        for i, source in enumerate(self.native['rounds'][200:210], 1):
            self.session['attempts'].append({'number': i, 'round': source['round'], 'chapter': 1,
                'ruleVersion': self.native['ruleVersion'], 'levelFingerprint': source['fingerprint'],
                'outcome': 'won', 'startedUtc': '2026-10-03T12:00:00Z', 'finishedUtc': '2026-10-03T12:01:00Z',
                'snapshotSettled': True, 'committedAttempt': 1, 'campaignRun': i,
                'inventoryBoostersUsed': 0, 'difficulty': 'fair', 'goalClarity': 'clear', 'specialClarity': 'clear'})

    def inspect(self, sessions=None):
        sessions = [self.session] if sessions is None else sessions
        return tool.review([(str(i), s, 'synthetic-hash') for i, s in enumerate(sessions)], self.native)

    def test_empty_input_never_becomes_coverage(self):
        result = self.inspect([])
        self.assertEqual(result['status'], 'needs_human_recording')
        self.assertEqual(result['eligibleOutcomeCount'], 0)
        self.assertEqual(len(result['rounds']), 10)

    def test_full_rated_block_requires_human_review_and_never_grants_acceptance(self):
        result = self.inspect()
        self.assertEqual(result['status'], 'ready_for_human_review')
        self.assertEqual(result['eligibleOutcomeCount'], 10)
        self.assertFalse(result['humanAcceptanceGranted'])
        self.assertFalse(result['balanceChangesRecommendedAutomatically'])

    def test_automated_provenance_excluded(self):
        self.session['source'] = 'automated_campaign'
        self.assertEqual(self.inspect()['eligibleOutcomeCount'], 0)

    def test_sandbox_provenance_excluded(self):
        self.session['source'] = 'manual_sandbox'
        self.assertEqual(self.inspect()['eligibleOutcomeCount'], 0)

    def test_stale_campaign_even_with_matching_rounds_is_excluded(self):
        self.session['contentVersion'] = 'old-content'
        self.assertEqual(self.inspect()['eligibleOutcomeCount'], 0)

    def test_changed_round_fingerprint_is_excluded(self):
        self.session['attempts'][0]['levelFingerprint'] = '0' * 64
        self.assertEqual(self.inspect()['eligibleOutcomeCount'], 9)

    def test_changed_rule_version_is_excluded(self):
        self.session['attempts'][0]['ruleVersion'] = -1
        self.assertEqual(self.inspect()['eligibleOutcomeCount'], 9)

    def test_different_block_is_excluded(self):
        self.session.update(campaignStartRound=211, campaignEndRound=220, fixtureCompletedThrough=210)
        self.assertEqual(self.inspect()['eligibleOutcomeCount'], 0)

    def test_prior_progress_fixture_does_not_count_as_play(self):
        self.session['attempts'] = []
        self.assertEqual(self.inspect()['eligibleOutcomeCount'], 0)

    def test_dropped_history_excludes_session(self):
        self.session['droppedAttempts'] = 1
        self.assertEqual(self.inspect()['eligibleOutcomeCount'], 0)

    def test_duplicate_attempt_numbers_exclude_session(self):
        self.session['attempts'][1]['number'] = 1
        self.assertEqual(self.inspect()['eligibleOutcomeCount'], 0)

    def test_exact_copies_are_counted_once(self):
        result = self.inspect([self.session, copy.deepcopy(self.session)])
        self.assertEqual(result['eligibleOutcomeCount'], 10)
        self.assertEqual(result['duplicateCopiesIgnored'], 1)

    def test_conflicting_same_id_snapshots_require_canonical_export(self):
        other = copy.deepcopy(self.session); other['attempts'].pop()
        result = self.inspect([self.session, other])
        self.assertEqual(result['status'], 'recording_snapshot_conflict')
        self.assertEqual(result['eligibleOutcomeCount'], 0)

    def test_conflicting_provenance_is_not_silently_ignored(self):
        other = copy.deepcopy(self.session); other['source'] = 'automated_campaign'
        self.assertEqual(self.inspect([self.session, other])['status'], 'recording_snapshot_conflict')

    def test_interruption_retained_but_not_outcome(self):
        self.session['attempts'][0]['outcome'] = 'interrupted'
        result = self.inspect()
        self.assertEqual(result['excludedAttemptCount'], 1)
        self.assertEqual(result['attempts'][0]['outcome'], 'interrupted')

    def test_active_even_if_marked_won_cannot_count_as_completed_export(self):
        self.session['activeAttempts'] = [self.session['attempts'].pop()]
        self.assertEqual(self.inspect()['eligibleOutcomeCount'], 9)

    def test_losses_and_free_retries_both_remain(self):
        loss = copy.deepcopy(self.session['attempts'][4]); loss.update(number=11, outcome='lost')
        self.session['attempts'].append(loss)
        result = self.inspect(); round205 = result['rounds'][4]
        self.assertEqual((round205['wins'], round205['losses'], round205['currentOutcomes']), (1, 1, 2))
        self.assertEqual(result['eligibleOutcomeCount'], 11)

    def test_loss_only_round_needs_successful_completion(self):
        self.session['attempts'][4]['outcome'] = 'lost'
        self.assertEqual(self.inspect()['status'], 'needs_successful_completion')

    def test_missing_rating_remains_pending(self):
        self.session['attempts'][4]['specialClarity'] = 'unrated'
        self.assertEqual(self.inspect()['status'], 'ratings_needed')

    def test_booster_use_separated_from_unassisted_outcomes(self):
        self.session['attempts'][4]['inventoryBoostersUsed'] = 1
        row = self.inspect()['rounds'][4]
        self.assertEqual((row['boosterAssistedOutcomes'], row['noInventoryBoosterOutcomes']), (1, 0))

    def test_unknown_booster_use_excluded(self):
        self.session['attempts'][0]['inventoryBoostersUsed'] = -1
        self.assertEqual(self.inspect()['eligibleOutcomeCount'], 9)

    def test_recorded_measurements_are_retained_without_invented_defaults(self):
        self.session['attempts'][0].update(score=400, movesRemaining=2, activeSeconds=80.5, goalsRemaining=[0, 0])
        attempts = self.inspect()['attempts']
        self.assertEqual((attempts[0]['score'], attempts[0]['movesRemaining'], attempts[0]['activeSeconds']), (400, 2, 80.5))
        self.assertIsNone(attempts[1]['activeSeconds'])

    def test_cli_cannot_write_reviews_into_product_inputs(self):
        with tempfile.TemporaryDirectory() as temp:
            source = Path(temp) / 'playtest.json'; source.write_text(json.dumps(self.session))
            for folder in ('Assets', 'Packages', 'ProjectSettings', 'Backend'):
                output = Path(temp) / folder / 'review'
                with patch.object(tool, 'ROOT', Path(temp)), patch.object(tool, 'load_native', return_value=self.native), \
                     patch('sys.stderr', new=io.StringIO()):
                    with self.assertRaises(SystemExit):
                        tool.main(['--session', str(source), '--output', str(output)])
                self.assertFalse(output.exists())

    def test_malformed_attempt_object_is_reported(self):
        self.session['attempts'] = [None]
        self.assertIn('invalid_attempt_object', self.inspect()['files'][0]['issues'])

    def test_numeric_strings_are_reported_without_crash(self):
        self.session['attempts'][0]['campaignRun'] = '1'
        self.assertEqual(self.inspect()['eligibleOutcomeCount'], 9)

    def test_boolean_cannot_impersonate_numeric_rule_version(self):
        self.session['attempts'][0]['ruleVersion'] = True
        self.assertEqual(self.inspect()['eligibleOutcomeCount'], 9)

    def test_reversed_dates_excluded(self):
        self.session['attempts'][0]['finishedUtc'] = '2026-10-02T12:00:00Z'
        self.assertEqual(self.inspect()['eligibleOutcomeCount'], 9)

    def test_non_finite_json_is_rejected(self):
        with tempfile.TemporaryDirectory() as temp:
            path = Path(temp) / 'playtest.json'; path.write_text('{"score": NaN}')
            with self.assertRaises(ValueError): tool.read_recording(path)

    def test_symlink_input_is_not_followed(self):
        with tempfile.TemporaryDirectory() as temp:
            original = Path(temp) / 'one.json'; original.write_text('{}')
            linked = Path(temp) / 'two.json'; linked.symlink_to(original)
            with self.assertRaises(ValueError): tool.read_recording(linked)

    def test_cli_preserves_input_and_never_invokes_subprocess(self):
        with tempfile.TemporaryDirectory() as temp:
            source = Path(temp) / 'playtest.json'; source.write_text(json.dumps(self.session))
            before = source.read_bytes(); output = Path(temp) / 'review'
            with patch.object(tool.rules.subprocess, 'run', side_effect=AssertionError('Unexpected subprocess')), redirect_stdout(io.StringIO()):
                self.assertEqual(tool.main(['--session', str(source), '--output', str(output)]), 0)
            self.assertEqual(source.read_bytes(), before)
            self.assertFalse(json.loads((output / 'Review.json').read_text())['humanAcceptanceGranted'])
            with redirect_stdout(io.StringIO()), patch('sys.stderr', new=io.StringIO()):
                with self.assertRaises(SystemExit):
                    tool.main(['--session', str(source), '--output', str(output)])

if __name__ == '__main__':
    unittest.main(verbosity=2)
