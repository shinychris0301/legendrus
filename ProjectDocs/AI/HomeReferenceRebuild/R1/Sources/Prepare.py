from pathlib import Path
from PIL import Image
import hashlib,json,shutil
b=Path(__file__).resolve().parent;ref=Path('/home/k2/Downloads/match3final/3.home.png');shutil.copy2(ref,b/'OriginalReference.png');im=Image.open(ref).convert('RGB')
boxes={'logo':[203,107,733,486],'hero':[0,466,941,1456],'play':[178,998,762,1240],'map':[52,1214,260,1450],'shop':[263,1214,468,1450],'events':[472,1214,673,1450],'characters':[677,1214,884,1450],'footer':[0,1454,941,1672],'header':[0,0,941,140],'avatar':[0,0,174,235]}
for n,box in boxes.items():im.crop(box).save(b/'Inputs'/(n+'.png'))
(b/'Assessment.json').write_text(json.dumps({'source':str(ref),'sha256':hashlib.sha256(ref.read_bytes()).hexdigest(),'bounds':boxes,'workflow':'Scenario source alpha, original RGB; combined static lettering, dynamic values remain native.'},indent=2))
snapshot={str(p):hashlib.sha256(p.read_bytes()).hexdigest() for d in ['Assets','Packages','ProjectSettings'] for p in Path(d).rglob('*') if p.is_file()};(b.parent/'QA/Before.json').write_text(json.dumps(snapshot,indent=2))
