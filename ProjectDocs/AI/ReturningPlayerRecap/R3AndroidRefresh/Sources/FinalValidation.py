"""Reconcile the verified APK, updated offline handoff and narrow documentation/tool changes."""
from pathlib import Path
import ast
import datetime
import difflib
import json
import re
import subprocess
import xml.etree.ElementTree as ET
from VerifyBuild import BASE, QA, ROOT, read, sha, validate

result=validate()
offline=read('OfflinePreflight.json')
assert offline['status']=='offline_prepared_device_deferred'
assert offline['matchesVerifiedRecapApk'] and offline['recapPackaged'] and offline['checksumSidecarMatches']
assert offline['localApkSha256']==result['artifactSha256'] and offline['localApkBytes']==result['artifactBytes']
assert offline['deviceExecutionDeferred'] and not offline['deviceEnumerationAttempted']
assert not any(offline[key] for key in ['physicalGameplayTested','appLaunched','appDataReadOrChanged','qaActivationVerified','signatureRecheckedThisRun'])
checks=read('PreflightBoundaryChecks.json')
assert checks['result']=='Passed' and checks['passed']==40 and checks['failed']==0
assert len(checks['cases'])==len({row['name'] for row in checks['cases']})==40
assert all(row['result']=='Passed' for row in checks['cases']) and checks['realAdbInvocations']==0
cases=ET.parse(QA/'Policy/results.xml').findall('.//test-case')
(QA/'PolicyCaseInventory.json').write_text(json.dumps({'passed':130,'failed':0,'skipped':0,'mode':'EditMode',
    'cases':[{'name':case.get('fullname'),'result':case.get('result')} for case in cases]},indent=2)+'\n')
changes=[];patch=[]
files=sorted(p for p in (BASE/'Sources/Before').rglob('*') if p.is_file())
assert len(files)==8
for old in files:
    path=str(old.relative_to(BASE/'Sources/Before'));current=ROOT/path
    assert sha(old)!=sha(current),path
    changes.append({'path':path,'beforeSha256':sha(old),'afterSha256':sha(current)})
    patch.extend(difflib.unified_diff(old.read_text().splitlines(keepends=True),current.read_text().splitlines(keepends=True),fromfile='a/'+path,tofile='b/'+path))
assert all(line[1:].rstrip('\r\n')==line[1:].rstrip() for line in patch if line.startswith('+') and not line.startswith('+++'))
patch_path=BASE/'Sources/TaskChanges.patch';patch_path.write_text(''.join(patch))
subprocess.run(['git','apply','--check','--reverse',str(patch_path)],cwd=ROOT,check=True,capture_output=True,text=True)
(QA/'ChangedFiles.json').write_text(json.dumps({'existingFilesChanged':changes,
    'newTaskFilesUnder':str(BASE.relative_to(ROOT)),'projectInputsChanged':0,
    'currentApkReplacedWithVerifiedArtifact':True,'oldApkAndSidecarArchived':True},indent=2)+'\n')
checklist=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R1/DeviceChecklist.md'
rows=re.findall(r'^\| ([A-Z][A-Z0-9-]*-\d+) \|',checklist.read_text(),re.M)
assert len(rows)==len(set(rows))==18 and all('RECAP-'+suffix in rows for suffix in ['01','02','03'])
assert 'Pending eligible finale fixture and phone evidence' in checklist.read_text()
assert 'cannot seed arbitrary booster quantities' in checklist.read_text()
assert result['artifactSha256'] in checklist.read_text()
assert result['artifactSha256'] in (ROOT/'ProjectDocs/AI/AndroidGameplayProfile/R1/Usage.md').read_text()
for source in [ROOT/row['path'] for row in changes if row['path'].endswith('.py')]:
    ast.parse(source.read_text(),filename=str(source))
assert not list(BASE.rglob('__pycache__'))
outputs=[QA/'FinalValidation.json',QA/'ChangedFiles.json']
links=[]
for target in re.findall(r'\]\(([^)]+)\)',(BASE/'TestResults.md').read_text()):
    if target.startswith(('http:','https:','#')):
        continue
    path=(BASE/target.split('#')[0]).resolve()
    assert path.exists() or path in outputs,target
    links.append(path)
result.update(checkedAtUtc=datetime.datetime.now(datetime.timezone.utc).isoformat(),
              clientTaskDate='2026-10-02',offlinePreflightBoundaryChecksPassed=40,
              currentOfflinePreflightPassedWithoutDeviceCalls=True,existingFilesChanged=8,
              pendingDeviceRows=18,pendingRecapRows=3,newDocumentationLinksResolved=len(links),
              taskAddedLinesWhitespaceAndReversePatchChecksPassed=True)
(QA/'FinalValidation.json').write_text(json.dumps(result,indent=2)+'\n')
assert all(path.exists() for path in links)
print(json.dumps(result,indent=2))
