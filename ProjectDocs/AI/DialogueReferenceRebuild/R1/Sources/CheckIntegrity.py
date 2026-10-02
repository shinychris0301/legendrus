from pathlib import Path
import hashlib,json
from PIL import Image
import numpy as np
b=Path(__file__).resolve().parents[1];before=json.loads((b/'Sources/BeforeAssetsHashes.json').read_text())
protected=[p for p in before if p.endswith(('.png','.meta','.ttf','.otf')) or '/Pages/' in p and p.endswith('.prefab') and not p.endswith('/Dialogue.prefab')]
changed=[p for p in protected if not Path(p).exists() or hashlib.sha256(Path(p).read_bytes()).hexdigest()!=before[p]];assert not changed,changed
ref=Image.open(b/'Sources/Reference.png').convert('RGB');assets=Path('Assets/LegendsOfRus/Resources/ProductionUI/DialogueAligned');checks={}
for a in json.loads((b/'Sources/AssetProvenance.json').read_text())['assets']:
 name=a['asset'].replace('_ru_r2.png','');x,y,w,h=a['sourceBounds'];rx,ry,rw,rh=a['restoredRectangle']
 new=np.array(Image.open(assets/a['asset']).convert('RGBA'));old=np.array(Image.open(assets/(name+'.png')).convert('RGBA'));rgb=np.array(ref.crop((x,y,x+w,y+h)))
 mask=np.zeros((h,w),dtype=bool);mask[ry:ry+rh,rx:rx+rw]=True
 assert np.array_equal(new[:,:,:3][mask],rgb[mask]);assert np.array_equal(new[:,:,3],old[:,:,3]);assert np.array_equal(new[~mask],old[~mask])
 checks[name]={'restoredSourceRGBExact':True,'originalAlphaExact':True,'outsidePatchUnchanged':True}
report={'protectedExistingFiles':len(protected),'changedProtectedFiles':changed,'comparisons':checks}
(b/'QA/IntegrityChecks.json').write_text(json.dumps(report,indent=2)+'\n');print(json.dumps(report,indent=2))
