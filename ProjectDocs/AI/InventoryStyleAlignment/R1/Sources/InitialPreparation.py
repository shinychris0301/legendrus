from pathlib import Path
from PIL import Image
import numpy as np,cv2,json,hashlib,shutil
b=Path('ProjectDocs/AI/InventoryStyleAlignment/R1');s=b/'Sources'
for d in ['Crops','Inputs','RepairMasks','Scenario','Final']:(s/d).mkdir(parents=True,exist_ok=True)
(b/'QA').mkdir(exist_ok=True);shutil.copy2('/home/k2/Downloads/match3final/ChatGPT Image Aug 25, 2026, 09_00_51 PM (2).png',s/'OriginalReference.png');shutil.copy2('ProjectDocs/AI/ProfileStyleAlignment/R1/Sources/ScenarioRequests.py',s/'ScenarioRequests.py')
p=s/'ScenarioRequests.py';p.write_text(p.read_text().replace('Profile:','Inventory:'))
source=Image.open(s/'OriginalReference.png').convert('RGB');assets=[('title_ru',(159,124,787,289)),('feather_detail',(12,1093,303,1419)),('use',(321,1402,604,1484)),('details',(611,1402,901,1484)),('paper',(299,1102,928,1504))]
xs=[39,213,386,559,732];ys=[382,559,732,908]
for i in range(20):assets.append(('item_'+str(i),(xs[i%5],ys[i//5],xs[i%5]+173,ys[i//5]+174)))
records=[]
for n,box in assets:
 original=np.array(source.crop(box));rgb=original.copy();h,w=rgb.shape[:2];m=np.zeros((h,w),'uint8')
 if n.startswith('item_'):
  # Only the numeric quantity; retain nearby item and ornamental frame.
  x0=80 if n=='item_7' else 123;m[129:160,x0:154]=255;rgb=cv2.inpaint(rgb,m,3,cv2.INPAINT_TELEA)
 if n in ['use','details']:
  m[24:59,37:w-31]=255;rgb=cv2.inpaint(rgb,m,5,cv2.INPAINT_TELEA)
 if n=='paper':
  # Bounded paper donor over original writing/buttons, preserving outer frame.
  m[29:h-25,25:w-24]=255;patch=np.array(source.crop((311,1143,346,1274)).resize((w-49,h-54),Image.Resampling.LANCZOS));rgb[29:h-25,25:w-24]=patch
 Image.fromarray(original).save(s/'Crops'/(n+'.png'));Image.fromarray(rgb).save(s/'Inputs'/(n+'_native.png'));Image.fromarray(rgb).resize((w*3,h*3),Image.Resampling.LANCZOS).save(s/'Inputs'/(n+'.png'));Image.fromarray(m).save(s/'RepairMasks'/(n+'.png'));records.append({'name':n,'box':box,'sourceSha256':hashlib.sha256((s/'OriginalReference.png').read_bytes()).hexdigest()})
(s/'Assessment.json').write_text(json.dumps({'assets':records,'approach':'Source-derived framed item cards, title and detail illustration; Scenario alpha. Local numeric/lettering clearing, paper donor repair. Reuse approved clean wood/scenery, header and shop footer. No RGB regeneration. Actual four boosters retain existing artwork/functions; twenty reference objects are labelled collectible previews.'},indent=2))
