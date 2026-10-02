from pathlib import Path
import hashlib,json,shutil,xml.etree.ElementTree as E
root=Path.cwd();base=root/'ProjectDocs/AI/GameplayMechanics/R3'
for suffix in ['', '.meta']:shutil.copy2(base/('Sources/link.xml'+suffix),root/('Assets/AddressableAssetsData/link.xml'+suffix))
before=json.loads((base/'Sources/BeforeHashes.json').read_text())
after={str(p.relative_to(root)):hashlib.sha256(p.read_bytes()).hexdigest() for d in ['Assets','Packages','ProjectSettings'] for p in (root/d).rglob('*') if p.is_file()}
scope={'changed':sorted(p for p in before if p in after and before[p]!=after[p]),'added':sorted(set(after)-set(before)),'deleted':sorted(set(before)-set(after)),'unchanged':sum(after.get(p)==h for p,h in before.items())}
(base/'QA/Scope.json').write_text(json.dumps(scope,indent=2));print(json.dumps(scope,indent=2))
results=[]
for run,stage in [('Verify01','EditMode'),('Verify03','Seal')]+[('Verify04'+x,x) for x in ['Mechanics','Spark','CampaignUI','CampaignBoard','NativeJourney']]:
 p=base/'QA'/run/stage/'results.xml';r=E.parse(p).getroot().attrib;assert r['result']=='Passed'
 results.append({'run':run,'suite':stage,**{k:r[k] for k in ['total','passed','failed','skipped']},'xml':str(p.relative_to(base))})
record={'passed':True,'unity':'6000.5.9f1','suites':results,'editModeTotal':sum(int(r['total']) for r in results if r['suite']=='EditMode'),'playModeTotal':sum(int(r['total']) for r in results if r['suite']!='EditMode'),'newSealCoreCases':17,'newSealRuntimeCases':2,'captures':'Verify03/Journey: three actual Unity renders at576x1280','scope':'Development sandbox only; no campaign activation','notRun':['Physical Android acceptance','IL2CPP build','Human difficulty playtest','Full-project test suite'],'earlierRuns':{'Verify01/Seal':'Wrong test line chosen first; corrected intended action coordinates.','Verify02/Seal':'Diagnostic GetMesh overload compile error; fixed.','Verify03/Seal':'Both final seal tests passed; captures inspected.','Verify04NativeJourney':'Runner wrapper exited143 while Unity continued. Final passing XML recovered after Unity finished; child exit code unavailable.'}}
(base/'QA/Verification.json').write_text(json.dumps(record,indent=2));print('Totals',record['editModeTotal'],record['playModeTotal'])
