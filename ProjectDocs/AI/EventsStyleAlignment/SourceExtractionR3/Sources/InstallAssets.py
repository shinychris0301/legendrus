from pathlib import Path
from PIL import Image
import json,hashlib,shutil
b=Path(__file__).resolve().parent;base=b.parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned');records=json.loads((base/'AssetProvenance.json').read_text())
for e in records:
 src=Path(e['output']);dest=out/src.name
 if dest.exists() and dest.read_bytes()!=src.read_bytes():raise RuntimeError('Refusing to overwrite installed asset without an explicit correction')
 shutil.copy2(src,dest);e['output']=str(dest)
source=b/'Scenario/backgrounds/booster_medallion_trial01.png';im=Image.open(source).convert('RGBA');assert im.size==(1024,768)
for i,n in enumerate(['firebird','yaga','merchant','knight']):
 box=((i%2)*512,(i//2)*384,(i%2+1)*512,(i//2+1)*384);plate=im.crop(box);dest=out/('scene_'+n+'_r3.png');plate.save(dest);records.append({'asset':'scene_'+n,'output':str(dest),'sha256':hashlib.sha256(dest.read_bytes()).hexdigest(),'source':str(source),'sourceBox':box,'processing':'Split newly generated clean four-plate sheet; original generated RGB retained, opaque surface; not a reference screenshot crop.'})
(base/'AssetProvenance.json').write_text(json.dumps(records,indent=2)+'\n');print('Installed',len(records),'new assets; existing PNG/meta untouched.')
