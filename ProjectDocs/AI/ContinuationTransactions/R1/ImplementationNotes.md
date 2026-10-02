# CONT-02A and RuStore foundation — 2026-09-29

The user authorized the next continuation step, then confirmed RuStore as the release store and instructed us to provide the backend. Backend/store selection is resolved: develop our own service with RuStore Pay integration. This delivery provides two bounded foundations; it does not enable paid/ad offers or playable continuation.

## Unity local recovery transaction

`GameState.TryReserveContinuation` is internal and has no production caller. It accepts a reviewed request contract, validates an exact retained-attempt digest and journals the gem deduction, unused move entitlement and retained checkpoint in one existing save transaction. Paid requests require exactly +5 moves; the price is supplied by a future approved policy. Rewarded candidates have no gem cost. A callback or constructed local request does not prove a real purchase/ad reward.

Matching transaction retries return the prior result. Changed terms under the same ID, evidence reuse, stale attempts, altered snapshots, insufficient funds, invalid histories, multiple pending grants and configured limits are rejected. Write failure leaves live state unchanged; a durable primary commit remains adopted when later cleanup fails. Reservation records are detached from callers.

An unused reservation blocks a new attempt and out-of-band completion/failure. Declining or retrying atomically refunds unused gems, marks the receipt cancelled, records the loss and clears the checkpoint. Tombstones prevent replay after relaunch. The new optional collection is compatible with older v6 saves; it is not a secure server ledger or a downgrade-safe commercial migration. A future server bridge must introduce authoritative revisions and handle old clients deliberately.

The local journal caps at 256 records and fails closed instead of deleting replay evidence. This is an operational limit, not a campaign or pricing rule. Server archival/reconciliation is needed before commercial use. Test values of 17 gems and three ad moves are synthetic. No actual price, ad reward/limit, SDK, provider success mock, UI listener or activation flag was installed. Playable grant consumption remains the next continuation implementation dependency after authority is established.

## RuStore backend

[Backend source and operating notes](../../../../Backend/LegendsOfRusService/README.md) describe the implemented guest-session, server-wallet, catalog/order, invoice-verification and atomic grant endpoints. The backend uses SQLite locally with a schema guard and a loopback WSGI runner. No third-party backend account is required for local tests.

Invoice evidence is fetched using the RuStore server API; client price/balance fields cannot set the grant. Orders bind application, environment and server player. Tests use synthetic HTTP-provider responses; no real RuStore request or credential was used. A separate real loopback HTTP smoke test exercised health, guest creation and authenticated wallet retrieval.

Production account recovery, hosting, Unity transport, Pay SDK, acknowledgement/refund lifecycle and authoritative rounds remain pending. Current local game saves are unchanged by the backend and are not trusted as remote economic authority. Do not connect the two wallets by blindly copying gems or reset a user's accepted progress.

## Source ownership and verification

Two existing runtime files changed: `GameState.cs` and `GameState.RetainedRound.cs`. Added `ContinuationTransactionData.cs`, `GameState.ContinuationTransactions.cs`, two Play Mode test sources and their four metadata files. The new backend contains five code/config files plus README. No prefab/builder change was needed because no UI behavior was activated.

Task-start hashes cover 7,726 Assets/Packages/ProjectSettings files; 7,724 remain identical, two changed, eight added and none deleted after test artifact cleanup. Backend files have a separate manifest. Earlier dirty working-tree changes, GUIDs and approval history were preserved. No art, campaign, matching, effect, package, project setting, commit or deployment change.

See [test results](TestResults.md). Verification reused the prior process-only Vulkan/descriptor4096 authorization and isolated temporary saves. It does not establish Android, live-provider or full Play Mode acceptance. Animation/effect work remains last.
