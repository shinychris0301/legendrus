from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,cv2,json,hashlib,shutil
s=Path(__file__).resolve().parent;b=s.parent;out=s/'Final';audit=[]
for r in json.loads((s/'Assessment.json').read_text())['assets']:
 n=r['name'];original=np.array(Image.open(s/'Crops'/(n+'.png')).convert('RGB'));rgb=np.array(Image.open(s/'Inputs'/(n+'_native.png')).convert('RGB'));h,w=rgb.shape[:2];m=np.array(Image.open(s/'RepairMasks'/(n+'.png')));assert np.array_equal(rgb[m==0],original[m==0]);a=np.array(Image.open(s/'Scenario'/r.get('scenarioName',n)/'booster_medallion_trial01.png').getchannel('A').resize((w,h),Image.Resampling.LANCZOS));cs,_=cv2.findContours((a>65).astype('uint8'),cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);keep=np.zeros((h,w),'uint8');cv2.drawContours(keep,[max(cs,key=cv2.contourArea)],-1,255,-1)
 if n.startswith(('hero_','relic_')) or n=='portrait_main':
  # Preserve intentionally opaque framed-card faces, while Scenario supplies exterior alpha.
  face=np.zeros((h,w),'uint8');face[9:h-27 if n.startswith('hero_') else h-8,7:w-6]=255;a=np.maximum(a,face)
  if n=='portrait_main':a[8:h-8,8:w-8]=255
  if n.startswith('hero_'):
   gold=(rgb[:,:,0]>110)&(rgb[:,:,0].astype(int)>rgb[:,:,2].astype(int)*1.5);a[h-26:]=np.maximum(a[h-26:],(gold[h-26:]*255).astype('uint8'))
 else:a=np.minimum(a,cv2.dilate(keep,np.ones((3,3),'uint8')))
 if n=='hero_4':
  # Recover full source star faces without the fragmented neighboring baseline.
  a[232:]=0
  stars=Image.new('L',(w*3,h*3));draw=ImageDraw.Draw(stars)
  import math
  for cx in [40,60,79,99,119]:
   pts=[((cx+(9 if k%2==0 else 4)*math.cos(-math.pi/2+k*math.pi/5))*3,(243+(9 if k%2==0 else 4)*math.sin(-math.pi/2+k*math.pi/5))*3) for k in range(10)];draw.polygon(pts,fill=255)
  a=np.maximum(a,np.array(stars.resize((w,h),Image.Resampling.LANCZOS)))
 if n=='title_ru':
  allowed=Image.new('L',(w,h));ImageDraw.Draw(allowed).polygon([(0,62),(155,30),(174,0),(249,0),(280,30),(w,62),(w,h-13),(0,h-13)],fill=255);a=np.minimum(np.maximum(a,keep),np.array(allowed));a[h-13:]=0
 if n=='edit':a=np.maximum(a,keep)
 im=Image.fromarray(rgb).convert('RGBA');im.putalpha(Image.fromarray(a));im.resize((w*3,h*3),Image.Resampling.LANCZOS).save(out/(n+'.png'));audit.append({'name':n,'box':r['box'],'sourceRGBOutsideRepairMaskIdentical':True,'repairedPixels':int(np.count_nonzero(m)),'sha256':hashlib.sha256((out/(n+'.png')).read_bytes()).hexdigest()})
 if n=='title_ru':
  m=np.zeros((h,w),'uint8');m[50:95,87:333]=255;rgb=cv2.inpaint(rgb,m,5,cv2.INPAINT_TELEA);Image.fromarray(m).save(s/'RepairMasks/title_blank.png');im=Image.fromarray(rgb).convert('RGBA');im.putalpha(Image.fromarray(a));im.resize((w*3,h*3),Image.Resampling.LANCZOS).save(out/'title_blank.png');audit.append({'name':'title_blank','operation':'Original title lettering cleared locally for live English title','sha256':hashlib.sha256((out/'title_blank.png').read_bytes()).hexdigest()})
for label,color in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(220,0,150))]:
 sheet=Image.new('RGB',(1250,((len(audit)+4)//5)*260),color);d=ImageDraw.Draw(sheet)
 for i,p in enumerate(sorted(out.glob('*.png'))):
  im=Image.open(p);im.thumbnail((230,225));x=i%5*250+(250-im.width)//2;y=i//5*260;sheet.paste(im,(x,y),im);d.text((i%5*250+4,y+238),p.stem,fill='black' if label=='White' else 'white')
 sheet.save(b/'QA'/('AssetsOn'+label+'.png'))
(b/'AssetProvenance.json').write_text(json.dumps(audit,indent=2));runtime=Path('Assets/LegendsOfRus/Resources/ProductionUI/ProfileAligned');runtime.mkdir(exist_ok=True)
for p in out.glob('*.png'):shutil.copy2(p,runtime/p.name)
print('Prepared',len(audit),'runtime sprites')
