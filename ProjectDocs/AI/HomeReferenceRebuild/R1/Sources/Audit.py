from pathlib import Path
import json,hashlib,ast,re
from PIL import Image
import numpy as np
b=Path(__file__).resolve().parent;qa=b.parent/'QA';before=json.loads((qa/'Before.json').read_text());sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
now={str(p):sha(p)for d in ['Assets','Packages','ProjectSettings']for p in Path(d).rglob('*')if p.is_file()}
changed=[p for p,h in before.items()if now.get(p)!=h];new=[p for p in now if p not in before];deleted=[p for p in before if p not in now]
(qa/'SourceChanges.json').write_text(json.dumps({'changed':changed,'added':new,'deleted':deleted,'unchangedCount':sum(now.get(p)==h for p,h in before.items())},indent=2))
manifest=json.loads((b/'FinalManifest.json').read_text());ref=Image.open(b/'OriginalReference.png').convert('RGBA');exact={}
for n in ['source_hero','source_logo','source_avatar','source_play_ru','source_map_ru','source_shop_ru','source_events_ru','source_characters_ru','source_settings','source_plus','source_daily_badge']:
 a=np.array(Image.open(b/'Final'/(n+'.png')));s=np.array(ref.crop(manifest[n]['sourceBounds']));visible=a[:,:,3]>0;exact[n]=bool(np.all(a[:,:,:3][visible]==s[:,:,:3][visible]))
assert all(exact.values());(qa/'SourceRGB.json').write_text(json.dumps(exact,indent=2))
audit=[]
for d in sorted((b/'Scenario').iterdir()):
 if not d.is_dir():continue
 audit.append({'name':d.name,'job':json.loads((d/'Job.json').read_text()),'billing':json.loads((d/'Billing.json').read_text()),'acceptedAsArt':d.name!='tail','note':'Tail mask rejected: selected nearby card. Local bounded source-alpha restoration instead.'if d.name=='tail'else ''})
(qa/'ScenarioAudit.json').write_text(json.dumps({'acceptedJobs':len(audit),'totalCU':sum(j['billing']['cuCost']for j in audit),'duplicateSubmissions':0,'jobs':audit},indent=2))
for p in b.glob('*.py'):ast.parse(p.read_text())
old=Path('ProjectDocs/AI/HomeReferenceRebuild/R1/Sources/BeforeCode/Home.prefab').read_text();current=Path('Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab').read_text();ids=lambda s:set(re.findall(r'^--- !u!\d+ &(-?\d+)',s,re.M));oldids=ids(old);currentids=ids(current)
(qa/'PrefabIdentity.json').write_text(json.dumps({'previousObjects':len(oldids),'preservedObjects':len(oldids&currentids),'removedIds':sorted(oldids-currentids),'addedObjects':len(currentids-oldids)},indent=2))
protected=[p for p in changed if any(x in p for x in ['SplashTitle','Loading','ProjectSettings/','Packages/'])]
assert not protected,protected
print('Changed',len(changed),'added',len(new),'deleted',len(deleted),'sourceRGB',all(exact.values()),'ScenarioCU',sum(j['billing']['cuCost']for j in audit),'oldIDs lost',len(oldids-currentids))
