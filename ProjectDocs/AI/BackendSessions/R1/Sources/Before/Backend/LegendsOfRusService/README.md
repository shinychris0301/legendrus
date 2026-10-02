# Legends of Rus — RuStore backend foundation

The confirmed release store is RuStore. This repository now contains our own backend foundation; it does not depend on selecting a third-party game backend. It is a locally tested service, **not a deployed commercial backend**. The Unity client is not connected yet.

## Implemented

- Server-created guest identity, expiring bearer sessions stored as hashes, session revocation and a separate server wallet.
- Server-owned product recipes and idempotent order creation. An order freezes its player, application, environment, product, price and reward.
- RuStore Pay invoice verification, followed by an atomic SQLite grant and wallet credit. Duplicate invoices, concurrent callbacks, mismatched ownership and interrupted writes cannot add a second credit.
- Explicit sandbox/production separation, bounded requests and provider responses, generic errors and no token forwarding across HTTP redirects.
- A loopback development runner and executable tests. Purchases have an empty catalog by default; continuation requests remain unavailable until server round authority exists.

## Run locally

From this directory, with Python 3 (tested with 3.14.6):

```sh
python3 -m unittest discover -s tests -v
python3 run_local.py --database /tmp/lor-backend-local/state.db
```

The development listener binds only `127.0.0.1:8097`. `wsgiref` is not a production web server. SQLite databases and their journals stay outside source control. Do not expose this runner publicly.

`RUSTORE_APP_ID` and `RUSTORE_PUBLIC_TOKEN` are optional environment inputs for the local sandbox runner; absence never simulates verification. No credentials are supplied, stored in source, or needed for the tests. The runner's catalog stays empty even when these inputs exist. Test prices and rewards are synthetic and are not approved commercial products.

## HTTP contract

All responses are JSON with `Cache-Control: no-store`. Authenticated routes require `Authorization: Bearer <session>`. Do not log that header or authentication response bodies.

| Method / path | Request / behavior |
|---|---|
| `GET /health` | Reports store/environment and continuation disabled. |
| `POST /v1/auth/guest` | `{}` → new player, one-day access token, zero server gems. |
| `GET /v1/player` | Returns the authenticated server player and balance. |
| `DELETE /v1/session` | Revokes this session. |
| `POST /v1/rustore/orders` | `product_id`, canonical UUID `request_id`; retries return the same order. |
| `POST /v1/rustore/fulfill` | `order_id`, `invoice_id`; verifies the provider invoice before credit. |
| `POST /v1/continuations` | Returns `503 round_authority_unavailable`. |

The future Unity Pay integration must pass the server player as `appUserId` and the server order as `orderId`. A client callback, a local gem balance or an offline save is not payment evidence. Existing local progress is not automatically overwritten or uploaded as trusted server state.

## RuStore verification contract

The verifier uses the official [v2 invoice lookup](https://www.rustore.ru/help/work-with-rustore-api/api-subscription-payment/v2-purchase-invoiceid), checked on 2026-09-29. It requires `CONFIRMED` plus matching application, player, order, item, RUB amount and supported acknowledgement state. `PAID` is not treated as final confirmation. The documented nested order example and top-level form are parsed; contradictory forms are rejected. Actual response parity still requires a RuStore sandbox transaction.

Grant persistence precedes acknowledgement. This foundation returns `acknowledgement_required`; it does **not** execute or retry RuStore acknowledgement. An acknowledged invoice without a matching local grant is rejected for reconciliation. Refund detection prevents a new credit, but revocation of an earlier grant and refund notifications are not implemented. See also [Pay SDK migration](https://www.rustore.ru/help/sdk/pay/migration) before integrating the current SDK rather than an older Billing contract.

## Next implementation packages

1. Stable account recovery/linking and Unity session transport, using existing service composition. Guest tokens currently expire after one day without renewal or recovery; production purchases must wait for a durable account lifecycle. Define the offline-to-server save and wallet reconciliation explicitly.
2. Server-authoritative attempts and continuation entitlements, then same-attempt playable resume. The local Unity reservation journal is recovery infrastructure, not server authority.
3. RuStore Pay Android SDK integration, approved app/product catalog, server credential rotation, acknowledgement retry/reconciliation and refund processing. Verify real sandbox purchases and lifecycle interruption on Android before enabling offers. Rewarded-ad provider remains a separate decision.
4. Hosting with HTTPS, a production WSGI server, account/request rate limits, backups/restore and operational monitoring; evaluate PostgreSQL for multi-instance deployment. The current SQLite schema is version 1 and rejects unknown newer versions. Only disposable test databases have been created for this delivery.

The release gates above are unfinished work, not configuration already installed. No external deployment, public endpoint, live purchase, store signing change or ad-provider activation has occurred.
