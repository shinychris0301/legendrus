from pathlib import Path
from PIL import Image
import numpy as np,json
root=Path(__file__).resolve().parents[5];r2=root/'ProjectDocs/AI/GameplayStyleAlignment/FramePolishR2';r1=r2.parent/'R1'
a=np.array(Image.open(r1/'UnityVerification/Gameplay/Gameplay_941x1672_RU.png').convert('RGBA'))
b=np.array(Image.open(r2/'UnityVerification/Gameplay/Gameplay_941x1672_RU.png').convert('RGBA'))
allowed=np.zeros(a.shape[:2],bool)
areas={'board':[69,341,878,1309],'stars':[388,20,601,77],'moves':[358,64,622,296],'goals':[619,64,896,198],'score':[174,1513,768,1629]}
for x0,y0,x1,y1 in areas.values():allowed[y0:y1,x0:x1]=True
d=np.max(np.abs(a.astype(int)-b.astype(int)),axis=2);outside=d[~allowed]
c=np.array(Image.open(r2/'UnityVerification/Gameplay/Gameplay_941x1672_RU_roundtrip.png').convert('RGBA'))
roundtrip=np.abs(b.astype(int)-c.astype(int))
result={'comparison':'R1 vs R2 actual 941x1672 RU camera captures','allowedVisualRegions':areas,'changedPixels':int(np.count_nonzero(d)),
 'outsideRegionsChangedPixels':int(np.count_nonzero(outside)),'outsideRegionsMaxChannelDelta':int(outside.max()),
 'r2LanguageRoundtripChangedPixels':int(np.count_nonzero(np.any(roundtrip,axis=2))),'r2LanguageRoundtripMaxChannelDelta':int(roundtrip.max()),
 'notAReferencePixelIdentityClaim':True}
(r2/'QA/VisualScopeComparison.json').write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result))
