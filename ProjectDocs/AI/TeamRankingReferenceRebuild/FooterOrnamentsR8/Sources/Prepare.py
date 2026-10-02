from pathlib import Path
from PIL import Image,ImageDraw
import json,hashlib
b=Path(__file__).resolve().parent;ref=Image.open(b/'OriginalReference.png').convert('RGB')
items=[('banner_left',(0,1444,218,1672),[(13,1459),(66,1459),(72,1447),(90,1447),(100,1467),(158,1465),(158,1490),(157,1570),(180,1570),(197,1585),(216,1640),(208,1661),(22,1660),(12,1625)]),('banner_right',(728,1444,941,1672),[(786,1463),(832,1463),(837,1447),(851,1447),(856,1463),(913,1463),(920,1500),(925,1645),(900,1664),(738,1664),(730,1630),(735,1580),(763,1568),(777,1530)]),('footer_carving',(198,1597,734,1672),[(198,1597),(267,1597),(296,1613),(440,1613),(458,1607),(472,1597),(489,1613),(625,1613),(664,1597),(734,1597),(734,1672),(198,1672)])]
records=[]
for name,box,points in items:
 im=ref.crop(box);mask=Image.new('L',im.size);ImageDraw.Draw(mask).polygon([(x-box[0],y-box[1])for x,y in points],fill=255)
 # Staging guard removes neighboring button/table fragments before isolation.
 staging=Image.composite(im,Image.new('RGB',im.size,(5,17,26)),mask).resize((im.width*3,im.height*3),Image.Resampling.LANCZOS)
 p=b/'Inputs'/(name+'.png');staging.save(p);mask.save(b/'Inputs'/(name+'_support.png'))
 records.append(dict(name=name,sourceBox=box,support=points,inputSHA256=hashlib.sha256(p.read_bytes()).hexdigest()))
(b/'Assessment.json').write_text(json.dumps(records,indent=2)+'\n')
