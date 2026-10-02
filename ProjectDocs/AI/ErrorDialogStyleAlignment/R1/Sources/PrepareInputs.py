from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter,ImageOps
import numpy as np,cv2,json,hashlib
b=Path('ProjectDocs/AI/ErrorDialogStyleAlignment/R1');s=b/'Sources';src=Image.open(s/'OriginalReference.png').convert('RGB');regions={'panel':(124,407,820,1266),'emblem':(342,590,602,823),'retry':(225,922,718,1028),'close':(259,1042,682,1133),'support':(276,1142,671,1212)};records=[]
def tile(box,size):
 t=src.crop(box);w,h=t.size;tt=Image.new('RGB',(2*w,2*h));tt.paste(t,(0,0));tt.paste(ImageOps.mirror(t),(w,0));tt.paste(ImageOps.flip(t),(0,h));tt.paste(ImageOps.flip(ImageOps.mirror(t)),(w,h));o=Image.new('RGB',size)
 for y in range(0,size[1],h*2):
  for x in range(0,size[0],w*2):o.paste(tt,(x,y))
 return o
for n,box in regions.items():
 im=src.crop(box);w,h=im.size;mask=Image.new('L',im.size);draw=ImageDraw.Draw(mask)
 if n=='panel':
  paper=mask.copy();d=ImageDraw.Draw(paper)
  for rect in [(330,586,612,824),(226,821,715,904),(217,916,726,1031),(252,1037,689,1137),(270,1139,680,1215)]:d.rectangle(tuple(v-box[i%2] for i,v in enumerate(rect)),fill=255)
  paper=paper.filter(ImageFilter.MaxFilter(9)).filter(ImageFilter.GaussianBlur(2));im=Image.composite(tile((182,618,310,798),im.size),im,paper)
  head=mask.copy();ImageDraw.Draw(head).rectangle((199-box[0],499-box[1],746-box[0],558-box[1]),fill=255);head=head.filter(ImageFilter.MaxFilter(7)).filter(ImageFilter.GaussianBlur(2));im=Image.composite(tile((247,484,660,501),im.size),im,head);mask=Image.fromarray(np.maximum(np.array(paper),np.array(head)))
 elif n!='emblem':
  rect={'retry':(335,946,609,1001),'close':(335,1065,603,1110),'support':(309,1166,635,1195)}[n];draw.rectangle(tuple(v-box[i%2] for i,v in enumerate(rect)),fill=255);im=Image.fromarray(cv2.inpaint(np.array(im),np.array(mask),7,cv2.INPAINT_TELEA))
 im.save(s/'Inputs'/f'{n}.png');mask.save(s/'RepairMasks'/f'{n}.png');records.append({'name':n,'bounds':box,'inputSha256':hashlib.sha256((s/'Inputs'/f'{n}.png').read_bytes()).hexdigest()})
(s/'Assessment.json').write_text(json.dumps({'source':'/home/k2/Downloads/match3final/ChatGPT Image Aug 25, 2026, 09_01_29 PM (12).png','sourceSha256':hashlib.sha256((s/'OriginalReference.png').read_bytes()).hexdigest(),'assets':records,'decision':'Scenario source alpha isolation; local masked title/face/lettering cleanup. Real current page behind modal shade, no reference background crop.'},indent=2));print('Five extraction inputs prepared.')
