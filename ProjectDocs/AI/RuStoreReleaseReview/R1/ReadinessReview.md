# RuStore development and commercial readiness review

Reviewed 2026-10-02 (America/New_York). Business model confirmed by the user: **free download with in-app purchases**. This is a review and release backlog, not commercial activation or publication approval.

**Decision: the offline game foundation is substantial, but the current project is not ready for a monetized RuStore release. Profitability is not yet demonstrated.** The next existing development milestone is a guarded development APK refresh that packages the already implemented disposable QA presets. A commercial release additionally needs the payment, account, economy, device, legal and store work below. No meaningful overall completion percentage can be calculated before those release criteria are agreed and weighted.

## Evidence and present state

| Area | Verified state | Remaining acceptance |
|---|---|---|
| Engine and approved UI | Unity 6000.5.9f1; existing startup/services, approved Splash/Title and native screen implementation retained. | Actual phone readability, navigation, performance and final gameplay presentation. Preserve ColorFix06/PrefabFix07 acceptance. |
| Chapter 1 | Active JSON contains 500 levels, 58 story scenes, 57 milestones and 1,569 localization entries in each RU/EN catalog. A 20-chapter definition exists; that is not 20 playable chapters. | Human pacing/difficulty and current-content acceptance; Chapter 2 remains locked. Narrative restoration is distinct from the construction/decorating system proposed in the market documents. |
| Gameplay, story, persistence | R7 records 1,377 latest distinct Unity passes, zero failures and three deferred backend cases. Broad sweep: 1,339 passes/14 conditional skips, supplemented by isolated restart/profile and focused tests. Recap and failure/save recovery foundations exist. | Those are retained results, not a Unity rerun in this review. Solver completion does not establish enjoyment, retention or phone usability. |
| QA fixtures | R4 fixed-preset source work records 60 distinct passes/77 executions and six disposable selectors. | Current APK excludes the new selectors; package them before using them on a phone. |
| Android artifact | Fresh offline preflight matches the actual 766,017,314-byte APK, SHA256 `2302b069bddf4b0261513fa5696bfa60dc9940f1c9bd61bc34a54c063079335e`, its checksum sidecar and linked build evidence. Recap is packaged. | It is a development APK, with QA markers, `com.example.legendsofrus.v117debug`, version 0.0.0/code 1 and recorded debuggable=true. It is not a store release candidate. Signature/manifest were not freshly revalidated in this review. |
| Physical/human acceptance | Earlier Android 16 credential-storage checks passed. Gameplay checklist has 18 pending physical rows. The existing human audit found no current manual rounds 31–500 outcomes in its searched scope: 470 rounds/47 blocks pending. | Resume gameplay/device acceptance when the prior phone deferral is lifted; begin human 201–210, then 201–250 and 491–500. Earlier manual 11–30 evidence is retained but historical. This review did not search personal saves or additional devices. |
| Economy and shop | Local currency/energy/booster/save boundaries exist; the seven-day daily reward schedule is implemented. `economy.json` is explicitly a non-authoritative proposal; economy activation is false. | Approve a single currency source/sink model, production starting balances, offer contents and RUB prices. Do not treat proposal values or existing UI cards as active catalog products. Monthly reward extensions need their own acceptance. |
| Purchases and ads | Runtime monetization starts disabled; commercial config disables IAP, ads and analytics upload. No RuStore SDK dependency was identified in the package manifest or reviewed commercial/plugin paths. | Real Pay SDK adapter, approved catalog, authenticated server fulfillment, lifecycle reconciliation, purchase support. Rewarded ads remain a separate provider/product decision; no ad income assumed in the base model. |
| Backend | Own-backend account/session, invoice verification and transactional wallet foundation exists. Fresh run: **47 Python tests passed**, using temporary databases and synthetic providers. | Public hosted HTTPS is deferred. Catalog defaults empty; continuation authority unavailable. RuStore acknowledgement execution/retries, refund notifications/revocation and actual sandbox parity remain unfinished. Cross-device recovery, deletion, offline/server reconciliation and operations need acceptance. |
| Store and legal | Local validators and draft staging exist. | Metadata descriptions are blank, final export folder contains only README, legal/store/commercial approvals remain false. Existing store/test staging is Google Play oriented; create a RuStore-specific checklist. |

Evidence: [fresh audit](AuditEvidence.json), [fresh APK preflight](OfflinePreflight.json), [R7 Unity results](../../CoreGameplayRegression/R7RecapIntegration/TestResults.md), [fixed presets](../../AndroidGameplayProfile/R4FixedPresets/TestResults.md), [device checklist](../../AndroidGameplayJourney/R1/DeviceChecklist.md), [human acceptance](../../ChapterOneHumanAcceptance/R1/TestResults.md), [backend](../../../../Backend/LegendsOfRusService/README.md). Paths in AuditEvidence.json are relative to the project root unless absolute.

## Supplied development and revenue documents

Reviewed the text, tables and embedded-image overviews of these supplied market documents, without changing or importing their images:

- `/home/k2/Downloads/Telegram Desktop/rustore.docx` — 663 nonempty paragraphs; market/competitor research, example IAP tiers, rewarded ads, restoration/meta, events and staged testing. Embedded images are game/marketing references, not financial statements.
- `Strategy.docx` in the same folder — 495 paragraphs; 500-level/world strategy, offers, subscriptions/events and a suggested mature revenue mix.
- `super games.docx` in the same folder — 440 paragraphs; competitor-system references and recommendations for core, meta and monetization.
- The four project `ProjectDocs/SourceDocuments/Match3_*.docx` documents — master production bible, developer design specification and the two story/scenario documents. Their plan favors a playable Chapter 1 with basic monetization before later chapters and advanced social systems.

The unrelated GIS/contract documents in that Downloads folder were excluded after checking their titles/opening sections. No whole-folder import is appropriate.

The documents provide useful product strategy but **do not supply this game's observed revenue, install cohorts, retention, payer conversion, advertising yield, acquisition cost, development/operating cost or a costed profit forecast**. Competitor estimates and example dollar/RUB prices cannot substitute for those inputs. The bible's RUB ranges (small 99–299, medium 499–999, large/pass 1,499–2,999) are planning guidance; its claim that these are successful prices is not independently established for this game. They are not approved production prices.

Two corrections matter for the commercial plan:

1. Strategy's suggested mix includes IAP 55–65% **and** battle pass 10–15%, while acknowledging overlap. A pass is an IAP subtype: count it once. Use mutually exclusive reporting buckets, such as consumable IAP, non-consumable/pass IAP, rewarded ads and other ads. Ad provider proceeds and RuStore purchase proceeds have different deductions.
2. The forest-building/animal-restoration examples are a different proposed meta game. This repository implements the accepted Russian-fairy-tale campaign; a persistent building/decorating loop was not identified in the reviewed current core services. Do not claim that mechanic in the listing or replace accepted art to imitate competitors. Review whether the existing story/map/recap progression produces the intended emotional reward in human tests. Additional meta construction requires an explicit product scope decision.

The documents also propose losing a life only on failure. Current `ScreenRouter.StartLevel()` spends energy on entry. This is a product/balance difference to settle during the economy review, not evidence of an implementation defect or permission to change the existing save contract. Proposal continuation price 20 gems and earlier deferred-feature reference 40 gems likewise require one approved production rule; neither is an active purchase offer.

Selected market claims were checked against their linked primary sources. VK reports puzzles at 29% of RuStore game downloads and 8% of its most-paid game genres; that supports channel relevance, not a revenue forecast for this title. Event/promotion uplifts in the source are aggregate observations, not a guaranteed uplift here. [VK category report](https://vk.company.ru/ru/press/releases/12293/), [VK event report](https://vk.company.ru/ru/press/releases/12212/).

The Royal Match scale/revenue example comes from Sensor Tower's Russia Q4 2025 **unified-platform estimates**. It is not this game's revenue, a RuStore-only cohort, or a measured payer conversion for a new indie game. RuStore's publisher examples and installation share are likewise selected channel evidence. [Sensor Tower report](https://sensortower.com/blog/2025-q4-unified-top-5-match-3-games-revenue-ru-6564ce96e1714cfff145168c), [VK publisher report](https://vk.company.ru/ru/press/releases/12308/).

Older bible suggestions about engine/backend/ads, styling and languages do not supersede the installed Unity version, approved own-backend/RuStore direction, accepted navy/gold UI or existing RU/EN localization. Indonesian, all 20 chapters, guilds and a battle pass are not initial-release blockers unless added to the release promises.

## Current RuStore requirements affecting this release

RuStore payment-tool monetization has required an **ИП or legal entity** since February 1, 2026; individual/self-employed payment access was discontinued. The owner's actual account eligibility and monetization onboarding are not verified here. No account credentials or private registration documents were requested. [Current account rule](https://www.rustore.ru/developer/blog/self-employed).

For the standard Russian developer agreement, the platform fee is **15%**. That is the model's default, not a claim that developer taxes are included. A foreign entity/additional agreement must be checked against its actual terms. Refunds, settlement timing and transaction rounding affect cash receipts. [Current agreement §6](https://www.rustore.ru/help/legal/devs/developers-agreement), [monetization reporting](https://www.rustore.ru/help/developers/monetization/monetization-report), [foreign developer agreement](https://www.rustore.ru/help/legal/devs/non-resident-developers-agreement).

Moderation expects a functioning application, truthful Russian listing/media and appropriate declarations. The guide specifically identifies debug mode and debug signing as potential rejection signals. The current QA artifact must be replaced by an accepted production artifact, with QA facilities excluded and a reviewed production identity/version/signing process. Finalize accessible privacy/support and account deletion when production accounts/data collection are introduced; declare actual payment/advertising SDKs. [RuStore application requirements](https://www.rustore.ru/help/developers/publishing-and-verifying-apps/requirement-apps).

Current Pay integration and server invoice validation must be reconciled before activation. A two-step purchase's `PAID` status means held funds, not completed fulfillment. The existing backend correctly requires `CONFIRMED`, but actual confirmation/acknowledgement and interruption recovery still need implementation and sandbox proof. [Unity Pay SDK](https://www.rustore.ru/help/en/sdk/pay/unity/10-3-1), [server invoice lookup](https://www.rustore.ru/help/work-with-rustore-api/api-subscription-payment/v2-purchase-invoiceid).

The 766 MB development package is a download/conversion risk to measure, not a proven hard RuStore size rejection. Decide APK/AAB delivery and evaluate the actual release artifact against the current chosen-format rules. Do not transplant Google Play's closed-test population/time rules into the RuStore checklist. [RuStore AAB publication](https://www.rustore.ru/help/developers/publishing-and-verifying-apps/app-publication/new-version-app/upload-aab).

## Prioritized implementation and acceptance backlog

The existing core-first order and explicit phone/hosting deferrals remain in force. This review identifies commercial dependencies; it does not silently resume deferred work or enable prices/providers. Detailed deliverables and acceptance evidence are in [ReleaseBacklog.csv](ReleaseBacklog.csv).

| Stage | Work | Exit evidence |
|---|---|---|
| 1 — next existing milestone | Guarded development APK refresh including R4 fixed presets. | Current source/builder revisions and six selectors packaged; existing policy/signing/ZIP/JNI checks pass; normal saves isolated. This remains a QA build. |
| 2 — core acceptance | Human 201–210 → 201–250 → 491–500, then remaining current-content gaps; complete the offline Android journey when device work resumes. | Recorded wins/losses/retries and feedback, all physical checklist rows, lifecycle/save/reward correctness, phone readability and measured frame/memory/loading behavior. |
| 3 — bounded commercial product | Freeze launch promises around the implemented Chapter 1; approve RUB SKUs, rewards, currency sinks, energy and continuation rules; settle unavailable features and account recovery/support policy. | One consistent approved catalog/balance specification; no advertised unfinished construction/hero/social/events/pass system. Separate release-critical work from later content. |
| 4 — payment authority, when resumed | Deploy reviewed HTTPS foundation; complete durable account identity/recovery/deletion, server wallet and reconciliation; implement attempt authority if paid continues are included. | Android authenticated session continuity, reliable purchased ownership and balances, idempotent grant/spend/refund/recovery; approved operating costs and backup/restore evidence. |
| 5 — RuStore IAP | Current SDK provider adapter + products; order binding, confirmation, durable fulfillment, acknowledgement retries, refund reconciliation and support diagnostics. | Actual sandbox transactions on Android: cancel, pending, success, duplicate callback, lost network/process, restart/restore, wrong owner/order/product and refund. No grants from client claims alone. |
| 6 — retained-player and measurement slice | Verify existing daily rewards; implement approved energy/refill/booster/continue flows; consent-aware funnel/revenue/crash measurement. Add rewarded ads only if separately selected. | Clear free progression, real fulfilled value, replay-safe rewards and actual cohort reports. No projected ad revenue presented as active income. |
| 7 — presentation and store RC | Final deferred effects after core acceptance; actual-phone polish; RuStore-specific listing/exports/legal/age/permissions review; production identity/version/signing build. | Accepted native visuals, non-debug QA-free release artifact and reproducible release checks; accurate store screenshots and working support/privacy. |
| 8 — measured pilot before acquisition scale | Controlled release/test distribution through the selected store workflow and a measured pilot; reconcile purchase totals to the provider report. | Real D1/D7/D30 retention, early-level funnels/failure/energy friction, conversion and 90-day payer spend; net acquisition contribution and cash runway justify marketing. Publication remains separately gated. |

Do not add subscriptions, battle pass, guild/chat, multiplayer rankings, all later chapters or full restoration construction merely to increase a feature count. Those need their own product/economic acceptance and service costs. The initial monetized release should deliver the particular features it promises reliably.

## Profitability model and verification

[Interactive calculator](ProfitabilityReview.html) and [scenario data](ProfitabilityScenarios.json) use a **90-day install cohort**, not monthly revenue or lifetime LTV. All inputs except the sourced default fee are hypothetical; advertising contribution defaults to zero. Payer spend is the total per payer over those 90 days, not one purchase or monthly ARPPU. Paid acquisition cost applies only to paid installs. The 90-day fixed cost is allocated to this same cohort; development/owner labor can be added separately. Do not allocate the same fixed costs twice across multiple cohorts.

```
Gross IAP = installs × payer fraction × gross 90-day spend per payer
Net IAP = gross IAP × (1 − refunds) × (1 − platform fee)
Net contribution/install = net IAP/install + net ads/install − variable cost/install
90-day pre-tax surplus = installs × contribution/install
                        − paid installs × CPI − allocated fixed cost − allocated development cost
```

This simplified model excludes actual developer taxes, working-capital/settlement delays and any unentered costs. Refund/fee treatment is an approximation for planning, not the official transaction settlement ledger. Reconcile to actual RuStore reports before treating it as financial results. Positive results are scenario surplus, not proof of accounting profit or payback of historical development.

The three examples all assume 10,000 paid installs, CPI ₽20, refunds 2%, variable cost ₽0.50/install, allocated 90-day operating cost ₽30,000, ads ₽0 and development/labor allocation ₽0:

| Hypothetical case | Payer fraction | 90-day gross spend/payer | Net IAP | 90-day surplus before tax and unentered development/labor |
|---|---:|---:|---:|---:|
| Low conversion | 1% | ₽299 | ₽24,906.70 | −₽210,093.30 |
| Working example | 2% | ₽500 | ₽83,300 | −₽151,700 |
| High conversion | 5% | ₽1,000 | ₽416,500 | +₽181,500 |

For the working example, net IAP is ₽8.33/install. After example variable/fixed costs, break-even paid CPI is **₽4.83**, far below its assumed ₽20. Holding the other inputs fixed, break-even conversion at ₽500 spend is **5.64%**, or gross spend per payer at 2% conversion is about **₽1,410.56**. These are calculated boundaries, not market predictions or required player spending targets.

Commercial conclusion: the supplied documents establish a plausible channel and monetization design, but do not establish this game's profitability. Finish safe IAP delivery and collect real cohorts before committing to paid acquisition at scale. Test a small approved set of consumable RUB offers first; verify player value, refunds, retention and net proceeds. Include hosting, support, accounting, taxes, creative/marketing and owner labor in the real budget. Price experiments cannot compensate for an unreliable purchase/recovery path or poor early retention.

## Checks and scope

- Fresh: actual APK hash/size/sidecar and linked-evidence preflight; no ADB/device access or installation.
- Fresh: 47 Python backend tests, temporary databases and synthetic provider transport; all passed.
- Fresh: active campaign/config/package/store staging inspection; DOCX paragraphs/tables and image overviews; selected official source verification.
- Calculator checks and final scope/link validation are recorded in [ReviewChecks.json](ReviewChecks.json).
- Not run here: Unity compilation/Edit/Play suites, new APK/AAB build, human gameplay, physical gameplay/performance, real RuStore sandbox purchase, public deployment or publication. Prior evidence remains clearly historical.

Only review artifacts and AI continuity/schedule documentation are changed. Game source, art, prefabs/builders, GUIDs/metas, settings/packages, activation guards, backend and existing build/evidence files remain untouched. The user's approved visuals, core-first order, phone/hosting deferrals and effects-last preference are retained.
