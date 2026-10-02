from pathlib import Path
from PIL import Image
import numpy as np,json
b=Path(__file__).resolve().parent.parent
results={}
for old in (b.parent/'FooterOrnamentsR8/UnityCaptures').glob('TeamRanking_*.png'):
    new=b/'UnityCaptures'/old.name
    if not new.exists():continue
    a=np.array(Image.open(old).convert('RGBA'));c=np.array(Image.open(new).convert('RGBA'));r={'sameDimensions':a.shape==c.shape}
    if a.shape==c.shape:
        diff=np.any(a!=c,axis=2);r['changedPixels']=int(diff.sum());r['maxChannelDelta']=int(np.abs(a.astype(int)-c.astype(int)).max())
    results[old.name]=r
(b/'QA/BaselineComparison.json').write_text(json.dumps(results,indent=2)+'\n');print(json.dumps(results,indent=2))
