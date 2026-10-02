from pathlib import Path
from PIL import Image
import json,hashlib,shutil
b=Path(__file__).resolve().parent;s=Image.open('ProjectDocs/AI/EventDetailReferenceRebuild/EdgesAndOrnamentsR3/Sources/OriginalReference.png').convert('RGBA')
p=b/'BeforeAssetsHashes.json'
if not p.exists():p.write_text(json.dumps({str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in Path('Assets').rglob('*') if p.is_file()},indent=2))
items={'footer':([0,1498,941,1672],[20,20]),'row':([30,1218,430,1283],[20,230]),'row_player':([30,1417,430,1487],[20,330]),'back_rim':([40,1503,223,1667],[450,230]),'reward_rim':([737,1500,929,1666],[680,230]),'rank0':([33,1219,86,1282],[20,480]),'rank1':([33,1285,86,1348],[110,480]),'rank2':([33,1351,86,1414],[200,480])}
a=Image.new('RGBA',(1024,1024),(48,48,48,255))
for key,(box,xy) in items.items():
 im=s.crop(box);a.paste(im,tuple(xy));items[key]={'sourceBox':box,'atlasBox':[*xy,xy[0]+im.width,xy[1]+im.height]};im.save(b/(key+'_source.png'))
for i,y in enumerate([1219,1285,1351]):s.crop((88,y,156,y+66)).save(b/('portrait'+str(i)+'_source.png'))
s.crop((775,1543,878,1644)).save(b/'chest_source.png')
a.convert('RGB').save(b/'Guidance.png');(b/'SourceBoxes.json').write_text(json.dumps(items,indent=2));s.save(b/'OriginalReference.png')
