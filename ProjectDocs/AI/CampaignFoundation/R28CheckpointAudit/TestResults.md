# R28 —150-round checkpoint audit

**Unity6000.5.9f1:16 tests passed,0 failed,0 skipped.** [Verification](QA/Verification.json) · [Unity XML](QA/Verify01/Checkpoint/results.xml).

|Check|Result|
|---|---|
|Failed first attempt and retry at50/100/150|3 new cases pass: attempts, retries, failure count and best score survive reload; no first-clear reward, extra unlock, story acknowledgement/dialogue or chapter completion|
|Unread checkpoint reload and single acknowledgement50–150|11 existing cases pass|
|All22 pending milestones in order; unavailable151/Chapter2 guard|2 existing cases pass|
|Installed scenario structure|23 referenced scenes,22 ordered milestones,300 outcome reactions,337 required RU/EN speaker/line/reaction keys; no missing required keys or orphan scenes|
|Active content parity|All four installed content JSONs match the R27 candidate exactly|
|Draft151–160|10 structurally valid briefs; not installed, Unity-tested or solved|
|Protected project files|7612 unchanged; only CampaignCheckpointLifecycleTests.cs modified;0 added/deleted project files|

Failures are explicit service-level fixtures, not simulated board losses or player difficulty evidence. Earlier progress is also a fixture. Tests use isolated save stores; existing authorized process-only Vulkan/descriptor4096, without project backend changes. Runtime behavior passed without a production patch.

[Scenario/economy audit](QA/ContentAudit.json) confirms sector101–150 first-clear totals9200gold/30gems/1Mace/1Firebird/2LivingLight/1SeaPearl. Chapter1 finale remains planned500 and is not installed; normal gameplay remains1–150.

**Retained evidence, not rerun:** [R27](../R27MillCheckpoint/TestResults.md) recorded234 passed tests and450 legal-start/completion replays across1–150. R28 rechecked the stored XML totals and active/candidate equality; these counts are not additional R28 tests. Solver replays do not establish human win rates. [Evidence record](QA/RetainedEvidence.json).

**Human evidence:** five valid JSON recordings in the default manual export folder, covering11–30 only. No31–150 recording found there; external folders were not searched. Human later-round difficulty, Android/device performance, full-project test suite and new visual/pointer acceptance remain unverified. [Export audit](QA/ManualEvidence.json).

Task test patch and Python sources pass scoped whitespace/syntax checks. Repository-wide `git diff --check` reports pre-existing whitespace in unrelated prefabs; task-start hashes prove those files were unchanged and they were left intact.

[Next-sector plan](NextSectorPlan.md) and [151–160 briefs](Round151To160Briefs.json) are ready for bounded candidate implementation. They preserve recovery151/156, provisional budgets, existing mechanics and the150→orchard narrative transition. Draft reward total1840gold/6gems/1SeaPearl. Human pacing, optional returning-player recap and older generic reactions remain open. Animation/effect work remains last.

Actual project change: [test patch](Sources/TestChanges.patch). Task-start snapshot, reproducible scripts and [scope](QA/Scope.json) retained; temporary test artifacts cleaned; no commit.
