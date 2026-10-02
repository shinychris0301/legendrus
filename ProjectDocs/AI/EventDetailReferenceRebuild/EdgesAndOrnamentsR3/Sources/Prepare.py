from pathlib import Path
from PIL import Image
import json,hashlib,shutil
b=Path(__file__).resolve().parent
s=Image.open('ProjectDocs/AI/EventDetailReferenceRebuild/OriginalReferenceR2/Sources/OriginalReference.png').convert('RGBA')
p=b/'BeforeAssetsHashes.json'
if not p.exists():p.write_text(json.dumps({str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in Path('Assets').rglob('*') if p.is_file()},indent=2))
items={'bird':([108,402,818,745],[100,20]),'countdown':([218,681,708,744],[30,420]),'balance':([649,748,893,832],[570,420]),'premium_inset':([455,1284,910,1415],[30,570])}
a=Image.new('RGBA',(1024,1024),(48,48,48,255))
for key,(box,xy) in items.items():
 im=s.crop(box);a.paste(im,tuple(xy));items[key]={'sourceBox':box,'atlasBox':[*xy,xy[0]+im.width,xy[1]+im.height]};im.save(b/(key+'_source.png'))
a.convert('RGB').save(b/'Guidance.png');(b/'SourceBoxes.json').write_text(json.dumps(items,indent=2))
shutil.copyfile('ProjectDocs/AI/EventDetailReferenceRebuild/OriginalReferenceR2/Sources/OriginalReference.png',b/'OriginalReference.png')
for key in ['firebird','firebird_sparks','firebird_halo']:
 shutil.copyfile('Assets/LegendsOfRus/Resources/ProductionUI/EventDetailAligned/'+key+'.png',b/('before_'+key+'.png'))
