# Scenario API setup for Legends of Rus

## Current status — 2026-09-17

The sections below record the initial credential/model-list setup. Asset generation and isolation were subsequently implemented through per-revision scripts. Events R1/R2 alone have nine recorded successful Scenario jobs (81 CU). The setup helper remains a model-list check, not the generation driver. See [Current workflow and audit](CurrentWorkflow.md) for the active process and its extraction-first corrections. This update did not make a fresh API request.

Prepared 2026-09-15. Scenario is the user's preferred source for new art. Existing approved assets remain preserved. This helper configures credentials and checks model-list access only; it is not yet an asset-generation pipeline.

## Create the key

In Scenario, open Team settings (called Organization settings in the quick-start guide), then API Keys → Create/Add API key. Name it `LegendsOfRus-Local`, choose **Project**, and select the project intended for this game's art.

For the intended upload/generation/download workflow, choose Custom permissions with **Assets: Write** and **Models: Run**. These cumulative levels include their read permissions. No Team administration role is needed. Optionally assign a compute-unit usage cap. Save the API key and API secret; the secret is shown once. A missing API Keys menu can be an account-plan or role limitation.

Sources checked:
- [Key creation quick start](https://docs.scenario.com/get-started/documentation/quick-start-guide/step-1-obtain-your-api-key)
- [Project keys, permissions and scopes](https://help.scenario.com/articles/2952168846-api-keys-and-scopes?lang=en)
- [Basic authentication](https://docs.scenario.com/get-started/documentation/quick-start-guide/step-2-authenticate-your-requests)
- [Model-list endpoint](https://docs.scenario.com/get-started/content/get-and-filter-models)

## Configure locally

Run in your own interactive VS Code terminal at the Unity project root:

```bash
python3 Tools/scenario_access.py setup
```

Paste the key at the first hidden prompt, press Enter, then paste the secret at the second. Neither value appears on screen or in shell command history. Do not paste them into chat, commands, Unity files or documentation.

Storage: `~/.config/legends-of-rus/scenario/credentials.json`, outside the repository and Assets directory. It is local plaintext, **not encrypted**: the containing directory has mode 700 and the file mode 600, accessible to your Linux account. Setup refuses existing credentials unless explicitly run with `setup --replace`. It needs only Python 3's standard library; no Unity packages or SDK installation.

## Verify

```bash
python3 Tools/scenario_access.py check
```

Expected:

```text
PASS: Scenario returned HTTP 200 for GET /v1/models.
Model-list access verified. No upload or generation was requested.
Generation permissions, credits and individual model access are not yet verified.
```

The helper reads the file privately inside its process and sends Basic authentication only to Scenario's fixed HTTPS model-list URL. It blocks redirects, retains certificate validation, suppresses account/model response bodies and never prints headers or credentials.

- HTTP 401: key/secret pair invalid or revoked; enter a replacement pair with `setup --replace`.
- HTTP 403: check project selection, Models scope and account/IP restrictions.
- HTTP 429: wait before retrying.
- Network/TLS failure: check connectivity. A successful user-terminal check does not establish that the agent sandbox permits the same request; agent-side access needs a subsequent check under its network rules.

After setup, tell the assistant that credentials are configured and share only the status line. The assistant can invoke the helper rather than displaying the credential file. No need to export environment variables or restart the IDE for this file-based helper.

## Verification performed during preparation

Passed Python syntax parsing and CLI help; dummy-credential mocked setup and owner-only file/directory permissions; request-method/URL and Basic-auth construction checks; mocked HTTP 200/401/403/429/302/500 and network-error handling; redirect refusal; suppression of dummy secrets in output; rejection of insecure permissions and missing credentials. Temporary test data was removed. No real credential was inspected or configured, no live authenticated API call or paid generation ran, and no Unity build/tests or Android checks were performed.

## Live verification — 2026-09-15

After the user configured credentials, they reported HTTP200 for the model-list check. The assistant then ran the same helper: default sandbox access returned a Network/TLS failure; an automatically approved run outside the network sandbox returned HTTP200 for GET /v1/models. Thus model-list access works in both environments. No credentials or response bodies were printed. No upload or generation was requested; generation permissions, credits and specific model availability remain unverified.
