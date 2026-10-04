"""Record actual source-stage outcomes and preserve earlier continuity decisions."""
from pathlib import Path
import difflib,json,shutil
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
read=lambda n:json.loads((QA/n).read_text())
v=read('SourceValidation.json');t=read('TestReconciliation.json');a=read('ArtifactBefore.json');assert v['result']==t['result']=='Passed'
play=v['unityPlayDistinctPassed'];edit=v['unityEditDistinctPassed'];total=v['unityTotalDistinctPassed'];groups=', '.join(f"{n} {c}" for n,c in t['groups'].items())
report=f'''# Startup migration recovery R1 — source complete

The user's “I approve, go to next” authorizes the next startup/language audit after [Persistence R2](../../PersistenceRecovery/R2AndroidRefresh/TestResults.md). Five actual Unity failures reproduced: rejected v5/v6 normalization adopted a new Save before durable acknowledgement; locked migration could still replace in-memory state after journal repair; a healthy no-op replaced loaded identity; Changed migration reentry replaced the acknowledged owner again. [Reproduction](QA/Reproduce/summary.json) · [Final reconciliation](QA/TestReconciliation.json) · [Source validation](QA/SourceValidation.json) · [Final validation](QA/FinalValidation.json).

Startup now prepares and normalizes the migration candidate separately. It uses the existing shared TryCommitBalance acknowledgement policy before installing it or publishing Changed. A rejected write preserves the exact loaded Save, raw progress and diagnostic and leaves the rewrite pending; the session stays locked. The general persistence guard also blocks migration callback reentry. Successful/no-op migration retains the expected ownership: acknowledged candidates are adopted once; a healthy unchanged load keeps its original identity without a write. Finally cleanup releases the busy guard and clears the pending rewrite when adoption actually occurred, even if a Changed observer throws. Migration validation failure locks further saving. Candidate-only tutorial/story/chapter normalizers and chapter prerequisites preserve all existing rules, including nonempty unknown story IDs.

One production file changed: Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs. Its five changed methods are MigrateSave, NormalizeTutorialProgress, NormalizeStoryProgress, NormalizeChapterCompletionProgress and ChapterCompletionPrerequisitesMet. All other existing production methods, including Awake, ordinary persistence, resets, spending, energy, rewards, receipts and round lifecycle, are byte-identical to the pre-task owner. One existing fixture extended: Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs. Earlier fixture content remains intact. [Review patch](Sources/TaskChanges.patch). No source/dependency/asset additions or deletions.

Unity 6000.5.9f1: **{play} Play + {edit} Edit = {total} distinct final cases passed**, zero failures/skips, seven final launchers exit 0. Final groups: {groups}. Focus 5 and BoundariesCorrected 10 are counted once inside Core. LanguageWrite and LanguageRead are two successful fresh process phases of one test method, counted once in the distinct total. The separate language process check is healthy Editor restart evidence, not Android acceptance. All applicable journal/migration/persistence, settings/feedback/pause/language/native journeys and retained/continuation checks pass.

Boundaries cover healthy v1/v5/v6 commitment and no-op reuse, acknowledged state visible to Changed observers, rejection of nested saving, all four final-round/final-story prerequisite combinations, legacy primary and recovered-backup rewrites with immutable original load evidence, observer-exception cleanup, revision exhaustion and post-repair lock preservation. The initial boundary run passed 7/10: three expectations incorrectly required unknown story IDs to be removed. The existing StoryProgress.Normalize only removes empty/duplicate IDs. That fixture expectation was corrected; policy and production behavior retained. Initial logs/results remain available alongside corrected 10/10 evidence. [Initial boundaries](QA/Boundaries/summary.json) · [Corrected boundaries](QA/BoundariesCorrected/summary.json).

[Language audit](QA/LanguageAudit.json): language is a separate device preference and retains independent QA/profile ownership. Existing language UI and service files are unchanged. Healthy native selection/reload and the dedicated write/read check confirm English survives a fresh Unity process and is shown on the actual Splash text. Source ordering still changes DemoFeatureState's cached selection before LanguagePreference.Save; PlayerPrefs update precedes its flush. This identifies a remaining failure-handling risk, not a reproduced rejected-storage failure. Language write acknowledgement and existing native rejection feedback remain a subsequent source task. No personal-save or credential material was accessed; all runtime tests used isolated generated configuration roots.

All **7,837 of 7,839** other project inputs unchanged; approved Splash/art/metas/GUIDs/prefabs/builders/fonts/revisions/settings/packages/backend/guards and earlier user edits preserved. Four continuity documents updated from six pre-task file backups; acceptance/history retained. New scripts, patches, JSON/XML, actual Editor captures and logs are under this task folder; captures are regression evidence and do not introduce a new design. No Android build, Git commit/push, phone/hosting action or store upload occurred.

Current APK and exact sidecar remain **{a['bytes']:,} bytes/SHA256 `{a['sha256']}`**. The previously audited packaged MigrateSave still adopts then delegates to Persist; the unchanged artifact **does not include this startup source fix**. Previous persistence and all earlier recovery fixes remain packaged. Physical restart/gameplay/audibility/haptics/notification delivery and all 18 device rows remain pending. Real paid/video settlement, production RuStore readiness and measured profitability remain unverified. Human rounds 201–210 still unrecorded from prior confirmation; no rescan or tuning. Stock-max restricted; effects last.

**Next:** guarded Android refresh packaging startup migration recovery, then acknowledge language writes and handle rejection in the existing native UI. Human 201–210 recordings/ratings/story and difficulty observations remain parallel before tuning; phone/hosting deferred.
'''
(BASE/'TestResults.md').write_text(report)
entry=f'''# 2026-10-03 — Startup migration recovery R1 complete (source)

[Results](StartupRecovery/R1/TestResults.md) · [Reconciliation](StartupRecovery/R1/QA/TestReconciliation.json) · [Final validation](StartupRecovery/R1/QA/FinalValidation.json). Five pre-fix Unity failures reproduced. Migration now normalizes a candidate and acknowledges shared durable saving before adoption/publication; rejected rewrites preserve loaded identity/progress/evidence and stay pending/locked. Healthy no-ops keep loaded identity. Migration callback reentry and observer-exception cleanup are guarded; candidate chapter prerequisites retain existing rules.

Unity 6000.5.9f1: {play} Play + {edit} Edit = {total} distinct final passes, zero failures/skips, seven final launchers exit 0. Focus 5/BoundariesCorrected 10 counted once; dedicated language write/read use two fresh Unity processes for one distinct test case. Healthy v1/v5/v6, finale prerequisite matrix, legacy/backup rewrites, rejected revision exhaustion, locked repair and prior journal/runtime/navigation/retained regressions pass. Initial three boundary expectations for unknown scene-ID removal corrected to existing retention policy; evidence preserved. Language healthy restart verified; rejected-language-write handling remains a source-audit finding, not a reproduced/fixed storage failure.

One production owner and one fixture changed; 7,837/7,839 inputs unchanged, no add/delete. Other existing GameState methods and earlier fixtures/art/metas/prefabs/builders/fonts/settings/packages/backend/guards/user edits preserved. Four continuity documents updated with history retained. No build/personal journal/credentials/Git/phone/hosting/store action. All 18 physical rows pending, real paid/video settlement/production RuStore readiness/measured profitability unverified; human 201–210 still unrecorded, no rescan/tuning; stock-max restricted, effects last.

**Next:** guarded APK refresh packaging startup recovery, then language-save acknowledgement and existing native rejection feedback. Current APK/sidecar remains {a['bytes']:,} bytes/SHA256 `{a['sha256']}` and excludes this new startup fix while retaining ordinary persistence and previous recovery. Healthy Unity process restart is separate from Android acceptance. Human 201–210 recording/review remains parallel; phone/hosting deferred.

'''
for name in ('CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule'):
    p=ROOT/'ProjectDocs/AI'/f'{name}.md';old=(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text();assert p.read_text()==old
    p.write_text(entry+old if name in ('CurrentTask','ConversationHandoff') else (old.rstrip()+'\n\n'+entry).rstrip()+'\n')
for folder in sorted(QA.iterdir()):
    if not folder.is_dir() or not (folder/'summary.json').is_file():continue
    r=json.loads((folder/'summary.json').read_text());logs=Path(r['rawLogs'])
    for name in ('unity.log','launcher.log'):shutil.copy2(logs/name,folder/name)
patch=[]
for name in ('CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule'):
    rel='ProjectDocs/AI/'+name+'.md';patch.extend(difflib.unified_diff((BASE/'Sources/Before'/rel).read_text().splitlines(True),(ROOT/rel).read_text().splitlines(True),fromfile='a/'+rel,tofile='b/'+rel))
(BASE/'Sources/HandoffChanges.patch').write_text(''.join(patch))
print('Startup source report and four continuity documents recorded with earlier decisions preserved.')
