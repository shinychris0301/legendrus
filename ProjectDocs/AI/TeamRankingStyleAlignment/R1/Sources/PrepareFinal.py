from pathlib import Path
from PIL import Image,ImageDraw
import json,numpy as np,cv2,hashlib
b=Path(__file__).resolve().parent;qa=b.parent/'QA';out=b/'Final';out.mkdir(exist_ok=True);records=json.loads((b/'Assessment.json').read_text())['assets'];audit=[]
def polygon(size,points):
 m=Image.new('L',(size[0]*3,size[1]*3));ImageDraw.Draw(m).polygon([(x*3,y*3)for x,y in points],fill=255);return np.array(m.resize(size,Image.Resampling.LANCZOS))
for r in records:
 n=r['name'];original=Image.open(b/'Crops'/(n+'.png')).convert('RGB');rgb=np.array(Image.open(b/'Inputs'/(n+'_native.png')).convert('RGB'));w,h=original.size;mask=np.array(Image.open(b/'RepairMasks'/(n+'.png')));alpha=Image.open(b/'Scenario'/n/'booster_medallion_trial01.png').getchannel('A').resize((w,h),Image.Resampling.LANCZOS);a=np.array(alpha);notes=[]
 if n=='title_ru':
  allowed=polygon((w,h),[(0,53),(260,53),(285,29),(313,21),(344,0),(377,15),(397,28),(445,52),(708,52),(708,153),(0,153)]);face=polygon((w,h),[(65,67),(657,67),(657,139),(67,139)]);a=np.minimum(np.maximum(a,face),allowed);a[146:,:320]=0;a[146:,376:]=0;notes.append('Remove header/progress fragments and restore opaque title face inside source frame')
 elif n=='details':
  face=polygon((w,h),[(57,42),(536,42),(536,107),(57,107)]);a=np.maximum(a,face);notes.append('Keep opaque emerald button face after removing the baked caption')
 elif n.startswith('podium_'):
  if n=='podium_gold':a[:54,:117]=0;a[:54,207:]=0
  binary=(a>70).astype('uint8');cs,_=cv2.findContours(binary,cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);filled=np.zeros((h,w),'uint8');cv2.drawContours(filled,[max(cs,key=cv2.contourArea)],-1,255,-1);a=np.maximum(a,filled)
  notes.append('Retain intentionally opaque podium interior and source emblem; remove neighboring tabs from gold top')
 elif n.startswith('crest_'):
  stop={'crest_wolf':91,'crest_knot':80,'crest_stag':83,'crest_eagle':92,'crest_iron':88,'crest_sun':83,'crest_flame':80}[n];a[stop:]=0;cs,_=cv2.findContours((a>40).astype('uint8'),cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);keep=np.zeros((h,w),'uint8');cv2.drawContours(keep,[max(cs,key=cv2.contourArea)],-1,255,-1);a=np.minimum(a,cv2.dilate(keep,np.ones((3,3),'uint8')));notes.append('Remove next-row crest/rail fragments beneath source shield')
 else:
  cs,_=cv2.findContours((a>60).astype('uint8'),cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);keep=np.zeros((h,w),'uint8')
  for c in cs:
   if cv2.contourArea(c)>4:cv2.drawContours(keep,[c],-1,255,-1)
  a=np.minimum(a,cv2.dilate(keep,np.ones((3,3),'uint8')))
 assert np.array_equal(rgb[mask==0],np.array(original)[mask==0])
 im=Image.fromarray(rgb).convert('RGBA');im.putalpha(Image.fromarray(a));im.resize((w*3,h*3),Image.Resampling.LANCZOS).save(out/(n+'.png'))
 audit.append({'name':n,'sourceBox':r['box'],'originalRGBOutsideRepairMaskIdentical':True,'repairedPixels':int(np.count_nonzero(mask)),'notes':notes,'sha256':hashlib.sha256((out/(n+'.png')).read_bytes()).hexdigest()})
 if n=='title_ru':
  blank=rgb.copy();repair=np.zeros((h,w),'uint8');repair[69:137,74:661]=255;donor=np.array(Image.open(b/'OriginalReference.png').convert('RGB').crop((300,848,378,862)));tile=np.tile(donor,(h//donor.shape[0]+1,w//donor.shape[1]+1,1))[:h,:w];blank[repair>0]=tile[repair>0];im=Image.fromarray(blank).convert('RGBA');im.putalpha(Image.fromarray(a));im.resize((w*3,h*3),Image.Resampling.LANCZOS).save(out/'title_blank.png');Image.fromarray(repair).save(b/'RepairMasks/title_blank.png');audit.append({'name':'title_blank','operation':'Source label face replaced by source lacquer for live English heading','sha256':hashlib.sha256((out/'title_blank.png').read_bytes()).hexdigest()})
for label,color in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(220,0,150))]:
 s=Image.new('RGB',(1100,((len(audit)+3)//4)*300),color);d=ImageDraw.Draw(s)
 for i,p in enumerate(sorted(out.glob('*.png'))):
  im=Image.open(p);im.thumbnail((255,264));x=i%4*275+(275-im.width)//2;y=i//4*300;s.paste(im,(x,y),im);d.text((i%4*275+5,y+276),p.stem,fill='white' if label!='White' else 'black')
 s.save(qa/('AssetsOn'+label+'.png'))
(b.parent/'AssetProvenance.json').write_text(json.dumps(audit,indent=2));print('Prepared',len(audit),'assets')
