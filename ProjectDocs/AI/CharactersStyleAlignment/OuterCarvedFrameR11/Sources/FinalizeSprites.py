from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json,hashlib,cv2
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/CharactersAligned');records=[]
for name in ['outer_upper_left_r11','outer_upper_right_r11','outer_lower_left_r11','outer_lower_right_r11']:
 original=Image.open(b/'Inputs'/f'{name}.png').convert('RGBA');im=original.copy();job='outer_upper_left_isolation_r11' if name=='outer_upper_left_r11' else name
 raw=Image.open(b/'Scenario'/job/'booster_medallion_trial01.png').convert('RGBA').resize(im.size,Image.Resampling.LANCZOS);mask=Image.new('L',im.size);d=ImageDraw.Draw(mask)
 if name=='outer_upper_left_r11':
  mask=Image.open(b/'UpperLeftControlsMask.png');d=ImageDraw.Draw(mask)
  d.polygon([(23,24),(55,30),(62,90),(51,82),(47,56),(32,49)],fill=255)
  d.polygon([(117,22),(155,9),(148,39),(120,61),(108,76)],fill=255)
 elif name=='outer_upper_right_r11':
  d.polygon([(0,0),(48,0),(42,15),(31,26),(38,48),(46,70),(40,91),(26,108),(0,119)],fill=255)
  d.polygon([(0,122),(110,122),(125,135),(134,180),(128,192),(0,192)],fill=255)
 elif name=='outer_lower_left_r11':
  d.polygon([(29,0),(58,0),(58,75),(43,65),(32,53)],fill=255)
  d.polygon([(48,82),(58,73),(58,97),(25,97)],fill=255)
 else:d.polygon([(0,0),(15,0),(14,47),(8,58),(0,59)],fill=255)
 # Slight feather only at the ornament/control separation; retained pixels always use original RGB.
 mask=mask.filter(ImageFilter.GaussianBlur(.45));alpha=np.minimum(np.array(raw.getchannel('A')),255-np.array(mask))
 # Use the actual accepted title alpha instead of a broad polygon at the plaque seam.
 # The occupied region is carved UI backing, not environmental scenery; restore it without changing RGB.
 if 'upper' in name:
  titleAlpha=Image.open(out/'title_r4.png').getchannel('A');placed=Image.new('L',im.size)
  left,top=(0,118) if name=='outer_upper_left_r11' else (734,118)
  placed.paste(titleAlpha,(170-left,102-top))
  seam=Image.new('L',im.size);sd=ImageDraw.Draw(seam)
  if name=='outer_upper_left_r11':sd.polygon([(181,0),(245,0),(245,122),(221,122),(209,107),(190,98),(171,77),(167,52),(177,34),(197,23)],fill=255)
  else:sd.polygon([(0,0),(48,0),(42,15),(31,26),(38,48),(46,70),(40,91),(26,108),(0,119)],fill=255)
  restore=np.array(seam)>0;alpha[restore]=(255-np.array(placed))[restore]
  seam.save(b/(name+'_TitleSeamRestoreMask.png'))
 count,labels,stats,_=cv2.connectedComponentsWithStats(np.uint8(alpha>30),8)
 support=np.zeros_like(alpha)
 for k in range(1,count):
  if stats[k,cv2.CC_STAT_AREA]>=5:support[labels==k]=1
 support=cv2.dilate(support,np.ones((3,3),np.uint8));alpha*=support
 im.putalpha(Image.fromarray(alpha));p=out/(name+'.png');im.save(p);mask.save(b/(name+'_ControlExclusionMask.png'))
 assert np.array_equal(np.array(im)[:,:,:3],np.array(original)[:,:,:3])
 records.append({'asset':str(p),'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'originalRGBPreserved':True,'input':str(b/'Inputs'/f'{name}.png'),'alphaJob':job,'controlExclusionMask':str(b/(name+'_ControlExclusionMask.png')),'method':'Original RGB; Scenario alpha with source-control exclusion, actual-title-alpha seam fit and disconnected-speck cleanup'})
for name,box in [('outer_left_trim_r11',[0,310,8,1408]),('outer_right_trim_r11',[933,310,941,1408])]:
 im=Image.open(b/'Inputs'/f'{name}.png').convert('RGBA');p=out/(name+'.png');im.save(p);records.append({'asset':str(p),'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'originalRGBPreserved':True,'sourceBounds':box,'method':'Clean opaque carved UI trim only, no scenery; exact source pixels'})
(b.parent/'AssetProvenance.json').write_text(json.dumps({'assets':records,'assessment':'Sources/Assessment.json','rejectedAlpha':'outer_upper_left_r11: selected avatar banner rather than dark carving; one corrected-input isolation replaces it','generatedRGB':False,'limitations':'Visible original carving only; hidden carvings behind controls are not invented. Original low-resolution ornament retained.'},indent=2))
for color,name in [((0,0,0),'black'),((255,255,255),'white'),((180,0,160),'magenta')]:
 sheet=Image.new('RGB',(640,330),color)
 positions=[(0,0),(275,0),(0,220),(110,220)]
 for r,pos in zip(records[:4],positions):
  im=Image.open(r['asset']).convert('RGBA');sheet.paste(im,pos,im)
 for i,r in enumerate(records[4:]):
  im=Image.open(r['asset']).convert('RGBA');im=im.resize((8,320));sheet.paste(im,(550+i*35,0),im)
 sheet.save(b.parent/'QA'/f'Alpha_{name}.png')
costs=[{'job':p.parent.name,**json.loads(p.read_text())} for p in (b/'Scenario').glob('*/Billing.json')]
(b.parent/'QA/ScenarioAudit.json').write_text(json.dumps({'jobs':costs,'totalCU':sum(x['cuCost'] for x in costs),'acceptedJobs':len(costs),'rejectedMasks':1,'correctedInputJobs':1,'duplicateSubmissions':0,'rgbGenerations':0},indent=2))
print('Six source sprites prepared with original RGB preserved.')
