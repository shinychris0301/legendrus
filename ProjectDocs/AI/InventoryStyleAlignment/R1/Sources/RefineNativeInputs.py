from pathlib import Path
from PIL import Image
import numpy as np,cv2,json
s=Path(__file__).resolve().parent;source=Image.open(s/'OriginalReference.png').convert('RGB')
for r in json.loads((s/'Assessment.json').read_text())['assets']:
 n=r['name'];rgb=np.array(Image.open(s/'Crops'/(n+'.png')).convert('RGB'));h,w=rgb.shape[:2];mask=np.zeros((h,w),'uint8')
 if n.startswith('item_'):
  x0=77 if n=='item_7' else 108;roi=np.zeros((h,w),'uint8');roi[128:160,x0:154]=255
  gold=(rgb[:,:,0]>100)&(rgb[:,:,0].astype(int)>rgb[:,:,2].astype(int)*1.5);mask=cv2.dilate((gold.astype('uint8')*roi),np.ones((3,3),'uint8'));mask[160:]=0;mask[:,155:]=0;rgb=cv2.inpaint(rgb,mask,4,cv2.INPAINT_TELEA)
 elif n=='paper':
  roi=np.zeros((h,w),'uint8');roi[30:294,43:w-32]=255;dark=cv2.cvtColor(rgb,cv2.COLOR_RGB2GRAY)<175;mask=cv2.dilate(dark.astype('uint8')*roi,np.ones((3,3),'uint8'));rgb=cv2.inpaint(rgb,mask,5,cv2.INPAINT_TELEA)
  donor=np.array(source.crop((313,1151,341,1360)));tile=np.concatenate([donor,donor[:,::-1]],axis=1);tile=np.concatenate([tile,tile[::-1]],axis=0);patch=np.tile(tile,(2,12,1));rgb[294:h-23,22:w-24]=patch[:h-317,:w-46];mask[294:h-23,22:w-24]=255
 else:continue
 Image.fromarray(rgb).save(s/'Inputs'/(n+'_native.png'));Image.fromarray(mask).save(s/'RepairMasks'/(n+'.png'))
