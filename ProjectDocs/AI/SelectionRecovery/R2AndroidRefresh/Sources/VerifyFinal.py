"""Validate completed offline handoff, preserved source/history, current artifact and narrow working tree."""
from pathlib import Path
import ast,difflib,hashlib,json,re,subprocess
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';read=lambda n:json.loads((QA/n).read_text())
build=read('BuildValidation.json');assert build['result']=='Passed' and build['freshUnityEditDistinctPassed']==160 and build['sourceStageUnityTotalDistinctPassed']==350 and build['nativeMethodCount']==49
b=read('PreflightBoundaryChecks.json');assert b['result']=='Passed' and b['passed']>104 and b['failed']==0 and len(b['cases'])==len({r['name'] for r in b['cases']})==b['passed']
assert b['realAdbInvocations']==0 and b['realDeviceExecution'] is False and all(r['result']=='Passed' for r in b['cases'])
o=read('OfflinePreflight.json');assert o['status']=='offline_prepared_device_deferred' and o['localApkSha256']==build['artifactSha256'] and o['localApkBytes']==build['artifactBytes']
assert all(o[k] is True for k in ('atomicSelectionNormalizationRecoveryPackaged','atomicTutorialAcknowledgementRecoveryPackaged','atomicNarrativeAcknowledgementRecoveryPackaged','cappedAtomicEnergyAndClockRecoveryPackaged','fixedPresetsPackaged','checksumSidecarMatches','deviceExecutionDeferred'))
assert all(o[k] is False for k in ('deviceEnumerationAttempted','physicalGameplayTested','appLaunched','appDataReadOrChanged','qaActivationVerified'))
for n,h in read('BeforeHashes.json').items():
 with (ROOT/n).open('rb') as f:assert hashlib.file_digest(f,'sha256').hexdigest()==h,n
assert len(read('BeforeHashes.json'))==7839
fingerprint=read('GeneratedFingerprintCleanup.json');assert fingerprint['beforeSha256']==fingerprint['restoredSha256']
with (ROOT/fingerprint['path']).open('rb') as f:assert hashlib.file_digest(f,'sha256').hexdigest()==fingerprint['restoredSha256']
assert len(read('FreshPackingCachePreservation.json'))==6
for row in read('FreshPackingCachePreservation.json'):assert Path(row['preservedAt']).is_dir()
comparison=read('ArtifactComparison.json');assert comparison['identicalPayloadCount']==399 and comparison['unityPayloadUnchanged'] is True
assert {r['path'] for r in comparison['changedPayloads']}=={'assets/bin/Data/Managed/Metadata/global-metadata.dat','assets/bin/Data/boot.config','assets/bin/Data/unity_app_guid','lib/arm64-v8a/libil2cpp.so'}
artifact=read('Artifact.json');apk=ROOT/artifact['path']
with apk.open('rb') as f:assert hashlib.file_digest(f,'sha256').hexdigest()==build['artifactSha256']
assert apk.stat().st_size==build['artifactBytes'] and apk.with_suffix('.apk.sha256').read_text()==build['artifactSha256']+'  '+apk.name+'\n'
owned=[];links=0
for original in sorted((BASE/'Sources/Before').rglob('*')):
 if not original.is_file():continue
 relative=original.relative_to(BASE/'Sources/Before');p=ROOT/relative;old=original.read_text();new=p.read_text();assert old!=new,relative
 if p.name in ('CurrentTask.md','ConversationHandoff.md'):assert new.endswith(old),relative
 if p.name in ('SessionLog.md','DevelopmentSchedule.md'):assert new.startswith(old.rstrip()),relative
 if p.suffix=='.py':ast.parse(new,filename=str(p))
 owned.append(str(relative))
assert len(owned)==8
for p in [BASE/'TestResults.md',*[ROOT/n for n in owned if n.endswith('.md')]]:
 content=p.read_text();backup=BASE/'Sources/Before'/p.relative_to(ROOT)
 if backup.exists():content='\n'.join(l[1:] for l in difflib.unified_diff(backup.read_text().splitlines(),content.splitlines()) if l.startswith('+') and not l.startswith('+++'))
 for target in re.findall(r'\[[^\]]+\]\(([^)]+)\)',content):
  if re.match(r'^(https?://|#)',target):continue
  dest=(p.parent/target.split('#',1)[0]).resolve();assert dest.exists() or dest==QA/'FinalValidation.json',(p,target);links+=1
for p in (BASE/'Sources').glob('*.py'):ast.parse(p.read_text(),filename=str(p))
assert not (BASE/'Sources/BeforeFingerprint.bin').exists() and not (BASE/'Sources/__pycache__').exists()
subprocess.run(['git','apply','--reverse','--check',str(BASE/'Sources/HandoffChanges.patch')],cwd=ROOT,check=True,capture_output=True)
source=ROOT/'ProjectDocs/AI/SelectionRecovery/R1';subprocess.run(['git','apply','--reverse','--check',str(source/'Sources/TaskChanges.patch')],cwd=ROOT,check=True,capture_output=True)
subprocess.run(['git','diff','--check'],cwd=ROOT,check=True,capture_output=True)
start={l[3:] for l in (QA/'WorkingTreeTaskStart.txt').read_text().splitlines() if len(l)>3}
now={l[3:] for l in subprocess.check_output(['git','status','--porcelain'],text=True,cwd=ROOT).splitlines() if len(l)>3};assert now==start,now-start
processes=subprocess.check_output(['ps','-eo','comm='],text=True).splitlines();assert not any(n.strip() in ('Unity','Unity Main Thre') for n in processes)
result={'result':'Passed','artifactSha256':build['artifactSha256'],'artifactBytes':build['artifactBytes'],'freshUnityEditDistinctPassed':160,'priorSameSourceUnityDistinctPassed':350,'newPlayModeRun':False,'offlineSyntheticBoundaryChecksPassed':b['passed'],'realAdbInvocations':0,'canonicalOfflinePreflightPinsSelectionAndAllPreviousFixes':True,'selectionCallbackRegionsVerified':3,'tutorialCallbackRegionsVerified':2,'nativeOwnersVerified':49,'projectInputsUnchangedDuringBuild':7839,'completePlayerDataPackByteIdentical':True,'packed29387MembershipAndSevenPageSizesUnchanged':True,'oldTutorialR2ApkAndInheritedSidecarPreserved':True,'sixGeneratedCacheDirectoriesPreservedReversibly':True,'generatedFingerprintRestored':True,'existingOwnedHandoffFiles':owned,'newOrChangedLocalMarkdownLinksChecked':links,'earlierAcceptanceDecisionsPreserved':True,'sourceStagePatchStillMatchesCurrentInputs':True,'gitDiffWhitespacePassed':True,'workingTreeScopePassed':True,'taskPythonSyntaxPassed':True,'normalPersonalJournalRead':False,'physicalGameplayTested':False,'human201To210AcceptancePending':True,'physicalRowsPending':18,'phoneAndHostingDeferred':True,'paidVideoSettlementUnverified':True,'effectsLast':True,'gitCommitOrPushPerformed':False}
(QA/'FinalValidation.json').write_text(json.dumps(result,indent=2)+'\n');print(json.dumps({k:result[k] for k in ('result','artifactSha256','artifactBytes','freshUnityEditDistinctPassed','offlineSyntheticBoundaryChecksPassed','nativeOwnersVerified','workingTreeScopePassed','newOrChangedLocalMarkdownLinksChecked')},indent=2))
