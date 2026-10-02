from pathlib import Path
from PIL import Image
import numpy as np,json
b=Path(__file__).resolve().parent.parent
results={}
for old in (b.parent/'SafeEdgesR3/UnityCaptures').glob('*.png'):
    new=b/'UnityCaptures'/old.name
    if not new.exists():continue
    a=np.array(Image.open(old).convert('RGBA'));c=np.array(Image.open(new).convert('RGBA'));r={'sameDimensions':a.shape==c.shape}
    if a.shape==c.shape:
        diff=np.any(a!=c,axis=2);r['changedPixels']=int(diff.sum());r['maxChannelDelta']=int(np.abs(a.astype(int)-c.astype(int)).max())
    if a.shape==c.shape and r["changedPixels"]:
        ys,xs=np.where(diff);r["changedBoundsInclusive"]=[int(xs.min()),int(ys.min()),int(xs.max()),int(ys.max())]
    if a.shape==c.shape:
        info=json.loads(new.with_suffix('.json').read_text());safe=info['safeArea'];scale=min(safe['width']/941,safe['height']/1672)
        top=info['height']-(safe['y']+safe['height']);extra=max(0,safe['height']/scale-1672)
        y1=max(0,int(top+845*scale)-2);y2=min(info['height'],int(top+(1469+extra)*scale)+3)
        outside=diff.copy();outside[y1:y2,:]=False
        r['collectionBandExcludedY']=[y1,y2];r['outsideCollectionChangedPixels']=int(outside.sum())
        delta=np.abs(a.astype(int)-c.astype(int));r['outsideCollectionMaxDelta']=int(delta[outside].max()) if outside.any() else 0
    results[old.name]=r
(b/'QA/BaselineComparison.json').write_text(json.dumps(results,indent=2)+'\n');print(json.dumps(results,indent=2))
