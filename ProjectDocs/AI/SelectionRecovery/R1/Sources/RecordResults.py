"""Record verified selection source work and preserve accepted history."""
from pathlib import Path
import json,shutil
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
v=json.loads((QA/'SourceValidation.json').read_text());assert v['result']=='Passed';total=v['unityTotalDistinctPassed'];play=v['unityPlayDistinctPassed'];edit=v['unityEditDistinctPassed']
report=f'''# Level selection recovery R1 — complete (source)

[Source validation](QA/SourceValidation.json) · [Test reconciliation](QA/TestReconciliation.json) · [Final validation](QA/FinalValidation.json) · [Source patch](Sources/TaskChanges.patch).

User requested the next task and reconfirmed human rounds201–210 are still unrecorded. Chapter1 review/tuning remains pending. The core-first schedule permits recovery defect work; this task addresses the verified level-selection/navigation gap following tutorial APK delivery. No phone or hosted service work resumed.

Six actual isolated Unity pre-fix failures reproduced: two rejected/locked selections reported success, two normalization calls mutated live progress, and the native map node/Play paths opened PreLevel despite saving rejection. The first service reproduction fixtures also left the injected future-version journal active during teardown; preserved XML identifies both the primary failing assertions and teardown logs. Fixture cleanup was corrected to restore the isolated journal before teardown. No production failure is inferred solely from teardown.

SelectLevel now checks the persistence lock, validates the existing target/retained/continuation rules, prepares a cloned candidate and returns atomic commit acknowledgement. TryEnsureSelectedLevelIsUnlocked applies the exact existing normalization rules to a candidate before committing; its void compatibility wrapper delegates to it. Migration uses the same normalization rules with its explicit live save. Other GameState methods, prior recovery fixes and save schema6 remain byte-preserved.

The actual chapter map checks selection acknowledgement and owns a Func<bool> open action. Rejected node/Play callbacks retain the exact map root and show the existing localized error modal; retry is disabled. Close/Back dismisses it; repeated controls preserve the locked write result and still permit normal map exit. ScreenRouter acknowledges normalization before opening story/PreLevel or spending energy for gameplay. Successful navigation preserves the authored intro/pending story flow and existing routes. No visual, serialized binding, prefab/builder revision or localization changes.

Fresh Unity6000.5.9f1 verification: **{play}Play+{edit}Edit={total}distinct passes**, zero final failures/skips, four final launcher groups exit0. Focus8 and Boundaries9 are counted once within final groups. RU/EN native rejected node/Play, valid map/preparation/play, retained continuation/new-attempt restrictions, invalid targets, healthy selection/reload, duplicate/invalid progress normalization and all prior core transaction/narrative/tutorial recovery checks pass. A repaired journal does not re-enable selection in the locked object; a fresh GameState reopens the prior durable choice and commits the next selection once. This is same-process Unity recreation, not physical/process restart acceptance. Existing read-only native captures from the navigation fixtures remain regression evidence, with no new design approval.

Changed three production files: Core/GameState, UI/ProductionChapterMapView and UI/ScreenRouter. Changed three existing fixtures: LevelSystemRuntime, ChapterMapAlignment and PreLevelAlignment (the latter removes its old direct-Persist error-log expectation and retains all behavioral assertions). 7,833of7,839production inputs unchanged, none added/deleted. Every meta/GUID/art/prefab/builder/font/settings/package/backend/build guard and pending user change preserved. Four continuity documents updated with ten pre-task file backups; earlier acceptance decisions retained.

Current APK/sidecar unchanged:766,026,550bytes/SHA256`b699646db5f7ac9effc403c768eb19200b5eb3b58f14858b331f5edb80aa5fb4`. It includes tutorial and prior recovery fixes but **does not include this selection fix**. No new Android build, Git commit/push, normal personal journal/credentials, phone/hosting or store action. All18physical rows pending; real paid/video settlement and RuStore production readiness unverified. General Persist/settings, registration/failure and other lifecycle callers remain outside this patch; no attempt/energy combined transaction or arbitrary board checkpoint is added. Stock-max restricted, effects last.

Next: guarded APK refresh packaging selection recovery. Collect human201–210recordings/ratings/observations in parallel and review before balance/story tuning. Remaining registration/failure/settings recovery needs a separate demonstrated-gap audit.
'''
(BASE/'TestResults.md').write_text(report)
entry=f'''# 2026-10-03 — Level selection recovery R1 complete (source)

[Results](SelectionRecovery/R1/TestResults.md) · [Reconciliation](SelectionRecovery/R1/QA/TestReconciliation.json) · [Final validation](SelectionRecovery/R1/QA/FinalValidation.json). User reconfirmed human201–210still unrecorded; no rescan/tuning. Six pre-fix selection/normalization/native map failures reproduced. SelectLevel and new TryEnsureSelectedLevelIsUnlocked commit candidates before adoption, preserving live state/events on rejected saving and respecting the session lock. Existing normalization rules retained. Native node/Play controls acknowledge selection/normalization, keep the exact map and existing localized error modal, and remain usable after Close/Back; StartLevel checks normalization before spending.

Unity6000.5.9f1:{play}Play+{edit}Edit={total}distinct passes, zero final failures/skips, four final launchers exit0; Focus8/Boundaries9 counted once. RU/EN rejection/navigation, healthy selection/reload/repair rules, fresh-state journal recovery and retained/core/save regression pass. Fresh GameState remains same-process evidence; no physical restart/design acceptance. Initial reproduction teardown logs from injected journal cleanup are documented separately from six primary failures.

Three production files/three existing fixtures changed;7833/7839inputs unchanged, none added/deleted. Other source/art/metas/GUIDs/prefabs/builders/fonts/settings/packages/backend/build guards/user edits preserved. Four handoff documents updated, earlier acceptance/history retained. No personal journal/credentials/Git/phone/hosting/build/store action. All18physical rows pending, paid/video settlement and RuStore production readiness unverified; stock-max restricted, effects last. Registration/failure/settings and other Persist/lifecycle callers remain outside this patch.

**Next:** guarded APK refresh packaging selection recovery. Current APK/sidecar remains766,026,550bytes/SHA256`b699646db5f7ac9effc403c768eb19200b5eb3b58f14858b331f5edb80aa5fb4` and excludes these new source fixes while retaining tutorial/prior recovery. Human201–210collection/review before tuning remains parallel.

'''
for name in ('CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule'):
 p=ROOT/'ProjectDocs/AI'/f'{name}.md';old=p.read_text();assert old==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
 p.write_text(entry+old if name in ('CurrentTask','ConversationHandoff') else (old.rstrip()+'\n\n'+entry).rstrip()+'\n')
for run in ('Reproduce','Focus','Boundaries','Core','Navigation','Retained','Edit'):
 summary=json.loads((QA/run/'summary.json').read_text());local=Path(summary['rawLogs'])
 for name in ('unity.log','launcher.log'):shutil.copy2(local/name,QA/run/name)
print('Actual results and four handoff documents recorded; earlier history retained.')
