from pathlib import Path
from PIL import Image,ImageDraw,ImageOps
import numpy as np,cv2,json,hashlib,shutil
b=Path('ProjectDocs/AI/OfflineStyleAlignment/ReferenceAlignmentR2');s=b/'Sources';source=Path('/home/k2/Downloads/match3final/ChatGPT Image Aug 25, 2026, 09_01_26 PM (3).png');shutil.copyfile(source,s/'OriginalReference.png');original=Image.open(source).convert('RGB')
regions={'panel':(120,185,820,1535),'emblem':(233,475,710,900),'retry':(239,1054,706,1196),'home':(267,1193,675,1300)}
records=[]
for name,bounds in regions.items():
 im=original.crop(bounds);w,h=im.size;rgb=np.array(im);mask=Image.new('L',(w,h));d=ImageDraw.Draw(mask)
 if name=='panel':
  for rect in [(194,350,751,431),(222,474,715,892),(225,911,725,1030),(238,1054,708,1197),(266,1192,676,1301),(241,1321,702,1414)]:d.rectangle(tuple(v-bounds[i%2] for i,v in enumerate(rect)),fill=255)
  tile=original.crop((175,568,222,834));tw,th=tile.size;t=Image.new('RGB',(tw*2,th*2));t.paste(tile,(0,0));t.paste(ImageOps.mirror(tile),(tw,0));t.paste(ImageOps.flip(tile),(0,th));t.paste(ImageOps.flip(ImageOps.mirror(tile)),(tw,th));patch=Image.new('RGB',(w,h))
  for y in range(0,h,th*2):
   for x in range(0,w,tw*2):patch.paste(t,(x,y))
  m=np.array(mask);rgb[m>0]=np.array(patch)[m>0]
 elif name in ('retry','home'):
  rect=(94,50,372,98) if name=='retry' else (61,37,344,83)
  d.rectangle(rect,fill=255);rgb=cv2.inpaint(rgb,np.array(mask),7,cv2.INPAINT_TELEA)
 Image.fromarray(rgb).save(s/'Inputs'/f'{name}.png');mask.save(s/'RepairMasks'/f'{name}.png');raw=(s/'Inputs'/f'{name}.png').read_bytes();records.append({'name':name,'bounds':bounds,'inputSha256':hashlib.sha256(raw).hexdigest()})
(s/'Assessment.json').write_text(json.dumps({'source':str(source),'sha256':hashlib.sha256(source.read_bytes()).hexdigest(),'assets':records,'decision':'Source extraction for panel, emblem and buttons; bounded navy-face and caption repair. Existing approved header and clean scene reused, no RGB generation.'},indent=2))
print('Prepared four extraction inputs.')
