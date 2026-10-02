"""Guard against accepting automated, stale or incomplete observations as human evidence."""
import copy
import json
import unittest
from AuditCoverage import BASE, attempt_issues, session_issues


class EvidenceClassificationTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.native = json.loads((BASE / 'QA/NativeAudit/InstalledContent.json').read_text())

    def setUp(self):
        round31 = next(row for row in self.native['rounds'] if row['round'] == 31)
        self.attempt = dict(number=1, round=31, chapter=1, ruleVersion=self.native['ruleVersion'],
            levelFingerprint=round31['fingerprint'], outcome='won', startedUtc='2026-10-01T12:00:00Z',
            finishedUtc='2026-10-01T12:01:00Z', snapshotSettled=True, committedAttempt=1,
            campaignRun=1, inventoryBoostersUsed=0)
        self.session = dict(schemaVersion=1, source='manual_campaign', sessionId='a' * 32,
            contentVersion=self.native['contentFingerprint'], campaignStartRound=31,
            campaignEndRound=40, fixtureCompletedThrough=30, droppedAttempts=0,
            collectionContext='CampaignActive; disposable earlier-progress fixture; 30 energy; zero starting boosters',
            attempts=[self.attempt], activeAttempts=[])

    def eligible(self):
        return not session_issues(self.session, self.native) and not attempt_issues(self.attempt, self.session, self.native)

    def test_matching_manual_outcome_is_reviewable(self):
        self.assertTrue(self.eligible())

    def test_automated_or_sandbox_ratings_cannot_supply_manual_coverage(self):
        for source in ('automated_campaign', 'automated_test', 'manual_sandbox'):
            self.session['source'] = source
            self.assertFalse(self.eligible())

    def test_unchanged_round_inside_stale_campaign_stays_historical(self):
        self.session['contentVersion'] = 'CampaignSector01R13-old'
        self.assertFalse(self.eligible())
        self.assertNotIn('round_fingerprint_mismatch', attempt_issues(self.attempt, self.session, self.native))

    def test_changed_round_or_rules_rejected_in_matching_campaign(self):
        for field, value in [('levelFingerprint', '0' * 64), ('ruleVersion', -1)]:
            original = self.attempt[field]
            self.attempt[field] = value
            self.assertFalse(self.eligible())
            self.attempt[field] = original

    def test_interrupted_and_active_snapshots_do_not_become_outcomes(self):
        for outcome in ('interrupted', 'ongoing'):
            self.attempt['outcome'] = outcome
            self.assertFalse(self.eligible())

    def test_prior_fixture_rounds_cannot_be_observed_coverage(self):
        self.session.update(campaignStartRound=41, campaignEndRound=50, fixtureCompletedThrough=40)
        self.assertFalse(self.eligible())

    def test_truncated_or_duplicate_attempt_history_rejected(self):
        self.session['droppedAttempts'] = 1
        self.assertFalse(self.eligible())
        self.session['droppedAttempts'] = 0
        self.session['attempts'].append(copy.deepcopy(self.attempt))
        self.assertFalse(self.eligible())

    def test_legacy_missing_context_is_historical_without_crashing(self):
        for field in ('campaignStartRound', 'campaignEndRound', 'fixtureCompletedThrough', 'collectionContext'):
            self.session.pop(field)
        self.assertFalse(self.eligible())
        self.assertIn('outside_recorded_block', attempt_issues(self.attempt, self.session, self.native))


if __name__ == '__main__':
    unittest.main(verbosity=2)
