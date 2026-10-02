# Chapter 1 human acceptance coverage R1

Completed the approved next scheduled evidence audit and collection preparation. **No current manual outcomes were found for rounds 31–500 in the two searched scopes. All 470 rounds / 47 recording blocks remain open.** No balancing or artwork change follows from that absence.

[Interactive coverage](CoverageReview.html) · [Per-round CSV](CoverageMatrix.csv) · [Ordered queue](CollectionQueue.csv) · [Recording instructions](CollectionInstructions.md) · [Evidence inventory](QA/EvidenceInventory.json)

## Evidence examined

Searched recorder source-tagged JSON under `ProjectDocs/AI` (this report excluded), plus the normal product's `CampaignPlaytests` folder. **212 candidate files**: **191 raw session files**, **21 historical inventories**; raw sessions deduplicated to **152 unique sessions**, with **39 extra copies**. No malformed files or conflicting source/content identities were found. Inventories are historical reports, never raw outcomes. Other external folders and devices were not searched.

| Source | Files | Unique sessions | Treatment |
|---|---:|---:|---|
| manual_campaign | 8 | 5 | Historical manual evidence; three duplicate archived copies removed from counts. |
| automated_campaign | 20 | 20 | Harness evidence, excluded from human coverage and ratings. |
| automated_test | 162 | 126 | Sandbox/test evidence, excluded from human coverage. |
| manual_sandbox | 1 | 1 | Historical sandbox example, excluded from campaign acceptance. |

The five manual sessions contain **52 attempts covering 11–30: 46 wins, five losses, one interruption**. Their campaign fingerprints identify older R9/R12/R13 candidates. **42 attempts match their installed per-round definitions; 10 differ at rounds 13/14/17/19.** Eighteen attempts have difficulty/goal/special ratings; sixteen of those match the current round definitions. All remain historical because the full installed campaign/story/locale fingerprint and current block context differ. Prior feedback and acceptance are retained, including R13's “normal” difficulty response; no per-round or current 31–500 acceptance is inferred from it.

Five canonical manual JSON snapshots are preserved under [Sources/ManualSessions](Sources/ManualSessions), retaining their original session IDs, hashes and outcomes. The inventory records all original paths and copy hashes. Existing player recordings were only read.

## Current installed content and recorder readiness

Unity **6000.5.9f1** executed the temporary read-only Editor helper successfully: **exit 0, 115.22 seconds**. It validated all **50 aligned blocks**, exact ten-round boundaries, 30-energy/zero-booster in-memory fixtures, prior fixture progress with zero observed run sequence, unchanged in-memory reopen, and continuous installed rounds 1–500. It exported **500 native per-round `JsonUtility` fingerprints** and the existing recorder's five-resource content fingerprint. The offline byte/text calculation matches it exactly:

`CampaignActive-87f504a454cad5e4b52755755e07af856af32b51be9b9413a9f14a0e22003a80`

[Native export](QA/NativeAudit/InstalledContent.json) · [Process result](QA/NativeAudit/summary.json) · [Unity log](QA/NativeAudit/unity.log). Product storage was isolated under `/tmp/lor-chapter-one-acceptance-r1/config`. No scene or Play Mode was opened. Temporary Editor source/meta removed; no normal campaign/player journal was loaded. Runtime enum serializes the existing current rule version as **5**; this is not a new rules revision.

Eight offline classification scenarios passed: current matching manual outcome, automated/sandbox exclusion, unchanged round in stale campaign, changed round/rules, interrupted/ongoing snapshot exclusion, fixture progress exclusion, dropped/duplicate attempt history and legacy missing context. [Checks](QA/ClassificationChecks.txt). These are Python checks of the evidence audit, not Unity test-runner cases or human play.

Seven isolated Chrome checks passed with zero JavaScript exceptions: default 201–250 view, all 47 queued blocks, final block, hard-round filter, 495 search, all-round reset and report links. [Browser checks](QA/Browser/Checks.json). Desktop and portrait planning-report captures were inspected; these are offline report screenshots, not Unity captures. CSV/JSON/embedded report data and blank observation rows agree. [Final validation](QA/FinalValidation.json) also verifies unchanged normal recording hashes and canonical archived snapshots.

Prepared a **470-row matrix and 47-block queue**, with actual authored difficulty/move budgets/goals/obstacles/story checkpoints, native fingerprints, current outcome/rating counters and explicit pending states. Begin **201–210**, continue through **201–250**, then **491–500**. The final block includes hard 495, recovery 496 and hard/finale 500. Later sector-ending samples precede complete earlier and remaining late coverage. Blank observation fields carry no invented feedback.

## Scope and limitations

All **7,821** task-start Assets/Packages/ProjectSettings/Backend inputs are unchanged; no additions/deletions. [Scope](QA/Scope.json). No changes to approved Splash, art/fonts, prefabs/owners/GUIDs, rules/balance, services, input, saves, settings, packages, backend or Android guards. Documentation/report tools and audit evidence only. Prior user changes preserved.

Current R4 APK remains **765,772,146 bytes**, SHA256 `924ecabcdbdb58f30512e6e2b134db4246be22c8f9ecc6b6086551e4ad937eff`; checksum sidecar retained. No APK build, device execution, deployment, hosting or commit. R6's aggregate 1,342 passes/three deferred backend cases remain historical; this task did not rerun its runtime suite. No new gameplay captures, human difficulty/pacing result or normal finale navigation acceptance is claimed.

**Next action:** collect the first current human recording for **201–210** through the existing Active recorder, rate outcomes before Continue/Retry, retain losses/retries and review the exported evidence. Continue the queue before tuning. Phone work remains deferred; stock badge limits, font provenance and Android lifecycle/performance remain open; final gameplay effects stay last.
