import concurrent.futures
import hashlib
import json
from pathlib import Path
import secrets
import sqlite3
import tempfile
import unittest
import uuid
from service import Service, ApiError
from rustore import RuStoreVerifier
from sessions import REFRESH_SECONDS


class SessionTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.path = Path(self.temp.name) / 'state.db'
        self.now = [1000]
        self.verifier = RuStoreVerifier('123', 'sandbox', lambda: '')
        self.service = Service(self.path, self.verifier, clock=lambda: self.now[0])
        self.secret, self.request = secrets.token_hex(32), str(uuid.uuid4())
        self.initial = self.service.bootstrap_session(self.secret, self.request)

    def tearDown(self):
        self.temp.cleanup()

    def assert_error(self, code, action):
        with self.assertRaises(ApiError) as caught:
            action()
        self.assertEqual(code, caught.exception.code)

    def test_bootstrap_retry_has_one_identity_and_no_plaintext_secrets(self):
        self.assertEqual(self.initial, self.service.bootstrap_session(self.secret, self.request))
        with self.service.connect() as db:
            self.assertEqual(1, db.execute('SELECT count(*) FROM players').fetchone()[0])
            dump = '\n'.join(db.iterdump())
        self.assertNotIn(self.secret, dump)
        self.assertNotIn(self.initial['access_token'], dump)
        self.assert_error('session_request_conflict', lambda: self.service.bootstrap_session(self.secret, str(uuid.uuid4())))

    def test_expired_access_renews_same_player_and_preserves_wallet(self):
        with self.service.connect() as db:
            db.execute('UPDATE players SET gems=123')
        self.now[0] += 86401
        self.assert_error('unauthorized', lambda: self.service.authenticate(self.initial['access_token']))
        rotated = self.service.refresh_session(self.secret, secrets.token_hex(32), str(uuid.uuid4()))
        self.assertEqual(self.initial['player_id'], self.service.authenticate(rotated['access_token']))
        self.assertEqual(123, self.service.player(rotated['player_id'])['gems'])

    def test_lost_refresh_response_retried_after_server_restart(self):
        next_secret, request = secrets.token_hex(32), str(uuid.uuid4())
        rotated = self.service.refresh_session(self.secret, next_secret, request)
        self.service = Service(self.path, self.verifier, clock=lambda: self.now[0])
        self.assertEqual(rotated, self.service.refresh_session(self.secret, next_secret, request))
        self.assert_error('unauthorized', lambda: self.service.authenticate(self.initial['access_token']))
        self.assert_error('refresh_replay_rejected', lambda: self.service.refresh_session(self.secret, secrets.token_hex(32), request))
        self.assert_error('refresh_replay_rejected', lambda: self.service.refresh_session(self.secret, next_secret, str(uuid.uuid4())))

    def test_consumed_child_cannot_be_resurrected(self):
        second, third, request = secrets.token_hex(32), secrets.token_hex(32), str(uuid.uuid4())
        self.service.refresh_session(self.secret, second, request)
        latest = self.service.refresh_session(second, third, str(uuid.uuid4()))
        self.assert_error('session_recovery_required', lambda: self.service.refresh_session(self.secret, second, request))
        self.assertEqual(self.initial['player_id'], self.service.authenticate(latest['access_token']))

    def test_logout_revokes_whole_family_but_not_another_player(self):
        next_secret, request = secrets.token_hex(32), str(uuid.uuid4())
        rotated = self.service.refresh_session(self.secret, next_secret, request)
        other = self.service.bootstrap_session(secrets.token_hex(32), str(uuid.uuid4()))
        self.service.revoke(rotated['access_token'])
        self.assert_error('unauthorized', lambda: self.service.authenticate(rotated['access_token']))
        self.assert_error('session_recovery_required', lambda: self.service.refresh_session(self.secret, next_secret, request))
        self.assert_error('session_recovery_required', lambda: self.service.refresh_session(next_secret, secrets.token_hex(32), str(uuid.uuid4())))
        self.assertEqual(other['player_id'], self.service.authenticate(other['access_token']))

    def test_refresh_expiry_does_not_silently_create_player(self):
        self.now[0] += REFRESH_SECONDS + 1
        self.assert_error('session_recovery_required', lambda: self.service.refresh_session(self.secret, secrets.token_hex(32), str(uuid.uuid4())))
        self.assert_error('session_recovery_required', lambda: self.service.bootstrap_session(self.secret, self.request))
        with self.service.connect() as db:
            self.assertEqual(1, db.execute('SELECT count(*) FROM players').fetchone()[0])

    def test_parallel_bootstrap_and_refresh_are_idempotent(self):
        second, request = secrets.token_hex(32), str(uuid.uuid4())
        with concurrent.futures.ThreadPoolExecutor(max_workers=8) as pool:
            values = list(pool.map(lambda _: self.service.bootstrap_session(self.secret, self.request), range(16)))
            rotations = list(pool.map(lambda _: self.service.refresh_session(self.secret, second, request), range(16)))
        self.assertTrue(all(v == self.initial for v in values))
        self.assertTrue(all(v == rotations[0] for v in rotations))
        with self.service.connect() as db:
            self.assertEqual(1, db.execute('SELECT count(*) FROM sessions').fetchone()[0])

    def test_competing_rotations_have_one_winner(self):
        def rotate(_):
            try:
                return self.service.refresh_session(self.secret, secrets.token_hex(32), str(uuid.uuid4()))
            except ApiError:
                return None
        with concurrent.futures.ThreadPoolExecutor(max_workers=8) as pool:
            results = list(pool.map(rotate, range(16)))
        self.assertEqual(1, sum(v is not None for v in results))

    def test_rotation_failure_rolls_back_credentials_and_access(self):
        with self.service.connect() as db:
            db.execute("CREATE TRIGGER fail_rotation BEFORE INSERT ON refresh_credentials BEGIN SELECT RAISE(ABORT,'fixture'); END;")
        second, request = secrets.token_hex(32), str(uuid.uuid4())
        with self.assertRaises(sqlite3.Error):
            self.service.refresh_session(self.secret, second, request)
        self.assertEqual(self.initial['player_id'], self.service.authenticate(self.initial['access_token']))
        with self.service.connect() as db:
            db.execute('DROP TRIGGER fail_rotation')
        self.assertEqual(self.initial['player_id'], self.service.refresh_session(self.secret, second, request)['player_id'])

    def test_weak_malformed_reused_and_foreign_credentials_rejected(self):
        for secret in ['', 'a'*63, 'G'*64, None]:
            self.assert_error('invalid_session_request', lambda: self.service.bootstrap_session(secret, self.request))
        self.assert_error('refresh_must_rotate', lambda: self.service.refresh_session(self.secret, self.secret, self.request))
        other = secrets.token_hex(32)
        self.service.bootstrap_session(other, str(uuid.uuid4()))
        self.assert_error('refresh_conflict', lambda: self.service.refresh_session(self.secret, other, str(uuid.uuid4())))
        self.assert_error('session_recovery_required', lambda: self.service.refresh_session(secrets.token_hex(32), other, str(uuid.uuid4())))

    def test_http_recovery_contract_rejects_extra_fields_and_does_not_echo_secrets(self):
        import io
        from service import Application
        app = Application(self.service)
        def call(path, data):
            body = json.dumps(data).encode()
            status = []
            result = b''.join(app({'REQUEST_METHOD':'POST', 'PATH_INFO':path, 'CONTENT_LENGTH':str(len(body)),
                                  'wsgi.input':io.BytesIO(body)}, lambda s,h:status.append(int(s.split()[0]))))
            return status[0], json.loads(result)
        code, replay = call('/v1/auth/bootstrap', {'refresh_token':self.secret, 'request_id':self.request})
        self.assertEqual(200, code)
        self.assertEqual(self.initial, replay)
        second, request = secrets.token_hex(32), str(uuid.uuid4())
        code, renewed = call('/v1/auth/refresh', {'refresh_token':self.secret, 'next_refresh_token':second, 'request_id':request})
        self.assertEqual(200, code)
        self.assertEqual(self.initial['player_id'], renewed['player_id'])
        code, error = call('/v1/auth/refresh', {'refresh_token':self.secret, 'next_refresh_token':second,
                                              'request_id':str(uuid.uuid4())})
        self.assertEqual(401, code)
        self.assertNotIn(self.secret, json.dumps(error))
        self.assertEqual(400, call('/v1/auth/bootstrap', {'refresh_token':self.secret, 'request_id':self.request, 'gems':'999'})[0])

    def test_v1_migration_preserves_legacy_session_wallet_and_schema(self):
        # Independent actual v1 schema fixture, not a version-2 database with a changed flag.
        legacy_path = Path(self.temp.name) / 'legacy.db'
        with sqlite3.connect(legacy_path) as db:
            db.executescript('''PRAGMA user_version=1;
                CREATE TABLE schema_info(version INTEGER PRIMARY KEY CHECK(version=1)); INSERT INTO schema_info VALUES(1);
                CREATE TABLE players(id TEXT PRIMARY KEY,gems INTEGER NOT NULL);
                CREATE TABLE sessions(token_hash TEXT PRIMARY KEY,player_id TEXT NOT NULL REFERENCES players(id),expires_at INTEGER NOT NULL);
                INSERT INTO players VALUES('old-player',77);''')
            db.execute('INSERT INTO sessions VALUES(?,?,?)', (hashlib.sha256(b'x'*43).hexdigest(), 'old-player', 999999))
        migrated = Service(legacy_path, self.verifier, clock=lambda: self.now[0])
        self.assertEqual('old-player', migrated.authenticate('x'*43))
        self.assertEqual(77, migrated.player('old-player')['gems'])
        with migrated.connect() as db:
            self.assertEqual(2, db.execute('PRAGMA user_version').fetchone()[0])
            self.assertEqual(2, db.execute('SELECT version FROM schema_info').fetchone()[0])
        self.assertEqual('old-player', Service(legacy_path, self.verifier, clock=lambda:self.now[0]).authenticate('x'*43))
