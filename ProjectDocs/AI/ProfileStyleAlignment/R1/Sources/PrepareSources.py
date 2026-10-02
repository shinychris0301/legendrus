from pathlib import Path
from PIL import Image
import numpy as np,cv2,json,hashlib
s=Path(__file__).resolve().parent;source=Image.open(s/'OriginalReference.png').convert('RGB');assets=[('title_ru',(260,101,685,231)),('portrait_main',(27,217,409,597)),('edit',(334,521,386,580))]
for i,box in enumerate([(39,725,214,985),(216,727,380,985),(385,728,557,985),(561,728,728,985),(730,728,893,985)]):assets.append(('hero_'+str(i),box))
for i,box in enumerate([(28,1040,169,1260),(175,1040,309,1260),(315,1040,455,1260),(467,1040,613,1260),(620,1040,757,1260),(759,1040,903,1260)]):assets.append(('relic_'+str(i),box))
for n,box in [('sword',(421,408,479,463)),('shield',(422,463,480,518)),('heart',(422,525,480,578)),('star',(663,400,719,456)),('feather',(663,458,719,515)),('boots',(663,523,719,581)),('footer_profile',(286,1540,343,1605)),('footer_trophy',(523,1535,585,1604)),('footer_history',(696,1538,772,1606))]:assets.append((n,box))
records=[]
for n,box in assets:
 orig=np.array(source.crop(box));rgb=orig.copy();h,w=rgb.shape[:2];mask=np.zeros((h,w),'uint8')
 if n.startswith('hero_'):
  # Preserve original portrait, badge face and frame; remove example name and level for live labels.
  x0,y0,x1,y1=box;mask[max(0,924-y0):min(h,957-y0),12:w-9]=255
  mask[max(0,886-y0):min(h,919-y0),max(0,box[0]-x0+1):44]=255
 if n.startswith('relic_'):mask[166:212,13:w-10]=255
 if n=='portrait_main':cv2.circle(mask,(334-27+26,521-217+29),32,255,-1)
 if np.any(mask):rgb=cv2.inpaint(rgb,mask,5,cv2.INPAINT_TELEA)
 Image.fromarray(orig).save(s/'Crops'/(n+'.png'));Image.fromarray(rgb).save(s/'Inputs'/(n+'_native.png'));Image.fromarray(rgb).resize((w*3,h*3),Image.Resampling.LANCZOS).save(s/'Inputs'/(n+'.png'));Image.fromarray(mask).save(s/'RepairMasks'/(n+'.png'));records.append({'name':n,'box':box,'sourceSha256':hashlib.sha256((s/'OriginalReference.png').read_bytes()).hexdigest()})
(s/'Assessment.json').write_text(json.dumps({'assets':records,'approach':'Original reference individual components; alpha via Scenario, bounded live-caption/level clearing. Portrait environments retained only inside intentional framed portrait illustrations.'},indent=2));print(len(records))
