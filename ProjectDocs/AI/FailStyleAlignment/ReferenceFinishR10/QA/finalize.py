from pathlib import Path
import json,hashlib,shutil,re,difflib,xml.etree.ElementTree as ET
r=Path.cwd();o=r/'ProjectDocs/AI/FailStyleAlignment/ReferenceFinishR10';qa=o/'QA'
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
write=lambda p,obj:p.write_text(json.dumps(obj,indent=2,ensure_ascii=False)+'\n')
b=json.loads((o/'BeforeProductionHashes.json').read_text());changed=[x['path'] for x in b if sha(r/x['path'])!=x['sha256']]
new=['Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/'+n+ext for n in ('environment_r10','ivan_r10','panel_r10','title_r10','goals_r10','nameplate_r10') for ext in ('.png','.png.meta')]
old=Path('/tmp/lor-fail-r10/before/Assets/LegendsOfRus/Resources/ProductionUI/Pages/Fail.prefab').read_text();now=(r/'Assets/LegendsOfRus/Resources/ProductionUI/Pages/Fail.prefab').read_text();ids=lambda s:set(re.findall(r'^--- !u!\d+ &(\d+)',s,re.M))
prev=json.loads((r/'ProjectDocs/AI/FailStyleAlignment/OfferCardsR9/UnityVerification/EvidenceHashes.json').read_text());metas=[x for x in b if x['path'].endswith('.meta')]
a={'baselineCount':len(b),'unchangedCount':len(b)-len(changed),'changedFiles':changed,'newProductionFiles':new,'existingMetas':len(metas),'allExistingMetasUnchanged':all(sha(r/x['path'])==x['sha256'] for x in metas),'oldPrefabObjectCount':len(ids(old)),'newPrefabObjectCount':len(ids(now)),'missingOldPrefabIds':sorted(ids(old)-ids(now)),'priorEvidenceCount':len(prev),'priorEvidenceUnchanged':all(sha(r/x['path'])==x['sha256'] for x in prev),'csTrailingWhitespace':{p:[i for i,l in enumerate((r/p).read_text().splitlines(),1) if l.rstrip()!=l] for p in changed if p.endswith('.cs')}}
write(qa/'SourceAudit.json',a);write(o/'FinalProductionHashes.json',[{'path':p,'sha256':sha(r/p)} for p in sorted([x['path'] for x in b]+new)])
diff=[]
for p in changed:
 before=Path('/tmp/lor-fail-r10/before')/p
 if before.exists():diff+=difflib.unified_diff(before.read_text().splitlines(True),(r/p).read_text().splitlines(True),fromfile='before/'+p,tofile='after/'+p)
(o/'AppliedSourceChanges.diff').write_text(''.join(diff))
v=o/'UnityVerification';v.mkdir(exist_ok=True);results={}
for part in ('ImportFail','EditMode','PlayMode','Fail','Victory','PreLevel'):
 source=Path('/tmp/lor-fail-r10/run3')/part;dest=v/part;dest.mkdir(exist_ok=True)
 for p in source.iterdir():
  if p.suffix in ('.json','.png','.xml','.log'):shutil.copy2(p,dest/p.name)
 if (source/'results.xml').exists():results[part]={k:ET.parse(source/'results.xml').getroot().attrib[k] for k in ('result','total','passed','failed','skipped')}
 if (source/'command.json').exists():results[part+'ExitCode']=json.loads((source/'command.json').read_text())['exitCode']
 if part in ('Fail','Victory','PreLevel'):results[part+'CapturePairs']=len(list(dest.glob('*.png')))
write(qa/'UnityResults.json',results)
print(json.dumps(a,indent=2));print(json.dumps(results,indent=2))
