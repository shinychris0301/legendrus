"""Record settings recovery evidence and preserve all earlier handoff history."""
from pathlib import Path
import hashlib,json,shutil
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
v=json.loads((QA/'SourceValidation.json').read_text());assert v['result']=='Passed'
play=v['unityPlayDistinctPassed'];edit=v['unityEditDistinctPassed'];total=v['unityTotalDistinctPassed'];artifact=json.loads((QA/'ArtifactBefore.json').read_text())
rows='\n'.join(f'| {name} | {json.loads((QA/name/"summary.json").read_text())["mode"]} | {count} | 0 | 0 |' for name,count in v['freshUnityGroupCounts'].items())
files='\n'.join('- `'+n+'`' for n in v['changedProductionInputs'])
captures=[]
for name in ('Settings_Rejected_RU','Settings_Rejected_360_EN'):
 p=QA/'Boundaries/Captures/LOR_SETTINGS_CAPTURE_DIR'/f'{name}.png'
 with p.open('rb') as f:h=hashlib.file_digest(f,'sha256').hexdigest()
 captures.append({'path':str(p.relative_to(BASE)),'sha256':h,'inspected':True,'source':'Actual isolated Unity Play Mode capture, process-only Vulkan','physicalDevice':False})
(QA/'CaptureReview.json').write_text(json.dumps({'captures':captures,'existingPanelAndCommittedSwitchStateVisible':True,'ruAndEnTextReadable':True,'newDesignAcceptance':False},indent=2)+'\n')
report=f'''# 2026-10-03 — Settings recovery R1: source delivery

The user approved the next task after the round lifecycle APK refresh. This delivery repairs reproduced preference-saving and native callback failures in the existing Settings page. Two production files, two fixtures and four continuity documents were backed up before editing. Human rounds 201–210 remain unrecorded from the user's prior confirmation; their review remains pending.

[Reproduction](QA/Reproduce/summary.json) records **12 primary failures**, zero skips, Unity launcher exit 2: eight legacy toggles mutated the live preference despite rejected or already locked saving; standalone and paused native switches displayed that uncommitted state; a Changed callback switched twice; a retired binding listener still acted. [Focus](QA/Focus/summary.json) passes the same 12 after repair. [Boundaries](QA/Boundaries/summary.json) passes 12 additional checks. No production or test corrections were required after the focused fix.

`TryTogglePreference` validates the index, honors the persistence lock, clones the current save, changes exactly one boolean and delegates to the existing acknowledged commit helper. Music, SFX, vibration and notification legacy wrappers delegate to this API. Rejection leaves the live object, serialized journal, events and feedback preferences unchanged. Successful commitment adopts the candidate before Changed observers run; tests also verify that pending booster receipt identity and the transient failure result survive. Existing durable-primary adoption and post-write cleanup policy remain covered by the journal regression suites. No schema, audio asset, notification permission or platform scheduling change is introduced.

The actual native Settings view consumes the acknowledgement, refreshes from the committed preference and shows its existing InfoPanel with the existing Russian/English nonretry error wording on failure. Close or Back dismisses the message on the exact original Settings root. Paused Settings retains its board, moves and input lock, then returns to Pause through the existing route. The error panel blocks unrelated controls and refreshes correctly in both languages. A binding-owner token rejects callbacks captured before rebind, and a scoped busy guard prevents Changed callback reentry. The existing Settings Revision 5 prefab, builder, serialized bindings, art and layout are unchanged.

Final actual Unity **6000.5.9f1 (b57deb96f08d)**, isolated storage per launcher:

| Group | Mode | Passed | Failed | Skipped |
|---|---|---:|---:|---:|
{rows}

**{play} Play + {edit} Edit = {total} distinct final passes**, zero failures/skips, all five final launchers exit 0. Focus and Boundaries are subsets counted once. [Reconciliation](QA/TestReconciliation.json) · [Source validation](QA/SourceValidation.json) · [Final validation](QA/FinalValidation.json). Raw Unity/launcher logs and results XML are retained. These targeted suites cover Settings controls, feedback configuration, Pause/navigation, native win/fail journeys, retained rounds/continuation, earlier transaction recovery, journal loading/writing and migration. They are actual Unity runs, not source-only checks or a full-project/device acceptance claim. The repaired-journal test creates a fresh GameState in the same Unity process; the old instance remains locked and only the fresh instance can commit. No physical/process restart was tested.

Two actual isolated Unity captures were visually inspected: [Russian portrait](QA/Boundaries/Captures/LOR_SETTINGS_CAPTURE_DIR/Settings_Rejected_RU.png), 941×1672, and [English portrait](QA/Boundaries/Captures/LOR_SETTINGS_CAPTURE_DIR/Settings_Rejected_360_EN.png), 360×640. The existing dark panel and readable localized error text are visible over the unchanged Settings page with the committed switches. [Capture review](QA/CaptureReview.json) records hashes and provenance. Automated layout, input shielding, font/glyph and touch-target checks passed. These are Editor captures, not phone or newly approved design evidence; audibility, physical haptics and notification delivery were not measured.

Changed source/fixtures:

{files}

The task-local [patch](Sources/TaskChanges.patch) reverses cleanly. Exactly four of 7,839 inputs changed, **7,835 unchanged**, none added/deleted. GameState outside preference toggles, Settings outside acknowledgement/callback ownership/error handling, and all earlier fixture checks are preserved. Existing approved Splash, art, metas/GUIDs, actual prefabs/builders, fonts, settings, packages, backend, Android guards and prior user edits remain intact. Four continuity documents retain earlier history and acceptance decisions.

Current development APK remains **{artifact['bytes']:,} bytes**, SHA256 `{artifact['sha256']}`, with unchanged sidecar. Inspected packaged metadata lacks TryTogglePreference: **this APK does not yet include Settings recovery**. Source validation is not an Android compilation. No APK build, Git commit/push, personal-journal or credential access, phone/hosting action or store upload occurred.

**Next:** refresh the Android APK with Settings recovery through the existing guarded workflow, then update artifact pins and evidence. General Persist/reset/load lifecycle paths and the separate language preference remain outside this narrow patch. Human 201–210 recordings/ratings/review remain required before tuning; no rescan or tuning here. All 18 physical rows remain pending and phone/hosting deferred. Real paid/video settlement, RuStore production readiness and measured profitability remain unverified; stock-max remains restricted and effects stay last.
'''
(BASE/'TestResults.md').write_text(report)
entry=f'''# 2026-10-03 — Settings recovery R1 complete (source)

[Results](SettingsRecovery/R1/TestResults.md) · [Reconciliation](SettingsRecovery/R1/QA/TestReconciliation.json) · [Final validation](SettingsRecovery/R1/QA/FinalValidation.json). Twelve pre-fix failures reproduced. Four preference toggles now acknowledge atomic saving before adoption and Changed publication; rejected/locked writes preserve live preferences, journal and feedback state. Existing native Settings error panel explains rejection in RU/EN while preserving the exact standalone or paused root and board. Owned callbacks reject retired listeners and Changed reentry.

Unity 6000.5.9f1: {play} Play + {edit} Edit = {total} distinct final passes, zero failures/skips, five launchers exit 0. Focus 12/Boundaries 12 counted once. Healthy commitment, pending receipt/failure-result preservation, invalid indices, repaired-journal fresh-state recovery, RU/EN modal shielding/localization, settings/feedback/navigation, retained/continuation and journal/migration regression pass. Two actual Editor portraits inspected; fresh GameState recovery is same-process evidence, no physical/process restart or new design acceptance.

Two production files and two fixtures changed; 7,835/7,839 inputs unchanged, none added/deleted. Existing Settings Revision 5 prefab/builder/layout, approved Splash/art/metas/fonts/settings/packages/backend/guards and earlier user edits preserved. Four continuity documents updated with history retained. No build/personal journal/credentials/Git/phone/hosting/store action. General Persist/reset/load lifecycle and separate language preference remain outside this patch. Human 201–210 still unrecorded from prior confirmation; no rescan/tuning. All 18 physical rows pending; real paid/video settlement, RuStore production readiness and measured profitability unverified; stock-max restricted, effects last.

**Next:** guarded APK refresh packaging Settings recovery. Current APK/sidecar remains {artifact['bytes']:,} bytes/SHA256 `{artifact['sha256']}` and excludes these new fixes while retaining round lifecycle and prior recovery. Human 201–210 recording/review remains parallel.

'''
for name in ('CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule'):
 p=ROOT/'ProjectDocs/AI'/f'{name}.md';old=p.read_text();assert old==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
 p.write_text(entry+old if name in ('CurrentTask','ConversationHandoff') else (old.rstrip()+'\n\n'+entry).rstrip()+'\n')
for folder in sorted(QA.iterdir()):
 if not folder.is_dir() or not (folder/'summary.json').exists():continue
 summary=json.loads((folder/'summary.json').read_text());local=Path(summary['rawLogs'])
 for name in ('unity.log','launcher.log'):shutil.copy2(local/name,folder/name)
print('Recorded settings results, copied logs and preserved four continuity histories.')
