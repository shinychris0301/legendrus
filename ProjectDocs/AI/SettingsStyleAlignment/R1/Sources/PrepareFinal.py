from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,cv2,json,hashlib,shutil
s=Path(__file__).resolve().parent;b=s.parent;out=s/'Final';audit=[];source=Image.open(s/'OriginalReference.png').convert('RGB')
def save(name,rgb,alpha,record):
 im=Image.fromarray(rgb).convert('RGBA');im.putalpha(Image.fromarray(alpha));scale=1 if name=='frame' else 2;im.resize((im.width*scale,im.height*scale),Image.Resampling.LANCZOS).save(out/(name+'.png'));audit.append(dict(record,name=name,sha256=hashlib.sha256((out/(name+'.png')).read_bytes()).hexdigest()))
for r in json.loads((s/'Assessment.json').read_text())['assets']:
 n=r['name'];key=r.get('runtimeName',n);orig=np.array(source.crop(r['bounds']));rgb=np.array(Image.open(s/'Inputs'/(n+'_native.png')).convert('RGB'));h,w=rgb.shape[:2];m=np.array(Image.open(s/'RepairMasks'/(n+'.png')));assert np.array_equal(orig[m==0],rgb[m==0]);assert hashlib.sha256((s/'Inputs'/(n+'.png')).read_bytes()).hexdigest()==r['inputSha256'];im=Image.open(s/'Scenario'/n/'booster_medallion_trial01.png');im.load();a=np.array(im.getchannel('A').resize((w,h),Image.Resampling.LANCZOS));cs,_=cv2.findContours((a>80).astype('uint8'),cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);keep=np.zeros((h,w),'uint8');cv2.drawContours(keep,[max(cs,key=cv2.contourArea)],-1,255,-1);a=np.minimum(a,cv2.dilate(keep,np.ones((3,3),'uint8')))
 if key in ['paper','row','logout']:a[7:h-7,8:w-8]=255
 elif key!='frame':a=np.maximum(a,keep)
 if key=='frame':
  # Remove every content region and original Back. Keep only the carved surrounding frame.
  allowed=Image.new('L',(w,h));d=ImageDraw.Draw(allowed)
  d.polygon([(0,40),(52,0),(110,0),(110,48),(48,70),(48,1189),(282,1189),(346,1220),(359,1260),(350,1314),(284,1281),(49,1281),(0,1232)],fill=255)
  d.polygon([(w,40),(w-52,0),(w-110,0),(w-110,48),(w-43,70),(w-43,1189),(w-282,1189),(w-345,1220),(w-359,1260),(w-350,1314),(w-284,1281),(w-49,1281),(w,1232)],fill=255)
  a=np.minimum(a,np.array(allowed));Image.fromarray(a).save(s/'RepairMasks/frame_alpha.png')
 if key=='title_ru':
  # Largest Scenario contour already excludes detached header ornaments. Preserve full plaque edges.
  a=np.maximum(a,keep)
 if key.startswith('icon_') and key!='icon_logout':
  cy={'music':48,'sfx':48,'vibration':47,'notifications':48,'language':49,'cloud':51,'account':51,'support':49,'privacy':48}[key[5:]]
  allowed=Image.new('L',(w,h));ImageDraw.Draw(allowed).ellipse((0,cy-51,w-1,cy+51),fill=255);a=np.minimum(a,np.array(allowed))
 if key in ['icon_music','icon_sfx']:a[97:]=0
 if key=='icon_vibration':a[96:]=0
 if key=='icon_cloud':a[:8,:36]=0
 record={'source':n,'bounds':r['bounds'],'originalRGBOutsideRepairMaskIdentical':True,'repairPixels':int((m>0).sum()),'process':'Scenario alpha, isolated contour with opaque interiors retained; bounded local repairs recorded'}
 if key=='switch':
  save('switch_on',rgb,a,record)
  hsv=cv2.cvtColor(rgb,cv2.COLOR_RGB2HSV);green=(hsv[:,:,0]>27)&(hsv[:,:,0]<100);gray=cv2.cvtColor(rgb,cv2.COLOR_RGB2GRAY);off=rgb.copy();off[green]=np.stack([gray,gray,gray],axis=-1)[green];save('switch_off',off[:,::-1],a[:,::-1],{'source':'switch','operation':'Mirror knob to left; desaturate green face for off state; source on preserved'});continue
 save(key,rgb,a,record)
 if key=='title_ru':
  mask=np.zeros((h,w),'uint8');mask[80:174,106:611]=255;blank=cv2.inpaint(rgb,mask,7,cv2.INPAINT_TELEA);Image.fromarray(mask).save(s/'RepairMasks/title_blank.png');save('title_blank',blank,a,{'source':n,'operation':'Bounded source-lettering clearing for live English title'})
for label,color in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(220,0,150))]:
 sheet=Image.new('RGB',(1000,((len(audit)+3)//4)*260),color);draw=ImageDraw.Draw(sheet)
 for i,p in enumerate(sorted(out.glob('*.png'))):
  im=Image.open(p);im.thumbnail((230,230));x=i%4*250+(250-im.width)//2;y=i//4*260;sheet.paste(im,(x,y),im);draw.text((i%4*250+4,y+238),p.stem,fill='black' if label=='White' else 'white')
 sheet.save(b/'QA'/('AssetsOn'+label+'.png'))
(b/'AssetProvenance.json').write_text(json.dumps(audit,indent=2));runtime=Path('Assets/LegendsOfRus/Resources/ProductionUI/SettingsAligned');runtime.mkdir(exist_ok=True)
for p in out.glob('*.png'):shutil.copy2(p,runtime/p.name)
print('Prepared',len(audit),'runtime sprites')
