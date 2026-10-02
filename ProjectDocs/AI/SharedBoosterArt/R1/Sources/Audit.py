from pathlib import Path
from PIL import Image
import json,numpy as np,cv2,hashlib,xml.etree.ElementTree as ET
b=Path(__file__).resolve().parent.parent;q=b/'QA';s=Image.open(b/'Sources/OriginalReference.png').convert('RGBA');names=['mace','firebird','shield','potion','scroll','lightning','bomb','swap'];rs=json.loads((b/'Sources/SubjectBoxes.json').read_text());checks={}
for i,n in enumerate(names):
 src=np.asarray(s.crop(rs[i]['source']));im=np.asarray(Image.open('Assets/LegendsOfRus/Resources/ProductionUI/SharedBoosters/'+n+'.png'));matches=cv2.matchTemplate(src[:,:,:3].copy(),im[:,:,:3].copy(),cv2.TM_SQDIFF);_,_,loc,_=cv2.minMaxLoc(matches);x,y=loc;region=src[y:y+im.shape[0],x:x+im.shape[1],:3];visible=im[:,:,3]>0;checks[n]={'visiblePixels':int(visible.sum()),'RGBMismatches':int(np.any(region!=im[:,:,:3],axis=2)[visible].sum()),'sourceCropOffset':[x,y]}
(q/'OriginalSubjectRGB.json').write_text(json.dumps(checks,indent=2))
old=json.loads((b/'Sources/BeforeAssetsHashes.json').read_text());now={str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in Path('Assets').rglob('*') if p.is_file()};result={'modified':[p for p,h in old.items() if p in now and h!=now[p]],'added':sorted(set(now)-set(old)),'removed':sorted(set(old)-set(now)),'verification':{}}
for run in ['Verify01','Verify02','Mail01']:
 for n in ['EditMode','PlayMode']:
  p=Path('/tmp/lor-shared-booster-art-r1')/run/n/'results.xml'
  if p.exists():result['verification'][run+'/'+n]=ET.parse(p).getroot().attrib
(q/'ScopeAndVerification.json').write_text(json.dumps(result,indent=2));print(json.dumps({'modified':result['modified'],'addedCount':len(result['added']),'removed':result['removed'],'sourceVisible':sum(x['visiblePixels'] for x in checks.values()),'sourceMismatch':sum(x['RGBMismatches'] for x in checks.values())},indent=2))
