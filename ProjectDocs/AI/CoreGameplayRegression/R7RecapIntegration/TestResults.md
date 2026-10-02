# Core gameplay regression R7 after recap integration — 2026-10-02

**1,377 latest distinct Unity cases passed, zero failed; three backend cases deferred.** The current eight-assembly broad sweep has **1,339 passes, zero failures and14 conditional skips**. Seventeen separate restart/profile processes exercise ten distinct local skips; the explicit fresh-QA recap resolves one more. Another27 targeted Edit Mode cases pass. Rows overlap: execution counts must not be summed as distinct coverage.

| Invocation | Passed | Failed | Skipped |
| --- | ---: | ---: | ---: |
| Broad: eight Play Mode assemblies | 1,339 | 0 | 14 |
| Retained failure: writer, recovery/decline, second restart | 3 | 0 | 0 |
| Continuation: reservation, refund, reject replay | 3 | 0 | 0 |
| Language: separate writer/reader | 2 | 0 | 0 |
| Complete saved player: separate writer/reader | 2 | 0 | 0 |
| Profile: normal writer, QA writer/resume/corrupt/reset/empty-energy, normal reader | 7 | 0 | 0 |
| Recap: explicitly fresh QA profile | 1 | 0 | 0 |
| Edit Mode: recap, Profile alignment and campaign story | 27 | 0 | 0 |

[Run inventory](QA/RunInventory.json) · [Latest distinct cases](QA/LatestCases.json) · [Eight assemblies](QA/BroadAssemblyResults.json) · [Actual17-phase plan](Sources/PhasePlan.json) · [Fresh QA recap](QA/QARecap/summary.json) · [Edit Mode](QA/EditRecap/summary.json).

The broad XML covers1353cases in39.59minutes and has no failures. Unity explicitly logged test completion/exit0. After the user interrupted the turn, the launcher session was unavailable and its summary was missing. Recovered the completed XML/log evidence without inferring a launcher exit status; that field remains null. [Recovered broad summary](QA/Broad/summary.json) · [Recovery source](Sources/RecoverBroad.py). Every later restart/focused launcher exited0. No post-correction broad rerun was needed: the explicit QA precondition was installed before this sweep and no fixture/runtime correction followed it.

Unity6000.5.9f1 runs sequentially with disposable product stores under /tmp. Play Mode uses process-only Vulkan and descriptor limit4096; no project graphics/settings change. Only the existing Editor entitlement directory is linked, without inspecting/copying license contents. The normal player journal and credentials are not read.

One Editor test fixture changed. DisposableRecapPreservesSyntheticNormalJournal now conditionally skips when the required QA command is absent, matching other staged fixtures. Its exact fresh-command assertion and all complete save/journal comparisons remain. A wrong nonempty command still fails; [the explicit fresh run](QA/QARecap/summary.json) passes. Production recap, story/save/economy/navigation behavior and APK code are unchanged.

Current native checks cover installed recap eligibility, RU/EN lines, real pointer entry, repeated Continue, Back/origin paging, pending normal story, stale control retirement and repeated finale replay without ChapterComplete or duplicate grants. All17 cold-process phases pass: retained failure/decline, continuation reservation/refund protection, language and complete saved-player restoration, plus disposable profile write/resume/corruption/reset/empty-energy/normal restoration with synthetic normal-key fingerprints preserved.

All **500 rounds ×3seeds =1,500 replay witnesses** have complete round/seed pairs and finish within move limits, using2–20moves. [Witness summary](QA/CampaignWitnessSummary.json). This establishes sampled automated paths, not human difficulty/pacing or every seed.

Retained **839 broad PNGs / 1779 actual Unity PNGs across runs**. Six portraits across Profile History, recap, Gameplay, Victory and Chapter Complete were inspected. [Visual observations](VisualSpotChecks.md) · [Capture inventory](QA/CaptureInventory.json). These are native offscreen Unity captures with simulated viewports, not phone evidence; full ten-digit stock values remain small.

Of7833 task-start project inputs, **7832 remain byte-identical, one test file changed, zero added/deleted**. [Scope](QA/Scope.json) · [Task-only patch](Sources/TaskChanges.patch) · [Final reconciliation](QA/FinalValidation.json). All production scripts, art/fonts, active prefabs, GUIDs/metas, approved Splash, settings/packages/backend/guards and inherited edits are preserved. The initial in-run scope check encountered Unity's temporary scene; final scope runs after cleanup. Four continuity documents and new task tools/evidence complete the patch. Five existing normal manual exports remain unchanged.

The current R3 APK remains766,017,314bytes, SHA2562302b069bddf4b0261513fa5696bfa60dc9940f1c9bd61bc34a54c063079335e; sidecar unchanged. [APK identity](QA/ExistingApk.json). No new Android build, install/device/ADB action, hosting/deployment or commit.

Three backend HTTP/session-rotation cases remain skipped because their loopback/backend process runner was not invoked; exact names/reasons are in the inventory. Historical backend passes are not added here. Physical-phone input/readability/lifecycle/performance, human Chapter1 pacing/usefulness and visual acceptance remain separate. No new effects or balance acceptance is inferred.

**Next:** Prepare the bounded scope and ownership checks for disposable large-stock and acknowledged-story/finale QA fixtures through the existing development profile. Human201–210 recording remains parallel, followed by the prepared gap matrix before tuning. Phone execution and hosting remain deferred; physical readability, exact font provenance and Android lifecycle/performance remain open. Final gameplay effects stay last.
