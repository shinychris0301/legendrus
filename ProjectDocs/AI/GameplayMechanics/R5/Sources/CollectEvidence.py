from pathlib import Path
import hashlib,json,shutil,xml.etree.ElementTree as E
root=Path.cwd();base=root/'ProjectDocs/AI/GameplayMechanics/R5'
for suffix in ['', '.meta']:shutil.copy2(base/('Sources/link.xml'+suffix),root/('Assets/AddressableAssetsData/link.xml'+suffix))
before=json.loads((base/'Sources/BeforeHashes.json').read_text());after={str(p.relative_to(root)):hashlib.sha256(p.read_bytes()).hexdigest() for d in ['Assets','Packages','ProjectSettings'] for p in (root/d).rglob('*') if p.is_file()}
scope={'changed':sorted(p for p in before if p in after and before[p]!=after[p]),'added':sorted(set(after)-set(before)),'deleted':sorted(set(before)-set(after)),'unchanged':sum(after.get(p)==h for p,h in before.items())};(base/'QA/Scope.json').write_text(json.dumps(scope,indent=2));print(json.dumps(scope,indent=2))
results=[]
for run,stage in [('Verify02','Recording'),('Verify03','Rounds'),('Verify04','Mechanics')]:
 p=base/'QA'/run/stage/'results.xml';r=E.parse(p).getroot().attrib;assert r['result']=='Passed';results.append({'run':run,'suite':stage,**{k:r[k] for k in ['total','passed','failed','skipped']},'xml':str(p.relative_to(base))})
record={'passed':True,'unity':'6000.5.9f1','suites':results,'playModeTotal':sum(int(r['total']) for r in results),'captures':'Verify02/Journey: two actual Unity renders at576x1280','scope':'Local development sandbox recording only; no human feedback collected','notRun':['New EditMode suite run (no GameplayCore changes)','Physical Android acceptance','IL2CPP build','Human difficulty playtest','Full-project test suite'],'earlierRuns':{'Verify01':'Five tests passed; inspection found JsonUtility null activeAttempt serialized as a placeholder. Changed to activeAttempts array and added assertion; final Verify02 passes.'}}
(base/'QA/Verification.json').write_text(json.dumps(record,indent=2));print('PlayMode total',record['playModeTotal'])
