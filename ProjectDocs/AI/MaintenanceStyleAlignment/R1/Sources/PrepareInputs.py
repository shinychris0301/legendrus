from pathlib import Path
from PIL import Image,ImageDraw,ImageOps,ImageFilter
import numpy as np,cv2,json,hashlib
b=Path('ProjectDocs/AI/MaintenanceStyleAlignment/R1');s=b/'Sources';src=Image.open(s/'OriginalReference.png').convert('RGB');regions={'panel':(60,311,880,1523),'timer':(166,854,774,1046),'chest':(438,1046,788,1296),'button':(206,1302,738,1436)};records=[]
def tiled(box,size):
 t=src.crop(box);w,h=t.size;tile=Image.new('RGB',(w*2,h*2));tile.paste(t,(0,0));tile.paste(ImageOps.mirror(t),(w,0));tile.paste(ImageOps.flip(t),(0,h));tile.paste(ImageOps.flip(ImageOps.mirror(t)),(w,h));out=Image.new('RGB',size)
 for y in range(0,size[1],h*2):
  for x in range(0,size[0],w*2):out.paste(tile,(x,y))
 return out
for n,box in regions.items():
 im=src.crop(box);w,h=im.size;mask=Image.new('L',im.size);d=ImageDraw.Draw(mask)
 if n=='panel':
  # Keep perimeter, red header and original dividers; clear occupied paper with source parchment.
  for rect in [(168,652,780,805),(153,844,788,1049),(153,1054,790,1298),(195,1300,747,1440)]:d.rectangle(tuple(v-box[i%2] for i,v in enumerate(rect)),fill=255)
  mask=mask.filter(ImageFilter.MaxFilter(13)).filter(ImageFilter.GaussianBlur(3));im=Image.composite(tiled((122,664,168,809),im.size),im,mask)
  red=Image.new('L',im.size);ImageDraw.Draw(red).rectangle((190-box[0],410-box[1],754-box[0],569-box[1]),fill=255);red=red.filter(ImageFilter.MaxFilter(9)).filter(ImageFilter.GaussianBlur(3));im=Image.composite(tiled((162,491,199,570),im.size),im,red);mask=Image.fromarray(np.maximum(np.array(mask),np.array(red)))
 elif n in ['timer','button']:
  rects=[(235,877,709,915),(240,938,710,1015)] if n=='timer' else [(318,1339,626,1401)]
  for rect in rects:d.rectangle(tuple(v-box[i%2] for i,v in enumerate(rect)),fill=255)
  im=Image.fromarray(cv2.inpaint(np.array(im),np.array(mask),7,cv2.INPAINT_TELEA))
 im.save(s/'Inputs'/f'{n}.png');mask.save(s/'RepairMasks'/f'{n}.png');records.append({'name':n,'bounds':box,'inputSha256':hashlib.sha256((s/'Inputs'/f'{n}.png').read_bytes()).hexdigest()})
(s/'Assessment.json').write_text(json.dumps({'source':'/home/k2/Downloads/match3final/ChatGPT Image Aug 25, 2026, 09_01_26 PM (5).png','sourceSha256':hashlib.sha256((s/'OriginalReference.png').read_bytes()).hexdigest(),'assets':records,'decision':'Four source alpha jobs; repair occupied faces using source textures. Reuse approved logo and clean environment.'},indent=2));print('Four inputs prepared.')
