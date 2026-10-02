from pathlib import Path
from PIL import Image
import numpy as np,json
root=Path(__file__).resolve().parents[5];r2=root/'ProjectDocs/AI/GameplayStyleAlignment/HudCompletionR3';r1=r2.parent/'FramePolishR2'
a=np.array(Image.open(r1/'UnityVerification/Gameplay/Gameplay_941x1672_RU.png').convert('RGBA'))
b=np.array(Image.open(r2/'UnityVerification/Gameplay/Gameplay_941x1672_RU.png').convert('RGBA'))
allowed=np.zeros(a.shape[:2],bool)
areas={'headerAndStatus':[0,0,941,345],'footer':[0,1308,941,1672]}
for x0,y0,x1,y1 in areas.values():allowed[y0:y1,x0:x1]=True
d=np.max(np.abs(a.astype(int)-b.astype(int)),axis=2);outside=d[~allowed]
c=np.array(Image.open(r2/'UnityVerification/Gameplay/Gameplay_941x1672_RU_roundtrip.png').convert('RGBA'))
roundtrip=np.abs(b.astype(int)-c.astype(int))
result={'comparison':'R2 vs R3 actual 941x1672 RU camera captures','allowedVisualRegions':areas,'changedPixels':int(np.count_nonzero(d)),
 'outsideRegionsChangedPixels':int(np.count_nonzero(outside)),'outsideRegionsMaxChannelDelta':int(outside.max()),
 'r3LanguageRoundtripChangedPixels':int(np.count_nonzero(np.any(roundtrip,axis=2))),'r3LanguageRoundtripMaxChannelDelta':int(roundtrip.max()),
 'notAReferencePixelIdentityClaim':True}
(r2/'QA/VisualScopeComparison.json').write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result))
