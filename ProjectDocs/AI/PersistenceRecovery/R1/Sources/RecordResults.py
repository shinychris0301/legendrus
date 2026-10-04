"""Record ordinary persistence recovery, explicit-reset boundaries and source handoff."""
from pathlib import Path
import json,shutil
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
v=json.loads((QA/'SourceValidation.json').read_text());assert v['result']=='Passed'
play=v['unityPlayDistinctPassed'];edit=v['unityEditDistinctPassed'];total=v['unityTotalDistinctPassed'];artifact=json.loads((QA/'ArtifactBefore.json').read_text())
rows='\n'.join(f'| {name} | {json.loads((QA/name/"summary.json").read_text())["mode"]} | {count} | 0 | 0 |' for name,count in v['freshUnityGroupCounts'].items())
files='\n'.join('- `'+n+'`' for n in v['changedProductionInputs'])
report=f'''# 2026-10-03 — Ordinary persistence recovery R1: source delivery

The user approved auditing the remaining save/reset/load paths after the Settings APK refresh. This delivery fixes the verified general Persist lock bypass and reentry defect. One production owner, one existing fixture and four continuity documents were backed up before edits. Existing startup migration, reset replacement, journal codecs/repository and retained-round retry rules were inspected; no new startup, lifecycle handler or reset UI was added.

[Reproduction](QA/Reproduce/summary.json) records **six primary failures**, zero skips, Unity launcher exit 2: four existing or newly loaded locked sessions overwrote a repaired future/corrupt journal with their stale/fallback save; a locked ordinary retry rescanned and replaced its original failure diagnostic; a Changed observer caused a second ordinary write/publication. [Focus](QA/Focus/summary.json) passes the same six after repair. [Corrected boundary run](QA/BoundariesCorrected/summary.json) passes **13 additional checks**.

`TryPersist` now returns an acknowledgement, checks CanPersist and rejects nested general-persistence calls. It delegates the existing live Save to the shared acknowledged commit helper under a busy guard released in finally. Healthy writes preserve the existing Save/progress object identity, pending booster receipt and failure result; observers see the saved journal before publication. Existing durable-primary adoption/cleanup-failure policy is reused: committed primaries are acknowledged and later saving remains locked if cleanup failed. The void Persist wrapper preserves failed-write diagnostics and suppresses its own reentrant call. A repaired disk journal alone does not unlock the old state; a fresh GameState can load the repaired history and persist normally. No ordinary retry replaces the first diagnostic or writes over a read-locked fallback.

This legacy API persists **already prepared live Save data**. It does not roll back arbitrary direct edits to the public Save object made by callers before invoking it. The guard covers general persistence and its own Changed callbacks; it is not a new global ownership guard for every gameplay transaction. Existing candidate-based gameplay/settings APIs retain their previous behavior and tests. The active runtime call found outside Persist itself is the existing MigrateSave rewrite; other external usages are Editor/test setup. No timer or fabricated progress was introduced.

Explicit ResetProgress remains byte-identical. Tests verify that future/corrupt storage rejects reset without clearing progress, completion/failure presentation or journal evidence. A deliberate reset after journal repair remains allowed to replace both committed copies even if ordinary persistence is locked; backup recovery cannot resurrect pre-reset progress. This is the existing explicit replacement policy, exercised only in isolated fixtures. No normal/personal progress was reset.

Startup migration/reset/load methods remain byte-identical. Healthy v5 legacy and corrupt-primary/valid-backup startup rewrites preserve balances and original load diagnostics while producing readable current journals. An exhausted-revision v5 startup rewrite preserves every journal slot, records RevisionExhausted and locks ordinary retries. Startup still normalizes its in-memory representation before rewrite as in the existing implementation; this delivery does not introduce migration rollback or redesign load fallback/presentation. Same-process fresh GameState evidence is distinct from a physical/process restart.

Final actual Unity **6000.5.9f1 (b57deb96f08d)**, isolated product storage per launcher:

| Group | Mode | Passed | Failed | Skipped |
|---|---|---:|---:|---:|
{rows}

**{play} Play + {edit} Edit = {total} distinct final passes**, zero failures/skips, all five final launchers exit 0. Focus 6 and BoundariesCorrected 13 are subsets counted once. [Reconciliation](QA/TestReconciliation.json) · [Source validation](QA/SourceValidation.json) · [Final validation](QA/FinalValidation.json). Logs/XML/cleanup records retained. Suites cover previous transaction and Settings recovery, legacy/future/corrupt/read-only loading, migrations/journal policies, native Settings/Pause/error/navigation, win/fail journeys and retained/continuation recovery. Targeted regression only; no fresh full-project/device or human gameplay acceptance claim. Play Mode uses process-only Vulkan; project graphics/backend/settings remain unchanged.

The initial boundary launch failed test compilation because this Play Mode assembly does not reference SaveCore directly. The fixture was corrected to reflect the already loaded public codec and its actual Envelope property, preserving the existing asmdef/dependencies. No tests executed in that failed launch. [Initial evidence](QA/Boundaries/summary.json) · [Fixture correction](QA/BoundaryFixtureCorrection.json). This required no production correction beyond the original focused fix; all 13 boundary checks passed on the corrected run.

The first broader Core launcher stalled before testing on Unity licensing reconnect/timeouts. Its verified task-only process was stopped after TERM failed; evidence and summary remain under Core. CoreFinal also stalled, and its launcher reported disk I/O errors. A disposable SQLite probe independently reproduced that error in /tmp while project and /dev/shm probes passed. Two known generated cache backups were byte-verified and relocated to /dev/shm, preserving their original paths with links. The /tmp SQLite probe then passed. CoreVerified uses isolated configuration under /tmp with logs in /dev/shm. [Storage recovery](QA/TemporaryStorageRecovery.json) records the bounded relocation. [Launcher recovery](QA/CoreLauncherRecovery.json) records the exact scope; no credentials/license files or other Editors were changed. Only completed passing runs contribute to the final totals.

Changed source/fixture:

{files}

The task-local [patch](Sources/TaskChanges.patch) reverses cleanly. Exactly two of **7,839 inputs** changed, **7,837 unchanged**, none added/deleted. GameState outside general persistence and its private busy flag, including Awake/migration/reset/previous gameplay recovery, is byte-preserved. All earlier fixture checks are retained. Approved Splash, Settings Revision 5 prefab/builder/layout, other art/metas/GUIDs/fonts, packages/backend/project settings/build guards and prior user edits remain intact. Four continuity documents retain earlier history and acceptance decisions.

Current development APK remains **{artifact['bytes']:,} bytes**, SHA256 `{artifact['sha256']}`, unchanged sidecar. Its inspected metadata lacks TryPersist: **this APK does not yet include ordinary persistence recovery**, while retaining Settings and earlier fixes. Source checks are not an Android compilation. No build, Git commit/push, personal-journal/credential access, phone/hosting action or store upload occurred. Host UTC test timestamps are 2026-10-04; task date 2026-10-03 follows the user's America/New_York date.

**Next:** refresh the APK through the installed guarded build workflow, update pins and verify compiled ordinary persistence/reentry inclusion. Further migration normalization/load-presentation and separate language recovery remain outside this narrow fix. Human 201–210 recordings/ratings/review are still required before tuning, previously confirmed unrecorded; no rescan/tuning. All 18 physical rows remain pending, phone/hosting deferred. Real paid/video settlement, RuStore production readiness and measured profitability remain unverified; stock-max remains restricted and effects stay last.
'''
(BASE/'TestResults.md').write_text(report)
entry=f'''# 2026-10-03 — Ordinary persistence recovery R1 complete (source)

[Results](PersistenceRecovery/R1/TestResults.md) · [Reconciliation](PersistenceRecovery/R1/QA/TestReconciliation.json) · [Final validation](PersistenceRecovery/R1/QA/FinalValidation.json). Six pre-fix failures reproduced. General Persist now delegates to acknowledging TryPersist, respects the session lock, retains the original rejection evidence and guards its Changed reentry. Existing live Save/progress identity and pending receipts remain intact; shared durable-primary policy reused. Old locked sessions cannot overwrite repaired history; fresh state recovery and explicit reset retain distinct existing policies.

Unity 6000.5.9f1: {play} Play + {edit} Edit = {total} distinct final passes, zero failures/skips, five launchers exit 0. Focus 6/BoundariesCorrected 13 counted once. Healthy/failed writes, future/corrupt/read-locked state, revision exhaustion, legacy/backup startup rewrites, explicit reset rejection/history replacement and observer-exception cleanup pass alongside prior recovery/navigation/retained/journal regression. Initial test-assembly reference error corrected in the fixture with no dependency/production change; evidence retained. Fresh state recovery is same-process evidence, no physical/process restart acceptance.

One production file and one fixture changed; 7,837/7,839 inputs unchanged, none added/deleted. Existing startup migration/reset methods, approved art/metas/prefabs/builders/fonts/settings/packages/backend/guards and prior user edits preserved. Four continuity documents updated with history retained. No build/personal journal/credentials/Git/phone/hosting/store action. The legacy API persists already prepared live Save; arbitrary prior direct edits and migration normalization rollback/load-presentation remain outside this fix. Separate language recovery remains pending. Human 201–210 still unrecorded from prior confirmation; no rescan/tuning. All 18 physical rows pending, real paid/video settlement/RuStore production readiness/measured profitability unverified, stock-max restricted, effects last.

**Next:** guarded APK refresh packaging ordinary persistence recovery. Current APK/sidecar remains {artifact['bytes']:,} bytes/SHA256 `{artifact['sha256']}` and excludes these new fixes while retaining Settings and prior recovery. Human 201–210 recording/review remains parallel.

'''
for name in ('CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule'):
 p=ROOT/'ProjectDocs/AI'/f'{name}.md';old=p.read_text();assert old==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
 p.write_text(entry+old if name in ('CurrentTask','ConversationHandoff') else (old.rstrip()+'\n\n'+entry).rstrip()+'\n')
for folder in sorted(QA.iterdir()):
 if not folder.is_dir() or not (folder/'summary.json').exists():continue
 summary=json.loads((folder/'summary.json').read_text());local=Path(summary['rawLogs'])
 for name in ('unity.log','launcher.log'):shutil.copy2(local/name,folder/name)
print('Recorded ordinary persistence results and four continuity histories; earlier decisions retained.')
