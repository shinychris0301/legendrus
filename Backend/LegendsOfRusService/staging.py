"""HTTPS-proxy entry point for isolated session staging, never live commerce."""
from http import HTTPStatus
from contextlib import closing
import json
import os
from pathlib import Path
import re
import sqlite3

from rustore import RuStoreVerifier
from service import Application, Service


class StagingApplication:
    routes = {
        ("GET", "/health"), ("GET", "/ready"),
        ("POST", "/v1/auth/bootstrap"), ("POST", "/v1/auth/refresh"),
        ("GET", "/v1/player"), ("DELETE", "/v1/session"),
    }

    def __init__(self, service, host):
        self.service, self.host = service, host
        self.application = Application(service)

    @staticmethod
    def reply(start_response, status, result):
        body = json.dumps(result).encode()
        start_response(f"{status} {HTTPStatus(status).phrase}", [
            ("Content-Type", "application/json"), ("Content-Length", str(len(body))),
            ("Cache-Control", "no-store"), ("X-Content-Type-Options", "nosniff")])
        return [body]

    def __call__(self, environ, start_response):
        # Gunicorn accepts forwarded HTTPS only from the local reverse proxy.
        # Never trust an arbitrary X-Forwarded-Host or query-string credential.
        if environ.get("wsgi.url_scheme") != "https":
            return self.reply(start_response, 400, {"error": "https_required"})
        if environ.get("HTTP_HOST") != self.host:
            return self.reply(start_response, 400, {"error": "invalid_host"})
        if environ.get("QUERY_STRING"):
            return self.reply(start_response, 400, {"error": "query_not_supported"})
        method, path = environ.get("REQUEST_METHOD"), environ.get("PATH_INFO")
        if (method, path) not in self.routes:
            return self.reply(start_response, 404, {"error": "not_found"})
        if method == "POST":
            if environ.get("CONTENT_TYPE", "").split(";", 1)[0].strip().lower() != "application/json":
                return self.reply(start_response, 415, {"error": "json_required"})
            if not environ.get("CONTENT_LENGTH"):
                return self.reply(start_response, 411, {"error": "length_required"})
        if path == "/ready":
            try:
                # mode=ro must not silently recreate a lost/deleted database.
                uri = Path(self.service.database).as_uri() + "?mode=ro"
                with closing(sqlite3.connect(uri, uri=True, timeout=2)) as db:
                    if db.execute("PRAGMA user_version").fetchone()[0] != 2:
                        raise sqlite3.DatabaseError()
                    db.execute("SELECT id FROM auth_families LIMIT 1").fetchone()
                return self.reply(start_response, 200, {"status": "ready"})
            except (sqlite3.Error, OSError):
                return self.reply(start_response, 503, {"error": "storage_unavailable"})
        try:
            return self.application(environ, start_response)
        except Exception:
            # No traceback/locals/request body containing credentials in logs.
            return self.reply(start_response, 500, {"error": "internal_error"})


def create_app():
    host = os.environ.get("LOR_API_HOST", "")
    if (len(host) > 253 or "." not in host or
            not all(re.fullmatch(r"[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?", p) for p in host.split("."))):
        raise ValueError("Configure a lowercase API hostname without scheme or port")
    path = Path(os.environ.get("LOR_DATABASE", ""))
    if not path.is_absolute() or not path.parent.is_dir() or path.is_symlink():
        raise ValueError("Configure an absolute database in an existing private directory")
    if path.parent.stat().st_mode & 0o077:
        raise ValueError("Database directory must be private (0700)")
    # This staging entry deliberately does not consume provider credentials.
    # No transient guest endpoint, commercial catalog, continuation or provider calls.
    return StagingApplication(Service(path, RuStoreVerifier("", "sandbox", lambda: "")), host)
