from pathlib import Path
from PIL import Image,ImageDraw
import json,numpy as np,cv2,hashlib,shutil
s=Path(__file__).resolve().parent;b=s.parent;out=s/'Final';audit=[]
for old in ['gift','help','sticker']:
 p=out/(old+'.png')
 if p.exists():p.unlink()
 p=Path('Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned')/(old+'.png')
 if p.exists():p.unlink()
def poly(w,h,points):
 im=Image.new('L',(w,h));ImageDraw.Draw(im).polygon(points,fill=255);return np.array(im)
for r in json.loads((s/'Assessment.json').read_text())['assets']:
 n=r['name'];original=np.array(Image.open(s/'Crops'/(n+'.png')).convert('RGB'));rgb=np.array(Image.open(s/'Inputs'/(n+'_native.png')).convert('RGB'));h,w=rgb.shape[:2];mask=np.array(Image.open(s/'RepairMasks'/(n+'.png')));assert np.array_equal(rgb[mask==0],original[mask==0]);a=np.array(Image.open(s/'Scenario'/n/'booster_medallion_trial01.png').getchannel('A').resize((w,h),Image.Resampling.LANCZOS));notes=[]
 if n=='title_ru':
  allowed=poly(w,h,[(0,62),(137,45),(200,32),(274,0),(368,0),(438,37),(539,49),(646,64),(646,141),(400,141),(363,167),(280,167),(238,142),(0,139)])
  face=poly(w,h,[(67,60),(576,60),(584,83),(576,125),(66,125),(62,96)])
  a=np.minimum(np.maximum(a,face),allowed);a[124:145,:32]=0;notes.append('Remove neighboring header/chest fragments; restore opaque plaque face')
 else:
  if n=='footer_help':a[148:]=0
  if n=='portrait_alena':a[129:]=0
  if n=='portrait_mikhail':a[121:]=0
  if n=='portrait_nikolai':a[119:]=0
  binary=(a>70).astype('uint8');cs,_=cv2.findContours(binary,cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);c=max(cs,key=cv2.contourArea);keep=np.zeros((h,w),'uint8');cv2.drawContours(keep,[c],-1,255,-1)
  a=np.minimum(a,cv2.dilate(keep,np.ones((3,3),'uint8')))
  if n.startswith('portrait') or n.endswith('_frame') or n=='chest' or n.startswith('footer_'):a=np.maximum(a,keep)
  notes.append('Keep main silhouette; preserve opaque portrait/panel interior')
 im=Image.fromarray(rgb).convert('RGBA');im.putalpha(Image.fromarray(a));im.resize((w*3,h*3),Image.Resampling.LANCZOS).save(out/(n+'.png'))
 audit.append({'name':n,'sourceBox':r['box'],'originalRGBOutsideRepairMaskIdentical':True,'repairedPixels':int(np.count_nonzero(mask)),'notes':notes,'sha256':hashlib.sha256((out/(n+'.png')).read_bytes()).hexdigest()})
 if n=='title_ru':
  m=np.zeros((h,w),'uint8');m[68:124,123:519]=255;blank=cv2.inpaint(rgb,m,5,cv2.INPAINT_TELEA);Image.fromarray(m).save(s/'RepairMasks/title_blank.png');im=Image.fromarray(blank).convert('RGBA');im.putalpha(Image.fromarray(a));im.resize((w*3,h*3),Image.Resampling.LANCZOS).save(out/'title_blank.png');audit.append({'name':'title_blank','operation':'Source plaque with bounded local text removal for live English heading','sha256':hashlib.sha256((out/'title_blank.png').read_bytes()).hexdigest()})
# The composer's own isolated circular smile is reused as its native button graphic.
emoji=Image.open(s/'Crops/composer_frame.png').convert('RGBA').crop((482,12,539,70));em=Image.new('L',emoji.size);ImageDraw.Draw(em).ellipse((2,2,55,56),fill=255);emoji.putalpha(em);emoji.resize((171,174),Image.Resampling.LANCZOS).save(out/'emoji.png');audit.append({'name':'emoji','operation':'Native circular child from Scenario-isolated composer source; original RGB and bounded circle alpha','sha256':hashlib.sha256((out/'emoji.png').read_bytes()).hexdigest()})
for label,color in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(220,0,150))]:
 sheet=Image.new('RGB',(1100,((len(audit)+3)//4)*300),color);d=ImageDraw.Draw(sheet)
 for i,p in enumerate(sorted(out.glob('*.png'))):
  im=Image.open(p);im.thumbnail((255,264));x=i%4*275+(275-im.width)//2;y=i//4*300;sheet.paste(im,(x,y),im);d.text((i%4*275+5,y+276),p.stem,fill='black' if label=='White' else 'white')
 sheet.save(b/'QA'/('AssetsOn'+label+'.png'))
(b/'AssetProvenance.json').write_text(json.dumps(audit,indent=2));runtime=Path('Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned');runtime.mkdir(exist_ok=True)
for p in out.glob('*.png'):shutil.copy2(p,runtime/p.name)
print('Prepared',len(audit),'runtime assets')
