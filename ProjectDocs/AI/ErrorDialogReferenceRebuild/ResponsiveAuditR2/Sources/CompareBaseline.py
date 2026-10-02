from pathlib import Path
from PIL import Image
import numpy as np,json
b=Path(__file__).resolve().parent.parent
results={}
for old in (b.parent/'R1/UnityCaptures').glob('*.png'):
    new=b/'UnityCaptures'/old.name
    if not new.exists():continue
    a=np.array(Image.open(old).convert('RGBA'));c=np.array(Image.open(new).convert('RGBA'));r={'sameDimensions':a.shape==c.shape}
    if a.shape==c.shape:
        diff=np.any(a!=c,axis=2);r['changedPixels']=int(diff.sum());r['maxChannelDelta']=int(np.abs(a.astype(int)-c.astype(int)).max())
    if a.shape==c.shape and r["changedPixels"]:
        ys,xs=np.where(diff);r["changedBoundsInclusive"]=[int(xs.min()),int(ys.min()),int(xs.max()),int(ys.max())]
    if a.shape==c.shape:
        info=json.loads(new.with_suffix('.json').read_text());safe=info['safeArea'];scale=min(safe['width']/941,safe['height']/1672)
        cx=safe['x']+safe['width']/2;cy=info['height']-(safe['y']+safe['height']/2)
        box=[round(cx-300*scale),round(cy-249.5*scale),round(cx+300*scale),round(cy+399.5*scale)]
        x1,y1,x2,y2=box;pa=a[y1:y2,x1:x2];pc=c[y1:y2,x1:x2]
        r['opaqueInteriorBounds']=box;r['opaqueInteriorChangedPixels']=int(np.any(pa!=pc,axis=2).sum());r['opaqueInteriorMaxDelta']=int(np.abs(pa.astype(int)-pc.astype(int)).max())
    results[old.name]=r
(b/'QA/BaselineComparison.json').write_text(json.dumps(results,indent=2)+'\n');print(json.dumps(results,indent=2))
