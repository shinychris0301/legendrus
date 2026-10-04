"""Correlate generated owner methods and native ELF bytes with the actual packaged library."""
from pathlib import Path
import hashlib,json,re,subprocess,struct,sys,zipfile,io
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';QA.mkdir(exist_ok=True)
stage=sys.argv[1];assert stage in ('Before','After')
source='\n'.join(p.read_text() for p in sorted((ROOT/'Library/Bee/artifacts/Android/il2cppOutput/cpp').glob('LegendsOfRus.Runtime*.cpp')))
targets=['GameState_SpendGold','GameState_SpendGems','GameState_SpendEnergy','GameState_TryUseBooster','GameState_CompleteLevel','GameState_TryCompleteLevel','GameState_TryDoubleLastLevelGoldReward','GameState_TryAddGold','GameState_TryAddGems','GameState_TryAddBooster','GameState_TryCredit','GameState_TryCommitBalance','GameState_TryBeginBoosterCommand','GameState_TrySettleBoosterCommand','GameState_TryRecoverPendingBoosterCommand','GameState_ClaimDailyReward','Match3Board_ConsumeCommandBooster','Match3Board_RefundCommand','Match3Board_RecoverActiveCommand','Match3Board_TryResume','ScreenRouter_StartLevel','ScreenRouter_CommitGameplayVictory','GameState_RefreshEnergyFromClock','GameState_AddEnergy','GameState_RefillEnergy','GameState_EnergyRemaining','GameState_TryRefreshEnergyFromClock','GameState_TryAddEnergy','GameState_TryRefillEnergy','GameState_AdvanceEnergyClock','GameState_CompleteStoryScene','GameState_AcknowledgeChapterCompletion','StoryService_AdvanceDialogue','ScreenRouter_ContinueAfterDialogue','ScreenRouter_ContinueAfterChapterComplete','GameState_CompleteTutorial','LevelTutorialPresenter_Advance','LevelTutorialPresenter_BuildOverlay','ScreenRouter_ShowGameplayTutorialError','ScreenRouter_BuildGameplayOverlay','ScreenRouter_NavigateBack','ScreenRouter_RetireModalRootImmediately','GameState_SelectLevel','GameState_EnsureSelectedLevelIsUnlocked','GameState_TryEnsureSelectedLevelIsUnlocked','GameState_NormalizeLevelProgress','ScreenRouter_OpenSelectedLevel','ProductionChapterMapView_BindNavigation','ScreenRouter_BuildInteractionLayer']
targets += ['GameState_RegisterLevelAttempt', 'GameState_CanRegisterLevelAttempt', 'GameState_TryRegisterLevelAttempt', 'GameState_TryStartLevel', 'GameState_CandidateLevelProgress', 'GameState_TryPrepareLevelAttempt', 'GameState_TryCommitRoundCandidate', 'GameState_RecordLevelFailure', 'GameState_TryRecordLevelFailure', 'GameState_TryFinalizeRetainedFailure', 'Match3Board_Initialize', 'Match3Board_InitializeCore']
targets += ['GameState_TryTogglePreference', 'ProductionSettingsView_TogglePreference', 'ProductionSettingsView_Unbind', 'ProductionSettingsView_InputReady', 'ProductionSettingsView_Listen', 'ProductionSettingsView_RefreshInfo', 'ProductionSettingsView_Bind']
targets += ['GameState_Persist', 'GameState_TryPersist', 'GameState_MigrateSave', 'GameState_ResetProgress']
pattern=re.compile(r'^IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR \S+ ((?:GameState|Match3Board|ScreenRouter|StoryService|LevelTutorialPresenter|ProductionChapterMapView|ProductionSettingsView)_[A-Za-z0-9_]+_m[0-9A-F]+) \([^\n;]*\)\s*\n\{',re.M)
selected={}
for m in pattern.finditer(source):
 owner=m.group(1).rsplit('_m',1)[0]
 if owner not in targets:continue
 if owner=='Match3Board_Initialize' and 'LevelData_' not in m.group(0):continue
 if owner=='GameState_ClaimDailyReward' and 'DailyRewardGrant_' not in m.group(0):continue
 if owner=='GameState_NormalizeLevelProgress' and 'PlayerSaveData_' not in m.group(0):continue
 assert owner not in selected,owner
 opening=source.index('{',m.start());depth=1;end=opening+1
 while depth:
  if source[end]=='{':depth+=1
  elif source[end]=='}':depth-=1
  end+=1
 selected[owner]=(m.group(1),source[m.start():end])
if stage=='After':assert set(selected)==set(targets),(set(targets)-set(selected))
else:assert set(selected)==set(targets)-{'GameState_TryPersist'}

local=ROOT/'Library/Bee/artifacts/Android/d8kzr/libil2cpp.so'
listing=subprocess.check_output(['readelf','--wide','--syms',str(local)],text=True);symbols={}
for line in listing.splitlines():
 cols=line.split()
 if len(cols)>=8 and cols[3]=='FUNC':symbols[cols[-1]]=(int(cols[1],16),int(cols[2]))
def load_segments(stream):
 stream.seek(0);h=stream.read(64);assert h[:6]==b'\x7fELF\x02\x01'
 offset=struct.unpack_from('<Q',h,32)[0];stride,count=struct.unpack_from('<HH',h,54);rows=[]
 for i in range(count):
  stream.seek(offset+i*stride);p=stream.read(stride)
  if struct.unpack_from('<I',p)[0]==1:
   fileoff,virtual,_,size=struct.unpack_from('<QQQQ',p,8);rows.append((fileoff,virtual,size))
 return rows
def method(stream,segments,address,size):
 for offset,virtual,filesize in segments:
  if virtual<=address and address+size<=virtual+filesize:
   stream.seek(offset+address-virtual);b=stream.read(size);assert len(b)==size;return b
 raise AssertionError('Method is not in a file-backed load segment.')
apk=ROOT/'Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk'
with zipfile.ZipFile(apk) as z:packed=z.read('lib/arm64-v8a/libil2cpp.so');metadata=z.read('assets/bin/Data/Managed/Metadata/global-metadata.dat')
gradle=ROOT/'Library/Bee/Android/Prj/IL2CPP/Gradle/unityLibrary/src/main/jniLibs/arm64-v8a/libil2cpp.so'
with gradle.open('rb') as f:assert hashlib.file_digest(f,'sha256').hexdigest()==hashlib.sha256(packed).hexdigest()
record={'stage':stage,'methods':{},'packagedLibrarySha256':hashlib.sha256(packed).hexdigest(),'packagedLibraryMatchesGradleInput':True,'physicalExecutionVerified':False}
pstream=io.BytesIO(packed);psegments=load_segments(pstream)
with local.open('rb') as f:
 segments=load_segments(f)
 for owner,(symbol,body) in selected.items():
  assert symbol in symbols,symbol
  address,size=symbols[symbol];assert size>0
  native=method(f,segments,address,size);assert native==method(pstream,psegments,address,size)
  out=BASE/'Sources'/f'{stage}{owner}.cpp';assert not out.exists() or out.read_text()==body+'\n';out.write_text(body+'\n')
  record['methods'][owner]={'symbol':symbol,'generatedMethodSha256':hashlib.sha256(body.encode()).hexdigest(),'nativeMethodBytes':size,'nativeMethodSha256':hashlib.sha256(native).hexdigest(),'packagedMethodBytesMatchCompiledOutput':True,'generatedOverflowCatch':'catch(Il2CppExceptionWrapper&' in body and 'OverflowException_' in body,'rejectsBeforeClearingFinalSequence':owner=='StoryService_AdvanceDialogue' and 'return (int32_t)(5);' in body and body.index('return (int32_t)(5);')<body.index('->___activeSequence ='),'callsStoryCommit':'GameState_CompleteStoryScene_' in body,'callsErrorDialog':'ScreenRouter_ShowErrorDialog_' in body,'callsChapterAcknowledgement':'StoryService_AcknowledgeChapterComplete_' in body,'callsPersist':'GameState_Persist_' in body,'callsAtomicBalanceCommit':'GameState_TryCommitBalance_' in body,'callsSharedEnergyClock':'GameState_AdvanceEnergyClock_' in body,'hasWideCapMath':'int64_t' in body and 'Math_Min_' in body,'callsTryEnergyApi':any(n in body for n in ('GameState_TryAddEnergy_','GameState_TryRefillEnergy_','GameState_TryRefreshEnergyFromClock_'))}
if stage=='After':
 for owner in ('GameState_SpendGold','GameState_SpendGems','GameState_SpendEnergy','GameState_TryUseBooster','GameState_TryAddEnergy','GameState_TryRefillEnergy','GameState_TryRefreshEnergyFromClock'):
  assert record['methods'][owner]['callsAtomicBalanceCommit'] and not record['methods'][owner]['callsPersist']
 for owner in ('GameState_ClaimDailyReward','GameState_TryDoubleLastLevelGoldReward','GameState_TryCompleteLevel','GameState_TryCredit','GameState_TrySettleBoosterCommand'):
  assert record['methods'][owner]['generatedOverflowCatch'],owner
 for name in ('BoosterCommandReceipt','boosterCommand','TryBeginBoosterCommand','TrySettleBoosterCommand','ShowGameplayInventoryError'):
  assert (name+'\0').encode() in metadata,name
 record['validatedReceiptAndRecoveryMetadataPresent']=True
 before=json.loads((QA/'BeforeNative.json').read_text())
 changed=[owner for owner in before['methods'] if before['methods'][owner]['nativeMethodSha256']!=record['methods'][owner]['nativeMethodSha256']]
 assert {'GameState_Persist'}<=set(changed)
 for owner in ('GameState_SpendEnergy','GameState_TryRefreshEnergyFromClock'):
  assert record['methods'][owner]['callsSharedEnergyClock']
 for owner in ('GameState_TryAddEnergy','GameState_AdvanceEnergyClock'):
  assert record['methods'][owner]['hasWideCapMath']
 for owner in ('GameState_AddEnergy','GameState_RefillEnergy','GameState_RefreshEnergyFromClock'):
  assert record['methods'][owner]['callsTryEnergyApi'] and not record['methods'][owner]['callsPersist']
 for name in ('TryAddEnergy','TryRefillEnergy','TryRefreshEnergyFromClock','AdvanceEnergyClock'):
  assert (name+'\0').encode() in metadata,name
 record['cappedAtomicEnergyAndClockRecoveryPackaged']=True
 for owner in ('GameState_CompleteStoryScene','GameState_AcknowledgeChapterCompletion'):
  assert record['methods'][owner]['callsAtomicBalanceCommit'] and not record['methods'][owner]['callsPersist']
 assert record['methods']['StoryService_AdvanceDialogue']['callsStoryCommit']
 assert record['methods']['StoryService_AdvanceDialogue']['rejectsBeforeClearingFinalSequence']
 for owner in ('ScreenRouter_ContinueAfterDialogue','ScreenRouter_ContinueAfterChapterComplete'):
  assert record['methods'][owner]['callsErrorDialog']
 assert record['methods']['ScreenRouter_ContinueAfterChapterComplete']['callsChapterAcknowledgement']
 assert b'SaveRejected\0' in metadata
 record['atomicNarrativeAcknowledgementRecoveryPackaged']=True
 advance=selected['LevelTutorialPresenter_Advance'][1]
 reject=advance.index('___AcknowledgementRejected')
 clear=advance.index('LevelTutorialPresenter_set_CurrentStep_')
 record['methods']['LevelTutorialPresenter_Advance']['rejectsBeforeClearingTutorialStep']=reject<advance.index('return;',reject)<clear
 record['methods']['LevelTutorialPresenter_Advance']['guardsInactiveTutorialPresenter']=all(t in advance[:reject] for t in ('Object_op_Equality_','Behaviour_get_isActiveAndEnabled_','LevelTutorialPresenter_get_IsPresenting_'))
 overlay=selected['LevelTutorialPresenter_BuildOverlay'][1]
 record['methods']['LevelTutorialPresenter_BuildOverlay']['capturesPresentedOverlayAndStep']=all(t in overlay for t in ('___step =','___presentedOverlay =','UnityEvent_AddListener_'))
 modal=selected['ScreenRouter_ShowGameplayTutorialError'][1]
 record['methods']['ScreenRouter_ShowGameplayTutorialError']['scopedTutorialErrorOwner']=all(t in modal for t in ('___activeBoard','Match3Board_get_Phase_','___contentRoot','GameObject_GetComponentInChildren_TisLevelTutorialPresenter','___modalMotionState','ScreenRouter_get_IsScreenTransitioning_','ScreenRouter_ShowModal_','___tutorialErrorOwner =','ScreenRouter_CloseModal_','ProductionErrorDialogView_Bind_')) and '(Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07*)NULL' in modal
 back=selected['ScreenRouter_NavigateBack'][1]
 record['methods']['ScreenRouter_NavigateBack']['tutorialBackOwnerPriority']=back.index('___tutorialErrorOwner')<back.index('ScreenRouter_IsGameplayInventoryError_') and 'ScreenRouter_CloseModal_' in back
 retire=selected['ScreenRouter_RetireModalRootImmediately'][1]
 record['methods']['ScreenRouter_RetireModalRootImmediately']['clearsTutorialErrorOwner']=retire.index('___tutorialErrorOwner =')<retire.index('___recapEntries =') and re.search(r'___tutorialErrorOwner = [^;]*NULL;',retire) is not None
 record['methods']['ScreenRouter_BuildGameplayOverlay']['registersAcknowledgementRejected']='LevelTutorialPresenter_add_AcknowledgementRejected_' in selected['ScreenRouter_BuildGameplayOverlay'][1]
 tutorial_flags={'LevelTutorialPresenter_Advance':['rejectsBeforeClearingTutorialStep','guardsInactiveTutorialPresenter'],'LevelTutorialPresenter_BuildOverlay':['capturesPresentedOverlayAndStep'],'ScreenRouter_ShowGameplayTutorialError':['scopedTutorialErrorOwner'],'ScreenRouter_NavigateBack':['tutorialBackOwnerPriority'],'ScreenRouter_RetireModalRootImmediately':['clearsTutorialErrorOwner'],'ScreenRouter_BuildGameplayOverlay':['registersAcknowledgementRejected']}
 assert all(record['methods'][o][k] for o,keys in tutorial_flags.items() for k in keys)
 assert record['methods']['GameState_CompleteTutorial']['callsAtomicBalanceCommit'] and not record['methods']['GameState_CompleteTutorial']['callsPersist']
 assert all((n+'\0').encode() in metadata for n in ('ShowGameplayTutorialError','AcknowledgementRejected','tutorialErrorOwner'))
 callbacks={};selection_callbacks={}
 all_methods=re.compile(r'^IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR \S+ ([A-Za-z0-9_]+_m[0-9A-F]+) \([^\n;]*\)\s*\n\{',re.M)
 for m in all_methods.finditer(source):
  opening=source.index('{',m.start());depth=1;end=opening+1
  while depth:
   if source[end]=='{':depth+=1
   elif source[end]=='}':depth-=1
   end+=1
  body=source[m.start():end];label=None
  if all(t in body for t in ('___presentedOverlay','___step','LevelTutorialPresenter_get_CurrentStep_','LevelTutorialPresenter_Advance_')):label='OwnedTutorialContinue'
  if all(t in body for t in ('ScreenRouter_ShowGameplayTutorialError_','___board')) and m.group(1).startswith('U3CU3Ec__DisplayClass'):label='OwnedTutorialError'
  if all(t in body for t in ('___openSelectedLevel','Func_1_Invoke_','___committed =')) and 'ProductionChapterMapView_' in body:
   label='AcknowledgedMapNode' if 'GameState_SelectLevel_' in body else 'AcknowledgedMapPlay'
  if m.group(1).startswith('ScreenRouter_U3CBuildInteractionLayer') and 'ScreenRouter_ShowErrorDialog_' in body:label='SelectionError'
  if label is None:continue
  inventory=selection_callbacks if label in ('AcknowledgedMapNode','AcknowledgedMapPlay','SelectionError') else callbacks
  assert label not in inventory,label
  address,size=symbols[m.group(1)]
  with local.open('rb') as f:native=method(f,load_segments(f),address,size)
  assert size>0 and native==method(pstream,psegments,address,size)
  if label=='AcknowledgedMapNode':
   start=body.index('GameState_SelectLevel_');error=body.index('___selectionRejected',start);opened=body.index('Func_1_Invoke_',error)
   assert error<body.index('return;',error)<opened and body.index('___committed =')>opened
  if label=='AcknowledgedMapPlay':assert body.index('Func_1_Invoke_')<body.index('___committed =')
  if label=='SelectionError':assert m.group(1) in selected['ScreenRouter_BuildInteractionLayer'][1] and '*)NULL, NULL);' in body
  if label=='OwnedTutorialContinue':assert 'Object_op_Equality_' in body and body.index('___presentedOverlay')<body.index('LevelTutorialPresenter_Advance_')
  out=BASE/'Sources'/('After'+label+'.cpp');assert not out.exists() or out.read_text()==body+'\n';out.write_text(body+'\n')
  inventory[label]={'symbol':m.group(1),'generatedMethodSha256':hashlib.sha256(body.encode()).hexdigest(),'nativeMethodBytes':size,'nativeMethodSha256':hashlib.sha256(native).hexdigest(),'packagedMethodBytesMatchCompiledOutput':True}
 assert set(callbacks)=={'OwnedTutorialContinue','OwnedTutorialError'},set(callbacks)
 record['tutorialCallbacks']=callbacks
 record['methods']['LevelTutorialPresenter_BuildOverlay']['ownedContinueCallbackCompiledAndPackaged']=True
 record['methods']['ScreenRouter_BuildGameplayOverlay']['ownedErrorCallbackCompiledAndPackaged']=True
 assert set(selection_callbacks)=={'AcknowledgedMapNode','AcknowledgedMapPlay','SelectionError'}
 record['selectionCallbacks']=selection_callbacks
 for owner in ('GameState_SelectLevel','GameState_TryEnsureSelectedLevelIsUnlocked'):
  body=selected[owner][1]
  record['methods'][owner]['respectsSelectionWriteLock']=body.index('___canPersist')<body.index('return (bool)0;')<body.rindex('JsonUtility_FromJson_TisPlayerSaveData')
  assert record['methods'][owner]['callsAtomicBalanceCommit'] and not record['methods'][owner]['callsPersist'] and record['methods'][owner]['respectsSelectionWriteLock']
 record['methods']['GameState_TryEnsureSelectedLevelIsUnlocked']['normalizesCandidateBeforeCommit']=selected['GameState_TryEnsureSelectedLevelIsUnlocked'][1].index('GameState_NormalizeLevelProgress_')<selected['GameState_TryEnsureSelectedLevelIsUnlocked'][1].index('GameState_TryCommitBalance_')
 wrapper=selected['GameState_EnsureSelectedLevelIsUnlocked'][1]
 record['methods']['GameState_EnsureSelectedLevelIsUnlocked']['delegatesToAcknowledgedNormalization']='GameState_TryEnsureSelectedLevelIsUnlocked_' in wrapper and 'GameState_Persist_' not in wrapper
 normalizer=selected['GameState_NormalizeLevelProgress'][1]
 record['methods']['GameState_NormalizeLevelProgress']['normalizesExplicitCandidate']='___0_save' in normalizer and '__this' not in normalizer and 'GameState_CalculateTotalStars_' in normalizer
 start=selected['ScreenRouter_StartLevel'][1];first=start.index('GameState_TryEnsureSelectedLevelIsUnlocked_');spend=start.index('GameState_TryStartLevel_',first)
 record['methods']['ScreenRouter_StartLevel']['normalizationCheckedBeforeEnergySpend']=first<spend and 'ScreenRouter_ShowErrorDialog_' in start[first:spend] and 'if (' in start[first:spend]
 opened=selected['ScreenRouter_OpenSelectedLevel'][1];first=opened.index('GameState_TryEnsureSelectedLevelIsUnlocked_');story=opened.index('StoryService_TryBeginChapterIntro_',first)
 record['methods']['ScreenRouter_OpenSelectedLevel']['normalizationCheckedBeforeStoryRouting']=first<opened.index('return (bool)0;',first)<story and 'ScreenRouter_ShowErrorDialog_' in opened[first:story]
 record['methods']['ProductionChapterMapView_BindNavigation']['acknowledgedMapCallbacksCompiledAndPackaged']=all(r['packagedMethodBytesMatchCompiledOutput'] for r in selection_callbacks.values()) and 'Func_1_' in selected['ProductionChapterMapView_BindNavigation'][1]
 binding=selected['ScreenRouter_BuildInteractionLayer'][1]
 record['methods']['ScreenRouter_BuildInteractionLayer']['bindsAcknowledgedMapOpenAndError']=all(t in binding for t in ('Func_1_','ScreenRouter_OpenSelectedLevel_','ProductionChapterMapView_BindNavigation_')) and selection_callbacks['SelectionError']['symbol'] in binding
 selection_flags={'GameState_SelectLevel':('respectsSelectionWriteLock',),'GameState_TryEnsureSelectedLevelIsUnlocked':('respectsSelectionWriteLock','normalizesCandidateBeforeCommit'),'GameState_EnsureSelectedLevelIsUnlocked':('delegatesToAcknowledgedNormalization',),'GameState_NormalizeLevelProgress':('normalizesExplicitCandidate',),'ScreenRouter_StartLevel':('normalizationCheckedBeforeEnergySpend',),'ScreenRouter_OpenSelectedLevel':('normalizationCheckedBeforeStoryRouting',),'ProductionChapterMapView_BindNavigation':('acknowledgedMapCallbacksCompiledAndPackaged',),'ScreenRouter_BuildInteractionLayer':('bindsAcknowledgedMapOpenAndError',)}
 assert all(record['methods'][owner][key] is True for owner,keys in selection_flags.items() for key in keys)
 assert b'TryEnsureSelectedLevelIsUnlocked\0' in metadata
 record['atomicSelectionNormalizationRecoveryPackaged']=True
 record['atomicTutorialAcknowledgementRecoveryPackaged']=True
 record['changedNativeMethodsFromBaseline']=changed

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


if stage=='After':
 policy=json.loads((BASE/'Sources/SettingsNativePolicy.json').read_text())
 pref=body('GameState_TryTogglePreference');clone=call(pref,'JsonUtility_FromJson_TisPlayerSaveData')
 flags('GameState_TryTogglePreference',validatesPreferenceBeforeClone=pref.index('ArgumentOutOfRangeException_')<clone,respectsPreferenceWriteLock=pref.index('___canPersist')<clone,changesAllFourPreferenceCandidates=all('___'+n+' =' in pref for n in ('music','sfx','vibration','notifications')))
 ui=body('ProductionSettingsView_TogglePreference');commit=call(ui,'GameState_TryTogglePreference');refresh=call(ui,'ProductionSettingsView_RefreshState');info=call(ui,'ProductionSettingsView_ShowInfo')
 flags('ProductionSettingsView_TogglePreference',guardsPreferenceReentry='ProductionSettingsView_CanAct_' in ui[:commit] and ui.index('___preferenceChangeInProgress = (bool)1;')<commit and ui.index('il2cpp::utils::Finally(')<ui.index('___preferenceChangeInProgress = (bool)0;')<ui.index('\n\t\ttry\n')<commit<refresh,refreshesAfterAcknowledgement=commit<refresh<info,rejectionUsesExistingInfoPanel='7, NULL);' in ui[info:] and 'ScreenRouter_Show' not in ui)
 unbind=body('ProductionSettingsView_Unbind')
 flags('ProductionSettingsView_Unbind',retiresBindingBeforeRemovingListeners=unbind.index('___bindingOwner =')<call(unbind,'UnityEvent_RemoveListener'))
 ready=body('ProductionSettingsView_InputReady')
 flags('ProductionSettingsView_InputReady',blocksPreferenceCommitReentry='___preferenceChangeInProgress' in ready,preservesModalTransitionGuard=all(t in ready for t in ('ScreenRouter_get_ActiveModal_','ScreenRouter_get_IsModalTransitioning_','ScreenRouter_get_HasActiveModalBlocker_','ScreenRouter_get_IsScreenTransitioning_')))
 listening=body('ProductionSettingsView_Listen')
 flags('ProductionSettingsView_Listen',capturesBindingOwner=listening.index('___bindingOwner')<listening.index('___owner ='))
 binding=body('ProductionSettingsView_Bind')
 flags('ProductionSettingsView_Bind',retiresPreviousBindingsBeforeRebinding=call(binding,'ProductionSettingsView_Unbind')<call(binding,'ProductionSettingsView_Listen'))
 settings_callbacks={}
 for m in all_methods.finditer(source):
  opening=source.index('{',m.start());depth=1;end=opening+1
  while depth:
   if source[end]=='{':depth+=1
   elif source[end]=='}':depth-=1
   end+=1
  cb=source[m.start():end]
  if not all(t in cb for t in ('___owner','___bindingOwner','ProductionSettingsView_InputReady_','___overlay','Action_Invoke_')):continue
  label='OwnedSettingsListener';assert not settings_callbacks
  address,size=symbols[m.group(1)]
  with local.open('rb') as f:native=method(f,load_segments(f),address,size)
  assert size>0 and native==method(pstream,psegments,address,size)
  assert cb.index('___bindingOwner')<call(cb,'ProductionSettingsView_InputReady')<call(cb,'Action_Invoke')
  assert all(t in cb for t in ('___button','VirtualFuncInvoker0< bool >::Invoke(24,','___InfoPanel','GameObject_get_activeSelf_'))
  assert cb.index('___button')<cb.index('VirtualFuncInvoker0< bool >::Invoke(24,')<cb.index('___InfoPanel')<call(cb,'Action_Invoke')
  assert m.group(1) in listening
  (BASE/'Sources'/('After'+label+'.cpp')).write_text(cb+'\n')
  settings_callbacks[label]={'symbol':m.group(1),'nativeMethodBytes':size,'nativeMethodSha256':hashlib.sha256(native).hexdigest(),'generatedMethodSha256':hashlib.sha256(cb.encode()).hexdigest(),'packagedMethodBytesMatchCompiledOutput':True,'rejectsRetiredBindingBeforeInputAndAction':True,'checksButtonAndInfoShieldBeforeAction':True}
 assert set(settings_callbacks)=={'OwnedSettingsListener'}
 record['settingsCallbacks']=settings_callbacks
 flags('ProductionSettingsView_Listen',ownedSettingsCallbackCompiledAndPackaged=True)
 info=body('ProductionSettingsView_RefreshInfo')
 flags('ProductionSettingsView_RefreshInfo',existingLocalizedErrorBranchPackaged='___infoKind' in info and '7)' in info and all(t in info for t in ('___LanguageChoices','___GeneralInfoContent','___InfoHeading','___InfoDescription','___CloseInfoLabel')))
 # Decode actual Unity metadata strings; do not accept method/type names alone as content proof.
 offset,size,count,literal_offset,literal_bytes,literal_count=struct.unpack_from('<6I',metadata,8);indices=struct.unpack_from('<'+str(count)+'I',metadata,offset)
 assert indices[0]==0 and indices[-1]==literal_bytes
 literals={metadata[literal_offset+a:literal_offset+b].decode('utf-8',errors='replace') for a,b in zip(indices,indices[1:])}
 assert all(t in literals for t in ('AN ERROR OCCURRED','ПРОИЗОШЛА ОШИБКА','Action could not be completed.\nClose this message and try again.','Действие не выполнено.\nЗакройте окно и попробуйте снова.'))
 assert all((n+'\0').encode() in metadata for n in ('TryTogglePreference','bindingOwner','preferenceChangeInProgress'))
 for owner,keys in policy['flags'].items():assert all(record['methods'][owner].get(k) is True for k in keys),(owner,record['methods'][owner])
 assert not record['methods']['GameState_TryTogglePreference']['callsPersist']
 record['atomicSettingsPreferenceRecoveryPackaged']=True
 record['settingsOwnerRegionsVerified']=len(policy['extraOwners'])


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

(QA/(stage+'Native.json')).write_text(json.dumps(record,indent=2)+'\n')
print(stage+': '+str(len(selected))+' generated/native methods match the packaged library.')
