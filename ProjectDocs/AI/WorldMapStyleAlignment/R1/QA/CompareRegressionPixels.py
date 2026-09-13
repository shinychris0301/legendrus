"""Read-only comparison of complete, unedited Unity captures."""
from PIL import Image
from pathlib import Path
import numpy as np,json
base=Path('ProjectDocs/AI/HomeStyleAlignment/SelectiveLightingR11/UnityVerification'); now=Path('/tmp/lor-worldmap-r1/finalplay4');out=[]
for category in ['Home','SplashRegression','LoadingRegression']:
 for p in sorted((base/category).glob('*.png')):
  if p.name.startswith('LightingBackground_') or p.name.startswith('ActualLoading_'):continue
  q=now/category/p.name
  a=np.asarray(Image.open(p).convert('RGBA'));b=np.asarray(Image.open(q).convert('RGBA'))
  out.append({'category':category,'name':p.name,'changedPixels':int(np.count_nonzero(np.any(a!=b,axis=2))),'maxChannelDelta':int(np.max(np.abs(a.astype(int)-b.astype(int))))})
a=np.asarray(Image.open(now/'WorldMap/WorldMap_941x1672_RU.png'));b=np.asarray(Image.open(now/'WorldMap/WorldMap_941x1672_RU_roundtrip.png'))
out.append({'category':'WorldMap','name':'RU language roundtrip','changedPixels':int(np.count_nonzero(np.any(a!=b,axis=2))),'maxChannelDelta':int(np.max(np.abs(a.astype(int)-b.astype(int)))),'alphaChanges':int(np.count_nonzero(a[:,:,3]!=b[:,:,3]))})
Path('ProjectDocs/AI/WorldMapStyleAlignment/R1/UnityVerification/RegressionPixelComparison.json').write_text(json.dumps(out,indent=2)+'\n')
print(json.dumps(out,indent=2))
