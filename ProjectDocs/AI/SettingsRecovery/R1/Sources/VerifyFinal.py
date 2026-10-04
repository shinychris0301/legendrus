"""Validate final source handoff, exact production scope and current APK limitations."""
from pathlib import Path
import ast,difflib,hashlib,json,re,subprocess
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
v=json.loads((QA/'SourceValidation.json').read_text());assert v['result']=='Passed'
for n,h in json.loads((QA/'BeforeHashes.json').read_text()).items():
 with (ROOT/n).open('rb') as f:now=hashlib.file_digest(f,'sha256').hexdigest()
 assert (now!=h)==(n in v['changedProductionInputs']),n
for p in (BASE/'Sources').glob('*.py'):ast.parse(p.read_text(),filename=str(p))
owned=[];links=0
for name in ('CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule'):
 p=ROOT/'ProjectDocs/AI'/f'{name}.md';old=(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text();new=p.read_text();assert new!=old
 assert new.endswith(old) if name in ('CurrentTask','ConversationHandoff') else new.startswith(old.rstrip())
 owned.append(str(p.relative_to(ROOT)))
for p in [BASE/'TestResults.md',*[ROOT/n for n in owned]]:
 backup=BASE/'Sources/Before'/p.relative_to(ROOT);s=p.read_text()
 if backup.exists():s='\n'.join(l[1:] for l in difflib.unified_diff(backup.read_text().splitlines(),s.splitlines()) if l.startswith('+') and not l.startswith('+++'))
 for target in re.findall(r'\[[^\]]+\]\(([^)]+)\)',s):
  if re.match(r'^(https?://|#)',target):continue
  destination=(p.parent/target.split('#',1)[0]).resolve();assert destination.exists() or destination==QA/'FinalValidation.json';links+=1
subprocess.run(['git','apply','--reverse','--check',str(BASE/'Sources/TaskChanges.patch')],cwd=ROOT,check=True,capture_output=True)
subprocess.run(['git','diff','--check'],cwd=ROOT,check=True,capture_output=True)
start={l[3:] for l in (QA/'WorkingTreeBefore.txt').read_text().splitlines() if len(l)>3}
now={l[3:] for l in subprocess.check_output(['git','status','--porcelain'],cwd=ROOT,text=True).splitlines() if len(l)>3};assert now==start|set(v['changedProductionInputs'])|{'ProjectDocs/AI/SettingsRecovery/'},now-start
assert not any(n.strip() in ('Unity','Unity Main Thre') for n in subprocess.check_output(['ps','-eo','comm='],text=True).splitlines())
assert v['apkAndSidecarUnchanged'] and v['settingsFixNotYetPackaged']
a=json.loads((QA/'ArtifactBefore.json').read_text());apk=ROOT/a['path']
with apk.open('rb') as f:assert hashlib.file_digest(f,'sha256').hexdigest()==a['sha256']
assert apk.stat().st_size==a['bytes'] and Path(str(apk)+'.sha256').read_text()==a['sidecar']
backups=[p for p in (BASE/'Sources/Before').rglob('*') if p.is_file()];assert len(backups)==8
result=dict(v,earlierAcceptanceDecisionsPreserved=True,continuityDocumentsUpdated=owned,preTaskFileBackups=8,newLocalMarkdownLinksChecked=links,workingTreeScopePassed=True,gitDiffWhitespacePassed=True,taskPythonSyntaxPassed=True,physicalRowsPending=18,phoneAndHostingDeferred=True,paidVideoSettlementUnverified=True,effectsLast=True)
(QA/'FinalValidation.json').write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps({k:result[k] for k in ('result','unityTotalDistinctPassed','projectInputsUnchanged','apkAndSidecarUnchanged','settingsFixNotYetPackaged','workingTreeScopePassed')},indent=2))
