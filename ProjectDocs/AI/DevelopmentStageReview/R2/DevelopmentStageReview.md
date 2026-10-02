# Interim development review — 2026-09-29

The project is a substantial locally playable game with a largely implemented visual interface. It is **not yet a connected commercial game**. Its biggest gap is now service implementation and integration, not missing screen designs. The user selected **live commercial and social features first** for the next milestone. Animation/effect refinement remains the final development phase.

This review supersedes the current-status conclusions of DevelopmentStageReview/R1 and the September 15 schedule. It preserves earlier visual approvals and deferred feature commitments. The IDE's R22 report is historical; R43 is the latest broad regression baseline.

## Evidence and scope

Reviewed the four original DOCX requirements, current campaign resources, native page inventory, runtime services/views, commercial composition/fulfillment, activation manifests, package manifest, deferred-feature specifications, and retained campaign/regression/Android evidence. Extracted source text and hashes are retained in [Sources](Sources/) and [SourceDocuments.json](QA/SourceDocuments.json). Fresh machine inventory: [CurrentInventory.json](QA/CurrentInventory.json).

This is a source-and-evidence review, not a new visual acceptance of all pages, a security certification, or proof that every method has been tested. Existing working-tree edits were preserved. No overall completion percentage is assigned: a native screen, a simulated service, and a production service are different deliverables.

## Current position

| Area | Current evidence | Remaining gap |
|---|---|---|
| Interface | 36 native page prefabs, shared visual components, source-based art and extensive prior responsive passes | Final device-wide readability, live loading/error/empty states, full localization and interaction acceptance |
| Gameplay | Normal gameplay uses newer matching rules and approved gem/item/obstacle artwork; goals, cascades and four stock boosters work locally | Failed-attempt continuation, four preview boosters, later chapter-specific mechanics; final effects explicitly deferred |
| Campaign | Chapter 1 rounds 1–200 installed; all are 8×7 with six tile types. Definition contains 20 chapters × 500 rounds | Chapter 1 rounds 201–500 and its finale; playable Chapters 2–20 |
| Narrative | 28 scenes, 27 milestones, 400 outcome reactions; 632 populated entries per story locale, RU/EN | Branching choices, relationships, hero customization, companion unlocks, restoration states, later chapter scripts and recap/history flow |
| Persistence | Local journal/migration and repeat-reward protections; fresh-process persistence evidence | Account identity, cloud conflict/recovery, authoritative economy, durable purchase/ad fulfillment |
| Commerce | Consent/analytics/remote-config/verifier abstractions and candidate preparation exist | Providers, real receipts/ad verification, product reward recipes, durable grant/acknowledgement, paid/ad continuation |
| Social | Ranking, Friends, Team, Chat and Mail pages; sample lists/local interactions | Actual multi-user services, membership, ranking authority, gifts, moderation, mail claims |
| QA | R43 full Edit Mode 1,917 passed; latest runtime-case aggregate 857 passed / 40 failed | Resolve failures and perform one clean combined runtime rerun; Android and human acceptance |
| Android | Historical development APK built and statically checked | A current-content build and real-device installation/performance/lifecycle/provider tests; release delivery |

The 857/40 runtime figure combines latest results from several runs; it is **not** a single green suite. R43's 600 legal completion replays (three per installed round) demonstrate automated reachability, not human enjoyment or difficulty. A fresh check of the default playtest folder found five valid recordings covering 11–30 and none covering 31–200; external locations were not searched. See [ManualEvidence.json](QA/ManualEvidence.json). The old development APK still exists; this review checked its presence/size only, not a current build or installation.

Detailed status and source anchors: [FeatureMatrix.md](FeatureMatrix.md). Design requirements and accepted exceptions: [DesignRequirements.md](DesignRequirements.md). Narrative omissions: [ScenarioCoverage.md](ScenarioCoverage.md).

## Most consequential findings

1. **Commercial activation is more than a switch.** `CommercialRuntimeComposition` constructs disabled providers. `CommercialFulfillmentCoordinator` prepares detached candidates and explicitly never commits them. Known IAP offers have no approved reward recipe; rewarded energy is also unmapped. A reward-double candidate is not a durable grant. Real provider integration and an authoritative transaction lifecycle are required.
2. **The social interface has no production social service behind it.** Login is local guest entry. Friends search/sort sample data; team chat adds messages only to a local session. Ranking scores, seasons, gifts and mail attachments are examples. Keep these disclosures until their actual service is connected.
3. **Previously approved functionality fell out of the active sequence.** Failure continuation, real account integration, Chapter Complete SMS verification, hero ownership and monthly daily-reward bonuses must remain explicit backlog items. They must not disappear behind further screen refinements.
4. **The campaign framework and content are different milestones.** Validated multi-chapter packages, save migration and lifecycle scaffolding exist, including explicit development compositions. Normal `ActiveCampaignContent` currently adds only Chapter 1. Its story completion is null; round 200 is an installed-content boundary, not the chapter finale at 500.
5. **Requirements conflict across generations.** The old 200,000-plus-round proposal, old warm red/gold palette, suggested engines and old Russian-only language statement do not override later accepted 20×500 scope, current navy/gold design, installed Unity version or RU/EN work. Economy examples are not approved product prices.
6. **Evidence needs a release boundary.** Numerous focused captures and structural tests exist, but the project still lacks a current, unified acceptance result. Some R43 captures depend on a `/tmp` symlink; retain a portable selected evidence package before release rather than treating temporary paths as permanent acceptance records.

## Revised direction

Use the existing game as the client. Build one connected account → persistent progress/wallet → verified commercial transaction → social profile path before expanding the catalog of online features. Preserve offline core gameplay, approved art and all existing save data while adding it. Do not create a second router, save authority or success-simulating production service.

The revised ordered plan is [DevelopmentSchedule.md](../../DevelopmentSchedule.md). The first service-readiness work package is [LiveServicesFirst.md](LiveServicesFirst.md), including dependencies, concrete acceptance criteria and unresolved product/provider decisions. Provider names and store/region requirements must be established before installing SDKs or wiring actual network services; no credentials are needed for that decision.

## First step performed in this review

Created a repeatable, read-only inventory/check tool and ran the existing commercial and runtime route checks using isolated storage. Results are in [TestResults.md](TestResults.md). This establishes what can be reused and what must be built. No service was activated and no production feature is described as newly implemented by this review.

**Next bounded implementation:** prepare resumable failed-attempt state for FAIL-CONT-01, including exact board/goal/RNG identity and interruption tests, while resolving the live provider/account contract. Paid/ad grants follow durable entitlement integration; retry must remain a distinct new attempt. Clear existing regression failures before accepting the integrated milestone. Further level expansion and cosmetic VFX iteration are not the next milestone.
