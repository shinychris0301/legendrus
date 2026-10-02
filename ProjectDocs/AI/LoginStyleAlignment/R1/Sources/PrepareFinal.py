from pathlib import Path
from PIL import Image,ImageDraw,ImageFont
import numpy as np,cv2,json,hashlib
b=Path('ProjectDocs/AI/LoginStyleAlignment/R1');s=b/'Sources';runtime=Path('Assets/LegendsOfRus/Resources/ProductionUI/LoginAligned');runtime.mkdir(exist_ok=True);records=[]
assessment=json.loads((s/'Assessment.json').read_text());original=Image.open(s/'OriginalReference.png').convert('RGB')
for r in assessment['assets']:
 name=r['name'];p=s/'Scenario'/name/'booster_medallion_trial01.png'
 if not p.exists():raise RuntimeError('Missing completed output '+name)
 assert hashlib.sha256((s/'Inputs'/(name+'.png')).read_bytes()).hexdigest()==r['inputSha256']
 rgb=np.array(Image.open(s/'Inputs'/(name+'_native.png')).convert('RGB'));h,w=rgb.shape[:2];a=np.array(Image.open(p).convert('RGBA').getchannel('A').resize((w,h),Image.Resampling.LANCZOS));repair=np.array(Image.open(s/'RepairMasks'/(name+'.png')))
 if name=='panel':
  donor=Image.open('Assets/LegendsOfRus/Resources/ProductionUI/SettingsAligned/paper.png').convert('RGB');dw,dh=donor.size;donor=donor.crop((dw*.15,dh*.15,dw*.85,dh*.85));rgb[144:767,56:622]=np.array(donor.resize((566,623),Image.Resampling.LANCZOS))
  # Keep the opaque face inside the genuine isolated wood silhouette.
  a[81:806,31:w-31]=255
 elif name=='corner_top':
  keep=np.zeros_like(a);cv2.fillPoly(keep,[np.array([(0,0),(310,0),(260,26),(189,44),(125,62),(87,108),(52,155),(29,218),(0,284)])],255);a=np.minimum(a,keep)
  # The isolated corner also selected adjacent birch/logotype. Retain only ornament bounds, excluding cool-colored scenery.
  hsv=cv2.cvtColor(rgb,cv2.COLOR_RGB2HSV);a[(hsv[:,:,0]>65)&(hsv[:,:,0]<145)]=0
 elif name in ['field','login','guest','link','checkbox','vk','ok','mail','google']:
  contours,_=cv2.findContours((a>128).astype('uint8'),cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE)
  if contours:
   fill=np.zeros_like(a);cv2.drawContours(fill,[max(contours,key=cv2.contourArea)],-1,255,-1);a=np.maximum(a,fill)
 if name=='panel':
  keep=np.zeros_like(a);cv2.fillPoly(keep,[np.array([(0,78),(29,57),(204,56),(224,39),(252,38),(283,20),(309,0),(331,31),(353,40),(388,49),(560,52),(643,52),(680,86),(680,813),(646,852),(388,852),(343,878),(309,878),(266,856),(28,853),(0,813)])],255);a=np.minimum(a,keep)
 if name=='checkbox':
  donor=original.crop((268,1121,332,1170)).resize((36,35),Image.Resampling.LANCZOS);rgb[8:43,8:44]=np.array(donor)
 if name=='subject':
  # Covered by the panel at runtime; do not retain its border/scenery as part of the sprite.
  a[repair>0]=0
 if name=='frame':
  keep=np.zeros_like(a)
  cv2.fillPoly(keep,[np.array([(0,0),(320,0),(202,70),(102,106),(61,294),(0,365)]),np.array([(941,0),(621,0),(739,70),(839,106),(880,294),(941,365)]),np.array([(0,1250),(74,1320),(110,1520),(186,1590),(330,1672),(0,1672)]),np.array([(941,1205),(865,1260),(830,1510),(765,1590),(620,1672),(941,1672)])],255)
  a=np.minimum(a,keep)
 src=np.array(original.crop(r['bounds']));assert np.array_equal(rgb[repair==0],src[repair==0]),name
 out=Image.fromarray(np.dstack([rgb,a]));factor=2 if max(w,h)<1000 else 1;out=out.resize((w*factor,h*factor),Image.Resampling.LANCZOS);out.save(s/'Final'/(name+'.png'));out.save(runtime/(name+'.png'))
 records.append({**r,'runtime':str(runtime/(name+'.png')),'sha256':hashlib.sha256((runtime/(name+'.png')).read_bytes()).hexdigest(),'sourceRgbOutsideRepairMaskVerified':True})
 if name=='panel':
  mask=np.zeros((h,w),np.uint8);mask[80:121,123:554]=255;blank=cv2.inpaint(rgb,mask,6,cv2.INPAINT_TELEA);Image.fromarray(mask).save(s/'RepairMasks/panel_blank.png');out=Image.fromarray(np.dstack([blank,a])).resize((w*factor,h*factor),Image.Resampling.LANCZOS);out.save(s/'Final/panel_blank.png');out.save(runtime/'panel_blank.png');records.append({'name':'panel_blank','derivedFrom':'panel','runtime':str(runtime/'panel_blank.png'),'sha256':hashlib.sha256((runtime/'panel_blank.png').read_bytes()).hexdigest()})
(b/'AssetProvenance.json').write_text(json.dumps(records,indent=2))
for label,color in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(220,0,150))]:
 sheet=Image.new('RGB',(1200,2000),color);draw=ImageDraw.Draw(sheet)
 for i,p in enumerate(sorted((s/'Final').glob('*.png'))):
  im=Image.open(p);im.thumbnail((290,355));x=(i%4)*300+(300-im.width)//2;y=(i//4)*400;sheet.paste(im,(x,y),im);draw.text(((i%4)*300+8,y+368),p.stem,fill=(150,150,150))
 sheet.save(b/'QA'/('AssetsOn'+label+'.png'))
print('Prepared',len(records),'runtime sprites')
