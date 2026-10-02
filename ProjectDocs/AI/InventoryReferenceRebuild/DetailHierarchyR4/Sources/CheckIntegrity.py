from pathlib import Path
import hashlib,json,sys
from PIL import Image
import numpy as np
b=Path(__file__).resolve().parents[1];before=json.loads((b/'Sources/BeforeAssetsHashes.json').read_text())
protected=[p for p in before if p.endswith(('.png','.meta','.ttf','.otf')) or '/Pages/' in p and p.endswith('.prefab') and not p.endswith('/Inventory.prefab')]
changed=[p for p in protected if not Path(p).exists() or hashlib.sha256(Path(p).read_bytes()).hexdigest()!=before[p]]
assert not changed
report={'protectedExistingFiles':len(protected),'changedProtectedFiles':changed,'comparisons':{}}
run=Path('/tmp/lor-inventory-detail-r4')/sys.argv[1]
for name in ('Inventory_941x1672_RU.png','Inventory_941x1672_EN.png','Inventory_Boosters_RU.png'):
 a=np.array(Image.open(b.parent/'TabsAndLetteringR3/UnityCaptures'/name));c=np.array(Image.open(run/'Inventory'/name));d=np.any(a!=c,axis=2)
 record={'totalChangedPixels':int(d.sum())}
 d[1120:1395,330:905]=False
 record['outsideDetailTextChangedPixels']=int(d.sum());assert not d.any(),record
 report['comparisons'][name]=record
(b/'QA/IntegrityChecks.json').write_text(json.dumps(report,indent=2)+'\n');print(json.dumps(report,indent=2))
