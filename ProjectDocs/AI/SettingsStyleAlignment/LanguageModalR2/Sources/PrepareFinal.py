from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,cv2,json,hashlib,shutil
s=Path(__file__).resolve().parent;b=s.parent;out=s/'Final';source=Image.open(s/'OriginalReference.png').convert('RGB');audit=[]
def write(n,rgb,a,record):
 im=Image.fromarray(rgb).convert('RGBA');im.putalpha(Image.fromarray(a));im.resize((im.width*2,im.height*2),Image.Resampling.LANCZOS).save(out/(n+'.png'));audit.append(dict(record,name=n,sha256=hashlib.sha256((out/(n+'.png')).read_bytes()).hexdigest()))
for r in json.loads((s/'Assessment.json').read_text())['assets']:
 n=r['name'];rgb=np.array(Image.open(s/'Inputs'/(n+'_native.png')).convert('RGB'));orig=np.array(source.crop(r['bounds']));mask=np.array(Image.open(s/'RepairMasks'/(n+'.png')));assert np.array_equal(orig[mask==0],rgb[mask==0]);assert hashlib.sha256((s/'Inputs'/(n+'.png')).read_bytes()).hexdigest()==r['inputSha256'];h,w=rgb.shape[:2];im=Image.open(s/'Scenario'/n/'booster_medallion_trial01.png');im.load();a=np.array(im.getchannel('A').resize((w,h),Image.Resampling.LANCZOS));cs,_=cv2.findContours((a>60).astype('uint8'),cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);keep=np.zeros((h,w),'uint8');cv2.drawContours(keep,[max(cs,key=cv2.contourArea)],-1,255,-1);a=np.maximum(np.minimum(a,cv2.dilate(keep,np.ones((3,3),'uint8'))),keep)
 if n=='language_panel_ru':
  a=np.array(im.getchannel('A').resize((w,h),Image.Resampling.LANCZOS))
  a[66:h-13,15:w-15]=255
  # Remove remnants of the neighboring settings rows above the true crest/shoulders.
  allowed=Image.new('L',(w,h));d=ImageDraw.Draw(allowed);d.polygon([(0,64),(132,48),(165,26),(235,20),(289,0),(333,25),(384,31),(413,56),(w,61),(w,h),(0,h)],fill=255);a=np.minimum(a,np.array(allowed))
 record={'sourceBounds':r['bounds'],'sourceRGBOutsideMaskIdentical':True,'repairedPixels':int((mask>0).sum()),'operation':'Scenario alpha on original prepared RGB; opaque face retained, local contour cleanup'}
 write(n,rgb,a,record)
 if n=='language_panel_ru':
  mask=np.zeros((h,w),'uint8');mask[89:149,89:494]=255;blank=cv2.inpaint(rgb,mask,5,cv2.INPAINT_TELEA);Image.fromarray(mask).save(s/'RepairMasks/language_panel_blank.png');write('language_panel_blank',blank,a,{'source':'language_panel_ru','operation':'Bounded lettering removal for live English heading'})
for label,color in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(220,0,150))]:
 sheet=Image.new('RGB',(1200,700),color);d=ImageDraw.Draw(sheet)
 for i,p in enumerate(sorted(out.glob('*.png'))):
  im=Image.open(p);im.thumbnail((380,290));x=i%3*400+(400-im.width)//2;y=i//3*350;sheet.paste(im,(x,y),im);d.text((i%3*400+8,y+305),p.stem,fill='black' if label=='White' else 'white')
 sheet.save(b/'QA'/('AssetsOn'+label+'.png'))
(b/'AssetProvenance.json').write_text(json.dumps(audit,indent=2));runtime=Path('Assets/LegendsOfRus/Resources/ProductionUI/SettingsAligned')
for p in out.glob('*.png'):shutil.copy2(p,runtime/p.name)
print('Prepared',len(audit),'modal sprites')
