"""Record validated results and append continuity without removing earlier decisions."""
from pathlib import Path
import json,shutil
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
v=json.loads((QA/'SourceValidation.json').read_text());assert v['result']=='Passed'
counts=v['freshUnityGroupCounts'];play=v['unityPlayDistinctPassed'];edit=v['unityEditDistinctPassed'];total=v['unityTotalDistinctPassed'];artifact=json.loads((QA/'ArtifactBefore.json').read_text())
rows='\n'.join(f'| {name} | {json.loads((QA/name/"summary.json").read_text())["mode"]} | {count} | 0 | 0 |' for name,count in counts.items())
files='\n'.join('- `'+n+'`' for n in v['changedProductionInputs'])
report=f'''# 2026-10-03 — Round lifecycle recovery R1: source delivery

The user approved the next core task after the selection recovery APK. This delivery fixes reproduced round-registration/failure defects in the existing runtime. Backups of four production files, four fixtures and four continuity documents were taken before edits. Human rounds 201–210 remain unrecorded from the user's prior confirmation; no rescan, gameplay tuning, phone or hosting work was performed.

Eight pre-fix cases failed in isolated Unity: rejected/locked registration and legacy failure writes mutated live progress/presentation; attempt/retry counters wrapped; native Play entered Gameplay after attempt overflow; retained failure finalization wrapped and discarded the checkpoint. [Reproduction](QA/Reproduce/summary.json) retains eight primary assertion failures, zero skips, launcher exit 2. [Focus](QA/Focus/summary.json) subsequently passed all eight. [Boundary checks](QA/BoundariesFinal/summary.json) passed 11/11.

`TryRegisterLevelAttempt` and the legacy integer wrapper now return an acknowledgement/zero and publish a cloned candidate only after commitment. `TryStartLevel` commits real energy-clock catch-up, one energy debit, the selected level and attempt/retry counters in one journal transaction. Invalid levels, pending booster recovery, insufficient energy and arithmetic rejection cannot publish partial changes. A rejected journal write locks further ordinary writes until existing recovery/reinitialization; it leaves live balances, timestamp, counters and events untouched. As in earlier transaction paths, a durably committed primary is adopted even if later cleanup fails, with further saving locked; this policy remains covered by the journal tests, not a new Android crash test.

Actual `ScreenRouter.StartLevel` recovers pending inventory, checks normalization, acknowledges the combined start and stays on the preparation page with the existing localized error modal on rejection. Its scoped reentry guard prevents a Changed callback from starting twice. The synchronous Gameplay build consumes the exact committed level/attempt once; its ordinary initializer validates the journal attempt and does not register again. Existing ordinary inventory receipts, campaign/sandbox ownership, retained-round rules and deterministic seed utility remain intact. RU/EN tests cover actual Play rejection after normalization, repeated locked Play and callback reentry; normal-board tests verify the exact seed, unchanged attempt/energy during initialization and a single Applied SeaPearl receipt with five moves. The no-energy test exercises the existing modal Close button; Android Back behavior is not changed or newly accepted here.

`TryRecordLevelFailure` and its legacy void wrapper now commit progress before publishing the copied/clamped transient failure result. Changed observers see both committed counters and that result. Attempts/retries/failures use checked increments. Existing retained failure finalization rejects overflow and preserves the checkpoint/journal. The retained memory checkpoint and deliberate in-session retry policy remain unchanged; legacy failure recording does not clear that optional memory owner. These changes do not add an arbitrary mid-round checkpoint or paid continuation.

Final actual Unity 6000.5.9f1 (b57deb96f08d), fresh isolated storage per launcher:

| Group | Mode | Passed | Failed | Skipped |
|---|---|---:|---:|---:|
{rows}

**{play} Play + {edit} Edit = {total} distinct final passes**, zero failures/skips, all five final launchers exit 0. Focus 8 and BoundariesFinal 11 are subsets counted once. [Reconciliation](QA/TestReconciliation.json) · [Source preservation](QA/SourceValidation.json) · [Final validation](QA/FinalValidation.json). XML, summaries, fixture cleanup records and copied launcher/Unity logs are retained. Play Mode uses process-only Vulkan; no project backend/color-space settings changed. Fresh GameState recovery is same-process Unity evidence, not an Android/process restart test or human acceptance. The suites cover previous rewards, spending/refunds, energy, selection/tutorial and journal/migration policies, native win/fail/retry, ordinary boosters, retained attempts and continuation recovery. This is targeted regression, not a fresh full-project sweep or physical performance measurement.

The first boundary launch had two test compilation errors (seed helper signature and the receipt's Applied enum); corrected before execution. The corrected launch passed 9/11 and exposed two inaccurate test expectations: startup already applies energy regeneration, and no-energy Close must be exercised through the existing button rather than unrelated generic Back navigation. Those assertions were corrected without changing the production clock/navigation policy. [Initial boundary evidence](QA/Boundaries/summary.json) and [9/11 run](QA/BoundariesCorrected/summary.json) are preserved separately from the final passing run.

Changed source/fixtures:

{files}

Task-local [patch](Sources/TaskChanges.patch) reverses cleanly against current files. Exactly eight of 7,839 inputs changed, 7,831 unchanged, none added/deleted. GameState outside attempt/failure methods, retained partial outside the checked failure increment, router outside normal start/attempt forwarding, board outside ordinary initialization/acknowledgement, and all earlier fixture checks are byte-preserved. Approved art, metas/GUIDs, actual prefabs/builders, fonts, settings, packages, backend, Android guards and other user changes remain unchanged. Four continuity documents retain earlier acceptance/history.

Current development APK remains **{artifact['bytes']:,} bytes**, SHA256 `{artifact['sha256']}` with unchanged sidecar. Its inspected metadata lacks TryStartLevel: **this APK does not include R1 source fixes**. No build, signing/credential access, personal-save access, Git commit/push, installation, store upload or phone/hosting action occurred.

**Next:** use the existing guarded build workflow to refresh the Android APK with round lifecycle recovery, then update artifact pins and evidence. Human 201–210 recording/ratings/review before tuning remains parallel; all 18 physical acceptance rows remain pending. Hosted identity, real paid/video settlement, RuStore production readiness and profitability validation remain later unverified work; stock-max remains restricted and final effects stay last. Settings and other general Persist/lifecycle callers remain outside this source patch.
'''
(BASE/'TestResults.md').write_text(report)
entry=f'''# 2026-10-03 — Round lifecycle recovery R1 complete (source)

[Results](RoundLifecycleRecovery/R1/TestResults.md) · [Reconciliation](RoundLifecycleRecovery/R1/QA/TestReconciliation.json) · [Final validation](RoundLifecycleRecovery/R1/QA/FinalValidation.json). Eight pre-fix failures reproduced. Attempt registration and failure results now acknowledge saving before publishing; checked counters reject overflow. Actual native start commits energy/clock/attempt together and forwards the committed attempt once, preserving ordinary booster and retained recovery ownership. Reentry/rejected-saving paths retain existing localized controls. Retained failure overflow preserves the checkpoint; existing in-session retry policy remains unchanged.

Unity 6000.5.9f1: {play} Play + {edit} Edit = {total} distinct passes, zero final failures/skips, five final launchers exit 0. Focus 8/BoundariesFinal 11 counted once. Actual RU/EN rejection/reentry, fresh-state recovery, ordinary seed/SeaPearl ownership, rewards/spending/refunds/tutorial/selection, retained/continuation and native journeys pass. Initial boundary test API/expectation corrections are documented with preserved evidence. Fresh state is same-process evidence; no physical/process restart or full-project acceptance claim.

Four production files and four existing fixtures changed;7,831/7,839 inputs unchanged, none added/deleted. Other source/art/metas/GUIDs/prefabs/builders/fonts/settings/packages/backend/guards/user edits preserved. Four continuity documents updated with earlier history retained. No personal journal/credentials/Git/build/phone/hosting/store action. Human 201–210 remains unrecorded from the user's confirmation; no rescan/tuning. All 18 physical rows pending; real paid/video settlement and RuStore production readiness/profitability remain unverified; stock-max restricted, effects last. Settings/general Persist/lifecycle callers remain outside this patch.

**Next:** guarded APK refresh packaging round lifecycle recovery. Current APK/sidecar remains {artifact['bytes']:,}bytes/SHA256 `{artifact['sha256']}` and excludes these new source fixes while retaining selection and prior recovery. Human 201–210 collection/review remains parallel.

'''
for name in ('CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule'):
 p=ROOT/'ProjectDocs/AI'/f'{name}.md';old=p.read_text();assert old==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
 p.write_text(entry+old if name in ('CurrentTask','ConversationHandoff') else (old.rstrip()+'\n\n'+entry).rstrip()+'\n')
for folder in sorted(QA.iterdir()):
 if not folder.is_dir() or not (folder/'summary.json').exists():continue
 summary=json.loads((folder/'summary.json').read_text());local=Path(summary['rawLogs'])
 for name in ('unity.log','launcher.log'):shutil.copy2(local/name,folder/name)
print('Validated results and four continuity documents recorded; prior history retained.')
