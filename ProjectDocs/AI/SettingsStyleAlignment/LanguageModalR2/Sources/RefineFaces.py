from pathlib import Path
from PIL import Image
import cv2,numpy as np,json
s=Path(__file__).resolve().parent;im=Image.open(s/'OriginalReference.png').convert('RGB')
for r in json.loads((s/'Assessment.json').read_text())['assets']:
 n=r['name']
 if n not in ['language_selected','language_unselected']:continue
 a=np.array(im.crop(r['bounds']));mask=np.zeros(a.shape[:2],'uint8');mask[21:82,101:351]=255
 rgb=cv2.inpaint(a,mask,5,cv2.INPAINT_TELEA);Image.fromarray(rgb).save(s/'Inputs'/(n+'_native.png'));Image.fromarray(mask).save(s/'RepairMasks'/(n+'.png'))
print('Native face cleanup refined; submitted inputs unchanged')
