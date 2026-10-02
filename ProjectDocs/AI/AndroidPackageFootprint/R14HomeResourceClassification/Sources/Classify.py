"""Join source, packed and Unity evidence without claiming rendering/device acceptance."""
from pathlib import Path
import json
BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';VERIFIED=QA/'EditorAccepted'
assets=json.loads((QA/'Assets.json').read_text());refs=json.loads((QA/'References.json').read_text())['references'];prefabs=json.loads((VERIFIED/'PrefabImages.json').read_text())
observed=[json.loads(p.read_text()) for p in VERIFIED.glob('*.json') if p.stem.startswith(('Fresh','Installed','LegacyMigration','CurrentMigration'))]
keep={'home_logo','avatar_portrait','coin_icon','gems_icon','energy_icon'}
shared={'avatar_ring','resource_capsule','resource_plus','settings_medallion'}
rows=[]
def eligible(i):return i['enabled'] and i['activeWithinRoot'] and i['alpha']>0 and i['ancestorGroupAlpha']>0
for asset in assets:
    name=asset['name'];inventory=json.loads((VERIFIED/(name+'.json')).read_text())
    images=[{'page':p['page'],**i} for p in prefabs['pages'] for i in p['images'] if i['resource']==name and i['slot']=='sprite']
    completed=[{'page':p['page'],'mode':p['mode'],**i} for p in observed for i in p.get('images',[]) if i['resource']==name and i['slot']=='sprite']
    category='retain-live-consumer' if name in keep else 'retain-shared-builder-dependency' if name in shared else 'home-owner-cleanup-required'
    if name=='footer_base_generated':category='home-and-worldmap-cleanup-required'
    active=[i for i in images if eligible(i)]
    if category=='retain-live-consumer':assert active
    elif category.endswith('cleanup-required'):assert not active and not any(eligible(i) for i in completed)
    source=[r for r in refs if name in r['matches'] and r['path'].endswith('.cs') and '/Tests/' not in r['path']]
    rows.append({'name':name,'category':category,'serializedConsumerRootCount':len(inventory['consumers']),'serializedConsumerRoots':inventory['consumers'],'spriteImageCount':len(images),'prefabImageStateEligibleCount':len(active),'prefabImages':images,'completedConstructionImages':completed,'freshAndLegacyMissingKeyThrows':inventory['freshDenialConfirmed'] and inventory['legacyDenialConfirmed'],'packedObjectBytes':asset['packedObjectBytes'],'android':asset['android'],'sourceSites':source})
counts={c:sum(r['category']==c for r in rows) for c in sorted({r['category'] for r in rows})}
assert sum(counts.values())==21
record={'assets':rows,'categoryCounts':counts,'recursiveDependencyRoots':len(prefabs['roots']),'prefabRootsInspected':len(prefabs['pages']),'all21StillRequiredByFreshAndLegacyHome':True,'readyToArchiveWithoutOwnerChanges':[],'nextCandidates':['hud_frame','quest_panel'],'nextCandidatePackedObjectBytes':sum(a['packedObjectBytes'] for a in assets if a['name'] in ('hud_frame','quest_panel')),'totalPackedObjectBytes':sum(a['packedObjectBytes'] for a in assets),'compressedSavingsMeasured':False,'physicalDeviceTested':False,'imageStateLimit':'Enabled, locally active hierarchy, positive Image/group alpha are structural observations, not actual visibility, clipping/occlusion or pixel measurements. Prefab assets activeInHierarchy is false by Unity design; use activeWithinRoot for their local serialized hierarchy. overrideSprite getter mirrors sprite when no override exists; counts use sprite slot only.'}
(QA/'Classification.json').write_text(json.dumps(record,indent=2)+'\n')
print(json.dumps({k:v for k,v in record.items() if k!='assets'},indent=2))
