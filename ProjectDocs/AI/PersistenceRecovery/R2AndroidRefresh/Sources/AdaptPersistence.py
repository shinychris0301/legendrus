"""Extend existing compiled-code checks for ordinary persistence, retaining earlier proof."""
from pathlib import Path
import ast,json
BASE=Path(__file__).resolve().parent
owners=['GameState_Persist','GameState_TryPersist','GameState_MigrateSave','GameState_ResetProgress']
flags={'GameState_Persist':['guardsNestedPublication','delegatesAcknowledgingPersistence','logsRejectedPersistence'],'GameState_TryPersist':['rejectsLockAndReentryBeforeCommit','commitsCurrentLiveSave','scopedFinallyReleasesPersistenceGuard'],'GameState_MigrateSave':['retainsStartupPersistDelegation'],'GameState_ResetProgress':['replacesHistoryBeforeAdoption'],'GameState_TryCommitBalance':['durablePrimaryAcknowledgementBeforeAdoption','commitsBeforeChanged']}
(BASE/'PersistenceNativePolicy.json').write_text(json.dumps({'extraOwners':owners,'flags':flags},indent=2)+'\n')
p=BASE/'CaptureNative.py';s=p.read_text().replace('pattern=re.compile(', 'targets += '+repr(owners)+'\npattern=re.compile(',1)
s=s.replace("set(targets)-{'GameState_TryTogglePreference'}", "set(targets)-{'GameState_TryPersist'}")
s=s.replace("{'ProductionSettingsView_TogglePreference','ProductionSettingsView_Unbind','ProductionSettingsView_InputReady','ProductionSettingsView_Listen','ProductionSettingsView_RefreshInfo'}<=set(changed)","{'GameState_Persist'}<=set(changed)")
block=r'''
if stage=='After':
 policy=json.loads((BASE/'Sources/PersistenceNativePolicy.json').read_text())
 legacy=body('GameState_Persist'); attempt=call(legacy,'GameState_TryPersist')
 flags('GameState_Persist',guardsNestedPublication=legacy.index('___persistenceInProgress')<legacy.index('return;')<attempt,delegatesAcknowledgingPersistence='SaveService_TrySave_' not in legacy and 'GameState_TryCommitBalance_' not in legacy and 'GameState_TryPersist_' in legacy,logsRejectedPersistence=attempt<call(legacy,'Debug_LogError') and 'GameState_get_LastWriteResult_' in legacy)
 ordinary=body('GameState_TryPersist'); commit=call(ordinary,'GameState_TryCommitBalance')
 flags('GameState_TryPersist',rejectsLockAndReentryBeforeCommit=ordinary.index('___canPersist')<ordinary.index('___persistenceInProgress')<ordinary.index('return (bool)0;')<ordinary.index('___persistenceInProgress = (bool)1;')<commit,commitsCurrentLiveSave='__this->___Save' in ordinary and 'JsonUtility_' not in ordinary and 'SaveService_TrySave_' not in ordinary,scopedFinallyReleasesPersistenceGuard=ordinary.index('il2cpp::utils::Finally(')<ordinary.index('___persistenceInProgress = (bool)0;')<ordinary.index('\n\t\ttry\n')<commit)
 flags('GameState_MigrateSave',retainsStartupPersistDelegation='GameState_Persist_' in body('GameState_MigrateSave'))
 reset=body('GameState_ResetProgress'); replacement=call(reset,'SaveService_TryReplaceHistory')
 flags('GameState_ResetProgress',replacesHistoryBeforeAdoption=replacement<reset.index('__this->___Save =') and 'PlayerSaveWriteResult_get_PrimaryDurablyCommitted_' in reset and 'SaveService_TrySave_' not in reset)
 balance=body('GameState_TryCommitBalance'); written=call(balance,'SaveService_TrySave'); primary=call(balance,'PlayerSaveWriteResult_get_PrimaryDurablyCommitted'); adoption=balance.index('__this->___Save ='); publication=call(balance,'Action_Invoke')
 flags('GameState_TryCommitBalance',durablePrimaryAcknowledgementBeforeAdoption=written<primary<adoption and 'GameState_set_LastWriteResult_' in balance[:primary] and 'return (bool)0;' in balance[primary:adoption],commitsBeforeChanged=written<adoption<publication)
 for owner,keys in policy['flags'].items(): assert all(record['methods'][owner].get(k) is True for k in keys),(owner,record['methods'][owner])
 assert all((n+'\0').encode() in metadata for n in ('TryPersist','persistenceInProgress'))
 record['acknowledgedOrdinaryPersistenceRecoveryPackaged']=True
 record['persistenceOwnerRegionsVerified']=len(policy['extraOwners'])
else:
 assert b'TryPersist\0' not in metadata and b'persistenceInProgress\0' not in metadata
 record['ordinaryPersistenceFixAbsentFromPriorApk']=True
'''
needle="(QA/(stage+'Native.json')).write_text";assert needle in s;s=s.replace(needle,block+'\n'+needle,1);ast.parse(s);p.write_text(s)
for name in ('VerifyBuild.py','VerifyFinal.py'):
 p=BASE/name;s=p.read_text()
 if name=='VerifyBuild.py':
  needle='with zipfile.ZipFile(apk) as z:'
  extra="policy=json.loads((BASE/'Sources/PersistenceNativePolicy.json').read_text()); assert native['acknowledgedOrdinaryPersistenceRecoveryPackaged'] and native['persistenceOwnerRegionsVerified']==4\nassert all(native['methods'][o][flag] is True for o,keys in policy['flags'].items() for flag in keys)\nassert read('BeforeNative.json')['ordinaryPersistenceFixAbsentFromPriorApk'] is True\n"
  assert needle in s;s=s.replace(needle,extra+needle,1)
  s=s.replace("'atomicSettingsPreferenceRecoveryPackaged':True", "'acknowledgedOrdinaryPersistenceRecoveryPackaged':True,'persistenceOwnerRegionsVerified':4,'atomicSettingsPreferenceRecoveryPackaged':True",1)
 else:
  s=s.replace("('atomicSettingsPreferenceRecoveryPackaged'","('acknowledgedOrdinaryPersistenceRecoveryPackaged','atomicSettingsPreferenceRecoveryPackaged'",1)
  s=s.replace("'nativeOwnersVerified':72", "'nativeOwnersVerified':72,'persistenceOwnerRegionsVerified':4",1)
 ast.parse(s);p.write_text(s)
print('Persistence native audit prepared: 71 before/72 after owners, four persistence regions; earlier callbacks retained.')
