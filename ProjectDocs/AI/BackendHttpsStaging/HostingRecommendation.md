# Hosting recommendation for backend staging

**Postponed by the user on 2026-09-30.** Core app development now takes priority. Retain this proposal for later; no purchase, provisioning or endpoint activation is requested under the current plan.

Prepared 2026-09-30 following the user's request to recommend hosting. **Proposed, not purchased or deployed.** I recommend Timeweb Cloud for the first hosted session test, using one Cloud-50 server in Saint Petersburg. This is a sizing proposal for staging, not a measured production capacity guarantee.

| Item | Proposed setup |
|---|---|
| Provider and region | Timeweb Cloud, Saint Petersburg |
| Server | Cloud-50: 2 vCPU, 4 GB RAM, 50 GB NVMe |
| Operating system | Plain Ubuntu 24.04 LTS image, with availability confirmed in the order panel |
| Billing | Monthly to begin; review the final order total before purchase |
| Address | One public IPv4; API DNS A record pointing to it |
| API name | `api-staging.<your-domain>`; placeholder only, no domain chosen/registered |
| Runtime | Existing nginx → Gunicorn → Python service, managed by systemd |
| Database | Existing SQLite on the same server, one worker; retain current schema and session protocol |
| TLS | Publicly trusted certificate with automatic renewal; verify before enabling Android sessions |
| Backups | Application-consistent SQLite snapshots copied off-server; one provider server backup initially |

The provider currently lists Cloud-50 at **1,080 RUB/month with the 12-month, 10% discount selected**. Removing that discount implies approximately **1,200 RUB/month** for the displayed configuration; that is a calculation, not a verified monthly checkout quote. IPv4 addresses are paid separately and the final rate is shown in the control panel. Server backups are charged at 6 RUB per GB of disk for each retained backup: a 50 GB backup implies approximately 300 RUB/month per retained copy. Check the selected options so an IP is not counted twice. Sources: [server plans](https://timeweb.cloud/services/cloud-servers), [public IP billing](https://timeweb.cloud/docs/public-ip), [backup pricing](https://timeweb.cloud/docs/cloud-servers/manage-servers/backup).

My provisional budget allowance is **2,000–2,500 RUB/month**, excluding domain registration, for this small staging deployment and modest backup storage. This is an estimate, not an approved spend or provider quote. Seven full-disk backups would alone imply roughly 2,100 RUB/month at the stated backup rate, so do not enable a large retention count without reviewing its cost. Application database snapshots should have their own retention and tested restore process; a VM snapshot alone does not establish SQLite restore correctness.

Selectel is a viable alternative with configurable cloud servers and backup services. I do not see a project requirement that justifies changing our existing deployment architecture or committing to that alternative now. [Selectel cloud server options](https://selectel.ru/services/cloud/servers/).

## Implementation sequence

1. Establish the provider account and billing under the user's ownership, confirm the monthly total and register or select a domain. No purchase has been authorized through an authenticated provider session here, and no credentials should be pasted into chat.
2. Provision the proposed server with an administrator public SSH key. Obtain the public IP and chosen API hostname; use an approved local SSH connection for deployment. Do not upload player saves, Unity build artifacts or local credential fixtures.
3. Deploy the existing [staging package](../../../Backend/LegendsOfRusService/deploy/README.md) into its documented paths. Configure the service account/private state directory and host firewall. Keep Gunicorn on loopback; restrict SSH administration. Arrange certificate issuance/renewal, accounting for the template's intentional HTTP 404 behavior.
4. Verify the systemd unit on that host, nginx syntax, DNS, public TLS, health/readiness, request limits and a restore drill. The earlier local results do not substitute for these host checks. Record the exact deployed source version.
5. Implement the explicit development Android staging-session entry with the now-known endpoint, then run bootstrap, app restart, renewal and interrupted-response recovery using the existing protected credential storage. Preserve any existing credentials and never silently replace an account.
6. After session acceptance, proceed to linking/recovery/deletion and server-authoritative round transactions, then RuStore Pay sandbox integration. Reassess database/concurrency/availability requirements using measurements before commercial launch.

Current inspected state: backend deployment templates and 47 backend / 23 local HTTPS-stack checks already exist from the prior task. Android credential storage passed on the connected Android 16 phone in R5, but authenticated hosted Android sessions remain unverified. This recommendation adds no runtime feature, endpoint, build or new test pass. No Unity/backend source, package, Android guard or device setting changed. Effects remain last.
