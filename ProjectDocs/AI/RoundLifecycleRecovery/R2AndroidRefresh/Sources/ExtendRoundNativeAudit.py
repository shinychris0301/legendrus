"""Add generated control-flow checks to the compiled/packaged owner correlation."""
from pathlib import Path
p=Path(__file__).resolve().parent/'CaptureNative.py';s=p.read_text()
needle="(QA/(stage+'Native.json')).write_text"
block=r'''
if stage=='After':
 policy=json.loads((BASE/'Sources/RoundNativePolicy.json').read_text())
 def body(owner):return selected[owner][1]
 def flags(owner,**values):record['methods'][owner].update(values)
 def call(b,prefix):return re.search(r'\b'+re.escape(prefix)+r'[A-Za-z0-9_]*\(',b).start()
 for owner in policy['atomicApis']:
  flags(owner,callsAtomicRoundCommit='GameState_TryCommitRoundCandidate_' in body(owner))
  assert record['methods'][owner]['callsAtomicRoundCommit'] and not record['methods'][owner]['callsPersist'],owner
 flags('GameState_RegisterLevelAttempt',delegatesToAcknowledgedAttemptRegistration='GameState_TryRegisterLevelAttempt_' in body('GameState_RegisterLevelAttempt') and 'GameState_Persist_' not in body('GameState_RegisterLevelAttempt'))
 flags('GameState_RecordLevelFailure',delegatesToAcknowledgedFailure='GameState_TryRecordLevelFailure_' in body('GameState_RecordLevelFailure') and 'GameState_Persist_' not in body('GameState_RecordLevelFailure'))
 gate=body('GameState_CanRegisterLevelAttempt')
 flags('GameState_CanRegisterLevelAttempt',guardsRetainedAndContinuation=all(t in gate for t in ('GameState_ContinuationBlocksNewAttempt_','GameState_TryGetRetainedRound_','GameState_IsLevelUnlocked_')),respectsRoundWriteLock=gate.index('___canPersist')<gate.index('GameState_ContinuationBlocksNewAttempt_'))
 for owner in ('GameState_TryRegisterLevelAttempt','GameState_TryStartLevel'):
  b=body(owner);assert b.index('GameState_CanRegisterLevelAttempt_')<call(b,'JsonUtility_FromJson_TisPlayerSaveData')
  flags(owner,preparesAttemptBeforeCommit=b.index('GameState_TryPrepareLevelAttempt_')<b.index('GameState_TryCommitRoundCandidate_'))
 started=body('GameState_TryStartLevel');clock=started.index('GameState_AdvanceEnergyClock_');debit=started.index('___energy =',clock);prepare=started.index('GameState_TryPrepareLevelAttempt_',debit)
 flags('GameState_TryStartLevel',debitsEnergyInCandidate=clock<debit<prepare and 'GameState_SpendEnergy_' not in started)
 commit=body('GameState_TryCommitRoundCandidate');write=commit.index('SaveService_TrySave_');adopt=commit.index('___Save =',write);notify=commit.index('___Changed',adopt)
 flags('GameState_TryCommitRoundCandidate',savesBeforeAdoptingCandidate=write<adopt<notify,publishesFailureBeforeChanged=adopt<commit.index('GameState_set_LastFailureResult_')<notify,preservesPrimaryCommitPolicy='PlayerSaveWriteResult_get_PrimaryDurablyCommitted_' in commit[write:adopt] and '___canPersist =' in commit)
 failed=body('GameState_TryRecordLevelFailure')
 flags('GameState_TryRecordLevelFailure',respectsRoundWriteLock=failed.index('___canPersist')<call(failed,'JsonUtility_FromJson_TisPlayerSaveData'))
 ordinary=body('Match3Board_Initialize')
 flags('Match3Board_Initialize',validatesCommittedOrdinaryAttempt=all(t in ordinary for t in ('Nullable_1_get_HasValue_','GameState_GetLevelProgress_','ArgumentException_')) and ordinary.index('GameState_GetLevelProgress_')<ordinary.index('Match3Board_InitializeCore_'),forwardsOrdinaryOwnership='___3_registeredAttempt' in ordinary and 'Match3Board_InitializeCore_' in ordinary)
 core=body('Match3Board_InitializeCore');core=core[core.index('{'):];register=core.index('GameState_RegisterLevelAttempt_');reject=core.index('InvalidOperationException_',register);create=core.index('LevelBoardFactory_Create_',reject)
 flags('Match3Board_InitializeCore',rejectsUncommittedAttempt=register<reject<create,keepsOrdinaryBoosterOwner='___5_preRegisteredOrdinaryAttempt' in core and core.index('___5_preRegisteredOrdinaryAttempt')<core.index('___campaignManaged ='))
 router=body('ScreenRouter_StartLevel');begin=router.index('GameState_TryStartLevel_');gameplay=router.index('ScreenRouter_Show_',begin)
 flags('ScreenRouter_StartLevel',guardsReentrantStart=router.index('___levelStartInProgress')<router.index('GameState_TryEnsureSelectedLevelIsUnlocked_') and '___levelStartInProgress = (bool)0;' in router,checksAtomicStartBeforeGameplay=begin<gameplay and 'ScreenRouter_ShowErrorDialog_' in router[begin:gameplay],forwardsCommittedAttemptOnce=begin<router.index('___pendingGameplayLevel =',begin)<router.index('___pendingGameplayAttempt =',begin)<gameplay and 'GameState_SpendEnergy_' not in router)
 overlay=body('ScreenRouter_BuildGameplayOverlay');clear=overlay.index('___pendingGameplayAttempt = 0;');initialize=overlay.index('Match3Board_Initialize_',clear)
 flags('ScreenRouter_BuildGameplayOverlay',consumesCommittedAttemptBeforeInitialization=overlay.index('___pendingGameplayLevel =')<clear<initialize,forwardsCommittedOrdinaryAttempt='Nullable_1_FromKnownValue' in overlay[:initialize] or 'Nullable_1__ctor_' in overlay[:initialize])
 for owner,keys in policy['flags'].items():
  assert all(record['methods'][owner].get(k) is True for k in keys),(owner,{k:record['methods'][owner].get(k) for k in keys})
 for name in ('TryStartLevel','TryRegisterLevelAttempt','TryRecordLevelFailure','TryCommitRoundCandidate','TryPrepareLevelAttempt','pendingGameplayAttempt','levelStartInProgress'):
  assert (name+'\0').encode() in metadata,name
 record['atomicRoundLifecycleRecoveryPackaged']=True
 record['roundLifecycleOwnerRegionsVerified']=len(policy['extraOwners'])
'''
assert needle in s;s=s.replace(needle,block+'\n'+needle,1);p.write_text(s)
