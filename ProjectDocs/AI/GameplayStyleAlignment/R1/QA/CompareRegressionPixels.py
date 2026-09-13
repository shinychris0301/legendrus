"""Read-only comparison of whole, unedited Unity renders; no tolerance-based pass masking."""
from pathlib import Path
from PIL import Image
import numpy as np,json
root=Path(__file__).resolve().parents[5];base=root/'ProjectDocs/AI/PreLevelStyleAlignment/R1/UnityVerification';doc=root/'ProjectDocs/AI/GameplayStyleAlignment/R1';now=doc/'UnityVerification';records=[]
def compare(p,q,category,name):
 a=np.asarray(Image.open(p).convert('RGBA'));b=np.asarray(Image.open(q).convert('RGBA'));assert a.shape==b.shape
 return {'category':category,'name':name,'changedPixels':int(np.count_nonzero(np.any(a!=b,axis=2))),'maxChannelDelta':int(np.max(np.abs(a.astype(int)-b.astype(int)))),'alphaChanges':int(np.count_nonzero(a[:,:,3]!=b[:,:,3]))}
for old,new in [('Home','Home'),('SplashRegression','Splash'),('LoadingRegression','Loading'),('WorldMap','WorldMap'),('ChapterMap','ChapterMap'),('PreLevel','PreLevel')]:
 for p in sorted((base/old).glob('*.png')):
  q=now/new/p.name
  if not q.exists() or p.name.startswith(('LightingBackground_','ActualLoading_')):continue
  records.append(compare(p,q,new,p.name))
for category,a,b in [('Gameplay','Gameplay_941x1672_RU.png','Gameplay_941x1672_RU_roundtrip.png'),('Splash','Splash_941x1672.png','Splash_941x1672_roundtrip.png'),('SplashRecheck','Splash_941x1672.png','Splash_941x1672_roundtrip.png')]:
 if (now/category/a).exists() and (now/category/b).exists():records.append(compare(now/category/a,now/category/b,category,'RU language roundtrip'))
(now/'RegressionPixelComparison.json').write_text(json.dumps(records,indent=2)+'\n')
for cat in sorted(set(r['category'] for r in records)):
 group=[r for r in records if r['category']==cat];print(cat,'exact',sum(r['changedPixels']==0 for r in group),'/',len(group),'max changed pixels',max(r['changedPixels'] for r in group),'max delta',max(r['maxChannelDelta'] for r in group))
