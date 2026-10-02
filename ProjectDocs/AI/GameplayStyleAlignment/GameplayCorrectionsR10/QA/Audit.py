from pathlib import Path
import json,hashlib,re
root=Path.cwd();docs=root/'ProjectDocs/AI/GameplayStyleAlignment/GameplayCorrectionsR10';before=json.loads((docs/'BeforeProductionHashes.json').read_text());changed=[];same=[];missing=[]
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
for e in before:
 p=root/e['path']
 if not p.exists():missing.append(e['path'])
 elif sha(p)!=e['sha256']:changed.append(e['path'])
 else:same.append(e['path'])
assert not missing
old=json.loads(Path('/tmp/lor-gameplay-r10/before/Assets/LegendsOfRus/Resources/Config/chapter01_levels.json').read_text());new=json.loads((root/'Assets/LegendsOfRus/Resources/Config/chapter01_levels.json').read_text())
for a,b in zip(old['levels'],new['levels']):
 assert b['startingScore']==0
 assert [x-a['startingScore'] for x in a['starScoreThresholds']]==b['starScoreThresholds']
 for ag,bg in zip(a['goals'],b['goals']):
  expected=ag.copy()
  if ag['type']=='score':expected['target']-=a['startingScore']
  assert expected==bg
 a['startingScore']=b['startingScore'];a['starScoreThresholds']=b['starScoreThresholds'];a['goals']=b['goals'];assert a==b
ids={}
for name in ['Gameplay','PreLevel']:
 rel=f'Assets/LegendsOfRus/Resources/ProductionUI/Pages/{name}.prefab';oldids=re.findall(r'^--- !u!\d+ &(-?\d+)',Path('/tmp/lor-gameplay-r10/before',rel).read_text(),re.M);newids=re.findall(r'^--- !u!\d+ &(-?\d+)',(root/rel).read_text(),re.M);assert set(oldids)<=set(newids);ids[name]={'old':len(oldids),'new':len(newids),'allExistingRetained':True}
metas=[e for e in before if e['path'].endswith('.meta')];assert all(e['path'] in same for e in metas)
record={'unchangedCount':len(same),'changedExistingFiles':changed,'missing':missing,'existingMetasPreserved':len(metas),'serializedObjects':ids,'scoreEffortAndOtherLevelDataPreserved':True}
(docs/'QA/SourceAudit.json').write_text(json.dumps(record,indent=2)+'\n');print(json.dumps(record,indent=2))
final=before+[{'path':str(p.relative_to(root)),'sha256':sha(p)} for p in (root/'Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned').glob('*_r10*') if str(p.relative_to(root)) not in [x['path'] for x in before]]
for e in final:e['sha256']=sha(root/e['path'])
(docs/'FinalProductionHashes.json').write_text(json.dumps(final,indent=2)+'\n')
