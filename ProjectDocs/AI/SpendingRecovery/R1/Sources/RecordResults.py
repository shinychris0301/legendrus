"""Record source completion while retaining all historical acceptance notes."""
from pathlib import Path
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent
report='''# Spending and rejected booster refund recovery R1 — source complete

[Final validation](QA/FinalValidation.json) · [442-case reconciliation](QA/TestReconciliation.json) · [Source patch](Sources/TaskChanges.patch).

SpendGold, SpendGems, SpendEnergy and TryUseBooster prepare cloned candidates and adopt balances only after the existing journal reports successful saving or an already committed primary. Negative energy spending is rejected before clock mutation; real energy regeneration and its timestamp are calculated in the same spending candidate. Failed saves retain live balances, suppress Changed and follow the existing persistence lock. Already committed primary/failed cleanup is adopted once and locks further writes. Full rewards/credits from preceding tasks remain intact. Public clock refresh, energy credits/refills and general Persist callers are outside this change.

Ordinary board commands atomically debit a booster with a single optional v6 receipt (no schema-version bump). Applied/refunded settlement is acknowledged and idempotent. Unknown/stale IDs cannot settle another receipt. Missing legacy fields remain accepted; malformed receipt fields fail validation. An abandoned reserved command recovers once on a fresh GameState startup. This compensates inventory after the old board is lost; it does not checkpoint board state or replay effects. Campaign's existing owner remains in use.

Rejected refund restores board/moves/score/goals but keeps its active command and durable reservation, blocks input and opens the existing localized error modal. Retry keeps the exact current paused board; successful compensation clears it once. Storage lock disables retry. Close/back leaves for Chapter Map, preserving the reservation for recovery before the next entry. Rejected applied settlement cannot publish a false win. Unresolved reservations block first-clear payout. No design/asset/builder/revision changes.

Unity 6000.5.9f1: **221 Play Mode +221 Edit Mode =442 distinct passes**, zero final failures/skips, seven final launchers exit0. Core112, Runtime16, UI40, Retained51, Journeys2, Edit61, Policy160. Focused duplicate runs count once. Nine pre-fix native failures reproduce seven partial debits, negative-energy minting and incorrectly resumed rejected refund. Three retained native fixture failures were corrected: QA-preset dimensions differed from the real board, modal capture checked blocked underlying controls, and modal Close has no TouchTarget child. Production guards were retained; final native tests use the real 8×7 board and direct close-button raycast. Native RU/EN reject→repeat retry→capacity release→one refund and close→reentry recovery pass; actual Russian pending-refund portrait inspected. Fresh GameState recreation uses isolated test storage and is not a physical/process restart test.

Five production source files and five existing test fixtures changed;7,829of7,839 inputs unchanged, none added/deleted. All metas/GUIDs/art/fonts/prefabs/approved Splash/packages/settings/backend/Android guards retained. Four continuity docs updated without deleting previous decisions. The pre-refresh APK remains766,014,334bytes/SHA2569f72045fe3a2e68114be24d687dfb2a51c07c48d39ec58aa4afbafe9a5744654 at this source milestone; this source-stage pin is historical once the authorized R2refresh finishes. No normal personal journal/credentials inspection, phone/hosting/store action or Git commit/push.

Next: finish the already-authorized guarded Android refresh, packaging doubled-gold, first-clear, ordinary credit and current spending/refund fixes. Human201–210 feedback remains pending; paid/video settlement unverified, all18physical rows pending, phone/hosting deferred, effects last. Stock-max remains display/consumption only.
'''
(BASE/'TestResults.md').write_text(report)
entry='''# 2026-10-03 — Spending and rejected booster refunds R1 complete (source)

[Results](SpendingRecovery/R1/TestResults.md) · [442-case reconciliation](SpendingRecovery/R1/QA/TestReconciliation.json) · [Validation](SpendingRecovery/R1/QA/FinalValidation.json). Seven pre-fix partial debits, negative-energy minting and rejected-refund resume reproduced. Four spending APIs now prepare atomic candidates; failed saves preserve balances/clock and lock further writes. Optional validated v6 booster-command receipts reserve/debit together and settle applied/refunded once. Rejected refund pauses the real board, retains its command and opens the existing localized error modal; exact-board retry or close→reentry recovery compensates once. Fresh GameState startup recovers abandoned reservations. No board checkpoint or physical/process-restart acceptance claimed; campaign owner retained.

Unity6000.5.9f1:221Play+221Edit=442distinct passes, zero final failures/skips; all seven final launchers exit0. Retained51, journeys2, native RU/EN error recovery, legacy receipt validation, overflow/save rejection and build guards included. Focused repeated cases counted once; nine pre-fix failures and three corrected capture-fixture failures retained. Five production files plus five existing fixtures changed;7829of7839 inputs unchanged, none added/deleted. Art/metas/prefabs/fonts/approved Splash/settings/packages/backend/guards preserved. Public clock-refresh/energy-credit/refill/general Persist paths remain outside this change.

**Next (already authorized):** guarded APK refresh with accumulated daily/double/first-clear/credit/spending/refund fixes. At this source milestone APK is still766,014,334bytes/SHA2569f72045fe3a2e68114be24d687dfb2a51c07c48d39ec58aa4afbafe9a5744654 and excludes newer fixes. No phone/hosting/credentials/personal journal/Git action. Human201–210 feedback and18physical rows remain pending; paid/video settlement unverified, stock-max display/consumption only, effects last.

'''
for n in ('CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule'):
 p=ROOT/'ProjectDocs/AI'/f'{n}.md';old=p.read_text();backup=BASE/'Sources/Before'/p.relative_to(ROOT);assert old==backup.read_text()
 p.write_text(entry+old if n in ('CurrentTask','ConversationHandoff') else (old.rstrip()+'\n\n'+entry).rstrip()+'\n')
print('Source results and four continuity documents recorded.')
