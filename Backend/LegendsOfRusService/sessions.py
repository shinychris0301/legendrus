"""Device-bound guest recovery. Secrets are hashed; pending rotations are retryable."""
import hashlib
import hmac
import re
import uuid
from errors import ApiError

ACCESS_SECONDS = 86400
REFRESH_SECONDS = 30 * 86400


def validate_request(secret, request_id):
    if not isinstance(secret, str) or not re.fullmatch(r"[0-9a-f]{64}", secret):
        raise ApiError(400, "invalid_session_request")
    try:
        if str(uuid.UUID(request_id)) != request_id:
            raise ValueError()
    except (ValueError, TypeError, AttributeError):
        raise ApiError(400, "invalid_session_request") from None


def access_token(secret, request_id):
    # Deterministic only for this high-entropy secret and issuance ID. This lets a lost
    # response be recovered without storing plaintext bearer credentials on the server.
    return hmac.new(bytes.fromhex(secret), ("lor-access-v1:" + request_id).encode(), hashlib.sha256).hexdigest()


def migrate_sessions(db):
    db.execute("BEGIN IMMEDIATE")
    try:
        if db.execute("PRAGMA user_version").fetchone()[0] < 2:
            db.execute("CREATE TABLE auth_families(id TEXT PRIMARY KEY, player_id TEXT NOT NULL REFERENCES players(id), revoked INTEGER NOT NULL DEFAULT 0)")
            db.execute("ALTER TABLE sessions ADD COLUMN family_id TEXT REFERENCES auth_families(id)")
            db.execute('''CREATE TABLE refresh_credentials(token_hash TEXT PRIMARY KEY, family_id TEXT NOT NULL REFERENCES auth_families(id),
                issuance_id TEXT NOT NULL, access_expires INTEGER NOT NULL, refresh_expires INTEGER NOT NULL,
                consumed_request TEXT, next_hash TEXT)''')
            db.execute("DROP TABLE schema_info")
            db.execute("CREATE TABLE schema_info(version INTEGER PRIMARY KEY CHECK(version=2))")
            db.execute("INSERT INTO schema_info VALUES(2)")
            db.execute("PRAGMA user_version=2")
        db.execute("COMMIT")
    except BaseException:
        db.execute("ROLLBACK")
        raise


class RecoverableSessions:
    def _issue(self, db, family, secret, request_id):
        now = int(self.clock())
        expiry, refresh_expiry = now + ACCESS_SECONDS, now + REFRESH_SECONDS
        db.execute("INSERT INTO refresh_credentials VALUES(?,?,?,?,?,NULL,NULL)",
                   (self.token_hash(secret), family, request_id, expiry, refresh_expiry))
        db.execute("INSERT INTO sessions(token_hash,player_id,expires_at,family_id) SELECT ?,player_id,?,id FROM auth_families WHERE id=?",
                   (self.token_hash(access_token(secret, request_id)), expiry, family))

    def _response(self, db, row, secret):
        family = db.execute("SELECT * FROM auth_families WHERE id=?", (row["family_id"],)).fetchone()
        if family["revoked"] or row["consumed_request"] or row["refresh_expires"] <= self.clock():
            raise ApiError(401, "session_recovery_required")
        return {"player_id": family["player_id"], "access_token": access_token(secret, row["issuance_id"]),
                "expires_at": row["access_expires"], "refresh_expires_at": row["refresh_expires"]}

    def bootstrap_session(self, secret, request_id):
        validate_request(secret, request_id)
        hashed = self.token_hash(secret)
        with self.transaction() as db:
            row = db.execute("SELECT * FROM refresh_credentials WHERE token_hash=?", (hashed,)).fetchone()
            if row:
                if row["issuance_id"] != request_id:
                    raise ApiError(409, "session_request_conflict")
                return self._response(db, row, secret)
            player, family = str(uuid.uuid4()), str(uuid.uuid4())
            db.execute("INSERT INTO players VALUES(?,0)", (player,))
            db.execute("INSERT INTO auth_families VALUES(?,?,0)", (family, player))
            self._issue(db, family, secret, request_id)
            row = db.execute("SELECT * FROM refresh_credentials WHERE token_hash=?", (hashed,)).fetchone()
            return self._response(db, row, secret)

    def refresh_session(self, secret, next_secret, request_id):
        validate_request(secret, request_id)
        validate_request(next_secret, request_id)
        if secret == next_secret:
            raise ApiError(400, "refresh_must_rotate")
        hashed, next_hash = self.token_hash(secret), self.token_hash(next_secret)
        with self.transaction() as db:
            row = db.execute("SELECT * FROM refresh_credentials WHERE token_hash=?", (hashed,)).fetchone()
            if row is None or row["refresh_expires"] <= self.clock():
                raise ApiError(401, "session_recovery_required")
            family = db.execute("SELECT * FROM auth_families WHERE id=?", (row["family_id"],)).fetchone()
            if family["revoked"]:
                raise ApiError(401, "session_recovery_required")
            if row["consumed_request"]:
                if row["consumed_request"] != request_id or row["next_hash"] != next_hash:
                    raise ApiError(401, "refresh_replay_rejected")
                child = db.execute("SELECT * FROM refresh_credentials WHERE token_hash=?", (next_hash,)).fetchone()
                return self._response(db, child, next_secret)
            if db.execute("SELECT 1 FROM refresh_credentials WHERE token_hash=?", (next_hash,)).fetchone():
                raise ApiError(409, "refresh_conflict")
            db.execute("DELETE FROM sessions WHERE family_id=?", (family["id"],))
            db.execute("UPDATE refresh_credentials SET consumed_request=?,next_hash=? WHERE token_hash=?", (request_id, next_hash, hashed))
            self._issue(db, family["id"], next_secret, request_id)
            child = db.execute("SELECT * FROM refresh_credentials WHERE token_hash=?", (next_hash,)).fetchone()
            return self._response(db, child, next_secret)
