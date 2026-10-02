# Backend sessions R1 — test results

2026-09-29 (local date). **1,980 distinct Unity tests and 34 backend tests passed. No unresolved failures/skips in the selected final coverage.** Device-bound renewal and the Unity connection work against the local backend. Production stays unconfigured pending protected Android credential storage and hosted/device verification.

| Verification | Result | Evidence |
|---|---:|---|
| Full project Edit Mode | 1,917 passed | [XML](QA/EditMode/results.xml) |
| Combined Unity account, continuation and commercial wiring | 61 passed | [XML](QA/Integration/results.xml) |
| Real HTTP renewal committed; response deliberately lost | 1 passed | [Writer XML](QA/Restart-write/results.xml) |
| Fresh Unity process recovers pending renewal and same player | 1 passed | [Reader XML](QA/Restart-read/results.xml) |
| Backend suite, including prior RuStore verification regression | 34 passed | [Log](QA/BackendFinal.log) |

[Machine-readable verification](QA/Verification.json) · [Scope](QA/Scope.json) · [Account/save contract](AccountAndSaveContract.md) · [Backend](../../../../Backend/LegendsOfRusService/README.md) · [Task patch](Sources/Implementation.patch).

## Verified behavior

- Exact bootstrap/renewal retries preserve one server identity. Access expiry can renew for the same player without losing server wallet data.
- Parallel identical requests converge; competing rotations have one winner. Reused old credentials with altered terms and previously consumed successors are rejected. Logout revokes the family, not another player.
- A failed rotation transaction preserves the prior access/credential. Version-1 database migration preserves the legacy player, balance and session; subsequent opens preserve version 2. Newer unknown versions remain rejected.
- Unity journals a pending operation before sending. Failed credential reads/writes prevent an unsafe new request. Lost responses and failed local commits can retry the same operation. Stored origin/version and returned identity are validated; failure never silently substitutes a new guest.
- Real managed HTTP calls from Unity exercise bootstrap, refresh and authenticated wallet reads against the actual Python service. A separate-process test persists a synthetic pending credential, discards a successful real refresh response, exits Unity, starts a new Editor, retries, and confirms the same player. Test-only credentials/database are removed from the private temporary directory afterward.
- Server wallet results do not overwrite `GameState`. The account service is present on the existing runtime root but has no production configuration or automatic traffic. Existing continuation and commercial wiring tests pass.

## Intermediate failures and corrections

First Unity run: 17 passed/3 failed because tests expected null where Unity JSON restores empty strings. Production validation already accepted both; corrected those fixture assertions. Second combined run: the 20 account and 11 commercial cases passed; 30 continuation cases rejected the new runner's temporary-path prefix before setup. The runner was changed to use those tests' existing required isolated namespace; all61 then passed together. No production save was used. These initial runs are retained and not counted as final passes.

## Scope and environment

Unity6000.5.9f1, prior process-only Vulkan/descriptor4096 approval and isolated temporary saves. Python3.14.6, disposable SQLite databases, real loopback HTTP with synthetic test accounts. No real RuStore invoice, store credential, external server, ad provider or Android test.

Changed `RuntimeBootstrap.cs`, backend `service.py` and README. Added three runtime C# sources, two Play Mode test sources, their five metadata files, and three backend Python files. Of7,740 baseline files across Assets/Packages/ProjectSettings/Backend, 7,737 remain identical; three changed, thirteen added, none deleted. No art, prefab, campaign, matching, VFX, project-setting or package change. Test scene/linker artifacts cleaned. Working-tree edits and GUIDs preserved; no commit/deployment.

## Remaining work

This is recovery while a device retains its credential and renews within the30-day window. No protected Android credential adapter, linked sign-in, device-loss/cross-device recovery, cloud-save reconciliation or account deletion is installed. The old transient guest endpoint remains transient. No playable continuation consumption, server round authority, Pay SDK/acknowledgement/refund integration or active purchase/ad offer is delivered here.

R43's40 historical runtime failures, human31–500 difficulty/pacing and current broad visual/device acceptance remain open. This was not a full Play Mode rerun. Effects remain the final development step.

**Next:** implement protected Android credential persistence for the installed pending-operation protocol, then hosted HTTPS/device restart checks. Continue with linked-account recovery and explicit offline/server reconciliation before enabling commercial operations.
