from pathlib import Path
from PIL import Image
import numpy as np,json
base=Path(__file__).resolve().parent.parent
old=base.parent/'TypographyAndFirebirdR7/UnityCaptures'
results={}
for source in old.glob('EventDetail_*.png'):
    target=base/'UnityCaptures'/source.name
    if not target.exists():continue
    a=np.array(Image.open(source).convert('RGBA'));b=np.array(Image.open(target).convert('RGBA'))
    item={'sameDimensions':a.shape==b.shape}
    if a.shape==b.shape:
        change=np.any(a!=b,axis=2);y,x=np.where(change)
        item.update(changedPixels=int(change.sum()),maxChannelDelta=int(np.abs(a.astype(int)-b.astype(int)).max()))
        if x.size:item['differenceBounds']=[int(x.min()),int(y.min()),int(x.max()),int(y.max())]
    results[source.name]=item
(base/'QA/BaselineComparison.json').write_text(json.dumps(results,indent=2)+'\n')
print(json.dumps(results,indent=2))
