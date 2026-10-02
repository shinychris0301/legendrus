# Backend HTTPS staging R1 — 2026-09-30

Implemented the next backend step after Android credential R5: an isolated session staging entry, pinned Gunicorn configuration, nginx TLS/request limits, systemd template and verified SQLite snapshot/restore tool. **Locally verified; not publicly deployed and not an Android HTTPS-session test.** No server/domain has been supplied. Normal Unity accounts remain unconfigured.

| Check | Actual result |
|---|---|
| Python backend suite | **47 passed, 0 failed**, final run without resource warnings. Includes prior 34 tests and 13 new staging/snapshot cases. |
| Real nginx + TLS + Gunicorn integration | **23 named checks passed, 0 failed**, including process restart and recovery. |
| TLS | Trusted fixture connection passed; default trust rejected the fixture certificate; unknown TLS hostname rejected. No system/device trust modification. |
| HTTP/proxy boundaries | Plain HTTP returned 404 without redirect; query strings, oversized bodies, commerce and legacy transient guest routes rejected; auth rate limit returned 429. |
| Session lifecycle | Bootstrap retry preserved identity; authenticated zero wallet; refresh; same committed refresh response after server-process restart; old access rejected; new access valid; logout revoked access and recovery. |
| Backup/restore | Committed WAL data preserved, 0600 snapshot, restore to new path retained identity/balance; restored rotation/logout worked; source fixture unchanged. Existing targets/journals, symlinks, shared destinations, corrupt/unknown-schema input rejected. |
| Readiness | Healthy DB passed; missing DB and unknown schema failed without recreating the DB; stopped Gunicorn produced proxy 502. Readiness is read-only, not a write-capacity check. |
| Logs | Synthetic fixture credentials absent from local stack logs; no token values exported. No review of future host/provider logs claimed. |
| systemd | `systemd-analyze verify` returned 1 because `/opt/lor-staging/venv/bin/gunicorn` is not installed on this workstation. No host service was installed/started; host execution remains unverified. |
| Unity/Android | No source, plugin, package, setting or APK changed. No new Unity compilation/build/device test was needed or run for this backend-only change. Previous R5 device acceptance remains historical evidence. |

Evidence: [backend results](QA/BackendTests.json), [case log](QA/BackendTests.txt), [real stack checks](QA/StackVerification.json), [systemd prerequisite result](QA/SystemdVerify.json), [scope](QA/Scope.json), [task patch](Sources/TaskChanges.patch). Reproducible stack runner: [verify_staging_stack.py](../../../../Backend/LegendsOfRusService/tests/verify_staging_stack.py). Deployment/runbook: [deploy/README.md](../../../../Backend/LegendsOfRusService/deploy/README.md).

Local verification used Python 3.14.6, hash-pinned Gunicorn 26.2.0 in a temporary venv and Ubuntu nginx 1.28.3 extracted under `/tmp` without installing a system service. The test rendered only host/certificate/temp-path/listener substitutions into the actual nginx template, bound loopback high ports and removed its processes, certificate and database afterwards. The first harness attempts failed nginx validation because distro-default temporary/log paths did not exist; the harness now confines those paths to its disposable directory. Final integration passed. A pre-existing migration-test SQLite connection leak produced a ResourceWarning; the test now closes that connection, and the final suite has none.

Shared request parsing also now rejects duplicate JSON fields and excessive nesting with a generic 400 response. Game rules, economy values, service contract and commercial activation remain unchanged. Staging deliberately excludes provider credentials and commercial routes. One host/one Gunicorn worker/four threads; this is not a multi-host database architecture or a production load/performance acceptance.

## Android AssetPackManager diagnostic

The exact Unity 6000.5.9f1 IL2CPP Development `classes.jar` contains the lookup in `com.unity3d.player.PlayAssetDeliveryUnityWrapper`. Bytecode shows the constructor reflectively loads `com.google.android.play.core.assetpacks.AssetPackManager`, catches `ClassNotFoundException` and writes the error to Unity's logger. Later asset-pack calls would throw when that optional API is unavailable. No calls to this wrapper/AndroidAssetPacks were found in the project's C# scripts.

This explains the caught startup log seen in R5; it does **not** prove all asset-delivery configurations are safe. The earlier real-device run reached Title and completed the storage tests. Do not add Google Play dependencies or bypass plugin guards just to hide this lookup in a RuStore build. No engine patch/SDK change was made. Evidence: [jar fingerprint](QA/AssetPackLookup.json), [javap output](QA/PlayAssetDeliveryUnityWrapper.txt).

## Next action and limits

Obtain the intended hosting provider/API domain, provision this reviewed staging package, verify DNS/public certificate/renewal and host service operation, then add an explicit Android staging-session path and test bootstrap → app restart → renewal → interrupted-response recovery on the connected phone. No production endpoint, real account, payment, provider call, remote deploy, live backup or certificate was created here. No device data or credentials were read/exported.

Account linking/deletion, offline/server reconciliation, authoritative attempts, RuStore Pay SDK/sandbox acknowledgement/refund recovery, operational monitoring and backup retention remain unfinished. Retain historical broad-runtime and human campaign gaps. Animation/effect work stays last.
