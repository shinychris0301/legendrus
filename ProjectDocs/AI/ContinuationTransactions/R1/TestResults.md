# CONT-02A / RuStore foundation — test results

2026-09-29. **2,065 distinct Unity tests and 22 backend tests passed. Zero final failures/skips in the selected coverage.** Paid/ad offers remain disabled. Backend code exists and runs locally; it is not deployed or connected to the game.

| Verification | Result | Evidence |
|---|---:|---|
| Full project Edit Mode | 1,917 passed | [XML](QA/EditMode/results.xml) |
| Continuation/retained round/Failure UI/save/level/story/checkpoint runtime | 134 passed | [XML](QA/Integration/results.xml) |
| Separate-process reservation write | 1 passed | [XML](QA/RestartWrite/results.xml) |
| Separate-process load, duplicate and refund | 1 passed | [XML](QA/RestartRead/results.xml) |
| Separate-process refund load and replay rejection | 1 passed | [XML](QA/RestartRefunded/results.xml) |
| Existing commercial runtime wiring | 11 passed | [XML](QA/CommercialRuntime/results.xml) |
| Self-hosted backend tests, including real loopback HTTP | 22 passed | [Log](QA/BackendFinal.log) |

The initial 30 focused continuation passes repeat within the 134 integration checks and are not counted again. [Machine-readable verification](QA/Verification.json).

## What was exercised

Unity: atomic spend plus unused entitlement; exact/insufficient balance; unchanged result on retry; conflicting identity/evidence and stale snapshot rejection; storage failure and committed-primary recovery; refund plus failure finalization in one save; duplicate decline; refund overflow; reentrant actions; malformed/bounded history; restart persistence; unchanged attempt, stock and energy semantics; legacy-save compatibility; native offers still disabled. No production reservation caller exists yet.

Backend: guest identity/session hash, expiry/revocation and restart; server-priced orders and idempotency; RuStore invoice field/status validation; wrong owner/application/environment/product/amount; unconfigured provider; acknowledged-without-grant rejection; 16 concurrent duplicate requests crediting once; transaction rollback after ledger insertion but before wallet update; restart replay; integer overflow; request bounds and schema downgrade rejection. A real local HTTP listener exercised health, guest entry and authenticated wallet retrieval. Provider invoices were synthetic test responses, not a live RuStore sandbox transaction.

Intermediate backend discovery used the wrong directory, then two restart fixtures mixed a synthetic clock with the real clock. The discovery command and fixtures were corrected; final 22-test run passed. These were not passing live-provider evidence. Unity runs had no failed cases in this task.

## Scope and limits

Unity 6000.5.9f1, prior verification-only Vulkan and descriptor4096 approval, isolated `/tmp/lor-continuation-r1` save storage. Python 3.14.6 backend tests used temporary SQLite databases. Normal player saves and real credentials were not used. Test scene/linker artifacts were cleaned after Unity exited.

[Project scope](QA/Scope.json): two existing runtime files changed, eight source/meta additions, no deletions; 7,724 task-start files unchanged. [Backend manifest](QA/BackendScope.json): six new backend source/config/documentation files. [Task patch](Sources/Implementation.patch) excludes generated caches. Approved art, current 500-round chapter, difficulty, matching rules, effects, UI, project settings and packages are preserved. No commit/deployment.

**Still unverified or unfinished:** playable same-attempt grant consumption; Unity network integration; stable linked accounts and recovery; authoritative server rounds; actual RuStore Pay SDK/sandbox purchase, acknowledgement/refund processing, production hosting and Android device acceptance. Rewarded-ad provider, product IDs/prices and grant policies remain unconfigured. This was not a full Play Mode rerun and does not clear R43's historical 40 runtime failures. Human 31–500 difficulty/pacing and broad visual acceptance remain pending. Animation/effects stay last.

**Next:** implement stable recoverable backend sessions and the Unity service transport; define progress/wallet reconciliation before connecting real economic operations. Then bind authoritative attempts/continuation and RuStore sandbox purchase lifecycle. [Backend details](../../../../Backend/LegendsOfRusService/README.md) · [Implementation notes](ImplementationNotes.md).
