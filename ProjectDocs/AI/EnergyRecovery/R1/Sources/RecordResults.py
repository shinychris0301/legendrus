"""Record verified source completion without erasing previous acceptance or APK milestones."""
from pathlib import Path
import json
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
v=json.loads((QA/'SourceValidation.json').read_text());assert v['result']=='Passed'
play=v['unityPlayDistinctPassed'];edit=v['unityEditDistinctPassed'];total=v['unityTotalDistinctPassed'];counts=v['freshUnityGroupCounts'];h=v['currentApkSha256']
report=f'''# Energy grant, refill and clock recovery R1 — source complete

[Source validation](QA/SourceValidation.json) · [Test reconciliation](QA/TestReconciliation.json) · [Source patch](Sources/TaskChanges.patch) · [Final validation](QA/FinalValidation.json).

The scheduled human block201–210 is still unrecorded, confirmed by the user this turn. Fresh review of the five manual recorder exports finds0eligible outcomes and grants no acceptance. [Current recording review](QA/HumanRecordingReview/Review.json). Difficulty/pacing tuning remains dependent on real outcomes/ratings/block observations. Phone/hosting work remains deferred. Independent save-integrity work addresses nine demonstrated native failures.

Before this patch, AddEnergy(int.MaxValue) wrapped energy1/3/5 to0, clock catch-up near an Int32.MaxValue cap wrapped to a negative value or incorrectly rejected spending, and rejected AddEnergy/RefillEnergy/RefreshEnergyFromClock changed live energy/timestamps. A refresh even changed a full-energy timestamp while saving was locked. All9pre-fix failures are retained in QA/Reproduce (actual Unity6000.5.9f1); these are synthetic arithmetic/storage boundaries, not evidence of a normal production cap change.

Existing void entry points delegate to TryAddEnergy, TryRefillEnergy and TryRefreshEnergyFromClock. Credits/refills prepare candidates through the existing atomic balance writer. Saving failure preserves the original live save/energy/clock and emits no Changed event; subsequent energy mutations reject while locked. Existing durably committed-primary handling remains in the shared writer. The clock prepares its capped energy/timestamp together, using long arithmetic before the existing cap; SpendEnergy reuses that candidate rule and retains its atomic debit. Default energy cap5 and30-minute interval are unchanged. Negative/zero grants keep existing nonpositive behavior. Full energy or no due refill is a healthy successful no-op, preserving the original timestamp/LastWriteResult with no write/event; spending from full starts the countdown at the actual spending time. No fabricated elapsed time or new economy cap.

Fresh Unity6000.5.9f1: **{play}Play+{edit}Edit={total}distinct passes**, zero final failures/skips, six final launchers exit0. Groups: Core{counts['Core']}, UI{counts['UI']}, Runtime{counts['Runtime']}, Retained{counts['Retained']}, Journey{counts['Journey']}, Edit{counts['Edit']}. Focused24/24 repeated within final groups and counted once. Initial focused10/11 failed because a live HUD callback advanced the test clock during fixture setup; the isolated fixture now seeds its journal without invoking unrelated Changed listeners, with no production listener changes. Normal grants/refills, nonpositive/Int32 limits,30-minute/remainder/rollback/no-op/reload, failed-save preservation/locks, prior reward/spending/booster receipts, RU/EN native energy-modal gate/regeneration/full state, live HUD, legal win/fail journeys and retained decisions pass. Edit includes journal-write/load interruption/future-version handling, migration, campaign boosters and energy UI guards.

Actual isolated native portrait [regenerated energy RU](QA/UI/Captures/LOR_OUT_OF_ENERGY_CAPTURE_DIR/OutOfEnergy_Regenerated_RU.png) inspected: badge/header1energy and real countdown visible; service offers remain unavailable. No new design, human/physical readability acceptance, OS lifecycle simulation or personal-save inspection. Save-journal fault matrices are separate from GameState's synthetic future-journal rejection; no actual disk-full/power-loss injection is claimed.

Only GameState.cs and the existing LevelSystemRuntimePlayModeTests.cs changed from this task's start;7,837of7,839 inputs unchanged, none added/deleted. Other GameState methods/previous fixes byte-preserved apart from the shared writer's clarified comment. All current Router/Board/receipt changes, metas/GUIDs/art/prefabs/fonts/approved Splash/settings/packages/backend/Android guards and pending user work retained. Four continuity docs updated with preserved historical decisions. Five manual exports remain unchanged. General Persist/progress/settings callers are outside this fix; commerce/ads are not enabled. Void wrappers cannot acknowledge rejection; callers needing acknowledgement must use the Try APIs.

The existing development APK/sidecar remains766,022,530bytes/SHA256`{h}`. Its three new energy API names are absent from current player metadata, so this energy fix is **not yet packaged**. The APK still includes earlier verified reward/credit/spending/booster fixes. No new Android build, Git commit/push, phone/hosting/store or credential action occurred.

Next implementation: guarded development APK refresh after preserving the current SpendingR2artifact, verifying actual compiled energy owner methods and retaining signature/identity/permissions/packed-page membership. Human201–210 recording/feedback remains parallel; all18physical rows pending, paid/video settlement unverified, stock-max display/consumption only, effects last.
'''
(BASE/'TestResults.md').write_text(report)
entry=f'''# 2026-10-03 — Energy save/clock recovery R1 complete (source)

[Results](EnergyRecovery/R1/TestResults.md) · [Reconciliation](EnergyRecovery/R1/QA/TestReconciliation.json) · [Final validation](EnergyRecovery/R1/QA/FinalValidation.json). User confirmed human201–210still unrecorded; fresh five-export review returns0eligible outcomes. Nine native pre-fix failures reproduced: three largest-grant wraps, two clock/spend arithmetic failures and four rejected/locked-save balance/timestamp mutations. AddEnergy/RefillEnergy/RefreshEnergyFromClock now delegate to acknowledging Try APIs and prepare atomic candidates. Shared widened clock helper also protects SpendEnergy. Existing cap/30-minute interval retained; full/no-due clock preserves timestamp without writes/events, and full-energy spending starts its countdown at real spending time.

Unity6000.5.9f1:{play}Play+{edit}Edit={total}distinct passes, zero final failures/skips; six final launchers exit0. Focus24counted once within final groups; initial focused fixture failure corrected by avoiding unrelated HUD callbacks during journal seeding. Native energy RU/EN/full/regenerated/HUD, prior rewards/spending/refunds, retained rounds, win/fail and journal/migration guards pass. Actual regenerated RUportrait inspected, no new design/human/physical acceptance. Two production inputs changed/7837unchanged, none added/deleted; other GameState code and existing art/metas/prefabs/fonts/settings/packages/backend/guards/user edits preserved. No normal journal/credentials/Git/phone/hosting/build action.

**Next:** guarded APK refresh packaging this source fix. Current APK remains766,022,530bytes/SHA256`{h}` and excludes new energy APIs, while retaining prior transaction fixes. Human201–210outcomes remain required before tuning;18physical rows pending, paid/video settlement unverified, stock-max restricted, effects last. General Persist/settings/progress operations remain outside this fix.

'''
for n in ('CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule'):
 p=ROOT/'ProjectDocs/AI'/f'{n}.md';old=p.read_text();assert old==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text(),n
 p.write_text(entry+old if n in ('CurrentTask','ConversationHandoff') else (old.rstrip()+'\n\n'+entry).rstrip()+'\n')
print('Verified source results and four continuity records written.')
