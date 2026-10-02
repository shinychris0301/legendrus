from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,cv2,json,hashlib,shutil
s=Path(__file__).resolve().parent;b=s.parent;out=s/'Final';audit=[]
for r in json.loads((s/'Assessment.json').read_text())['assets']:
 n=r['name'];orig=np.array(Image.open(s/'Crops'/(n+'.png')).convert('RGB'));rgb=np.array(Image.open(s/'Inputs'/(n+'_native.png')).convert('RGB'));h,w=rgb.shape[:2];m=np.array(Image.open(s/'RepairMasks'/(n+'.png')));assert np.array_equal(orig[m==0],rgb[m==0]);im=Image.open(s/'Scenario'/n/'booster_medallion_trial01.png');im.load();a=np.array(im.getchannel('A').resize((w,h),Image.Resampling.LANCZOS));cs,_=cv2.findContours((a>80).astype('uint8'),cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);keep=np.zeros((h,w),'uint8');cv2.drawContours(keep,[max(cs,key=cv2.contourArea)],-1,255,-1);a=np.minimum(a,cv2.dilate(keep,np.ones((3,3),'uint8')))
 if n.startswith('item_'):a[8:h-9,7:w-8]=255
 elif n=='paper':a[4:h-4,4:w-4]=255
 elif n=='feather_detail':a[9:h-9,9:w-9]=255
 else:a=np.maximum(a,keep)
 if n=='title_ru':
  allowed=Image.new('L',(w,h));ImageDraw.Draw(allowed).polygon([(0,77),(69,27),(259,24),(289,0),(342,0),(370,26),(w-66,32),(w,77),(w-10,h-8),(13,h-8)],fill=255);a=np.minimum(a,np.array(allowed))
 rgba=Image.fromarray(rgb).convert('RGBA');rgba.putalpha(Image.fromarray(a));rgba.resize((w*3,h*3),Image.Resampling.LANCZOS).save(out/(n+'.png'));audit.append({'name':n,'box':r['box'],'sourceRGBOutsideRepairMaskIdentical':True,'repairSource':'ScenarioPaperRepair bounded generated interior' if n=='paper' else 'Local recorded cleanup','repairedPixels':int(np.count_nonzero(m)),'sha256':hashlib.sha256((out/(n+'.png')).read_bytes()).hexdigest()})
 if n=='title_ru':
  mask=np.zeros((h,w),'uint8');mask[65:126,99:528]=255;blank=cv2.inpaint(rgb,mask,5,cv2.INPAINT_TELEA);Image.fromarray(mask).save(s/'RepairMasks/title_blank.png');im=Image.fromarray(blank).convert('RGBA');im.putalpha(Image.fromarray(a));im.resize((w*3,h*3),Image.Resampling.LANCZOS).save(out/'title_blank.png');audit.append({'name':'title_blank','operation':'Bounded original-title lettering cleanup for live English title','sha256':hashlib.sha256((out/'title_blank.png').read_bytes()).hexdigest()})
for label,color in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(220,0,150))]:
 sheet=Image.new('RGB',(1250,((len(audit)+4)//5)*235),color);draw=ImageDraw.Draw(sheet)
 for i,p in enumerate(sorted(out.glob('*.png'))):
  im=Image.open(p);im.thumbnail((230,202));x=i%5*250+(250-im.width)//2;y=i//5*235;sheet.paste(im,(x,y),im);draw.text((i%5*250+4,y+210),p.stem,fill='black' if label=='White' else 'white')
 sheet.save(b/'QA'/('AssetsOn'+label+'.png'))
(b/'AssetProvenance.json').write_text(json.dumps(audit,indent=2));runtime=Path('Assets/LegendsOfRus/Resources/ProductionUI/InventoryAligned');runtime.mkdir(exist_ok=True)
for p in out.glob('*.png'):shutil.copy2(p,runtime/p.name)
print('Prepared',len(audit),'runtime sprites')
