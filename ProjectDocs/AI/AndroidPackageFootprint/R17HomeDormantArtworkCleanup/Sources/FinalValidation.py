"""Validate retained task evidence, authored scope and current artifact without rerunning Unity."""
from pathlib import Path
import ast,datetime,hashlib,json,re,shutil,subprocess,xml.etree.ElementTree as ET
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
for p in BASE.rglob('*.json'):json.loads(p.read_text())
for p in BASE.rglob('*.xml'):ET.parse(p)
for p in (BASE/'Sources').glob('*.py'):ast.parse(p.read_text())
for name,count in (('EditFinal',211),('PlayFinal',44)):
 summary=json.loads((QA/name/'summary.json').read_text());assert summary['exitCode']==0 and summary['result']['result']=='Passed';assert int(summary['result']['passed'])==count and int(summary['result']['failed'])==0 and int(summary['result']['skipped'])==0
cases=set()
for name in ('EditFinal','PlayFinal'):
 for t in ET.parse(QA/name/'results.xml').findall('.//test-case'):assert t.get('result')=='Passed';cases.add(t.get('fullname'))
assert len(cases)==255
broken=[]
for p in [BASE/'TestResults.md',ROOT/'ProjectDocs/AI/CurrentTask.md',ROOT/'ProjectDocs/AI/ConversationHandoff.md',ROOT/'ProjectDocs/AI/DevelopmentSchedule.md']:
 s=p.read_text()
 if p.name=='CurrentTask.md':s=s.split('# 2026-10-01 — Home/WorldMap footer cleanup R16 complete')[0]
 elif p.name=='ConversationHandoff.md':s=s.split('Prepared 2026-09-12.')[0]
 elif p.name=='DevelopmentSchedule.md':s='\n'.join(x for x in s.splitlines() if 'R17' in x or '**Immediate next action:** Return' in x)
 for link in re.findall(r'\]\(([^)]+)\)',s):
  if '://' not in link and not link.startswith('#') and not (p.parent/link.split('#')[0]).exists():broken.append((str(p),link))
assert not broken,broken
artifact=json.loads((QA/'Artifact.json').read_text());apk=ROOT/artifact['path'];assert hashlib.file_digest(apk.open('rb'),'sha256').hexdigest()==artifact['sha256'];assert apk.with_suffix('.apk.sha256').read_text().strip()==artifact['sha256']+'  '+apk.name
scope=json.loads((QA/'Scope.json').read_text());assert scope['unchanged']==7765 and len(scope['changed'])==7 and len(scope['added'])==20 and len(scope['deleted'])==18
for helper in ('HomeDormantArtworkCleanupBuildAudit.cs','HomeDormantArtworkCleanupBuildAudit.cs.meta'):assert not (ROOT/'Assets/LegendsOfRus/Scripts/Editor'/helper).exists()
subprocess.run(['git','apply','--reverse','--check',str(QA/'TaskChanges.patch')],cwd=ROOT,check=True)
lines=(QA/'TaskChanges.patch').read_text().splitlines();assert not any(l[1:].rstrip()!=l[1:] for l in lines if l.startswith('+') and not l.startswith('+++'))
cache=BASE/'Sources/__pycache__'
if cache.exists():shutil.rmtree(cache)
record={'checkedAtUtc':datetime.datetime.now(datetime.timezone.utc).isoformat(),'jsonXmlPythonSyntaxPassed':True,'distinctFinalPassingCases':len(cases),'newDocumentationLinksResolved':True,'currentApkAndChecksumSidecarVerified':True,'temporaryBuildHelperAbsent':True,'taskPatchReverseCheckPassed':True,'taskPatchNewLineWhitespacePassed':True,'sourceScopeUnchangedFromRecordedFinal':True,'baselineInputCount':7790,'finalInputCount':7792,'taskPythonCacheRemoved':True,'deviceExecution':False,'deploymentOrCommit':False}
(QA/'FinalValidation.json').write_text(json.dumps(record,indent=2)+'\n');print(json.dumps(record,indent=2))
