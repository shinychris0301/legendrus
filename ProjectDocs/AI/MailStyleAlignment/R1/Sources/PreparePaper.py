from pathlib import Path
from PIL import Image
import numpy as np,json,hashlib,cv2
s=Path(__file__).resolve().parent;doc=json.loads((s/'Assessment.json').read_text());donor=np.array(Image.open(s/'Letter.png').convert('RGB').crop((704,956,832,1052)));smooth=cv2.GaussianBlur(donor.astype('float32'),(0,0),12);donor=np.clip(donor.astype('float32')-smooth+np.median(donor.reshape(-1,3),axis=0),0,255).astype('uint8');tile=np.concatenate([donor,donor[:,::-1]],axis=1);tile=np.concatenate([tile,tile[::-1]],axis=0)
for n,ref,box,pad in [('preview_frame','Inbox',(48,1032,894,1476),(27,22,31,25)),('letter_frame','Letter',(10,288,932,1531),(48,37,49,42))]:
 orig=np.array(Image.open(s/(ref+'.png')).convert('RGB').crop(box));h,w=orig.shape[:2];rgb=orig.copy();l,t,r,b=pad;m=np.zeros((h,w),'uint8');m[t:h-b,l:w-r]=255;paper=np.tile(tile,((h+tile.shape[0]-1)//tile.shape[0],(w+tile.shape[1]-1)//tile.shape[1],1))[:h,:w];rgb[m>0]=paper[m>0]
 if n=='letter_frame':
  rgb[:27,320:602]=orig[:27,100:382];m[:27,320:602]=255
  for x0,x1 in [(0,185),(w-190,w)]:rgb[h-42:h,x0:x1]=orig[:42,x0:x1][::-1];m[h-42:h,x0:x1]=255
 Image.fromarray(orig).save(s/'Crops'/(n+'.png'));Image.fromarray(rgb).save(s/'Inputs'/(n+'_native.png'));
 if not (s/'Inputs'/(n+'.png')).exists():Image.fromarray(rgb).resize((w*2,h*2),Image.Resampling.LANCZOS).save(s/'Inputs'/(n+'.png'))
 Image.fromarray(m).save(s/'RepairMasks'/(n+'.png'));doc['assets']=[x for x in doc['assets'] if x['name']!=n];doc['assets'].append({'name':n,'reference':ref,'box':box,'sourceSha256':hashlib.sha256((s/(ref+'.png')).read_bytes()).hexdigest(),'repair':'Lighting-normalized mirrored clean parchment donor from Letter [704,956,832,1052] fills masked writing area; outer frame source pixels unchanged except overlapping title tip removed from the top edge and letter lower corners restored from mirrored upper frame to exclude overlapping Back/envelope buttons. Submitted Scenario inputs remain immutable.'})
(s/'Assessment.json').write_text(json.dumps(doc,indent=2))
