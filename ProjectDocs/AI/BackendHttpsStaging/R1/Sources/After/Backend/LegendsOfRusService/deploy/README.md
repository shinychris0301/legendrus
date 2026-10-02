# Isolated HTTPS session staging

This package deploys the existing backend on one Linux host using nginx, Gunicorn and SQLite. It is **session staging only**: no real purchases, imported saves, ads, social accounts or commercial wallet grants. The game remains unconfigured. A server/domain has not yet been supplied; nothing has been publicly deployed.

The WSGI entry is `staging:create_app()`. It accepts bootstrap, refresh, player, logout, health and readiness only. The legacy guest and all commerce/continuation routes are closed. The provider is fixed to an unconfigured sandbox and no provider secrets are consumed. Use a separate staging hostname/database, never a production database or real customer accounts.

## Host preparation

Use a supported Linux host with systemd, Python 3.10+ and a maintained nginx supporting `ssl_reject_handshake` (1.19.4+). This package was tested locally with Python 3.14.6, Gunicorn 26.2.0 and Ubuntu nginx 1.28.3. Host distribution selection, provisioning, DNS, firewall, public certificate issuance/renewal and operational ownership remain to be completed.

1. Create the unprivileged `lor-staging` service account. Put only this backend directory at `/opt/lor-staging/current`, owned by the deployment administrator and read-only to the service. Do not upload the Unity project, player saves or local QA data.
2. Create `/opt/lor-staging/venv`; install `deploy/requirements.txt` using `pip install --require-hashes --only-binary=:all: -r deploy/requirements.txt`. The version and wheel hash are pinned. Review updates separately.
3. Create `/etc/lor-staging/config` with **only these non-secret settings**, substituting the actual hostname:

   ```text
   LOR_API_HOST=api-staging.example.com
   LOR_DATABASE=/var/lib/lor-staging/state.db
   ```

   Do not use that example hostname as a live endpoint. systemd creates `/var/lib/lor-staging` owned by the service with mode 0700. Existing state directories must have that owner/mode. The application refuses missing/private-directory misconfiguration and database symlinks.
4. Install `lor-staging.service` in `/etc/systemd/system/`. Run `systemd-analyze verify` **on that host after paths exist**, then daemon-reload and start the service. Gunicorn binds only 127.0.0.1:8097, with one process/four threads and a private umask. Do not expose 8097 or use multiple hosts sharing this SQLite file.
5. Point the actual staging DNS name to the host and provision its trusted TLS certificate using the host's approved certificate workflow. Private keys stay on the host. This repository does not generate or distribute production certificates.
6. Install `lor-staging-proxy.conf` as `/etc/nginx/snippets/lor-staging-proxy.conf`. Render `nginx.conf.template` with the actual `API_HOST` and absolute `CERT_DIRECTORY` holding `fullchain.pem` and `privkey.pem`. Include it under nginx `http {}`. This template assumes a **dedicated** host: reconcile existing default listeners first; do not overwrite other sites.
7. Run `nginx -t` before reload. Open only the required HTTPS port plus restricted administration. The example port-80 listener returns 404, **never redirects credential-bearing requests**. For HTTP-01 certificate renewal, explicitly arrange the ACME challenge location; otherwise use DNS-based issuance. Monitor expiry/renewal. The template currently binds IPv4 only; do not publish an AAAA record without an explicitly tested IPv6 configuration.

nginx buffers requests, limits bodies to 16 KiB, overwrites forwarded headers and applies per-IP limits: authentication 10 requests/minute with burst 10, other allowed routes 5/second with burst 20, eight simultaneous connections. These are conservative **staging** limits and need real-user/shared-NAT tuning and monitoring. No account-wide quotas or distributed abuse protection are claimed. Only nginx on the same trusted host should reach Gunicorn. The app enforces the exact configured host and HTTPS scheme. Query strings and arbitrary routes are rejected. Rate limits return 429; the client must retain its pending recovery request and retry later, never create a new identity to evade limits.

Access logging is disabled; do not enable request-body/header/response-body or debug logging on the host, proxy, load balancer or monitoring agent. Application errors are generic. Supervise process health and aggregate status codes separately from credentials. The local verification checked synthetic credentials were absent from logs; this is not a review of a future hosting provider's logging configuration.

## Verification and rollback

`GET https://<actual-host>/health` is liveness; `/ready` verifies a read-only connection, known database schema and session table. Readiness does not prove write capacity, backups, disk space or provider availability. Monitor those separately. Public tests must use normal TLS validation, with no Android cleartext permission or trust bypass. Review deployment before configuring the game or creating a disposable Android staging account.

For a local integration rehearsal without installing system services:

```sh
python -m unittest discover -s tests -v
python tests/verify_staging_stack.py --nginx /path/to/nginx --output /tmp/lor-stack-result.json
```

Run the latter with the pinned Gunicorn environment. It binds only loopback high ports, creates a one-day fixture certificate trusted only by its own test client, starts real nginx/Gunicorn, checks recovery after a server-process restart, and deletes its fixtures/processes. No system/device trust store changes. It exports only named check results, never tokens. This does not replace a public certificate or physical Android session test.

Before deployment changes, create a verified snapshot with the service account:

```sh
python snapshot_database.py /var/lib/lor-staging/state.db /private-backups/unique-snapshot.db
```

The destination directory must already exist with mode 0700. The tool uses SQLite's online backup API, includes committed WAL data, validates schema/integrity/foreign keys and publishes a new 0600 file without overwriting an existing file or journal. Backups contain private account state; retain/encrypt/transfer them only through the host's approved backup system. No off-host backup schedule, encryption key management or retention automation is supplied yet.

For a restore drill, use the same command with the backup as source and a **new** destination path, then run the staging service against that restored copy. Never overwrite an open database or copy only its main file while WAL is active. A live rollback requires stopping the service and ensuring no other writer exists, a fresh pre-rollback snapshot, and a reviewed switch of `LOR_DATABASE`. Restoring older state can lose later grants/revocations and invalidate device recovery credentials; do not treat it as a safe commercial rollback. Keep the prior code release and current database untouched until acceptance. Schema-changing code rollback needs its own migration plan.

Next: select/provide the host and domain, provision and validate public TLS, then implement explicit Android staging-session entry and run bootstrap → app restart → renewal → interrupted response recovery on the connected phone. Account linking/deletion, offline reconciliation, server round authority and RuStore sandbox commerce follow separately.

Sources checked 2026-09-30: [Gunicorn deployment](https://gunicorn.org/deploy/), [Gunicorn settings](https://gunicorn.org/reference/settings/), [nginx request limits](https://nginx.org/en/docs/http/ngx_http_limit_req_module.html), [nginx proxy buffering](https://nginx.org/en/docs/http/ngx_http_proxy_module.html), [Python SQLite backup](https://docs.python.org/3/library/sqlite3.html#sqlite3.Connection.backup).
