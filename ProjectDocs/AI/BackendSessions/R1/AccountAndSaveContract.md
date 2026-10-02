# Recoverable guest sessions and Unity transport

2026-09-29. Continuation of the approved own-backend/RuStore plan. This package implements device-bound guest-session renewal and a Unity transport; it is not a completed account-login or cloud-save feature.

## Implemented flow

1. Before the first request, the client creates 256 bits of random recovery material and a request UUID, then saves the pending operation through `IBackendCredentialStore`.
2. `/v1/auth/bootstrap` creates one server player and session family. Repeating the exact pending request returns the same identity. Only hashed credentials are persisted by the server.
3. `/v1/auth/refresh` atomically rotates recovery credentials, revokes the previous family access token and issues a new one for the same player. The client durably stores old/new credentials and the request ID before sending.
4. A lost response or client-side commit failure leaves a retryable pending operation. The server reproduces the already-committed response; the client replaces its stored credential only after a successful local write.
5. Reused credentials with altered terms, an expired/revoked family, mismatched server identity, a malformed stored record or a different backend origin fail closed. No automatic replacement account is created.

Access credentials last one day; recovery credentials last 30 days. Renewal within that window is implemented. Loss of the device/credential or expiry beyond that window is **not** solved by this protocol. Email/SMS/provider linking, cross-device recovery and account deletion remain separate product work. The old `/v1/auth/guest` endpoint stays transient; migration preserves its old sessions without claiming to upgrade them to recoverable accounts.

The backend now uses schema 2. Its transaction adds session-family/recovery tables and a nullable family reference to existing sessions. Wallets, orders and purchase grants are retained; unknown newer schema versions are rejected. Exact renewal retries are supported while their immediate successor remains usable, not after it has itself been rotated or revoked. Renewal race losers receive rejection without revoking the winning session.

## Unity ownership and activation

`BackendAccountService` is registered by the existing `RuntimeBootstrap` on the existing service root. No new startup scene, Canvas or EventSystem was added. It starts unconfigured, sends no background requests, and returns `NotConfigured` until the integration supplies a transport and protected store. Its read-only wallet result is separate from `GameState`.

`BackendSessionClient` handles pending operations, identity validation, one bounded retry after unauthorized wallet access and concurrent-call exclusion. `BackendHttpTransport` uses the managed HTTP client with normal TLS certificate validation, no redirects, a 10-second bound and a 64-KiB response limit. Production accepts HTTPS origins only. Explicit Editor verification can use literal loopback HTTP; no Android insecure-HTTP/backend/project setting changed. No account values or credentials are logged by the client.

`IBackendCredentialStore` requires atomic protected persistence and must distinguish a failed read from no existing record. **No production storage adapter was installed.** Access tokens are memory-only; recovery credentials are never stored in PlayerSaveData or PlayerPrefs. Tests use detached in-memory copies and a disposable file fixture under a runner-created private temporary directory. Those files are removed after verification and are not evidence artifacts.

## Save and economic contract

- Keep current local campaign progress, rewards, inventory and failure checkpoints intact while the account service is unconfigured.
- A server guest starts with zero server gems. This neither deletes nor overwrites offline gems.
- `ReadServerWallet` reports a separate authenticated server balance. It does not call `SpendGems`, `GrantReward`, import a save or approve a continuation.
- Future account linking must reconcile progress explicitly. Client-reported balances, completion totals and ad callbacks are not server economic authority. Offline saves must not be accepted as purchase proof.
- Never silently select a different account after storage/network/recovery failure. The production UI needs a localized recovery choice before account switching is enabled.

RuStore invoice verification from the prior package remains in place and all its tests are rerun. No RuStore SDK, real invoice, remote deployment, sign-in provider, ads or commercial activation flag was added. Existing Login/Settings copy remains accurate: online sign-in and account linking are unavailable.

## Next bounded step

Implement a platform-protected Android credential-store adapter with atomic pending-write recovery, then verify it on Android with the hosted HTTPS endpoint. Continue with recoverable linked identity and an explicit account UI before purchase activation. Authoritative attempts, same-attempt continuation consumption, Pay SDK sandbox acknowledgement/refunds, and cross-device progress remain subsequent dependencies. Human campaign and historical runtime regression acceptance remain open; effects stay last.
