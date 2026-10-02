from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/CharactersAligned');records=[]
for name,ellipse in [('nav_map_r8',(18,17,162,155)),('nav_shop_r8',(15,19,155,151)),('nav_events_r8',(18,27,161,162)),('nav_team_r8',(24,18,170,156)),('nav_caption_r8',None)]:
 original=Image.open(b/'Inputs'/f'{name}.png').convert('RGBA');im=original.copy();w,h=im.size
 raw=Image.open(b/'Scenario'/name/'booster_medallion_trial01.png').convert('RGBA').resize(im.size,Image.Resampling.LANCZOS);alpha=np.array(raw.getchannel('A'))
 count,labels,stats,_=cv2.connectedComponentsWithStats(np.uint8(alpha>25),8);keep=1+np.argmax(stats[1:,cv2.CC_STAT_AREA]);support=cv2.dilate(np.uint8(labels==keep),np.ones((3,3),np.uint8));alpha*=support
 face=Image.new('L',im.size);d=ImageDraw.Draw(face)
 if ellipse:d.ellipse(ellipse,fill=255)
 else:d.rectangle((20,8,151,35),fill=255)
 alpha=np.maximum(alpha,np.array(face));im.putalpha(Image.fromarray(alpha));repair=Image.new('L',im.size)
 if name=='nav_events_r8':
  ImageDraw.Draw(repair).polygon([(136,0),(184,0),(184,47),(167,50),(145,45),(133,33),(130,18)],fill=255)
  # Mirror only the obscured upper-right rim from its unoccluded left counterpart.
  im=Image.composite(im.transpose(Image.Transpose.FLIP_LEFT_RIGHT),im,repair)
 elif name=='nav_caption_r8':
  ImageDraw.Draw(repair).rectangle((22,10,150,32),fill=255);repair=repair.filter(ImageFilter.GaussianBlur(.65))
  patch=im.copy();donor=original.crop((24,32,149,35)).resize((129,23),Image.Resampling.BILINEAR);donor.putalpha(255);patch.paste(donor,(22,10));im=Image.composite(patch,im,repair)
 p=out/(name+'.png');im.save(p);repair.save(b/(name+'_RepairMask.png'));face.save(b/(name+'_OpaqueFaceMask.png'))
 unchanged=np.array(repair)==0;equal=bool(np.array_equal(np.array(im)[:,:,:3][unchanged],np.array(original)[:,:,:3][unchanged]));assert equal
 records.append({'asset':str(p),'input':str(b/'Inputs'/f'{name}.png'),'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'sourceRGBOutsideRepairMaskPreserved':equal,'repairMask':str(b/(name+'_RepairMask.png')),'opaqueFaceMask':str(b/(name+'_OpaqueFaceMask.png')),'operation':'Scenario alpha + connected-component cleanup, original RGB and opaque interior; bounded event badge or live-caption cleanup where recorded'})
raw=Image.open(b/'Scenario/backgrounds/booster_medallion_trial01.png').convert('RGB');a=np.array(raw);ys=np.where(np.mean(a,axis=(1,2))<150)[0];box=(0,int(ys[0]),raw.width,int(ys[-1]+1));im=raw.crop(box);p=out/'footer_backing_r8.png';im.save(p);records.append({'asset':str(p),'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'operation':'Scenario generated hidden backing; white exterior margin removed only','outputCrop':box,'originalOutputSize':raw.size})
(b.parent/'AssetProvenance.json').write_text(json.dumps({'assets':records,'assessment':'Sources/Assessment.json','limitations':'Backing hidden by original icons is reconstructed, not recovered. Source event badge removed; existing actual navigation/Back/localization retained.'},indent=2))
for color,name in [((0,0,0),'black'),((255,255,255),'white'),((180,0,160),'magenta')]:
 sheet=Image.new('RGB',(980,250),color)
 for i,r in enumerate(records[:5]):
  im=Image.open(r['asset']).convert('RGBA');sheet.paste(im,(i*194+1,10),im)
 sheet.save(b.parent/'QA'/f'Alpha_{name}.png')
costs=[]
for f in (b/'Scenario').glob('*/Billing.json'):costs.append({'job':f.parent.name,**json.loads(f.read_text())})
(b.parent/'QA/ScenarioAudit.json').write_text(json.dumps({'jobs':costs,'acceptedJobs':len(list((b/'Scenario').glob('*/Job.json'))),'automaticResubmissions':0},indent=2))
print('Prepared',len(records),'sprites; source RGB outside repair masks verified; backing crop',box)
