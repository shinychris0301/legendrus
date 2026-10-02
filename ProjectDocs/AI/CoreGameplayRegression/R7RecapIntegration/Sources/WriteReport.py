"""Write completion documents only from verified current run inventories."""
from pathlib import Path
import json

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
def read(path): return json.loads(path.read_text())
inventory=read(BASE/'QA/RunInventory.json')
assert inventory['latestCounts']=={'Passed':1377,'Skipped':3}
plan=read(BASE/'Sources/PhasePlan.json')
for phase in plan:
    summary=read(BASE/'QA'/phase['name']/'summary.json')
    assert summary['exitCode']==0 and int(summary['result']['passed'])==1 and not summary['nonPassing']
for name,count in [('QARecap',1),('EditRecap',27)]:
    summary=read(BASE/'QA'/name/'summary.json')
    assert summary['exitCode']==0 and int(summary['result']['passed'])==count and not summary['nonPassing']
scope=read(BASE/'QA/Scope.json');assert scope['unchanged']==7832 and len(scope['changed'])==1
broad=read(BASE/'QA/Broad/summary.json')
assert broad['reportedUnityExitCode']==0 and broad['exitCode'] is None
captures=read(BASE/'QA/CaptureInventory.json')
broad_captures=sum(row['path'].startswith('QA/Broad/') for row in captures)
witness=read(BASE/'QA/CampaignWitnessSummary.json')
assert witness['witnessCount']==1500 and witness['withinMoveLimit']
prefix='CoreGameplayRegression/R7RecapIntegration'
next_action='Prepare the bounded scope and ownership checks for disposable large-stock and acknowledged-story/finale QA fixtures through the existing development profile. Human201–210 recording remains parallel, followed by the prepared gap matrix before tuning. Phone execution and hosting remain deferred; physical readability, exact font provenance and Android lifecycle/performance remain open. Final gameplay effects stay last.'
report=f'''# Core gameplay regression R7 after recap integration — 2026-10-02

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

The broad XML covers1353cases in{float(broad['result']['duration'])/60:.2f}minutes and has no failures. Unity explicitly logged test completion/exit0. After the user interrupted the turn, the launcher session was unavailable and its summary was missing. Recovered the completed XML/log evidence without inferring a launcher exit status; that field remains null. [Recovered broad summary](QA/Broad/summary.json) · [Recovery source](Sources/RecoverBroad.py). Every later restart/focused launcher exited0. No post-correction broad rerun was needed: the explicit QA precondition was installed before this sweep and no fixture/runtime correction followed it.

Unity6000.5.9f1 runs sequentially with disposable product stores under /tmp. Play Mode uses process-only Vulkan and descriptor limit4096; no project graphics/settings change. Only the existing Editor entitlement directory is linked, without inspecting/copying license contents. The normal player journal and credentials are not read.

One Editor test fixture changed. DisposableRecapPreservesSyntheticNormalJournal now conditionally skips when the required QA command is absent, matching other staged fixtures. Its exact fresh-command assertion and all complete save/journal comparisons remain. A wrong nonempty command still fails; [the explicit fresh run](QA/QARecap/summary.json) passes. Production recap, story/save/economy/navigation behavior and APK code are unchanged.

Current native checks cover installed recap eligibility, RU/EN lines, real pointer entry, repeated Continue, Back/origin paging, pending normal story, stale control retirement and repeated finale replay without ChapterComplete or duplicate grants. All17 cold-process phases pass: retained failure/decline, continuation reservation/refund protection, language and complete saved-player restoration, plus disposable profile write/resume/corruption/reset/empty-energy/normal restoration with synthetic normal-key fingerprints preserved.

All **500 rounds ×3seeds =1,500 replay witnesses** have complete round/seed pairs and finish within move limits, using{witness['minMoves']}–{witness['maxMoves']}moves. [Witness summary](QA/CampaignWitnessSummary.json). This establishes sampled automated paths, not human difficulty/pacing or every seed.

Retained **{broad_captures} broad PNGs / {len(captures)} actual Unity PNGs across runs**. Six portraits across Profile History, recap, Gameplay, Victory and Chapter Complete were inspected. [Visual observations](VisualSpotChecks.md) · [Capture inventory](QA/CaptureInventory.json). These are native offscreen Unity captures with simulated viewports, not phone evidence; full ten-digit stock values remain small.

Of7833 task-start project inputs, **7832 remain byte-identical, one test file changed, zero added/deleted**. [Scope](QA/Scope.json) · [Task-only patch](Sources/TaskChanges.patch) · [Final reconciliation](QA/FinalValidation.json). All production scripts, art/fonts, active prefabs, GUIDs/metas, approved Splash, settings/packages/backend/guards and inherited edits are preserved. The initial in-run scope check encountered Unity's temporary scene; final scope runs after cleanup. Four continuity documents and new task tools/evidence complete the patch. Five existing normal manual exports remain unchanged.

The current R3 APK remains766,017,314bytes, SHA2562302b069bddf4b0261513fa5696bfa60dc9940f1c9bd61bc34a54c063079335e; sidecar unchanged. [APK identity](QA/ExistingApk.json). No new Android build, install/device/ADB action, hosting/deployment or commit.

Three backend HTTP/session-rotation cases remain skipped because their loopback/backend process runner was not invoked; exact names/reasons are in the inventory. Historical backend passes are not added here. Physical-phone input/readability/lifecycle/performance, human Chapter1 pacing/usefulness and visual acceptance remain separate. No new effects or balance acceptance is inferred.

**Next:** {next_action}
'''
(BASE/'TestResults.md').write_text(report)
summary=f'''# 2026-10-02 — Core gameplay regression R7 after recap complete

**1,377 latest distinct Unity checks passed, zero failures; three backend cases deferred.** Current broad sweep: eight Play Mode assemblies,1,339passed/zero failed/14conditional skips,39.59minutes. All17 separate restart/profile processes pass, resolving ten distinct local skips; explicit fresh-QA recap1/1 and targeted recap/Profile/story Edit27/27 resolve the remaining local skip and add27distinct checks. Counts are deduplicated, not summed executions.

Broad XML completed and Unity logged exit0. After the turn interruption, launcher session/summary were unavailable: XML/log evidence recovered, launcher exit remains unknown. All subsequent invocations exit0. No broad failure or post-sweep fixture/runtime correction. One pre-sweep Editor fixture adds the missing conditional QA precondition; exact fresh assertion and complete save/journal comparisons retained.

[Results]({prefix}/TestResults.md) · [Case inventory]({prefix}/QA/RunInventory.json) · [Scope]({prefix}/QA/Scope.json) · [Final validation]({prefix}/QA/FinalValidation.json). All1500round/seed witnesses within move limits,{witness['minMoves']}–{witness['maxMoves']}moves; not human pacing evidence. {broad_captures}broad/{len(captures)}total native Unity PNGs, six portraits inspected. Recap/pending-story/origin/finale isolation, native stock/input and durable save/restart/profile contracts pass.

7,832of7,833project inputs unchanged; one test file changed/zero added/deleted. Production scripts/art/fonts/prefabs/GUIDs/metas/approved Splash/settings/packages/backend/guards and inherited edits preserved. Four continuity docs updated; task evidence/tools added. Current R3 APK/sidecar unchanged:766,017,314bytes/SHA2562302b069bddf4b0261513fa5696bfa60dc9940f1c9bd61bc34a54c063079335e. Five normal recorder exports unchanged; isolated test journals only. No Android build/device/ADB/normal-save access/hosting/deployment/commit.

**Next:** {next_action}

'''
docs=ROOT/'ProjectDocs/AI'
current=docs/'CurrentTask.md';current.write_text(summary+current.read_text())
session=docs/'SessionLog.md';session.write_text(session.read_text().rstrip()+'\n\n'+summary)
handoff=docs/'ConversationHandoff.md';text=handoff.read_text()
text=text.replace('Latest completed task: [Recap Android refresh R3]','Previous completed task: [Recap Android refresh R3]',1)
anchor='Previous completed task: [Recap Android refresh R3]'
entry=summary.split('\n\n',1)[1].rsplit('**Next:**',1)[0].strip()
text=text.replace(anchor,'Latest completed task: [Core regression R7 after recap]('+prefix+'/TestResults.md), 2026-10-02.\n\n'+entry+'\n\n'+anchor,1)
old_next=text[text.index('Next independent scheduled action:'):text.index('Prepared 2026-09-12.')]
text=text.replace(old_next,'Next independent scheduled action: '+next_action+' Preserve historical decisions below.\n\n',1)
handoff.write_text(text)
schedule=docs/'DevelopmentSchedule.md';text=schedule.read_text()
old_row=next(line for line in text.splitlines() if line.startswith('| 1 | Gameplay regression'))
text=text.replace(old_row,'| 1 | Gameplay regression and round lifecycle | Current R7 after recap:1,377 latest distinct passes, zero failures, three deferred backend cases. Broad eight-assembly sweep1,339/0/14; all17 restart/profile processes, fresh-QA recap1/1 and focused Edit27/27 pass. Completed broad XML/Unity exit0 recovered after interruption; launcher status unknown. Preserve current runtime evidence; physical and human acceptance remain separate. |',1)
old_next=next(line for line in text.splitlines() if line.startswith('**Immediate next action:**'))
text=text.replace(old_next,'**Immediate next action:** '+next_action,1)
text+='\n**2026-10-02 current core regression:** [R7 after recap]('+prefix+'/TestResults.md) completes the current eight-assembly sweep plus17 isolated restart/profile processes, fresh-QA recap and27targeted Edit cases.1,377distinct pass/zero failed/three backend deferred. Broad1,339/0/14; completed XML/Unity exit0 recovered after interruption, launcher status unknown. One staged QA test precondition added before broad; production inputs/current R3 APK and five manual exports preserved. All1500replay witnesses within moves; '+str(len(captures))+'native captures/six portraits inspected. Human/physical acceptance remains open; effects-last and phone/hosting deferrals retained.\n'
schedule.write_text(text)
print(json.dumps({'distinctPassed':1377,'actualPngFiles':len(captures),'existingContinuityDocumentsUpdated':4}))
