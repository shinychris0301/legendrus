"""Reconcile refreshed compiled selection code, guarded build, retained artifact/assets and source scope."""
from pathlib import Path
import hashlib,json,xml.etree.ElementTree as ET,zipfile
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';read=lambda n:json.loads((QA/n).read_text())
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
source=json.loads((ROOT/'ProjectDocs/AI/PersistenceRecovery/R1/QA/FinalValidation.json').read_text());assert source['unityTotalDistinctPassed']==426
subprocess_source_patch=ROOT/'ProjectDocs/AI/PersistenceRecovery/R1/Sources/TaskChanges.patch'
import subprocess
subprocess.run(['git','apply','--reverse','--check',str(subprocess_source_patch)],cwd=ROOT,check=True,capture_output=True)
s=read('Policy/summary.json');cases=ET.parse(QA/'Policy/results.xml').findall('.//test-case');assert s['exitCode']==0 and len(cases)==len({c.get('fullname') for c in cases})==160 and all(c.get('result')=='Passed' for c in cases)
assert read('Build/summary.json')['exitCode']==0
scope=read('Scope.json');assert scope['baselineFiles']==scope['finalFiles']==scope['unchanged']==7839 and not any(scope[n] for n in ('changed','added','deleted'))
assert all(sha(ROOT/p)==h for p,h in read('BeforeHashes.json').items())
artifact=read('Artifact.json');prior=read('PreviousArtifact.json');assert prior['bytes']==766032162 and prior['sha256']=='0310f6a446f268fb36a70c3a7af604cc89b54c5cbc51f19a864316aeaf421adc'
for r in (artifact,prior):assert sha(ROOT/r['path'])==r['sha256'] and (ROOT/r['path']).stat().st_size==r['bytes']
apk=ROOT/artifact['path'];assert apk.with_suffix('.apk.sha256').read_text().strip()==artifact['sha256']+'  '+apk.name
assert Path(str(ROOT/prior['path'])+'.sha256.inherited').read_text().strip()==prior['sha256']+'  '+apk.name
inspection=read('ApkVerification.json');assert all(inspection[k]==artifact[k] for k in ('bytes','sha256'))
assert all(inspection[k] is True for k in ('signatureVerified','sameCertificateAsPriorLocalApk','zipCrcPassed','zip16KiBAlignmentPassed','editorAndTestHelpersAbsentFromPlayerMetadata'))
assert inspection['package']=='com.example.legendsofrus.v117debug' and inspection['debuggable'] and len(inspection['elfLibraries'])==6 and all(inspection['presetSelectors'].values())
comparison=read('ArtifactComparison.json');assert not comparison['addedEntries'] and not comparison['removedEntries'] and comparison['permissionsUnchanged']
assert all(r['exactAccounting'] and r['unindexedBytes']==0 for r in (comparison['old'],comparison['new']))
packed=read('PackedAssets.json');membership=read('PackedComparison.json');assert packed['result']=='Succeeded' and packed['unityVersion']=='6000.5.9f1'
assert membership['beforeRows']==membership['afterRows']==29387 and membership['allExistingPathGuidTypeMembershipUnchanged']
assert len(membership['pageDeltas'])==8 and all(r['packedObjectByteDelta']==0 for r in membership['pageDeltas'].values())
assert len(read('CredentialDex.json')['compiledCredentialClasses'])==9
payload=read('PayloadReview.json');assert payload['allFourCredentialJavaSourceMetadataPinsUnchanged'] and read('NativeEngineComparison.json')['binaryIdentical'] and payload['changedPayloadCount']==len(comparison['changedPayloads'])
native=read('AfterNative.json');assert native['stage']=='After' and len(native['methods'])==72 and native['validatedReceiptAndRecoveryMetadataPresent'] and native['cappedAtomicEnergyAndClockRecoveryPackaged'] and native['packagedLibraryMatchesGradleInput']
assert native['physicalExecutionVerified'] is False and all(r['packagedMethodBytesMatchCompiledOutput'] for r in native['methods'].values())
for owner in ('SpendGold','SpendGems','SpendEnergy','TryUseBooster','TryAddEnergy','TryRefillEnergy','TryRefreshEnergyFromClock','CompleteStoryScene','AcknowledgeChapterCompletion'):
 m=native['methods']['GameState_'+owner];assert m['callsAtomicBalanceCommit'] and not m['callsPersist']
for owner in ('ClaimDailyReward','TryDoubleLastLevelGoldReward','TryCompleteLevel','TryCredit','TrySettleBoosterCommand'):assert native['methods']['GameState_'+owner]['generatedOverflowCatch']
for owner in ('TryAddEnergy','AdvanceEnergyClock'):assert native['methods']['GameState_'+owner]['hasWideCapMath']
for owner in ('SpendEnergy','TryRefreshEnergyFromClock'):assert native['methods']['GameState_'+owner]['callsSharedEnergyClock']
for owner in ('AddEnergy','RefillEnergy','RefreshEnergyFromClock'):assert native['methods']['GameState_'+owner]['callsTryEnergyApi'] and not native['methods']['GameState_'+owner]['callsPersist']
assert native['atomicNarrativeAcknowledgementRecoveryPackaged'] and native['methods']['StoryService_AdvanceDialogue']['rejectsBeforeClearingFinalSequence']
assert all(native['methods'][n]['callsErrorDialog'] for n in ('ScreenRouter_ContinueAfterDialogue','ScreenRouter_ContinueAfterChapterComplete'))
assert native['packagedLibrarySha256']==comparison['new']['entries']['lib/arm64-v8a/libil2cpp.so']['sha256']
assert native['atomicTutorialAcknowledgementRecoveryPackaged']
assert set(native['tutorialCallbacks'])=={'OwnedTutorialContinue','OwnedTutorialError'}
assert all(r['packagedMethodBytesMatchCompiledOutput'] for r in native['tutorialCallbacks'].values())
flags={'LevelTutorialPresenter_Advance':('rejectsBeforeClearingTutorialStep','guardsInactiveTutorialPresenter'),'LevelTutorialPresenter_BuildOverlay':('capturesPresentedOverlayAndStep','ownedContinueCallbackCompiledAndPackaged'),'ScreenRouter_ShowGameplayTutorialError':('scopedTutorialErrorOwner',),'ScreenRouter_BuildGameplayOverlay':('registersAcknowledgementRejected','ownedErrorCallbackCompiledAndPackaged'),'ScreenRouter_NavigateBack':('tutorialBackOwnerPriority',),'ScreenRouter_RetireModalRootImmediately':('clearsTutorialErrorOwner',)}
assert all(native['methods'][o][f] is True for o,keys in flags.items() for f in keys)
assert len(read('BeforeNative.json')['methods'])==71
assert native['methods']['GameState_CompleteTutorial']['callsAtomicBalanceCommit'] and not native['methods']['GameState_CompleteTutorial']['callsPersist']
assert native['atomicSelectionNormalizationRecoveryPackaged']
selection_flags={'GameState_SelectLevel':('respectsSelectionWriteLock',),'GameState_TryEnsureSelectedLevelIsUnlocked':('respectsSelectionWriteLock','normalizesCandidateBeforeCommit'),'GameState_EnsureSelectedLevelIsUnlocked':('delegatesToAcknowledgedNormalization',),'GameState_NormalizeLevelProgress':('normalizesExplicitCandidate',),'ScreenRouter_StartLevel':('normalizationCheckedBeforeEnergySpend',),'ScreenRouter_OpenSelectedLevel':('normalizationCheckedBeforeStoryRouting',),'ProductionChapterMapView_BindNavigation':('acknowledgedMapCallbacksCompiledAndPackaged',),'ScreenRouter_BuildInteractionLayer':('bindsAcknowledgedMapOpenAndError',)}
assert all(native['methods'][owner][flag] is True for owner,flags in selection_flags.items() for flag in flags)
assert set(native['selectionCallbacks'])=={'AcknowledgedMapPlay','AcknowledgedMapNode','SelectionError'}
assert all(row['packagedMethodBytesMatchCompiledOutput'] for row in native['selectionCallbacks'].values())
for owner in ('GameState_SelectLevel','GameState_TryEnsureSelectedLevelIsUnlocked'):
 assert native['methods'][owner]['callsAtomicBalanceCommit'] and not native['methods'][owner]['callsPersist']
policy=json.loads((BASE/'Sources/RoundNativePolicy.json').read_text())
assert native['atomicRoundLifecycleRecoveryPackaged'] and native['roundLifecycleOwnerRegionsVerified']==12
assert all(native['methods'][o]['callsAtomicRoundCommit'] and not native['methods'][o]['callsPersist'] for o in policy['atomicApis'])
assert all(native['methods'][o][flag] is True for o,keys in policy['flags'].items() for flag in keys)
settings=json.loads((BASE/'Sources/SettingsNativePolicy.json').read_text());assert native['atomicSettingsPreferenceRecoveryPackaged'] and native['settingsOwnerRegionsVerified']==7
assert all(native['methods'][o][flag] is True for o,keys in settings['flags'].items() for flag in keys)
assert set(native['settingsCallbacks'])=={'OwnedSettingsListener'} and all(r['packagedMethodBytesMatchCompiledOutput'] for r in native['settingsCallbacks'].values())
assert not native['methods']['GameState_TryTogglePreference']['callsPersist']
policy=json.loads((BASE/'Sources/PersistenceNativePolicy.json').read_text()); assert native['acknowledgedOrdinaryPersistenceRecoveryPackaged'] and native['persistenceOwnerRegionsVerified']==4
assert all(native['methods'][o][flag] is True for o,keys in policy['flags'].items() for flag in keys)
assert read('BeforeNative.json')['ordinaryPersistenceFixAbsentFromPriorApk'] is True
with zipfile.ZipFile(apk) as z:
 metadata=z.read('assets/bin/Data/Managed/Metadata/global-metadata.dat');assert all(n.encode() not in metadata for n in ('PersistenceRefreshBuildAudit','LevelSystemRuntimePlayModeTests','GameplayAlignmentPlayModeTests'))
result={'result':'Passed','artifactSha256':artifact['sha256'],'artifactBytes':artifact['bytes'],'freshUnityEditDistinctPassed':160,'sourceStageUnityPlayDistinctPassed':341,'sourceStageUnityEditDistinctPassed':85,'sourceStageUnityTotalDistinctPassed':426,'newPlayModeRun':False,'projectInputsUnchanged':7839,'previousSettingsR2ApkAndSidecarPreserved':True,'acknowledgedOrdinaryPersistenceRecoveryPackaged':True,'persistenceOwnerRegionsVerified':4,'atomicSettingsPreferenceRecoveryPackaged':True,'settingsOwnerRegionsVerified':7,'settingsCallbackRegionsVerified':1,'atomicRoundLifecycleRecoveryPackaged':True,'roundLifecycleOwnerRegionsVerified':12,'atomicSelectionNormalizationRecoveryPackaged':True,'selectionCallbackRegionsVerified':3,'atomicTutorialAcknowledgementRecoveryPackaged':True,'atomicNarrativeAcknowledgementRecoveryPackaged':True,'cappedAtomicEnergyAndClockRecoveryPackaged':True,'dailyRewardOverflowRejectionPackaged':True,'doubledGoldAndFirstClearRecoveryPackaged':True,'ordinaryCreditRecoveryPackaged':True,'atomicSpendingAndDurableBoosterRefundRecoveryPackaged':True,'tutorialCallbackRegionsVerified':2,'nativeMethodCount':72,'beforeNativeMethodCount':71,'nativeMethodBytesMatchBuildOutput':True,'nativeRecoveryLibrarySha256':native['packagedLibrarySha256'],'existingAll29387PackedRowsUnchanged':True,'eightNativePagesPackedSizesUnchanged':True,'identicalZipPayloads':comparison['identicalPayloadCount'],'reviewedChangedZipPayloads':len(comparison['changedPayloads']),'normalPersonalJournalRead':False,'physicalGameplayTested':False,'productionReleaseBuilt':False,'gitCommitOrPushPerformed':False}
(QA/'BuildValidation.json').write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result,indent=2))
