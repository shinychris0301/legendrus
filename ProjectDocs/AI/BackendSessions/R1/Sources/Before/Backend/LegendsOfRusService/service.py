"""Small self-hosted backend foundation. SQLite persistence; local WSGI runner only."""
from contextlib import contextmanager
from dataclasses import dataclass
import hashlib
import json
import secrets
import sqlite3
import time
import uuid
from rustore import RuStoreVerifier, VerificationError, invoice_id


class ApiError(Exception):
    def __init__(self, status, code):
        self.status, self.code = status, code


@dataclass(frozen=True)
class Product:
    product_id: str
    price_minor: int
    gems: int

    def __post_init__(self):
        if not self.product_id or len(self.product_id) > 128 or type(self.price_minor) is not int or type(self.gems) is not int or not (0 < self.price_minor <= 2_000_000_000 and 0 < self.gems <= 2_000_000_000):
            raise ValueError("Invalid server product recipe")


class Service:
    def __init__(self, database, verifier, products=(), clock=time.time):
        self.database, self.verifier, self.clock = str(database), verifier, clock
        self.products = {p.product_id: p for p in products}
        if len(self.products) != len(products):
            raise ValueError("Duplicate product")
        with self.connect() as db:
            if db.execute("PRAGMA user_version").fetchone()[0] not in (0, 1):
                raise ValueError("Unsupported backend database schema")
            db.executescript('''
                PRAGMA journal_mode=WAL;
                PRAGMA user_version=1;
                CREATE TABLE IF NOT EXISTS schema_info(version INTEGER PRIMARY KEY CHECK(version=1));
                INSERT OR IGNORE INTO schema_info VALUES(1);
                CREATE TABLE IF NOT EXISTS players(id TEXT PRIMARY KEY, gems INTEGER NOT NULL CHECK(gems>=0 AND gems<=2147483647));
                CREATE TABLE IF NOT EXISTS sessions(token_hash TEXT PRIMARY KEY, player_id TEXT NOT NULL REFERENCES players(id), expires_at INTEGER NOT NULL);
                CREATE TABLE IF NOT EXISTS orders(id TEXT PRIMARY KEY, player_id TEXT NOT NULL REFERENCES players(id), request_id TEXT NOT NULL,
                    product_id TEXT NOT NULL, price_minor INTEGER NOT NULL, gems INTEGER NOT NULL, environment TEXT NOT NULL, app_id TEXT NOT NULL,
                    UNIQUE(player_id,request_id));
                CREATE TABLE IF NOT EXISTS grants(environment TEXT NOT NULL, invoice_id TEXT NOT NULL, order_id TEXT NOT NULL UNIQUE REFERENCES orders(id),
                    player_id TEXT NOT NULL REFERENCES players(id), gems INTEGER NOT NULL, created_at INTEGER NOT NULL,
                    PRIMARY KEY(environment,invoice_id));
            ''')

    @contextmanager
    def connect(self):
        db = sqlite3.connect(self.database, timeout=5, isolation_level=None)
        db.row_factory = sqlite3.Row
        db.execute("PRAGMA foreign_keys=ON")
        db.execute("PRAGMA synchronous=FULL")
        try:
            yield db
        finally:
            db.close()

    @contextmanager
    def transaction(self):
        with self.connect() as db:
            db.execute("BEGIN IMMEDIATE")
            try:
                yield db
                db.execute("COMMIT")
            except BaseException:
                db.execute("ROLLBACK")
                raise

    def guest(self):
        player, token = str(uuid.uuid4()), secrets.token_urlsafe(32)
        expires = int(self.clock()) + 86400
        with self.transaction() as db:
            # No client balance import or implicit free economic grant.
            db.execute("INSERT INTO players VALUES(?,0)", (player,))
            db.execute("INSERT INTO sessions VALUES(?,?,?)", (self.token_hash(token), player, expires))
        return {"player_id": player, "access_token": token, "expires_at": expires}

    @staticmethod
    def token_hash(token):
        return hashlib.sha256(token.encode()).hexdigest()

    def authenticate(self, token):
        if not isinstance(token, str) or not 32 <= len(token) <= 128:
            raise ApiError(401, "unauthorized")
        with self.connect() as db:
            row = db.execute("SELECT player_id,expires_at FROM sessions WHERE token_hash=?", (self.token_hash(token),)).fetchone()
        if row is None or row["expires_at"] <= self.clock():
            raise ApiError(401, "unauthorized")
        return row["player_id"]

    def revoke(self, token):
        with self.transaction() as db:
            db.execute("DELETE FROM sessions WHERE token_hash=?", (self.token_hash(token),))
        return {"revoked": True}

    def player(self, player):
        with self.connect() as db:
            row = db.execute("SELECT id,gems FROM players WHERE id=?", (player,)).fetchone()
        if not row:
            raise ApiError(401, "unauthorized")
        return dict(row)

    def order(self, player, product_id, request_id):
        try:
            if str(uuid.UUID(request_id)) != request_id:
                raise ValueError()
        except (ValueError, TypeError, AttributeError):
            raise ApiError(400, "invalid_request_id") from None
        with self.transaction() as db:
            previous = db.execute("SELECT * FROM orders WHERE player_id=? AND request_id=?", (player, request_id)).fetchone()
            if previous:
                if previous["product_id"] != product_id:
                    raise ApiError(409, "request_conflict")
                return dict(previous)
            product = self.products.get(product_id)
            if not product:
                raise ApiError(503, "product_not_configured")
            order_id = str(uuid.uuid4())
            db.execute("INSERT INTO orders VALUES(?,?,?,?,?,?,?,?)", (order_id, player, request_id, product.product_id, product.price_minor, product.gems, self.verifier.environment, self.verifier.app_id))
            return dict(db.execute("SELECT * FROM orders WHERE id=?", (order_id,)).fetchone())

    def fulfill(self, player, order_id, invoice):
        try:
            invoice = invoice_id(invoice)
        except VerificationError:
            raise ApiError(400, "invalid_invoice") from None
        with self.connect() as db:
            order = db.execute("SELECT * FROM orders WHERE id=? AND player_id=?", (order_id, player)).fetchone()
        if not order:
            raise ApiError(404, "order_not_found")
        if order["environment"] != self.verifier.environment:
            raise ApiError(409, "environment_mismatch")
        if order["app_id"] != self.verifier.app_id:
            raise ApiError(409, "app_mismatch")
        try:
            evidence = self.verifier.verify(invoice, order)
        except VerificationError:
            raise ApiError(503, "purchase_not_verified") from None
        with self.transaction() as db:
            grant = db.execute("SELECT * FROM grants WHERE environment=? AND invoice_id=?", (evidence["environment"], invoice)).fetchone()
            if grant:
                if grant["player_id"] != player or grant["order_id"] != order_id:
                    raise ApiError(409, "invoice_already_assigned")
                return {"granted": True, "duplicate": True, "acknowledgement_required": not evidence["acknowledged"]}
            if evidence["acknowledged"]:
                raise ApiError(409, "acknowledged_without_local_grant")
            if db.execute("SELECT 1 FROM grants WHERE order_id=?", (order_id,)).fetchone():
                raise ApiError(409, "order_already_fulfilled")
            balance = db.execute("SELECT gems FROM players WHERE id=?", (player,)).fetchone()["gems"]
            if balance + order["gems"] > 2147483647:
                raise ApiError(409, "balance_limit")
            db.execute("INSERT INTO grants VALUES(?,?,?,?,?,?)", (evidence["environment"], invoice, order_id, player, order["gems"], int(self.clock())))
            db.execute("UPDATE players SET gems=gems+? WHERE id=?", (order["gems"], player))
        return {"granted": True, "duplicate": False, "acknowledgement_required": True}


class Application:
    def __init__(self, service):
        self.service = service

    def __call__(self, environ, start_response):
        try:
            method, path = environ["REQUEST_METHOD"], environ["PATH_INFO"]
            length = int(environ.get("CONTENT_LENGTH") or "0")
            if not 0 <= length <= 16384:
                raise ApiError(413, "request_too_large")
            data = json.loads(environ["wsgi.input"].read(length)) if length else {}
            if not isinstance(data, dict):
                raise ApiError(400, "invalid_request")
            if (method, path) == ("GET", "/health"):
                result, status = {"status": "ok", "store": "rustore", "environment": self.service.verifier.environment, "continuation_enabled": False}, 200
            elif (method, path) == ("POST", "/v1/auth/guest"):
                self.fields(data, set());result, status = self.service.guest(), 201
            else:
                header = environ.get("HTTP_AUTHORIZATION", "")
                if not header.startswith("Bearer "):
                    raise ApiError(401, "unauthorized")
                token = header[7:];player = self.service.authenticate(token);status = 200
                if (method, path) == ("GET", "/v1/player"):
                    self.fields(data, set());result = self.service.player(player)
                elif (method, path) == ("DELETE", "/v1/session"):
                    self.fields(data, set());result = self.service.revoke(token)
                elif (method, path) == ("POST", "/v1/rustore/orders"):
                    self.fields(data, {"product_id", "request_id"});result = self.service.order(player, data["product_id"], data["request_id"])
                elif (method, path) == ("POST", "/v1/rustore/fulfill"):
                    self.fields(data, {"order_id", "invoice_id"});result = self.service.fulfill(player, data["order_id"], data["invoice_id"])
                elif (method, path) == ("POST", "/v1/continuations"):
                    raise ApiError(503, "round_authority_unavailable")
                else:
                    raise ApiError(404, "not_found")
        except ApiError as error:
            result, status = {"error": error.code}, error.status
        except (ValueError, TypeError, KeyError):
            result, status = {"error": "invalid_request"}, 400
        except (sqlite3.Error, OSError):
            result, status = {"error": "storage_unavailable"}, 503
        body = json.dumps(result).encode()
        from http import HTTPStatus
        start_response(f"{status} {HTTPStatus(status).phrase}", [("Content-Type", "application/json"), ("Content-Length", str(len(body))), ("Cache-Control", "no-store")])
        return [body]

    @staticmethod
    def fields(data, allowed):
        if set(data) != allowed or any(not isinstance(value, str) or len(value) > 256 for value in data.values()):
            raise ApiError(400, "invalid_request")
