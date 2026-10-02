# Failure header R3 and scheduled continuation

Implemented the Failure-only header refinement in the canonical prefab and owning builder/runtime at revision3. [Actual Unity portraits and original reference](Fail_HeaderAlignment_R3_Review.html) · [Detailed comparison](ReferenceComparison.md).

The user also requested paid +5 moves and rewarded-ad continuation **after UI design is complete**. These are now scheduled in [DevelopmentSchedule.md](../../DevelopmentSchedule.md), with implementation order, existing service entry points and acceptance criteria in [FAIL-CONT-01](../../DeferredFeatures/FAIL-CONT-01.md). No calendar date was given. They are scheduled, not implemented: board retention/resumption precedes atomic gem spending and verified rewarded-provider integration. The reference40-gem price remains a proposed balance value. Current retry/chapter-map actions remain functional until that milestone.

## Changes

A single sun-emblem coin replaces the header currency artwork. One Scenario generation, one output, medium quality512×512, reported12CU. Original output, request, job and billing remain in `Sources/Scenario/coin_r3`. Used the original full Failure image only as style guidance, never as runtime art or a crop. Authorized local alpha cleanup and transparent-padding trim yielded479×480; retained RGB is unchanged. Inspected over black, white and magenta.

Adjusted the existing portrait ring/mask together, live name/level and resource alignment, settings rectangle and local material tints. Restored profile-track visibility by placing it last within the profile. Existing controls and serialized object identities remain. No player-XP field exists: the empty track displays a dash instead of invented blue progress. Existing local font assignments remain; exact reference font is unknown.

Only five existing production files changed: Failure prefab, its owning builder, Failure runtime/view and existing EditMode fixture. Two new production files: coin PNG/meta. Exact paths: [SourceAudit.json](QA/SourceAudit.json). Other pages, R2 body design, packages/settings/signing/Android guards and unrelated dirty work were preserved.

## Checks performed

- Read AGENTS/handoff/current task/reference provenance and inspected working changes, source ownership, original reference and R2 captures. Verified recorded Unity6000.5.9f1 and all1,817 R2 production hashes before editing; backed up source/prefabs.
- Actual Scenario submit/status/billing/download succeeded. Inspected original/prepared sprite and three alpha composites. No source screenshot extraction.
- Unity6000.5.9f1 compilation/import and Failure-only prefab rebuild/readback at revision3 succeeded, exit0.
- **10/10 EditMode and18/18 PlayMode tests passed**, zero failures/skips. Existing Failure art/binding/reuse tests now check revision3 and new coin. Existing native retry/settings/exit/booster explanation, real state, failed-goal snapshot and RU/EN/safe-area tests passed, as did Victory/Pre-Level regressions. No new payment/ad behavior was tested because it is deferred.
- Retained9 Failure,12 Victory and20 Pre-Level final PNG/JSON pairs. Visually inspected final Failure941×1672 RU and576×1280 three-goal EN. All captures are actual Unity renders with simulated display/safe area and isolated save, not device screenshots. Other captures were not individually reviewed.
- Preserved all939 existing metas/GUIDs, all439 Failure object IDs and135 R2 evidence hashes.1,812 baseline production files unchanged. Changed C# whitespace check passed. Final1,819 production hashes verified.
- Headless Chrome390×1100 review page rendered; default portrait inspected. All11 image choices and4 links resolve. Python syntax checks passed for the three R3 helper/preparation/runner scripts. Retained evidence hashes verified.

## Not performed / limitations

No APK/AAB, physical Android, full test suite, manual loss/animation playback, prolonged play/profiling, calibrated color or blur metrics, exact-font identification, browser dropdown interaction or live ad/payment transaction. Failure callback test invokes the registered delegate; it does not play through a loss. Profile/plus-button routes were preserved, but this run did not add separate manual route tests for each header button.

The result is closer to the reference, not pixel-identical: pendant/filigree details differ and XP is unavailable. Prior R2 body differences remain documented. Further UI acceptance is not assumed. User's continuation request is recorded for the post-UI stage rather than falsely marked complete. No approval rejection or subagent delegation occurred.
