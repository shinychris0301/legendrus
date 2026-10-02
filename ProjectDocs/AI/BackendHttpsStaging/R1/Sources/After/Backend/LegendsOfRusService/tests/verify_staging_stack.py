"""Opt-in real nginx/TLS/Gunicorn test. Disposable data, loopback listeners only.

Run with a Python environment containing deploy/requirements.txt:
python tests/verify_staging_stack.py --nginx /path/to/nginx --output /path/to/report.json
The fixture certificate is trusted only by this process; no system/device trust change.
"""
import argparse
import http.client
import json
import os
from pathlib import Path
import secrets
import socket
import ssl
import subprocess
import sys
import tempfile
import time
import uuid


def port():
    with socket.socket() as listener:
        listener.bind(("127.0.0.1", 0))
        return listener.getsockname()[1]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--nginx", required=True)
    parser.add_argument("--output", required=True)
    args = parser.parse_args()
    root = Path(__file__).resolve().parents[1]
    checks = []
    def check(name, condition):
        if not condition:
            raise RuntimeError("Failed check: " + name)
        checks.append(name)
    def stop(process):
        if process and process.poll() is None:
            process.terminate()
            try:
                process.wait(timeout=35)
            except subprocess.TimeoutExpired:
                process.kill()
                process.wait(timeout=5)
    with tempfile.TemporaryDirectory(prefix="lor-staging-stack-") as folder:
        temp = Path(folder)
        backend_port, https_port, http_port = port(), port(), port()
        env = {"PATH": os.environ["PATH"], "LANG": "C.UTF-8", "PYTHONDONTWRITEBYTECODE": "1",
               "LOR_API_HOST": "api.example.test", "LOR_DATABASE": str(temp / "state.db")}
        subprocess.run(["openssl", "req", "-x509", "-newkey", "rsa:2048", "-nodes", "-days", "1",
                        "-subj", "/CN=api.example.test", "-addext", "subjectAltName=DNS:api.example.test",
                        "-keyout", str(temp / "privkey.pem"), "-out", str(temp / "fullchain.pem")],
                       check=True, capture_output=True, timeout=30)
        (temp / "privkey.pem").chmod(0o600)
        proxy = (root / "deploy/lor-staging-proxy.conf").read_text().replace("127.0.0.1:8097", f"127.0.0.1:{backend_port}")
        (temp / "proxy.conf").write_text(proxy)
        config = (root / "deploy/nginx.conf.template").read_text()
        config = config.replace("listen 80", f"listen 127.0.0.1:{http_port}")
        config = config.replace("listen 443", f"listen 127.0.0.1:{https_port}")
        config = config.replace("API_HOST", "api.example.test").replace("CERT_DIRECTORY", str(temp))
        config = config.replace("/etc/nginx/snippets/lor-staging-proxy.conf", str(temp / "proxy.conf"))
        config = config.replace("/var/log/nginx/lor-staging-error.log", str(temp / "nginx-error.log"))
        full = (f"pid {temp}/nginx.pid; error_log {temp}/nginx-master.log crit;\n"
                f"events {{ worker_connections 128; }}\nhttp {{ access_log off; client_body_temp_path {temp}/body; "
                f"proxy_temp_path {temp}/proxy; fastcgi_temp_path {temp}/fastcgi; "
                f"uwsgi_temp_path {temp}/uwsgi; scgi_temp_path {temp}/scgi;\n{config}\n}}\n")
        (temp / "nginx.conf").write_text(full)
        nginx_command = [args.nginx, "-p", str(temp), "-c", str(temp / "nginx.conf")]
        validated = subprocess.run(nginx_command + ["-t"], capture_output=True, timeout=10)
        if validated.returncode:
            # Validation occurs before any requests or credentials exist.
            print(validated.stderr.decode(), file=sys.stderr)
        check("nginx_configuration_valid", validated.returncode == 0)
        context = ssl.create_default_context(cafile=str(temp / "fullchain.pem"))

        def request(path, method="GET", payload=None, token=None, host="api.example.test", raw=None):
            connection = http.client.HTTPConnection("127.0.0.1", https_port, timeout=10)
            sock = socket.create_connection(("127.0.0.1", https_port), timeout=10)
            connection.sock = context.wrap_socket(sock, server_hostname="api.example.test")
            headers = {"Host": host, "Content-Type": "application/json"}
            if token:
                headers["Authorization"] = "Bearer " + token
            body = raw if raw is not None else json.dumps(payload).encode() if payload is not None else None
            try:
                connection.request(method, path, body=body, headers=headers)
                response = connection.getresponse()
                data = response.read(65537)
                return response.status, data, response.getheader("Cache-Control", "")
            finally:
                connection.close()

        log = (temp / "gunicorn.log").open("w+")
        def start():
            return subprocess.Popen([sys.executable, "-m", "gunicorn", "--config", "deploy/gunicorn.conf.py",
                                     "--bind", f"127.0.0.1:{backend_port}", "staging:create_app()"],
                                    cwd=root, env=env, stdout=log, stderr=log)
        server = proxy_process = None
        try:
            server = start()
            proxy_process = subprocess.Popen(nginx_command + ["-g", "daemon off;"], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
            def ready():
                deadline = time.monotonic() + 15
                while time.monotonic() < deadline:
                    try:
                        if request("/ready")[0] == 200:
                            return
                    except (OSError, http.client.HTTPException):
                        pass
                    time.sleep(0.3)
                raise RuntimeError("Staging stack did not become ready")
            ready()
            check("https_readiness", request("/ready")[0] == 200)
            check("responses_not_cached", "no-store" in request("/health")[2])
            with socket.create_connection(("127.0.0.1", https_port)) as sock:
                try:
                    ssl.create_default_context().wrap_socket(sock, server_hostname="api.example.test")
                    rejected = False
                except ssl.SSLCertVerificationError:
                    rejected = True
            check("untrusted_certificate_rejected", rejected)
            with socket.create_connection(("127.0.0.1", https_port)) as sock:
                try:
                    context.wrap_socket(sock, server_hostname="wrong.example.test")
                    rejected = False
                except ssl.SSLError:
                    rejected = True
            check("unknown_tls_hostname_rejected", rejected)
            plain = http.client.HTTPConnection("127.0.0.1", http_port, timeout=5)
            plain.request("GET", "/health", headers={"Host": "api.example.test"})
            response = plain.getresponse()
            check("plaintext_no_redirect_or_api", response.status == 404)
            response.read(); plain.close()
            check("query_rejected", request("/health?token=fixture")[0] == 400)
            check("oversized_body_rejected", request("/v1/auth/bootstrap", "POST", raw=b"x" * 16385)[0] == 413)
            check("commerce_closed", request("/v1/rustore/orders", "POST", {})[0] == 404)
            check("transient_guest_closed", request("/v1/auth/guest", "POST", {})[0] == 404)
            secret, next_secret = secrets.token_hex(32), secrets.token_hex(32)
            bootstrap = {"refresh_token": secret, "request_id": str(uuid.uuid4())}
            code, data, _ = request("/v1/auth/bootstrap", "POST", bootstrap)
            check("bootstrap_over_tls", code == 200)
            initial = json.loads(data)
            code, data, _ = request("/v1/auth/bootstrap", "POST", bootstrap)
            check("bootstrap_retry_same_account", code == 200 and json.loads(data) == initial)
            code, data, _ = request("/v1/player", token=initial["access_token"])
            check("authenticated_zero_wallet", code == 200 and json.loads(data) == {"id": initial["player_id"], "gems": 0})
            rotate = {"refresh_token": secret, "next_refresh_token": next_secret, "request_id": str(uuid.uuid4())}
            code, data, _ = request("/v1/auth/refresh", "POST", rotate)
            check("rotation", code == 200)
            rotated = json.loads(data)
            stop(server); server = start(); ready()
            code, data, _ = request("/v1/auth/refresh", "POST", rotate)
            check("lost_response_retry_after_process_restart", code == 200 and json.loads(data) == rotated)
            check("old_access_revoked", request("/v1/player", token=initial["access_token"])[0] == 401)
            check("new_access_after_restart", request("/v1/player", token=rotated["access_token"])[0] == 200)
            check("logout", request("/v1/session", "DELETE", token=rotated["access_token"])[0] == 200)
            check("logout_revokes_access", request("/v1/player", token=rotated["access_token"])[0] == 401)
            check("logout_revokes_recovery", request("/v1/auth/refresh", "POST", rotate)[0] == 401)
            statuses = [request("/v1/auth/bootstrap", "POST", {})[0] for _ in range(18)]
            check("auth_rate_limit_429", 429 in statuses)
            stop(server); server = None
            check("readiness_fails_when_backend_down", request("/ready")[0] == 502)
            log.flush()
            # Only synthetic credentials exist here. Test absence; never output them or raw logs.
            logs = (temp / "gunicorn.log").read_text() + "".join(p.read_text() for p in temp.glob("nginx-*.log"))
            check("no_fixture_credentials_in_logs", not any(value in logs for value in
                  [secret, next_secret, initial["access_token"], rotated["access_token"]]))
        finally:
            stop(proxy_process); stop(server); log.close()
    report = {"passed": len(checks), "failed": 0, "checks": checks,
              "scope": "Local loopback nginx TLS + Gunicorn; disposable certificate and database",
              "publicDeployment": False, "androidSessionTested": False, "credentialValuesExported": False}
    Path(args.output).write_text(json.dumps(report, indent=2) + "\n")
    print(json.dumps(report))


if __name__ == "__main__":
    main()
