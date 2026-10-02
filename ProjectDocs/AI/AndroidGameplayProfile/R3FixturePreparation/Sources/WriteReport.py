from pathlib import Path
import json

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
evidence=json.loads((BASE/'QA/Reconciliation.json').read_text())
assert evidence['result']=='Passed' and evidence['unitySeparateProcessesPassed']==14
prefix='AndroidGameplayProfile/R3FixturePreparation'
next_action='Implement the six bounded preset selectors and a pure fixture factory in the existing DevelopmentGameplayProfile, including foreign-owner rejection and failed-commit isolation. Preserve the current fresh/resume/empty-energy behavior and development/release guards; verify normal saves, cold resume, native stock and recap before a separate guarded APK refresh. Human rounds 201–210 remain the parallel acceptance action. Phone execution and hosting remain deferred; final gameplay effects stay last.'
table='\n'.join('| `'+row['selector']+'` | '+str(row['completedRounds'])+' | '+str(row['recapScenes'])+' | '+str(row['stars'])+' | [DTO]('+row['dtoPath']+') |' for row in evidence['rehearsedPresets'])
total=sum(row['durationSeconds'] for row in evidence['runs'])
report=f'''# Disposable fixed-fixture preparation R3 — 2026-10-02

Prepared six bounded synthetic reference fixtures. **All 14 separate Unity process checks passed**, zero runtime failures/skips; **34 standalone compiled entry-guard checks passed**. The existing profile and APK still support only fresh/resume/empty-energy. Proposed selector names are not installed Android commands, and the exported DTOs are not a supported import route.

| Proposed preset | Completed rounds | Eligible recap scenes | Stars | Reference |
| --- | ---: | ---: | ---: | --- |
{table}

[Bounded scope](FixtureScope.md) · [Machine-readable plan](FixturePlan.json) · [Actual reconciliation](QA/Reconciliation.json) · [14-process plan](Sources/PhasePlan.json) · [Compiled guard results](QA/EntryGuards/summary.json).

Unity 6000.5.9f1 ran sequentially in one new disposable product store under /tmp, with normal writer/reader processes bracketing six fresh writer/resume reader pairs. All launchers exited 0; their total duration was {total/60:.2f} minutes. These are 14 phase executions of two test methods, not 14 distinct method names or a full-project sweep. Synthetic normal save primary/backup/staging, language and compliance fingerprints remain identical across every QA phase and the final normal restore.

The active v6 journal is kept separate from the v9 campaign development session. Each fixture commits through existing GameState/SaveService behavior. All four booster quantities round-trip exactly at 999, 1 000, 9 999 and Int32.MaxValue. Early story has eight completed rounds, three normally advanced scenes and History page2. Finale has all500 completed rounds, all58 scenes, 1 500 stars and an acknowledged actual round500 ending; Chapter2 remains unavailable. These are synthetic service-level completions with maximum test scores, not legal-swap replays or human outcomes.

Both story resume processes opened the actual native Profile History reader on page2. Early Back and completed finale return preserve the same page, complete player payload and all three QA journal slots. A replay completion rejects duplicate first-clear currency and booster grants. Stock maximum is limited to display/consumption: the existing reward code adds booster quantities without overflow protection; this milestone does not change that economy behavior.

An initial temporary test compilation failed because this NUnit version lacks Is.AnyOf. The corrected prototype uses ordinary membership assertions; original source, initial run/log and initial phase plan are retained. No production correction or assertion removal was made. [Initial compiler run](QA/NormalBefore/summary.json) · [Corrected normal writer](QA/NormalBeforeVerified/summary.json) · [Archived prototype](Sources/FixturePrototype.cs).

The 34 guard checks compile the actual unchanged profile source for Android development and release against fake JNI/preferences. They cover existing activation/package/debuggable/owner/failure/session-lock controls and rejection of all six proposed commands before writes. Release paths make no JNI call or mutation and exclude the badge. This is not actual Android JNI, a new Unity release build or device activation. The planned additional foreign-owner rule for new presets remains an implementation requirement.

The temporary prototype and its metadata were removed after Unity closed. **All 7,833 task-start Assets/Packages/ProjectSettings/Backend inputs are byte-identical**, including existing tests, production runtime, art/fonts/prefabs/GUIDs/metas, approved Splash, settings/packages/backend/guards and inherited edits. Six documentation files change, plus new task scope/tools/evidence. [Scope](QA/Scope.json) · [Exact temporary cleanup](QA/TemporaryFixtureCleanup.json) · [Task-only patch](Sources/TaskChanges.patch) · [Final reconciliation](QA/FinalValidation.json).

Existing R3 APK/sidecar remains 766,017,314 bytes / SHA256 2302b069bddf4b0261513fa5696bfa60dc9940f1c9bd61bc34a54c063079335e. Five normal manual recordings are unchanged. No normal personal journal/credential access, new Edit Mode suite, Android build/device/ADB action, hosting/deployment or commit. No new design/capture or visual acceptance is claimed; R7's native captures are historical.

**Next:** {next_action}
'''
(BASE/'TestResults.md').write_text(report)
summary=f'''# 2026-10-02 — Disposable fixed-fixture preparation R3 complete

Prepared six bounded Editor reference fixtures: stock 999/1 000/9 999/Int32.MaxValue, early acknowledged story and fully acknowledged round500 finale. **14 separate Unity process checks passed**, zero runtime failures/skips; **34 compiled entry-guard checks passed** with fake JNI/preferences. These are phase executions of two methods, not a full sweep or phone evidence. Initial temporary NUnit API compile failure retained and corrected; every final launcher exited 0.

Six fresh writer/resume reader pairs preserve exact payloads and five synthetic normal preference fingerprints, bracketed by normal writer/reader processes. Early has eight completed rounds/three scenes/History page2; finale has 500 rounds/58 scenes/1 500 stars and acknowledged Chapter1. Both actual native recap returns preserve page/full save/journal; replay rejects duplicate grants. Active v6 profile kept separate from v9 campaign development state. Maximum stock is display/consumption only because reward additions lack overflow protection.

[Results]({prefix}/TestResults.md) · [Fixed scope]({prefix}/FixtureScope.md) · [Reference plan]({prefix}/FixturePlan.json) · [Final validation]({prefix}/QA/FinalValidation.json). Current profile/APK still accepts only fresh/resume/empty-energy; proposed selectors are not installed, exports are not an import API. Foreign-owner rejection for new presets remains part of implementation.

All 7,833 project inputs unchanged; temporary rehearsal C#/meta removed. Production scripts/tests/art/fonts/prefabs/GUIDs/metas/approved Splash/settings/packages/backend/guards and prior edits preserved. Six existing docs updated plus new preparation evidence. Current R3 APK/sidecar unchanged at 766,017,314 bytes/SHA256 2302b069bddf4b0261513fa5696bfa60dc9940f1c9bd61bc34a54c063079335e; five normal manual exports unchanged. Isolated test storage only; no new Edit Mode suite/build/device/ADB/normal personal journal/credentials/hosting/commit or visual/human acceptance.

**Next:** {next_action}

'''
docs=ROOT/'ProjectDocs/AI'
current=docs/'CurrentTask.md';current.write_text(summary+current.read_text())
session=docs/'SessionLog.md';session.write_text(session.read_text().rstrip()+'\n\n'+summary)
handoff=docs/'ConversationHandoff.md';text=handoff.read_text()
text=text.replace('Latest completed task: [Core regression R7 after recap]','Previous completed task: [Core regression R7 after recap]',1)
anchor='Previous completed task: [Core regression R7 after recap]'
text=text.replace(anchor,'Latest completed task: [Disposable fixture preparation R3]('+prefix+'/TestResults.md), 2026-10-02.\n\n'+summary.split('\n\n',1)[1].rsplit('**Next:**',1)[0].strip()+'\n\n'+anchor,1)
start=text.index('Next independent scheduled action:');end=text.index('Prepared 2026-09-12.')
text=text[:start]+'Next independent scheduled action: '+next_action+' Preserve historical decisions below.\n\n'+text[end:]
handoff.write_text(text)
schedule=docs/'DevelopmentSchedule.md';text=schedule.read_text()
old=next(line for line in text.splitlines() if line.startswith('**Immediate next action:**'))
text=text.replace(old,'**Immediate next action:** '+next_action,1)
text+='\n**2026-10-02 fixed disposable fixture preparation:** [R3 scope and rehearsal]('+prefix+'/TestResults.md) verifies four stock sizes and early/finale references through 14 separate Unity processes; 34 actual-source entry-guard checks pass with fake JNI/preferences. All 7,833 project inputs and current APK remain unchanged; no new player selectors are installed. Native History page2/recap return and normal-key fingerprints pass; synthetic service completions are not human acceptance. Next implement the bounded factory/selectors and owner/commit guards before a separate APK refresh. Phone/hosting and effects-last deferrals remain.\n'
schedule.write_text(text)
usage=docs/'AndroidGameplayProfile/R1/Usage.md'
usage.write_text(usage.read_text().rstrip()+'\n\n## Fixed fixtures — Editor preparation only\n\n[Fixture preparation R3](../R3FixturePreparation/TestResults.md) supplies six rehearsed reference DTOs and a [bounded implementation scope](../R3FixturePreparation/FixtureScope.md): four stock sizes and early/fully acknowledged finale story. All six proposed names are rejected by the current entry logic; the APK still accepts only fresh/resume/empty-energy. Do not pass proposed selectors as supported extras or import the DTOs. Selector/factory implementation and a later guarded APK refresh remain prerequisites. Physical rows stay pending; normal saves are never a fixture source.\n')
checklist=docs/'AndroidGameplayJourney/R1/DeviceChecklist.md'
checklist.write_text(checklist.read_text().rstrip()+'\n\n### 2026-10-02 fixture preparation update\n\n[Fixed fixture scope and Editor rehearsal](../../AndroidGameplayProfile/R3FixturePreparation/TestResults.md) now defines the stock boundaries, eight-round/three-scene early recap and fully acknowledged 500-round finale. Fourteen isolated Unity process checks and 34 fake-JNI entry checks pass. The current APK has no new selectors and reference DTOs have no supported import route. STOCK-01/02/03 and RECAP-02/03 still require selector/factory implementation, a separately verified APK refresh and resumed physical execution. Every device row remains pending. This preparation does not turn service-level synthetic completions into earned phone evidence.\n')
print(json.dumps({'unitySeparateProcessesPassed':14,'referenceFixtures':6,'existingDocumentsUpdated':6}))
