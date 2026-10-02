from pathlib import Path
from PIL import Image
import numpy as np,json,cv2
s=Path(__file__).resolve().parent;im=Image.open(s/'roster.png').convert('RGB');a=json.loads((s/'Assessment.json').read_text())
for i,box in enumerate([(10,1498,201,1666),(201,1498,377,1666),(378,1484,564,1666),(564,1498,750,1666),(752,1497,926,1666)]):
 n='nav_'+str(i);crop=im.crop(box);crop.save(s/'Crops'/(n+'.png'));rgb=np.array(crop);m=np.zeros(rgb.shape[:2],'uint8');x1,x2=box[0]+20,box[2]-19;y1,y2=1621,1647;m[y1-box[1]:y2-box[1],20:rgb.shape[1]-19]=255;rgb=cv2.inpaint(rgb,m,5,cv2.INPAINT_TELEA);p=Image.fromarray(rgb);p.save(s/'Inputs'/(n+'_native.png'));p.resize((p.width*3,p.height*3),Image.Resampling.LANCZOS).save(s/'Inputs'/(n+'.png'));Image.fromarray(m).save(s/'RepairMasks'/(n+'.png'));a['assets']=[r for r in a['assets'] if r['name']!=n];a['assets'].append({'name':n,'reference':'roster','box':box,'repairs':[(x1,y1,x2,y2)],'note':'Complete footer button with local caption removal; source Events badge is illustrative only'})
(s/'Assessment.json').write_text(json.dumps(a,indent=2))
