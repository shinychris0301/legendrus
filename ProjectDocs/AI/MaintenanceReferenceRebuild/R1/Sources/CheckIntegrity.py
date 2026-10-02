from pathlib import Path
import hashlib,json
from PIL import Image
import numpy as np
b=Path(__file__).resolve().parents[1];before=json.loads((b/'Sources/BeforeAssetsHashes.json').read_text())
protected=[p for p in before if p.endswith(('.png','.meta','.ttf','.otf')) or '/Pages/' in p and p.endswith('.prefab') and not p.endswith('/MaintenanceState.prefab')]
changed=[p for p in protected if not Path(p).exists() or hashlib.sha256(Path(p).read_bytes()).hexdigest()!=before[p]];assert not changed
ref=Image.open(b/'Sources/Reference.png').convert('RGB');assets=Path('Assets/LegendsOfRus/Resources/ProductionUI/MaintenanceAligned');checks={}
for name,box,last in [('panel',(60,311,880,1523),300),('timer',(166,854,774,1046),74),('button',(206,1302,738,1436),134)]:
 new=np.array(Image.open(assets/(name+'_ru_r2.png')).convert('RGBA'));old=np.array(Image.open(assets/(name+'.png')).convert('RGBA'));rgb=np.array(ref.crop(box))
 assert np.array_equal(new[:last,:,:3],rgb[:last]);assert np.array_equal(new[:,:,3],old[:,:,3]);assert np.array_equal(new[last:],old[last:])
 checks[name]={'restoredSourceRGBExact':True,'originalAlphaExact':True,'remainingRowsUnchanged':True}
report={'protectedExistingFiles':len(protected),'changedProtectedFiles':changed,'comparisons':checks}
(b/'QA/IntegrityChecks.json').write_text(json.dumps(report,indent=2)+'\n');print(json.dumps(report,indent=2))
