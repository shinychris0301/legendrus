"""Check recorded completion, retained history, valid local links and working-tree scope."""
from pathlib import Path
import ast,difflib,hashlib,json,re,subprocess
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
v=json.loads((QA/'SourceValidation.json').read_text());assert v['result']=='Passed' and v['unityTotalDistinctPassed']==342
r=json.loads((QA/'TestReconciliation.json').read_text());assert r['totalDistinctPassed']==342 and r['playPassed']==249 and r['editPassed']==93 and r['failed']==r['skipped']==0
owned=[];links=0
for n in ('CurrentTask','ConversationHandoff','SessionLog','DevelopmentSchedule'):
 p=ROOT/'ProjectDocs/AI'/f'{n}.md';old=(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text();new=p.read_text()
 assert new.endswith(old) if n in ('CurrentTask','ConversationHandoff') else new.startswith(old.rstrip())
 assert 'EnergyRecovery/R1/TestResults.md' in new and '342' in new
 owned.append(p)
for p in [BASE/'TestResults.md',*owned]:
 content=p.read_text();backup=BASE/'Sources/Before'/p.relative_to(ROOT)
 if backup.exists():content='\n'.join(l[1:] for l in difflib.unified_diff(backup.read_text().splitlines(),content.splitlines()) if l.startswith('+') and not l.startswith('+++'))
 for target in re.findall(r'\[[^\]]+\]\(([^)]+)\)',content):
  if re.match(r'^(https?://|#)',target):continue
  path=(p.parent/target.split('#',1)[0]).resolve();assert path.exists() or path==QA/'FinalValidation.json',(p,target);links+=1
for p in (BASE/'Sources').glob('*.py'):ast.parse(p.read_text(),filename=str(p))
# Previous test fixtures outside the newly inserted energy block are exact.
p=ROOT/v['changedProductionInputs'][1];old=(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text();new=p.read_text()
a=new.index('        [TestCase(1)] [TestCase(3)] [TestCase(5)]\n        public void EnergyGrantLargestAmount');b=new.index('        void SnapshotAndClearSaveJournal()',a);assert new[:a]+new[b:]==old
subprocess.run(['git','diff','--check'],cwd=ROOT,check=True,capture_output=True)
start={l[3:] for l in (QA/'WorkingTreeBefore.txt').read_text().splitlines() if len(l)>3}
now={l[3:] for l in subprocess.check_output(['git','status','--porcelain'],cwd=ROOT,text=True).splitlines() if len(l)>3}
assert now==start|{'ProjectDocs/AI/EnergyRecovery/'},now-start
subprocess.run(['git','apply','--reverse','--check',str(BASE/'Sources/TaskChanges.patch')],cwd=ROOT,check=True,capture_output=True)
assert not any(n.strip() in ('Unity','Unity Main Thre') for n in subprocess.check_output(['ps','-eo','comm='],text=True).splitlines())
result={**v,'existingContinuityDocumentsUpdated':len(owned),'earlierAcceptanceDecisionsPreserved':True,'priorTestFixturesOutsideNewEnergyBlockBytePreserved':True,'newLocalMarkdownLinksChecked':links,'gitDiffWhitespacePassed':True,'workingTreeScopePassed':True,'taskPythonSyntaxPassed':True,'nextImplementation':'guarded APK refresh packaging verified energy recovery; human201-210 remains pending','effectsLast':True,'physicalChecklistRowsStillPending':18}
(QA/'FinalValidation.json').write_text(json.dumps(result,indent=2)+'\n');print(json.dumps({k:result[k] for k in ('result','unityTotalDistinctPassed','projectInputsUnchanged','energyFixNotYetPackaged','existingContinuityDocumentsUpdated','newLocalMarkdownLinksChecked','workingTreeScopePassed')},indent=2))
