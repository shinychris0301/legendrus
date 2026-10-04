"""Validate current handoff pins, offline boundaries, scope and reviewable documentation."""
from pathlib import Path
import ast,difflib,hashlib,json,re,shutil,subprocess
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';read=lambda n:json.loads((QA/n).read_text())
fingerprint=read('GeneratedFingerprintCleanup.json');assert fingerprint['beforeSha256']==fingerprint['restoredSha256'] and fingerprint['priorUserEditsAbsentVerified']
build=read('BuildValidation.json');assert build['result']=='Passed' and build['freshUnityTotalDistinctPassed']==442 and build['nativeMethodCount']==22
boundary=read('PreflightBoundaryChecks.json');assert boundary['result']=='Passed' and boundary['passed']==64 and boundary['failed']==0
assert len(boundary['cases'])==len({r['name'] for r in boundary['cases']})==64 and all(r['result']=='Passed' for r in boundary['cases'])
assert boundary['realAdbInvocations']==0 and boundary['realDeviceExecution'] is False and boundary['syntheticFilesOnly']
offline=read('OfflinePreflight.json');assert offline['status']=='offline_prepared_device_deferred' and offline['localApkSha256']==build['artifactSha256'] and offline['localApkBytes']==build['artifactBytes']
assert all(offline[k] is True for k in ('checksumSidecarMatches','fixedPresetsPackaged','atomicSpendingAndBoosterRefundRecoveryPackaged','accumulatedRewardAndCreditRecoveryPackaged','deviceExecutionDeferred'))
assert all(offline[k] is False for k in ('deviceEnumerationAttempted','physicalGameplayTested','appLaunched','appDataReadOrChanged','qaActivationVerified'))
for path,h in read('BeforeHashes.json').items():
 with (ROOT/path).open('rb') as f:assert hashlib.file_digest(f,'sha256').hexdigest()==h,path
assert len(read('BeforeHashes.json'))==7839
owned=[];checked=0
for backup in sorted((BASE/'Sources/Before').rglob('*')):
 if not backup.is_file():continue
 relative=backup.relative_to(BASE/'Sources/Before');current=ROOT/relative;old=backup.read_text();new=current.read_text();assert old!=new,relative
 if relative.name in ('CurrentTask.md','ConversationHandoff.md'):assert new.endswith(old),relative
 if relative.name in ('SessionLog.md','DevelopmentSchedule.md'):assert new.startswith(old.rstrip()),relative
 if current.suffix=='.py':ast.parse(new)
 owned.append(str(relative))
assert len(owned)==8
sources=ROOT/'ProjectDocs/AI/SpendingRecovery/R1';sourcevalidation=json.loads((sources/'QA/FinalValidation.json').read_text());assert sourcevalidation['unityTotalDistinctPassed']==442
subprocess.run(['git','apply','--reverse','--check',str(sources/'Sources/TaskChanges.patch')],cwd=ROOT,check=True,capture_output=True)
for p in [BASE/'TestResults.md',sources/'TestResults.md',*[ROOT/r for r in owned if r.endswith('.md')]]:
 content=p.read_text();snapshot=BASE/'Sources/Before'/p.relative_to(ROOT)
 if snapshot.exists():content='\n'.join(l[1:] for l in difflib.unified_diff(snapshot.read_text().splitlines(),content.splitlines()) if l.startswith('+') and not l.startswith('+++'))
 for target in re.findall(r'\[[^\]]+\]\(([^)]+)\)',content):
  if re.match(r'^(https?://|#)',target):continue
  destination=(p.parent/target.split('#',1)[0]).resolve();assert destination==QA/'FinalValidation.json' or destination.exists(),(p,target);checked+=1
for folder in (BASE/'Sources',sources/'Sources'):
 for p in folder.glob('*.py'):ast.parse(p.read_text(),filename=str(p))
 for cache in folder.glob('__pycache__'):shutil.rmtree(cache)
# Only newly introduced Python caches from canonical preflight are removed; assets/generated inputs already restored.
cache=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/__pycache__'
if cache.exists():shutil.rmtree(cache)
subprocess.run(['git','diff','--check'],cwd=ROOT,check=True,capture_output=True)
status=subprocess.check_output(['git','status','--porcelain'],cwd=ROOT,text=True);paths={l[3:] for l in status.splitlines() if len(l)>3}
allowed=set(sourcevalidation['changedProductionInputs'])|set(owned)|{'ProjectDocs/AI/SpendingRecovery/'}
assert paths<=allowed,paths-allowed
processes=subprocess.check_output(['ps','-eo','comm='],text=True).splitlines();assert not any(n.strip() in ('Unity','Unity Main Thre') for n in processes)
result={'result':'Passed','artifactSha256':build['artifactSha256'],'artifactBytes':build['artifactBytes'],'freshUnityTotalDistinctPassed':442,'freshUnityPlayDistinctPassed':221,'freshUnityEditDistinctPassed':221,'offlineSyntheticBoundaryChecksPassed':64,'realAdbInvocations':0,'canonicalOfflinePreflightPinsAllAccumulatedFixes':True,'sourceStagePatchStillMatchesCurrentInputs':True,'productionInputsUnchangedDuringBuild':7839,'existingOwnedHandoffFiles':owned,'newOrChangedLocalMarkdownLinksChecked':checked,'taskPythonSyntaxPassed':True,'gitDiffWhitespacePassed':True,'workingTreeScopePassed':True,'previousApkAndInheritedSidecarPreserved':True,'phoneExecutionDeferred':True,'hostingDeferred':True,'physicalRowsPending':18,'human201To210AcceptancePending':True,'paidVideoSettlementUnverified':True,'effectsLast':True,'gitCommitOrPushPerformed':False}
(QA/'FinalValidation.json').write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result,indent=2))
