from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,cv2,json,hashlib,shutil
s=Path(__file__).resolve().parent;b=s.parent;out=s/'Final';audit=[]
def poly(w,h,points):
 m=Image.new('L',(w,h));ImageDraw.Draw(m).polygon(points,fill=255);return np.array(m)
for r in json.loads((s/'Assessment.json').read_text())['assets']:
 n=r['name'];original=np.array(Image.open(s/'Crops'/(n+'.png')).convert('RGB'));rgb=np.array(Image.open(s/'Inputs'/(n+'_native.png')).convert('RGB'));h,w=rgb.shape[:2];m=np.array(Image.open(s/'RepairMasks'/(n+'.png')));assert np.array_equal(rgb[m==0],original[m==0]);a=np.array(Image.open(s/'Scenario'/n/'booster_medallion_trial01.png').getchannel('A').resize((w,h),Image.Resampling.LANCZOS))
 if n=='title_ru':
  allowed=poly(w,h,[(0,69),(139,38),(173,0),(265,0),(317,46),(w,70),(w,h),(0,h)]);face=poly(w,h,[(61,77),(390,77),(409,113),(390,164),(64,164),(45,120)]);a=np.minimum(np.maximum(a,face),allowed)
 else:
  if n.startswith('request_'):a[h-5:]=0
  cs,_=cv2.findContours((a>70).astype('uint8'),cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);keep=np.zeros((h,w),'uint8');cv2.drawContours(keep,[max(cs,key=cv2.contourArea)],-1,255,-1);a=np.minimum(a,cv2.dilate(keep,np.ones((3,3),'uint8')))
  if n.startswith(('portrait_','request_','nav_')) or n.endswith('_frame') or n in ['accept','decline']:a=np.maximum(a,keep)
 im=Image.fromarray(rgb).convert('RGBA');im.putalpha(Image.fromarray(a));im.resize((w*3,h*3),Image.Resampling.LANCZOS).save(out/(n+'.png'));audit.append({'name':n,'reference':r['reference'],'box':r['box'],'originalRGBOutsideRepairMaskIdentical':True,'repairedPixels':int(np.count_nonzero(m)),'sha256':hashlib.sha256((out/(n+'.png')).read_bytes()).hexdigest()})
 if n=='title_ru':
  m=np.zeros((h,w),'uint8');m[84:158,75:383]=255;rgb=cv2.inpaint(rgb,m,5,cv2.INPAINT_TELEA);rgb[:,:,0][m>0]=np.minimum(rgb[:,:,0][m>0],rgb[:,:,2][m>0]*1.05).astype('uint8');rgb[:,:,1][m>0]=np.minimum(rgb[:,:,1][m>0],rgb[:,:,2][m>0]*1.15).astype('uint8');Image.fromarray(m).save(s/'RepairMasks/title_blank.png');im=Image.fromarray(rgb).convert('RGBA');im.putalpha(Image.fromarray(a));im.resize((w*3,h*3),Image.Resampling.LANCZOS).save(out/'title_blank.png');audit.append({'name':'title_blank','operation':'Original title face locally cleared for live English label','sha256':hashlib.sha256((out/'title_blank.png').read_bytes()).hexdigest()})
for label,color in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(220,0,150))]:
 sheet=Image.new('RGB',(1250,((len(audit)+4)//5)*260),color);d=ImageDraw.Draw(sheet)
 for i,p in enumerate(sorted(out.glob('*.png'))):
  im=Image.open(p);im.thumbnail((230,225));x=i%5*250+(250-im.width)//2;y=i//5*260;sheet.paste(im,(x,y),im);d.text((i%5*250+4,y+238),p.stem,fill='black' if label=='White' else 'white')
 sheet.save(b/'QA'/('AssetsOn'+label+'.png'))
(b/'AssetProvenance.json').write_text(json.dumps(audit,indent=2));runtime=Path('Assets/LegendsOfRus/Resources/ProductionUI/FriendsListAligned');runtime.mkdir(exist_ok=True)
for p in out.glob('*.png'):shutil.copy2(p,runtime/p.name)
print('Prepared',len(audit),'runtime sprites')
