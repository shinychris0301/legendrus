"""Reconcile guarded build, accumulated compiled fixes, inventory and retained project scope."""
from pathlib import Path
import hashlib,json,xml.etree.ElementTree as ET,zipfile
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
read=lambda n:json.loads((QA/n).read_text())
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
source=json.loads((ROOT/'ProjectDocs/AI/SpendingRecovery/R1/QA/FinalValidation.json').read_text());assert source['unityTotalDistinctPassed']==442
policy=ROOT/'ProjectDocs/AI/SpendingRecovery/R1/QA/Policy';s=json.loads((policy/'summary.json').read_text());cases=ET.parse(policy/'results.xml').findall('.//test-case')
assert s['exitCode']==0 and len(cases)==160 and all(c.get('result')=='Passed' for c in cases)
assert read('Build/summary.json')['exitCode']==0
scope=read('Scope.json');assert scope['baselineFiles']==scope['finalFiles']==scope['unchanged']==7839 and not any(scope[n] for n in ('changed','added','deleted'))
assert all(sha(ROOT/p)==h for p,h in read('BeforeHashes.json').items())
artifact=read('Artifact.json');prior=read('PreviousArtifact.json')
assert prior['bytes']==766014334 and prior['sha256']=='9f72045fe3a2e68114be24d687dfb2a51c07c48d39ec58aa4afbafe9a5744654'
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
assert len(membership['pageDeltas'])==7 and all(r['packedObjectByteDelta']==0 for r in membership['pageDeltas'].values())
assert len(read('CredentialDex.json')['compiledCredentialClasses'])==9
payload=read('PayloadReview.json');assert payload['allFourCredentialJavaSourceMetadataPinsUnchanged'] and read('NativeEngineComparison.json')['binaryIdentical']
assert payload['changedPayloadCount']==len(comparison['changedPayloads'])
native=read('AfterNative.json');assert native['stage']=='After' and len(native['methods'])==22 and native['validatedReceiptAndRecoveryMetadataPresent'] and native['packagedLibraryMatchesGradleInput']
assert native['physicalExecutionVerified'] is False and all(r['packagedMethodBytesMatchCompiledOutput'] for r in native['methods'].values())
for owner in ('SpendGold','SpendGems','SpendEnergy','TryUseBooster'):
 m=native['methods']['GameState_'+owner];assert m['callsAtomicBalanceCommit'] and not m['callsPersist']
for owner in ('ClaimDailyReward','TryDoubleLastLevelGoldReward','TryCompleteLevel','TryCredit','TrySettleBoosterCommand'):assert native['methods']['GameState_'+owner]['generatedOverflowCatch']
assert native['packagedLibrarySha256']==comparison['new']['entries']['lib/arm64-v8a/libil2cpp.so']['sha256']
with zipfile.ZipFile(apk) as z:
 metadata=z.read('assets/bin/Data/Managed/Metadata/global-metadata.dat')
 assert all(n.encode() not in metadata for n in ('SpendingRefreshBuildAudit','LevelSystemRuntimePlayModeTests','GameplayAlignmentPlayModeTests'))
result={'result':'Passed','artifactSha256':artifact['sha256'],'artifactBytes':artifact['bytes'],'freshUnityEditDistinctPassed':221,'freshUnityPlayDistinctPassed':221,'freshUnityTotalDistinctPassed':442,'projectInputsUnchanged':7839,'previousDailyR2ApkAndSidecarPreserved':True,'dailyRewardOverflowRejectionPackaged':True,'doubledGoldAndFirstClearRecoveryPackaged':True,'ordinaryCreditRecoveryPackaged':True,'atomicSpendingAndDurableBoosterRefundRecoveryPackaged':True,'nativeMethodCount':22,'nativeMethodBytesMatchBuildOutput':True,'nativeRecoveryLibrarySha256':native['packagedLibrarySha256'],'existingAll29387PackedRowsUnchanged':True,'sevenNativePagesPackedSizesUnchanged':True,'identicalZipPayloads':comparison['identicalPayloadCount'],'reviewedChangedZipPayloads':len(comparison['changedPayloads']),'normalPersonalJournalRead':False,'physicalGameplayTested':False,'productionReleaseBuilt':False,'gitCommitOrPushPerformed':False}
(QA/'BuildValidation.json').write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result,indent=2))
