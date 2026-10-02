from pathlib import Path
import hashlib,json,sys
from PIL import Image
import numpy as np
b=Path(__file__).resolve().parents[1];before=json.loads((b/'Sources/BeforeAssetsHashes.json').read_text())
protected=[p for p in before if p.endswith(('.png','.meta','.ttf','.otf')) or '/Pages/' in p and p.endswith('.prefab') and not p.endswith('/OfflineState.prefab')]
changed=[p for p in protected if not Path(p).exists() or hashlib.sha256(Path(p).read_bytes()).hexdigest()!=before[p]]
assert not changed
ref=Image.open(b/'Sources/Reference.png').convert('RGB');assets=Path('Assets/LegendsOfRus/Resources/ProductionUI/OfflineAligned');checks={}
for name,box in [('retry',(239,1054,706,1196)),('home',(267,1193,675,1300))]:
 new=np.array(Image.open(assets/(name+'_ru_r3.png')).convert('RGBA'));old=np.array(Image.open(assets/(name+'.png')).convert('RGBA'))
 assert np.array_equal(new[:,:,:3],np.array(ref.crop(box)));assert np.array_equal(new[:,:,3],old[:,:,3])
 checks[name]={'sourceRGBExact':True,'originalAlphaExact':True}
panel=np.array(Image.open(assets/'panel_ru_r3.png').convert('RGBA'));old=np.array(Image.open(assets/'panel.png').convert('RGBA'));box=(190,346,753,436)
assert np.array_equal(panel[161:251,70:633,:3],np.array(ref.crop(box)))
mask=np.ones(panel.shape[:2],dtype=bool);mask[161:251,70:633]=False;assert np.array_equal(panel[mask],old[mask])
checks['panel']={'headingRGBExact':True,'outsideHeadingUnchanged':True}
report={'protectedExistingFiles':len(protected),'changedProtectedFiles':changed,'comparisons':checks}
(b/'QA/IntegrityChecks.json').write_text(json.dumps(report,indent=2)+'\n');print(json.dumps(report,indent=2))
