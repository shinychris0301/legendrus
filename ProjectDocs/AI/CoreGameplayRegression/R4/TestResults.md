# Core gameplay regression R4 — broad runtime audit

**Latest evidence: 1,332 distinct Play Mode cases passed, 0 remain failed, 3 backend cases remain skipped.** This combines the broad baseline with corrected and dedicated-process reruns; it is **not a single clean post-correction full-suite run**.

Unity 6000.5.9f1. The initial filter included all eight project Play Mode assemblies: [inventory](Sources/AssemblyInventory.json), [broad assembly results](QA/BroadAssemblyResults.json). Product saves were isolated under `/tmp`; existing legitimate licensing-directory links only, previously approved process-only Vulkan and descriptor limit 4096. Project graphics settings were unchanged. No phone, APK, Edit Mode or hosted-service execution in this task.

| Run | Passed | Failed | Skipped |
|---|---:|---:|---:|
| Broad baseline, all eight assemblies | 1,290 | 33 | 12 |
| Focused baseline with required continuation storage | 32 | 1 | 0 |
| Privacy fixture followed by Settings, reproducing order failure | 6 | 1 | 0 |
| Corrected affected fixtures together | **52** | **0** | **0** |
| Retained failure: write → restart/decline → restart after decline | 3 | 0 | 0 |
| Reservation: write → restart/refund → restart/reject replay | 3 | 0 | 0 |
| Language: separate write/read processes | 2 | 0 | 0 |
| Player save: separate write/read processes | 2 | 0 | 0 |

Rows overlap; do not add them. Language write/read executes the same test name in two processes, so those two passes count as one distinct case. The ten restart phases cover nine cases skipped by the baseline. [Run inventory and exact exclusions](QA/RunInventory.json) · [Latest result for every case](QA/LatestCases.json) · [Corrected combined run](QA/Corrected/summary.json).

## Findings and corrections

1. **Thirty continuation setup failures:** the broad harness used a generic isolated directory, while this fixture requires `/tmp/lor-continuation-r1/`. Its unchanged tests pass with the required directory. No safety guard was weakened; the original failed results remain recorded.
2. **Chapter Map's stale round-500 expectation:** the test expected “coming soon.” Round 500 is now implemented and locked by progression. It now checks the previous-level notice, no navigation and an unchanged save.
3. **Settings failure caused by a preceding test:** the compliance fixture destroyed the shared `PrivacySupportService`, leaving the later Settings test without its singleton. The seven-case reproduction failed only Settings. The fixture now preserves the original component/state and stored preference, uses owned service instances for its restart scenarios, and restores the original singleton. The combined corrected run verifies Settings afterwards without changing production services.
4. **Conversion timing sensitive to capture frames:** the broad run measured 0.068115 seconds against a fixed 0.07 minimum; the original test passed in isolation. The scheduled conversion lead-in is 0.12 seconds, but conversion is observed on the next rendered frame and PNG capture can delay it. The test now checks the scheduled minimum, release on a strictly later frame, and the measured interval allowing the observed conversion-frame duration. Board outcome and converted-item identity checks remain. No animation timing/effect change was made.

[Detailed triage](QA/FailureTriage.json) · [Task-only patch](Sources/Changes.patch).

## Verified coverage and limits

- All **500 campaign rounds × three seeds = 1,500 legal completion replays**, without inventory boosters, produced records within their move limits. [Coverage summary](QA/CampaignWitnessSummary.json). Automated search/replay is not human difficulty, pacing or all-random-seed acceptance.
- Broad runtime coverage includes current rules, item creation/activation, board input/recovery, inventory, native pages and routing, chapter progression/finale, and local service guards. Existing animation assertions passing is not approval of visual quality.
- Fresh processes restored retained Failure, finalized its decline once, preserved reservation/refund replay protection, and restored English before interaction. These local transaction tests do not enable real paid/ad continuation.
- Player-save restart compared the complete saved payload, settings, language and inventory; a service-level repeat completion did not duplicate first-clear rewards. [Restart outcome](QA/SaveCheckpoint.json.read.json). This is not Android process death or a second human-played replay.
- Three cases remain skipped: backend real HTTP bootstrap/refresh and its two restart phases require an isolated loopback backend runner. Hosting/online work remains postponed; earlier backend results are not counted as current passes.
- Inspected the actual English Chapter Complete capture at 360×640. Reward values and secondary captions remain small. [Observation and capture](QA/VisualObservation.md). Other generated captures have not received comprehensive visual review; physical phone readability/performance is unverified.

## Scope and next task

Only three test files changed: `MechanicsCausalAnimationTests.cs`, `PrivacySupportFoundationPlayModeTests.cs`, and `ChapterMapAlignmentPlayModeTests.cs`. [Hash scope](QA/Scope.json): **7,782 baseline Assets/Packages/ProjectSettings/Backend files unchanged; 3 changed; 0 added/deleted.** Existing working-tree edits preserved. Verification scripts/evidence and CurrentTask, SessionLog and DevelopmentSchedule updated. Source whitespace and verification-script syntax checks passed. No runtime, rules, art, prefabs, packages, settings, backend, approval guards or APK changes; no commit/deployment.

**Next concrete core task:** improve Chapter Complete's reward values and secondary captions at small portrait sizes, using the existing approved design and checking both Russian and English. Actual phone testing remains postponed at the user's request. Human rounds 31–500 difficulty/pacing, optional recap, physical-device lifecycle/performance and a single clean post-correction broad run remain open. Hosting/online activation is deferred; animation/effect redesign remains last.
