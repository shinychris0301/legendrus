"""Loopback-only development runner. Never expose wsgiref as a release server."""
import argparse
import os
from pathlib import Path
from wsgiref.simple_server import make_server, WSGIRequestHandler
from service import Service, Application
from rustore import RuStoreVerifier

class QuietHandler(WSGIRequestHandler):
    def log_message(self, format, *args):
        pass

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--database", required=True)
    parser.add_argument("--port", type=int, default=8097)
    args = parser.parse_args()
    Path(args.database).parent.mkdir(parents=True, exist_ok=True)
    verifier = RuStoreVerifier(os.environ.get("RUSTORE_APP_ID", ""), "sandbox", lambda: os.environ.get("RUSTORE_PUBLIC_TOKEN", ""))
    # Empty release catalog deliberately disables purchases until IDs/recipes are approved.
    app = Application(Service(args.database, verifier))
    with make_server("127.0.0.1", args.port, app, handler_class=QuietHandler) as server:
        print("RuStore backend development listener ready on loopback", flush=True)
        server.serve_forever()
