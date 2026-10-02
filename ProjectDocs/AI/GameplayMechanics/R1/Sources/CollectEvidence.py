from pathlib import Path
import hashlib,json,shutil,xml.etree.ElementTree as E
root=Path.cwd();base=root/'ProjectDocs/AI/GameplayMechanics/R1'
# Exact pre-run restoration, including existing GUID.
for suffix in ['', '.meta']: shutil.copy2(base/('Sources/link.xml'+suffix),root/('Assets/AddressableAssetsData/link.xml'+suffix))
before=json.loads((base/'Sources/BeforeHashes.json').read_text())
after={str(p.relative_to(root)):hashlib.sha256(p.read_bytes()).hexdigest() for d in ['Assets','Packages','ProjectSettings'] for p in (root/d).rglob('*') if p.is_file()}
scope={'changed':sorted(p for p in before if p in after and before[p]!=after[p]),'added':sorted(set(after)-set(before)),'deleted':sorted(set(before)-set(after)),'unchanged':sum(after.get(p)==h for p,h in before.items())}
(base/'QA/Scope.json').write_text(json.dumps(scope,indent=2));print(json.dumps(scope,indent=2))
results=[]
for run,stage in [('Verify03','EditMode'),('Verify06','Mechanics'),('Verify03','CampaignUI'),('Verify03','CampaignBoard'),('Verify03','NativeJourney')]:
 p=base/'QA'/run/stage/'results.xml';r=E.parse(p).getroot().attrib;assert r['result']=='Passed'
 results.append({'run':run,'suite':stage,**{k:r[k] for k in ['total','passed','failed','skipped']},'xml':str(p.relative_to(base))})
record={'passed':True,'unity':'6000.5.9f1','suites':results,'editModeTotal':297,'playModeTotal':11,'newMechanicsCases':12,'captures':'Verify06/Journey: actual Unity render, sampled frames, not timing measurement','scope':'Development sandbox only; no production campaign activation','notRun':['Physical Android acceptance','IL2CPP build','Human difficulty test','Full-project test suite'],'earlierRuns':{'Verify01':'Old collapse test expected the wrong destination; fixed assertion only.','Verify02':'Final creation fixture swiped wrong coordinate direction; corrected.','Verify04':'Diagnostic GetMesh call used wrong overload; corrected test instrumentation.','Verify05':'Diagnostic exposed missing CanvasRenderer on new impact graphic; added required renderer.','Verify06':'Final sandbox rendering and all fixtures pass.'}}
(base/'QA/Verification.json').write_text(json.dumps(record,indent=2))
shutil.copy2('/tmp/lor-gameplay-mechanics-r1/Verify06/Journey/ImpactDiagnostics.txt',base/'QA/ImpactDiagnostics.txt')
