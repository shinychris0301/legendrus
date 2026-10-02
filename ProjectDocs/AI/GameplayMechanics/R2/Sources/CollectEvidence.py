from pathlib import Path
import hashlib,json,shutil,xml.etree.ElementTree as E
root=Path.cwd();base=root/'ProjectDocs/AI/GameplayMechanics/R2'
for suffix in ['', '.meta']:shutil.copy2(base/('Sources/link.xml'+suffix),root/('Assets/AddressableAssetsData/link.xml'+suffix))
before=json.loads((base/'Sources/BeforeHashes.json').read_text())
after={str(p.relative_to(root)):hashlib.sha256(p.read_bytes()).hexdigest() for d in ['Assets','Packages','ProjectSettings'] for p in (root/d).rglob('*') if p.is_file()}
scope={'changed':sorted(p for p in before if p in after and before[p]!=after[p]),'added':sorted(set(after)-set(before)),'deleted':sorted(set(before)-set(after)),'unchanged':sum(after.get(p)==h for p,h in before.items())}
(base/'QA/Scope.json').write_text(json.dumps(scope,indent=2));print(json.dumps(scope,indent=2))
results=[]
for run,stage in [('Verify04','EditMode'),('Verify06','Spark'),('Verify07','Mechanics'),('Verify03','CampaignUI'),('Verify03','CampaignBoard'),('Verify03','NativeJourney')]:
 p=base/'QA'/run/stage/'results.xml';r=E.parse(p).getroot().attrib;assert r['result']=='Passed'
 results.append({'run':run,'suite':stage,**{k:r[k] for k in ['total','passed','failed','skipped']},'xml':str(p.relative_to(base))})
record={'passed':True,'unity':'6000.5.9f1','suites':results,'editModeTotal':sum(int(r['total']) for r in results if r['suite']=='EditMode'),'playModeTotal':sum(int(r['total']) for r in results if r['suite']!='EditMode'),'newSparkCases':18,'captures':'Verify06/Journey: actual Unity render, sampled frames, not timing measurement','scope':'Development sandbox only; no production campaign activation','notRun':['Physical Android acceptance','IL2CPP build','Human difficulty test','Full-project test suite'],'earlierRuns':{'Verify01':'Two synthetic fixtures contained incidental matches; corrected fixture data.','Verify02':'All suites passed; captured duplicate source icon and generic effects.','Verify03':'All suites passed; hint-only-square case added, takeoff fixed.','Verify04':'Final EditMode including cascade-created Sparks and inventory activation.','Verify05':'Native Editor signal11 during combined PlayMode capture; no results XML, not counted as passed. Removed only generated InitTestScene files absent from before hashes.','Verify06':'Isolated final Spark runtime fixtures and captures.','Verify07':'Isolated baseline mechanics regression.'}}
(base/'QA/Verification.json').write_text(json.dumps(record,indent=2))
shutil.copy2('/tmp/lor-gameplay-mechanics-r2/Verify06/Journey/ImpactDiagnostics.txt',base/'QA/ImpactDiagnostics.txt')
