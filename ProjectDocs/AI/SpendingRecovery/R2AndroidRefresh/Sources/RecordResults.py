"""Document the inspected artifact and update current offline handoff while retaining history."""
from pathlib import Path
import json,shutil
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';a=json.loads((QA/'Artifact.json').read_text());h=a['sha256'];size=f"{a['bytes']:,}"
report=f'''# Spending and refund recovery Android refresh R2 — complete

[Build reconciliation](QA/BuildValidation.json) · [Native inclusion](QA/AfterNative.json) · [Source results](../R1/TestResults.md) · [Final handoff validation](QA/FinalValidation.json).

Current development APK: `{a['path']}`, **{size}bytes**, SHA256 `{h}`. It packages daily reward, doubled-gold, first-clear and ordinary-credit recovery plus atomic spending and durable ordinary booster-command/refund recovery. Previous DailyR2APK766,014,334bytes/SHA2569f72045fe3a2e68114be24d687dfb2a51c07c48d39ec58aa4afbafe9a5744654 and inherited sidecar are preserved under `Builds/Verification/v1_17_step01/BeforeSpendingRefreshR2/`.

Unity6000.5.9f1 existing guarded development command: exit0, native BuildReport Succeeded;79.07s launcher wall time. Source-stage **221Play+221Edit=442distinct passes** includes160Android/content/fixed-preset policy checks. No additional post-build runtime run claimed. **64 synthetic/mocked offline preflight checks pass**, zero real ADB/device calls. Actual default preflight verifies this new pin and accumulated native transaction evidence, with device work deferred.

APK signature and original development certificate match; package `com.example.legendsofrus.v117debug`, version0.0.0/code1, ARM64, min26/target36, debuggable/portrait, permissions unchanged. ZIP CRC, exact byte accounting/no unindexed gaps,16KiB ZIP alignment, six ELF load alignments, nine credential DEX/JNI classes and four Java/meta pins pass. Existing libunity binary/dynamic bindings are identical. All403ZIP paths retained:398payloads identical, five reviewed changes—IL2CPP metadata, libil2cpp.so, player data pack and two build-identifier payloads. Player data pack grows5bytes; it is not byte-identical. All29,387path/GUID/type rows and seven native page object counts/packed sizes remain equal. Existing page revisions Profile13/Dialogue2/Gameplay24/Victory12/Daily5/receipt2 and all six exact fixed selectors retained.

Twenty-two actual generated C++ definitions and compiled method regions match the corresponding packaged stripped IL2CPP library. Four spending methods call atomic balance commitment without direct Persist; checked catch paths exist in daily, doubled-gold, first-clear, ordinary-credit and refund settlement. New receipt/recovery metadata is present. Prior ten compiled owners are captured; expected spending/recovery changes are verified by native method hashes. This is compiled inclusion evidence, not phone execution or store readiness.

All7,839source-complete production inputs are unchanged after removal of the temporary Editor helper and restoration of four generated Addressables inputs. Existing source fixes/user edits/metas/GUIDs/art/prefabs/fonts/approved Splash/packages/settings/backend/signing context and guards preserved. Three generated packing caches were reversibly relocated to `/tmp/lor-spending-android-refresh-r2/BeforeFreshPacking`; two packaging incremental states are retained under Library/LegendsOfRusPackagingBackups/BeforeSpendingRefreshR2. No private signing material, personal journal, phone or hosting action, Git commit/push or production activation. Eight handoff/checklist/tool files updated with backups; earlier results/acceptance decisions retained.

Next: human rounds201–210 recording and feedback review before tuning; device verification of transactions, lifecycle, input and save isolation when phone work resumes. All18physical checklist rows pending. Paid/video settlement remains unverified; production RuStore readiness/profitability is not established by this development build. Stock-max remains display/consumption only; final effects stay last. General clock refresh/energy credits/refills/Persist callers remain outside the source fix.
'''
(BASE/'TestResults.md').write_text(report)
entry=f'''# 2026-10-03 — Spending/refund recovery and guarded APK refresh complete

[Source results](SpendingRecovery/R1/TestResults.md) · [Build results](SpendingRecovery/R2AndroidRefresh/TestResults.md) · [Build validation](SpendingRecovery/R2AndroidRefresh/QA/BuildValidation.json) · [Handoff validation](SpendingRecovery/R2AndroidRefresh/QA/FinalValidation.json). APK **{size}bytes**, SHA256`{h}`, now packages atomic spending/durable booster refund recovery plus accumulated daily/double/first-clear/ordinary-credit fixes. Previous DailyR2APK and inherited checksum preserved under BeforeSpendingRefreshR2.

Unity6000.5.9f1:221Play+221Edit=442distinct source-stage passes, zero final failures/skips; guarded native Android build succeeds/launcher exit0. Twenty-two generated/native method regions match packaged library; validated optional receipt and exact-board error recovery included. Signature/certificate/package/permissions/CRC/16KiB/sixELFs/nineDEX classes/fourJava-meta pins pass.403ZIP paths retained,398payloads identical/five reviewed changes; data pack grows5bytes but all29387membership rows and seven page sizes remain unchanged.64mocked offline checks pass, zero real ADB; actual default preflight verifies new pin. All7839source-complete build inputs restored/unchanged after temporary-helper cleanup. Approved art/metas/prefabs/fonts/settings/packages/backend/guards retained. Generated caches preserved reversibly; no normal journal/keys/phone/hosting/Git/store action.

**Next:** collect human201–210 recordings/feedback and review before tuning; all18physical rows remain pending until phone work resumes. Real paid/video settlement and production RuStore readiness remain unverified; stock-max display/consumption only, effects last. Fresh GameState recovery is isolated Unity evidence, not a physical/process restart test. Public clock-refresh/energy-credit/refill/general Persist paths remain outside the transaction fix.

'''
for n in ('CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule'):
 p=ROOT/'ProjectDocs/AI'/f'{n}.md';old=p.read_text()
 p.write_text(entry+old if n in ('CurrentTask','ConversationHandoff') else (old.rstrip()+'\n\n'+entry).rstrip()+'\n')
p=ROOT/'ProjectDocs/AI/AndroidGameplayProfile/R1/Usage.md';s=p.read_text()
# Current operational instructions get the current artifact; dated historical sections remain unchanged.
end=s.index('## 2026-10-03 — daily-reward build pin');current,history=s[:end],s[end:]
current=current.replace('verified daily-reward R2','verified spending/refund R2').replace('current R2 APK','current spending/refund R2 APK')
current=current.replace('[verified daily-reward R2 APK](../../DailyRewardRecovery/R2AndroidRefresh/TestResults.md)','[verified spending/refund R2 APK](../../SpendingRecovery/R2AndroidRefresh/TestResults.md)')
current=current.replace('766,014,334 bytes',size+' bytes').replace('9f72045fe3a2e68114be24d687dfb2a51c07c48d39ec58aa4afbafe9a5744654',h)
current=current.replace('Existing ordinary first-clear reward additions do not guard booster overflow; avoid reward-producing actions with this maximum fixture.','Checked reward/credit rejection and durable command refunds are now packaged; avoid reward-producing actions with this maximum fixture, which has no grant capacity.')
current=current.replace('[R5 packaging checks](../R5AndroidRefresh/TestResults.md) verify the factory and exact selectors in the current APK.','[Current packaging checks](../../SpendingRecovery/R2AndroidRefresh/TestResults.md) verify the factory and exact selectors; [R5 checks](../R5AndroidRefresh/TestResults.md) remain historical.')
current=current.replace('The current [verified spending/refund R2 APK](../../DailyRewardRecovery/R2AndroidRefresh/TestResults.md)','The current [verified spending/refund R2 APK](../../SpendingRecovery/R2AndroidRefresh/TestResults.md)')
append=f'''\n## 2026-10-03 — spending/refund build pin

[Current R2 results](../../SpendingRecovery/R2AndroidRefresh/TestResults.md) establish the {size}byte artifact/SHA256`{h}`, matching original certificate and22compiled transaction owners. All accumulated reward/credit/spending fixes and durable booster refunds are packaged. Canonical offline preflight now checks these owner methods and passes64synthetic/mocked boundaries. Earlier dated pins above are historical. No phone activation, process restart, isolation, physical readability or store acceptance claimed; all18physical rows pending. Stock-max remains display/consumption only.\n'''
p.write_text((current+history).rstrip()+'\n'+append)
p=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R1/DeviceChecklist.md';s=p.read_text();end=s.index('## Entry conditions');top=s[:end].replace('[daily-reward Android refresh R2](../../DailyRewardRecovery/R2AndroidRefresh/TestResults.md)','[spending/refund Android refresh R2](../../SpendingRecovery/R2AndroidRefresh/TestResults.md)').replace('766,014,334 bytes',size+' bytes').replace('9f72045fe3a2e68114be24d687dfb2a51c07c48d39ec58aa4afbafe9a5744654',h)
s=top+s[end:];s=s.replace('linked daily-reward R2 build/inspection/preset/native-method evidence','linked spending/refund R2 build/inspection/preset and22native-method evidence').replace('under DailyRewardRecovery/R2AndroidRefresh/QA','under SpendingRecovery/R2AndroidRefresh/QA')
p.write_text(s.rstrip()+f'''\n\n### 2026-10-03 spending/refund packaging R2

[Current results](../../SpendingRecovery/R2AndroidRefresh/TestResults.md) verify this new pin and original certificate,22compiled transaction owners,442Unity source-stage passes and64mocked offline checks. Prior daily-R2artifact/sidecar/evidence retained. All accumulated rewards/credits/spending and durable booster compensation are packaged. Physical process-kill/relaunch, rejected storage/refund recovery and same-board input still require device observations in the existing lifecycle/save/booster rows. No additional physical acceptance; all18rows pending, zero ADB/install/launch/player-data action. Dated older artifact statements above are historical; current supported preset list is in Usage.\n''')
for n in ('Build.unity.log','Build.launcher.log'):shutil.copy2(Path('/tmp/lor-spending-android-refresh-r2/build')/n,QA/'Build'/n)
# A prior source milestone append retained a surplus final blank line; retain its text, normalize EOF.
sourceTool=ROOT/'ProjectDocs/AI/SpendingRecovery/R1/Sources/RecordResults.py';s=sourceTool.read_text().replace("else old.rstrip()+'\\n\\n'+entry)","else (old.rstrip()+'\\n\\n'+entry).rstrip()+'\\n')");sourceTool.write_text(s)
print('Build results and eight current handoff/tool files updated; raw build logs retained.')
