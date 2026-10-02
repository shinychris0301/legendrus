import io
import json
import os
from pathlib import Path
import secrets
import sqlite3
import tempfile
import unittest
from unittest.mock import patch
import uuid

from errors import ApiError
from service import Service
from snapshot_database import snapshot
from staging import create_app


class StagingTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.root = Path(self.temp.name)
        self.database = self.root / "state.db"
        self.settings = {"LOR_DATABASE": str(self.database), "LOR_API_HOST": "api.example.test"}
        with patch.dict(os.environ, self.settings):
            self.app = create_app()

    def tearDown(self):
        self.temp.cleanup()

    def call(self, path="/health", method="GET", body=b"", **changes):
        environ = {"REQUEST_METHOD": method, "PATH_INFO": path, "QUERY_STRING": "",
                   "wsgi.url_scheme": "https", "HTTP_HOST": "api.example.test",
                   "CONTENT_TYPE": "application/json", "CONTENT_LENGTH": str(len(body)),
                   "wsgi.input": io.BytesIO(body)}
        environ.update(changes)
        status = []
        headers = []
        result = b"".join(self.app(environ, lambda s, h: (status.append(int(s.split()[0])), headers.extend(h))))
        self.assertIn(("Cache-Control", "no-store"), headers)
        return status[0], json.loads(result)

    def test_factory_rejects_missing_or_ambiguous_host(self):
        for host in ("", "localhost", "https://api.example.test", "api.example.test:443", "a..test", "*.test", "a.test/path"):
            with self.subTest(host=host), patch.dict(os.environ, {**self.settings, "LOR_API_HOST": host}):
                with self.assertRaises(ValueError):
                    create_app()

    def test_factory_rejects_relative_missing_directory_and_shared_directory(self):
        for path in ("state.db", str(self.root / "missing" / "state.db")):
            with patch.dict(os.environ, {**self.settings, "LOR_DATABASE": path}):
                with self.assertRaises(ValueError):
                    create_app()
        self.root.chmod(0o755)
        try:
            with patch.dict(os.environ, self.settings), self.assertRaises(ValueError):
                create_app()
        finally:
            self.root.chmod(0o700)

    def test_https_host_query_enforced_without_header_spoofing(self):
        for changes in ({"wsgi.url_scheme": "http", "HTTP_X_FORWARDED_PROTO": "https"},
                        {"HTTP_HOST": "evil.test", "HTTP_X_FORWARDED_HOST": "api.example.test"},
                        {"QUERY_STRING": "token=synthetic"}):
            self.assertEqual(400, self.call(**changes)[0])

    def test_staging_does_not_expose_commerce_or_transient_guests(self):
        for path in ("/v1/auth/guest", "/v1/rustore/orders", "/v1/rustore/fulfill", "/v1/continuations", "/unknown"):
            self.assertEqual(404, self.call(path, "POST", b"{}")[0])

    def test_json_length_and_content_type_required(self):
        self.assertEqual(415, self.call("/v1/auth/bootstrap", "POST", b"{}", CONTENT_TYPE="text/plain")[0])
        self.assertEqual(411, self.call("/v1/auth/bootstrap", "POST", CONTENT_LENGTH="")[0])
        self.assertEqual(413, self.call("/v1/auth/bootstrap", "POST", CONTENT_LENGTH="16385")[0])

    def test_duplicate_deep_invalid_utf8_and_malformed_json_rejected(self):
        for body in (b'{"refresh_token":"x","refresh_token":"y","request_id":"z"}',
                     b'[' * 2000 + b']' * 2000, b'\xff', b'{', b'[]'):
            self.assertEqual(400, self.call("/v1/auth/bootstrap", "POST", body)[0])

    def test_readiness_checks_database_without_recreating_it(self):
        self.assertEqual((200, {"status": "ready"}), self.call("/ready"))
        self.database.rename(self.root / "saved.db")
        self.assertEqual(503, self.call("/ready")[0])
        self.assertFalse(self.database.exists())

    def test_readiness_rejects_unknown_schema(self):
        with self.app.service.connect() as db:
            db.execute("PRAGMA user_version=99")
        self.assertEqual(503, self.call("/ready")[0])

    def test_unexpected_error_is_redacted(self):
        with patch.object(self.app.service, "bootstrap_session", side_effect=RuntimeError("fixture-not-for-response")):
            code, body = self.call("/v1/auth/bootstrap", "POST", json.dumps({"refresh_token": "x", "request_id": "y"}).encode())
        self.assertEqual((500, {"error": "internal_error"}), (code, body))

    def test_snapshot_includes_wal_and_restores_session_identity(self):
        secret, request = secrets.token_hex(32), str(uuid.uuid4())
        first = self.app.service.bootstrap_session(secret, request)
        # Keep a connection open so committed data still resides in WAL during backup.
        with self.app.service.connect() as held:
            held.execute("UPDATE players SET gems=731")
            self.assertGreater(Path(str(self.database) + "-wal").stat().st_size, 0)
            backup = self.root / "backup.db"
            snapshot(self.database, backup)
        self.assertEqual(0o600, backup.stat().st_mode & 0o777)
        restored_path = self.root / "restored.db"
        snapshot(backup, restored_path)
        restored = Service(restored_path, self.app.service.verifier)
        self.assertEqual(first, restored.bootstrap_session(secret, request))
        self.assertEqual(731, restored.player(first["player_id"])["gems"])
        rotated = restored.refresh_session(secret, secrets.token_hex(32), str(uuid.uuid4()))
        restored.revoke(rotated["access_token"])
        with self.assertRaises(ApiError):
            restored.authenticate(rotated["access_token"])
        # Restored fixture is independent of the original running service.
        self.assertEqual(first["player_id"], self.app.service.authenticate(first["access_token"]))

    def test_snapshot_refuses_existing_destination_and_journal(self):
        destination = self.root / "copy.db"
        for suffix in ("", "-wal", "-shm", "-journal"):
            marker = Path(str(destination) + suffix)
            marker.write_bytes(b"preserve")
            with self.assertRaises(ValueError):
                snapshot(self.database, destination)
            self.assertEqual(b"preserve", marker.read_bytes())
            marker.unlink()

    def test_snapshot_rejects_bad_schema_and_corruption_without_publishing(self):
        for payload in (b"not-a-database", b""):
            source = self.root / "invalid.db"
            source.write_bytes(payload)
            destination = self.root / "rejected.db"
            with self.assertRaises((ValueError, sqlite3.Error)):
                snapshot(source, destination)
            self.assertFalse(destination.exists())
            self.assertEqual([], list(self.root.glob(".lor-snapshot-*")))

    def test_snapshot_rejects_symlinks_and_shared_destination(self):
        link = self.root / "link.db"
        link.symlink_to(self.database)
        with self.assertRaises(ValueError):
            snapshot(link, self.root / "copy.db")
        destination = self.root / "dangling.db"
        destination.symlink_to(self.root / "absent.db")
        with self.assertRaises(ValueError):
            snapshot(self.database, destination)
        self.root.chmod(0o755)
        try:
            with self.assertRaises(ValueError):
                snapshot(self.database, self.root / "copy.db")
        finally:
            self.root.chmod(0o700)


if __name__ == "__main__":
    unittest.main()
